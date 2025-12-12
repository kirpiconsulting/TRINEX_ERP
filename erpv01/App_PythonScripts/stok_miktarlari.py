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

    code, msg, data = rapor_calistir("D_ENT_14")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT ek_alan_1 FROM stok_miktarlari")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        ek = str(d["_key"])
        stok_kod = d["stokkartkodu"]
        depo_kod = d["depokodu"]
        miktar = d["miktar"]
        anabirim = d["anabirim"]

        suan = datetime.now()

        if ek in mevcutlar:
            cur.execute("""
                UPDATE stok_miktarlari SET 
                    stok_kod=?, depo_kod=?, stok_miktar=?, stok_birim=?, guncelleme_tarihi=?, guncelleyen_kullanici='ENTEGRASYON_01', aktif_mi = 1
                WHERE ek_alan_1=?
            """, stok_kod, depo_kod, miktar, anabirim, suan, ek)
        else:
            cur.execute("""
                INSERT INTO stok_miktarlari (ek_alan_1, stok_kod, depo_kod, stok_miktar, stok_birim, olusturma_tarihi, olusturan_kullanici, aktif_mi)
                VALUES (?, ?, ?, ?, ?,?, 'ENTEGRASYON_01', 1)
            """, ek, stok_kod,depo_kod, miktar, anabirim, suan)

    for ek in mevcutlar:
        if ek not in keys:
            cur.execute("DELETE FROM stok_miktarlari WHERE ek_alan_1=?", ek)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='stok_miktarlari'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Stok Miktarları başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_stok_miktarlari", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
