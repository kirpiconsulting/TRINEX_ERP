import json
import pyodbc
import urllib.request
import time
from datetime import datetime

# --- YARDIMCI FONKSİYONLAR ---

def oku_json():
    """bilgiler.json dosyasını okur."""
    try:
        with open('bilgiler.json', 'r', encoding='utf-8') as f:
            return json.load(f)
    except FileNotFoundError:
        print("Hata: bilgiler.json dosyası bulunamadı.")
        return None

def baglanti_kur():
    """Veritabanı bağlantısını oluşturur."""
    config = oku_json()
    if not config: return None
    
    raw_cs = config.get('connectionstring')
    if not raw_cs:
        print("Hata: Connection string bulunamadı.")
        return None

    # Connection string parçalama
    parts = dict(p.split("=", 1) for p in raw_cs.split(";") if "=" in p)
    
    try:
        conn = pyodbc.connect(
            f"DRIVER={{ODBC Driver 17 for SQL Server}};"
            f"SERVER={parts.get('Server')};"
            f"DATABASE={parts.get('Database')};"
            f"UID={parts.get('Uid')};"
            f"PWD={parts.get('Pwd')};"
            f"TrustServerCertificate=yes"
        )
        return conn
    except Exception as e:
        print(f"Bağlantı hatası: {e}")
        return None

def al_session_id():
    """Veritabanından güncel Session ID'yi çeker."""
    conn = baglanti_kur()
    if not conn: return None
    try:
        cur = conn.cursor()
        cur.execute("SELECT deger_1 FROM entegrasyon_tanimlari WHERE kod='SESSION_ID'")
        row = cur.fetchone()
        return row[0] if row else None
    except Exception as e:
        print(f"Session ID alma hatası: {e}")
        return None
    finally:
        conn.close()

def apiye_gonder(payload, islem_adi="API İsteği"):
    """JSON verisini DIA API'sine gönderir ve cevabı dict olarak döner."""
    ws_adres = "https://bartez.ws.dia.com.tr/api/v3/scf/json"
    
    # JSON verisini byte'a çevir
    json_data = json.dumps(payload, ensure_ascii=False).encode('utf-8')
    
    req = urllib.request.Request(
        ws_adres, 
        data=json_data, 
        headers={"Content-Type": "application/json;charset=UTF-8"}
    )
    
    try:
        with urllib.request.urlopen(req) as response:
            result_str = response.read().decode('utf-8')
            result_json = json.loads(result_str)
            
            # Loglama
            code = result_json.get('code', '')
            msg = result_json.get('msg', '')
            print(f"[{islem_adi}] Sonuç Kodu: {code} | Mesaj: {msg}")
            
            return result_json
    except Exception as e:
        print(f"[{islem_adi}] BAĞLANTI HATASI: {e}")
        return None

def sql_guncelle_tamamlandi(tablo_adi, id_listesi):
    """İşlenen satırların ek_alan_10 değerini 'A' yapar."""
    if not id_listesi: return

    conn = baglanti_kur()
    if not conn: return
    try:
        cursor = conn.cursor()
        # Listeyi virgülle ayrılmış stringe çevir (1,2,3)
        ids_str = ",".join(str(x) for x in id_listesi)
        
        query = f"UPDATE {tablo_adi} SET ek_alan_10 = 'A' WHERE id IN ({ids_str})"
        cursor.execute(query)
        conn.commit()
        print(f"SQL GÜNCELLEME: {tablo_adi} tablosunda {len(id_listesi)} kayıt 'A' olarak işaretlendi.")
    except Exception as e:
        print(f"SQL Güncelleme Hatası: {e}")
    finally:
        conn.close()

# --- İŞ MANTIĞI FONKSİYONLARI ---

