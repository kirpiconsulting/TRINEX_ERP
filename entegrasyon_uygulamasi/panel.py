import tkinter as tk
from tkinter import ttk, messagebox, scrolledtext
import subprocess
import os
import sys
import threading
import queue
import datetime
import json
import shutil

# --- AYARLAR VE SABİTLER ---

SCRIPTS = [
    "oturum_kimligi.py",
    "depolar.py",
    "cariler.py",
    "operasyonlar.py",
    "operatorler.py",
    "tezgahlar.py",
    "stoklar.py",
    "satis_siparisleri.py",
    "satis_siparis_kalemleri.py",
    "satinalma_siparisleri.py",
    "satinalma_siparis_kalemleri.py",
    "receteler.py",
    "recete_kalemleri.py",
    "sistem_stok_birimleri.py",
    "lot_seri_tanimlari.py",
    "stok_miktarlari.py",
    "urun_ozellikleri.py",
    "dia_veri_yazma.py"
]

# Renk Paleti
COLOR_BG_DARK = "#2c3e50"
COLOR_BG_LIGHT = "#ecf0f1"
COLOR_ACCENT = "#3498db"
COLOR_SUCCESS = "#27ae60"
COLOR_DANGER = "#e74c3c"
COLOR_TEXT = "#2c3e50"
FONT_MAIN = ("Segoe UI", 10)
FONT_BOLD = ("Segoe UI", 10, "bold")

log_queue = queue.Queue()

# --- YARDIMCI FONKSİYONLAR ---

