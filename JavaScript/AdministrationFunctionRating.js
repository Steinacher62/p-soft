var functionRatingTree;
var ratingTree;
var historyTable;
var rootItemTable;
var ratingDetailTable;


$(document).ready(function () {
    functionRatingTree = $find('ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl00_FunctionRatingnTree');
    ratingTree = $find('ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl03_FunctionRatingnRatingTree');
    historyTable = $find('ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl01_RatingHistoryGrid');
    rootItemTable = $find('ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl01_RatingItemsGrid');
    ratingDetailTable = $find('ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl02_RatingDetail');
    var paneWidth = $("#ctl00_ContentPlaceHolder1_LRORMRU_Layout_Splitter").css("width").replace("px", "");
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneL").css("width", "500px");
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneR").css("width", paneWidth - 500 + 'px');
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRO").css("width", paneWidth - 500 + 'px');
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRM").css("width", paneWidth - 500 + 'px');
    $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRU").css("width", paneWidth - 500 + 'px');
    PaneROResized();
    PaneRUResized();
    PaneLResized();
    PaneRMResized();
});

function PaneLResized() {
    var paneWidth = $("#ctl00_ContentPlaceHolder1_LRORMRU_Layout_Splitter").css("width").replace("px", "");
    var paneHeight = $("#ctl00_ContentPlaceHolder1_LRORMRU_Layout_Splitter").css("height").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl00_FunctionRatingnTree").css("height", paneHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl00_FunctionRatingnTree").css("width", paneWidth / 4 + 'px');
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneL').css('width', paneWidth / 4 + 10 + 'px')
}

function PaneROResized() {
    var PaneROHeight = $('#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRO').css('height').replace("px", "");
    historyTable.GridDataDiv.style.height =  PaneROHeight - 120 + 'px';
    rootItemTable.GridDataDiv.style.height = PaneROHeight - 45 + 'px';
}

function PaneRMResized() {
    var paneWidth = $("#ctl00_ContentPlaceHolder1_LRORMRU_Layout_Splitter").css("width").replace("px", "");
    var PaneROHeight = $('#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRM').css('height').replace("px", "");
    ratingDetailTable.GridDataDiv.style.height = PaneROHeight - 45 + 'px';
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl03_FunctionRatingnRatingTree').css("height", paneRUHeight - 10 + 'px');
}

function FunctionRatingTreeNodeDropping(sender, args) {
    if (args.get_destNode() === null || args.get_sourceNode().get_treeView().get_id() !== 'ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl00_FunctionRatingnTree') {
        args.set_cancel(true);
    }
    else {
        parameter = {};
        parameter.SourceFunctionRatingId = historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
        parameter.destFunctionId = args.get_destNode().get_value();
        detailDat = getData(parameter, 'CopyFunctionRating');

        args.get_destNode().set_imageUrl(detailDat[0].IMAGE);
        args.get_destNode().get_attributes().setAttribute("HASRATING", "1");
        args.get_destNode().set_selected(true);
        SetFunctionRating(args.get_destNode());
        }
        
}

function FunctionRatingTreeNodeDragStart(sender, args) {
    if (historyTable.get_masterTableView()._dataItems.length === 0) {
        
        radalert(Translate('selectRatingBevorCopy', 'functionRating')[0].Text, 400, 200,'', function() {
            args.set_cancel(true);
        });
    }
    if (args.get_node().get_attributes().getAttribute('HASRATING') === '0') {
        args.set_cancel(true);
    }
}

function FunctionRatingTreePopulating() {

}

function FunctionRatingTreeContextMenuShowing(sender, args) {
    args.get_node().select();
    SetFunctionRating(args.get_node());
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    switch (typ) {
        case 'FUNCTION':
            args.get_node().get_contextMenu().findItemByValue("Add").show();
            break;
        case 'FUNCTIONGROUP':
            args.get_node().get_contextMenu().findItemByValue("Add").hide();
            break;
    }
}

