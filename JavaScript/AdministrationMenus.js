var menuTree;
var itemList;

$(document).ready(function () {
    menuTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_MenuTree');
    itemList = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_SearchListBox');
    PaneLResized();
    PaneRUResized();
    ClearDetailData();
});

function PaneLResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_MenuTree").css("height", paneLHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_MenuTree").css("width", paneLWidth - 10 + 'px');
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_SearchListBox').css("height", paneRUHeight - 22 + 'px');
}

function SaveMenuClicking() {
    var isValid;

    if ($('#ContentPlaceHolder1_LMRORU_Layout_PageViewM1')[0].clientHeight > 0)
    {
        ValidatorEnable('detailDataItem');
        isValid = Page_ClientValidate();
        ValidatorDisable('detailDataItem');
    }
    else{
        ValidatorEnable('detailDataGroup');
        isValid = Page_ClientValidate();
        ValidatorDisable('detailDataGroup');
    }
    if (isValid) {
        if (newMenuItem) {
            if (newMenuItemTyp === 'MENUGROUP') {
                SaveMenugroup(0, newMenuItemParentId, 0);
            }
            else {
                SaveMenuItem(0, newMenuItemParentId, 0);
            }
        }
        else {
            var typ = menuTree.get_selectedNode().get_attributes().getAttribute('TYP');
            var id = menuTree.get_selectedNode().get_value();
            var parentId = menuTree.get_selectedNode().get_parent().get_value();
            var ordNumber = menuTree.get_selectedNode().get_attributes().getAttribute('ORDNUMBER');
            if (typ === 'MENUGROUP') {
                SaveMenugroup(id, parentId, ordNumber);
            }
            else {
                SaveMenuItem(id, parentId, ordNumber);
            }
        }
    }
}

function SaveMenugroup(id, ParentId, ordNumber) {
    parameter = {};
    parameter.id = id;
    parameter.parentId = ParentId;
    parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBGroupName').get_value();
    parameter.nameShort = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBShortName').get_value();
    parameter.ordNumber = ordNumber;
    detailDat = getData(parameter, 'SaveMenugroup');
    if (newMenuItem) {
        var newNode = new Telerik.Web.UI.RadTreeNode();
        id = detailDat[0].ID;
        newNode.set_value(detailDat[0].ID);
        newNode.set_text(detailDat[0].TITLE);
        newNode.set_imageUrl(detailDat[0].IMAGE);
        newNode.get_attributes().setAttribute('TYP', 'MENUGROUP');
        newNode.get_attributes().setAttribute('ORDNUMBER', detailDat[0].ORDNUMBER);
        menuTree.findNodeByValue(ParentId).get_nodes().add(newNode);
        menuTree.findNodeByValue(ParentId).expand();
        menuTree.findNodeByValue(id).set_selected(true);
    }
    else {
        menuTree.get_selectedNode().set_text(detailDat[0].TITLE);
    }

    SetDetailDataGroup(id);
}

function SaveMenuItem(id, ParentId, ordNumber) {
    parameter = {};
    parameter.id = id;
    parameter.parentId = ParentId;
    parameter.ordNumber = ordNumber;
    parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBItemName').get_value();
    parameter.link = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBShortcut').get_value();
    parameter.target = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBFrame').get_value();
    detailDat = getData(parameter, 'SaveMenuItem');
    if (newMenuItem) {
        var newNode = new Telerik.Web.UI.RadTreeNode();
        id = detailDat[0].ID;
        newNode.set_value(detailDat[0].ID);
        newNode.set_text(detailDat[0].TITLE);
        newNode.set_imageUrl(detailDat[0].IMAGE);
        newNode.get_attributes().setAttribute('TYP', 'MENUITEM');
        newNode.get_attributes().setAttribute('ORDNUMBER', detailDat[0].ORDNUMBER);
        menuTree.findNodeByValue(ParentId).get_nodes().add(newNode);
        menuTree.findNodeByValue(ParentId).expand();
        menuTree.findNodeByValue(id).set_selected(true);
    }
    else {
        menuTree.get_selectedNode().set_text(detailDat[0].TITLE);
    }
    SetDetailDataItem(id);
}

function DeleteMenuClicking() {
    var typ = menuTree.get_selectedNode().get_attributes().getAttribute('TYP');
    var id = menuTree.get_selectedNode().get_value();
    if (menuTree.get_selectedNode().get_parent() !== menuTree) {
        if (typ === 'MENUGROUP') {
            DeleteMenuGroup(id);
        }
        else {
            DeleteMenuItem(id);
        }
    }
}

