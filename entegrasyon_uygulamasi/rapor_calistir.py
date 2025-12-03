import json
import base64
import requests
import pyodbc

def oku_json():
    with open("bilgiler.json", "r", encoding="utf-8") as f:
        return json.load(f)

def al_session_id():
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
    cur.execute("SELECT deger_1 FROM entegrasyon_tanimlari WHERE kod='SESSION_ID'")
    sid = cur.fetchone()[0]
    conn.close()
    return sid

def rapor_calistir(report_code):
    cfg = oku_json()
    session_id = al_session_id()

    wsAdres = "https://bartez.ws.dia.com.tr/api/v3/rpr/json"
    wsInput = {
        "rpr_raporsonuc_getir": {
            "session_id": session_id,
            "firma_kodu": int(cfg["firma"]),
            "donem_kodu": int(cfg["donem"]),
            "report_code": report_code,
            "param": {"firma": cfg["firma"], "donem": cfg["donem"]},
            "format_type": "json"
        }
    }

    try:
        r = requests.post(wsAdres, json=wsInput, timeout=10)
        r.raise_for_status()
        data = r.json()

        code = str(data.get("code"))  # HER ZAMAN STRING

        # -------------------------------
        # HATA DURUMU
        # -------------------------------
        if code != "200":
            msg = data.get("msg", "Bilinmeyen hata")
            return code, msg, []

        # -------------------------------
        # BAŞARILI DURUM – BASE64 DECODE
        # -------------------------------
        encoded = data.get("result", "")
        if not encoded:
            return "200", "Boş çıktı", []

        decoded = base64.b64decode(encoded).decode("utf-8")
        result_json = json.loads(decoded)

        rows = result_json.get("__rows", [])

        return "200", "Başarılı", rows

    except Exception as e:
        return "-1", str(e), []