function FunctionRatingTreeMenuClicked(sender, args) {
    var command = args.get_menuItem().get_value();
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    nodeId = args.get_node().get_value();
    switch (command) {
        case 'Add':
            AddFunctionRating(functionRatingTree.get_selectedNode().get_value(), functionRatingTree.get_selectedNode().get_attributes().getAttribute('HASRATING'));
            break;
    }

}

function FunctionRatingTreeNodeClicked(sender, args) {
    SetFunctionRating(args.get_node());
}

function SetFunctionRating(functionNode) {
    ratingDetailTable.get_masterTableView().set_dataSource([]);
    ratingDetailTable.get_masterTableView().dataBind();
    ratingTree.get_nodes().clear();

    SetFuncionratingTot(functionNode.get_value());

    if (historyTable.get_masterTableView().get_selectedItems()[0] !== undefined && functionNode.get_attributes().getAttribute('HASRATING') === '1') {
        SetRootItems(historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID);
    }
    else {
        rootItemTable.get_masterTableView().set_dataSource('');
        rootItemTable.get_masterTableView().dataBind();
    }
}

function RatingItemsGridRowSelected(sender, args){
    parameter = {};
    if (args.get_item()._dataItem !== null) {
        parameter = {};
        parameter.id = args.get_item()._dataItem.ID;
        detailDat = getData(parameter, 'GetRatingTreeRoot');
        ratingTree.get_nodes().clear();
        var node = new Telerik.Web.UI.RadTreeNode();
        node.set_text(detailDat[0].BEZEICHNUNG);
        node.set_value(detailDat[0].ID);
       // node.set_imageUrl(node.IMAGE);
        node.get_attributes().setAttribute("TYP", "GROUP");
        node.get_attributes().setAttribute("ORDNUMBER", detailDat[0].ORDNUMBER);
        node.get_attributes().setAttribute("BESCHREIBUNG", detailDat[0].BESCHREIBUNG);
        node.get_attributes().setAttribute("ERLAEUTERUNG", detailDat[0].ERLAEUTERUNG);
        node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
        ratingTree.get_nodes().add(node);
        SetRequirement(historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID, rootItemTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID);
    }
}

function SetFuncionratingTot(functionID) {
    rootItemTable.get_masterTableView().set_dataSource('');
    rootItemTable.get_masterTableView().dataBind();

    parameter = {};
    parameter.id = functionID;
    detailData = getData(parameter, 'GetFunctionRating');
    historyTable.get_masterTableView().set_dataSource(detailData);
    historyTable.get_masterTableView().dataBind();
    var dataItems = historyTable.get_masterTableView().get_dataItems();
    for (var i = 0; i < dataItems.length; i++) {
        if (new Date(GetDateFromJson(dataItems[i]._dataItem.GUELTIG_BIS)) >= new Date()) {
            historyTable.get_masterTableView().clearSelectedItems();
            historyTable.get_masterTableView().selectItem(dataItems[i].get_element());
        }
    }

}

function SetRootItems(ratingId) {
    rootItemTable.get_masterTableView().set_dataSource('');
    rootItemTable.get_masterTableView().dataBind();
    parameter = {};
    parameter.id = ratingId;
    detailData = getData(parameter, 'GetFunctionratingRootItems');
    rootItemTable.get_masterTableView().set_dataSource(detailData);
    rootItemTable.get_masterTableView().dataBind();
}

function RatingHistoryGridRowSelected(sender, args) {
    SetRootItems(args.get_item()._dataItem.ID);
}

function SetRequirement(fbwId, rootItemId) {
    parameter = {};
    parameter.fbwId = fbwId;
    parameter.rootItemId = rootItemId;
    detailDat = getData(parameter, 'GeRequirement');
    ratingDetailTable.get_masterTableView().set_dataSource(detailDat);
    ratingDetailTable.get_masterTableView().dataBind();
    
    if (detailDat.length > 0) {
        detailDat.forEach(function (item) {
            ExpandRatingTreeToRequirementItem(item.ID, false);
        });
    }

}


