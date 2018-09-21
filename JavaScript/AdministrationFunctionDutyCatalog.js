var dutyTree;
var itemListBox;
var dutyDetailview;
var dutyGroupDetailview;
var dutyMultiPageview;

$(document).ready(function () {
    dutyTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_DutyTree');
    itemListBox = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DutyListBox');
    dutyDetailview = $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM')[0];
    dutyGroupDetailview = $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM1')[0];
    dutyMultiPageview = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM');
    PaneLResized();
    PaneRUResized();
    ClearDetailData();
});

function PaneLResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_DutyTree").css("height", paneLHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_DutyTree").css("width", paneLWidth - 10 + 'px');
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DutyListBox').css("height", paneRUHeight + 'px');

}

function SaveDutyOrGroupClicking(sender, args) {
    if (newItem || newGroup) {
        if (newItem) {
            AddTreeItem(dutyTree.get_selectedNode().get_value());
        }
        else {
            AddTreeGroup(dutyTree.get_selectedNode().get_value());
        }
    }
    else {
        var typ = dutyTree.get_selectedNode().get_attributes().getAttribute('TYP');
        var id = dutyTree.get_selectedNode().get_value();
        if (typ === 'GROUP') {
            UpdateDutyGroup(id);
        }
        else {
            UpdateDuty(id);
        }
    }
}

function DutyTreeNodePopulating(sender, args) {

}

function AddTreeItem(sourceId) {
    ValidatorEnable('DutyTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('DutyTable');
    saveNode = dutyTree.findNodeByValue(sourceId);
    if (isValid) {
        parameter = {};
        parameter.groupId = sourceId;
        parameter.number = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_numberData').get_value();
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_descriptionData').get_value();
        parameter.from = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_fromData_dateInput').get_value();
        parameter.to = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_toData_dateInput').get_value();
        detailDat = getData(parameter, "AddDuty");
        parameter = {};
        parameter.sourceNodeId = sourceId;
        parameter.attributes = { 'TYP': 'DUTY' };
        newTreePart = getData(parameter, 'getDutyTreeData');
        if (dutyTree.findNodeByValue(sourceId).get_nodes().get_count() > 0) {
            nodesInFolder = GetCopyFolderNodes(dutyTree, sourceId);
            nodesInFolder.nodes.forEach(function (node) {
                if (node.get_attributes().getAttribute('TYP') === 'DUTY') {
                    dutyTree.findNodeByValue(sourceId).get_nodes().remove(dutyTree.findNodeByValue(node.get_value()));
                }
            });
        }

        newTreePart.forEach(function (node) {
            var item = new Telerik.Web.UI.RadTreeNode();
            item.set_text(node.Text);
            item.set_value(node.Value);
            item.set_imageUrl(node.ImageUrl);
            item.get_attributes().setAttribute("TYP", node.Attributes["TYP"]);
            item.get_attributes().setAttribute("ORDNUMBER", node.Attributes["ORDNUMBER"]);
            dutyTree.findNodeByValue(sourceId).get_nodes().add(item);
        });
        dutyTree.findNodeByValue(sourceId).expand();
        dutyTree.findNodeByValue(detailDat[0].DUTY_ID).set_selected(true);
        dutyTree.get_selectedNode().scrollIntoView();

        newItem = false;
    }
}

function AddTreeGroup(sourceId) {
    ValidatorEnable('DutyGroupTable');
    isValid = Page_ClientValidate();
    if (isValid) {
        ValidatorDisable('DutyGroupTable');
        parameter = {};
        parameter.groupId = sourceId;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').get_value();
        detailDat = getData(parameter, "AddDutyGroup");

        var item = new Telerik.Web.UI.RadTreeNode();
        item.set_text(detailDat[0].TITLE);
        item.set_value(detailDat[0].ID);
        item.set_imageUrl(detailDat[0].IMAGE);
        item.get_attributes().setAttribute("TYP", 'GROUP');
        item.get_attributes().setAttribute("ORDNUMBER", detailDat[0].ORDNUMBER);
        dutyTree.findNodeByValue(sourceId).get_nodes().add(item);
        dutyTree.findNodeByValue(sourceId).expand();
        dutyTree.findNodeByValue(detailDat[0].ID).set_selected(true);
        dutyTree.get_selectedNode().scrollIntoView();

        newGroup = false;
    }
}

function UpdateDuty(id) {
    ValidatorEnable('DutyTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('DutyTable');
    if (isValid) {
        parameter = {};
        parameter.id = id;
        parameter.number = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_numberData').get_value();
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_descriptionData').get_value();
        parameter.from = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_fromData_dateInput').get_value();
        parameter.to = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_toData_dateInput').get_value();
        getData(parameter, "UpdateDuty");
        dutyTree.get_selectedNode().set_text(parameter.title);
    }
}

function UpdateDutyGroup(id) {
    ValidatorEnable('DutyGroupTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('DutyGroupTable');
    if (isValid) {
        parameter = {};
        parameter.id = id;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').get_value();
        getData(parameter, "UpdateDutyGroup");
        dutyTree.get_selectedNode().set_text(parameter.title);
    }
}

function DeleteDutyClicking(sender, args) {
    var selectedNode = dutyTree.get_selectedNode();
    if (selectedNode !== null) {
        var typ = dutyTree.get_selectedNode().get_attributes().getAttribute('TYP');
        var id = dutyTree.get_selectedNode().get_value();
        if (typ === 'GROUP') {
            DeleteDutyGroup(id);
        }
        else {
            DeleteDuty(id);
        }

    }
}

function DeleteDuty(id) {
    parameter = {};
    parameter.id = id;
    var selectedNode = dutyTree.get_selectedNode();
    var parentNode = selectedNode.get_parent();
    confirmWindow = radconfirm(Translate('deleteDutyConfirm', 'functionDescription')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            getData(parameter, "DeleteDuty");
            parentNode.get_nodes().remove(selectedNode);
            parentNode.set_selected(true);
            DutyTreeNodeClicked();
        }
    });
}

function DeleteDutyGroup(id) {
    parameter = {};
    parameter.id = id;
    var selectedNode = dutyTree.get_selectedNode();
    var parentNode = selectedNode.get_parent();
    confirmWindow = radconfirm(Translate('deleteDutyGroupConfirm', 'functionDescription')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            getData(parameter, "DeleteDutyGroup");
            parentNode.get_nodes().remove(selectedNode);
            parentNode.set_selected(true);
            DutyTreeNodeClicked();
        }
    });
}

