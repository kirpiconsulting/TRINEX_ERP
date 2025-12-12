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

    code, msg, data = rapor_calistir("D_ENT_08")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT kalem_kodu FROM satis_siparis_kalemleri")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        kalem_kodu = str(d["_key"])
        fisno = d["fisno"]
        sira_no = d["_serial"]
        stok_kod = d["stok_kod"]
        anamiktar = d["anamiktar"]
        anabirim = d["anabirim"]
        teslimattarihi = d["teslimattarihi"]
        suan = datetime.now()

        if kalem_kodu in mevcutlar:
            cur.execute("""
                UPDATE satis_siparis_kalemleri SET 
                    evrak_no=?, sira_numarasi=?, stok_kod=?, miktar=?, stok_birim=?, teslim_tarihi=?, guncelleme_tarihi=?, guncelleyen_kullanici='ENTEGRASYON_01'
                WHERE kalem_kodu=?
            """, fisno, sira_no, stok_kod, anamiktar, anabirim, teslimattarihi, suan, kalem_kodu)
        else:
            cur.execute("""
                INSERT INTO satis_siparis_kalemleri (kalem_kodu, evrak_no, stok_kod, miktar,stok_birim,teslim_tarihi, olusturma_tarihi,sira_numarasi, olusturan_kullanici)
                VALUES (?, ?, ?, ?, ?,?,?, ?, 'ENTEGRASYON_01')
            """, kalem_kodu, fisno, stok_kod, anamiktar, anabirim, teslimattarihi, suan, sira_no)

    for kalem_kodu in mevcutlar:
        if kalem_kodu not in keys:
            cur.execute("DELETE FROM satis_siparis_kalemleri WHERE kalem_kodu=?", kalem_kodu)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='satis_siparis_kalemleri'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Satış Sipariş Kalemleri başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_satis_siparis_kalemleri", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