function ExpandRatingTreeToRequirementItem(itemId ,calledSelf) {
        parameter = {};
        parameter.itemId = itemId;
        requirementItemIds = getData(parameter, "GetRatingTreeNodesToRequirementItem");
    
    for (i = requirementItemIds.length - 1; i >= 0; i--) {
        var node = ratingTree.findNodeByValue(requirementItemIds[i].PARENT_ID);
        if (node === null) {
            setTimeout(function () { ExpandRatingTreeToRequirementItem(itemId);}, 10);
        }
        else {

            node.expand();
        }
    }

}


function FunctionRatingRatingTreeMenuClicked(sender, args){

}

function FunctionRatingRatingTreeNodeClicked(sender, args) {

}

function FunctionRatingRatingTreeContextMenuShowing(sender, args) {
    args.get_node().select(true);
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    if (typ === 'NODE') {
        args.get_node().get_contextMenu().findItemByValue("ShowReference").show();
        args.get_node().get_contextMenu().findItemByValue("Description").show();
        var active = args.get_node().get_attributes().getAttribute('ACTIVE');
        if (active === '1') {
            args.get_node().get_contextMenu().findItemByValue("Add").hide();
            args.get_node().get_contextMenu().findItemByValue("Remove").show();
        }
        else {
            if (OtherRatingItemExist(args.get_node().get_value())) {
                //args.get_node().get_contextMenu().findItemByValue("Add").hide();
                args.get_node().get_contextMenu().findItemByValue("Add").show();
            }
            else {
                args.get_node().get_contextMenu().findItemByValue("Add").show();
            }
            args.get_node().get_contextMenu().findItemByValue("Remove").hide();
        }
    }
    else {
        args.get_node().get_contextMenu().findItemByValue("Add").hide();
        args.get_node().get_contextMenu().findItemByValue("Remove").hide();
        args.get_node().get_contextMenu().findItemByValue("ShowReference").hide();
        args.get_node().get_contextMenu().findItemByValue("Description").hide();
    }
}

function FunctionRatingRatingTreeContextMenuItemClicking(sender, args) {
    var command = args.get_menuItem().get_value();
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    nodeId = args.get_node().get_value();
    switch (command) {
        case 'Add':
            AddRatingItem(args.get_node().get_value());
            break;
        case 'Remove':
            RemoveRatingItem(args.get_node().get_value());
            break;
        case 'ShowReference':
            ShowReferences(args.get_node().get_value());
            break;
        case 'Description':
            ShowArgumentDetail(args.get_node().get_value());
            break;
    }
}

function AddRatingItem(itemId) {
    parameter = {};
    parameter.ratingId = historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
    parameter.argumentId = itemId;
    detailDat = getData(parameter, 'AddRatingItem');
    ratingTree.get_selectedNode().set_imageUrl(detailDat[0].IMAGE);
    ratingTree.get_selectedNode().get_attributes().setAttribute("ACTIVE", "1");
    SetRequirement(parameter.ratingId, rootItemTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID);
    var ratingId = historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
    var rootItemId = rootItemTable.get_selectedItems()[0]._dataItem.ID;
    SetfunctionRatingValue(ratingId);
    SetRootItemValue(rootItemId, ratingId);
}

function RemoveRatingItem(itemId) {
    parameter = {};
    parameter.ratingId = historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
    parameter.argumentId = itemId;
    detailDat = getData(parameter, 'RemoveRatingItem');
    ratingTree.get_selectedNode().set_imageUrl(detailDat[0].IMAGE);
    ratingTree.get_selectedNode().get_attributes().setAttribute("ACTIVE", "0");
    SetRequirement(parameter.ratingId, rootItemTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID);
    var ratingId = historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
    var rootItemId = rootItemTable.get_selectedItems()[0]._dataItem.ID;
    SetfunctionRatingValue(ratingId);
    SetRootItemValue(rootItemId, ratingId);
}

