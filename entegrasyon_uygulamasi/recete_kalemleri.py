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

    code, msg, data = rapor_calistir("D_ENT_12")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT kalem_kodu FROM recete_kalemleri")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        kalem_kodu = str(d["_key"])
        evrak_no = d["evrak_no"] if d["evrak_no"] else "-"
        sira_numarasi = d["sira_numarasi"]
        kaynak_tipi = d["kaynak_tipi"]
        kaynak_kod = d["kaynak_kod"]
        kullanim_miktar = d["kullanim_miktar"]
        ek_alan_2 = d["tuketim_tezgah"]
        ek_alan_3 = d["tuketim_depo"]
        operasyon_kod = d["operasyon_kod"]
        not1= d["not1"]
        not2=d["not2"]
        suan = datetime.now()
        
        raw_miktar = str(d["kullanim_miktar"] if d["kullanim_miktar"] is not None else "0")
        raw_miktar = raw_miktar.replace(",", ".")
        try:
            kullanim_miktar = float(raw_miktar) if raw_miktar.strip() != "" else 0.0
        except ValueError:
            kullanim_miktar = 0.0

   
        if kalem_kodu in mevcutlar:
            cur.execute("""
                UPDATE recete_kalemleri SET
                    evrak_no=?,
                    sira_numarasi=?,
                    kaynak_tipi=?,
                    kaynak_kod=?,
                    kullanim_miktar=?,
                    ek_alan_2=?,
                    ek_alan_3=?,
                    operasyon_kod=?,
                    guncelleme_tarihi=?,
                    guncelleyen_kullanici='ENTEGRASYON_01',
                    not_1=?,
                    not_2=?
                WHERE kalem_kodu=?
            """,
            evrak_no, sira_numarasi, kaynak_tipi, kaynak_kod,
            kullanim_miktar, ek_alan_2, ek_alan_3, operasyon_kod,
            suan,not1,not2, kalem_kodu)
        else:
            cur.execute("""
                    INSERT INTO recete_kalemleri
                    (kalem_kodu, evrak_no, sira_numarasi, kaynak_tipi, kaynak_kod,
                    kullanim_miktar, ek_alan_2, ek_alan_3, operasyon_kod,
                    olusturma_tarihi, olusturan_kullanici,not_1,not_2)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'ENTEGRASYON_01',?,?)
                """,
                kalem_kodu, evrak_no, sira_numarasi, kaynak_tipi, kaynak_kod,
                kullanim_miktar, ek_alan_2, ek_alan_3, operasyon_kod,
                suan,not1,not2)

    for kalem_kodu in mevcutlar:
        if kalem_kodu not in keys:
            cur.execute("DELETE FROM recete_kalemleri WHERE kalem_kodu=?", kalem_kodu)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='recete_kalemleri'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Reçete Kalemleri başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_recete_kalemleri", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
