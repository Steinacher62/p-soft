
var chartListBox;
var textLinkGrid;
var iconLinkGrid;
var oeTree;

$(document).ready(function () {
    chartListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_ChartListBox');
    textLinkGrid = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_TextLinkGrid');
    iconLinkGrid = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_IconLinkGrid');
    oeTree = $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_OETree1');
    PaneRUResized();
    PaneLResized();
});

function PaneROResized() {

}

function PaneLResized() {
    var paneLwidth = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL').css('width').replace('px', '');
    var paneLheight = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL').css('height').replace('px', '');
    //$('#ContentPlaceHolder1_LRORU_Layout_PageViewL .CommandRow').css('width', paneLwidth + 'px');
    $('#imageDiv').css('width', paneLwidth - 3 + 'px');
    $('#imageDiv').css('height', paneLheight - 55 + 'px');
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_ChartListBox').css("height", paneRUHeight - 22 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetChartList');
    chartListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        chartListBox.get_items().add(item);
    });
    ClearChartDetail();

}

function OEPaneResizing() {
    var paneHeight = $('#ctl00_ContentPlaceHolder1_NewChartWindow_C_OEPane').css('height').replace('px', '');
    var paneWidth = $('#ctl00_ContentPlaceHolder1_NewChartWindow_C_OEPane').css('width').replace('px', '');
    $('#ctl00_ContentPlaceHolder1_NewChartWindow_C_OETree1').css('height', paneHeight - 57 + 'px');
    $('#ctl00_ContentPlaceHolder1_NewChartWindow_C_OETree1').css('width', paneWidth + 'px');

}

function OETree1NodeExpanded(sender, args) {
    OEPaneResizing();
}

function ChartListBoxIndexChanged(sender, args) {
    SetChart(chartListBox.get_selectedItem().get_value());
}

function SetChart(chartId) {
    $('#content').css('cursor', 'wait');
    parameter = {};
    parameter.chartId = chartId;
    detailData = getData(parameter, 'GetChart');
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_navigationImage').attr("src", detailData[0].path);
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_navigationImage').attr("width", detailData[0].imageWidth);
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_navigationImage').attr("height", detailData[0].imageHeight);

    $('#TreeMapId')[0].innerHTML = detailData[0].map;
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_Label1')[0].innerHTML = detailData[0].chartTitle;
    $('#content').css('cursor', '');
}

function OrgentityClicked(id) {

    var chartId = chartListBox.get_selectedItem().get_value();

    //is root?
    if (IsRootnode(id)) {
        var chartDetail = GetChartDetailData(chartId);
        SetChartDetailData(chartDetail);
        var nodeDetail = GetNodeDetail(id);
        SetNodeDetailData(nodeDetail);
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_PaneOL').Expand();
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail').show();
    }
    else {
        var nodeDetail = GetNodeDetail(id);
        SetNodeDetailData(nodeDetail);
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_PaneOL').Collapse();
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail').show();
    }
    SetTexts(id);
    SetIconLinks(id);
}

function IsRootnode(nodeId) {
    var chartId = chartListBox.get_selectedItem().get_value();
    if (nodeId - 1 === chartId) {
        return true;
    }
    else {
        return false;
    }
}

function GetChartDetailData(chartId) {
    parameter = {};
    parameter.id = chartId;
    return detailData = getData(parameter, "GetChartDetail");
}

function SetChartDetailData(chartDetail) {
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_OrganisationData')[0].innerHTML = chartDetail[0].ORGANISATION;
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_LayoutData').findItemByValue(chartDetail[0].CHARTLAYOUT_ID).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_TextLayoutData').findItemByValue(chartDetail[0].TEXTLAYOUT_ID).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_AligmentData').findItemByValue(chartDetail[0].CHARTALIGNMENT_ID).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NameData').set_value(chartDetail[0].TITLE);
}

function GetNodeDetail(id) {
    parameter = {};
    parameter.id = id;
    return detailData = getData(parameter, "GetNodeDetail");
}

