var skillTree;
var itemListBox;
var skillDetailview;
var skillGroupDetailview;
var skillMultiPageview;

$(document).ready(function () {
    skillTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_SkillTree');
    itemListBox = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_SkillsListBox');
    skillDetailview = $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM')[0];
    skillGroupDetailview = $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM1')[0];
    skillMultiPageview = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM');
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRO').css('height', '300px');
    $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM1').css('height', '300px');
    PaneLResized();
    PaneRUResized();
    ClearDetailData();
});

function PaneLResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_SkillTree").css("height", paneLHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_SkillTree").css("width", paneLWidth - 10 + 'px');
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_SkillsListBox').css("height", paneRUHeight - 73 + 'px');

}

function SaveSkillOrGroupClicking(sender, args) {
    if (newItem || newGroup) {
        if (newItem) {
            AddTreeItem(skillTree.get_selectedNode().get_value());
        }
        else {
            AddTreeGroup(skillTree.get_selectedNode().get_value());
        }
    }
    else {
        var typ = skillTree.get_selectedNode().get_attributes().getAttribute('TYP');
        var id = skillTree.get_selectedNode().get_value();
        if (typ === 'GROUP') {
            UpdateSkillGroup(id);
        }
        else {
            UpdateSkill(id);
        }
    }
}

function SkillTreeNodePopulating(sender, args) {

}

