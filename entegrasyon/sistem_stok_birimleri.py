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

    code, msg, data = rapor_calistir("D_ENT_13")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT ek_alan_1 FROM veri_seti_kalemleri where veri_seti_kod = 'STOK_BIRIMLERI'")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        ek = str(d["_key"])
        kod = d["birimkod"]
        ad = d["birimadi"]
        sira = d["_serial"]
        suan = datetime.now()

        if ek in mevcutlar:
            cur.execute("""
                UPDATE veri_seti_kalemleri SET 
                    kod=?, ad=?, sira_numarasi=?, guncelleme_tarihi=?, guncelleyen_kullanici='ENTEGRASYON_01'
                WHERE veri_seti_kod = 'STOK_BIRIMLERI' and ek_alan_1=?
            """, kod, ad, sira, suan, ek)
        else:
            cur.execute("""
                INSERT INTO veri_seti_kalemleri (ek_alan_1, kod, ad, sira_numarasi, olusturma_tarihi, olusturan_kullanici, veri_seti_kod)
                VALUES (?, ?, ?, ?, ?, 'ENTEGRASYON_01', 'STOK_BIRIMLERI')
            """, ek, kod, ad, sira, suan)

    for ek in mevcutlar:
        if ek not in keys:
            cur.execute("DELETE FROM veri_seti_kalemleri WHERE veri_seti_kod = 'STOK_BIRIMLERI' and ek_alan_1=?", ek)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='sistem_stok_birimleri'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Sistem Stok Birimleri başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_sistem_stok_birimleri", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