function SetNodeDetailData(nodeDetail) {
    ClearNodeDetail();
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value = nodeDetail[0].ID;
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_ChartData')[0].innerHTML = nodeDetail[0].CHART;
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_OrgentityData')[0].innerHTML = nodeDetail[0].ORGENTITY;
    if (nodeDetail[0].LAYOUT_ID !== null) {
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeLayoutData').findItemByValue(nodeDetail[0].LAYOUT_ID).select();
    }
    if (nodeDetail[0].CHILDLAYOUT_ID !== null) {
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_SubNodeLayoutData').findItemByValue(nodeDetail[0].CHILDLAYOUT_ID).select();
    }
    if (nodeDetail[0].CHARTALIGNMENT_ID !== null) {
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeAligmentData').findItemByValue(nodeDetail[0].CHARTALIGNMENT_ID).select();
    }
    if (nodeDetail[0].TEXTLAYOUT_ID !== null) {
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeTextLayoutData').findItemByValue(nodeDetail[0].TEXTLAYOUT_ID).select();
    }
    if (nodeDetail[0].TYP !== null) {
        $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeTypData').findItemByValue(nodeDetail[0].TYP).select();
    }

    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeShowPersonData').set_checked(nodeDetail[0].SHOWEMPLOYEES);
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeOffsetVerticalLineData').set_value(nodeDetail[0].VERTICAL_ALIGN_OFFSET);
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeHorizontalSpaceData').set_value(nodeDetail[0].GAP_VERTICAL);
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeVerticalSpaceData').set_value(nodeDetail[0].GAP_HORIZONTAL);
}

function ClearNodeDetail() {
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value = 0;
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_ChartData')[0].innerHTML = '';
    $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_OrgentityData')[0].innerHTML = '';
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeLayoutData').findItemByValue(0).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_SubNodeLayoutData').findItemByValue(0).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeAligmentData').findItemByValue(0).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeTextLayoutData').findItemByValue(0).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeTypData').findItemByValue(0).select();
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeShowPersonData').set_checked(0);
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeOffsetVerticalLineData').set_value('');
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeHorizontalSpaceData').set_value('');
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeVerticalSpaceData').set_value('');
}

function ClearChartDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_navigationImage').attr("src", '');
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_navigationImage').attr("width", 0);
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_navigationImage').attr("height", 0);
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_Label1')[0].innerHTML = '';
}

function SetTexts(nodeId) {
    parameter = {};
    parameter.nodeId = nodeId;
    detailData = getData(parameter, 'GetNodeTexts');
    textLinkGrid.get_masterTableView().set_dataSource(detailData);
    textLinkGrid.get_masterTableView().dataBind();

}

function SetIconLinks(nodeId) {
    parameter = {};
    parameter.nodeId = nodeId;
    detailData = getData(parameter, 'GetNodeIconLinks');
    iconLinkGrid.get_masterTableView().set_dataSource(detailData);
    iconLinkGrid.get_masterTableView().dataBind();
}

function AddClick() {
    $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_NewChartNameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_NewChartWindow').show();
}

function DeleteClick() {
    var confirmWindow = radconfirm(Translate('deleteChartConfirm', 'chart')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            DeleteChart(chartListBox.get_selectedItem().get_value());
        }
    });
}

function DeleteChart(id) {
    parameter = {};
    parameter.id = id;
    getData(parameter, 'DeleteChart');
    ClearChartDetail();
    chartListBox.get_items().remove(chartListBox.get_selectedItem());

}

function SaveChartDetailClick() {
    var chartId = chartListBox.get_selectedItem().get_value();
    var nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
    if (nodeId !== "0") {
        if (IsRootnode(nodeId)) {
            SaveChartDetailData(chartId);
        }
        ClearChartDetail();
        SaveNodeDetailData(nodeId);
        SetChart(chartId);
    }
}

function SaveChartDetailData(chartId) {
    parameter = {};
    parameter.chartId = chartId;
    parameter.layoutId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_LayoutData').get_selectedItem().get_value();
    parameter.textLayoutId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_TextLayoutData').get_selectedItem().get_value();
    parameter.aligId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_AligmentData').get_selectedItem().get_value();
    parameter.title = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NameData').get_value();
    detailData = getData(parameter, 'SaveChartDetail');

}

