/// <reference path="../Scripts/jquery-3.3.1.min.js" />
/// <reference path="jquery.json-2.2.min.js" />
/// <reference path="../scripts/svg.js" />
///// <reference path="ig.ui.combo.js" />
///// <reference path="jquery.svg.min.js" />
///// <reference path="jquery.svganim.min.js" />
///// <reference path="jquery.svgdom.min.js" />
///// <reference path="jquery.svgfilter.min.js" />
///// <reference path="jquery.svggraph.min.js" />
///// <reference path="jquery.svgplot.min.js" />
var activecell;
var svg;
var wirkungselementeArray;
var activeWp;
var permission;
var mapMax = false;
var activeKnowledgeId;

$(document).ready(function () {
    $(".RemoveWirkungspaket").hide();
    // AbbruchButton Funktion
    $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').click(function () {
        $(".WirkungselementBorder").removeClass("WirkungselementBorder");
        $('.ColorButton').show();
        $('.MainCell').unbind();
        $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').hide();
        $(".SelectionDiv").remove();
        SetWeButtons("ViewMode");
    });
    // add companyLogo
    var Logo = $("<img></img>");
    $(Logo).attr("id", "sokratesLogo");
    $(Logo).attr("src", "../images/Kundenlogo.png");
    $(Logo).appendTo($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3"));
    //add DaD images
    //var MoveImage = $("<img></img>");
    ////$(MoveImage).attr("id", "moveImage");
    //$(MoveImage).attr("src", "../images/move.png");
    //$(MoveImage)[0].style.position = 'absolute';
    //$(MoveImage).addClass('moveImage');
    //$(MoveImage)[0].style.visibility = 'hidden';
    //$(MoveImage).appendTo($('#content'));

    var InsertAfterImage = $("<img></img>");
    $(InsertAfterImage).attr("id", "insertAfterImage");
    $(InsertAfterImage).attr("src", "../images/InsertAfter.png");
    $(InsertAfterImage)[0].style.visibility = 'hidden';
    $(InsertAfterImage)[0].style.position = 'fixed';
    $(InsertAfterImage)[0].style.opacity = '0.5';
    $(InsertAfterImage).appendTo($('#content'));
    var InsertBeforImage = $("<img></img>");
    $(InsertBeforImage).attr("id", "insertBeforImage");
    $(InsertBeforImage).attr("src", "../images/InsertBefor.png");
    $(InsertBeforImage)[0].style.visibility = 'hidden';
    $(InsertBeforImage)[0].style.position = 'fixed';
    $(InsertBeforImage)[0].style.opacity = '0.5';
    $(InsertBeforImage).appendTo($('#content'));
    var InsertAboveImage = $("<img></img>");
    $(InsertAboveImage).attr("id", "insertAboveImage");
    $(InsertAboveImage).attr("src", "../images/InsertAbove.png");
    $(InsertAboveImage)[0].style.visibility = 'hidden';
    $(InsertAboveImage)[0].style.position = 'fixed';
    $(InsertAboveImage)[0].style.opacity = '0.5';
    $(InsertAboveImage).appendTo($('#content'));
    var InsertBelowImage = $("<img></img>");
    $(InsertBelowImage).attr("id", "insertBelowImage");
    $(InsertBelowImage).attr("src", "../images/InsertBelow.png");
    $(InsertBelowImage)[0].style.visibility = 'hidden';
    $(InsertBelowImage)[0].style.position = 'fixed';
    $(InsertBelowImage)[0].style.opacity = '0.5';
    $(InsertBelowImage).appendTo($('#content'));

    //Main menu-------
    $("#changeTableButton").click(function () {
        closeAllMenus();
        //set the height, top position
        var height = $("#changeTableButton").height();
        var top = $("#changeTableButton").offset().top;
        //set  the left position
        var left = $("#changeTableButton").offset().left;
        $('#ContentPlaceHolder1__pl__cl__detail_mainMenu').css({
            'left': left,
            'top': top
        });
        $('#ContentPlaceHolder1__pl__cl__detail_mainMenu').show("fast");
    });
    $("#mapMaxButton").click(function () {
        if (mapMax == false) {
            $("#banner").hide();
            $("#bannerBackground").hide()
            $("#logo").hide();
            $("#loginCell").hide();
            $("#breadcrumbCell").hide();
            $("#left").hide();
            $("#ContentPlaceHolder1__pl_titleRow").hide();
            $("#mapMaxButton")[0].attributes.title.value = "Karte verkleinern    (Ctrl + G";
            $("#mapMaxButton")[0].attributes.src.value = "../images/min_map.png";
            $("#mapMaxButton")[0].attributes.id.value = "mapMinButton";
            mapMax = true;
        }
        else {
            $("#banner").show();
            $("#bannerBackground").show();
            $("#logo").show();
            $("#loginCell").show();
            $("#breadcrumbCell").show();
            $("#left").show();
            $("#ContentPlaceHolder1__pl_titleRow").show();
            $("#mapMinButton")[0].attributes.title.value = "Karte maximieren    (Ctrl + G";
            $("#mapMinButton")[0].attributes.src.value = "../images/max_map.png";
            $("#mapMinButton")[0].attributes.id.value = "mapMaxButton";
            mapMax = false;
        }
    });
    $("#ContentPlaceHolder1__pl__cl__detail_mainMenu").mouseleave(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_mainMenu').hide("fast");
    });
    // shortcuts
    $(window).keydown(function (event) {
        if (event.ctrlKey && event.keyCode == 69) { // Ctrl + E Editmode
            event.preventDefault();
            $("#editModeButton").click();
        }
        if (event.ctrlKey && event.keyCode == 77) { // Ctrl + M Change Map
            $("#changeTableButton").click();
            event.preventDefault();
        }
    });
    // Color Menu---------
    $('.ColorButton').click(function () { //show colorationMenu
        showColorationMenu(this)
    });
    $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').mouseleave(function () { // hide
        $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').hide("fast");
    });
    $(".ColorMenuButton").click(function () { // change color
        $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').hide("fast");
        changecolor(activecell, $('#ContentPlaceHolder1__pl__cl__detail_colorMenu tr').index($(this).parents("tr")));
    });
    //Create SVG----------------
    //$('#content').svg();
    svg = SVG('content');
    $('svg').insertBefore($('#ContentPlaceHolder1__pl__cl__detail_MainTable'));
    //svg = $('#content').svg('get');
    //svg = SVG.get('content');
    $('svg').attr('pointer-events', 'none');
    //WirkungspaketButtons
    $(".WirkungspaketCell input").css("background-image", "url(../images/morph/Checkbox_Normal.png)");
    $(".WirkungspaketCell input").click(function () { toggleWirkungspaket(this); });
    // removewirkungspaket button
    $(".RemoveWirkungspaket").click(function (event) {
        removeWirkungspaket(this);
        event.stopPropagation();
    });
    //Load Wirkungselemente
    var params = {};
    params.matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
    wirkungselementeArray = callWebservice("loadWirkungselementArray", params);
    /*$.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/loadWirkungselementArray",
        cache: false,
        async: false,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            wirkungselementeArray = msg.d; //JSON object mit eigenschaft "d"
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });*/
    permission = $("#ContentPlaceHolder1__pl__cl__detail_Permission")[0].value;
    SetActiveWP($(".WirkungspaketCell")[0]);
    //Add Handler DaD Rows
    $(".RowCell0").toArray().forEach(function (row) {
        row.addEventListener('dragstart', RowCell0DragStart, false);
        row.addEventListener('dragend', RowCell0DragEnd, false);
        row.addEventListener('dragover', RowCell0DragOver, false);
        row.addEventListener('dragenter', RowCell0DragEnter, false);
        row.addEventListener('dragleave', RowCell0DragExit, false);
        row.addEventListener('drop', RowCell0Drop, false);
    });
    $(".RowCell1").toArray().forEach(function (row) {
        row.addEventListener('dragstart', RowCell1DragStart, false);
        row.addEventListener('dragend', RowCell1DragEnd, false);
        row.addEventListener('dragover', RowCell1DragOver, false);
        row.addEventListener('dragenter', RowCell1DragEnter, false);
        row.addEventListener('dragleave', RowCell1DragExit, false);
        row.addEventListener('drop', RowCell1Drop, false);
    });

    //Add Handler DaD Cells
    $(".RowCellDetail").toArray().forEach(function (cell) {
        cell.addEventListener('dragstart', RowCellDetailDragStart, false);
        cell.addEventListener('dragend', RowCellDetailDragEnd, false);
        cell.addEventListener('dragover', RowCellDetailDragOver, false);
        cell.addEventListener('dragenter', RowCellDetailDragEnter, false);
        cell.addEventListener('dragleave', RowCellDetailDragExit, false);
        cell.addEventListener('drop', RowCellDetailDrop, false);
    });
});
function SetActiveWP(wp) {
    if (permission == "WRITE") {
        $(".WirkungspaketCell").removeClass("ActiveWP");
        wp.classList.add("ActiveWP");
        activeWp = wp;
        SetWeButtons(editMode ? "EditMode" : "ViewMode");
    }
}
function SetWeButtons(mode) {
    if (mode == "ViewMode") {
        $('.TextRow .WsButtonAdd').removeClass("WsButtonAddInvisible").addClass("WsButtonAddVisible");
        $('.TextRow .WsButtonDel').removeClass("WsButtonDelVisible").addClass("WsButtonDelInvisible");
        if ($("#" + activeWp.id)[0].childNodes[0].attributes[3].nodeValue.match("Active") == "Active") {
            for (var i in wirkungselementeArray) {
                if (wirkungselementeArray[i].Wirkungspaket_ID == activeWp.id.substring(activeWp.id.lastIndexOf("_") + 1, activeWp.id.length)) {
                    for (var i1 = 0; i1 < $(".MainCell").length; i1++) {
                        var cellId = $(".MainCell")[i1].id.toString().substring($(".MainCell")[i1].id.lastIndexOf("_") + 1, $(".MainCell")[i1].id.toString().length);
                        if (cellId == wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID) {
                            $("#" + $(".MainCell")[i1].id + " td.SupportColumnAbove").find(".WsButtonDel").removeClass("WsButtonDelInvisible").addClass("WsButtonDelVisible");
                        }
                    }
                }
            }
        }
    }
    if (mode == "EditMode") {
        $('.TextRow .WsButtonAdd').removeClass("WsButtonAddVisible").addClass("WsButtonAddInvisible");
        $('.TextRow .WsButtonDel').removeClass("WsButtonDelVisible").addClass("WsButtonDelInvisible");
    }
}
function changecolor(actvieCell, newColor) {   //change Cell color
    //  $(actvieCell +" tr.TextRow td.TextColumn").removeClass().addClass("TextColumn").addClass("Color" + newColor).addClass("TextColumnAbove");

    var color = $('#ContentPlaceHolder1__pl__cl__detail_colorMenu tbody').children(":nth-child(" + (newColor + 1) + ")").children().children().css("background-color");

    if (color == "rgba(0, 0, 0, 0)" || color == "transparent") { // reset color
        if ($(activecell).parent().children().index($(activecell)) < 2) {
            $(actvieCell + " .CellTable").removeClass().addClass("CellTable SupportColorFirstColumn");
        } else {
            $(actvieCell + " .CellTable").removeClass().addClass("CellTable SupportColorDefault");
        }

        $(actvieCell).css("background-color", "");
    } else {
        $(actvieCell + " .CellTable").removeClass().addClass("CellTable SupportColor" + newColor);
        if ($(activecell).parent().children().index($(activecell)) < 2) {
            $(actvieCell).css("background-color", calculateFirstColumnColor(color));
        } else {
            $(actvieCell).css("background-color", color);
        }
    }
    var params = {};
    var coords = getCoords(activecell);
    params.MatrixId = window.location.search.split("=")[1];
    //$("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
    params.CellId = coords[1];
    params.NewColor = newColor;
    var ret = callWebservice("dbColorChange", params);
    updateLastChangeDate();
}
function calculateFirstColumnColor(color) {
    var newColor;
    //color to be mixed in and amount
    //if these values are changed they must also be changed in the c# on the server-side calculation (matrixDetailCtrl calculateColorFirstColumn())

    var transparency = 0.5;
    var backgroundColorRGB = [210, 210, 210];
    var colorRGB = color.substr(4, color.length - 5).split(",");
    var newColorRGB = [0, 0, 0];
    newColorRGB[0] = Math.round(backgroundColorRGB[0] * transparency + parseInt(colorRGB[0]) * (1 - transparency));
    newColorRGB[1] = Math.round(backgroundColorRGB[1] * transparency + parseInt(colorRGB[1]) * (1 - transparency));
    newColorRGB[2] = Math.round(backgroundColorRGB[2] * transparency + parseInt(colorRGB[2]) * (1 - transparency));
    var test = "rgb(" + newColorRGB[0] + ", " + newColorRGB[1] + ", " + newColorRGB[2] + ")";
    return test;
}
function showColorationMenu(cell) {
    closeAllMenus();
    activecell = $(cell).parents(".MainCell").attr('id');
    activecell = "#" + activecell;
    //set the height, top position
    var height = $(cell).height();
    var top = $(cell).offset().top;
    //set  the left position
    var left = $(cell).offset().left + 15;
    $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').show();
    if ($('#ContentPlaceHolder1__pl__cl__detail_colorMenu').outerHeight() + top > $(window).height() + $(window).scrollTop()) {
        $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').css({
            'top': $(window).height() + $(window).scrollTop() - $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').outerHeight() - 20
        });
    } else {
        $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').css({
            'top': top
        });
    }
    if ($('#ContentPlaceHolder1__pl__cl__detail_colorMenu').outerWidth() + left > $(window).width() + $(window).scrollLeft()) {
        $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').css({
            'left': left - $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').outerWidth() - 15
        });
    } else {
        $('#ContentPlaceHolder1__pl__cl__detail_colorMenu').css({
            'left': left
        });
    }
}
function colorMenuTextChange(index, newtext) {
    $("#ContentPlaceHolder1__pl__cl__detail_colorMenu").find(".ColorMenuButtonText").eq(index).text(newtext);
}
function closeAllMenus() {
    $("#ContentPlaceHolder1__pl__cl__detail_colorMenu").hide();
    $("#ContentPlaceHolder1__pl__cl__detail_minusMenu").hide();
    $("#ContentPlaceHolder1__pl__cl__detail_plusMenu").hide();
    $("#ContentPlaceHolder1__pl__cl__detail_mainMenu").hide();
    $("#ContentPlaceHolder1__pl__cl__detail_printMenu").hide();
}
//SWITCH MODE
var editMode;
$(document).ready(function () {
    editMode = false;
    //create plus/minus buttons
    $(".TextRow td.SupportColumn").each(function () {
        $(this).html("<a Class=\"MinusButton\" href=\"javascript:void(0);\"><img src=\"../images/morph/minus.gif\" border=\"0\"></a>" + "<a Class=\"PlusButton\" href=\"javascript:void(0);\"><img src=\"../images/morph/plus.gif\" border=\"0\"></a>" + $(this).html());
    });
    $(".PlusButton").hide();
    $(".MinusButton").hide();
    $("#editModeButton").click(function () {
        if (editMode == false) { //to editMode
            //            //remove colors
            //            $(".CellTable td").css("background-color", "transparent");
            //            $(".CellTable tr.TextRow>td.TextColumn").removeClass().addClass("TextColumn");
            //disable links
            $(".CellTable tr.TextRow>td.TextColumn").each(function () {
                if ($(this).children("a").length > 0) {
                    $(this).html($(this).children("a").html() + $(this).html());
                    $(this).attr("style", "color:green;");
                }
            });
            SetWeButtons("EditMode");
            $(".CellTable tr.TextRow>td.TextColumn a").hide();
            //ColorButton not invisible MSR
            //            $(".SupportRow td.SupportColumn ismg").toggle();
            //enable +/- buttons
            $(".PlusButton").show();
            $(".MinusButton").show();
            //show Trash Can Button for Wirkungspaket
            $(".RemoveWirkungspaket").show();
            //enable edit text
            $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
            $(".CellTable tr.TextRow>td.TextColumn").click(function () { createTextbox(this) });
            $(".CellTable tr.SupportRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
            $(".CellTable tr.SupportRow>td.TextColumn").click(function () { createTextInsert(this) });
            //$(".SupportColorFirstColumn tr.SupportRow>td.TextColumn").removeAttr("href").unbind().css("border", "");
            $(".TextColumn").css("position", "relative");
            $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").css("position", "relative");
            $(".TitleChange").attr("href", "javascript:void(0);").css("border", "1px solid gray");
            $(".TitleChange").click(function () { createTextInsert(this) });
            $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").attr("href", "javascript:void(0);").css("border", "1px solid gray");
            $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").click(function () { createDescriptionInsert(this) });
            //changeWirkungspaketmode();
            editMode = true;
            drawWirkungselemente();
            $(".RowCell0").attr("draggable", "true");
            //$(".RowCell1").attr("draggable", "true");
            $('.ColorButton').closest('.RowCellDetail').attr("draggable", "true");
            //if ($('[draggable = "true"]').find('.moveImage').length === 0) {
            //    $('.moveImage').appendTo($('[draggable = "true"]').find('.TextColumnBelow'));
            //}
            //$('.moveImage').css('visibility', 'visible');

        } else { //to viewMode
            $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').click();
            $("input").focusout();
            $("textarea").focusout();
            //enable links
            $(".CellTable tr.TextRow>td.TextColumn a").show();
            //ColorButton not invisible MSR
            //            $(".SupportRow td.SupportColumn img").toggle();
            //remove Buttons
            $(".PlusButton").hide();
            $(".MinusButton").hide();
            // remove Trash Can Wirkungselement
            $(".RemoveWirkungspaket").hide();
            //disable edit text
            $(".TitleChange").removeAttr("href").css("border", "");
            $(".TitleChange").unbind();
            $(".CellTable tr.SupportRow>td.TextColumn").removeAttr("href").css("border", "");
            $(".CellTable tr.SupportRow>td.TextColumn").unbind();
            $(".CellTable tr.TextRow>td.TextColumn").removeAttr("href").css("border", "");
            $(".CellTable tr.TextRow>td.TextColumn").unbind();
            $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").removeAttr("href").css("border", "");
            $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").unbind();
            $(".CellTable tr.TextRow>td.TextColumn").each(function () {
                if ($(this).children("a").length > 0) {
                    $(this).contents().filter(function () { return this.nodeType == 3; }).remove();
                    $(this).children("br").remove();
                }
            });
            // change Wirkungspaket Buttons
            $(".WirkungspaketCell input[type='button']").unbind();
            $(".WirkungspaketCell input[type='button']").click(function () { toggleWirkungspaket(this); });
            $("#ContentPlaceHolder1__pl__cl__detail_minusMenu").hide();
            $("#ContentPlaceHolder1__pl__cl__detail_plusMenu").hide();
            $(".TextColumn").css("position", "");
            $("#ContentPlaceHolder1__pl__cl__detail_DescriptionCell1").css("position", "");
            editMode = false;
            drawWirkungselemente();
            SetWeButtons("ViewMode");
            $(".RowCell0").attr("draggable", "false");
            $(".RowCell1").attr("draggable", "false");
            $(".RowCellDetail").attr("draggable", "false");
            //$('.moveImage').css('visibility', 'hidden');

        }
    });
});
function createTextbox(cell) {  // change text multiline
    closeAllMenus();
    if ($(cell).children(".InputBox").index() == 0) {
        $(".InputBox").focus();
    } else {
        $(".CellTable tr.TextRow>td.TextColumn").unbind();
        $(".CellTable tr.TextRow>td.TextColumn").removeAttr("href");
        $(cell).html("<textarea Class=\"InputBox\" rows=\"3\"  wrap=\"hard\"></textarea>" + $(cell).html());
        $(".CellTable tr.TextRow>td.TextColumn textarea").focus();
        $(".InputBox").keyup(function () {
            if ($(this).get(0).scrollHeight > 55) {
                while ($(this).get(0).scrollHeight > 55) {
                    $(".InputBox").val($(".InputBox").val().slice(0, -1));
                }
            }
            var activeElementId = $(this).get(0).parentElement.id;
            var matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            var text = $("#" + activeElementId)[0].children[0].value;
            //sendCellChanges(matrixId, activeElementId, text);
        });
        var zeile = 0;
        $(cell).contents().filter(function () { return this.nodeType == 3; }).each(function () {
            if ($(this).text() != " " && zeile == 0) {
                $(".InputBox").val($(".InputBox").val() + $(this).text());
            } else if ($(this).text() != " ") {
                $(".InputBox").val($(".InputBox").val() + "\n" + $(this).text());
            }
            zeile++;
            $(this).remove();
        });
        $(cell).children("br").remove();
        $(".CellTable tr.TextRow>td.TextColumn textarea").focusout(function () {
            var newText = $(".CellTable tr.TextRow>td.TextColumn textarea").val();
            changeText($(cell).parent().parent().parent(), newText);
        });
    }
}
function hasKnowledge(cell) {
    return $(cell).find(".TextColumn.TextColumnAbove").find("a").length > 0;
}
function hasSubMatrix(cell) {
    return ($(cell).find(".SupportColumn.SupportColumnAbove").find("a").length == 5 || $(cell).find(".SupportColumn.SupportColumnAbove").find("a").length == 3);
}
function changeText(cell, newText) {
    var sendText = newText.replace(/(\r\n|\r|\n)/g, "\n");
    var textCell = $(cell).find(".TextColumn.TextColumnAbove");
    newText = newText.replace(/\n/g, "<br>");
    if ($(textCell).find("a").length > 0) {
        $(textCell).find("a").html(newText);
        $(textCell).html(newText + $(textCell).html())
    } else {
        $(textCell).html(newText)
    }
    $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);");
    $(".CellTable tr.TextRow>td.TextColumn").click(function () { createTextbox(this) });
    if (sendText != "" && ifCellEmpty($(textCell).parents(".MainCell")) == true) {
        var index = getCoords($(textCell).parents(".MainCell").attr("Id"));
        var newId = getNewCellId(index[0], $(textCell).parents(".MainCell"));
        $(textCell).parents(".MainCell").attr("Id", $(textCell).parents(".MainCell").attr("Id") + newId);
        $("<a class=\"ColorButton\" href=\"javascript:void(0);\"><img style=\"display: inline;\" src=\"../images/pen.png\" border=\"0\"></a>").appendTo(
            $(textCell).parents(".CellTable").find(".SupportRow td.SupportColumn"));
        $(textCell).parents(".CellTable").find('.ColorButton').click(function () { showColorationMenu(this) });
        $("<a Class=\"WsButtonAdd WsButtonAddInvisible\" href=\"javascript:void(0);\"><img src=\"../images/CreateWe.png\" border=\"0\" ><br></img></a><a Class=\"WsButtonDel WsButtonDelInvisible\" href=\"javascript:void(0);\"><img src=\"../images/DeleteWe.png\" border=\"0\" ></img></a>").appendTo(
            $(textCell).parents(".CellTable").find(".TextRow td.SupportColumn"));
        $(textCell).parents(".CellTable").find('.WsButtonAdd').click(function () {
            closeAllMenus();
            activecell = $(this).parents(".MainCell")
            drawWirkungselementBorder(activecell)                                       //Wirkungselement hinzufügen
        });
        $(textCell).parents(".CellTable").find('.WsButtonDel').click(function () {
            closeAllMenus();
            activecell = $(this).parents(".MainCell")
            removeWirkungselementBorder(activecell)                                     //Wirkungselement entfernen
        });
        AddDaDEventListener($(textCell).parents(".RowCellDetail")[0]);
    }
    if (sendText == "" && getCoords($(textCell).parents(".MainCell").attr("Id")) != "" && $(textCell).parents(".CellTable").attr("class") != "CellTable SupportColorFirstColumn" && $(textCell).parents(".CellTable").find(".SupportRow > .TextColumn").text() == "") {
        var activeRow = $(textCell).parents(".MainCell").parents(".MainRow");
        index = getCoords($(textCell).parents(".MainCell").attr("Id"));
        if (index[1] != "") {
            var params = {};
            params.RowID = index[0];
            params.CellId = index[1];
            params.Index = 2;
            $(textCell).parents(".MainCell").prevAll(".MainCell").each(function () {
                if (ifCellEmpty(this) == false && $(this).children(".CellTable").attr("Class") != "CellTable SupportColorFirstColumn") {
                    params.Index++;
                }
            });
            callWebservice("dbDeleteCell", params);
            RemoveDaDEventListener($(textCell).parents(".RowCellDetail")[0]);
            removeWirkungselemte(index[1]);
            drawWirkungselemente();
            //sendCellDeleted($(cell).parents(".MainCell")[0].id);
            $(textCell).parents(".MainCell").remove();
            addCellAfter($(activeRow).children(".MainCell").filter(":last-child"), true);
        }
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').hide("fast");
    } else {
        var params = {};
        var coords = getCoords($(textCell).parents(".MainCell").attr("id"));
        params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        params.RowId = coords[0];
        params.CellId = coords[1];
        params.Line0 = "";
        params.Line1 = "";
        params.Line2 = "";
        if (sendText.length <= 30) {
            params.Line0 = sendText;
        } else {
            params.Line0 = sendText.substr(0, 30);
        }
        if (sendText.length > 30) {
            params.Line1 = sendText.substr(30, 30);
        }
        if (sendText.length > 60) {
            params.Line2 = sendText.substr(60, 25);
        }
        callWebservice("dbCellTextChange", params);
        $(".CellTable tr.TextRow>td.TextColumn textarea").remove();
        updateLastChangeDate();
    }
}
function removeWirkungselemte(cellId) {
    for (var i = 0; wirkungselementeArray.length > i; i++) {
        if (cellId == wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID || cellId == wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID) {
            removeWirkungselementConnection(wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID, wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID, wirkungselementeArray[i].Wirkungspaket_ID);
            i--;
        }
    }
}
function createTextInsert(cell) { // change text single line
    closeAllMenus();
    if ($(cell).children(".Input").index() == 0) {
        $(".Input").focus();
    } else {
        var text = $(cell).text();
        $(cell).unbind()
        $(cell).contents().filter(function () { return this.nodeType == 3; }).remove();
        $(cell).html($(cell).html() + "<input type=\"text\" size=\"10px\" Class=\"Input\"></input>");
        $(".Input").focus();
        $(".Input").val(text);
        $(".Input").keyup(function () {
            var div = $("<div>");
            $("#content").append(div);
            $(div).css({
                position: 'absolute',
                left: -1000,
                top: -1000,
                display: 'none'
            });
            $(div).html($(this).val());
            var styles = ['font-size', 'font-style', 'font-weight', 'font-family', 'line-height', 'text-transform', 'letter-spacing'];
            for (var i in styles) {
                var s = styles[i].toString();
                $(div).css(s, $(cell).css(s));
            }
            while ($(div).outerWidth() > $(this).innerWidth() - 4) {
                $(this).val($(this).val().slice(0, -1));
                $(div).html($(this).val());
            }
            $(div).remove();
        });
        $(".Input").keypress(function (event) {
            if (event.keyCode == 10 || event.keyCode == 13)
                event.preventDefault();
        });
        $(".Input").keyup(function (event) {
            var activeElementId = event.currentTarget.parentElement.id;
            var matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            var text = event.currentTarget.value;
            //sendTextChange(matrixId, activeElementId, text);
        });
        $(".Input").focusout(function () {
            text = $(".Input").val();
            changeOneLineText(cell, text);
        });
    }
}
// cell has to be td where text is located (in case of supportText in main cell SupportCell (.TextCellBelow)).
function changeOneLineText(cell, text) {
    $(".Input").remove();
    $(cell).click(function () { createTextInsert(this); });
    if ($(cell).attr("Class").indexOf("TitleChange") != -1) {
        var params = {};
        params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        switch ($(cell).attr("Id")) {
            case "ContentPlaceHolder1__pl__cl__detail_TitleRowCell1":
                if (text.replace(/ /g, '').length == 0) {
                    alert('Leere Titel der Karte können nicht gespeichert werden');
                    $('#ContentPlaceHolder1__pl__cl__detail_TitleRowCell1')[0].innerText = 'Titel';
                }
                else {
                    params.CellId = "Title";
                }
                break;
            case "ContentPlaceHolder1__pl__cl__detail_TitleRowCell6":
                params.CellId = "Subtitle";
                break;
            case "ContentPlaceHolder1__pl__cl__detail_TitleRowCell3":
                params.CellId = "Author";
                break;
            case "ContentPlaceHolder1__pl__cl__detail_TitleRowCell5":
                params.CellId = "Date";
                break;
            case "ContentPlaceHolder1__pl__cl__detail_TitleRowCell8":
                params.CellId = "LastChange";
                break;
            case "ContentPlaceHolder1__pl__cl__detail_TitleRowCell10":
                params.CellId = "Revision";
                break;
            default:
                params.CellId = $(cell).attr("Id");
                params.CellId = params.CellId.substring(36);
                break;
        }
        // Wp name auf standard setzten falls leer
        if (text == "" && params.CellId.toString().indexOf("Wirkungspakete") != -1) {
            text = "WP" + ($(".WirkungspaketCell").index($(cell)) + 1).toString();
        }
        // ColorMenu aktualisieren
        if (params.CellId.toString().indexOf("Legend") != -1) {
            var index = parseInt(params.CellId.substring(6).split("_")[0] - 1) * 2 + parseInt(params.CellId.substring(6).split("_")[1]);
            colorMenuTextChange(index, text);
        }
        params.Text = text;
    } else {
        if (text == "" && getCoords($(cell).parents(".MainCell").attr("Id"))[1] != "" && $(cell).parents(".CellTable").attr("class") != "CellTable SupportColorFirstColumn" && $(cell).parents(".CellTable").find(".TextRow > .TextColumn").text() == "") {
            var activeRow = $(cell).parents(".MainCell").parents(".MainRow");
            index = getCoords($(cell).parents(".MainCell").attr("Id"));
            if (index[1] != "") {
                var params = {};
                params.RowID = index[0];
                params.CellId = index[1];
                params.Index = getIndex($(cell).parents(".MainCell"));
                callWebservice("dbDeleteCell", params);
                RemoveDaDEventListener($(cell).parents(".RowCellDetail")[0]);
                for (var i = 0; wirkungselementeArray.length > i; i++) {
                    if (index[1] == wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID || index[1] == wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID) {
                        removeWirkungselementConnection(wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID, wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID, wirkungselementeArray[i].Wirkungspaket_ID);
                        i--;
                    }
                }
                drawWirkungselemente();
                $(cell).parents(".MainCell").remove();
                addCellAfter($(activeRow).children(".MainCell").filter(":last-child"), true);
            }
            $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').hide("fast");
        } else if (ifCellEmpty($(cell).parents(".MainCell")) == true && text != "") {
            var index = getCoords($(cell).parents(".MainCell").attr("Id"));
            var newId = getNewCellId(index[0], $(cell).parents(".MainCell"));
            $(cell).parents(".MainCell").attr("Id", $(cell).parents(".MainCell").attr("Id") + newId);
            $("<a class=\"ColorButton\" href=\"javascript:void(0);\"><img style=\"display: inline;\" src=\"../images/pen.png\" border=\"0\"></a>").appendTo(
                $(cell).parents(".CellTable").find(".SupportRow td.SupportColumn"));
            $(cell).parents(".CellTable").find('.ColorButton').click(function () { //show colorationMenu
                showColorationMenu(this)
            });
        }
        text = text.replace(/(\r\n|\n|\r)/gm, "");
        var params = {};
        var coords = getCoords($(cell).parents(".MainCell").attr("id"));
        params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        params.CellId = coords[1];
        params.Text = text;
    }
    $(cell).html($(cell).html() + text);
    $(".WirkungspaketCell input[type='button']").unbind();
    $(".WirkungspaketCell input[type='button']").click(function (event) {
        toggleWirkungspaket(this);
        event.stopPropagation();
    });
    //$(".RemoveWirkungspaket").unbind();
    $(cell).children(".RemoveWirkungspaket").click(function (event) {
        removeWirkungspaket(this);
        event.stopPropagation();
    });
    callWebservice("dbTextChange", params);
}
function createDescriptionInsert(cell) { // beschreibungstext ändern
    closeAllMenus();
    $(cell).unbind();
    $(cell).removeAttr("href");
    $(cell).html("<textarea Class=\"InputBox\" rows=\"5\" cols=\"40\" wrap=\"hard\"></textarea>" + $(cell).html());
    $(".InputBox").focus();
    //$(".InputBox").change(function () {
    //    if ($(this).get(0).scrollHeight > 77) {
    //        while ($(".InputBox").get(0).scrollHeight > 77) {
    //            $(".InputBox").val($(".InputBox").val().slice(0, -1));
    //        }
    //    }
    //});
    var zeile = 0;
    $(cell).contents().filter(function () { return this.nodeType == 3; }).each(function () {
        if ($(this).text() != " " && zeile == 0) {
            $(".InputBox").val($(".InputBox").val() + $(this).text());
        } else if ($(this).text() != " ") {
            $(".InputBox").val($(".InputBox").val() + "\n" + $(this).text());
        }
        zeile++;
        $(this).remove();
    });
    $(cell).children("br").remove();
    $(".InputBox").focusout(function () {
        var newText = $(".InputBox").val();
        var sendText = newText.replace(/(\r\n|\r|\n)/g, "\n");
        //newText = newText.replace(/\n/g, "<br>");
        $(cell).html(newText);
        var params = {};
        params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        params.Text = sendText;
        callWebservice("dbDescriptionChange", params);
        $(cell).attr("href", "javascript:void(0);");
        $(cell).click(function () { createDescriptionInsert(this) });
    });
}
//Plus Menu
$(document).ready(function () {
    $('.PlusButton').click(function () { //show plusMenu
        showPlusMenu(this);
    });
    $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').mouseleave(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
    });
    $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
    $("#ContentPlaceHolder1__pl__cl__detail_AddCellAfter").click(function () {
        addCellAfter(activecell, false);
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
        $(".TextColumn").css("position", "relative");
        //$('.MinusButton').click(function () {
        //    showMinusMenu(this);
        //});
        //$('.PlusButton').click(function () {
        //    showPlusMenu(this);
        //});
        //$(".WsButtonAdd").click(function () {
        //    closeAllMenus();
        //    activecell = $(this).parents(".MainCell")
        //    drawWirkungselementBorder(activecell)                                       //Wirkungselement hinzufügen
        //});
        //$(".WsButtonDel").click(function () {
        //    closeAllMenus();
        //    activecell = $(this).parents(".MainCell")
        //    removeWirkungselementBorder(activecell)                                     //Wirkungselement entfernen
        //});
    });
    $(".WsButtonAdd").click(function () {
        closeAllMenus();
        activecell = $(this).parents(".MainCell")
        drawWirkungselementBorder(activecell)                                       //Wirkungselement hinzufügen
    });
    $(".WsButtonDel").click(function () {
        closeAllMenus();
        activecell = $(this).parents(".MainCell")
        removeWirkungselementBorder(activecell)                                     //Wirkungselement entfernen
    });
    $("#ContentPlaceHolder1__pl__cl__detail_EditKnowledge").click(function () {

        var knowledgeId = $(activecell).find(".TextColumnAbove a").first().attr("knowledgeId");
        if ($("#ContentPlaceHolder1__pl__cl__detail_isSimpleKnowledge")[0].value == 'true') {
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1").show();
            knowledgeSimpleLoad(knowledgeId);
        } else {
            window.open(document.URL.split("Morph")[0] + "/Knowledge/KnowledgeDetail.aspx?knowledgeId=" + knowledgeId);
        }
        $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    });
    $("#ContentPlaceHolder1__pl__cl__detail_AddCellBefore").click(function () {
        addCellBefore(activecell, false);
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
        $(activecell).prev().find("tr.TextRow > td.TextColumn").click();
    });
    $("#ContentPlaceHolder1__pl__cl__detail_AddRowAfter").click(function () { // zeile unten einfügen
        var activeRow = $(activecell).parents(".MainRow");
        var rowspan = $(activeRow).children(":first-child").attr("rowspan");
        var index = $(activeRow).siblings(".MainRow").addBack().index($(activeRow)) + 1;
        var secondColActive = $(activeRow).children("td.MainCell").eq(1).css("display") != "none";

        if ($(activecell).parents(".MainRow").children().index(activecell) == 0 && rowspan != "1") {
            var newRow = createNewRow(activeRow, index + parseInt(rowspan) - 1);

            $(activeRow).addBack().nextAll().eq(parseInt(rowspan) - 2).after(newRow);

        }
        else if ($(activeRow).children("td.MainCell").first().css("display") == "none" || rowspan != "1") {
            var newRow = createNewRow(activeRow, index);
            $(activeRow).after(newRow);
            $(newRow).children().first().hide();
            while ($(activeRow).children("td:first-child").attr("rowspan") == "1") {
                activeRow = $(activeRow).prev();
            }
            mergeLowerCell($(activeRow).children("td:first-child"))
        } else {
            var newRow = createNewRow(activeRow, index);
            $(activeRow).after(newRow);
        }
        if (!secondColActive) {
            $(newRow).children("td.MainCell").eq(1).css("display", "none");
        }
        drawWirkungselemente();
        $("tr.TextRow > td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
        $("tr.TextRow > td.TextColumn").click(function () { createTextbox(this) });
        $(".CellTable tr.SupportRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
        $(".TextColumn").css("position", "relative");
        $(".CellTable tr.SupportRow>td.TextColumn").click(function () { createTextInsert(this) });
    });
    $("#ContentPlaceHolder1__pl__cl__detail_AddRowBefore").click(function () { // zeile oben einfügen
        var activeRow = $(activecell).parents(".MainRow");
        var index = $(activeRow).siblings(".MainRow").addBack().index($(activeRow));
        var rowspan = $(activeRow).children(":first-child").attr("rowspan");
        var newRow = createNewRow(activeRow, index);
        var secondColActive = $(activeRow).children("td.MainCell").eq(1).css("display") != "none";
        if (!secondColActive) {
            $(newRow).children("td.MainCell").eq(1).css("display", "none");
        }
        if ($(activecell).parents(".MainRow").children().index(activecell) == 0 && rowspan != "1") {
            $(activeRow).before(newRow);
        }
        else if ($(activeRow).children("td.MainCell").first().css("display") == "none") {
            $(activeRow).before(newRow);
            $(newRow).children().first().hide();
            while ($(activeRow).children("td:first-child").attr("rowspan") == "1") {
                activeRow = $(activeRow).prev();
            }
            mergeLowerCell($(activeRow).children("td:first-child"))
        } else {
            $(activeRow).before(newRow);
        }
        drawWirkungselemente();
        $("tr.TextRow > td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
        $("tr.TextRow > td.TextColumn").click(function () { createTextbox(this) });
        $(".CellTable tr.SupportRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
        $(".TextColumn").css("position", "relative");
        $(".CellTable tr.SupportRow>td.TextColumn").click(function () { createTextInsert(this) });
    });
    $("#ContentPlaceHolder1__pl__cl__detail_MergeLowerCell").click(function () { // mit unterer Zelle verbinden
        mergeLowerCell(activecell);
    });
    $("#ContentPlaceHolder1__pl__cl__detail_AddTitleColumn2").click(function () { // 2. titelspalte einfügen
        if (!isSecondColActive()) {
            $(".MainRow").each(function () {
                $(this).children().eq(1).show();
            });
            $("<td>").appendTo($(".TitleSupportRow"));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan")) + 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan")) + 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan")) + 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan")) + 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan")) + 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan")) + 1));
            $("tr.TextRow > td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
            $(".TextColumn").css("position", "relative");
            $("tr.TextRow > td.TextColumn").click(function () { createTextbox(this) });
            var params = {};
            params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            params.OnOff = 1;
            callWebservice("changeHelpColumn", params);
        }
        drawWirkungselemente();
    });
    $("#ContentPlaceHolder1__pl__cl__detail_SeparateCell").click(function () { // zelle aufspalten
        separateCell(activecell);
    });
});
function showPlusMenu(button) {
    closeAllMenus();
    activecell = $(button).parents(".MainCell")
    $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddCellAfter").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddCellBefore").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddNewKnowledge").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddKnowledge").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddSubmatrix").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddWirkungselement").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddRowAfter").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_AddRowBefore").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_MergeLowerCell").parent().hide();
    $("#ContentPlaceHolder1__pl__cl__detail_AddTitleColumn2").parent().hide();
    $("#ContentPlaceHolder1__pl__cl__detail_EditKnowledge").parent().hide();
    //not created cell
    if (getCoords((activecell).attr("id"))[1] == "") {
        $("#ContentPlaceHolder1__pl__cl__detail_AddNewKnowledge").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_AddKnowledge").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_AddSubmatrix").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_AddWirkungselement").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_MergeLowerCell").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_AddTitleColumn2").parent().hide();
    } else {
        if ($(activecell).children(":first-child").hasClass("SupportColorFirstColumn")) {
            //first 2 columns
            if ($(activecell).parent().children().index(activecell) == 0) {
                $("#ContentPlaceHolder1__pl__cl__detail_MergeLowerCell").parent().show();
                if (!isSecondColActive()) {
                    $("#ContentPlaceHolder1__pl__cl__detail_AddTitleColumn2").parent().show();
                } else {
                    $("#ContentPlaceHolder1__pl__cl__detail_AddCellAfter").parent().hide();
                }
            } else {
            }
            $("#ContentPlaceHolder1__pl__cl__detail_AddCellBefore").parent().hide();
        }
        if (hasKnowledge(activecell)) {

            $("#ContentPlaceHolder1__pl__cl__detail_EditKnowledge").parent().show();

            $("#ContentPlaceHolder1__pl__cl__detail_AddKnowledge").parent().hide();
            $("#ContentPlaceHolder1__pl__cl__detail_AddNewKnowledge").parent().hide();
        }
    }
    //set the height, top position
    var height = $(button).height();
    var top = button.offsetTop - 20;
    //set  the left position
    var left = $(button).offset().left + 10;
    $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').css({
        'left': 0,
        'top': 0
    });
    if ($('#ContentPlaceHolder1__pl__cl__detail_plusMenu').outerHeight() + top > $(window).height() + $(window).scrollTop()) {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').css({
            'top': $(window).height() + $(window).scrollTop() - $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').outerHeight() - 20
        });
    } else {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').css({
            'top': top
        });
    }
    if ($('#ContentPlaceHolder1__pl__cl__detail_plusMenu').outerWidth() + left > $(window).width() + $(window).scrollLeft()) {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').css({
            'left': left - $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').outerWidth() - 10
        });
    } else {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').css({
            'left': left
        });
    }
}
//Minus Menu
$(document).ready(function () {
    var del = false;
    $('.MinusButton').click(function () {
        showMinusMenu(this);
    });
    $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').mouseleave(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').hide("fast");
    });
    $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').hide("fast");
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveCell").click(function () {// zelle löschen
        var activeRow = $(activecell).parents(".MainRow");
        index = getCoords($(activecell).attr("Id"));
        if (index[1] != "") {
            var params = {};
            params.RowID = index[0];
            params.CellId = index[1];
            params.Index = getIndex(activecell);
        }
        params1 = {};
        params1.text = 'deleteCellConfirm';
        params1.scope = 'matrix';
        if (confirm(callWebservice("translate", params1))) {
            if (index[1] != "") {
                callWebservice("dbDeleteCell", params);
                RemoveDaDEventListener(activecell[0]);
                for (var i = 0; wirkungselementeArray.length > i; i++) {
                    if (index[1] == wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID || index[1] == wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID) {
                        removeWirkungselementConnection(wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID, wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID, wirkungselementeArray[i].Wirkungspaket_ID);
                        i--;
                    }
                }
                drawWirkungselemente();
            }
            $(activecell).remove();
            addCellAfter($(activeRow).children(".MainCell").filter(":last-child"), true);
        }
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').hide("fast");
    });
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveRow").click(function () {// zeile löschen
        params = {};
        params.text = 'deleteRowConfirm';
        params.scope = 'matrix';
        if (confirm(callWebservice("translate", params))) {
            removeRow(activecell);
            drawWirkungselemente();
        }
    });
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveTitleColumn2").click(function () { //2. Titelspalte entfernen
        if (confirm('Wollen Sie die 2. Titelspalte wirklich löschen?')) {
            $(".MainRow").each(function () {
                var secondCell = $(this).children().eq(1);
                $(secondCell).hide();
            });
            $(".MainRow").each(function () {
                var secondCell = $(this).children().eq(1);
                stripCell(secondCell);
            });
            $(".TitleSupportRow").children(":first-child").remove();
            $("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan")) - 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan")) - 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan")) - 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan")) - 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan")) - 1));
            $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan")) - 1));
            var params = {};
            params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            params.OnOff = 0;
            callWebservice("changeHelpColumn", params);
        }
        drawWirkungselemente();
    });
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveSubmatrix").click(function () {
        removeSubmatrix($(activecell));
    });
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveKnowledge").click(function () {
        removeKnowledge($(activecell));
        $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    });
});
function showMinusMenu(button) {
    closeAllMenus();
    activecell = $(button).parents(".MainCell")
    //set the height, top position
    var height = $(button).height();
    var top = $(button).offset().top - 30;
    //set  the left position
    var left = $(button).offset().left + 10;
    $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').show();
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveCell").show();
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveKnowledge").parent().hide();
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveSubmatrix").parent().hide();
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveWirkungselement").parent().hide();
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveTitleColumn2").parent().hide();
    $("#ContentPlaceHolder1__pl__cl__detail_RemoveRow").parent().show();
    $("#ContentPlaceHolder1__pl__cl__detail_SeparateCell").parent().hide();
    if (getCoords((activecell).attr("id"))[1] == "") {
        $("#ContentPlaceHolder1__pl__cl__detail_RemoveKnowledge").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_RemoveSubmatrix").parent().hide();
        $("#ContentPlaceHolder1__pl__cl__detail_RemoveWirkungselement").parent().hide();
    } else {
        if (hasKnowledge(activecell)) {
            $("#ContentPlaceHolder1__pl__cl__detail_RemoveKnowledge").parent().show();
        }
        if (hasSubMatrix(activecell)) {
            $("#ContentPlaceHolder1__pl__cl__detail_RemoveSubmatrix").parent().show();
        }
        if ($(activecell).children(":first-child").hasClass("SupportColorFirstColumn")) {
            //first 2 columns
            if ($(activecell).parent().children().index(activecell) == 0) {
                $("#ContentPlaceHolder1__pl__cl__detail_SeparateCell").parent().show();
            } else {
                $("#ContentPlaceHolder1__pl__cl__detail_RemoveTitleColumn2").parent().show();
            }
            $("#ContentPlaceHolder1__pl__cl__detail_RemoveCell").hide();
        }
    }
    $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').css({
        'left': 0,
        'top': 0
    });
    if ($('#ContentPlaceHolder1__pl__cl__detail_minusMenu').outerHeight() + top > $(window).height() + $(window).scrollTop()) {
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').css({
            'top': $(window).height() + $(window).scrollTop() - $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').outerHeight() - 20
        });
    } else {
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').css({
            'top': top
        });
    }
    if ($('#ContentPlaceHolder1__pl__cl__detail_minusMenu').outerWidth() + left > $(window).width() + $(window).scrollLeft()) {
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').css({
            'left': left - $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').outerWidth() - 10
        });
    } else {
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').css({
            'left': left
        });
    }
}
//zelle rechts einfügen
function addCellAfter(cell, fillTable) {
    var newCell = $("<td>")
    $(cell).after(newCell);
    var index = getCoords($(cell).attr("id"));
    $(newCell).attr("Id", "ContentPlaceHolder1__pl__cl__detail_MainCell" + index[0] + "_").attr("Class", "MainCell RowCellDetail");
    fillNewCell(newCell);
    lastCell = $(newCell).siblings().addBack().filter(":last-child");
    indexLastCell = getCoords($(lastCell).attr("Id"))
    if (indexLastCell[1] == "" && $(lastCell).parents(".MainRow").children(".MainCell").index($(lastCell)) != $(newCell).parents(".MainRow").children(".MainCell").index($(newCell))) {
        $(newCell).nextAll(":last-child").remove();
        $(newCell).find("tr.TextRow > td.TextColumn").click();
    } else if (fillTable == false) {
        $(newCell).parents(".MainRow").siblings(".MainRow").each(function () {
            if ($(this).children(".MainCell").length != $(newCell).parents(".MainRow").children(".MainCell").length) {
                addCellAfter($(this).children(":last-child"), true);
            }
        });
        $("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan")) + 1));
        $("<td>").appendTo($(".TitleSupportRow"));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan")) + 1));
        $(newCell).find("tr.TextRow > td.TextColumn").click();
    }
}
//zelle links einfügen
function addCellBefore(cell, fillTable) {
    var newCell = $("<td>")
    $(cell).before(newCell);
    var index = getCoords($(cell).attr("id"));
    $(newCell).attr("Id", "ContentPlaceHolder1__pl__cl__detail_MainCell" + index[0] + "_").attr("Class", "MainCell RowCellDetail");
    fillNewCell(newCell);
    lastCell = $(newCell).siblings().addBack().filter(":last-child");
    indexLastCell = getCoords($(lastCell).attr("Id"))
    if (indexLastCell[1] == "") {
        $(newCell).nextAll(":last-child").remove();
        $(newCell).find("tr.TextRow > td.TextColumn").click();
    } else if (fillTable == false) {
        $(newCell).parents(".MainRow").siblings(".MainRow").each(function () {
            if ($(this).children(".MainCell").length != $(newCell).parents(".MainRow").children(".MainCell").length) {
                addCellAfter($(this).children(":last-child"), true);
            }
        });
        $("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan")) + 1));
        $("<td>").appendTo($(".TitleSupportRow"));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan")) + 1));
        $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan")) + 1));
        $(newCell).find("tr.TextRow > td.TextColumn").click();
    }
}
// zelltabelle erstellen
function fillNewCell(newCell) {
    $("<tbody><tr class=\"TextRow\"><td href=\"javascript:void(0);\" class=\"TextColumn TextColumnAbove ColorDefault\"></td><td class=\"SupportColumn SupportColumnAbove SupportDefault\"><a style=\"display: inline;\" class=\"MinusButton\" href=\"javascript:void\"><img src=\"../images/morph/minus.gif\" type=\"image/svg+xml\" border=\"0\"></a><a style=\"display: inline;\" class=\"PlusButton\" href=\"javascript:void(0);\"><img src=\"../images/morph/plus.gif\" border=\"0\"></a></td></tr><tr class=\"SupportRow\"><td href=\"javascript:void(0);\" class=\"TextColumn SupportColumnAbove SupportDefault\"></td><td class=\"SupportColumn SupportColumnBelow SupportDefault\"></td></tr></tbody>").appendTo(
        $("<table>")
            .attr("Class", "CellTable SupportColorDefault")
            .appendTo(
                $(newCell)
            ));
    $(newCell).find("tr.TextRow > td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    $(newCell).find("tr.TextRow > td.TextColumn").click(function () { createTextbox(this) });
    $(newCell).find(".CellTable tr.SupportRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    $(newCell).find(".CellTable tr.SupportRow>td.TextColumn").click(function () { createTextInsert(this) });
    $(newCell).find(".TextColumn").css("position", "relative");
    $(newCell).find('.MinusButton').click(function () {
        showMinusMenu(this);
    });
    $(newCell).find('.PlusButton').click(function () {
        showPlusMenu(this);
    });
}
function removeWirkungselementBorder(cell) {
    $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').show();
    $('.ColorButton').hide();
    $('.TextRow > .TextColumn').unbind();
    $('.SupportRow > .TextColumn').unbind();
    $(".PlusButton").hide();
    $(".MinusButton").hide();
    SetWeButtons("EditMode");
    $(cell).addClass("WirkungselementBorder");
    var hasGoal = false;
    for (var i in wirkungselementeArray) {
        if (wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID == getCoords($(activecell).attr("Id"))[1] && wirkungselementeArray[i].Wirkungspaket_ID == $(".ActiveWP").attr("id").toString().substring(36).split("_")[1]) {
            $(".MainCell").each(function () {
                if (wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID == getCoords($(this).attr("Id"))[1]) {
                    hasGoal = true;
                    $(this).addClass("WirkungselementBorder");
                    $(this).prepend("<div></div>");
                    $(this).children("div").addClass("SelectionDiv");
                    $(this).children(".SelectionDiv").click(function () {
                        removeWirkungselementConnection(getCoords($(activecell).attr("Id"))[1], getCoords($(this).parent().attr("Id"))[1], $(".ActiveWP").attr("id").toString().substring(36).split("_")[1]);
                        $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').click();
                        drawWirkungselemente();
                    });
                }
            });
        }
    }
    if (!hasGoal) {
        $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').click();
    }
}
function removeWirkungselementConnection(cell1_Id, cell2_Id, wirkungsPaketId) {
    for (var i in wirkungselementeArray) {
        if (cell1_Id == wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID && cell2_Id == wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID && wirkungsPaketId == wirkungselementeArray[i].Wirkungspaket_ID) {
            wirkungselementeArray.splice(i, 1);
            var params = {};
            params.cellId1 = cell1_Id;
            params.cellId2 = cell2_Id;
            params.wirkungspaketId = wirkungsPaketId;
            callWebservice("removeWirkungselement", params);
        }
    }
}
function drawWirkungselementBorder(cell) {
    if ($("#" + activeWp.id)[0].childNodes[0].attributes[3].nodeValue.match("Active") == null) {
        $("#" + activeWp.id)[0].childNodes[0].click();
    }
    $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').show();
    $('.ColorButton').hide();
    $('.TextRow > .TextColumn').unbind();
    $('.SupportRow > .TextColumn').unbind();
    $(cell).addClass("WirkungselementBorder");
    $(".MainCell").prepend("<div></div>");
    $(".MainCell > div").addClass("SelectionDiv");
    SetWeButtons("EditMode");
    $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
    //$('.TextRow > .SupportColumn').children().hide();
    var isWirkungselement = false;
    $('.SelectionDiv').on('mouseover', function () {
        var cell = $(this).parent();
        if ($(cell).attr('Class') != "MainCell WirkungselementBorder") {
            isWirkungselement = true;
            $(cell).addClass("WirkungselementBorder");
            $(".SelectionDiv").mouseleave(function () {
                $(this).parent().removeClass("WirkungselementBorder");
            });
        }
        else {
            isWirkungselement = false;
        }
    });
    $('.SelectionDiv').click(function () {
        var cell = $(this).parent();
        $(".WirkungselementBorder").removeClass("WirkungselementBorder");
        $('.SelectionDiv').unbind();
        var existsAlready = false;
        for (var i in wirkungselementeArray) {
            if (wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID == getCoords($(activecell).attr("Id"))[1] && wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID == getCoords($(cell).attr("Id"))[1] && wirkungselementeArray[i].Wirkungspaket_ID == $(".ActiveWP").attr("id").toString().substring(36).split("_")[1]) {
                existsAlready = true;
            }
        }
        if (!existsAlready) {
            var params = {};
            params.cellId1 = getCoords($(activecell).attr("Id"))[1];
            params.cellId2 = getCoords($(cell).attr("Id"))[1];
            params.wirkungspaketId = activeWp.id.toString().substring(36).split("_")[1];
            var formData = $.toJSON(params);
            var newWirkungselementId;
            $.ajax({
                type: "POST",
                url: "../WebService/SokratesService.asmx/addWirkungselement",
                cache: false,
                async: false,
                data: formData,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    newWirkungselementId = msg.d; //JSON object mit eigenschaft "d"
                },
                error: function (result, errortype, exceptionobject) {
                    alert('Error:' + result.responseText);
                }
            });
            var newestWirkungselement = {};
            newestWirkungselement.id = newWirkungselementId;
            newestWirkungselement.ORIGIN_CHARACTERISTIC_ID = params.cellId1;
            newestWirkungselement.GOAL_CHARACTERISTIC_ID = params.cellId2;
            newestWirkungselement.Wirkungspaket_ID = params.wirkungspaketId;
            $("svg").css("height", Math.max($("svg").css("height").split("p")[0], $(activecell).offset().top, $(cell).offset().top));
            wirkungselementeArray.push(newestWirkungselement);
        }
        SetWeButtons("ViewMode");
        $('.ColorButton').show();
        $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').hide();
        $('#ContentPlaceHolder1__pl__cl__detail_AbbruchButton').click();
        drawWirkungselemente();
    }
    );
    $(".MainCell").each(function () {
        var coords = getCoords($(this).attr("id"));
        if (coords[1] == "Title1" || coords[1] == "Title0" || coords[1] == "") {
            $(this).children("div .SelectionDiv").remove();
        }
    });
    $('.PlusMenu').hide();
    $(cell).children(".SelectionDiv").remove();
}
function drawWirkungselementConnection(cell1, cell2, wirkungspaketId, cellCoordinates) {
    var origin;
    var goal;
    var translation = cellCoordinates.translation;
    var strokeWidth = 3;
    var translationValue = 5;
    var margin = 20;
    var y1 = $(cell1).offset().top;
    var x1 = $(cell1).offset().left;
    var dy1 = $(cell1).height();
    var dx1 = $(cell1).width();
    var y2 = $(cell2).offset().top;
    var x2 = $(cell2).offset().left;
    var dy2 = $(cell2).height();
    var dx2 = $(cell2).width();
    var cell1_p = new Array(8);
    for (var i = 0; i < cell1_p.length; ++i) {
        cell1_p[i] = new Array(2);
    }
    cell1_p[0][0] = x1 + margin;
    cell1_p[0][1] = y1 + margin;
    cell1_p[1][0] = x1 + dx1 / 2;
    cell1_p[1][1] = y1 + margin;
    cell1_p[2][0] = x1 + dx1 - margin;
    cell1_p[2][1] = y1 + margin;
    cell1_p[3][0] = x1 + dx1 - margin;
    cell1_p[3][1] = y1 + dy1 / 2;
    cell1_p[4][0] = x1 + dx1 - margin;
    cell1_p[4][1] = y1 + dy1 - margin;
    cell1_p[5][0] = x1 + dx1 / 2;
    cell1_p[5][1] = y1 + dy1 - margin;
    cell1_p[6][0] = x1 + margin;
    cell1_p[6][1] = y1 + dy1 - margin;
    cell1_p[7][0] = x1 + margin;
    cell1_p[7][1] = y1 + dy1 / 2;
    var cell2_p = new Array(8);
    for (var i = 0; i < cell2_p.length; ++i) {
        cell2_p[i] = new Array(2)
    }
    cell2_p[0][0] = x2 + margin;
    cell2_p[0][1] = y2 + margin;
    cell2_p[1][0] = x2 + dx2 / 2;
    cell2_p[1][1] = y2 + margin;
    cell2_p[2][0] = x2 + dx2 - margin;
    cell2_p[2][1] = y2 + margin;
    cell2_p[3][0] = x2 + dx2 - margin;
    cell2_p[3][1] = y2 + dy2 / 2;
    cell2_p[4][0] = x2 + dx2 - margin;
    cell2_p[4][1] = y2 + dy2 - margin;
    cell2_p[5][0] = x2 + dx2 / 2;
    cell2_p[5][1] = y2 + dy2 - margin;
    cell2_p[6][0] = x2 + margin;
    cell2_p[6][1] = y2 + dy2 - margin;
    cell2_p[7][0] = x2 + margin;
    cell2_p[7][1] = y2 + dy2 / 2;
    if (cellCoordinates.xo == cellCoordinates.xg || Math.abs((cellCoordinates.yg - cellCoordinates.yo) / (cellCoordinates.xg - cellCoordinates.xo)) > 2) {
        if (cellCoordinates.yg > cellCoordinates.yo) {
            goal = 1;
            origin = 5;
        } else {
            goal = 5;
            origin = 1;
        }
    } else if (cellCoordinates.yo == cellCoordinates.yg || Math.abs((cellCoordinates.yg - cellCoordinates.yo) / (cellCoordinates.xg - cellCoordinates.xo)) < 0.5) {
        if (cellCoordinates.xo < cellCoordinates.xg) {
            goal = 7;
            origin = 3;
        } else {
            goal = 3;
            origin = 7;
        }
    } else if (cellCoordinates.xo < cellCoordinates.xg) { // nach rechts ..
        if (cellCoordinates.yg > cellCoordinates.yo) { // ..unten
            goal = 0;
            origin = 4;
        } else { // ..oben
            goal = 6;
            origin = 2;
        }
    } else { // nach links..
        if (cellCoordinates.yg > cellCoordinates.yo) { //..unten
            goal = 2;
            origin = 6;
        } else { // ..oben
            goal = 4;
            origin = 0;
        }
    }
    var xo = cell1_p[origin][0] - $('#ContentPlaceHolder1__pl__cl__detail_MainTable').offset().left;  // xo = x- Wert von Origin
    var yo = cell1_p[origin][1] - $('#ContentPlaceHolder1__pl__cl__detail_MainTable').offset().top;   // yo = y- Wert von Origin
    var xg = cell2_p[goal][0] - $('#ContentPlaceHolder1__pl__cl__detail_MainTable').offset().left;    // xg = x- Wert von Goal
    var yg = cell2_p[goal][1] - $('#ContentPlaceHolder1__pl__cl__detail_MainTable').offset().top;     // yg = y- wert von Goal
    var vektorX = (xg - xo);
    var vektorY = yg - yo;
    var vektorLength = Math.sqrt(Math.pow(vektorX, 2) + Math.pow(vektorY, 2));
    var senkrechterVektorX = (0 - vektorY) / vektorLength;
    var senkrechterVektorY = vektorX / vektorLength;
    vektorX = vektorX / vektorLength;
    vektorY = vektorY / vektorLength;
    var translationValueX = Math.abs(translationValue * senkrechterVektorX) + Math.abs(senkrechterVektorY * translationValue / vektorY * vektorX);
    var translationValueY = Math.abs(translationValue * senkrechterVektorY) + Math.abs(senkrechterVektorX * translationValue / vektorX * vektorY);
    if (origin == 3 || origin == 7) { // waagerecht
        if (translation % 2 == 0) {
            yg = yg + translationValueY * translation / 2;
            yo = yo + translationValueY * translation / 2;
        } else {
            yg = yg - translationValueY * (translation + 1) / 2;
            yo = yo - translationValueY * (translation + 1) / 2;
        }
    } else if (origin == 1 || origin == 5) { // senkrecht
        if (translation % 2 == 0) {
            xg = xg + translationValueX * translation / 2;
            xo = xo + translationValueX * translation / 2;
        } else {
            xg = xg - translationValueX * (translation + 1) / 2;
            xo = xo - translationValueX * (translation + 1) / 2;
        }
    } else if (origin == 6 && goal == 2) { // nach unten links | vektorY / vektorX < 0 && vektorY < 0
        if (translation % 2 == 0) {
            xg = xg - translationValueX * translation / 2;
            yo = yo - translationValueY * translation / 2;
        } else {
            yg = yg + translationValueY * (translation + 1) / 2;
            xo = xo + translationValueX * (translation + 1) / 2;
        }
    } else if (origin == 2 && goal == 6) { // nach oben rechts | vektorY / vektorX < 0 && vektorY > 0
        if (translation % 2 == 0) {
            yg = yg - translationValueY * translation / 2;
            xo = xo - translationValueX * translation / 2;
        } else {
            xg = xg + translationValueX * (translation + 1) / 2;
            yo = yo + translationValueY * (translation + 1) / 2;
        }
    } else if (origin == 0 && goal == 4) { // nach oben links | vektorY / vektorX > 0 && vektorY < 0
        if (translation % 2 == 0) {
            yo = yo + translationValueY * translation / 2;
            xg = xg - translationValueX * translation / 2;
        } else {
            xo = xo + translationValueX * (translation + 1) / 2;
            yg = yg - translationValueY * (translation + 1) / 2;
        }
    } else if (origin == 4 && goal == 0) { // nach unten rechts | vektorY / vektorX > 0 && vektorY > 0
        if (translation % 2 == 0) {
            yg = yg + translationValueY * translation / 2;
            xo = xo - translationValueX * translation / 2;
        } else {
            xg = xg + translationValueX * (translation + 1) / 2;
            yo = yo - translationValueY * (translation + 1) / 2;
        }
    }
    xg = xg - vektorX * 15;
    yg = yg - vektorY * 15;
    var color = $("#ContentPlaceHolder1__pl__cl__detail_Wirkungspakete_" + wirkungspaketId + " span").css("background-color");
    var line = svg.line(xo, yo, xg, yg);
    line.attr({ 'fill': 'none', 'stroke': color, 'stroke-width': strokeWidth, 'stroke-opacity': ' 0.7' });
    var polygon = svg.polygon([[xg + (strokeWidth + 2) * senkrechterVektorX, yg + (strokeWidth + 2) * senkrechterVektorY], [xg + 15 * vektorX, yg + 15 * vektorY], [xg - (strokeWidth + 2) * senkrechterVektorX, yg - (strokeWidth + 2) * senkrechterVektorY]]);
    polygon.attr({ fill: "#555555", 'fill-opacity': "0.7", stroke: "none" });
}
function getNewCellId(RowId, Cell) {
    var params = {};
    params.RowID = RowId;
    params.Index = getIndex(Cell);
    var formData = $.toJSON(params);
    var ret;
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/dbAddCell",
        cache: false,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            ret = msg.d; //JSON object mit eigenschaft "d"
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
    return ret;
}
// zeile erstellen
function createNewRow(activeRow, index) {
    var Cells;
    var firstColumns = $(activeRow).find(".CellTable.SupportColorFirstColumn").length;
    Cells = $(activeRow).children(".MainCell").length - firstColumns;
    var newRow = $("<tr>").attr("Class", "MainRow");
    var newCell = $("<td>")
    var params = {};
    params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
    params.Index = index;
    var formData = $.toJSON(params);
    var ret;
    // get new rowId
    var rowId;
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/dbAddRow",
        cache: false,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            rowId = msg.d; //JSON object mit eigenschaft "d"
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
    $(newCell).attr("Id", "ContentPlaceHolder1__pl__cl__detail_MainCell" + rowId + "_").attr("Class", "MainCell RowCell0").attr("rowspan", 1).attr("draggable", "true");
    AddDaDEventListener(newCell[0]);
    $("<tbody><tr class=\"TextRow\"><td href=\"javascript:void(0);\" class=\"TextColumn TextColumnAbove ColorFirstColumn\"></td><td class=\"SupportColumn SupportColumnAbove SupportFirstColumn\"><a class=\"MinusButton\" href=\"javascript:void(0);\" ><img src=\"../images/morph/minus.gif\"></a><a class=\"PlusButton\" href=\"javascript:void(0);\"><img src=\"../images/morph/plus.gif\" ></a><a class=\"WsButtonDel WsButtonDelInvisible\" href=\"javascript:void(0);\"><img src=\"../images/DeleteWe.png\" ><br></a><a class=\"WsButtonAdd WsButtonAddInvisible\" href=\"javascript:void(0);\"><img src=\"../images/CreateWe.png\"></a></td></tr><tr class=\"SupportRow\"><td href=\"javascript:void(0);\" class=\"TextColumn TextColumnBelow SupportFirstColumn\"></td><td class=\"SupportColumn SupportColumnBelow SupportFirstColumn\"><a Class=\"ColorButton\" href=\"javascript:void(0);\"><img src=\"../images/pen.png\" border=\"0\"></img></a></td></tr></tbody>").appendTo(
        $("<table>")
            .attr("Class", "CellTable SupportColorFirstColumn")
            .appendTo(
                $(newCell)
            ));
    $(newCell).find('.MinusButton').click(function () {
        showMinusMenu(this);
    });
    $(newCell).find('.PlusButton').click(function () {
        showPlusMenu(this);
    });
    $(newCell).find('.ColorButton').click(function () { showColorationMenu(this) });
    $(newCell).appendTo($(newRow));
    var cellID = getNewCellId(rowId, newCell);
    $(newCell).attr("Id", $(newCell).attr("Id") + cellID);
    newCell = $("<td>")
    $(newCell).attr("Id", "ContentPlaceHolder1__pl__cl__detail_MainCell" + rowId + "_").attr("Class", "MainCell RowCell1").attr("draggable", "true");
    AddDaDEventListener(newCell[0]);
    $("<tbody><tr class=\"TextRow\"><td href=\"javascript:void(0);\" class=\"TextColumn TextColumnAbove ColorFirstColumn\"></td><td class=\"SupportColumn SupportColumnAbove SupportFirstColumn\"><a class=\"MinusButton\" href=\"javascript:void(0);\" ><img src=\"../images/morph/minus.gif\"></a><a class=\"PlusButton\" href=\"javascript:void(0);\"><img src=\"../images/morph/plus.gif\" ></a><a class=\"WsButtonDel WsButtonDelInvisible\" href=\"javascript:void(0);\"><img src=\"../images/DeleteWe.png\" ><br></a><a class=\"WsButtonAdd WsButtonAddInvisible\" href=\"javascript:void(0);\"><img src=\"../images/CreateWe.png\"></a></td></tr><tr class=\"SupportRow\"><td href=\"javascript:void(0);\" class=\"TextColumn TextColumnBelow SupportFirstColumn\"></td><td class=\"SupportColumn SupportColumnBelow SupportFirstColumn\"><a Class=\"ColorButton\" href=\"javascript:void(0);\"><img src=\"../images/pen.png\" border=\"0\"></img></a></td></tr></tbody>").appendTo(
        $("<table>")
            .attr("Class", "CellTable SupportColorFirstColumn")
            .appendTo(
                $(newCell)
            ));
    $(newCell).find('.MinusButton').click(function () {
        showMinusMenu(this);
    });
    $(newCell).find('.PlusButton').click(function () {
        showPlusMenu(this);
    });
    $(newCell).find('.ColorButton').click(function () { showColorationMenu(this) });
    $(newCell).appendTo($(newRow));
    var cellID = getNewCellId(rowId, newCell);
    $(newCell).attr("Id", $(newCell).attr("Id") + cellID);
    for (var i = 0; i < Cells; i++) {
        addCellAfter($(newRow).children(":last-child"), true);
    }
    $("svg").css("height", $("svg").css("height").split("p")[0] + 80);
    return newRow;
}
// zeile löschen
function removeRow(cell) {
    cell = $(cell).parent(".MainRow").children(".MainCell").first();
    otherCell = cell;
    if ($(cell).css("display") == "none") {
        row = $(cell).parent(".MainRow");
        while ($(otherCell).css("display") == "none") {
            row = $(row).parent().children()[$(row).parent().children().index(row) - 1];
            otherCell = $(row).children(".MainCell").first();
        }
    }
    var rowspan = $(otherCell).attr("rowspan");
    if (rowspan == 1) {
        var index = getCoords($(otherCell).attr("Id"));
        var params = {};
        params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        params.RowId = index[0];
        params.Index = $(cell).parents(".MainRow").siblings(".MainRow").addBack().index($(cell).parents(".MainRow"));
        callWebservice("dbDeleteRow", params);
        $(activecell).parents(".MainRow").remove();
        $('#ContentPlaceHolder1__pl__cl__detail_minusMenu').hide();
    } else {
        separateCell($(otherCell));
        $(cell).parents(".MainRow").nextAll().filter(":lt(" + (rowspan - 1) + ")").each(function () {
            $(this).children(".MainCell").first().show();
            //var index = getCoords($(this).children("td[Id*=Title0]").attr("Id"));
            //var params = {};
            //params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            //params.RowId = index[0];
            //params.Index = $(this).siblings(".MainRow").addBack().index($(this));
            //callWebservice("dbDeleteRow", params);
            //$(this).remove();
        });
        removeRow($(cell));
    }
}
// zelle in titelspalte verbinden
function mergeLowerCell(cell) {
    var rowspan = parseInt($(cell).attr("rowspan"));
    if ($(cell).parents(".MainRow").nextAll(".MainRow").filter(":eq(" + (rowspan - 1) + ")").children(":first-child").length == 1) {
        if (parseInt($(cell).parents(".MainRow").nextAll(".MainRow").filter(":eq(" + (rowspan - 1) + ")").children(":first-child").attr("rowspan")) > 1) {
            separateCell($(cell).parents(".MainRow").nextAll(".MainRow").filter(":eq(" + (rowspan - 1) + ")").children(":first-child"));
        }
        $(cell).attr("rowspan", rowspan + 1).children(":first-child").css("height", ((rowspan + 1) * 80 + rowspan * 5) + "px");
        $(cell).parents(".MainRow").nextAll(".MainRow").filter(":eq(" + (rowspan - 1) + ")").children(":first-child").each(function () {
            $(this).hide();
            stripCell(this);
        });
        //rowSpan(string DimensionId, string rows)
        var params = {};
        var coords = getCoords($(cell).attr("Id"));
        params.DimensionId = coords[0];
        params.rows = rowspan + 1;
        callWebservice("rowspan", params);
        drawWirkungselemente();
    }
}
// zelle in titelspalte auftrennen
function separateCell(cell) {
    var rowspan = parseInt($(cell).attr("rowspan"));
    if (rowspan > 1) {
        $(cell).attr("rowspan", 1).children(":first-child").css("height", "80px");
        var params = {};
        var coords = getCoords($(cell).attr("Id"));
        params.DimensionId = coords[0];
        params.rows = 1;
        callWebservice("rowspan", params);
        $(cell).parents(".MainRow").nextAll().filter(":lt(" + (rowspan - 1) + ")").each(function () {
            $(this).children("td").first().show();
        });
    }
    $("tr.TextRow > td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    $("tr.TextRow > td.TextColumn").attr("href", "javascript:void(0);").css("position", "relative");
    $("tr.TextRow > td.TextColumn").click(function () { createTextbox(this) });
    $(".CellTable tr.SupportRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    $(".CellTable tr.SupportRow>td.TextColumn").attr("href", "javascript:void(0);").css("position", "relative");
    $(".CellTable tr.SupportRow>td.TextColumn").click(function () { createTextInsert(this) });
    $(".SupportColorFirstColumn tr.SupportRow>td.TextColumn").removeAttr("href").css("border", "").unbind();
    $('.MinusButton').click(function () {
        showMinusMenu(this);
    });
    $('.PlusButton').click(function () {
        showPlusMenu(this);
    });
}
// verdeckte zellen in titelspalte ausblenden
$(document).ready(function () {
    var i = 1;
    $(".MainRow").each(function () {
        var cell = $(this).children(":first-child");
        var rowspan = $(cell).attr("rowspan");
        $(cell).children(":first-child").css("height", ((rowspan) * 85 - 5) + "px");
        if (i > 1) {
            $(cell).hide();
            i--;
        } else {
            i = rowspan;
        }
    });
});
// cell rowId und cellId aus id von htmlzelle lesen
function getCoords(index) {
    index = index.substring(44);
    var coords = index.split("_");
    return coords;
}
// prüfe: existiert zelle in datenbank
function ifCellEmpty(cell) {
    var index = getCoords($(cell).attr("Id"));
    if (index[1] == "") {
        return true;
    } else {
        return false;
    }
}
//get ordnumber of cell
function getIndex(cell) {
    var index = 0;
    $(cell).prevAll(".MainCell").each(function () {
        if (ifCellEmpty(this) == false) {
            index++;
        }
    });
    return index;
}
//is second column active
function isSecondColActive() {
    return $(".MainRow").first().children().eq(1).css("display") != "none";
}
// webservice aufruf standard
function callWebservice(funcName, param) {
    refreshTimeout();
    var ret;
    var formData = $.toJSON(param);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/" + funcName,
        cache: false,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            if (msg.d == "False") {
                alert("fehler");
            }
            else {
                ret = msg.d
            }
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
    return ret;
}
function refreshTimeout() {
    window.clearInterval(timeoutInterval);
    timeoutInterval = window.setInterval('goBack()', 3600 * 1000)
}
$(document).ready(function () {
    // New Table
    $("#newTableButton").click(function () {
        var matrixId;
        params = {};
        params.template = "0";
        var formData = $.toJSON(params);
        $.ajax({
            type: "POST",
            url: "../WebService/SokratesService.asmx/addNewSokrates",
            cache: false,
            async: false,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                matrixId = msg.d; //JSON object mit eigenschaft "d"
            },
            error: function (result, errortype, exceptionobject) {
                alert('Error:' + result.responseText);
            }
        });
        if (matrixId == 'OrderNeed') {
            window.location.replace("../Payment/Order.aspx");
        }
        else {
            var newUrl = document.URL;
            newUrl = newUrl.split("?");
            window.open(newUrl[0] + "?matrixID=" + matrixId);
        }
    });
    // Delete Table
    $("#deleteTableButton").click(function () {
        if (confirm("Wollen Sie die Sokrateskarte wirklich löschen?")) {
            var params = {};
            params.matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            callWebservice("delSokrates", params);
            var delindex = document.URL.toString().indexOf("/Morph/");
            window.location.href = document.URL.substr(0, delindex + 7) + "MatrixSearch.aspx";
        }
    });
    //Print
    $("#printTableButton").click(function () {
        if ($("#ContentPlaceHolder1__pl__cl__detail_printMenu").css("display") == "none") {
            $("#ContentPlaceHolder1__pl__cl__detail_printMenu").show();
            if ($("#ContentPlaceHolder1__pl__cl__detail_MainTable").width() / $("#ContentPlaceHolder1__pl__cl__detail_MainTable").height() > 1.41) {
                $("#selectOrientation").prop("selectedIndex", 1);
            } else {
                $("#selectOrientation").prop("selectedIndex", 0);
            }
            $('#ContentPlaceHolder1__pl__cl__detail_printMenu').css({
                'left': $("#printTableButton").offset().left + 20
            });
            $('#ContentPlaceHolder1__pl__cl__detail_printMenu').css({
                'top': $("#printTableButton").offset().top + 20
            });
        } else {
            $("#ContentPlaceHolder1__pl__cl__detail_printMenu").hide();
        }
    });
    $("#ContentPlaceHolder1__pl__cl__detail_cancelPrintButton").click(function () {
        $("#ContentPlaceHolder1__pl__cl__detail_printMenu").hide();
    });
    $("#ContentPlaceHolder1__pl__cl__detail_printButton").click(function () {
        $("#ContentPlaceHolder1__pl__cl__detail_mainMenu").hide();
        closeAllMenus();
        //$("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell0").children().hide();
        $(".ColorButton").hide();
        $(".WsButtonAddVisible").hide();
        $(".WsButtonDelVisible").hide();
        $(".WirkungspaketCell").removeClass("ActiveWP");
        if (editMode) {
            $("#editModeButton").click();
        }
        //$('html').animate({ 'zoom': '100%' }, 400);
        $("svg").attr("xmlns", "https://www.w3.org/2000/svg");
        $("svg").height($("#ContentPlaceHolder1__pl__cl__detail_MainTable").height() + 10);
        $("svg").width($("#ContentPlaceHolder1__pl__cl__detail_MainTable").width() + 10);
        //var svghtml = svg.toSVG();
        var svghtml = svg.svg();

        //  $("svg").removeAttr("opacity");
        //var b64 = Base64.encode(svghtml);
        //$("#content").prepend("<img src='data:image/svg+xml;base64,\n" + b64 + "'>");
        params = {};
        params.sContent = $("#ContentPlaceHolder1__pl__cl_detailCellDiv").html();
        params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        params.port = window.location.port;
        params.width = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").width();
        params.height = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").height();
        params.svg = svghtml;
        params.svgHeight = $("svg").height();
        params.svgWidth = $("svg").width();
        params.orientation = $("#selectOrientation").children(":selected").index().toString();
        params.browser = BrowserDetection();
        params.zoomLevel = window.devicePixelRatio;
        // $("svg").width($("svg").width() / 0.99);
        //$("svg").height($("svg").height() / 0.99);
        if (params.port == "") {
            params.port = 80;
        }
        var pdf;
        var formData = $.toJSON(params);
        $.ajax({
            type: "POST",
            url: "../WebService/SokratesService.asmx/ConvertToPDF",
            cache: false,
            async: false,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                pdf = msg.d;
            },
            error: function (result, errortype, exceptionobject) {
                alert('Error:' + result.responseText);
            }
        });
        //$("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell0").children().show();
        $(".ColorButton").show();
        $(".WsButtonAddVisible").show();
        $(".WsButtonDelVisible").show();
        SetActiveWP(activeWp);
        var url = pdf.split("\Morph");
        url[1] = url[1].replace(/\\/gi, "/");
        var urlbase = document.URL.split("/Morph");
        var pdfWindow = $find('ctl00_ContentPlaceHolder1__pl__cl__detail_PdfWindow');
        pdfWindow.setUrl(urlbase[0] + "/Morph" + url[1]);
        pdfWindow.show();  
    });

    //permission
    $("#permissionsButton").click(function () {
        var permissionUrl = "../Common/Authorisations.aspx?tableName=MATRIX&rowID=";
        var permissionsWindow = window.open(permissionUrl + $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class"), "Permissions", "width=350,height=450,left=100,top=200");
        permissionsWindow.focus();
    });
    //copy Sokrates map
    $("#copyTableButton").click(function () {
        copyOptionWindow = $find('ctl00_ContentPlaceHolder1__pl__cl__detail_CopyOptionWindow');
        copyOptionWindow.show();
        copyOptionWindow.moveTo($('#mapMaxButton')[0].x + 30, $('#mapMaxButton')[0].y + 200)
    });
    //submatrix Menu
    var subMatrix;
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/loadSubmatrix",
        cache: false,
        async: true,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            subMatrix = msg.d; //JSON object mit eigenschaft "d"
            addSubmatrixMenu(subMatrix);
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
    $("#ContentPlaceHolder1__pl__cl__detail_AddSubmatrix a").after($("<select>").attr("Id", "SubmatrixDropdown").attr("name", "Submatrix Verlinken"));
    $("#SubmatrixDropdown").before($("<br>"));
    $("#SelectMap").change(function () {
        var matrixId = $("#SelectMap").children(":selected").attr("value");
        var newUrl = document.URL;
        newUrl = newUrl.split("?");
        window.location.href = newUrl[0] + "?matrixID=" + matrixId;
    });
    $("#SubmatrixDropdown").change(function (event) {
        removeSubmatrix($(activecell));
        var submatrixUrlget = document.URL.split("?")
        submatrixUrl = submatrixUrlget[0];
        $("<a href=\"" + submatrixUrl + "?MatrixId=" + $("#SubmatrixDropdown").children(":selected").attr("value") + "\"><img src=\"../images/morph/uf_matrix.png\" border=\"0\"></a>").appendTo($(activecell).find(".TextRow td.SupportColumn"));
        var params = {};
        var cellid = getCoords($(activecell).attr("id"));
        params.cellId = cellid[1];
        params.matrixId = $("#SubmatrixDropdown").children(":selected").attr("value");
        callWebservice("addsubMatrix", params);
        $(this).prop("selectedIndex", 0);
        event.stopPropagation();
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
    });
    $("#SubmatrixDropdown").focusin(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').unbind();
    });
    $("#SubmatrixDropdown").focusout(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide().mouseleave(function () {
            $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
        });
    });
    //knowledge Menu
    addKnowledgeEntries();
    $("#ContentPlaceHolder1__pl__cl__detail_AddNewKnowledge").click(function () {
        removeKnowledge(activecell);
        var knowledgeId;
        var params = {};
        params.cellId = getCoords($(activecell).attr("id"))[1];
        params.matrixId = params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        var formdata = $.toJSON(params)
        $.ajax({
            type: "POST",
            url: "../WebService/SokratesService.asmx/addNewKnowledge",
            cache: false,
            async: false,
            data: formdata,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                knowledgeId = msg.d; //JSON object mit eigenschaft "d"
            },
            error: function (result, errortype, exceptionobject) {
                alert('Error:' + result.responseText);
            }
        });
        if ($("#ContentPlaceHolder1__pl__cl__detail_isSimpleKnowledge")[0].value == 'false') {
            var url = document.URL.split("/Morph");
            var urlbase = url[0];
            $("<a  knowledgeId=\"" + knowledgeId + "\" href=\"" + urlbase + "/Knowledge/KnowledgeDetail.aspx?knowledgeID=" + knowledgeId + "\"></a>").appendTo($(activecell).find(".TextRow td.TextColumn"));
        }
        else {
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1").show();
            knowledgeSimpleLoad(knowledgeId);
            $("<a href=\"#\" knowledgeId=\"" + knowledgeId + "\" onclick=\"getKnowledgeSimple(" + knowledgeId + "); return false;\"></a>").appendTo($(activecell).find(".TextRow td.TextColumn"));
        }
        $(activecell).find(".TextRow td.TextColumn").click();
        $(activecell).find(".TextRow td.TextColumn").attr("style", "color:green;");
        $(".CellTable tr.TextRow>td.TextColumn textarea").focusout();
        $(activecell).find(".TextRow td.TextColumn a").hide();
        $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
        //knowledge = addKnowledgeEntries();
        //buildKnowledgeDropdown(knowledge);
    });
});

