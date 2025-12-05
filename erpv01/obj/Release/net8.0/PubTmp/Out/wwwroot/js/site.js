$(document).ready(function () {

    


    // 1. SIDEBAR AÇ/KAPA
    $('#sidebarToggle').on('click', function (e) {
        e.preventDefault();
        $('body').toggleClass('sb-sidenav-toggled');
    });

    // Mobilde dışarı tıklayınca kapat
    $(document).on('click', '#page-content-wrapper', function (e) {
        if ($('body').hasClass('sb-sidenav-toggled') && $(window).width() < 768) {
            $('body').removeClass('sb-sidenav-toggled');
        }
    });

    // 2. MENÜ ARAMA (Search)
    $("#menuSearchInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();

        if (value === "") {
            $(".sidebar-nav li").show();
            $(".sidebar-nav ul").slideUp(200); // Hafif animasyonla kapat
            $(".arrow-icon").css("transform", "rotate(0deg)");
            $("a[aria-expanded='true']").attr("aria-expanded", "false");
            return;
        }

        $(".sidebar-nav li").each(function () {
            var $li = $(this);
            // Hem title'dan hem text'ten ara
            var text = $li.find(".link-text").text().toLowerCase();

            var match = text.indexOf(value) > -1;

            if (match) {
                $li.show();
                $li.parents("ul").slideDown(200);
                $li.parents("li").show();
                var $parentLink = $li.parents("li").find("> a");
                $parentLink.attr("aria-expanded", "true");
                $parentLink.find(".arrow-icon").css("transform", "rotate(90deg)");
            } else {
                if ($li.find("li:visible").length === 0) {
                    $li.hide();
                }
            }
        });
    });

    // 3. ALT MENÜ AÇILIM (ACCORDION)
    $('.sidebar-nav a.has-submenu').on('click', function (e) {
        e.preventDefault();

        var $nextUl = $(this).next('ul');
        var $icon = $(this).find('.arrow-icon');

        // Diğer açık menüleri kapatmak istersen burayı açabilirsin (Opsiyonel)
        // $('.sidebar-nav ul').not($nextUl).slideUp();
        // $('.sidebar-nav a').not($(this)).find('.arrow-icon').css('transform', 'rotate(0deg)');

        if ($nextUl.is(':visible')) {
            $nextUl.slideUp(200);
            $icon.css('transform', 'rotate(0deg)');
            $(this).attr('aria-expanded', 'false');
        } else {
            $nextUl.slideDown(200);
            $icon.css('transform', 'rotate(90deg)');
            $(this).attr('aria-expanded', 'true');
        }
    });


    
});


function showToast(message, type = "info") {
    const toastEl = document.getElementById("globalToastItem");
    const toastMsg = document.getElementById("globalToastMessage");

    toastMsg.innerText = message;

    // Renkler
    let bg = "#0d6efd"; // info
    if (type === "success") bg = "#198754";
    if (type === "error") bg = "#dc3545";
    if (type === "warning") bg = "#ffc107";

    toastEl.style.backgroundColor = bg;
    toastEl.classList.add("show");

    const bsToast = new bootstrap.Toast(toastEl);
    bsToast.show();
}