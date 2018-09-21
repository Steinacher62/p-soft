/// <reference path="../Scripts/jquery-3.3.1.min.js" />
/// <reference path="../Javascript/jquery.json-2.2.min.js" />
///// <reference path="../Javascript/ig.ui.combo.js" />

$(document).ready(function () {
    // New Table
    $("#newTableButton").parent().remove();
    $("#copyTableButton").parent().remove();
    $("#newTableButton").hide();
    $("#copyTableButton").hide();
    $(".SupportFirstColumn .PlusButton").remove();
    $(".SupportFirstColumn .MinusButton").remove();
    
    
    // make  objective legend unchangeable
    $("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_1 td").removeClass("TitleChange");
    
});

$(document).ready(function () {
    // disable text change in cells
    $("#editModeButton").click(function () {

        //make description cell unchageable
        $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").css("border", "");
        $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").unbind();

        // make text cells unchangeable
        $(".CellTable tr.SupportRow>td.TextColumn").removeAttr("href").css("border", "");
        $(".CellTable tr.SupportRow>td.TextColumn").unbind();

        $(".CellTable tr.TextRow>td.TextColumn").removeAttr("href").css("border", "");
        $(".CellTable tr.TextRow>td.TextColumn").unbind();

        
    });
});