function DeleteMenuItem(id) {
    parameter = {};
    parameter.id = id;
    getData(parameter, "DeleteMenuItem");
    var selectedNode = menuTree.get_selectedNode();
    selectedNode.get_parent().get_nodes().remove(selectedNode);
    ClearDetailData();
}

function DeleteMenuGroup(id) {
    var confirmWindow = radconfirm(Translate('deleteMenuGroupConfirm', 'orgMenu')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            parameter = {};
            parameter.id = id;
            getData(parameter, "DeleteMenuGroup");
            var selectedNode = menuTree.get_selectedNode();
            selectedNode.get_parent().get_nodes().remove(selectedNode);
            ClearDetailData();
        }
    });

}

function AuthorisationMenuClicking() {
    if (menuTree.get_selectedNode() !== null) {
        id = menuTree.get_selectedNode().get_value();
        typ = menuTree.get_selectedNode().get_attributes().getAttribute('TYP');
        $('#ctl00_ContentPlaceHolder1_AuthorisationWindow_C_Id')[0].value = id;
        $('#ctl00_ContentPlaceHolder1_AuthorisationWindow_C_AuthorisationTyp')[0].value = typ;
        AuthorisationWindowShow(id, typ);
    }
}

var newMenuItem;
var newMenuItemParentId;
var newMenuItemTyp;

function MenuTreeContextMenuClicked(sender, args) {
    ClearDetailData();
    var itemValue = args.get_menuItem().get_value();
    var selectedNode = menuTree.get_selectedNode();
    var slectedId = selectedNode.get_value();
    if (args.get_menuItem().get_value() === 'NewMenu') {
        newMenuItemTyp = 'MENUITEM';
    }
    else {
        newMenuItemTyp = 'MENUGROUP';
    }
    var typ = selectedNode.get_attributes().getAttribute('TYP');
    switch (itemValue) {
        case 'NewMenu':
            $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(1);
            newMenuItem = true;
            newMenuItemParentId = slectedId;
            $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBItemName').focus(true);
            break;
        case 'NewMenugroup':
            $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(0);
            newMenuItem = true;
            newMenuItemParentId = slectedId;
            $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBGroupName').focus(true);
            break;
        case 'Rename':
            var typ = menuTree.get_selectedNode().get_attributes().getAttribute('TYP');
            var id = menuTree.get_selectedNode().get_value();
            if (typ === 'MENUGROUP') {
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(0);
                SetDetailDataGroup(id);
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBGroupName').focus(true);
            }
            else {
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(1);
                SetDetailDataItem(id);
                $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBItemName').focus(true);
            }
            break;

        case 'Delete':
            var typ = menuTree.get_selectedNode().get_attributes().getAttribute('TYP');
            var id = menuTree.get_selectedNode().get_value();
            if (typ === 'MENUGROUP') {
                DeleteMenuGroup(id);
            }
            else {
                DeleteMenuItem(id);
            }

            break;
    }
}

function MenuTreeNodeClicked(sender, args){
    var id = args.get_node().get_value();
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    if (typ === 'MENUGROUP') {
        SetDetailDataGroup(id);
    }
    else {
        SetDetailDataItem(id);
    }
}

function MenuTreeContextMenuShowing(sender, args) {
    var node = menuTree.findNodeByValue(args._node._properties._data.value);
    node.select();
    var nodeTyp = node.get_attributes().getAttribute('TYP');
    ClearDetailData();
    if (nodeTyp === 'MENUGROUP') {
        SetDetailDataGroup(node.get_value());
    }
    else {
        SetDetailDataItem(node.get_value());
    }
    setMenu(node, nodeTyp);
}

function setMenu(node, typ) {

    switch (typ) {
        case 'MENUITEM':
            node.get_contextMenu().findItemByValue("NewMenu").hide();
            node.get_contextMenu().findItemByValue("NewMenugroup").hide();
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("Delete").show();
            break;
        case 'MENUGROUP':
            node.get_contextMenu().findItemByValue("NewMenu").show();
            node.get_contextMenu().findItemByValue("NewMenugroup").show();
            if (node.get_parent() !== menuTree) {
                node.get_contextMenu().findItemByValue("Rename").show();
                node.get_contextMenu().findItemByValue("Delete").show();
            }
            else {
                node.get_contextMenu().findItemByValue("Rename").hide();
                node.get_contextMenu().findItemByValue("Delete").hide();
            }
           
            break;
    }
}

