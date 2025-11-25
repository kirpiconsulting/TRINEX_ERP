import json
import requests
import pyodbc
import time
from datetime import datetime
from rapor_calistir import oku_json, al_session_id

def run_table_sync():
    cfg = oku_json()
    
    # 1. Veritabanı Bağlantısı
    cs = cfg["connectionstring"]
    parts = dict(p.split("=",1) for p in cs.split(";") if "=" in p)

    conn = pyodbc.connect(
        f"DRIVER={{ODBC Driver 17 for SQL Server}};"
        f"SERVER={parts['Server']};"
        f"DATABASE={parts['Database']};"
        f"UID={parts['Uid']};"
        f"PWD={parts['Pwd']};"
        f"TrustServerCertificate=yes"
    )
    cur = conn.cursor()

    # 2. Depo Eşleştirme Haritasını Çıkar
    # Bizim 'depolar' tablosundaki ek_alan_1 (DIA Key) ile kod (Bizim Kod) eşleşmesini alıyoruz.
    print("Depo eşleştirmeleri yükleniyor...")
    cur.execute("SELECT ek_alan_1, kod FROM depolar WHERE aktif_mi=1")
    # Örnek yapı: {'38950': 'MERKEZ', '38951': 'SUBE1'}
    depo_map = {str(r[0]): str(r[1]) for r in cur.fetchall()}

    # 3. API'den Veriyi Çek
    sid = al_session_id()
    ws_url = "https://kirpi.ws.dia.com.tr/api/v3/scf/json"
    
    payload = {
        "scf_lot_depobazinda_listele": {
            "session_id": sid,
            "firma_kodu": int(cfg["firma"]),
            "donem_kodu": int(cfg["donem"])
        }
    }
    headers = {"Content-Type": "application/json;charset=UTF-8"}

    try:
        r = requests.post(ws_url, json=payload, headers=headers, timeout=60)
        r.raise_for_status()
        resp_data = r.json()
    except Exception as e:
        print("API Bağlantı Hatası:", e)
        conn.close()
        return

    if str(resp_data.get("code")) != "200":
        print("API Servis Hatası:", resp_data.get("msg"))
        conn.close()
        return

    raw_rows = resp_data.get("result", [])
    
    # 4. Veriyi İşle ve Hazırla (Flattening)
    islenen_veriler = []
    
    for row in raw_rows:
        # Durumu 'A' olmayanları (Pasifleri) baştan eliyoruz
        durum = row.get("durum")
        if durum != "A":
            continue

        # Üst bilgiler
        base_obj = {
            "turu": row.get("turu"),             # Tip
            "numarasi": row.get("numarasi"),     # Kod
            "tarih": row.get("tarih"),           # Tarih
            "key": row.get("_key"),
            "stokkartkodu": row.get("stokkartkodu"),
            "anabirimadi": row.get("anabirimadi")
        }

        # Depo-miktar alanlarını parçala
        for k, v in row.items():
            if not k.startswith("fiili_stok$"):
                continue

            try:
                miktar = float(v)
            except:
                miktar = 0.0

            if miktar == 0:
                continue

            # Key formatı: fiili_stok$DEPO_KEY
            depo_key = k.split("$")[1]
            
            # Bizim 'depolar' tablosundan bu key'e karşılık gelen kodu buluyoruz
            bizim_depo_kodu = depo_map.get(str(depo_key))

            # Eğer bu depo bizde tanımlı değilse, bu satırı atlayabiliriz veya NULL basabiliriz.
            # Şimdilik atlamıyoruz, depo_kod boş kalıyor ama veri geliyor.
            
            # İstenen Composite Key: _key|depo_key|turu
            unique_id = f"{base_obj['key']}|{depo_key}|{base_obj['turu']}"

            satir = {
                "unique_id": unique_id,           # ek_alan_1
                "tip": base_obj["turu"],          # turu -> tip
                "kod": base_obj["numarasi"],      # numarasi -> kod
                "tarih_1": base_obj["tarih"],     # tarih -> tarih_1
                "aktif_mi": 1,                    # Durum A olduğu için fix 1
                "stok_kod": base_obj["stokkartkodu"],
                "stok_birim": base_obj["anabirimadi"],
                "stok_miktar": float(f"{miktar:.2f}"),
                "depo_kod": bizim_depo_kodu       # Eşleşen depo kodu
            }
            islenen_veriler.append(satir)

    # 5. Veritabanı Eşitleme (Sync)
    
    # Mevcut kayıtları çek
    cur.execute("SELECT ek_alan_1 FROM lot_seri_tanimlari")
    db_mevcut_keys = [str(r[0]) for r in cur.fetchall()]
    
    incoming_keys = [d["unique_id"] for d in islenen_veriler]
    suan = datetime.now()

    print(f"Toplam {len(islenen_veriler)} satır lot/seri işlenecek...")

    # Insert veya Update
    for d in islenen_veriler:
        if d["unique_id"] in db_mevcut_keys:
            # UPDATE
            cur.execute("""
                UPDATE lot_seri_tanimlari SET 
                    tip=?, kod=?, tarih_1=?, aktif_mi=?, 
                    stok_kod=?, stok_birim=?, stok_miktar=?, depo_kod=?,
                    guncelleme_tarihi=?, guncelleyen_kullanici='ENTEGRASYON_01'
                WHERE ek_alan_1=?
            """, 
            d["tip"], d["kod"], d["tarih_1"], d["aktif_mi"],
            d["stok_kod"], d["stok_birim"], d["stok_miktar"], d["depo_kod"],
            suan, d["unique_id"])
        else:
            # INSERT
            cur.execute("""
                INSERT INTO lot_seri_tanimlari (
                    ek_alan_1, tip, kod, tarih_1, aktif_mi,
                    stok_kod, stok_birim, stok_miktar, depo_kod,
                    olusturma_tarihi, olusturan_kullanici
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'ENTEGRASYON_01')
            """, 
            d["unique_id"], d["tip"], d["kod"], d["tarih_1"], d["aktif_mi"],
            d["stok_kod"], d["stok_birim"], d["stok_miktar"], d["depo_kod"],
            suan)

    # Delete (Artık gelmeyenleri sil)
    silinecek_sayisi = 0
    for db_key in db_mevcut_keys:
        if db_key not in incoming_keys:
            cur.execute("DELETE FROM lot_seri_tanimlari WHERE ek_alan_1=?", db_key)
            silinecek_sayisi += 1

    # 6. Entegrasyon Tablosunu Güncelle
    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='lot_seri_tanimlari'
    """, datetime.now())

    conn.commit()
    conn.close()
    print(f"Lot/Seri başarıyla güncellendi. (Eklenen/Güncellenen: {len(islenen_veriler)}, Silinen: {silinecek_sayisi})")


if __name__ == "__main__":
    while True:
        try:
            print("\n>>> LOT SYNC BAŞLIYOR...", datetime.now())
            run_table_sync()
        except Exception as exc:
            print("Genel Hata:", exc)
        
        cfg = oku_json()
        # Config'den okuyamazsa varsayılan 60 dk
        dakika = int(cfg.get("periyot_dakika_lot_seri_tanimlari", 60))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)