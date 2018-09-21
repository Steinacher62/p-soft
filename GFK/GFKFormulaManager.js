/// <reference path="../Scripts/jquery-3.3.1.min.js" />
/// <reference path="../Javascript/jquery.json-2.2.min.js" />
var activecell;
$(document).ready(function () {
    
    $(".PlusButton").click(function () {
       activecell = $(this).parents(".MainCell")

    });
    $("#ContentPlaceHolder1__pl__cl__detail_FormelButton").click(function () {

        var rowId = $(activecell).attr("id").substr(44).split("_")[0];
        var url = document.URL.split("/Morph")[0] + "/GFK/FormelEditor.aspx?Row_ID=" + rowId;
        window.open(url);
    });
    $("#ContentPlaceHolder1__pl__cl__detail_FormelButtonCell").click(function () {

        var cellId = $(activecell).attr("id").substr(44).split("_")[1];
        if (cellId == "") {
            alert("Bitte Zelle beschriften");
        } else {
            var url = document.URL.split("/Morph")[0] + "/GFK/FormelEditorZelle.aspx?Cell_ID=" + cellId;
            window.open(url);
        }
    });
});