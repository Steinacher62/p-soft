
var windowClientId;
var accessorListBoxId;
var accessorWindowId;

$(document).ready(function () {
    pageName = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
    switch (pageName) {
        case 'ApplicationPermissions.aspx':
            windowClientId = 'ctl00_ContentPlaceHolder1_ApplicationAuthorisationWindow';
            accessorListBoxId = 'ctl00_ContentPlaceHolder1_AccessorWindow_C_ctl00_AccessorListBox';
            accessorWindowId = 'ctl00_ContentPlaceHolder1_AccessorWindow';
            break;
        case 'OrganisationMenu.aspx':
            windowClientId = 'ctl00_ContentPlaceHolder1_AuthorisationWindow';
            accessorListBoxId = 'ctl00_ContentPlaceHolder1_AccessorWindow_C_AccessorCtrl_AccessorListBox';
            accessorWindowId = 'ctl00_ContentPlaceHolder1_AccessorWindow';
            break;
        case 'KnowledgeMenu.aspx':
            windowClientId = 'ctl00_ContentPlaceHolder1_AuthorisationWindow';
            accessorListBoxId = 'ctl00_ContentPlaceHolder1_AccessorWindow_C_AccessorCtrl_AccessorListBox';
            accessorWindowId = 'ctl00_ContentPlaceHolder1_AccessorWindow';
            break;
        default:
            windowClientId = 'ctl00_ContentPlaceHolder1_AdminContentLayout_AuthorisationWindow';
            accessorListBoxId = 'ctl00_ContentPlaceHolder1_AdminContentLayout_AccessorWindow_C_ctl00_AccessorListBox';
            accessorWindowId = 'ctl00_ContentPlaceHolder1_AdminContentLayout_AccessorWindow';
    }
    if (!($("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_PaneMU").css("height") === undefined)) {
        PaneMUResized();
        PaneLUResized();
    }
});

function PaneMUResized() {
    var paneMUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_PaneMU").css("height").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox").css("height", paneMUHeight - 20 + 'px');
}

function PaneLUResized() {
    var paneLUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_PaneLU").css("height").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_MemberOfListBox").css("height", paneLUHeight - 65 + 'px');
    $("#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_GroupMembersListBox").css("height", paneLUHeight - 65 + 'px');
}

function getSourceTree(typ) {
    var sourceTree;
    if (typ === 'ORGENTITY' || typ === 'FIRM' || typ === 'JOB') {
        sourceTree = tree;
    }
    if (typ === 'FOLDER') {
        sourceTree = clipboardTree;
    }

    if (typ === 'MENUITEM' || typ === 'MENUGROUP') {
        sourceTree = menuTree;
    }

    return sourceTree;
}

function AuthorisationWindowShow(id, typ) {
    parameter = {};
    parameter.id = id;
    parameter.typ = typ;
    var accessors = getData(parameter, "GetAccessors");
    var listBox = $find(windowClientId + "_C_ctl00_AuthorisationListBox");
    listBox.get_items().clear();
    accessors.forEach(function (accessor) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(accessor.TITLE);
        item.set_value(accessor.ID);
        item.set_imageUrl(accessor.IMAGE);
        listBox.trackChanges();
        listBox.get_items().add(item);
        listBox.commitChanges();
    });


    param = {};
    param.typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;

    $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_items().clear();
    var modules = getData(param, 'GetModule');
    for (var i = 0; i < modules.length; i++) {
        var item = new Telerik.Web.UI.DropDownListItem();
        item.set_text(modules[i].modulname);
        item.set_value(modules[i].modulvalue);
        $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_items().add(item);
    }


    listBox.getItem(0).set_selected(true);
    $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").getItem(0).select();

    if (!(param.typ === 'RELEASE')) {
        $find(windowClientId + "_C_ctl01_TakeInherited").set_enabled(true);
        parameter.id = getSourceTree(typ).get_selectedNode().get_value();
        var inherited = GetPermissionInherited(parameter.id, parameter.typ);
        if (inherited === 1) {
            $find(windowClientId + "_C_ctl01_TakeInherited").set_checked(true);
        }
        else {
            $find(windowClientId + "_C_ctl01_TakeInherited").set_checked(false);
        }
    }
    else {
        $find(windowClientId + "_C_ctl01_TakeInherited").set_enabled(false);
    }
    if (windowClientId === 'ctl00_ContentPlaceHolder1_AdminContentLayout_AuthorisationWindow' || windowClientId === 'ctl00_ContentPlaceHolder1_AuthorisationWindow') {
        $find(windowClientId).show();
    }
}

