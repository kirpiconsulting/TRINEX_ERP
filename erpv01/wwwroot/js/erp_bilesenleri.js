/**
 * ERP Liste Ekranı Üst Butonları
 */
function liste_ekrani_butonlari_olustur(targetId, options) {
    var container = document.getElementById(targetId);
    if (!container) return;

    container.innerHTML = "";
    var fonksiyonListesi = (options && options.fonksiyonlar) || [];
    var ciktiListesi = (options && options.ciktilar) || [];

    var desktopHtml = `
        <div class="erp-toolbar-desktop d-flex justify-content-end align-items-center flex-wrap">
            <button type="button" class="btn-erp-base btn-erp-yeni" onclick="btnYeniClick()"><i class="fa-solid fa-plus"></i> Yeni</button>
            <button type="button" class="btn-erp-base btn-erp-kaydet" onclick="btnKaydetClick()"><i class="fa-solid fa-floppy-disk"></i> Kaydet</button>
            <button type="button" class="btn-erp-base btn-erp-guncelle" onclick="btnGuncelleClick()"><i class="fa-solid fa-pen-to-square"></i> Güncelle</button>
            <button type="button" class="btn-erp-base btn-erp-sil" onclick="btnSilClick()"><i class="fa-solid fa-trash-can"></i> Sil</button>

            <div class="dropdown d-inline-block ms-1">
                <button class="btn-erp-base btn-erp-fonksiyon dropdown-toggle" type="button" data-bs-toggle="dropdown"><i class="fa-solid fa-bolt"></i> Fonksiyonlar</button>
                <ul class="dropdown-menu shadow">
                    ${fonksiyonListesi.length > 0 ? fonksiyonListesi.map(f => `<li><a class="dropdown-item" href="#" onclick="${f.action}('${f.id || 0}')">${f.ad}</a></li>`).join('') : '<li><span class="dropdown-item text-muted">İşlem yok</span></li>'}
                </ul>
            </div>

            <div class="dropdown d-inline-block ms-1">
                <button class="btn-erp-base btn-erp-cikti dropdown-toggle" type="button" data-bs-toggle="dropdown"><i class="fa-solid fa-print"></i> Çıktılar</button>
                <ul class="dropdown-menu shadow">
                    ${ciktiListesi.length > 0 ? ciktiListesi.map(c => `<li><a class="dropdown-item" href="#" onclick="${c.action}('${c.id || 0}')">${c.ad}</a></li>`).join('') : '<li><span class="dropdown-item text-muted">Rapor yok</span></li>'}
                </ul>
            </div>
        </div>`;

    var mobileHtml = `
        <div class="erp-toolbar-mobile">
            <div class="dropdown">
                <button class="btn btn-dark dropdown-toggle w-100 py-2" type="button" data-bs-toggle="dropdown"><i class="fa-solid fa-bars"></i> İşlemler</button>
                <ul class="dropdown-menu w-100 shadow border-0">
                    <li><a class="dropdown-item py-2" href="#" onclick="btnYeniClick()">Yeni</a></li>
                    <li><a class="dropdown-item py-2" href="#" onclick="btnKaydetClick()">Kaydet</a></li>
                    </ul>
            </div>
        </div>`;

    container.innerHTML = desktopHtml + mobileHtml;
}

function btnYeniClick() { } function btnKaydetClick() { } function btnGuncelleClick() { }


// -----------------------------------------------
// GRID Bulucu
// Sayfada hangi grid varsa otomatik bulur
// -----------------------------------------------
function erpGetActiveGrid() {
    return window.listeGrid ||
        window.uretimGrid ||
        window.uretimFisGrid ||
        window.sureGrid ||
        window.isEmriGrid ||
        window.stokGrid ||
        window.cariGrid ||
        null;
}

// -----------------------------------------------
// API POST – standart request wrapper
// -----------------------------------------------
function erpPost(url, data) {
    return fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    }).then(r => r.json());
}

