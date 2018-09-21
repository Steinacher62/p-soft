﻿(function () {

    var $;
    var uploadsInProgress = 0;
    var demo = window.demo = window.demo || {};

    demo.initialize = function () {
        $ = $telerik.$;
    };

    window.onFileSelected = function (sender, args) {
        if (!uploadsInProgress)
            $(".save-button").attr("disabled", "disabled");

        uploadsInProgress++;

        var row = args.get_row();

        $(row).addClass("file-row");
    }

    window.onFileUploaded = function (sender, args) {
        document.body.style.cursor = 'wait';
        document.getElementById("ContentPlaceHolder1_btnDummy").click();
    }

   

    window.decrementUploadsInProgress = function () {
        uploadsInProgress--;

        if (!uploadsInProgress)
            $("save-button").removeAttr("disabled");
    }
})();