function AuthorisationListBoxIndexChanged(sender, args) {
    if (!($find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_selectedItem() === null)) {
        var accessorId = args.get_item().get_value();
        var modulId = $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_selectedItem().get_value();
        var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
        var id;
        if (typ !== 'RELEASE') {
            id = getSourceTree(typ).get_selectedNode().get_value();
        }
        else {
            id = 0;
        }
        GetPermissions(accessorId, modulId, typ, id);
    }
}

function ApplicationPermissionsSelected(sender, args) {
    var accessorId = $find(windowClientId + "_C_ctl00_AuthorisationListBox").get_selectedItem().get_value();
    var modulId = args.get_item().get_value();
    var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
    var id;
    if(typ === 'RELEASE') {
        id = 0;
    }
    else {
        id = getSourceTree(typ).get_selectedNode().get_value();
    }
    GetPermissions(accessorId, modulId, typ, id);
}

function GetPermissions(accessorId, modulId, typ, id) {
    parameter = {};
    parameter.accessorId = accessorId;
    parameter.modulId = modulId;
    parameter.typ = typ;
    if (typ === 'RELEASE') {
        parameter.id = 0;
        parameter.inherited = false;
    }
    if (typ === 'ORGENTITY' || typ === 'FIRM' || typ === 'JOB') {
        parameter.id = id;
        parameter.inherited = $find(windowClientId + "_C_ctl01_TakeInherited").get_checked();
    }

    if (typ === 'FOLDER' || typ === 'MENUGROUP' || typ === 'MENUITEM') {
        parameter.modulId = 0;
        parameter.inherited = $find(windowClientId + "_C_ctl01_TakeInherited").get_checked();
        parameter.id = id;
    }

    var permissions = getData(parameter, "GetPermissions");
    SetPermissionsForm(permissions, true);

    parameter.inherited = false;
    permissions = getData(parameter, "GetPermissions");
    SetPermissionsForm(permissions, false);
}

function SetPermissionsForm(permissions, inherited) {
    if (inherited) {
        $find(windowClientId + "_C_ctl01_ReadDataCBInherited").set_checked(permissions.read);
        $find(windowClientId + "_C_ctl01_InsertDataCBInherited").set_checked(permissions.insert);
        $find(windowClientId + "_C_ctl01_EditDataCBInherited").set_checked(permissions.update);
        $find(windowClientId + "_C_ctl01_DeleteDataCBInherited").set_checked(permissions.delete);
        $find(windowClientId + "_C_ctl01_AdminDataCBInherited").set_checked(permissions.admin);
        $find(windowClientId + "_C_ctl01_ExecuteDataCBInherited").set_checked(permissions.execute);
    }
    else {
        $find(windowClientId + "_C_ctl01_ReadDataCB").set_checked(permissions.read);
        $find(windowClientId + "_C_ctl01_InsertDataCB").set_checked(permissions.insert);
        $find(windowClientId + "_C_ctl01_EditDataCB").set_checked(permissions.update);
        $find(windowClientId + "_C_ctl01_DeleteDataCB").set_checked(permissions.delete);
        $find(windowClientId + "_C_ctl01_AdminDataCB").set_checked(permissions.admin);
        $find(windowClientId + "_C_ctl01_ExecuteDataCB").set_checked(permissions.execute);
    }
}

