/// <reference path="../Scripts/jquery-3.3.1.min.js" />


var userGrid;
var dataSource;
var dataSourceSeminars;

Sys.Application.add_load(function () {
    userGrid = $find("ctl00_ContentPlaceHolder1__pl__cl__detail_GridUser");
    dataSource = $find("ContentPlaceHolder1__pl__cl__detail_ClientDataSource");
    ContentResized();
    userGrid.get_element().style.width = ($("#content").width() - 20).toString() + "px";
    $("#ctl00_ContentPlaceHolder1__pl__cl__detail_GridUser_ctl00_ctl02_ctl00_AddNewRecordButton")[0].style.display = 'none';
    //$find("ctl00_ContentPlaceHolder1__pl__cl__detail_btnCardSeminar").set_visible(false);
    $(".btnUserSeminar").css("display", "none");
    if ($("#showImportErrors")[0].value === 'True') {
        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ImportError").Show();
    }
});   

$(window).resize(function () {
    ContentResized();
});

function ContentResized() {
    var contentHeight = $(document).height();
    userGrid._gridDataDiv.style.height = contentHeight -360 + "px";
}

$(function () {
    var Hub = $.connection.psoftHub;

    Hub.client.recieveNotification = function (message) {
        if (message.length > 0) {
            $("#RadWindowWrapper_ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar").hide();
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").show();
            $("#ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification_C")[0].innerHTML = message;
        }
        else {
            $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").hide();
            $("#RadWindowWrapper_ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar").show();
        }
    };
    $.connection.hub.start().done(function () {
        Hub.server.sendNotifications("");
    }).fail(function (e) {
        alert(e);
    });

});

function ParameterMap(sender, args) {
    //If you want to send a parameter to the select call you can modify the if 
    //statement to check whether the request type is 'read':
    //if (args.get_type() == "read" && args.get_data()) {
    if (args.get_type() !== "read" && args.get_data()) {
        args.set_parameterFormat({ userJSON: kendo.stringify(args.get_data().models) });
    }
}

function Parse(sender, args) {
    var response = args.get_response().d;
    if (response) {
        args.set_parsedData(JSON.parse(response));
    }

}

function UserAction(sender, args) {
    if (sender.get_batchEditingManager().hasChanges(sender.get_masterTableView()) &&
        !confirm("Bei dieser Operation gehen alle Änderungen verloren. Wollen Sie die Operation trotzdem durchführen?")) {
        args.set_cancel(true);
    }

}

function ddSeminarsItemSelected(sender, args) {
    setddSeminarValue(sender._selectedValue);
}

function setddSeminarValue(seminarId) {
    var params = {};
    params.SeminarId = seminarId;
    var formData = $.toJSON(params);

    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/SetSelectedSeminarId",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
            userGrid.MasterTableView.rebind();
            if ($find("ctl00_ContentPlaceHolder1__pl__cl__detail_ddSeminars")._selectedText === '') {
                $("#ctl00_ContentPlaceHolder1__pl__cl__detail_GridUser_ctl00_ctl02_ctl00_AddNewRecordButton")[0].style.display = 'none';
                $(".btnUserSeminar").css("display", "none");
            }
            else {
                $("#ctl00_ContentPlaceHolder1__pl__cl__detail_GridUser_ctl00_ctl02_ctl00_AddNewRecordButton")[0].style.display = '';
                $(".btnUserSeminar").css("display", "");
            }
        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });

}

function ParameterMapSeminars(sender, args) {
    //If you want to send a parameter to the select call you can modify the if 
    //statement to check whether the request type is 'read':
    //if (args.get_type() == "read" && args.get_data()) {
    if (args.get_type() !== "read" && args.get_data()) {
        args.set_parameterFormat({ seminarsJSON: kendo.stringify(args.get_data().models) });
    }
}

function ParseSeminars(sender, args) {
    var response = args.get_response().d;
    if (response) {
        args.set_parsedData(JSON.parse(response));
    }
}

function btnSeminarAdmin(args) {
    userId = $("#" + args).parents("tr.rgRow, tr.rgAltRow").children("td:nth-child(1)").text();
    SetPersonId(userId);
    var userName = $("#" + args).parents("tr.rgRow, tr.rgAltRow").children("td:nth-child(2)").text();
    var userFirstname = $("#" + args).parents("tr.rgRow, tr.rgAltRow").children("td:nth-child(3)").text();
    ctl00_ContentPlaceHolder1__pl__cl__detail_SeminarManagement_C_SeminarverwaltungUserName.innerHTML = userName;
    ctl00_ContentPlaceHolder1__pl__cl__detail_SeminarManagement_C_SeminarverwaltungUserFirstname.innerHTML = userFirstname;

    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_SeminarManagement_C_GridSeminars").get_masterTableView().rebind();
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_SeminarManagement").show();
}

function UserSeminarBound() {
    var dataSourceTmp = $find("ctl00_ContentPlaceHolder1__pl__cl__detail_SeminarManagement_C_GridSeminars").get_masterTableView()._dataSource;
    $.each(dataSourceTmp, function (index) { $(".CB1Class")[index].children[0].checked = dataSourceTmp[index].ISACTIVE; });
}