def hammaddeleri_isle(session_id, firma_kodu, donem_kodu):
    """Hammadde: Sarf Fişi (Turu: 3) - Tek Fiş"""
    print("\n>>> Hammadde (Sarf) İşlemleri Başlıyor...")
    conn = baglanti_kur()
    if not conn: return

    cursor = conn.cursor()
    query = """
        SELECT
            ham.id,
            (SELECT s.ek_alan_1 FROM stoklar s WHERE s.kod = ham.stok_kod) AS stok_key,
            (SELECT s.ek_alan_2 FROM stoklar s WHERE s.kod = ham.stok_kod) AS birim_key,
            ham.stok_miktar AS miktar,
            (SELECT d.ek_alan_1 FROM depolar d WHERE d.kod = ham.tuketim_deposu_kodu) AS depo_key,
            (SELECT LEFT(l.ek_alan_1, CHARINDEX('|', l.ek_alan_1 + '|') - 1) 
                FROM lot_seri_tanimlari l WHERE l.kod = ham.lot_no) AS lot_key
        FROM sure_kayitlari_kalemleri ham
        WHERE (ham.ek_alan_10 IS NULL OR ham.ek_alan_10 != 'A')
    """
    cursor.execute(query)
    rows = cursor.fetchall()
    conn.close()

    if not rows:
        print("Aktarılacak hammadde bulunamadı.")
        return

    ana_depo_key = int(rows[0].depo_key) if rows[0].depo_key else 0
    kalemler = []
    islenen_id_ler = []

    for i, row in enumerate(rows):
        stok_key = int(row.stok_key) if row.stok_key else 0
        if stok_key == 0: continue 

        miktar = str(row.miktar).replace(',', '.')
        lot_key = int(row.lot_key) if row.lot_key else None
        
        kalem = {
            "_key_scf_stokkart": stok_key,
            "_key_scf_stokkart_birimleri": int(row.birim_key or 0),
            "_key_sis_depo": int(row.depo_key or ana_depo_key),
            "_key_sis_doviz": {"adi": "TL"},
            "anamiktar": miktar,
            "birimfiyati": "0",
            "dovizkuru": "1.000000",
            "kalemturu": "MLZM",
            "miktar": miktar,
            "note": f"SQL_ID:{row.id}",
            "sirano": i + 1,
            "tutari": "0",
            "yerelbirimfiyati": "0",
            "m_lotlar": []
        }
        
        if lot_key:
            kalem["m_lotlar"].append({"_key_scf_serilot": lot_key, "miktar": miktar})
        
        kalemler.append(kalem)
        islenen_id_ler.append(row.id)

    if not kalemler: return

    simdi = datetime.now()
    # Fiş No Başına 'H' eklendi
    fis_no = "H" + simdi.strftime("%Y%m%d%H%M%S")

    payload = {
        "scf_malzeme_fisi_ekle": {
            "session_id": session_id,
            "firma_kodu": firma_kodu,
            "donem_kodu": donem_kodu,
            "kart": {
                "_key_sis_depo": ana_depo_key,
                "_key_sis_doviz": {"adi": "TL"},
                "aciklama1": f"Oto. Entegrasyon - Sarf {fis_no}",
                "dovizkuru": "1.000000",
                "durum": "T",
                "fisno": fis_no,
                "_key_sis_sube": {"subekodu": "00000001"},
                "m_kalemler": kalemler,
                "saat": simdi.strftime("%H:%M:%S"),
                "tarih": simdi.strftime("%Y-%m-%d"),
                "turu": 3 # Sarf Fişi
            }
        }
    }

    resp = apiye_gonder(payload, "Hammadde Fişi Ekleme")
    
    if resp and resp.get('code') == '200':
        sql_guncelle_tamamlandi("sure_kayitlari_kalemleri", islenen_id_ler)

