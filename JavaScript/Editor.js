function EditorLoaded(editor, args) {
    var clientWidth = $("#ContentPlaceHolder1__pl_baseLayoutCell")[0].clientWidth;
    var clientHeight = $("#ContentPlaceHolder1__pl_baseLayoutCell")[0].clientHeight;
    if (clientWidth > 1000) {
        $(".RadEditor")[0].style.width = "1200px";
    }
    else if (clientWidth < 641) {
        $(".RadEditor")[0].style.width = "640px";
    }
    else {
        $(".RadEditor")[0].style.width = clientWidth + "px";
    }

    if (clientHeight > 1000) {
        $(".RadEditor")[0].style.height = "1200px";
        $("#ctl00_ContentPlaceHolder1__pl__cl__edit_Input-THEME-DESCRIPTIONWrapper")[0].style.height = "1000px";
    }
    else if (clientHeight < 641) {
        $(".RadEditor")[0].style.height = "640px";
        $("#ctl00_ContentPlaceHolder1__pl__cl__edit_Input-THEME-DESCRIPTIONWrapper")[0].style.height = "600px";
    }
    else {
        $(".RadEditor")[0].style.height = clientHeight + "px";
        $("#ctl00_ContentPlaceHolder1__pl__cl__edit_Input-THEME-DESCRIPTIONWrapper")[0].style.height = clientHeight -20 + "px";
    }

    $("#ctl00_ContentPlaceHolder1__pl__cl__edit_Input-THEME-DESCRIPTION_contentIframe")[0].style.height = "100%";

}