function SetPersonId(id) {
    var params = {};
    params.id = id;
    var formData = $.toJSON(params);

    $.ajax({
        type: "POST",
        url: "../WebService/SBSService.asmx/SetPersonId",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {
        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });
}

function changeEditor(sender, args) {
    var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1__pl__cl__detail_SeminarManagement_C_GridSeminars")).get_batchEditingManager();
    var cb = $("#" + sender.id);
    batchManager.openCellForEdit(sender.parentElement.parentElement);
    sender.checked = !sender.checked;
    setTimeout(function () {
        batchManager._tryCloseEdits(document);
    }, 500);
}

function ParameterMapMatrix(sender, args) {
    //If you want to send a parameter to the select call you can modify the if 
    //statement to check whether the request type is 'read':
    //if (args.get_type() == "read" && args.get_data()) {
    if (args.get_type() !== "read" && args.get_data()) {
        args.set_parameterFormat({ matrixJSON: kendo.stringify(args.get_data().models) });
    }
}

function ParameterMapMatrixSeminar(sender, args) {
    //If you want to send a parameter to the select call you can modify the if 
    //statement to check whether the request type is 'read':
    //if (args.get_type() == "read" && args.get_data()) {
    if (args.get_type() !== "read" && args.get_data()) {
        args.set_parameterFormat({ matrixJSON: kendo.stringify(args.get_data().models) });
    }
}

function ParseMatrix(sender, args) {
    var response = args.get_response().d;
    if (response) {
        args.set_parsedData(JSON.parse(response));
    }
}

function ParseMatrixSeminar(sender, args) {
    var response = args.get_response().d;
    if (response) {
        args.set_parsedData(JSON.parse(response));
    }
}

function btnMatrixAdmin(args) {
    userId = $("#" + args).parents("tr.rgRow, tr.rgAltRow").children("td:nth-child(1)").text();
    var userName = $("#" + args).parents("tr.rgRow, tr.rgAltRow").children("td:nth-child(2)").text();
    var userFirstname = $("#" + args).parents("tr.rgRow, tr.rgAltRow").children("td:nth-child(3)").text();
    ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement_C_MatrixverwaltungUserName.innerHTML = userName;
    ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement_C_MatrixverwaltungUserFirstname.innerHTML = userFirstname;

    SetPersonId(userId);
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement_C_GridMatrix").get_masterTableView().rebind();

    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement").show();
}

function btnISActiveCardClicked(sender, args) {
    var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement_C_GridMatrix")).get_batchEditingManager();
    var cb = $("#" + sender.id);
    batchManager.openCellForEdit(sender.parentElement.parentElement);
    sender.checked = !sender.checked;
    setTimeout(function () {
        batchManager._tryCloseEdits(document);
    }, 500);

}

function btnReadWriteCardClicked(sender, args) {
    var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement_C_GridMatrix")).get_batchEditingManager();
    var cb = $("#" + sender.id);
    batchManager.openCellForEdit(sender.parentElement.parentElement);
    sender.checked = !sender.checked;
    setTimeout(function () {
        batchManager._tryCloseEdits(document);
    }, 500);
}

function UserMatrixBound() {
    var dataSourceTmp = $find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagement_C_GridMatrix").get_masterTableView()._dataSource;
    $.each(dataSourceTmp, function (index) { $(".CB3Class")[index].children[0].checked = dataSourceTmp[index].ISACTIVE; });
    $.each(dataSourceTmp, function (index) { $(".CB5Class")[index].children[0].checked = dataSourceTmp[index].READWRITE; });
}

function btnCardSeminarClicked(sender, eventArgs) {
    ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar_C_lblSeminar.innerHTML = $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ddSeminars")._selectedText;
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar_C_GridMatrixSeminar").get_masterTableView().rebind();
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar").show();
    eventArgs.set_cancel(true);
}

function btnImportUserClicked(sender, eventArgs) {
    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_ExcelImport").show();
}

function btnReadWriteCardSeminarClicked(sender, args) {
    var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar_C_GridMatrixSeminar")).get_batchEditingManager();
    var cb = $("#" + sender.id);
    batchManager.openCellForEdit(sender.parentElement.parentElement);
    sender.checked = !sender.checked;
    setTimeout(function () {
        batchManager._tryCloseEdits(document);
    }, 500);
}

function btnISActiveCardSeminarClicked(sender, args) {
    var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar_C_GridMatrixSeminar")).get_batchEditingManager();
    var cb = $("#" + sender.id);
    batchManager.openCellForEdit(sender.parentElement.parentElement);
    sender.checked = !sender.checked;
    setTimeout(function () {
        batchManager._tryCloseEdits(document);
    }, 500);

}

function SeminarMatrixBound() {
    var dataSourceTmp = $find("ctl00_ContentPlaceHolder1__pl__cl__detail_MatrixManagementSeminar_C_GridMatrixSeminar").get_masterTableView()._dataSource;
    $.each(dataSourceTmp, function (index) { $(".CB7Class")[index].children[0].checked = dataSourceTmp[index].ISACTIVE; });
    $.each(dataSourceTmp, function (index) { $(".CB9Class")[index].children[0].checked = dataSourceTmp[index].READWRITE; });
}

function UpdateNotification(sender, args) {

    $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").set_text(args._content.d);

}

function SeminaMatrixOnCommand(sender, args) {
    if (args.get_commandName() === 'update') {

        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").show();
        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").set_updateInterval(2000);
        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").set_autoCloseDelay(0);
    }
}

function NotificationUpdated(sender, args) {
    if ($("#ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification_XmlPanel")[0].innerText.length > 0) {
        sender.show();
        sender.set_updateInterval(2000);
        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").set_autoCloseDelay(2100);
    }
    else {
        sender.set_updateInterval(0);
        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").set_updateInterval(0);
        $find("ctl00_ContentPlaceHolder1__pl__cl__detail_CopyMapNotification").hidde();
        sender.hide();
    }
}