def mamulleri_isle(session_id, firma_kodu, donem_kodu):
    """
    Mamul: Üretim Fişi (Turu: 2) - Tek Fiş
    Önce Lotlar oluşturulur, sonra tek seferde fiş kesilir.
    """
    print("\n>>> Mamul (Üretim) İşlemleri Başlıyor...")
    conn = baglanti_kur()
    if not conn: return

    cursor = conn.cursor()
    # ek_alan_10 kontrolü ile çekiyoruz
    query = """
        SELECT
            ure.id,
            (SELECT s.ek_alan_1 FROM stoklar s WHERE s.kod = ure.stok_kod) AS stok_key,
            (SELECT s.ek_alan_2 FROM stoklar s WHERE s.kod = ure.stok_kod) AS birim_key,
            ure.miktar,
            (SELECT d.ek_alan_1 FROM depolar d WHERE d.kod = ure.uretim_deposu_kodu) AS depo_key,
            ure.is_emri_no AS lot
        FROM uretim_fisi_kalemleri ure
        WHERE (ure.ek_alan_10 IS NULL OR ure.ek_alan_10 != 'A')
    """
    cursor.execute(query)
    rows = cursor.fetchall()
    conn.close()

    if not rows:
        print("Aktarılacak mamul bulunamadı.")
        return

    ana_depo_key = int(rows[0].depo_key) if rows[0].depo_key else 0
    kalemler = []
    islenen_id_ler = []
    simdi = datetime.now()

    print(f"Toplam {len(rows)} mamul bulundu. Lot oluşturma işlemine başlanıyor...")

    for i, row in enumerate(rows):
        stok_key = int(row.stok_key) if row.stok_key else 0
        if stok_key == 0: 
            print(f"ID {row.id} için stok eşleşmedi, atlanıyor.")
            continue

        # --- 1. ADIM: LOT OLUŞTURMA (Tek Tek) ---
        # Lot numarası yoksa benzersiz bir string üret
        lot_no = str(row.lot) if row.lot else f"L-{simdi.strftime('%H%M%S')}-{i}"
        
        lot_payload = {
            "scf_serilot_ekle": {
                "session_id": session_id,
                "firma_kodu": firma_kodu,
                "donem_kodu": donem_kodu,
                "kart": {
                    "aciklama": f"SQL_ID: {row.id}",
                    "numarasi": lot_no,
                    "tarih": simdi.strftime("%Y-%m-%d"),
                    "turu": "L" # Lot Kartı
                }
            }
        }
        
        # API'ye sor: Lot ekle
        lot_resp = apiye_gonder(lot_payload, f"Lot Ekleme: {lot_no}")
        
        yeni_lot_key = None
        if lot_resp and lot_resp.get('code') == '200':
            yeni_lot_key = int(lot_resp.get('key'))
        else:
            print(f"!!! ID {row.id} için Lot oluşturulamadı, bu kalem fişe eklenmeyecek.")
            continue # Lot oluşmazsa bu satırı atla, fişe ekleme

        # --- 2. ADIM: KALEMİ LİSTEYE EKLEME ---
        miktar = str(row.miktar).replace(',', '.')
        
        kalem = {
            "_key_scf_stokkart": stok_key,
            "_key_scf_stokkart_birimleri": int(row.birim_key or 0),
            "_key_sis_depo": int(row.depo_key or ana_depo_key),
            "_key_sis_doviz": {"adi": "TL"},
            "anamiktar": miktar,
            "birimfiyati": "0",
            "dovizkuru": "1.000000",
            "kalemturu": "MLZM",
            "miktar": miktar,
            "note": f"SQL_ID:{row.id}",
            "sirano": i + 1,
            "tutari": "0",
            "yerelbirimfiyati": "0",
            "m_lotlar": [{
                "_key_scf_serilot": yeni_lot_key,
                "miktar": miktar
            }]
        }
        kalemler.append(kalem)
        islenen_id_ler.append(row.id)

    # Eğer eklenecek kalem kalmadıysa (hepsi hata verdiyse) çık
    if not kalemler: 
        print("Fişe eklenecek geçerli mamul kalemi oluşturulamadı.")
        return

    # --- 3. ADIM: TEK SEFERDE FİŞİ GÖNDERME ---
    # Fiş No Başına 'M' eklendi
    fis_no = "M" + simdi.strftime("%Y%m%d%H%M%S")
    
    payload_fis = {
        "scf_malzeme_fisi_ekle": {
            "session_id": session_id,
            "firma_kodu": firma_kodu,
            "donem_kodu": donem_kodu,
            "kart": {
                "_key_sis_depo": ana_depo_key,
                "_key_sis_doviz": {"adi": "TL"},
                "aciklama1": f"Oto. Entegrasyon - Uretim {fis_no}",
                "dovizkuru": "1.000000",
                "durum": "T",
                "fisno": fis_no,
                "_key_sis_sube": {"subekodu": "00000001"},
                "m_kalemler": kalemler,
                "saat": simdi.strftime("%H:%M:%S"),
                "tarih": simdi.strftime("%Y-%m-%d"),
                "turu": 2 # Üretim Fişi
            }
        }
    }

    print(f"Mamul Fişi gönderiliyor... ({len(kalemler)} Kalem)")
    resp_fis = apiye_gonder(payload_fis, "Mamul Fişi Ekleme")

    if resp_fis and resp_fis.get('code') == '200':
        sql_guncelle_tamamlandi("uretim_fisi_kalemleri", islenen_id_ler)