function GetPermissionForm(inherited) {

    permission = 0;
    if (inherited) {
        if ($find(windowClientId + "_C_ctl01_ReadDataCBInherited").get_checked()) {
            permission += 2;
        }
        if ($find(windowClientId + "_C_ctl01_InsertDataCBInherited").get_checked()) {
            permission += 4;
        }
        if ($find(windowClientId + "_C_ctl01_EditDataCBInherited").get_checked()) {
            permission += 8;
        }
        if ($find(windowClientId + "_C_ctl01_DeleteDataCBInherited").get_checked()) {
            permission += 16;
        }
        if ($find("_ctl01_AdminDataCBInherited").get_checked()) {
            permission += 32;
        }
        if ($find(windowClientId + "_C_ctl01_ExecuteDataCBInherited").get_checked()) {
            permission += 64;
        }
    }
    else {
        if ($find(windowClientId + "_C_ctl01_ReadDataCB").get_checked()) {
            permission += 2;
        }
        if ($find(windowClientId + "_C_ctl01_InsertDataCB").get_checked()) {
            permission += 4;
        }
        if ($find(windowClientId + "_C_ctl01_EditDataCB").get_checked()) {
            permission += 8;
        }
        if ($find(windowClientId + "_C_ctl01_DeleteDataCB").get_checked()) {
            permission += 16;
        }
        if ($find(windowClientId + "_C_ctl01_AdminDataCB").get_checked()) {
            permission += 32;
        }
        if ($find(windowClientId + "_C_ctl01_ExecuteDataCB").get_checked()) {
            permission += 64;
        }
    }
    return permission;
}

function GetPermissionInherited(id, typ) {
    parameter = {};
    parameter.id = id;
    parameter.typ = typ;
    var inhertied = getData(parameter, "GetPermissionInherited");
    return inhertied[0].INHERIT;
}

function SaveAuthorisation(id, typ) {
    $find("ctl00_ProgressWindow").show();
    if (typ === 'ORGENTITY' || typ === 'FIRM' || typ === 'JOB' || typ === 'FOLDER' || typ === 'MENUGROUP' || typ === 'MENUITEM') {
        param = {};
        if ($find(windowClientId + "_C_ctl01_TakeInherited").get_checked()) {
            param.inherited = 1;
        }
        else {
            param.inherited = 0;
        }
        param.id = getSourceTree(typ).get_selectedNode().get_value();
        param.typ = typ;
        getData(param, "SavePermissionInherited");
    }

    parameter = {};
    parameter.accessorId = $find(windowClientId + "_C_ctl00_AuthorisationListBox").get_selectedItem().get_value();
    parameter.permissions = GetPermissionForm(false);
    if (typ === 'RELEASE') {
        parameter.id = 0;
        parameter.typ = typ;
        parameter.inherited = false;
        parameter.modulId = $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_selectedItem().get_value();
    }
    if (typ === 'ORGENTITY' || typ === 'FIRM' || typ === 'JOB') {
        parameter.id = getSourceTree(typ).get_selectedNode().get_value();
        parameter.typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
        parameter.inherited = true;
        parameter.modulId = $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_selectedItem().get_value();
    }
    if (typ === 'FOLDER' || typ === 'MENUGROUP' || typ === 'MENUITEM') {
        parameter.id = getSourceTree(typ).get_selectedNode().get_value();
        parameter.typ = typ;
        parameter.inherited = true;
        parameter.modulId = 0;
    }



    getData(parameter, 'SavePermission');
    $find("ctl00_ProgressWindow").close();
}

function DeleteAuthorisation(id, typ) {
    $find("ctl00_ProgressWindow").show();
    parameter = {};
    parameter.typ = typ;
    parameter.accessorId = id;
    parameter.id = 0;
    if (typ !== 'RELEASE') {
        parameter.id = getSourceTree(typ).get_selectedNode().get_value();
    }
    var modules = $find(windowClientId + "_C_ctl01_ApplicationPermissionsData").get_items();
    var module = [];
    var i = 0;
    modules.forEach(function (modul) {
        module[i] = modul.toJsonString();
        i++;
    });
    parameter.modules = module;
    getData(parameter, "DeleteAccessor");
    $find("ctl00_ProgressWindow").close();
    AuthorisationWindowShow(parameter.id, parameter.typ);
}