function DutyTreeNodeDropping(sender, args) {
    if (!(args.get_destNode() == null)) {
        var sourceNode = args.get_sourceNode();
        var targetNode = args.get_destNode();
        var targetTyp = targetNode.get_attributes().getAttribute("TYP");
        var sourceTyp = sourceNode.get_attributes().getAttribute("TYP");
        parameter = {};

        if (targetTyp == 'GROUP' && sourceTyp == 'DUTY') {
            parameter.dutyId = sourceNode.get_value();
            parameter.groupId = targetNode.get_value();
            result = getData(parameter, "MoveDuty");
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(sourceNode.get_text());
            node.set_value(sourceNode.get_value());
            node.set_imageUrl(sourceNode.get_imageUrl());
            node.get_attributes().setAttribute("TYP", "DUTY");
            node.get_attributes().setAttribute("ORDNUMBER", sourceNode.get_attributes().getAttribute('ORDNUMBER'));
            targetNode.get_nodes().add(node);
            sourceNode.get_parent().get_nodes().remove(sourceNode);
            node.set_selected(true);

            parameter1 = {};
            parameter1.id = node.get_value();
            detailData = getData(parameter1, 'GetDutyData');
            SetDutyData(detailData);
        }
        if (targetTyp == 'GROUP' && sourceTyp == 'GROUP') {
            parameter.sourceGroupId = sourceNode.get_value();
            parameter.targetGroupId = targetNode.get_value();
            detailData = getData(parameter, "MoveDutyGroup");

            var CopyOfTreeNodes = GetCopyTreeNodes(dutyTree, sourceNode.get_value());

            var nodeRoot = new Telerik.Web.UI.RadTreeNode();
            nodeRoot.set_text(detailData[0].TITLE);
            nodeRoot.set_value(detailData[0].ID);
            nodeRoot.set_imageUrl(sourceNode.get_imageUrl());
            nodeRoot.get_attributes().setAttribute("TYP", "GROUP");
            nodeRoot.get_attributes().setAttribute("ORDNUMBER", detailData[0].ORDNUMBER);
            targetNode.get_nodes().add(nodeRoot);

            
            sourceNode.get_parent().get_nodes().remove(sourceNode);

            CopyOfTreeNodes.nodes.forEach(function (node) {
                if (node.get_attributes().getAttribute('PARENTID') === nodeRoot.get_value() ) {
                    dutyTree.findNodeByValue(nodeRoot.get_value()).get_nodes().add(node);
                }
                else {
                    dutyTree.findNodeByValue(node.get_attributes().getAttribute('PARENTID')).get_nodes().add(node);
                }
            });



        }
    }
}

var newItem;
var newGroup;