function btnSilClick() {
    const grid = erpGetActiveGrid();

    if (!grid) {
        showToast("Liste bulunamadı.", "error");
        return;
    }

    const selected = grid.getCheckedRows();
    if (!selected.length) {
        showToast("Silmek için kayıt seçiniz.", "warning");
        return;
    }

    const evrakNolari = selected.map(r => r.evrakNo || r.EvrakNo);

    if (!confirm("Seçilen kayıtlar silinecek:\n\n" + evrakNolari.join(", "))) {
        return;
    }

    const path = window.location.pathname.toLowerCase();

    let apiUrl = null;

    if (path.includes("uretimfis")) apiUrl = "/UretimFisi/Sil";
    else if (path.includes("uretim")) apiUrl = "/Uretim/IsEmriSil";
    else if (path.includes("surekayitlari")) apiUrl = "/SureKayitlari/Sil";
    else if (path.includes("stok")) apiUrl = "/Stok/Sil";
    else if (path.includes("cari")) apiUrl = "/Cari/Sil";

    if (!apiUrl) {
        showToast("Bu sayfa için silme adresi bulunamadı.", "error");
        return;
    }

    erpPost(apiUrl, { evrakNolari })
        .then(res => {
            if (res.success) {
                showToast(res.message, "success");

                const mevcut = grid.getData();
                const kalan = mevcut.filter(
                    x => !evrakNolari.includes(x.evrakNo)
                );
                grid.resetData(kalan);
            } else {
                showToast(res.message, "error");
            }
        })
        .catch(() => showToast("Sunucu hatası.", "error"));

}

/**
 * ANA GRID OLUŞTURUCU
 */
function liste_gridi_olustur(targetId, data, columns, ayarlar) {
    var el = document.getElementById(targetId);
    if (!el) { console.error("Grid Hedefi Bulunamadı: " + targetId); return; }

    // Varsayılanlar
    var cfg = Object.assign({
        filtreleme: true,
        siralama: true,
        excel: true,
        temizle: true,
        sutunSecimi: true,
        secim: true,
        sabitKolonSayisi: 0,
        bodyHeight: 'fitToParent'
    }, ayarlar || {});

    /* ============================================================
       1) NO SÜTUNU EN BAŞA SADECE 1 KERE EKLENİYOR
    ============================================================ */
    if (cfg.satirNo === true) {
        columns.unshift({
            name: "_RowNum",
            header: "No",
            width: 60,
            align: "center",
            sortable: false
        });
    }

    /* ============================================================
       2) DİĞER SÜTUN AYARLARI
    ============================================================ */
    columns.forEach(function (sutun) {
        if (cfg.filtreleme === true && !sutun.filter) sutun.filter = 'select';
        if (cfg.siralama === true) sutun.sortable = true;

        var lowName = (sutun.name || "").toLowerCase();
        if (lowName.includes("tutar") || lowName.includes("fiyat") || lowName.includes("toplam") ||
            lowName.includes("miktar") || lowName.includes("borc")) {
            sutun.align = "right";
        }
    });

    /* ============================================================
       3) ÜST MİNİ TOOLBAR
    ============================================================ */
    var prevToolbar = el.parentElement.querySelector(".grid-tools-wrapper");
    if (prevToolbar) prevToolbar.remove();

    var toolsHtml = `
    <div class="grid-tools-wrapper d-flex justify-content-end align-items-center mb-2 gap-1">
        ${cfg.temizle ? `<button type="button" class="btn-erp-base btn-erp-icon-only" id="btn-grid-clear-${targetId}" title="Filtreleri Temizle"><i class="fa-solid fa-filter-circle-xmark"></i></button>` : ""}
        ${cfg.excel ? `<button type="button" class="btn-erp-base btn-erp-icon-only" id="btn-grid-excel-${targetId}" title="Excel"><i class="fa-solid fa-file-excel"></i></button>` : ""}
        ${cfg.sutunSecimi ? `<button type="button" class="btn-erp-base btn-erp-icon-only" id="btn-grid-columns-${targetId}" data-bs-toggle="modal" data-bs-target="#grid-column-modal-${targetId}" title="Sütun Ayarları"><i class="fa-solid fa-table-columns"></i></button>` : ""}
    </div>`;
    el.insertAdjacentHTML("beforebegin", toolsHtml);

    /* ============================================================
       4) GRID OLUŞTURMA
    ============================================================ */
    var rowHeaders = [];
    if (cfg.secim) rowHeaders.push("checkbox");  // rowNum KULLANMIYORUZ! Bug'a sebep olur.

    var grid = new tui.Grid({
        el: el,
        data: data || [],
        columns: columns,
        scrollX: true,
        scrollY: true,
        bodyHeight: cfg.bodyHeight,
        minBodyHeight: 200,
        rowHeaders: rowHeaders,
        columnOptions: {
            resizable: true,
            draggable: true,
            frozenCount: cfg.sabitKolonSayisi,
            minWidth: 120
        }
    });

    /* ============================================================
       5) TOOLBAR OLAYLARI
    ============================================================ */
    if (cfg.temizle === true) {
        document.getElementById("btn-grid-clear-" + targetId).addEventListener("click", function () {
            grid.getColumns().forEach(c => { if (c.filter) grid.unfilter(c.name); });
        });
    }

    if (cfg.excel === true) {
        document.getElementById("btn-grid-excel-" + targetId).addEventListener("click", function () {
            if (typeof XLSX === "undefined") { alert("Excel modülü eksik!"); return; }
            grid.export("xlsx", { fileName: "Liste_" + new Date().toISOString().slice(0, 10), useFormattedValue: true });
        });
    }

    if (cfg.sutunSecimi === true) {
        createGridColumnModal(grid, targetId);
    }

   
    

    return grid;
}


