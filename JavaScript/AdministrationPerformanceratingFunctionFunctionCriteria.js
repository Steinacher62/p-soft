var functionTree;
var criteriaTable;
$(document).ready(function () {
    functionTree = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FunctionTree');
    criteriaTable = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionCriteriaGrid');
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneR').css("width", "");
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL").css("width", "600px");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_MultiPageRO').css("height", "100%");
    PaneLResized();
    PaneROResized();
    ClearDetailData();
});


function PaneLResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FunctionTree").css("height", paneLHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FunctionTree").css("width", paneLWidth - 10 + 'px');
}

function PaneROResized() {
    var windowWidth = $('#ctl00_ContentPlaceHolder1_LRORU_Layout_Splitter').css("width").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL").css("width").replace("px", "");
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRO").css("width", windowWidth - paneLWidth + 'px');
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_SplitterR').css("width", windowWidth - paneLWidth + 'px');
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU').css("width", windowWidth - paneLWidth + 'px');

}

function PaneRUResized() {

}

function ClearDetailData() {
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_FunctionCriteriaTyp').set_checked(0);
}

function FunctionCriteriaTypChanged(sender, args) {

}

function FunctionCriteriaGridOnRowSelected(sender, args) {

}

function BatchEditOpening(sender, args) {
    if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_FunctionCriteriaTyp').get_checked()) {
        args.set_cancel(true);
    }
}

function FunctionTreeNodeClicked(sender, args) {
    var parameter = {};
    parameter.id = args.get_node().get_value();
    detailData = getData(parameter, 'GetFunctionWaigthingTable');
    if (detailData !== '') {
        criteriaTable.get_masterTableView()._dataSource = detailData;
        criteriaTable.get_masterTableView().dataBind();
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_FunctionCriteriaTyp').set_checked(detailData[0].AUTOWEIGHT);
    }
}

function SaveClick() {
    var totalWaighting = 0;
    var originalDataItems = criteriaTable.get_masterTableView()._dataItems;
    var rows = $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionCriteriaGrid_ctl00 .rgRow, .rgAltRow').toArray();
 
    var items = [];
    rows.forEach(function (row) {
        weight = parseFloat(row.cells[2].innerText);
       
        originalDataItems.forEach(function (originalItem) {
            tableRowId = row.id;
            originalId = originalItem._element.id;
            if (tableRowId === originalId) {
                var item = {};
                item.Id = originalItem._dataItem.ID;
                item.weight = weight;
                items.push(item);
                totalWaighting += parseFloat(weight);
            }
        }); 
    });

    if (totalWaighting !== 100 && totalWaighting !== 0 ) {

        radalert(Translate('totalFunctionCriteriasWrong', 'performanceRating')[0].Text, 400, 200, '');
        return;
    }
    else {
        var parameter = {};
        parameter.id = functionTree.get_selectedNode().get_value();
        parameter.autoweight = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_FunctionCriteriaTyp').get_checked();
        parameter.items = items;
        detailDat = getData(parameter, "SaveFunctionCriteriasWeight");

        $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionCriteriaGrid td').removeClass('rgBatchChanged');
    }




}

