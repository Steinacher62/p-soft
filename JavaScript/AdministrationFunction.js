
var functionTree;
var functionList;


$(document).ready(function () {
    functionTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_FunctionTree');
    functionListBox = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_FunctionsListBox');
    PaneLResized();
    PaneRUResized();
    ClearDetailData();
});

function PaneLResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_FunctionTree").css("height", paneLHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_FunctionTree").css("width", paneLWidth - 10 + 'px');
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_FunctionsListBox').css("height", paneRUHeight - 22 + 'px');

}

function FunctionSearchClick() {
    parameter = {};
    parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBName').get_value();
    parameter.nameShort = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBNameShort').get_value();
    functionList = getData(parameter, "GetFunctionSearchList");
    functionListBox.get_items().clear();
    for (var i = 0; i < functionList.length; i++) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(functionList[i].TITLE);
        item.set_value(functionList[i].ID);
        item.set_imageUrl(functionList[i].IMAGE);
        item.get_attributes().setAttribute("TYP", functionList[i].TYP);
        item.get_attributes().setAttribute("GROUPID", functionList[i].FUNKTION_GROUP_ID);
        functionListBox.get_items().add(item);
    }
}

function FunctionListBoxIndexChanged(sender, args) {
    var selectedId = args.get_item().get_value();
    var typ = args.get_item().get_attributes().getAttribute('TYP');
    TreeCollapseAllNodes(functionTree);
    var functionTreeNode = functionTree.findNodeByValue(selectedId);

    var parentFunctionTreeNode = functionTreeNode.get_parent();
    var level = functionTreeNode.get_level();
    do {
        parentFunctionTreeNode.expand();
        level = parentFunctionTreeNode.get_level();
        parentFunctionTreeNode = parentFunctionTreeNode.get_parent();
    }
    while (level > 0);

    functionTreeNode.select();
    functionTreeNode.scrollIntoView();
    SetDetailData(selectedId, typ);
}

function FunctionTreeNodeClicked(sender, args) {
    var selectedId = args.get_node().get_value();
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    SetDetailData(selectedId, typ);
}

function SetDetailData(selectedId, typ) {
    parameter = {};
    parameter.id = selectedId;
    parameter.typ = typ;
    detailData = getData(parameter, 'GetFunctionOrGroupDetailData');
    ClearDetailData()
    if (typ === 'GROUP') {
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(1);
        $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_LabelGroupParentData')[0].innerText = detailData[0].TITLEPARENT;
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBGroup').set_value(detailData[0].TITLE);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBDescription').set_value(detailData[0].DESCRIPTION);
    }
    if (typ === 'FUNCTION') {
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(0);
        $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_LabelGroupData')[0].innerText = detailData[0].GROUP_NAME;
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBName').set_value(detailData[0].TITLE);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBNameShort').set_value(detailData[0].TITLESHORT);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBDescription').set_value(detailData[0].DESCRIPTION);
        $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CBdefault").set_checked(detailData[0].DFLT);
        if (!(detailData[0].FUNKTION_TYP_ID === null)) {
            $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TypeData").findItemByValue(detailData[0].FUNKTION_TYP_ID).select();
        }
        if (detailData[0].FBW_REVISION != null) {
            $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_FBWRevisionData").set_selectedDate(new Date(GetDateFromJson(detailData[0].FBW_REVISION)));
        }
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBBonusPart').set_value(detailData[0].BONUSLEVEL);
    }

}

function ClearDetailData() {
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_LabelGroupParentData')[0].innerText = '';
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBGroup').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBDescription').set_value('');
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_LabelGroupData')[0].innerText = '';
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBName').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBNameShort').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBDescription').set_value('');
    $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CBdefault").set_checked(false);
    $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TypeData").findItemByValue(0).select();
    $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_FBWRevisionData").clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBBonusPart').set_value(0);
}

function FunctionTreeContextMenuShowing(sender, args) {
    node = functionTree.findNodeByValue(args._node._properties._data.value);
    node.select();
    SetDetailData(node.get_value(), node.get_attributes().getAttribute('TYP'));
    nodeTyp = functionTree.get_selectedNode().get_attributes().getAttribute('TYP');
    setMenu(node, nodeTyp);
}