function ClearDetailData() {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBGroupName').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBShortName').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBItemName').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBShortcut').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBFrame').set_value('');
    newMenuItem = false;
    newMenuItemParentId = 0;
    newMenuItemTyp = '';

}

function SetDetailDataGroup(id) {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(0);
    ClearDetailData();
    if (menuTree.findNodeByValue(id).get_parent() !== menuTree) {
        parameter = {};
        parameter.id = id;
        var data = getData(parameter, 'GetMenuGroup');
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBGroupName').set_value(data[0].TITLE);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TBShortName').set_value(data[0].MNEMO);
    }
}

function SetDetailDataItem(id) {
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM').set_selectedIndex(1);
    ClearDetailData();
    parameter = {};
    parameter.id = id;
    var data = getData(parameter, 'GetMenuItem');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBItemName').set_value(data[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBShortcut').set_value(data[0].LINK);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBFrame').set_value(data[0].TARGET);
}

function SetSearchListClicked() {
    parameter = {};
    parameter.typ = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_DDTyp').get_selectedItem().get_value();
    parameter.filter = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBAlias').get_value();
    itemTable = getData(parameter, "GetSearchlistItems");
    itemList.get_items().clear();
    for (var i = 0; i < itemTable.length; i++) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(itemTable[i].TITLE);
        item.set_value(itemTable[i].UID);
        itemList.get_items().add(item);
    }
}

function SearchListBoxDropping(sender, args) {
    var sourceUid = args.get_sourceItem().get_value();
    var sourceText = args.get_sourceItem().get_text();
    var target = args.get_htmlElement();
    var targetNode = menuTree._extractNodeFromDomElement(target);
    var parameter = {};
    if (targetNode === null) {
        parameter.text = "nodeDropingError";
        parameter.scope = "error";
        errorMessage = getData(parameter, "Translate");
        radalert(errorMessage[0].Text);
        args.set_cancel(true);
    }
    else {
        var targetNode = menuTree._extractNodeFromDomElement(target);
        var targetId = targetNode.get_value();
        var targetTyp = targetNode.get_attributes().getAttribute('TYP');
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBItemName').set_value(sourceText);
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_TBShortcut').set_value('/goto.aspx?UID=' + sourceUid);
        if (targetTyp === 'MENUITEM') { 
            SaveMenuItem(targetNode.get_value(), targetNode.get_parent().get_value(), targetNode.get_attributes().getAttribute('ORDNUMBER'));
        }
        else {
            newMenuItem = true;
            SaveMenuItem(0, targetNode.get_value(), 0);
        }
    }
}

function MenuTreeDragStart(sender, args) {
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    var ordnumber = args.get_node().get_attributes().getAttribute('ORDNUMBER');
    if (typ === 'MENUGROUP' && ordnumber === '1') {
        args.set_cancel(true);
    }

}

function MenuTreeNodeDropping(sender, args) {
    parameter = {};
    var dropPosition = args.get_dropPosition();
    var typSource = args.get_sourceNode().get_attributes().getAttribute('TYP');
    var typDest = args.get_destNode().get_attributes().getAttribute('TYP');
    var ordNumberSource = args.get_sourceNode().get_attributes().getAttribute('ORDNUMBER');
    var ordNumberDest = args.get_destNode().get_attributes().getAttribute('ORDNUMBER');
    var destId = args.get_destNode().get_value();
    var sourceId = args.get_sourceNode().get_value();

    if (typDest === 'MENUGROUP' && dropPosition === 'over') {
        parameter.sourceId = sourceId;
        parameter.destId = destId;
        parameter.typSource = typSource;
        detailDat = getData(parameter, 'MoveMenuEntryToGroup');
        args.get_sourceNode().get_attributes().getAttribute('ORDNUMBER');
        args.get_destNode().get_nodes().add(args.get_sourceNode());
    }
    if (typDest === 'MENUITEM' && (dropPosition === 'above' || dropPosition === 'below')) {
        var destIndex = args.get_destNode().get_index();
        parameter.sourceId = sourceId;
        parameter.destId = destId;
        parameter.dropPosition = dropPosition;
        detailDat = getData(parameter, 'MoveMenuItem');
        args.get_destNode().get_parent().get_nodes().insert(destIndex, args.get_sourceNode());
        args.get_sourceNode().set_selected(true);
        SetDetailDataItem(args.get_sourceNode().get_value());
    }

    
}
