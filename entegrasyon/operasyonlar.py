from rapor_calistir import rapor_calistir, oku_json
import pyodbc
from datetime import datetime
import time

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

    code, msg, data = rapor_calistir("D_ENT_03")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT ek_alan_1 FROM operasyonlar")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        ek = str(d["_key"])
        kod = d["operasyonkodu"]
        ad = d["aciklama"]
        aktif = 1 if d["durum"] == "A" else 0
        suan = datetime.now()

        if ek in mevcutlar:
            cur.execute("""
                UPDATE operasyonlar SET 
                    kod=?, ad=?, aktif_mi=?, guncelleme_tarihi=?, guncelleyen_kullanici='ENTEGRASYON_01'
                WHERE ek_alan_1=?
            """, kod, ad, aktif, suan, ek)
        else:
            cur.execute("""
                INSERT INTO operasyonlar (ek_alan_1, kod, ad, aktif_mi, olusturma_tarihi, olusturan_kullanici)
                VALUES (?, ?, ?, ?, ?, 'ENTEGRASYON_01')
            """, ek, kod, ad, aktif, suan)

    for ek in mevcutlar:
        if ek not in keys:
            cur.execute("DELETE FROM operasyonlar WHERE ek_alan_1=?", ek)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='operasyonlar'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Operasyonlar başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_operasyonlar", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
