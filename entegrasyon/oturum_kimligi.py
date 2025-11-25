import json
import requests
import pyodbc
import time
from datetime import datetime

def parse_conn_str(cs):
    parts = cs.split(";")
    parsed = {}
    for p in parts:
        if "=" in p:
            k, v = p.split("=", 1)
            parsed[k.strip().lower()] = v.strip()
    return (
        parsed.get("server"),
        parsed.get("database"),
        parsed.get("uid"),
        parsed.get("pwd")
    )

def yenile_ve_kaydet():
    try:
        with open('bilgiler.json', 'r', encoding='utf-8') as f:
            c = json.load(f)
    except Exception as e:
        print(f"HATA: JSON okunamadı: {e}")
        return

    k = c.get("kullanici")
    p = c.get("parola")
    a = c.get("apikey")
    cs = c.get("connectionstring")
    periyot = int(c.get("periyot_dakika_oturum_kimligi", 30))

    server_name, db_name, uid, pwd = parse_conn_str(cs)
    if not server_name or not db_name or not uid or not pwd:
        print("HATA: Connection String hatalı veya eksik.")
        return

    conn_str = (
        f"DRIVER={{ODBC Driver 17 for SQL Server}};"
        f"SERVER={server_name};"
        f"DATABASE={db_name};"
        f"UID={uid};"
        f"PWD={pwd};"
        f"TrustServerCertificate=yes"
    )

    wsAdres = "https://kirpi.ws.dia.com.tr/api/v3/sis/json"
    wsInput = {
        "login": {
            "username": k,
            "password": p,
            "disconnect_same_user": "true",
            "lang": "tr",
            "params": {"apikey": a}
        }
    }
    headers = {"Content-Type": "application/json;charset=UTF-8"}

    while True:
        guncel_zaman = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
        print(f"[{guncel_zaman}] Session ID yenileme işlemi başladı.")

        try:
            r = requests.post(wsAdres, json=wsInput, headers=headers, timeout=10)
            r.raise_for_status()
            data = r.json()
            code = data.get("code")
            msg = data.get("msg")

            if code == '200':
                session_id = msg
                print(f"[{guncel_zaman}] Yeni Session ID alındı: {session_id}")

                conn = pyodbc.connect(conn_str)
                cursor = conn.cursor()
                sql = "UPDATE entegrasyon_tanimlari SET deger_1 = ?, deger_2 = ? WHERE kod = 'SESSION_ID';"
                cursor.execute(sql, session_id, guncel_zaman)
                conn.commit()
                conn.close()
                print("Veritabanı güncellendi.")
            else:
                print(f"[{guncel_zaman}] HATA: API Yanıtı Code={code}, Msg='{msg}'")

        except Exception as e:
            print(f"[{guncel_zaman}] HATA: {e}")

        print(f"{periyot} dakika bekleniyor...")
        time.sleep(periyot * 60)

if __name__ == "__main__":
    yenile_ve_kaydet()