function SaveNodeDetailData(nodeId) {
    parameter.nodeId = nodeId;
    parameter.layoutId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeLayoutData').get_selectedItem().get_value();
    parameter.layoutSubnodesId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_SubNodeLayoutData').get_selectedItem().get_value();
    parameter.textLayoutId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeTextLayoutData').get_selectedItem().get_value();
    parameter.aligId = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeAligmentData').get_selectedItem().get_value();
    parameter.typ = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeTypData').get_selectedItem().get_value();
    parameter.showPerson = $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeShowPersonData').get_checked();
    detailData = getData(parameter, 'SaveNodeDetail');

}

function SaveChartNodeDetailClick() {

}

function ContextMenuChartItemShowing(sender, args) {
    var itemId = args.get_targetElement().id;
    if (IsRootnode(itemId)){
        sender.findItemByValue("MoveBefore").hide();
        sender.findItemByValue("MoveAfter").hide();
        sender.findItemByValue("Delete").hide();
    }
    else {
        sender.findItemByValue("MoveBefore").show();
        sender.findItemByValue("MoveAfter").show();
        sender.findItemByValue("Delete").show();
    }

}

function ContextMenuChartItemClientItemClicked(sender, args) {
    var nodeId = args.get_targetElement().id;
    var command = args.get_item().get_value();
    switch (command) {
        case 'Delete':
            DeleteChartNode(nodeId, false);
            break;
        case 'MoveBefore':
            MoveNodeBefore(nodeId);
            break;
        case 'MoveAfter':
            MoveNodeAfter(nodeId);
            break;
        case 'InsertMissigItems':
            InsertMissingNodes(nodeId);
            break;
        case 'DeleteSubOe':
            DeleteChartNode(nodeId, true);
            break;
    }
}

function DeleteChartNode(nodeId, deleteSubnodes) {
    var confirmWindow = radconfirm(Translate('deleteChartNodeConfirm', 'chartnode')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            parameter = {};
            parameter.chartId = chartListBox.get_selectedItem().get_value();
            parameter.nodeId = nodeId;
            parameter.deleteSubnodes = deleteSubnodes;
            getData(parameter, 'DeleteChartnode');
            ClearChartDetail();
            SetChart(chartListBox.get_selectedItem().get_value());
        }
    });
}

function MoveNodeBefore(nodeId) {
    parameter = {};
    parameter.nodeId = nodeId;
    getData(parameter, 'MoveNodeBefore');
    ClearChartDetail();
    SetChart(chartListBox.get_selectedItem().get_value());
}

function MoveNodeAfter(nodeId) {
    parameter = {};
    parameter.nodeId = nodeId;
    getData(parameter, 'MoveNodeAfter');
    ClearChartDetail();
    SetChart(chartListBox.get_selectedItem().get_value());

}

function InsertMissingNodes(nodeId) {
    parameter = {};
    parameter.nodeId = nodeId;
    getData(parameter, 'InsertMissingNode');
    ClearChartDetail();
    document.body.style.cursor = 'wait';
    SetChart(chartListBox.get_selectedItem().get_value());
    document.body.style.cursor = '';
}

function SaveNewChartClick() {
    if (oeTree.get_selectedNode() !== null) {
        ValidatorEnable('tableNewChart');
        var isValid = Page_ClientValidate();
        ValidatorDisable('tableNewChart');
        if (isValid) {
            $find("ctl00_ProgressWindow").show();
            var parameter = {};
            parameter.oeId = oeTree.get_selectedNode().get_value();
            parameter.title = $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_NewChartNameData').get_value();
            parameter.layoutId = $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_NewChartNodeLayoutData').get_selectedItem().get_value();
            parameter.textLayoutId = $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_NewChartNodeTextLayoutData').get_selectedItem().get_value();
            parameter.aligId = $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_NewChartTextAligmnetData').get_selectedItem().get_value();
            detailData = getData(parameter, 'CreateNewChart');

            var item = new Telerik.Web.UI.RadListBoxItem;
            item.set_text(detailData[0].TITLE);
            item.set_value(detailData[0].ID);
            var itemIndex = GetNewItemPositionInItems(chartListBox.get_items(), item.get_text());
            chartListBox.get_items().insert(itemIndex, item);
            chartListBox.findItemByValue(detailData[0].ID).select();
            item.scrollIntoView();
            $find('ctl00_ContentPlaceHolder1_NewChartWindow').close();
            $find("ctl00_ProgressWindow").close();
        }
    }

}