def get_credentials():
    """bilgiler.json'dan kullanıcı adı şifre okur."""
    default_user = "admin"
    default_pass = "admin"
    try:
        if getattr(sys, 'frozen', False):
            base_path = os.path.dirname(sys.executable)
        else:
            base_path = os.path.dirname(os.path.abspath(__file__))
            
        json_path = os.path.join(base_path, 'bilgiler.json')
        
        with open(json_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
            return data.get("panel_kullanici", default_user), str(data.get("panel_parola", default_pass))
    except:
        return default_user, default_pass

# --- ARKA PLAN İŞLEMLERİ ---

class ProcessManager:
    def __init__(self):
        self.processes = {}

    def is_running(self, script_name):
        proc = self.processes.get(script_name)
        if proc and proc.poll() is None:
            return True
        return False

    def reader_thread(self, process, script_name):
        try:
            for line in iter(process.stdout.readline, b''):
                try:
                    decoded_line = line.decode('utf-8').strip()
                except UnicodeDecodeError:
                    decoded_line = line.decode('cp1254', errors='replace').strip()
                
                if decoded_line:
                    timestamp = datetime.datetime.now().strftime("%H:%M:%S")
                    log_queue.put(f"[{timestamp}] [{script_name}] {decoded_line}")
        except Exception as e:
            log_queue.put(f"Log Okuma Hatası ({script_name}): {e}")
        finally:
            if process.stdout:
                process.stdout.close()

    def start_process(self, script_name):
        if self.is_running(script_name): return False
        try:
            creation_flags = subprocess.CREATE_NO_WINDOW if os.name == 'nt' else 0
            my_env = os.environ.copy()
            my_env["PYTHONIOENCODING"] = "utf-8"

            # --- PYTHON YOLUNU BELİRLEME (Portable Öncelikli) ---
            python_cmd = "python" # Varsayılan sistem komutu
            
            if getattr(sys, 'frozen', False):
                # EXE Çalışıyorsa
                base_dir = os.path.dirname(sys.executable)
                
                # 1. Öncelik: Yanındaki 'python_env' klasörü (Müşteri için)
                embed_python = os.path.join(base_dir, "python_env", "python.exe")
                
                if os.path.exists(embed_python):
                    python_cmd = embed_python
                else:
                    # 2. Öncelik: Sistemdeki Python (Yedek)
                    log_queue.put(f"Uyarı: Gömülü Python bulunamadı, sistem Python deneniyor...")
                    
                    # Senin bilgisayarındaki yollar veya genel komutlar
                    possible_paths = [
                        r"C:\Users\pc\AppData\Local\Programs\Python\Python312\python.exe",
                        "python", "py"
                    ]
                    for cmd in possible_paths:
                        if shutil.which(cmd) or (os.path.isabs(cmd) and os.path.exists(cmd)):
                            python_cmd = cmd
                            break
            else:
                # Script çalışıyorsa (Geliştirme ortamı)
                base_dir = os.path.dirname(os.path.abspath(__file__))
                python_cmd = sys.executable

            # Script yolunu belirle
            script_full_path = os.path.join(base_dir, script_name)

            if not os.path.exists(script_full_path):
                log_queue.put(f"!!! HATA: Dosya bulunamadı -> {script_full_path}")
                return False

            proc = subprocess.Popen(
                [python_cmd, "-u", script_full_path],
                cwd=base_dir,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                creationflags=creation_flags,
                env=my_env
            )
            self.processes[script_name] = proc
            t = threading.Thread(target=self.reader_thread, args=(proc, script_name))
            t.daemon = True
            t.start()
            log_queue.put(f">>> {script_name} BAŞLATILDI...")
            return True
        except Exception as e:
            log_queue.put(f"!!! HATA: {script_name}: {e}")
            return False

    def stop_process(self, script_name):
        proc = self.processes.get(script_name)
        if proc:
            try:
                subprocess.call(['taskkill', '/F', '/T', '/PID', str(proc.pid)], creationflags=subprocess.CREATE_NO_WINDOW)
            except:
                proc.terminate()
            self.processes[script_name] = None
            log_queue.put(f">>> {script_name} DURDURULDU.")
            return True
        return False

    def stop_all(self):
        for script in list(self.processes.keys()):
            if self.is_running(script):
                self.stop_process(script)

# --- ARAYÜZ SINIFLARI ---

class LoginWindow:
    def __init__(self, root):
        self.root = root
        self.is_logged_in = False
        
        self.root.title("Giriş Yap - Bartez Entegrasyon")
        self.root.geometry("400x350")
        self.root.configure(bg=COLOR_BG_LIGHT)
        self.root.resizable(False, False)
        
        screen_width = root.winfo_screenwidth()
        screen_height = root.winfo_screenheight()
        x = (screen_width/2) - (400/2)
        y = (screen_height/2) - (350/2)
        self.root.geometry('%dx%d+%d+%d' % (400, 350, x, y))

        main_frame = tk.Frame(root, bg=COLOR_BG_LIGHT)
        main_frame.place(relx=0.5, rely=0.5, anchor="center")

        tk.Label(main_frame, text="TRINEX ERP", font=("Segoe UI", 20, "bold"), fg=COLOR_BG_DARK, bg=COLOR_BG_LIGHT).pack(pady=(0, 5))
        tk.Label(main_frame, text="Entegrasyon Yönetim Paneli", font=("Segoe UI", 10), fg="#7f8c8d", bg=COLOR_BG_LIGHT).pack(pady=(0, 30))

        tk.Label(main_frame, text="Kullanıcı Adı", font=FONT_BOLD, bg=COLOR_BG_LIGHT, fg=COLOR_TEXT, anchor="w").pack(fill="x")
        self.entry_user = tk.Entry(main_frame, font=FONT_MAIN, relief="flat", bd=10)
        self.entry_user.pack(fill="x", pady=(0, 15))

        tk.Label(main_frame, text="Parola", font=FONT_BOLD, bg=COLOR_BG_LIGHT, fg=COLOR_TEXT, anchor="w").pack(fill="x")
        self.entry_pass = tk.Entry(main_frame, font=FONT_MAIN, show="●", relief="flat", bd=10)
        self.entry_pass.pack(fill="x", pady=(0, 20))
        self.entry_pass.bind('<Return>', self.check_login)

        btn_login = tk.Button(main_frame, text="GİRİŞ YAP", font=FONT_BOLD, bg=COLOR_ACCENT, fg="white", 
                              relief="flat", pady=8, cursor="hand2", command=self.check_login)
        btn_login.pack(fill="x")
        self.entry_user.focus()

    def check_login(self, event=None):
        kullanici, parola = get_credentials()
        input_user = self.entry_user.get()
        input_pass = self.entry_pass.get()

        if input_user == kullanici and input_pass == parola:
            self.is_logged_in = True
            self.root.destroy()
        else:
            messagebox.showerror("Hata", "Kullanıcı adı veya parola hatalı!")

class MainApp:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("Bartez Entegrasyon Paneli v1.0")
        self.root.geometry("1000x750")
        self.root.configure(bg=COLOR_BG_LIGHT)
        
        screen_width = self.root.winfo_screenwidth()
        screen_height = self.root.winfo_screenheight()
        x = (screen_width/2) - (1000/2)
        y = (screen_height/2) - (750/2)
        self.root.geometry('%dx%d+%d+%d' % (1000, 750, x, y))

        self.manager = ProcessManager()
        self.ui_rows = {}

        header_frame = tk.Frame(self.root, bg=COLOR_BG_DARK, height=60)
        header_frame.pack(fill="x")
        header_frame.pack_propagate(False)
        
        tk.Label(header_frame, text="BARTEZ ENTEGRASYON YÖNETİCİSİ", font=("Segoe UI", 14, "bold"), fg="white", bg=COLOR_BG_DARK).pack(side="left", padx=20)
        tk.Label(header_frame, text=f"Tarih: {datetime.datetime.now().strftime('%d.%m.%Y')}", font=("Segoe UI", 10), fg="#bdc3c7", bg=COLOR_BG_DARK).pack(side="right", padx=20)

        main_pane = tk.PanedWindow(self.root, orient=tk.HORIZONTAL, bg=COLOR_BG_LIGHT, sashwidth=4)
        main_pane.pack(fill="both", expand=True, padx=10, pady=10)

        left_container = tk.Frame(main_pane, bg="white", bd=1, relief="solid")
        tk.Label(left_container, text="SERVİS LİSTESİ", bg="#f8f9fa", fg=COLOR_TEXT, font=FONT_BOLD, pady=10, relief="groove").pack(fill="x")

        list_canvas = tk.Canvas(left_container, bg="white", highlightthickness=0)
        list_scrollbar = tk.Scrollbar(left_container, orient="vertical", command=list_canvas.yview)
        scrollable_frame = tk.Frame(list_canvas, bg="white")

        scrollable_frame.bind("<Configure>", lambda e: list_canvas.configure(scrollregion=list_canvas.bbox("all")))
        list_canvas.create_window((0, 0), window=scrollable_frame, anchor="nw", width=430)
        list_canvas.configure(yscrollcommand=list_scrollbar.set)

        for script in SCRIPTS:
            self.create_row(scrollable_frame, script)

        list_canvas.pack(side="left", fill="both", expand=True)
        list_scrollbar.pack(side="right", fill="y")
        main_pane.add(left_container, width=460)

        right_container = tk.Frame(main_pane, bg="#1e1e1e", bd=1, relief="solid")
        log_header = tk.Frame(right_container, bg="#2d2d2d", pady=5, padx=10)
        log_header.pack(fill="x")
        tk.Label(log_header, text="CANLI SİSTEM GÜNLÜĞÜ", fg="#2ecc71", bg="#2d2d2d", font=("Consolas", 10, "bold")).pack(side="left")
        btn_clear = tk.Button(log_header, text="Temizle", bg="#444", fg="white", font=("Segoe UI", 8), bd=0, padx=10, command=self.clear_logs)
        btn_clear.pack(side="right")

        self.log_box = scrolledtext.ScrolledText(right_container, state='disabled', bg="#1e1e1e", fg="#d4d4d4", font=("Consolas", 9), bd=0, highlightthickness=0)
        self.log_box.pack(fill="both", expand=True)
        main_pane.add(right_container)

        footer = tk.Frame(self.root, bg="white", height=60, bd=1, relief="raised")
        footer.pack(fill="x", side="bottom")
        footer.pack_propagate(False)

        btn_start_all = tk.Button(footer, text="▶ TÜMÜNÜ BAŞLAT", bg=COLOR_SUCCESS, fg="white", font=FONT_BOLD, relief="flat", padx=20, command=self.start_all, cursor="hand2")
        btn_start_all.pack(side="left", padx=20, pady=12)

        btn_stop_all = tk.Button(footer, text="⏹ TÜMÜNÜ DURDUR", bg=COLOR_DANGER, fg="white", font=FONT_BOLD, relief="flat", padx=20, command=self.stop_all, cursor="hand2")
        btn_stop_all.pack(side="right", padx=20, pady=12)

        self.root.protocol("WM_DELETE_WINDOW", self.on_closing)
        self.check_log_queue()
        self.check_statuses()
        self.root.mainloop()

    def create_row(self, parent, script_name):
        row = tk.Frame(parent, bg="white", pady=8, padx=5, borderwidth=0)
        row.pack(fill="x")
        tk.Frame(parent, height=1, bg="#ecf0f1").pack(fill="x")

        tk.Label(row, text=script_name, font=("Consolas", 10), bg="white", fg=COLOR_TEXT, width=30, anchor="w").pack(side="left", padx=10)
        lbl_icon = tk.Label(row, text="●", fg="#ccc", bg="white", font=("Arial", 14))
        lbl_icon.pack(side="left", padx=5)
        lbl_status = tk.Label(row, text="DURDU", fg="#95a5a6", bg="white", font=("Segoe UI", 9, "bold"), width=10, anchor="w")
        lbl_status.pack(side="left", padx=0)

        btn_action = tk.Button(row, text="Başlat", width=8, bg=COLOR_ACCENT, fg="white", font=("Segoe UI", 9), relief="flat", cursor="hand2", command=lambda s=script_name: self.toggle_process(s))
        btn_action.pack(side="right", padx=10)
        self.ui_rows[script_name] = {"status_lbl": lbl_status, "icon": lbl_icon, "btn": btn_action}

    def toggle_process(self, script_name):
        if self.manager.is_running(script_name):
            self.manager.stop_process(script_name)
        else:
            self.manager.start_process(script_name)
        self.update_ui(script_name)

    def start_all(self):
        for script in SCRIPTS:
            if not self.manager.is_running(script):
                self.manager.start_process(script)
                self.update_ui(script)

    def stop_all(self):
        self.manager.stop_all()
        for script in SCRIPTS:
            self.update_ui(script)

    def update_ui(self, script_name):
        widgets = self.ui_rows[script_name]
        if self.manager.is_running(script_name):
            widgets["status_lbl"].config(text="ÇALIŞIYOR", fg=COLOR_SUCCESS)
            widgets["icon"].config(fg=COLOR_SUCCESS)
            widgets["btn"].config(text="Durdur", bg=COLOR_DANGER)
        else:
            widgets["status_lbl"].config(text="DURDU", fg="#95a5a6")
            widgets["icon"].config(fg="#ccc")
            widgets["btn"].config(text="Başlat", bg=COLOR_ACCENT)

    def check_statuses(self):
        for script in SCRIPTS:
            self.update_ui(script)
        self.root.after(2000, self.check_statuses)

    def check_log_queue(self):
        while not log_queue.empty():
            msg = log_queue.get()
            self.log_box.config(state='normal')
            self.log_box.insert(tk.END, msg + "\n")
            self.log_box.see(tk.END)
            self.log_box.config(state='disabled')
        self.root.after(100, self.check_log_queue)

    def clear_logs(self):
        self.log_box.config(state='normal')
        self.log_box.delete(1.0, tk.END)
        self.log_box.config(state='disabled')

    def on_closing(self):
        if messagebox.askokcancel("Çıkış", "Uygulama kapatılıyor. Tüm işlemler durdurulsun mu?"):
            self.manager.stop_all()
            self.root.destroy()
            os._exit(0)

if __name__ == "__main__":
    login_root = tk.Tk()
    login_window = LoginWindow(login_root)
    login_root.mainloop()

    if login_window.is_logged_in:
        MainApp()
    else:
        sys.exit()