function AddAccessorClick(sender, args) {
    $find("ctl00_ProgressWindow").show();
    var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
    selectedItem = $find(accessorListBoxId).get_selectedItem();
    if (selectedItem !== null) {
        parameter = {};
        if ($find(windowClientId + "_C_ctl01_TakeInherited").get_checked()) {
            parameter.inherited = true;
        }
        else {
            parameter.inherited = false;
        }

        parameter.accessorId = $find(accessorListBoxId).get_selectedItem().get_value();

        if (typ === 'ORGENTITY' || typ === 'FIRM' || typ === 'JOB') {
            parameter.id = getSourceTree(typ).get_selectedNode().get_value();
            parameter.modulId = 0;
            parameter.permissions = 2;
            parameter.typ = typ;
            getData(parameter, 'SavePermission');
        }
        if (typ === 'RELEASE') {
            parameter.applicationright = 1;
            parameter.permission = 2;
            parameter.tablename = "";
            getData(parameter, 'AddAccessorApplication');
        }

        if (typ === 'FOLDER') {
            parameter.id = getSourceTree(typ).get_selectedNode().get_value();
            parameter.modulId = 0;
            parameter.permissions = 2;
            parameter.typ = typ;
            getData(parameter, 'SavePermission');
        }
        if (typ === 'MENUITEM' || typ === 'MENUGROUP') {
            parameter.id = getSourceTree(typ).get_selectedNode().get_value();
            parameter.modulId = 0;
            parameter.permissions = 2;
            parameter.typ = typ;
            getData(parameter, 'SavePermission');
        }


        $find("ctl00_ProgressWindow").close();
    }
}

function AccessorWindowShow() {
    $find("ctl00_ProgressWindow").show();
    var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
    parameter = {};
    parameter.typ = typ;
    if (typ === 'ORGENTITY' || typ === 'FIRM' || typ === 'JOB') {
        parameter.id = getSourceTree(typ).get_selectedNode().get_value();
    }
    if (typ === 'RELEASE' || typ === 'FOLDER' || typ === 'MENUITEM' || typ === 'MENUGROUP') {
        parameter.id = 0;
    }

    var listBox = $find(accessorListBoxId);
    listBox.get_items().clear();

    var accessors = getData(parameter, "GetAccessorList");
    //listBox.trackChanges();
    for (var i = 0; i < accessors.length; i++) {

        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(accessors[i].TITLE);
        item.set_value(accessors[i].ID);
        item.set_imageUrl(accessors[i].IMAGE);
        item.get_attributes().setAttribute("TYP", accessors[i].TABLENAME);

        listBox.get_items().add(item);

    }
    //listBox.commitChanges();

    $find(accessorWindowId).show();
    $find(accessorWindowId).close();
    $find(accessorWindowId).show();
    $find("ctl00_ProgressWindow").close();
}

function AccessorWindowClose(sender, args) {
    $find(windowClientId).show();
    var id = $("#" + windowClientId + "_C_Id")[0].value;
    var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
    AuthorisationWindowShow(id, typ);
}

function AccessorWindowResized() {
    accesoorWindowHeight = $("#" + accessorWindowId + '_C').css("height").replace("px", "");
    $("#" + accessorListBoxId).css("height", accesoorWindowHeight - 28 + 'px');
}

function SetAccessorList() {
    $find("ctl00_ProgressWindow").show();
    ClearMemberListBoxes();
    ClearAccessorDetail();
    parameter = {};
    parameter.typ = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_DDTyp').get_selectedItem().get_value();
    parameter.accessor = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_TBAccessore').get_textBoxValue();
    parameter.editable = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_DDIsEditable').get_selectedItem().get_value();
    var accessors = getData(parameter, 'GetAccessorListEditable');

    var accessorListBox = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox');
    accessorListBox.get_items().clear();
    for (var i = 0; i < accessors.length; i++) {

        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(accessors[i].TITLE);
        item.set_value(accessors[i].ID);
        item.set_imageUrl(accessors[i].IMAGE);
        item.get_attributes().setAttribute("TYP", accessors[i].TABLENAME);
        if (accessors[i].VISIBLE === 1) {
            item.set_checked(true);
        }


        accessorListBox.get_items().add(item);

    }
    $find("ctl00_ProgressWindow").close();

    $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_TabStrip").get_tabs().getTab(0).set_selected(true);
}

function AccessorListBoxIndexChanged(sender, args) {
    var accessorId = args.get_item().get_value();
    var typ = args.get_item().get_attributes().getAttribute('TYP');
    SetMeberOfList(accessorId);
    if (typ === 'ORGENTITY' || typ === 'ACCESSORGROUP') {
        SetGroupmemberList(accessorId);
        $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_TabStrip").get_tabs().getTab(0).set_selected(true);
    }
    else {
        $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_TabStrip").get_tabs().getTab(0).set_enabled(false);
        $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_TabStrip").get_tabs().getTab(1).set_selected(true);
    }

    SetAccessorDetail(accessorId, typ);
}