function AddTreeItem(sourceId) {
    ValidatorEnable('SkillTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('SkillTable');
    saveNode = skillTree.findNodeByValue(sourceId);
    if (isValid) {
        parameter = {};
        parameter.groupId = sourceId;
        parameter.number = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_numberData').get_value();
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_descriptionData').get_value();
        parameter.from = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_fromData_dateInput').get_value();
        parameter.to = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_toData_dateInput').get_value();
        detailDat = getData(parameter, "AddSkill");
        parameter = {};
        parameter.sourceNodeId = sourceId;
        parameter.attributes = { 'TYP': 'SKILL' };
        newTreePart = getData(parameter, 'getSkillTreeData');
        if (skillTree.findNodeByValue(sourceId).get_nodes().get_count() > 0) {
            nodesInFolder = GetCopyFolderNodes(skillTree, sourceId);
            nodesInFolder.nodes.forEach(function (node) {
                if (node.get_attributes().getAttribute('TYP') === 'SKILL') {
                    skillTree.findNodeByValue(sourceId).get_nodes().remove(skillTree.findNodeByValue(node.get_value()));
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
            skillTree.findNodeByValue(sourceId).get_nodes().add(item);
        });
        skillTree.findNodeByValue(sourceId).expand();
        skillTree.findNodeByValue(detailDat[0].SKILL_ID).set_selected(true);
        skillTree.get_selectedNode().scrollIntoView();

        newItem = false;
    }
}

function AddTreeGroup(sourceId) {
    ValidatorEnable('SkillGroupTable');
    isValid = Page_ClientValidate();
    if (isValid) {
        ValidatorDisable('SkillGroupTable');
        parameter = {};
        parameter.groupId = sourceId;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').get_value();
        detailDat = getData(parameter, "AddSkillGroup");

        var item = new Telerik.Web.UI.RadTreeNode();
        item.set_text(detailDat[0].TITLE);
        item.set_value(detailDat[0].ID);
        item.set_imageUrl(detailDat[0].IMAGE);
        item.get_attributes().setAttribute("TYP", 'GROUP');
        item.get_attributes().setAttribute("ORDNUMBER", detailDat[0].ORDNUMBER);
        skillTree.findNodeByValue(sourceId).get_nodes().add(item);
        skillTree.findNodeByValue(sourceId).expand();
        skillTree.findNodeByValue(detailDat[0].ID).set_selected(true);
        skillTree.get_selectedNode().scrollIntoView();

        newGroup = false;
    }
}

function UpdateSkill(id) {
    ValidatorEnable('SkillTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('SkillTable');
    if (isValid) {
        parameter = {};
        parameter.id = id;
        parameter.number = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_numberData').get_value();
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_descriptionData').get_value();
        parameter.from = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_fromData_dateInput').get_value();
        parameter.to = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_toData_dateInput').get_value();
        getData(parameter, "UpdateSkill");
        skillTree.get_selectedNode().set_text(parameter.title);
    }
}

function UpdateSkillGroup(id) {
    ValidatorEnable('SkillGroupTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('SkillGroupTable');
    if (isValid) {
        parameter = {};
        parameter.id = id;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').get_value();
        getData(parameter, "UpdateSkillGroup");
        skillTree.get_selectedNode().set_text(parameter.title);
    }
}

function DeleteSkillClicking(sender, args) {
    var selectedNode = skillTree.get_selectedNode();
    if (selectedNode !== null) {
        var typ = skillTree.get_selectedNode().get_attributes().getAttribute('TYP');
        var id = skillTree.get_selectedNode().get_value();
        if (typ === 'GROUP') {
            DeleteSkillGroup(id);
        }
        else {
            DeleteSkill(id);
        }

    }
}

function DeleteSkill(id) {
    parameter = {};
    parameter.id = id;
    var selectedNode = skillTree.get_selectedNode();
    var parentNode = selectedNode.get_parent();
    confirmWindow = radconfirm(Translate('deleteSkillConfirm', 'skills')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            getData(parameter, "DeleteSkill");
            parentNode.get_nodes().remove(selectedNode);
            parentNode.set_selected(true);
            SkillTreeNodeClicked();
        }
    });
}

function DeleteSkillGroup(id) {
    parameter = {};
    parameter.id = id;
    var selectedNode = skillTree.get_selectedNode();
    var parentNode = selectedNode.get_parent();
    confirmWindow = radconfirm(Translate('deleteSkillGroupConfirm', 'skills')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            getData(parameter, "DeleteSkillGroup");
            parentNode.get_nodes().remove(selectedNode);
            parentNode.set_selected(true);
            SkillTreeNodeClicked();
        }
    });
}

function SkillTreeNodeDropping(sender, args) {
    if (!(args.get_destNode() == null)) {
        var sourceNode = args.get_sourceNode();
        var targetNode = args.get_destNode();
        var targetTyp = targetNode.get_attributes().getAttribute("TYP");
        var sourceTyp = sourceNode.get_attributes().getAttribute("TYP");
        parameter = {};

        if (targetTyp == 'GROUP' && sourceTyp == 'SKILL') {
            parameter.skillId = sourceNode.get_value();
            parameter.groupId = targetNode.get_value();
            result = getData(parameter, "MoveSkill");
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(sourceNode.get_text());
            node.set_value(sourceNode.get_value());
            node.set_imageUrl(sourceNode.get_imageUrl());
            node.get_attributes().setAttribute("TYP", "SKILL");
            node.get_attributes().setAttribute("ORDNUMBER", sourceNode.get_attributes().getAttribute('ORDNUMBER'));
            targetNode.get_nodes().add(node);
            sourceNode.get_parent().get_nodes().remove(sourceNode);
            node.set_selected(true);

            parameter1 = {};
            parameter1.id = node.get_value();
            detailData = getData(parameter1, 'GetSkillData');
            SetSkillData(detailData);
        }
        if (targetTyp == 'GROUP' && sourceTyp == 'GROUP') {
            parameter.sourceGroupId = sourceNode.get_value();
            parameter.targetGroupId = targetNode.get_value();
            detailData = getData(parameter, "MoveSkillGroup");

            var CopyOfTreeNodes = GetCopyTreeNodes(skillTree, sourceNode.get_value());

            var nodeRoot = new Telerik.Web.UI.RadTreeNode();
            nodeRoot.set_text(detailData[0].TITLE);
            nodeRoot.set_value(detailData[0].ID);
            nodeRoot.set_imageUrl(sourceNode.get_imageUrl());
            nodeRoot.get_attributes().setAttribute("TYP", "GROUP");
            nodeRoot.get_attributes().setAttribute("ORDNUMBER", detailData[0].ORDNUMBER);
            targetNode.get_nodes().add(nodeRoot);


            sourceNode.get_parent().get_nodes().remove(sourceNode);

            CopyOfTreeNodes.nodes.forEach(function (node) {
                if (node.get_attributes().getAttribute('PARENTID') === nodeRoot.get_value()) {
                    skillTree.findNodeByValue(nodeRoot.get_value()).get_nodes().add(node);
                }
                else {
                    skillTree.findNodeByValue(node.get_attributes().getAttribute('PARENTID')).get_nodes().add(node);
                }
            });
        }
        if (targetTyp == 'SKILL' && sourceTyp == 'SKILL') {
            parameter.sourceSkillId = sourceNode.get_value();
            parameter.destinationSkillId = targetNode.get_value();
            parameter.dropPosition = args._dropPosition;
            result = getData(parameter, "ReorderSkill");
        }

    }
}

var newItem;
var newGroup;

function SkillMenuClicked(sender, args) {
    var itemValue = args.get_menuItem().get_value();
    var node = args.get_node();
    node.select();
    switch (itemValue) {
        case 'NewItem':
            newItem = true;
            newGroup = false;
            AddSkill(node, 'SKILL');
            break;
        case 'NewGroup':
            newGroup = true;
            newItem = false;
            AddSkillGroup(node, 'SKILLGROUP');
            break;
        case 'Rename':
            typ = node.get_attributes().getAttribute('TYP');
            if (typ === 'SKILL') {
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_titleData').focus();
            }
            else {
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').focus();
            }
            break;

        case 'Delete':
            var selectedNode = skillTree.get_selectedNode();
            var id = selectedNode.get_value();
            if (selectedNode.get_attributes().getAttribute('TYP') === 'SKILL') {
                DeleteSkill(id);
            }
            else {
                DeleteSkillGroup(id);
            }
            break;
    }
}

function AddSkill(node, typ) {
    skillMultiPageview.set_selectedIndex(0);
    ClearDetailData();
    var today = new Date();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_fromData').set_selectedDate(today);
    var toDate = new Date(9999, 11, 30);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_toData').set_selectedDate(toDate);
    parameter = {};
    parameter.groupId = node.get_value();
    newItemNumber = getData(parameter, 'GetnewSkillNumber')[0].NUMBER;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_numberData').set_value(newItemNumber);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_titleData').focus();
}

