/// <reference path="../Scripts/jquery-3.3.1.min.js" />
/// <reference path="../Javascript/jquery.json-2.2.min.js" />
///// <reference path="../Javascript/ig.ui.combo.js" />

$(document).ready(function () {

    //is novis report
    if (window.location.search.split("=")[1].match("Novis[0-9]*")) {
        $("#newTableButton").hide();
        $("#editModeButton").hide();
        $("#editModeButton").unbind();
        $("#copyTableButton").hide();
        $("#newTableButton").hide();
        $("#deleteTableButton").hide();
        $("#permissionsButton").hide();
        $("#changeTableButton").hide();
        $("#changeTableButton").unbind();
        $(".SupportFirstColumn .PlusButton").remove();
        $(".SupportFirstColumn .MinusButton").remove();

        // cursor change on ajax
        $(document).ajaxStart(function () {
            $('body').addClass('wait');

        }).ajaxComplete(function () {

            $('body').removeClass('wait');

        });
 
        $("#printTableButton").unbind();
        $("#printTableButton").prop('title', 'Report fertigstellen');
        $("#printTableButton").click(function () {
            var matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            params = {};

            var formData = $.toJSON(params);
            $.ajax({
                type: "POST",
                url: "../WebService/SokratesService.asmx/createNovisReport",
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
            var urlbase = document.URL.split("/Morph");
            window.open(urlbase[0] + "/Report/CrystalReportViewer.aspx?alias=NovisReport.rpt");

        });
    }
        //is novis template
    else {

        $("#ContentPlaceHolder1__pl__cl__detail_RemoveCell").click(function (event) { event.preventDefault; });
        
        // New Table
        $("#copyTableButton").hide();
        $("#copyTableButton").unbind();
        $("#copyTableButton").parent().remove();
        $("#newTableButton").unbind();
        $("#newTableButton").prop('title', 'Sokrateskarte Ableiten');
        $("#newTableButton").click(function () {
            if ($("#newTableButton").prop("disabled")==null || $("#newTableButton").prop("disabled")!="true"){
            $("#newTableButton").prop("disabled", "true");
            var matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
            $('body').addClass('wait');
            window.setTimeout(sendDerive, 10);
          
        }
        });
    }
});

function sendDerive() {

    params = {};
    params.matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");;

    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/deriveSokrates",
        cache: false,
        async: true,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            matrixId = msg.d; //JSON object mit eigenschaft "d"
            var newUrl = document.URL;
            newUrl = newUrl.split("?");
            window.location.href = newUrl[0] + "?matrixID=" + matrixId;
            $("#newTableButton").prop("disabled", "false");
            $('body').removeClass('wait');
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
            $("#newTableButton").prop("disabled", "false");
             $('body').removeClass('wait');
        }
    });

    
}


$(document).ready(function () {
//submatrix Menu
    var subMatrix;

    var matrixId = $("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class");
    params = {};
    params.matrixId = matrixId;
    var formData = $.toJSON(params);
    $.ajax({
        type: "POST",
        url: "../WebService/SokratesService.asmx/loadNovisSubmatrix",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            subMatrix = msg.d; //JSON object mit eigenschaft "d"
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });

    var el1 = $("#SubmatrixDropdown");
    $("#SubmatrixDropdown").children().remove();
    el1.innerHTML = '';
    for (var i = (subMatrix.length - 1) ; i >= 0; i--) {
        var foo2 = document.createElement("option");
        foo2.appendChild(document.createTextNode(subMatrix[i].title.toString()));
        foo2.value = subMatrix[i].id.toString();
        el1.append(foo2);
    }
});