function AccessorListBoxShowContextMenu(sender, args) {
    var selectedItem = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_selectedItem();
    var accessorListBox = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_GroupMembersListBox');
    if (selectedItem === null || args.get_item().get_value() === selectedItem.get_value() || selectedItem.get_checked() === false || selectedItem === null || accessorListBox.findItemByValue(args.get_item().get_value()) !== null) {
        args.set_cancel = true;
    }
    else {
        var menu = $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListContextMenu");
        menu.get_attributes().setAttribute("selectItemId", args.get_item().get_value());
        var rawEvent = args.get_domEvent().rawEvent; menu.show(rawEvent);
        $telerik.cancelRawEvent(rawEvent);
    }
}

function AccessorListBoxContextMenuItemClicked(sender, args) {
    var accessorId = sender.get_attributes().getAttribute("selectItemId");
    var accessorGroupId = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_selectedItem().get_value();
    parameter = {};
    parameter.accesorId = accessorId;
    parameter.accessorGroupId = accessorGroupId;
    getData(parameter, 'AddAccessorToGroup');
    SetGroupmemberList(accessorGroupId);
}

function AccessorGroupDetailShowContextMenu(sender, args) {
    if ($find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_selectedItem().get_attributes().getAttribute("TYP") === 'ACCESSORGROUP') {
        var menu = $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_GroupMembersListContextMenu");
        args.get_item().select();
        var rawEvent = args.get_domEvent().rawEvent; menu.show(rawEvent);
        $telerik.cancelRawEvent(rawEvent);
    }
    else {
        args.set_cancel = true;
    }
}

function GroupMembersListBoxContextMenuItemClicked(sender, args) {
    var accessorId = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_GroupMembersListBox').get_selectedItem().get_value();
    var accessorGroupId = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_selectedItem().get_value();
    parameter = {};
    parameter.accesorId = accessorId;
    parameter.accessorGroupId = accessorGroupId;
    getData(parameter, 'DeleteAccessorFromGroup');
    SetGroupmemberList(accessorGroupId);
}

function SetGroupmemberList(accessorId) {
    parameter = {};
    parameter.accessorId = accessorId;
    var accessors = getData(parameter, 'GetAccessorGroupMember');
    var accessorListBox = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_GroupMembersListBox');
    accessorListBox.get_items().clear();
    $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_TabStrip").get_tabs().getTab(0).set_enabled(true);
    for (var i = 0; i < accessors.length; i++) {

        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(accessors[i].TITLE);
        item.set_value(accessors[i].ID);
        item.set_imageUrl(accessors[i].IMAGE);
        item.get_attributes().setAttribute("TYP", accessors[i].TABLENAME);
        if (accessors[i].VISIBLE === 1) {
            item.set_checked(true);
        }


        accessorListBox.get_items().add(item);

    }

}

function SetMeberOfList(accessorId) {
    parameter = {};
    parameter.accessorId = accessorId;
    var accessors = getData(parameter, 'GetAccessorMemberOf');
    var accessorListBox = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_MemberOfListBox');
    accessorListBox.get_items().clear();
    for (var i = 0; i < accessors.length; i++) {

        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(accessors[i].TITLE);
        item.set_value(accessors[i].ID);
        item.set_imageUrl(accessors[i].IMAGE);
        item.get_attributes().setAttribute("TYP", accessors[i].TABLENAME);
        if (accessors[i].VISIBLE === 1) {
            item.set_checked(true);
        }


        accessorListBox.get_items().add(item);

    }

}

function ClearMemberListBoxes() {
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_GroupMembersListBox').get_items().clear();
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_MemberOfListBox').get_items().clear();
}

function AccessorListBoxChecking(sender, args) {
    args.set_cancel(true);
}

function AddAccessorGroupClicking() {
    $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_NewAccessorgroupWindow").show();
}

function SaveAccesorGroupClicking() {
    var groupName = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup').get_value();
    var selectedGroup = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_selectedItem();
    if (groupName.length > 0) {
        parameter = {};
        parameter.id = selectedGroup.get_value();
        parameter.newName = groupName;

        getData(parameter, 'RenameAccessorGroup');
        selectedGroup.set_text(groupName);
    }

}

