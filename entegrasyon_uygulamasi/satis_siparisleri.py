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

    code, msg, data = rapor_calistir("D_ENT_07")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT ek_alan_1 FROM satis_siparisleri")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        ek = str(d["_key"])
        fisno = d["fisno"]
        tarih = d["tarih"]
        cari = d["cari_kod"] if d["cari_kod"] else "-"
        suan = datetime.now()

        if ek in mevcutlar:
            cur.execute("""
                UPDATE satis_siparisleri SET 
                    evrak_no=?, tarih=?, cari_kod=?, guncelleme_tarihi=?, guncelleyen_kullanici='ENTEGRASYON_01'
                WHERE ek_alan_1=?
            """, fisno, tarih, cari, suan, ek)
        else:
            cur.execute("""
                INSERT INTO satis_siparisleri (ek_alan_1, evrak_no, tarih, cari_kod, olusturma_tarihi, olusturan_kullanici)
                VALUES (?, ?, ?, ?, ?, 'ENTEGRASYON_01')
            """, ek, fisno, tarih, cari, suan)

    for ek in mevcutlar:
        if ek not in keys:
            cur.execute("DELETE FROM satis_siparisleri WHERE ek_alan_1=?", ek)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='satis_siparisleri'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Satış Siparişleri başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_satis_siparisleri", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