function setMenu(node, typ) {

    switch (nodeTyp) {
        case 'FUNCTION':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewFunctionFolder").hide();
            node.get_contextMenu().findItemByValue("NewFunction").hide();
            node.get_contextMenu().findItemByValue("Delete").show();
            break;
        case 'GROUP':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewFunctionFolder").show();
            node.get_contextMenu().findItemByValue("NewFunction").show();
            node.get_contextMenu().findItemByValue("Delete").show();
            break;
    }
}


function FunctionMenuClicked(sender, args) {
    var itemValue = args.get_menuItem().get_value();
    var node = args.get_node();
    switch (itemValue) {
        case 'NewFunction':
            newFunction = true;
            AddFunction(node, 'FUNCTION');
            break;
        case 'NewFunctionFolder':
            newGroup = true;
            AddGroup(node, 'GROUP');
            break;
        case 'Rename':
            RenameGroupOrFunction();
            break;

        case 'Delete':
            selectedNode = functionTree.get_selectedNode();
            var id = selectedNode.get_value();
            if (selectedNode.get_attributes().getAttribute('TYP') === 'FUNCTION') {
                DeleteFunction(id);
            }
            else{
                DeleteGroup(id);
            }
            break;
    }
}

var newFunction;
var newFunctionGroup;
var saveId;
var saveTyp;
var saveNode;
function AddFunction(node, typ) {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(0);
    ClearDetailData();
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_LabelGroupData')[0].innerText = node.get_text();
    saveNode = node;
    saveTyp = typ;
    saveId = 0;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBName').focus(true);
}

function AddGroup(node, typ) {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(1);
    ClearDetailData();
    saveNode = node;
    saveTyp = typ;
    saveId = 0;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBGroup').focus(true);
    newFunctionGroup = true;
}

