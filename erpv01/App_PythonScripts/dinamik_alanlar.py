from rapor_calistir import rapor_calistir, oku_json
import pyodbc
from datetime import datetime
import time
import json

def run_table_sync():
    cfg = oku_json()
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

    code, msg, data = rapor_calistir("D_ENT_16")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT ek_alan_1 FROM dinamik_alanlar")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        ek = str(d["_key"])
        kod = d["kodu"]
        a1 = d["adi"]
        a2 = d["tablo_adi"]
        a3 = d["kolon_adi"]

        # dict geldiği için JSON string yapıyoruz
        degerler = json.dumps(d["widgetprm"], ensure_ascii=False)

        suan = datetime.now()

        if ek in mevcutlar:
            cur.execute("""
                UPDATE dinamik_alanlar SET 
                    kod=?, 
                    ad=?, 
                    tad=?, 
                    kad=?, 
                    degerler=?, 
                    guncelleme_tarihi=?, 
                    guncelleyen_kullanici='ENTEGRASYON_01'
                WHERE ek_alan_1=?
            """, (kod, a1, a2, a3, degerler, suan, ek))
        else:
            cur.execute("""
                INSERT INTO dinamik_alanlar (
                    ek_alan_1, kod, ad, tad, kad, degerler, 
                    olusturma_tarihi, olusturan_kullanici, aktif_mi
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, 'ENTEGRASYON_01', 1)
            """, (ek, kod, a1, a2, a3, degerler, suan))

    # Silinmesi gerekenler
    for ek in mevcutlar:
        if ek not in keys:
            cur.execute("DELETE FROM dinamik_alanlar WHERE ek_alan_1=?", (ek,))

    # Log güncelle
    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='dinamik_alanlar'
    """, (datetime.now(),))

    conn.commit()
    conn.close()
    print("Dinamik Alanlar başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_dinamik_alanlar", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
