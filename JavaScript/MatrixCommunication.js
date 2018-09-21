/// <reference path="../Scripts/jquery.signalR-2.2.3.js" />
/// <reference path="../Scripts/jquery-3.3.1.min.js" />

//var Hub;

//$(function () {
//    Hub = $.connection.psoftHub;

//    Hub.client.updateCell = function (message) {
//        if ($("#ContentPlaceHolder1__pl__cl__detail_MainTable").attr("Class") == message[0]) {
//            if ($("#" + message[1])[0].children[0] == undefined) {
//                $("#" + message[1])[0].innerText = message[2];
//            }
//            else {
//                $("#" + message[1])[0].children[0].value = message[2];
//            }

//        };
//    };

//    Hub.client.deleteCell = function (cellId) {
//        if ($("#" + cellId).length > 0) {
//            var Row = $("#" + cellId).parents(".MainRow");
//            removeWirkungselemte(getCoords("#" + cellId)[1]);
//            drawWirkungselemente();
//            $("#" + cellId).remove();
//            addCellAfter($(Row).children(".MainCell").filter(":last-child"), true);
//        }
//    };

//    Hub.client.updateText = function (message) {

//        //$("#" + message[1]).text(message[2]);
//    }
        
    

//    $.connection.hub.start().done(function () {
        
//    }).fail(function (e) {
//        alert(e);
//    });

//});

//function sendCellChanges(matrixId, cellId, text) {
//    Hub.server.sendCellChange(matrixId, cellId, text);
//};

//function sendTextChange(matrixId, elementID, newText) {
//    Hub.server.sendTextChange(matrixId, elementID, newText)
//}

//function sendCellDeleted(cellId) {
//    Hub.server.sendCellDeleted(cellId);
//}