def zaman_damgasi_bas():
    """entegrasyon_tanimlari tablosuna son çalışma zamanını yazar."""
    conn = baglanti_kur()
    if not conn: return
    try:
        cursor = conn.cursor()
        # Örnek format: 2025-12-01 13:36:52
        simdi_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")
        
        # Eğer tablo yapısı 'kod' üzerine kurulu ise:
        cursor.execute("UPDATE entegrasyon_tanimlari SET deger_1 = ? WHERE kod = 'dia_veri_yazma'", simdi_str)
        conn.commit()
        print(f"Zaman damgası güncellendi: {simdi_str}")
    except Exception as e:
        print(f"Zaman damgası hatası: {e}")
    finally:
        conn.close()

# --- ANA DÖNGÜ ---

def main():
    print(">>> SERVİS BAŞLATILDI: dia_veri_yazma.py")
    
    while True:
        print(f"\n{'='*50}")
        print(f"PERİYODİK İŞLEM BAŞLIYOR: {datetime.now()}")
        print(f"{'='*50}")
        
        try:
            # 1. Config Oku
            config = oku_json()
            if not config:
                print("Config okunamadı, 1 dk sonra tekrar denenecek.")
                time.sleep(60)
                continue
                
            firma_kodu = int(config.get('firma', '1'))
            donem_kodu = int(config.get('donem', '1'))
            
            # 2. Session ID Al
            sid = al_session_id()
            
            if sid:
                print(f"Aktif Session ID: {sid}")
                # 3. İşlemleri Sırayla Yap
                hammaddeleri_isle(sid, firma_kodu, donem_kodu)
                mamulleri_isle(sid, firma_kodu, donem_kodu)
                
                # 4. Bitiş Zamanını Logla
                zaman_damgasi_bas()
            else:
                print("Session ID alınamadı! İşlemler pas geçiliyor.")

            # 5. Bekleme Süresi
            bekleme_dk = int(config.get("periyot_dakika_dia_veri_yazma", 30))
            print(f"\n>>> İşlemler tamamlandı. {bekleme_dk} dakika bekleniyor...")
            time.sleep(bekleme_dk * 60)
            
        except KeyboardInterrupt:
            print("\nKullanıcı tarafından durduruldu.")
            break
        except Exception as e:
            print(f"CRITICAL ERROR (ANA DÖNGÜ): {e}")
            print("1 Dakika sonra tekrar denenecek...")
            time.sleep(60)

if __name__ == "__main__":
    main()