function SetfunctionRatingValue(ratingId) {
    parameter = {};
    parameter.id = ratingId;
    detailDat = getData(parameter, 'GetFunctionRatingValue');
    historyTable.get_selectedItems()[0].get_element().children[0].innerText = detailDat[0].FUNKTIONSWERT;
    historyTable.get_selectedItems()[0]._dataItem.FUNKTIONSWERT = detailDat[0].FUNKTIONSWERT;
}

function SetRootItemValue(itemId, ratingId) {
    parameter = {};
    parameter.itemId = itemId;
    parameter.ratingId = ratingId;
    detailDat = getData(parameter, 'GetRootItemValue');
    rootItemTable.get_selectedItems()[0].get_element().children[1].innerText = detailDat[0].Punkte;
    rootItemTable.get_selectedItems()[0]._dataItem.FUNKTIONSWERT = detailDat[0].Punkte;

}

function FunctionRatingRatingTreePopulating(sender, args) {
    args.get_context()["FUNKTION_ID"] = historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
}

function FunctionRatingRatingTreeNodeDragStart(sender, args) {
    var typ = args._node.get_attributes().getAttribute('TYP');
    if (typ === 'NODE') {
        if (OtherRatingItemExist(args._node.get_value())) {
            //args.set_cancel(true);
        }
    }
    else
    {
        args.set_cancel(true);
    }
}

function OtherRatingItemExist(itemId) {
    var exist = false;
    nodesInGroup = ratingTree.findNodeByValue(itemId).get_parent().get_nodes()._array;
    for (var i = 0; i < nodesInGroup.length - 1; i++) {
        if (nodesInGroup[i]._attributes._data.ACTIVE === '1') {
            exist = true;
            break;
        }
    }
    return exist;
}

function FunctionRatingRatingTreeNodeDropping(sender, args) {
    var top = $('#ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl02_RatingDetail').offset().top;
    var left = $('#ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl02_RatingDetail').offset().left;
    var reight = left + parseInt($('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRM')[0].style.width.replace('px', ''));
    var bottom = top + parseInt($('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORMRU_Layout_PaneRM')[0].style.height.replace('px', ''));
    var dropX = args._domEvent.clientX;
    var dropY = args._domEvent.clientY;
    if (dropY > top && dropY < bottom && dropX > left && dropX < reight) {
        AddRatingItem(args.get_sourceNode().get_value());
    }
}

function AddFunctionRatingClick() {
    if (!(functionRatingTree.get_selectedNode() === null) && functionRatingTree.get_selectedNode().get_attributes().getAttribute('TYP') === 'FUNCTION') {
        AddFunctionRating(functionRatingTree.get_selectedNode().get_value());
    }
    else {
        parameter = {};
        parameter.text = 'selectFunctionFirstAdd';
        parameter.scope = 'functionRating';
        radalert(getData(parameter, 'Translate')[0].Text, 400, 200);
    }

}

function AddFunctionRating(functionId) {
    var selectedNode = functionRatingTree.get_selectedNode();
    parameter = {};
    parameter.id = selectedNode.get_value();
    detailDat = getData(parameter, 'AddFunctionRating');
    selectedNode.set_imageUrl(detailDat[0].IMAGE);
    selectedNode.get_attributes().setAttribute("HASRATING", "1");
    SetFunctionRating(functionRatingTree.get_selectedNode());
}

function DeleteFunctionRating(ratingId) {
    confirmWindow = radconfirm(Translate('deleteRatingConfirm', 'functionRating')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            parameter = {};
            parameter.id = ratingId;
            detailDat = getData(parameter, "DeleteFunctionRating");
            if (historyTable.get_masterTableView()._dataItems.length === 1) {
                functionRatingTree.get_selectedNode().get_attributes().setAttribute("HASRATING", "0");
                functionRatingTree.get_selectedNode().set_imageUrl(detailDat[0].IMAGE);
            }
            SetFunctionRating(functionRatingTree.get_selectedNode());
        }
    });
}

