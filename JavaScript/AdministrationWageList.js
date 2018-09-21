

$(document).ready(function () {
    var contentHeight = $('#content').css('height').replace('px', '');
    $('#ctl00_ContentPlaceHolder1_L_Layout_ctl00_WageGrid_GridData').css('height', contentHeight - 350 + 'px');
    window.onresize = function () {
        var contentHeight = $('#content').css('height').replace('px', '');

        $('#ctl00_ContentPlaceHolder1_L_Layout_ctl00_WageGrid_GridData').css('height', contentHeight - 350 + 'px');
    };
});

function WageParameterMap(sender, args) {
    //If you want to send a parameter to the select call you can modify the if 
    //statement to check whether the request type is 'read':
    //if (args.get_type() == "read" && args.get_data()) {
    if (args.get_type() !== "read" && args.get_data()) {
        args.set_parameterFormat({ wageJSON: kendo.stringify(args.get_data().models) });
    }
}

function WageParse(sender, args) {
    if (sender._kendoDataSource._data.length === 0) {
        var response = args.get_response().d;
        if (response) {
            args.set_parsedData(JSON.parse(response));
        }

    }
    else {
        var dataSourceTmp = $find("ctl00_ContentPlaceHolder1_L_Layout_ctl00_WageGrid").get_masterTableView()._dataSource;
        $.each(dataSourceTmp, function (index) { $(".CB1Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION_WAGE; });
        $.each(dataSourceTmp, function (index) { $(".CB3Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION; });
    }

}

function changeEditor(sender, args) {
    var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1_L_Layout_ctl00_WageGrid")).get_batchEditingManager();
    //if (batchManager.get_currentlyEditedRow() == null) {
    batchManager.openRowForEdit(sender.parentElement.parentElement.parentElement.parentElement);
    //}
    var cb = $("#" + sender.id);
    batchManager.openCellForEdit(sender.parentElement.parentElement);
    sender.checked = !sender.checked;
    setTimeout(function () {
        batchManager._tryCloseEdits(document);
    }, 500);
}

function WageOnDataBound() {
    var dataSourceTmp = $find("ctl00_ContentPlaceHolder1_L_Layout_ctl00_WageGrid").get_masterTableView().get_dataSource();
    $.each(dataSourceTmp, function (index) {
        $(".CB1Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION_WAGE;
    });
    $.each(dataSourceTmp, function (index) {
        $(".CB3Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION;
    });
}

function OnKeyPress(sender, args) {

    if (args._keyCode === 13) {
        args._domEvent.stopPropagation();
        args._domEvent.preventDefault();
    }
}

function RequestFailed(sender, args) {
    alert("Bei der Operation ist ein Fehler aufgetreten! Bitte laden Sie die Seite mittels F5 neu.");
}