function OETreeNodeClicking(sender, args) {
    $find('ctl00_ContentPlaceHolder1_NewChartWindow_C_NewChartNameData').set_value(args.get_node().get_text());
}

function ContextMenuLinkShow(sender, args) {
    textLinkGrid.clearSelectedItems();
    args.get_gridDataItem().set_selected(true);
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_contextMenuLink').show(args.get_domEvent());
}

function ContextMenuPiktoShow(sender, args) {
    iconLinkGrid.clearSelectedItems();
    args.get_gridDataItem().set_selected(true);
    $find('ctl00_ContentPlaceHolder1_WindowChartDetail_C_contextMenuPikto').show(args.get_domEvent());
}

function HeaderContextMenuLinkClicked(sender, args) {
    ShowNodeLinkWindow(0);
}

function HeaderContextMenuPiktoClicked(sender, args) {
    ShowNodePiktoWindow(0);
}

function SaveNodeLinkClick() {
    parameter = {};
    parameter.id = $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_LinkId')[0].value;
    parameter.nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
    if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ChartData').get_selectedItem() !== null) {
        parameter.linkChartId = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ChartData').get_selectedItem().get_value();
    }
    else {
        parameter.linkChartId = 'null';
    }
    parameter.textIndex = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ButtonListText').get_selectedIndex();
    parameter.linkIndex = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ButtonListLink').get_selectedIndex();
    parameter.newWindow = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_OpenNewWindowData').get_checked();
    parameter.layoutId = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_LayoutData').get_selectedItem().get_value();
    detailData = getData(parameter, 'SaveNodeLink');
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_LinkId')[0].value = detailData[0].linkID;
    SetTexts(parameter.nodeId);
    SetChart(chartListBox.get_selectedItem().get_value());
}

function ShowNodeLinkWindow(linkId) {
    nodeLinkstWindowClear();
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_LinkId')[0].value = linkId;
    var nodeLinkstWindow = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow');
    nodeLinkstWindow.set_title(Translate('orgentityLink', 'chartnode')[0].Text + ' ' + $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_OrgentityData')[0].textContent);
    var chartDDL = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ChartData');
    chartListBox.get_items()._array.forEach(function (item) {
        var dropdownItem = new Telerik.Web.UI.DropDownListItem();
        dropdownItem.set_text(item.get_text());
        dropdownItem.set_value(item.get_value());
        chartDDL.get_items().add(dropdownItem);
    });
    if (linkId > 0) {
        SetLinkData(linkId);
    }
    nodeLinkstWindow.show();
}

function SetLinkData(linkId) {
    parameter = {};
    parameter.id = linkId;
    detailData = getData(parameter, 'GetNodeLinkData');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ButtonListText').get_items()[detailData[0].TEXT].set_selected(true);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ButtonListLink').get_items()[detailData[0].LINK].set_selected(true);
    if (detailData[0].LINK == "4") {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ChartData').findItemByValue([detailData[0].LINK_CHART_ID]).set_selected(true);
        $('#orgRow').css('visibility', 'visible');
        $('#chartRow').css('visibility', 'visible');
    }

    if (detailData[0].TARGETFRAME === "1") {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_OpenNewWindowData').set_checked(true);
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_LayoutData').findItemByValue(detailData[0].LAYOUT_ID).set_selected(true);
}

function SetPiktokData(piktoId) {
    parameter = {};
    parameter.id = piktoId;
    detailData = getData(parameter, 'GetNodePiktoData');
    var itemText = detailData[0].TEXT;
    if (itemText === '0' || itemText === '1') {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonLisPiktotText').get_items()[detailData[0].TEXT].set_selected(true);
    }
    else {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_FreeTextData').set_value(itemText);
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonListPikto').get_items()[detailData[0].LINK].set_selected(true);
    if (detailData[0].LINK == "4") {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ChartPiktoData').findItemByValue([detailData[0].LINK_CHART_ID]).set_selected(true);
        $('#orgRowPikto').css('visibility', 'visible');
        $('#chartRowPikto').css('visibility', 'visible');
    }

    if (detailData[0].TARGETFRAME === "1") {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_OpenNewWindowPiktoData').set_checked(true);
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_PiktoData1').findItemByValue(detailData[0].LAYOUT_ID).set_selected(true);
}

function ButtonLisPiktotTextClicking() {
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_FreeTextData').set_value('');
}

function FreeTextDataValueChanged() {
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonLisPiktotText').get_items()[0].set_selected(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonLisPiktotText').get_items()[1].set_selected(false);
}

function nodeLinkstWindowClear() {
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ChartData').get_items().clear();
    $('#orgRow').css('visibility', 'collapse');
    $('#chartRow').css('visibility', 'collapse');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ButtonListLink').get_items()[0].set_selected(true);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_ButtonListText').get_items()[0].set_selected(true);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_OpenNewWindowData').set_checked(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodeLinkstWindow_C_LayoutData').findItemByValue(0).set_selected(true);
}