function SaveFunctionRatingClicking() {

}

function DeleteFunctionRatingClicking() {
    if (!(functionRatingTree.get_selectedNode() === null) && functionRatingTree.get_selectedNode().get_attributes().getAttribute('TYP') === 'FUNCTION') {
        DeleteFunctionRating(historyTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID);
    }
    else {
        parameter = {};
        parameter.text = 'selectFunctionFirstDelete';
        parameter.scope = 'functionRating';
        radalert(getData(parameter, 'Translate')[0].Text, 400, 200);
    }
}

function RowContextMenu(sender, args) {
    var menu = $find("ctl00_ContentPlaceHolder1_LRORMRU_Layout_ctl02_RadMenu1");
    var evt = args.get_domEvent();
    if (evt.target.tagName === "INPUT" || evt.target.tagName === "A") {
        return;
    }

    var index = args.get_itemIndexHierarchical();
    //document.getElementById("radGridClickedRowIndex").value = index;

    sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);

    menu.show(evt);

    evt.cancelBubble = true;
    evt.returnValue = false;

    if (evt.stopPropagation) {
        evt.stopPropagation();
        evt.preventDefault();
    }
}

function RadMenu1Clicked(sender, args) {
    var selectedItem = args._item.get_value();
    var selectedRowId = ratingDetailTable.get_masterTableView().get_selectedItems()[0]._dataItem.ID;
    switch(selectedItem) {
        case 'Delete':
            parameter = {};
            parameter.anforderungId = selectedRowId;
            detailDat = getData(parameter, 'GetArgumentFromAnforderung');
            ratingTree.findNodeByValue(detailDat[0].FBW_ARGUMENT_ID).set_selected(true);
            RemoveRatingItem(detailDat[0].FBW_ARGUMENT_ID);
        break;
        case 'ShowReference':
            parameter = {};
            parameter.anforderungId = selectedRowId;
            detailDat = getData(parameter, 'GetArgumentFromAnforderung');
            ShowReferences(detailDat[0].FBW_ARGUMENT_ID);
            
            break;
        case 'Description':
            parameter = {};
            parameter.anforderungId = selectedRowId;
            detailDat = getData(parameter, 'GetArgumentFromAnforderung');
            ShowArgumentDetail(detailDat[0].FBW_ARGUMENT_ID);
            break;
    }
}

function ShowReferences(argumentId) {
    parameter = {};
    parameter.argumentId = argumentId;
    detailDat = getData(parameter, 'GetFunctionsArgumentReference');
    var tableView = $find('ctl00_ContentPlaceHolder1_RatingReferences_C_RatingDetail').get_masterTableView();
    tableView.set_dataSource(detailDat);
    tableView.dataBind();

    $find('ctl00_ContentPlaceHolder1_RatingReferences').show();
}

function ShowArgumentDetail(argumentId) {
    parameter = {};
    parameter.argumentId = argumentId;
    detailDat = getData(parameter, 'GetArgumentDetail');
    $('#ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemTitleData')[0].innerText = detailDat[0].BEZEICHNUNG;
    $('#ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemValueData')[0].innerText = detailDat[0].PUNKTEZAHL;
    $find('ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemDescriptionData').set_value(detailDat[0].BESCHREIBUNG);
    $find('ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemExamplesData').set_value(detailDat[0].ERLAEUTERUNG);
    $('#ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemId')[0].value = detailDat[0].ID;
    $find('ctl00_ContentPlaceHolder1_RatingItemDetailWindow').show();

}

function SaveAnforderrungDetailClick() {
    parameter = {};
    parameter.id = $('#ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemId')[0].value;
    parameter.description = $find('ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemDescriptionData').get_value();
    parameter.example = $find('ctl00_ContentPlaceHolder1_RatingItemDetailWindow_C_itemExamplesData').get_value();
    getData(parameter, 'SaveArgumentDetail');
}