function DutyMenuClicked(sender, args) {
    var itemValue = args.get_menuItem().get_value();
    var node = args.get_node();
    node.select();
    switch (itemValue) {
        case 'NewItem':
            newItem = true;
            newGroup = false;
            AddDuty(node, 'DUTY');
            break;
        case 'NewGroup':
            newGroup = true;
            newItem = false;
            AddDutyGroup(node, 'DUTYGROUP');
            break;
        case 'Rename':
            typ = node.get_attributes().getAttribute('TYP');
            if (typ === 'DUTY') {
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').focus();
            }
            else {
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').focus();
            }
            break;

        case 'Delete':
            var selectedNode = dutyTree.get_selectedNode();
            var id = selectedNode.get_value();
            if (selectedNode.get_attributes().getAttribute('TYP') === 'DUTY') {
                DeleteDuty(id);
            }
            else {
                DeleteDutyGroup(id);
            }
            break;
    }
}

function AddDuty(node, typ) {
    dutyMultiPageview.set_selectedIndex(0);
    ClearDetailData();
    var today = new Date();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_fromData').set_selectedDate(today);
    var toDate = new Date(9999, 11, 30);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_toData').set_selectedDate(toDate);
    parameter = {};
    parameter.groupId = node.get_value();
    newItemNumber = getData(parameter, 'GetnewItemNumber')[0].NUMBER;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_numberData').set_value(newItemNumber);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').focus();
}

function AddDutyGroup(node, typ) {
    dutyMultiPageview.set_selectedIndex(1);
    ClearDetailData();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').focus();
}

function ClearDetailData() {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_numberData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_descriptionData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_fromData').clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_toData').clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').set_value('');
}

function DutyTreeNodeClicked(sender, args) {
    var typ = dutyTree.get_selectedNode().get_attributes().getAttribute('TYP');
    parameter = {};
    parameter.id = dutyTree.get_selectedNode().get_value();
    if (typ === 'GROUP') {
        detailData = getData(parameter, 'GetDutyGroupData');
        SetDutyGroupData(detailData);
    }
    else {
        detailData = getData(parameter, 'GetDutyData');
        SetDutyData(detailData);
    }
}

function SetDutyData(detailData) {
    dutyMultiPageview.set_selectedIndex(0);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_numberData').set_value(detailData[0].NUMBER);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_descriptionData').set_value(detailData[0].DESCRIPTION);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_fromData_dateInput').set_selectedDate(new Date(GetDateFromJson(detailData[0].VALID_FROM)));
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_toData_dateInput').set_selectedDate(new Date(GetDateFromJson(detailData[0].VALID_TO)));
}

function SetDutyGroupData(detailData) {
    dutyMultiPageview.set_selectedIndex(1);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').set_value(detailData[0].DESCRIPTION);
}

function DutyTreeContextMenuShowing(sender, args) {
    node = dutyTree.findNodeByValue(args._node._properties._data.value);
    node.select();
    typ = node.get_attributes().getAttribute('TYP');
    parameter = {};
    parameter.id = node.get_value();
    if (typ === 'GROUP') {
        detailData = getData(parameter, 'GetDutyGroupData');
        SetDutyGroupData(detailData);
    }
    else {
        detailData = getData(parameter, 'GetDutyData');
        SetDutyData(detailData);
    }
    setMenu(node, typ);
}

function setMenu(node, typ) {

    switch (typ) {
        case 'DUTY':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewGroup").hide();
            node.get_contextMenu().findItemByValue("NewItem").hide();
            node.get_contextMenu().findItemByValue("Delete").show();
            break;
        case 'GROUP':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewGroup").show();
            node.get_contextMenu().findItemByValue("NewItem").show();
            node.get_contextMenu().findItemByValue("Delete").show();
            break;
    }
}

function DutySearchClick() {
    parameter = {};
    parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_TBName').get_value();
    parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_TBDescriptionSearch').get_value();
    itemList = getData(parameter, "GetDutySearchList");
    itemListBox.get_items().clear();
    for (var i = 0; i < itemList.length; i++) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(itemList[i].TITLE);
        item.set_value(itemList[i].ID);
        item.set_imageUrl(itemList[i].IMAGE);
        item.get_attributes().setAttribute("TYP", itemList[i].TYP);
        item.get_attributes().setAttribute("GROUPID", itemList[i].DUTYGROUP_ID);
        itemListBox.get_items().add(item);
    }
}

function DutyListBoxIndexChanged(sender, args) {
    TreeCollapseAllNodes(dutyTree);
    TreeExpandToRoot(dutyTree, args.get_item());
    dutyTree.findNodeByValue(args.get_item().get_value()).set_selected(true);
    dutyTree.get_selectedNode().scrollIntoView();
    parameter = {};
    parameter.id = dutyTree.get_selectedNode().get_value();
    detailData = getData(parameter, 'GetDutyData');
    SetDutyData(detailData);
}