//check Browser
function BrowserDetection() {
    //Check if browser is IE
    if (navigator.userAgent.search("MSIE") > -1 || navigator.userAgent.search("Trident") > -1 || navigator.userAgent.search("Edge") > -1) {
        return 'IE';
    }
    //Check if browser is Chrome
    else if (navigator.userAgent.search("Chrome") > -1) {
        return 'CHROME';
    }
    //Check if browser is Firefox 
    else if (navigator.userAgent.search("Firefox") > -1) {
        return 'FIREFOX';
    }
    //Check if browser is Safari
    else if (navigator.userAgent.search("Safari") > -1 && navigator.userAgent.search("Chrome") < 0) {
        return 'SAFARI';
    }
    //Check if browser is Opera
    else if (navigator.userAgent.search("Opera") > -1) {
        return 'OPERA'
    }
    return 'UNKNOWN';
}

function CopyOptionWindowButtonClicked(sender, args) {
    copyColoration = $find('ctl00_ContentPlaceHolder1__pl__cl__detail_CopyOptionWindow_C_copyColoration').get_checked();
    copyWirkungspakete = $find('ctl00_ContentPlaceHolder1__pl__cl__detail_CopyOptionWindow_C_copyWirkungspaket').get_checked();
    $find('ctl00_ContentPlaceHolder1__pl__cl__detail_CopyOptionWindow').close();
    coppyTable(copyColoration, copyWirkungspakete);

}
function coppyTable(copyColoration, copyWirkungspakete) {
    if ($("#copyTableButton").prop("disabled") == null || $("#copyTableButton").prop("disabled") != "true") {
        $("#copyTableButton").prop("disabled", "true");
        $('body').addClass('wait');
        params = {};
        params.matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
        params.copyColoration = copyColoration;
        params.copyWirkungspakete = copyWirkungspakete;
        var newUrl = document.URL;
        var newSokratesId;
        var formData = $.toJSON(params);
        $.ajax({
            type: "POST",
            url: "../WebService/SokratesService.asmx/copySokratesMap",
            cache: false,
            async: true,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                newSokratesId = msg.d;
                if (newSokratesId == 'OrderNeed') {
                    window.location.replace("../Payment/Order.aspx");
                }
                else {
                    newUrl = newUrl.split("?");
                    window.location.href = newUrl[0] + "?matrixID=" + newSokratesId;
                }
                $('body').removeClass('wait');
                $("#copyTableButton").prop("disabled", "false");
            },
            error: function (result, errortype, exceptionobject) {
                alert('Error:' + result.responseText);
                $('body').removeClass('wait');
                $("#copyTableButton").prop("disabled", "false");
            }
        });
    }
}
function addSubmatrixMenu(subMatrix) {
    var SelectItems = "";
    var el = $("#SelectMap");
    el.innerHTML = '';
    for (var i = (subMatrix.length - 1); i >= 0; i--) {
        var foo2 = document.createElement("option");
        foo2.appendChild(document.createTextNode(subMatrix[i].title.toString()));
        foo2.value = subMatrix[i].id.toString();
        el.append(foo2);
    }
    $(el).children(":first-child").remove();
    var el1 = $("#SubmatrixDropdown");
    el1.innerHTML = '';
    for (var i = (subMatrix.length - 1); i >= 0; i--) {
        var foo2 = document.createElement("option");
        foo2.appendChild(document.createTextNode(subMatrix[i].title.toString()));
        foo2.value = subMatrix[i].id.toString();
        el1.append(foo2);
    }
}
function addKnowledgeEntries() {
    var ret;
    var params = {}
    params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
    refreshTimeout();
    var ret;
    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/loadKnowledge",
        cache: false,
        async: true,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            if (msg.d == "False") {
                alert("fehler");
            }
            else {
                ret = msg.d
                buildKnowledgeDropdown(ret);
            }
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
}
function buildKnowledgeDropdown(knowledge) {
    $("#ContentPlaceHolder1__pl__cl__detail_AddKnowledge a").after($("<select>").attr("Id", "KnowledgeDropdown").attr("name", "Knowledge Verlinken"));
    for (var i = (knowledge.length - 1); i >= 0; i--) {
        $("<option>").text(knowledge[i].title.toString()).attr("value", knowledge[i].id.toString()).appendTo($("#KnowledgeDropdown"));
    }
    $("#KnowledgeDropdown").before($("<br>"));
    $("#KnowledgeDropdown").change(function (event) {
        removeKnowledge(activecell);
        var url = document.URL.split("/Morph");
        var urlbase = url[0];
        $("<a  knowledgeId=\"" + $("#KnowledgeDropdown").children(":selected").attr("value") + "\" href=\"" + urlbase + "/Knowledge/KnowledgeDetail.aspx?knowledgeID=" + $("#KnowledgeDropdown").children(":selected").attr("value") + "\"></a>").appendTo($(activecell).find(".TextRow td.TextColumn"));
        $(activecell).find(".TextRow td.TextColumn").click();
        $(activecell).find(".TextRow td.TextColumn").attr("style", "color:green;");
        $(".CellTable tr.TextRow>td.TextColumn textarea").focusout();
        $(activecell).find(".TextRow td.TextColumn a").hide();
        var id = getCoords($(activecell).attr("id"));
        var params = {};
        params.cellId = id[1];
        params.knowledgeId = $("#KnowledgeDropdown").children(":selected").attr("value")
        callWebservice("addKnowledge", params);
        $(this).prop("selectedIndex", 0);
        event.stopPropagation();
        $("#KnowledgeDropdown").focusout();
        $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
    });
    $("#KnowledgeDropdown").focusin(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').unbind();
    });
    $("#KnowledgeDropdown").focusout(function () {
        $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide().mouseleave(function () {
            $('#ContentPlaceHolder1__pl__cl__detail_plusMenu').hide("fast");
        });
    });
}
function removeSubmatrix(cell) {
    if (hasSubMatrix(cell)) {
        $(cell).find(".TextRow td.SupportColumn a").last().remove();
        var id = getCoords($(cell).attr("id"));
        var params = {};
        params.cellId = id[1];
        callWebservice("delsubMatrix", params);
    }
}
function removeKnowledge(cell) {
    $(cell).find(".TextRow td.TextColumn a").remove();
    var id = getCoords($(cell).attr("id"));
    var params = {};
    params.cellId = id[1];
    callWebservice("delKnowledge", params);
    $(activecell).find(".TextRow td.TextColumn").attr("style", "color:black;");
}
function updateLastChangeDate() {
    var date = new Date();
    var now = date.getDate();
    if (now.toString().length == 1) { now = '0' + now; }
    var month = date.getMonth() + 1;
    if (month.toString().length == 1) { month = '0' + month; }
    now += "." + month;
    now += "." + date.getFullYear();
    var hour = date.getHours();
    if (hour.toString().length == 1) { hour = '0' + hour; }
    now += " " + hour;
    var min = date.getMinutes();
    if (min.toString().length == 1) { min = '0' + min; }
    now += ":" + min;
    var sec = date.getSeconds();
    if (sec.toString().length == 1) { sec = '0' + sec; }
    now += ":" + sec;
    document.getElementById("ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").innerHTML = now;
    var params = {};
    params.MatrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
    params.CellId = "LastChange";
    params.Text = now;
    callWebservice("dbTextChange", params)
}
function toggleWirkungspaket(button) {
    if ($(button).attr("Class") == "Active") {
        $(button).removeClass("Active");
        $(button).css("background-image", "url(../images/morph/Checkbox_Normal.png)");
    } else {
        $(button).addClass("Active");
        $(button).css("background-image", "url(../images/morph/Checkbox_Active.png)");
    }
    drawWirkungselemente()
}