function DeleteAccessorGroupClicking() {
    var selectedGroup = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_selectedItem();
    if (selectedGroup !== null) {
        confirmWindow = radconfirm(Translate('deleteGroupConfirm', 'authorisations')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                confirmWindow.Close();
                parameter = {};
                parameter.id = selectedGroup.get_value();
                getData(parameter, 'DeleteAccessorGroup');
                $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox').get_items().remove(selectedGroup);
                ClearAccessorDetail();
                ClearMemberListBoxes();
            }
        });
    }
}

function AddAccessorGroup() {
    var groupTitle = $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_NewAccessorgroupWindow_C_TBNewGroupTitle").get_value();
    if (groupTitle.length > 0) {
        parameter = {};
        parameter.groupTitle = groupTitle;
        var accessorGroup = getData(parameter, 'AddAccessorGroup');
        var accessorList = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_AccessorListBox');
        var position = GetNewNodePositionInNodes(accessorList.get_items(), accessorGroup[0].TITLE);

        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(accessorGroup[0].TITLE);
        item.set_value(accessorGroup[0].ID);
        item.set_imageUrl(accessorGroup[0].IMAGE);
        item.set_checked(true);
        item.get_attributes().setAttribute("TYP", 'ACCESSORGROUP');

        accessorList.trackChanges();
        accessorList.get_items().insert(position, item);
        accessorList.commitChanges();
        accessorList.findItemByValue(accessorGroup[0].ID).set_selected(true);
        item.scrollIntoView();
        $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_NewAccessorgroupWindow").close();
    }
}

function SetAccessorDetail(AccessorId, typ) {
    SetAccessorDetailVisibility(typ);
    parameter = {};
    parameter.accessorId = AccessorId;
    parameter.typ = typ;
    var detailData = getData(parameter, 'GetAccessorDetail');

    switch (typ) {
        case 'PERSON':
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBAccessor').set_value(detailData[0].ACCESSOR);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_SaveAccessorImageButton').set_enabled(false);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_DelteAccessorImageButton').set_enabled(false);

            break;
        case 'ORGENTITY':
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBAccessor').set_value(detailData[0].TITLE_ACCESSOR);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup').set_value(detailData[0].OE_TITLE);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_SaveAccessorImageButton').set_enabled(false);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_DelteAccessorImageButton').set_enabled(false);


            break;
        case 'ACCESSORGROUP':
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup').set_value(detailData[0].GROUP_NAME);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_SaveAccessorImageButton').set_enabled(true);
            $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_DelteAccessorImageButton').set_enabled(true);
            break;
    }
}

function SetAccessorDetailVisibility(typ) {
    switch (typ) {
        case 'PERSON':
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_LabelGroup')[0].style.visibility = 'hidden';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup')[0].style.visibility = 'hidden';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_LabelAccessor')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBAccessor')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup')[0].readOnly = true;
            break;
        case 'ORGENTITY':
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_LabelGroup')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_LabelAccessor')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBAccessor')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup')[0].readOnly = true;
            break;
        case 'ACCESSORGROUP':
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_LabelAccessor')[0].style.visibility = 'hidden';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBAccessor')[0].style.visibility = 'hidden';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_LabelGroup')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup')[0].style.visibility = 'visible';
            $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup')[0].readOnly = false;
            break;
    }
}

function ClearAccessorDetail() {
    $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBGroup").set_value('');
    $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_TBAccessor").set_value('');
    $find("ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_NewAccessorgroupWindow_C_TBNewGroupTitle").set_value('');

}

function SaveAuthorisationClicking(sender, args) {
    var id = $("#" + windowClientId + "_C_Id")[0].value;
    var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
    SaveAuthorisation(id, typ);
}

function DeleteAuthorisationClicking(sender, args) {
    var id = $find(windowClientId + "_C_ctl00_AuthorisationListBox").get_selectedItem().get_value();
    var typ = $("#" + windowClientId + "_C_AuthorisationTyp")[0].value;
    DeleteAuthorisation(id, typ);
}

function AddAuthorisationClick(sender, args) {
    AccessorWindowShow();
}

function ApplicationAuthorisationWindowShow(sender, args) {
    $("#" + windowClientId + "_C_Id")[0].value = 0;
    $("#" + windowClientId + "_C_AuthorisationTyp")[0].value = 'RELEASE';
    AuthorisationWindowShow(0, 'RELEASE');
}
