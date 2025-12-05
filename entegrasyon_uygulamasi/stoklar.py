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

    code, msg, data = rapor_calistir("D_ENT_06")
    if code != "200":
        print("API HATA:", msg)
        conn.close()
        return

    keys = [str(x["_key"]) for x in data]

    cur.execute("SELECT ek_alan_1 FROM stoklar")
    mevcutlar = [str(r[0]) for r in cur.fetchall()]

    for d in data:
        ek = str(d["_key"])
        kod = d["stokkartkodu"]
        ad = d["aciklama"]
        aktif = 1 if d["durum"] == "A" else 0
        suan = datetime.now()
        anabirim = d["anabirim"]
        ek2 = str(d["birim_key"])
        ek3 = str(d["ozellikler"])
        ek4 = str(d["resim_url"])

        ozelkod1 = str(d["ozel_kod1_aciklama"])
        ozelkod2 = str(d["ozel_kod2_aciklama"])
        ozelkod3 = str(d["ozel_kod3_aciklama"])
        ozelkod4 = str(d["ozel_kod4_aciklama"])
        ozelkod5 = str(d["ozel_kod5_aciklama"])

        en = str(d["en"])
        boy = str(d["boy"])
        yukseklik = str(d["yukseklik"])
        agirlik = str(d["agirlik"])
        

        if ek in mevcutlar:
            cur.execute("""
                UPDATE stoklar SET 
                    kod=?,
                    ad=?,
                    aktif_mi=?,
                    guncelleme_tarihi=?,
                    ana_birim=?,
                    ek_alan_2=?,
                    ek_alan_3=?,
                    ek_alan_4=?,
                    guncelleyen_kullanici='ENTEGRASYON_01',
                    ozel_kod01=?,
                    ozel_kod02=?,
                    ozel_kod03=?,
                    ozel_kod04=?,
                    ozel_kod05=?,
                    en=?,
                    boy=?,
                    yukseklik=?,
                    agirlik=?
                WHERE ek_alan_1=?
            """, (
                kod, ad, aktif, suan, anabirim,
                ek2, ek3, ek4,
                ozelkod1, ozelkod2, ozelkod3, ozelkod4, ozelkod5,
                en,boy,yukseklik,agirlik,
                ek
            ))
        else:
            cur.execute("""
                INSERT INTO stoklar (
                    ek_alan_1, kod, ad, aktif_mi,
                    olusturma_tarihi, olusturan_kullanici,
                    ana_birim, ek_alan_2, ek_alan_3, ek_alan_4,
                    ozel_kod01, ozel_kod02, ozel_kod03, ozel_kod04, ozel_kod05, en, boy, yukseklik, agirlik
                ) VALUES (
                    ?, ?, ?, ?, ?, 'ENTEGRASYON_01',
                    ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?
                )
            """, (
                ek, kod, ad, aktif, suan,
                anabirim, ek2, ek3, ek4,
                ozelkod1, ozelkod2, ozelkod3, ozelkod4, ozelkod5, en,boy,yukseklik,agirlik
            ))

    for ek in mevcutlar:
        if ek not in keys:
            cur.execute("DELETE FROM stoklar WHERE ek_alan_1=?", ek)

    cur.execute("""
        UPDATE entegrasyon_tanimlari 
        SET deger_1=? 
        WHERE kod='stoklar'
    """, datetime.now())

    conn.commit()
    conn.close()
    print("Stoklar başarıyla güncellendi.")


if __name__ == "__main__":
    while True:
        run_table_sync()
        cfg = oku_json()
        dakika = int(cfg.get("periyot_dakika_stoklar", 30))
        print(f"{dakika} dakika sonra tekrar çalışacak...")
        time.sleep(dakika * 60)