$(window).resize(function () {
    drawWirkungselemente();
});

function drawWirkungselemente() {
    $("svg").children().remove();
    var wirkungspaketArray1 = new Array();
    var wirkungspaketArray2 = new Array();
    var wirkungspaketArray3 = new Array();
    var wirkungspaketArray4 = new Array();
    var wirkungspaketArray5 = new Array();
    var wpId1 = $(".WirkungspaketCell").eq(0).attr("id").split("_").pop().toString();
    var wpId2 = $(".WirkungspaketCell").eq(1).attr("id").split("_").pop().toString();
    var wpId3 = $(".WirkungspaketCell").eq(2).attr("id").split("_").pop().toString();
    var wpId4 = $(".WirkungspaketCell").eq(3).attr("id").split("_").pop().toString();
    var wpId5 = $(".WirkungspaketCell").eq(4).attr("id").split("_").pop().toString();
    for (var i in wirkungselementeArray) {
        switch (wirkungselementeArray[i].Wirkungspaket_ID.toString()) {
            case wpId1:
                wirkungspaketArray1.push(wirkungselementeArray[i]);
                break;
            case wpId2:
                wirkungspaketArray2.push(wirkungselementeArray[i]);
                break;
            case wpId3:
                wirkungspaketArray3.push(wirkungselementeArray[i]);
                break;
            case wpId4:
                wirkungspaketArray4.push(wirkungselementeArray[i]);
                break;
            case wpId5:
                wirkungspaketArray5.push(wirkungselementeArray[i]);
                break;
        }
    }
    if ($("td.WirkungspaketCell").eq(0).children("input").attr("Class") != "Active") {
        wirkungspaketArray1 = [];
    }
    if ($("td.WirkungspaketCell").eq(1).children("input").attr("Class") != "Active") {
        wirkungspaketArray2 = [];
    }
    if ($("td.WirkungspaketCell").eq(2).children("input").attr("Class") != "Active") {
        wirkungspaketArray3 = [];
    }
    if ($("td.WirkungspaketCell").eq(3).children("input").attr("Class") != "Active") {
        wirkungspaketArray4 = [];
    }
    if ($("td.WirkungspaketCell").eq(4).children("input").attr("Class") != "Active") {
        wirkungspaketArray5 = [];
    }
    var wirkungselementeArrayTemp = new Array();
    wirkungselementeArrayTemp = wirkungspaketArray1.concat(wirkungspaketArray2, wirkungspaketArray3, wirkungspaketArray4, wirkungspaketArray5);
    if (wirkungselementeArrayTemp.length != 0) {
        $(".MainCell").each(function () {
            if (cellId != "") {
                var cellId = getCoords($(this).attr("id"))[1];
                var radiusChange = 0;
                var radiusmargin = 10;
                var xRadius = $(this).width() / 2 + radiusmargin;
                var yRadius = $(this).height() / 2 + radiusmargin;
                for (var i in wirkungspaketArray1) {
                    if (wirkungspaketArray1[i].ORIGIN_CHARACTERISTIC_ID == cellId || wirkungspaketArray1[i].GOAL_CHARACTERISTIC_ID == cellId) {
                        drawEllipse($(this), xRadius - radiusChange, yRadius - radiusChange, 1);
                        radiusChange += 4;
                        break;
                    }
                }
                for (var i in wirkungspaketArray2) {
                    if (wirkungspaketArray2[i].ORIGIN_CHARACTERISTIC_ID == cellId || wirkungspaketArray2[i].GOAL_CHARACTERISTIC_ID == cellId) {
                        drawEllipse($(this), xRadius - radiusChange, yRadius - radiusChange, 2);
                        radiusChange += 4;
                        break;
                    }
                }
                for (var i in wirkungspaketArray3) {
                    if (wirkungspaketArray3[i].ORIGIN_CHARACTERISTIC_ID == cellId || wirkungspaketArray3[i].GOAL_CHARACTERISTIC_ID == cellId) {
                        drawEllipse($(this), xRadius - radiusChange, yRadius - radiusChange, 3);
                        radiusChange += 4;
                        break;
                    }
                }
                for (var i in wirkungspaketArray4) {
                    if (wirkungspaketArray4[i].ORIGIN_CHARACTERISTIC_ID == cellId || wirkungspaketArray4[i].GOAL_CHARACTERISTIC_ID == cellId) {
                        drawEllipse($(this), xRadius - radiusChange, yRadius - radiusChange, 4);
                        radiusChange += 4;
                        break;
                    }
                }
                for (var i in wirkungspaketArray5) {
                    if (wirkungspaketArray5[i].ORIGIN_CHARACTERISTIC_ID == cellId || wirkungspaketArray5[i].GOAL_CHARACTERISTIC_ID == cellId) {
                        drawEllipse($(this), xRadius - radiusChange, yRadius - radiusChange, 5);
                        radiusChange += 4;
                        break;
                    }
                }
            }
        });
        var coordinateArray = new Array(); // saves column and row of each origin/goal cell
        while (wirkungselementeArrayTemp.length > 0) {
            var cellCoordinates = {};
            var originCell = "";
            var goalCell = "";
            //search for origin and goal cells
            $(".MainCell").each(function () {
                var cellId = getCoords($(this).attr("id"))[1];
                if (cellId == wirkungselementeArrayTemp[0].ORIGIN_CHARACTERISTIC_ID) {
                    originCell = $(this);
                } else if (cellId == wirkungselementeArrayTemp[0].GOAL_CHARACTERISTIC_ID) {
                    goalCell = $(this);
                }
            });
            //read coordinates
            cellCoordinates.xo = $(originCell).offset().left + $(originCell).width() / 2;//$(originCell).parent().find("table").index($(originCell).children("table"));
            cellCoordinates.yo = $(originCell).offset().top + $(originCell).height() / 2; //$(".MainRow").index($(originCell).parent());
            cellCoordinates.xg = $(goalCell).offset().left + $(goalCell).width() / 2;//$(goalCell).parent().find("table").index($(goalCell).children("table"));
            cellCoordinates.yg = $(goalCell).offset().top + $(goalCell).height() / 2; //$(".MainRow").index($(goalCell).parent());
            cellCoordinates.translation = 0;
            //search for other lines
            if (cellCoordinates.xo == cellCoordinates.xg) { // vertical
                for (var i = 0; i < coordinateArray.length; i++) {
                    if (coordinateArray[i].translation == cellCoordinates.translation) { // other has same translation
                        if (coordinateArray[i].xo == coordinateArray[i].xg && coordinateArray[i].xo == cellCoordinates.xo) { //other is vertical
                            if (isNumberBetween(cellCoordinates.yo, coordinateArray[i].yo, coordinateArray[i].yg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (isNumberBetween(cellCoordinates.yg, coordinateArray[i].yo, coordinateArray[i].yg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (isNumberBetween(coordinateArray[i].yo, cellCoordinates.yo, cellCoordinates.yg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (isNumberBetween(coordinateArray[i].yg, cellCoordinates.yo, cellCoordinates.yg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (coordinateArray[i].xo == cellCoordinates.xo && cellCoordinates.yo == coordinateArray[i].yo && cellCoordinates.xg == coordinateArray[i].xg && cellCoordinates.yg == coordinateArray[i].yg) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (coordinateArray[i].xo == cellCoordinates.xg && cellCoordinates.yo == coordinateArray[i].yg && cellCoordinates.xg == coordinateArray[i].xo && cellCoordinates.yg == coordinateArray[i].yo) {
                                cellCoordinates.translation++;
                                i = -1;
                            }
                        }
                    }
                }
            } else if (cellCoordinates.yo == cellCoordinates.yg) { // horizontal
                for (var i = 0; i < coordinateArray.length; i++) {
                    if (coordinateArray[i].translation == cellCoordinates.translation) { // other has same translation
                        if (coordinateArray[i].yo == coordinateArray[i].yg && coordinateArray[i].yo == cellCoordinates.yo) { //other is horizontal
                            if (isNumberBetween(cellCoordinates.xo, coordinateArray[i].xo, coordinateArray[i].xg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            }
                            else if (isNumberBetween(cellCoordinates.xg, coordinateArray[i].xo, coordinateArray[i].xg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            }
                            else if (isNumberBetween(coordinateArray[i].xo, cellCoordinates.xo, cellCoordinates.xg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (isNumberBetween(coordinateArray[i].xg, cellCoordinates.xo, cellCoordinates.xg)) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (coordinateArray[i].xo == cellCoordinates.xo && cellCoordinates.yo == coordinateArray[i].yo && cellCoordinates.xg == coordinateArray[i].xg && cellCoordinates.yg == coordinateArray[i].yg) {
                                cellCoordinates.translation++;
                                i = -1;
                            } else if (coordinateArray[i].xo == cellCoordinates.xg && cellCoordinates.yo == coordinateArray[i].yg && cellCoordinates.xg == coordinateArray[i].xo && cellCoordinates.yg == coordinateArray[i].yo) {
                                cellCoordinates.translation++;
                                i = -1;
                            }
                        }
                    }
                }
            } else { // diagonal
                //var m=0; //slope to the right
                //if(cellCoordinates.xg>cellCoordinates.xo){
                //    m = (cellCoordinates.yg-cellCoordinates.yo)/(cellCoordinates.xg-cellCoordinates.xo);
                //}else{
                //    m = (cellCoordinates.yo-cellCoordinates.yg)/(cellCoordinates.xo-cellCoordinates.xg);
                //}
                for (var i = 0; i < coordinateArray.length; i++) {
                    if (coordinateArray[i].translation == cellCoordinates.translation) { // other has same translation
                        if (coordinateArray[i].xo == cellCoordinates.xo && cellCoordinates.yo == coordinateArray[i].yo && cellCoordinates.xg == coordinateArray[i].xg && cellCoordinates.yg == coordinateArray[i].yg) {
                            cellCoordinates.translation++;
                            i = -1;
                        } else if (coordinateArray[i].xo == cellCoordinates.xg && cellCoordinates.yo == coordinateArray[i].yg && cellCoordinates.xg == coordinateArray[i].xo && cellCoordinates.yg == coordinateArray[i].yo) {
                            cellCoordinates.translation++;
                            i = -1;
                        }
                    }
                }
            }
            //drawWirkungselementConnection
            drawWirkungselementConnection(originCell, goalCell, wirkungselementeArrayTemp[0].Wirkungspaket_ID, cellCoordinates);
            //save to array
            coordinateArray.push(cellCoordinates);
            wirkungselementeArrayTemp.splice(0, 1);
        }
        $("svg *").attr("pointer-events", "none");
        $("svg").css("height", $("#ContentPlaceHolder1__pl__cl__detail_MainTable").height() + 10);
        $("svg").css("width", $("#ContentPlaceHolder1__pl__cl__detail_MainTable").width() + 10);
    }
}
function drawEllipse(Cell, xRadius, yRadius, wirkungspaketNr) {
    var x = Cell.offset().left + Cell.width() / 2 - $('#ContentPlaceHolder1__pl__cl__detail_MainTable').offset().left;
    var y = Cell.offset().top + Cell.height() / 2 - $('#ContentPlaceHolder1__pl__cl__detail_MainTable').offset().top;
    var color = $(".WirkungspaketCell span").eq(wirkungspaketNr - 1).css("background-color").toString();
    var ellipse = svg.ellipse(xRadius * 2, yRadius * 2);
    ellipse.attr({ cx: x, cy: y, fill: 'none', stroke: color, 'stroke-width': '3', 'stroke-opacity': ' 0.7', 'fill-opacity': '0.7' });

}
function removeWirkungspaket(button) {
    if (confirm('Wollen Sie das Wirkungspaket wirklich löschen?')) {
        var wirkungspaketId = $(button).parent().attr("id").substring(36).split("_")[1];
        for (var i = 0; i < wirkungselementeArray.length; i++) {
            if (wirkungspaketId == wirkungselementeArray[i].Wirkungspaket_ID) {
                wirkungselementeArray.splice(i, 1);
                i--;
            }
        }
        var params = {};
        params.wirkungspaketId = wirkungspaketId;
        params.ordnumber = $(".RemoveWirkungspaket").index($(button)) + 1;
        callWebservice("removeWirkungspaket", params);
        $(button).parent().contents().filter(function () { return this.nodeType == 3; }).remove();
        $(button).parent().append("WP" + params.ordnumber);
        drawWirkungselemente();
    }
}
function hasCellText(cell) {
    var textCell = $(cell).find(".TextColumn.TextColumnAbove");
    var text = "";
    if ($(textCell).find("a").length > 0) {
        text = $(textCell).find("a").text();
    } else {
        text = $(textCell).text();
    }
    return text != "";
}
function hasSupportText(cell) {
    var textCell = $(cell).find(".TextColumn.TextColumnBelow");
    var text = $(textCell).text();
    return text != "";
}
function stripCell(cell) {
    if (hasKnowledge(cell)) {
        removeKnowledge(cell);
    }
    if (hasCellText(cell)) {
        changeText(cell, "");
    }
    if (hasSubMatrix(cell)) {
        removeSubmatrix(cell);
    }
    if (hasSupportText(cell)) {
        changeOneLineText($(cell).find(".TextColumnBelow"), "");
    }
    var index = getCoords($(cell).attr("Id"));
    for (var i = 0; wirkungselementeArray.length > i; i++) {
        if (index[1] == wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID || index[1] == wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID) {
            removeWirkungselementConnection(wirkungselementeArray[i].ORIGIN_CHARACTERISTIC_ID, wirkungselementeArray[i].GOAL_CHARACTERISTIC_ID, wirkungselementeArray[i].Wirkungspaket_ID);
            i--;
        }
    }
}
function isNumberBetween(number, border1, border2) {
    if (border1 < border2) {
        return (border1 < number && number < border2);
    } else {
        return (border2 < number && number < border1);
    }
}
function knowledgeSimpleLoad(knowledgeId) {
    params = {};
    params.knowledgeId = knowledgeId;
    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/getKnowledgeSimple",
        cache: false,
        async: true,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $("#ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1_C_KnowledgeTitle")[0].value = msg.d[0].title;
            if (msg.d[0].description != null) {
                $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1_C_editor").set_html(msg.d[0].description);
            }
            else {
                $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1_C_editor").set_html("");
            }
            activeKnowledgeId = msg.d[0].id;
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
}
function saveKnowledgeClicked(sender, args) {
    params = {};
    params.knowledgeId = activeKnowledgeId;
    params.title = $("#ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1_C_KnowledgeTitle")[0].value;
    params.description = $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1_C_editor").get_html();
    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/saveKnowledgeSimple",
        cache: false,
        async: true,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
}
function closeKnowledgeClicked(sender, args) {
    saveKnowledgeClicked(sender, args);
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1").close();
}
function deleteKnowledgeClicked(sender, args) {
    params = {};
    params.knowledgeId = activeKnowledgeId;
    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/deleteKnowledgeSimple",
        cache: false,
        async: true,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            removeKnowledge(activecell);
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl01_RadWindow1").close();
            $(".CellTable tr.TextRow>td.TextColumn").attr("href", "javascript:void(0);").css("border", "1px solid gray");
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
}
function getKnowledgeSimple(knowledgeId) {
    params = {};
    params.knowledgeId = knowledgeId;
    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/getKnowledgeSimple",
        cache: false,
        async: true,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl00_RadWindow2").show();
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl00_RadWindow2").center();
            $("#TextContentTitle")[0].innerHTML = msg.d[0].title;
            $("#TextContent")[0].innerHTML = msg.d[0].description;
            activeKnowledgeId = msg.d[0].id;
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });


}

function CloseKnowledgeViewWindowClicked(sender, args) {
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ctl00_RadWindow2").close();

}

var DragCell = null;
var sourceCell;
function RowCellDetailDragStart(e) {
    //if ($(this).find('.InputBox').length === 1 || $(this).find('.Input').length === 1) {
    DragCell = this.id;
    sourceCell = DragCell.valueOf();
    if (navigator.userAgent.indexOf('Trident') === -1) {
        e.dataTransfer.setData('text/html', this.innerHTML);
        e.dataTransfer.effectAllowed = 'move';
    }
    this.style.opacity = '0.4';
    //}
    //else {
    //    return false;
    //}


}

function RowCellDetailDragEnd(e) {
    $('#' + DragCell).css('opacity', '1.0');
    $('#insertBeforImage')[0].style.visibility = 'hidden';
    $('#insertAfterImage')[0].style.visibility = 'hidden';
    $('.RowCellDetail').removeClass('over');
    DragCell = null;
}

function RowCellDetailDragOver(e) {
    var dragElementTmp = $(document.elementFromPoint(e.clientX, e.clientY)).closest('.MainCell');
    if (DragCell !== null && dragElementTmp[0] !== 'undefined' && $(dragElementTmp)[0].draggable === true) {
        this.classList.add('over');
        ShowArrow(e.clientX, e.clientY, dragElementTmp);
        e.preventDefault();
        return false;
    }
    else {
        $('#insertBeforImage')[0].style.visibility = 'hidden';
        $('#insertAfterImage')[0].style.visibility = 'hidden';
    }
}


function RowCellDetailDragEnter(e) {
    var dragElementTmp = $(document.elementFromPoint(e.clientX, e.clientY)).closest('.MainCell');
    if (DragCell !== null && dragElementTmp !== 'undefined' && $(dragElementTmp)[0].draggable === true) {
        this.classList.add('over');
    }
}

function RowCellDetailDragExit(e) {
    if (!$(document.elementFromPoint(e.clientX, e.clientY)).closest('.MainCell').hasClass('RowCellDetail')) {
        this.classList.remove('over');
        $('#insertBeforImage')[0].style.visibility = 'hidden';
        $('#insertAfterImage')[0].style.visibility = 'hidden';
    }
}

function RowCellDetailDrop(e) {
    this.classList.remove('over');
    $('#' + DragCell).css('opacity', '1.0');
    var startCell = $('#' + sourceCell);
    var targetCell = $(e.currentTarget);
    var position = GetPosition(e.clientX, e.clientY, targetCell);
    MoveMainCell(startCell, targetCell, position);
    sourceCell = null;
}

var RowCell0 = null;
function RowCell0DragStart(e) {
    //if ($(this).find('.InputBox').length === 1 || $(this).find('.Input').length === 1) {
    RowCell0 = this.id;
    sourceCell = RowCell0.valueOf();
    if (navigator.userAgent.indexOf('Trident') === -1) {
        e.dataTransfer.setData('text/html', this.innerHTML);
        e.dataTransfer.effectAllowed = 'move';
    }
    this.style.opacity = '0.4';
    //}
    //else {
    //    DragDropTouch.DragDropTouch.prototype._reset();
    //    return false;
    //}


}

function RowCell0DragEnd(e) {
    $('#' + RowCell0).css('opacity', '1.0');
    $('#insertBelowImage')[0].style.visibility = 'hidden';
    $('#insertAboveImage')[0].style.visibility = 'hidden';
    RowCell0 = null;


}

function RowCell0DragOver(e) {
    var dragElementTmp = $(document.elementFromPoint(e.clientX, e.clientY)).closest('.MainCell');

    if (RowCell0 !== null && dragElementTmp[0] !== 'undefined' && $(dragElementTmp)[0].draggable === true) {
        this.classList.add('over');
        ShowArrowRow(e.clientX, e.clientY, dragElementTmp);
        e.preventDefault();
        return false;
    }
}

function RowCell0DragEnter(e) {
    if (RowCell0 !== null) {
        this.classList.add('over');
    }

}

function RowCell0DragExit(e) {
    if (!$(document.elementFromPoint(e.clientX, e.clientY)).closest('.MainCell').hasClass('RowCell0')) {
        this.classList.remove('over');
        $('#insertBelowImage')[0].style.visibility = 'hidden';
        $('#insertAboveImage')[0].style.visibility = 'hidden';
    }
}

function RowCell0Drop(e) {
    this.classList.remove('over');
    //$('#' + RowCell0).css('opacity', '1.0');
    var startCell = $('#' + sourceCell);
    var targetCell = $(e.currentTarget);
    var position = GetPosition(e.clientX, e.clientY, targetCell);
    MoveMainRow(startCell, targetCell, position);
    sourceCell = null;
}

var RowCell1 = null;
function RowCell1DragStart(e) {
    dropZone = 'subrows';
    if (navigator.userAgent.indexOf('Trident') === -1) {
        e.dataTransfer.setData('text/html', this.innerHTML);
        e.dataTransfer.effectAllowed = 'move';
    }
    this.style.opacity = '0.4';
    RowCell1 = this.id;

}

function RowCell1DragEnd(e) {
    $('#' + RowCell1).css('opacity', '1.0');
    RowCell1 = null;
}

function RowCell1DragOver(e) {
    if (RowCell1 !== null) {
        this.classList.add('over');
        e.preventDefault();
        return false;
    }
    e.dataTransfer.dropEffect = 'move';
}

function RowCell1DragEnter(e) {
    if (RowCell1 !== null) {
        this.classList.add('over');
    }
}

function RowCell1DragExit(e) {
    this.classList.remove('over');
}

function RowCell1Drop(e) {
    this.classList.remove('over');
    $('#' + RowCell1).css('opacity', '1.0');
    var startCell = $('#' + sourceCell);
    var targetCell = $(e.currentTarget);
    var position = GetPosition(e.clientX, e.clientY, targetCell);
    MoveSubRow(startCell, targetCell, position);
}

function GetPosition(x, y, targetCell) {
    var position = {};
    var destX = targetCell[0].getBoundingClientRect().left;
    var destY = targetCell[0].getBoundingClientRect().top;
    var destWidth = targetCell[0].getBoundingClientRect().width;
    var destHeight = targetCell[0].getBoundingClientRect().height;

    if (x > (destX + destWidth / 2)) {
        position.horizontal = 'after';
    }
    else {
        position.horizontal = 'before';
    }

    if (y > (destY + destHeight / 2)) {
        position.vertical = 'below';
    }
    else {
        position.vertical = 'above';
    }
    return position;
}

function AddDaDEventListener(cell) {
    var className = GetDaDClass(cell);
    cell.addEventListener('dragstart', eval(className + 'DragStart'), false);
    cell.addEventListener('dragend', eval(className + 'DragEnd'), false);
    cell.addEventListener('dragover', eval(className + 'DragOver'), false);
    cell.addEventListener('dragenter', eval(className + 'DragEnter'), false);
    cell.addEventListener('dragleave', eval(className + 'DragExit'), false);
    cell.addEventListener('drop', eval(className + 'Drop'), false);
    $(cell).attr('draggable', 'true');
}

function RemoveDaDEventListener(cell) {
    var className = GetDaDClass(cell);
    cell.removeEventListener('dragstart', eval(className + 'DragStart'));
    cell.removeEventListener('dragend', eval(className + 'DragEnd'));
    cell.removeEventListener('dragover', eval(className + 'DragOver'));
    cell.removeEventListener('dragenter', eval(className + 'DragEnter'));
    cell.removeEventListener('dragleave', eval(className + 'DragExit'));
    cell.removeEventListener('drop', eval(className + 'Drop'));
    cell.removeAttribute('draggable');
}

function GetDaDClass(cell) {
    var className = '';
    if (cell.classList.contains('RowCellDetail')) {
        className = 'RowCellDetail';
    }
    if (cell.classList.contains('RowCell0')) {
        className = 'RowCell0';
    }
    if (cell.classList.contains('RowCell1')) {
        className = 'RowCell1';
    }
    return className;
}

function ShowArrow(x, y, targetCell) {
    var position = GetPosition(x, y, targetCell).horizontal;
    var cellPositions = targetCell[0].getClientRects()[0];
    if (position === 'before') {
        $('#insertAfterImage')[0].style.left = cellPositions.left - 75 + 'px';
        $('#insertAfterImage')[0].style.top = cellPositions.top + 'px';
        $('#insertAfterImage')[0].style.visibility = 'visible';
        $('#insertBeforImage')[0].style.visibility = 'hidden';
    }
    else {
        $('#insertBeforImage')[0].style.left = cellPositions.left + 150 + 'px';
        $('#insertBeforImage')[0].style.top = cellPositions.top + 'px';
        $('#insertBeforImage')[0].style.visibility = 'visible';
        $('#insertAfterImage')[0].style.visibility = 'hidden';
    }
}

function ShowArrowRow(x, y, targetCell) {
    var position = GetPosition(x, y, targetCell).vertical;
    var cellPositions = targetCell[0].getClientRects()[0];
    if (position === 'below') {
        $('#insertAboveImage')[0].style.left = cellPositions.left + 30 + 'px';
        $('#insertAboveImage')[0].style.top = cellPositions.top + cellPositions.height + 3 + 'px';
        $('#insertAboveImage')[0].style.visibility = 'visible';
        $('#insertBelowImage')[0].style.visibility = 'hidden';
    }
    else {
        $('#insertBelowImage')[0].style.left = cellPositions.left + 30 + 'px';
        $('#insertBelowImage')[0].style.top = cellPositions.top + -76 + 'px';
        $('#insertBelowImage')[0].style.visibility = 'visible';
        $('#insertAboveImage')[0].style.visibility = 'hidden';
    }

}

function MoveMainRow(dragElement, dropElement, position) {
    var targetId = null;
    if (position.vertical === 'below') {
        if (dropElement[0].parentNode.nextElementSibling === null) {
            targetId = dropElement[0].id;
        }
        else {
            targetId = dropElement[0].parentNode.nextElementSibling.cells[0].id;
        }

    }
    else {
        targetId = dropElement[0].parentNode.previousElementSibling.cells[0].id;
    }
    if (dragElement[0].id === dropElement[0].id || dragElement[0].id === targetId) {
        return;
    }

    var rowspan = parseInt(dragElement[0].attributes.rowspan.value);
    var dropRowspan = parseInt(dropElement[0].attributes.rowspan.value);

    var subrows = [];
    var rowPostmp = dragElement[0].parentNode;
    if (rowspan > 1) {
        for (i = 0; i < rowspan - 1; i++) {
            subrows[i] = rowPostmp.nextElementSibling;
            rowPostmp = subrows[i];
        }
    }

    //move row
    var sourceRow = dragElement[0].parentNode;
    var targetRow;
    if (dropRowspan > 1 && position.vertical === 'below') {
        targetRow = $(dropElement[0].parentNode).nextAll().eq(dropRowspan - 2);
    } else {
        targetRow = dropElement[0].parentNode;
    }
    if (position.vertical == "below") {
        $(sourceRow).detach().insertAfter($(targetRow));
    } else {
        $(sourceRow).detach().insertBefore($(targetRow));
    }

    //move subRows
    rowPostmp = sourceRow;
    if (rowspan > 1) {
        for (i = 0; i < subrows.length; i++) {
            $(subrows[i]).detach().insertAfter($(rowPostmp));
            rowPostmp = subrows[i];
        }
    }

    MoveMainRowDB($('.RowCell0'));
    // redraw wirkungselelmente
    drawWirkungselemente();
    $('.MainCell').removeClass('over');

}

function MoveSubRow(dragElement, dropElement, position) {

}

function MoveMainCell(dragElement, dropElement, position) {
    if (dragElement[0].id === dropElement[0].id || (position.horizontal == "after" && (dropElement[0].nextElementSibling !== null && dropElement[0].nextElementSibling.id === dragElement[0].id)) || (position.horizontal == "before" && dropElement[0].previousElementSibling.id === dragElement[0].id)) {
        return;
    }
    var OldOrdNr = getIndex(dragElement);
    var RowCellsDragRow = dragElement.parent()[0].cells.length;
    var DragElementRowIndex = dragElement.parent()[0].rowIndex;
    var DropElementRowIndex = dropElement.parent()[0].rowIndex;


    //move cell
    if (position.horizontal == "after") {
        $(dragElement).detach().insertAfter($(dropElement));
    } else {
        $(dragElement).detach().insertBefore($(dropElement));

    }
    var RowCellsDropRow = dropElement.parent()[0].cells.length;

    var index = getCoords($(dropElement).attr("id"));

    lastCell = $(dragElement).siblings().addBack().filter(":last-child");
    indexLastCell = getCoords($(lastCell).attr("Id"))

    //remove last cell in row if it is empty and movement not in same row
    if (indexLastCell[1] == "" && index[0] != getCoords($(dropElement).attr("id"))[0] && $(lastCell).parents(".MainRow").children(".MainCell").index($(lastCell)) != $(dragElement).parents(".MainRow").children(".MainCell").index($(dragElement))) {
        $(dragElement).nextAll(":last-child").remove();
    } else {
        //if (dragElement.parent()[0].rowIndex !== dropElement.parent()[0].rowIndex) {
        //    //else add new column to end of table
        //    $("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell1_2").attr("colspan")) + 1));
        //    $("<td>").appendTo($(".TitleSupportRow"));
        //    $("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleCell0_0").attr("colspan")) + 1));
        //    $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell3").attr("colspan")) + 1));
        //    $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell5").attr("colspan")) + 1));
        //    $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell8").attr("colspan")) + 1));
        //    $("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan", (parseInt($("#ContentPlaceHolder1__pl__cl__detail_TitleRowCell10").attr("colspan")) + 1));
        //}
    }

    var addColumnNr = 0;
    //remove last cell in drop row if empty

    if ($('#' + dropElement.parent()[0].cells[RowCellsDropRow - 1].id).find('.ColorButton').length === 0 && DragElementRowIndex !== DropElementRowIndex) {
        $('#' + dropElement.parent()[0].cells[RowCellsDropRow - 1].id).detach();
        addColumnNr - 1;
    }
    // fill all rows with empty cells
    $(dropElement).parents(".MainRow").siblings(".MainRow").each(function () {
        while ($(this).children(".MainCell").length + addColumnNr != $(dropElement).parents(".MainRow").children(".MainCell").length) {
            addCellAfter($(this).children(":last-child"), true);
        }
    });

    //update db
    MoveMainCellDB(dragElement, getCoords($(dropElement).attr("id"))[0], OldOrdNr);

    // replace id of row in html cellid
    $(dragElement).attr("id", $(dragElement).attr("id").replace(getCoords($(dragElement).attr("id"))[0], getCoords($(dropElement).attr("id"))[0]))


    // redraw wirkungselelmente
    drawWirkungselemente();
    $('.MainCell').removeClass('over');
}

function MoveMainCellDB(Cell, RowId, OldOrdNr) {
    var params = {};
    var index = getCoords($(Cell).attr("id"));
    params.OldRowID = index[0];
    params.CellID = index[1];
    params.RowID = RowId;
    params.OldIndex = OldOrdNr;
    params.Index = getIndex(Cell);
    var formData = $.toJSON(params);
    var ret;
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/dbMoveCell",
        cache: false,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            ret = msg.d; //JSON object mit eigenschaft "d"
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
    return ret;
}

function MoveMainRowDB(rows) {
    var params = {};
    params.rows = [];

    var i = 0;
    rows.toArray().forEach(function (row) {
        params.rows[i] = getCoords($(row).attr("id"))[0];
        i += 1;
    });
    var formData = $.toJSON(params);
    var ret;
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/dbMoveRow",
        cache: false,
        async: false,
        data: formData,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            ret = msg.d; //JSON object mit eigenschaft "d"
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });
    return ret;

}