function AddSkillGroup(node, typ) {
    skillMultiPageview.set_selectedIndex(1);
    ClearDetailData();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').focus();
}

function ClearDetailData() {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_numberData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_descriptionData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_fromData').clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_toData').clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').set_value('');
}

function SkillTreeNodeClicked(sender, args) {
    var typ = skillTree.get_selectedNode().get_attributes().getAttribute('TYP');
    parameter = {};
    parameter.id = skillTree.get_selectedNode().get_value();
    if (typ === 'GROUP') {
        detailData = getData(parameter, 'GetSkillGroupData');
        SetSkillGroupData(detailData);
    }
    else {
        detailData = getData(parameter, 'GetSkillData');
        SetSkillData(detailData);
    }
}

function SetSkillData(detailData) {
    skillMultiPageview.set_selectedIndex(0);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_numberData').set_value(detailData[0].NUMBER);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_descriptionData').set_value(detailData[0].DESCRIPTION);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_fromData_dateInput').set_selectedDate(new Date(GetDateFromJson(detailData[0].VALID_FROM)));
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_toData_dateInput').set_selectedDate(new Date(GetDateFromJson(detailData[0].VALID_TO)));
}

function SetSkillGroupData(detailData) {
    skillMultiPageview.set_selectedIndex(1);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').set_value(detailData[0].DESCRIPTION);
}

function SkillTreeContextMenuShowing(sender, args) {
    node = skillTree.findNodeByValue(args._node._properties._data.value);
    node.select();
    typ = node.get_attributes().getAttribute('TYP');
    parameter = {};
    parameter.id = node.get_value();
    if (typ === 'GROUP') {
        detailData = getData(parameter, 'GetSkillGroupData');
        SetSkillGroupData(detailData);
    }
    else {
        detailData = getData(parameter, 'GetSkillData');
        SetSkillData(detailData);
    }
    setMenu(node, typ);
}

function setMenu(node, typ) {

    switch (typ) {
        case 'SKILL':
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

function SkillSearchClick() {
    parameter = {};
    parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBName').get_value();
    parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBDescriptionSearch').get_value();
    itemList = getData(parameter, "GetSkillSearchList");
    itemListBox.get_items().clear();
    for (var i = 0; i < itemList.length; i++) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(itemList[i].TITLE);
        item.set_value(itemList[i].ID);
        item.set_imageUrl(itemList[i].IMAGE);
        item.get_attributes().setAttribute("TYP", itemList[i].TYP);
        item.get_attributes().setAttribute("GROUPID", itemList[i].SKILLGROUP_ID);
        itemListBox.get_items().add(item);
    }
}

function SkillListBoxIndexChanged(sender, args) {
    TreeCollapseAllNodes(skillTree);
    TreeExpandToRoot(skillTree, args.get_item());
    skillTree.findNodeByValue(args.get_item().get_value()).set_selected(true);
    skillTree.get_selectedNode().scrollIntoView();
    parameter = {};
    parameter.id = skillTree.get_selectedNode().get_value();
    detailData = getData(parameter, 'GetSkillData');
    SetSkillData(detailData);
}