function ShowNodePiktoWindow(piktoId){
    nodePiktoWindoeClear();
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_PiktoId')[0].value = piktoId;
    var nodePiktoWindow = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow');
    nodePiktoWindow.set_title(Translate('piktoLink', 'chartnode')[0].Text + ' ' + $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_OrgentityData')[0].textContent);
    var chartDDL = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ChartPiktoData');
    chartListBox.get_items()._array.forEach(function (item) {
        var dropdownItem = new Telerik.Web.UI.DropDownListItem();
        dropdownItem.set_text(item.get_text());
        dropdownItem.set_value(item.get_value());
        chartDDL.get_items().add(dropdownItem);
    });
    if (piktoId > 0) {
        SetPiktokData(piktoId);
    }
    nodePiktoWindow.show();
}

function nodePiktoWindoeClear() {
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_FreeTextData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ChartPiktoData').get_items().clear();
    $('#orgRowPikto').css('visibility', 'collapse');
    $('#chartRowPikto').css('visibility', 'collapse');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonListPikto').get_items()[0].set_selected(true);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonLisPiktotText').get_items()[0].set_selected(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_OpenNewWindowPiktoData').set_checked(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_PiktoData1').findItemByValue(0).set_selected(true);
}

function ButtonListLinkClicked(sender, args) {
    if (args.get_item().get_value() === 'chart') {
        $('#orgRow').css('visibility', 'visible');
        $('#chartRow').css('visibility', 'visible');
    }
    else {
        $('#orgRow').css('visibility', 'collapse');
        $('#chartRow').css('visibility', 'collapse');
    }
}

function SaveNodePicktoClick() {
    parameter = {};
    parameter.id = $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_PiktoId')[0].value;
    parameter.nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
    if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ChartPiktoData').get_selectedItem() !== null) {
        parameter.linkChartId = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ChartPiktoData').get_selectedItem().get_value();
    }
    else {
        parameter.linkChartId = 'null';
    }
    parameter.textIndex = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonLisPiktotText').get_selectedIndex();
    parameter.linkIndex = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_ButtonListPikto').get_selectedIndex();
    parameter.newWindow = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_OpenNewWindowPiktoData').get_checked();
    parameter.layoutId = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_PiktoData1').get_selectedItem().get_value();
    parameter.freeText = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_FreeTextData').get_value();
    detailData = getData(parameter, 'SavePiktoLink');
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NodePiktoWindow_C_PiktoId')[0].value = detailData[0].linkID;
    SetIconLinks(parameter.nodeId);
    SetChart(chartListBox.get_selectedItem().get_value());
}