/**
 * SÜTUN AYARLARI MODALI (ORTALANMIŞ & BİZİM BUTONLAR)
 */
function createGridColumnModal(grid, targetId) {
    var oldModal = document.getElementById("grid-column-modal-" + targetId);
    if (oldModal && oldModal.parentNode) oldModal.parentNode.removeChild(oldModal);

    var cols = grid.getColumns().filter(c => !!c.header);
    var listItems = cols.map(c => {
        var checked = c.hidden ? "" : "checked";
        return `
            <li class="list-group-item d-flex align-items-center" draggable="true" data-col-name="${c.name}" style="cursor: move;">
                <span class="me-3 text-muted"><i class="fa-solid fa-grip-vertical"></i></span>
                <div class="form-check mb-0">
                    <input class="form-check-input col-vis-chk" type="checkbox" ${checked}>
                    <label class="form-check-label ms-2 fw-bold">${c.header}</label>
                </div>
            </li>`;
    }).join("");

    // modal-dialog-centered EKLENDİ (ORTALA)
    var modalHtml = `
        <div class="modal fade" id="grid-column-modal-${targetId}" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-sm">
                <div class="modal-content border-0 shadow">
                    <div class="modal-header border-bottom-0 pb-0">
                        <h6 class="modal-title fw-bold"><i class="fa-solid fa-table-columns me-2"></i>Sütun Ayarları</h6>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <p class="text-muted small mb-2">Sıralamak için sürükleyin.</p>
                        <ul class="list-group list-group-flush" id="sort-list-${targetId}">${listItems}</ul>
                    </div>
                    <div class="modal-footer border-top-0 pt-0">
                        <button type="button" class="btn-erp-base" data-bs-dismiss="modal"><i class="fa-solid fa-xmark"></i> Vazgeç</button>
                        <button type="button" class="btn-erp-base" id="btn-apply-${targetId}" >
                            <i class="fa-solid fa-check"></i> Uygula
                        </button>
                    </div>
                </div>
            </div>
        </div>`;

    document.body.insertAdjacentHTML("beforeend", modalHtml);

    var listEl = document.getElementById("sort-list-" + targetId);
    var draggedItem = null;

    listEl.addEventListener("dragstart", function (e) { draggedItem = e.target.closest("li"); });
    listEl.addEventListener("dragover", function (e) {
        e.preventDefault();
        var target = e.target.closest("li");
        if (target && target !== draggedItem) {
            var rect = target.getBoundingClientRect();
            var next = (e.clientY - rect.top) / (rect.bottom - rect.top) > 0.5;
            listEl.insertBefore(draggedItem, next ? target.nextSibling : target);
        }
    });

    document.getElementById("btn-apply-" + targetId).addEventListener("click", function () {
        var newColumns = [];
        var allColsMap = {};
        grid.getColumns().forEach(c => allColsMap[c.name] = c);

        listEl.querySelectorAll("li").forEach(li => {
            var name = li.getAttribute("data-col-name");
            var visible = li.querySelector("input").checked;
            var colDef = allColsMap[name];
            if (colDef) {
                colDef.hidden = !visible;
                newColumns.push(colDef);
            }
        });
        grid.setColumns(newColumns);

        var modalInstance = bootstrap.Modal.getInstance(document.getElementById("grid-column-modal-" + targetId));
        modalInstance.hide();
    });
}