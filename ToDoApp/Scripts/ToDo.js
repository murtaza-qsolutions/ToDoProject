

$(function () {

    // ---------- DELETE ----------
    $(document).on("click", ".deletetab", function (e) {
        var $tab = $(this);
        var $btn = $tab.find("a[id*='lnkDelete']"); // find the asp:LinkButton inside

        if (!$tab.hasClass("confirm")) {
            // first click — show "SURE?"
            e.preventDefault();
            $tab.addClass("confirm").css({
                width: "44px",
                display: "block",
                right: "-64px"
            });
        } else  {
            $tab.removeClass("confirm").removeAttr("style");
        }
    
    });
});