function ButtonListPiktoClicked(sender, args) {
    if (args.get_item().get_value() === 'chart') {
        $('#orgRowPikto').css('visibility', 'visible');
        $('#chartRowPikto').css('visibility', 'visible');
    }
    else {
        $('#orgRowPikto').css('visibility', 'collapse');
        $('#chartRowPikto').css('visibility', 'collapse');
    }
}

function ContextMenuLinkClicked(sender, args) {
    var itemValue = args.get_item().get_value();

    switch (itemValue) {
        case 'AddLink':
            ShowNodeLinkWindow(0);
            break;
        case 'EditLink':
            ShowNodeLinkWindow(textLinkGrid.get_selectedItems()[0]._dataItem.ID);
            break;
        case 'DeleteLink':
            DeleteNodeLink(textLinkGrid.get_selectedItems()[0]._dataItem.ID);
            break;
        case 'MoveUpLink':
            MoveLinkUp(textLinkGrid.get_selectedItems()[0]._dataItem.ID, 0);
            break;
        case 'MoveDownLink':
            MoveLinkDown(textLinkGrid.get_selectedItems()[0]._dataItem.ID, 0);
            break;
        case 'CopyLink':
            CopyLinkToSubnotes(textLinkGrid.get_selectedItems()[0]._dataItem.ID);
            break;
    }

}

function ContextMenuPiktoClicked(sender, args) {
    var itemValue = args.get_item().get_value();

    switch (itemValue) {
        case 'AddPikto':
            ShowNodePiktoWindow(0);
            break;
        case 'EditPikto':
            ShowNodePiktoWindow(iconLinkGrid.get_selectedItems()[0]._dataItem.ID);
            break;
        case 'DeletePikto':
            DeleteNodePikto(iconLinkGrid.get_selectedItems()[0]._dataItem.ID);
            break;
        case 'MoveUpPikto':
            MoveLinkUp(iconLinkGrid.get_selectedItems()[0]._dataItem.ID, 1);
            break;
        case 'MoveDownPikto':
            MoveLinkDown(iconLinkGrid.get_selectedItems()[0]._dataItem.ID, 1);
            break;
        case 'CopyPikto':
            CopyLinkToSubnotes(iconLinkGrid.get_selectedItems()[0]._dataItem.ID);
            break;
    }
}

function DeleteNodeLink(linkId) {
    var confirmWindow = radconfirm(Translate('deleteLinkConfirm', 'chart')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            var nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
            var parameter = {};
            parameter.id = linkId;
            var detaiData = getData(parameter, "DeleteNodeLink");
            SetTexts(nodeId);
            SetChart(chartListBox.get_selectedItem().get_value());
        }
    });
}

function DeleteNodePikto(linkId) {
    var confirmWindow = radconfirm(Translate('deletePiktoConfirm', 'chart')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            var nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
            var parameter = {};
            parameter.id = linkId;
            var detaiData = getData(parameter, "DeleteNodeLink");
            SetIconLinks(nodeId);
            SetChart(chartListBox.get_selectedItem().get_value());
        }
    });
}

function MoveLinkUp(linkId, typ) {
    var parameter = {};
    parameter.id = linkId;
    parameter.nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
    parameter.typ = typ;
    detailData = getData(parameter, 'MoveLinkUp');
    if (typ == 0)
        SetTexts(parameter.nodeId);
    else
        SetIconLinks(parameter.nodeId);
}

function MoveLinkDown(linkId, typ) {
    var parameter = {};
    parameter.id = linkId;
    parameter.nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
    parameter.typ = typ;
    detailData = getData(parameter, 'MoveLinkDown');
    if (typ == 0)
        SetTexts(parameter.nodeId);
    else
        SetIconLinks(parameter.nodeId);
}

function CopyLinkToSubnotes(linkId, typ) {
    $find("ctl00_ProgressWindow").show();
    var parameter = {};
    parameter.id = linkId;
    parameter.nodeId = $('#ctl00_ContentPlaceHolder1_WindowChartDetail_C_NodeId')[0].value;
    detailData = getData(parameter, 'CopyLinkToSubnotes');
    SetChart(chartListBox.get_selectedItem().get_value());
    $find("ctl00_ProgressWindow").close();
}