function SaveFunctionOrGroupClick(sender, args) {
    if (saveTyp === 'GROUP'){
        ValidatorEnable('detailDataGroup');
        var isValid = Page_ClientValidate();
        ValidatorDisable('detailDataGroup');
    }
    if (saveTyp === 'FUNCTION'){
        ValidatorEnable('detailDataFunction');
        var isValid = Page_ClientValidate();
        ValidatorDisable('detailDataFunction');
    }


    if (functionTree.get_selectedNode() == null || isValid === false) {
        args.set_cancel(true);
    }
    else {
        parameter = {};
        if (newFunction) {
            parameter.id = saveId;
            parameter.typ = saveTyp;
            parameter.functionGroupId = saveNode.get_value();
        }
        else {
            if (newFunctionGroup) {
                parameter.id = 0;
            }
            else {
               parameter.id = functionTree.get_selectedNode().get_value();
            }
            saveNode = functionTree.get_selectedNode();
            parameter.typ = saveNode.get_attributes().getAttribute('TYP');
            saveTyp = parameter.typ;
            if (parameter.typ === 'FUNCTION') {
                parameter.functionGroupId = saveNode.get_parent().get_value();
            }
            else {
                parameter.functionGroupId = saveNode.get_value();
            }

        }

        if (saveTyp === 'FUNCTION') {
            parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBName').get_value();
            parameter.nameShort = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBNameShort').get_value();
            parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBDescription').get_value();
            parameter.dflt = $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CBdefault").get_checked();
            parameter.functiontyp = $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TypeData").get_selectedItem().get_value()
            if (!($find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_FBWRevisionData").get_selectedDate() === null)) {
                parameter.fbwRevision = $find("ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_FBWRevisionData").get_selectedDate().format("dd.MM.yyyy");
            }
            else {
                parameter.fbwRevision = null;
            }
            if ($find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBBonusPart').get_value() === '') {
                parameter.bonusPart =  0;
            }
            else {
                parameter.bonusPart = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBBonusPart').get_value();
            }

            detailDat = getData(parameter, 'SaveFunction');
            //SetDetailData(detailDat[0].ID, saveTyp);
            if (newFunction) {
                var newNode = new Telerik.Web.UI.RadTreeNode();
                newNode.set_value(detailDat[0].ID);
                newNode.set_text(detailDat[0].TITLE);
                newNode.set_imageUrl(detailDat[0].IMAGE);
                newNode.get_attributes().setAttribute('TYP', 'FUNCTION');
                var nodeIndex = GetNewNodePositionInNodes(saveNode.get_nodes(), node.get_text());
                saveNode.get_nodes().insert(nodeIndex, newNode);
                newNode.select();
                newNode.get_parent().expand();
                newNode.scrollIntoView();
            }
            else {
                functionTree.get_selectedNode().set_text(detailDat[0].TITLE);
            }
            SetDetailData(detailDat[0].ID, 'FUNCTION')
            newFunction = false;
        }
        if (saveTyp === 'GROUP') {
            parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBGroup').get_value();
            parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBDescription').get_value();
            detailDat = getData(parameter, 'SaveFunctionGroup');
            if (newFunctionGroup) {
                var newNode = new Telerik.Web.UI.RadTreeNode();
                newNode.set_value(detailDat[0].ID);
                newNode.set_text(detailDat[0].TITLE);
                newNode.set_imageUrl(detailDat[0].IMAGE);
                newNode.get_attributes().setAttribute('TYP', 'GROUP');
                var nodeIndex = GetNewFolderPositionInNodes(saveNode.get_nodes(), node.get_text(), 'TYP', 'GROUP');
                functionTree.trackChanges();
                saveNode.get_nodes().insert(nodeIndex, newNode);
                functionTree.commitChanges();
                newNode.select();
                newNode.get_parent().expand();
                newNode.scrollIntoView();
            }
            else {
                functionTree.get_selectedNode().set_text(detailDat[0].TITLE);
            }
            SetDetailData(detailDat[0].ID, 'GROUP')
            newFunctionGroup = false;
        }
    }
}

function RenameGroupOrFunction() {
    ClearDetailData(); 
    var typ = functionTree.get_selectedNode().get_attributes().getAttribute('TYP');
    saveTyp = typ;
    saveNode = functionTree.get_selectedNode();
    saveId = saveNode.get_value();
    if (typ === 'FUNCTION') {
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(0);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBName').focus(true);
    }
    else {
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(1);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBGroup').focus(true);
    }

}

function DeleteFunction(id) {
    var selectedNode = functionTree.get_selectedNode();
    var confirmWindow = radconfirm(Translate('DeleteFunctionConfirm', 'function')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            parameter = {};
            parameter.id = id;
            getData(parameter, 'DeleteFunction');
            selectedNode.get_parent().get_nodes().remove(selectedNode);
            ClearDetailData();
        }
    });
}

function DeleteGroup(id) {
    var selectedNode = functionTree.get_selectedNode();
    var confirmWindow = radconfirm(Translate('DeleteFunctiongroupConfirm', 'function')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            parameter = {};
            parameter.id = id;
            getData(parameter, 'DeleteFunctionGroup');
            selectedNode.get_parent().get_nodes().remove(selectedNode);
            ClearDetailData();
        }
    });
}

function FunctionTreeNodeDropping(sender, args) {
    var sourceNode = args.get_sourceNode();
    var sourceTyp = sourceNode.get_attributes().getAttribute('TYP');
    var sourceNodeId = sourceNode.get_value();
    var destNode = args.get_destNode();
    var destTyp = destNode.get_attributes().getAttribute('TYP');
    var destId = destNode.get_value();

    param = {};
    param.text = 'moveTreeElementFailet';
    param.scope = 'error';
    var error = getData(param, 'Translate');

    if (destTyp !== 'GROUP') {
        radalert(error[0].Text);
        args.set_cancel(true);
    }
    else {
        parameter = {};
        parameter.sourceId = sourceNodeId;
        parameter.sourceTyp = sourceTyp;
        parameter.destId = destId;
        parameter.destTyp = destTyp;
        getData(parameter, "MoveNodeInTree");
        newPosition = GetNewNodePositionInNodes(destNode.get_nodes(), sourceNode.get_text());
        destNode.get_nodes().insert(newPosition, sourceNode);
        sourceNode.select();
        SetDetailData(sourceNodeId, sourceTyp);
    }
}