var tree;
var personTree;
var clipboardTree;
var imageUrl;

$(document).ready(function () {
    tree = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl00_OETree");
    personTree = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl05_PersonTree");
    clipboardTree = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl00_ClipboardTree");
    imageUrl = $("#ContentPlaceHolder1_AdminContentLayout_imageUrl")[0].value;
    PaneLeftResized();
    PaneRightBottomResized();
    SetOECommandOff();
    SetPersonCommandOff();
    SetJobCommandOff();
    ClearPersonData();
    ClearJobData();
    ClearOEData();
    ClearClipboardData();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").set_selectedIndex(2);
});


function PaneRightBottomResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_AdminContentLayout_PaneRightBottom").css("height").replace("px", "");
    var paneRUWidth = $("#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_AdminContentLayout_PaneRightBottom").css("width").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl05_PersonTree').css("height", paneRUHeight - 10 + 'px');
    $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl05_PersonTree').css("width", paneRUWidth - 10 + 'px');
}

function PaneLeftResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_AdminContentLayout_PaneLeft").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_AdminContentLayout_PaneLeft").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl00_OETree").css("height", paneLHeight - 32 + 'px');
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl00_OETree").css("width", paneLWidth  + 'px');
}

function OrgClientContextMenuShowing(sender, args) {
    node = tree.findNodeByValue(args._node._properties._data.value);
    node.select();
    nodeTyp = tree.get_selectedNode().get_attributes()._data.TYP;
    setMenu(node, nodeTyp);
}

function MenuOrgClicking(sender, args) {
    var itemValue = args.get_menuItem().get_value();
    var node = args.get_node();
    var selectedNode = tree.get_selectedNode();
    parameter = {};
    var typ = node.get_attributes().getAttribute("TYP");
    switch (itemValue) {
        case 'Rename':
            if (typ == 'JOB') {
                RenameJob(node);
            }
            if (typ == 'ORGENTITY' || typ == 'FIRM') {
                RenameOrgentity(node);
            }

            break;
        case 'NewOrgentity':
            $find("ctl00_ProgressWindow").show();
            parameter.parentId = selectedNode.get_value();
            newOrgentity = getData(parameter, 'AddOrgentity');
            tree.trackChanges();
            var newNode = new Telerik.Web.UI.RadTreeNode();
            newNode.set_value(newOrgentity[0].ID);
            newNode.set_text(newOrgentity[0].TITLE);
            newNode.set_imageUrl(imageUrl + 'og_abteilung.gif');
            newNode.get_attributes().setAttribute("TYP", "ORGENTITY");
            var position = GetNewNodePositionInNodes(selectedNode.get_nodes(), newNode.get_text());
            selectedNode.get_nodes().insert(position, newNode);
            tree.commitChanges();
            selectedNode.expand();
            newNode.set_selected(true);
            newNode.scrollIntoView();
            setOrgData(newOrgentity[0].ID);
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("orgentity").set_selected(true);
            $find("ctl00_ProgressWindow").close();
            break;
        case 'NewJob':
            parameter.oeId = selectedNode.get_value();
            parameter.personId = 0;
            result = getData(parameter, "AddJob");
            tree.trackChanges();
            var newNode = new Telerik.Web.UI.RadTreeNode();
            newNode.set_value(result[0].ID);
            newNode.set_text(result[0].TITLE);
            newNode.set_imageUrl(imageUrl + 'og_stelle_vakant.gif');
            newNode.get_attributes().setAttribute("TYP", "JOB");
            newNode.get_attributes().setAttribute("VAKANT", "TRUE");
            var position = GetNewNodePositionInNodes(selectedNode.get_nodes(), newNode.get_text());
            node.get_nodes().insert(position, newNode);
            tree.commitChanges();
            selectedNode.expand();
            newNode.set_selected(true);
            newNode.scrollIntoView();
            setJopData(result[0].ID);
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("job").set_selected(true);
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EmploymentButton").set_text(Translate('VACANT', 'EMPLOYMENT')[0].Text + ' ');

            setOrgData(node.get_value());

            break;
        case 'Delete':
            if (typ == 'JOB') {
                DeleteJob(node);
            }
            if (typ == 'ORGENTITY' || typ == 'FIRM') {
                DeleteOrgentity(node);
            }
            break;

        case 'Authorisation':
            $("#ctl00_ContentPlaceHolder1_AdminContentLayout_AuthorisationWindow_C_Id")[0].value = node.get_value();
            $("#ctl00_ContentPlaceHolder1_AdminContentLayout_AuthorisationWindow_C_AuthorisationTyp")[0].value = typ;
            AuthorisationWindowShow(node.get_value(), typ);

            break;
    }


}

function setMenu(node, nodeTyp) {
    switch (nodeTyp) {
        case 'FIRM':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewJob").show();
            node.get_contextMenu().findItemByValue("NewOrgentity").show();
            break;
        case 'ORGENTITY':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewJob").show();
            node.get_contextMenu().findItemByValue("NewOrgentity").show();
            break;
        case 'JOB':
            node.get_contextMenu().findItemByValue("Rename").show();
            node.get_contextMenu().findItemByValue("NewJob").show();
            node.get_contextMenu().findItemByValue("NewOrgentity").hide();
            break;
    }
}

function NodeClicking(sender, args) {
    typ = args._node._attributes._data.TYP;
    id = args._node._properties._data.value;
    if (typ == 'FIRM' || typ == "ORGENTITY") {
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("orgentity").set_selected(true);
        setOrgData(id);
        $(".OrganiationTable").css('display', 'block');
        ClearJobData();
        var a = ClearPersonData();
        personTree.unselectAllNodes();
    }
    else {
        setJopData(id);
        ClearOEData();
        setOrgData(args._node.get_parent().get_value());
        parameter = {};
        parameter.jobId = id;
        var personId = getData(parameter, "GetPersonFromJob");
        if (!personId == '') {
            SetPersonDataDetail(personId[0].ID);
            var node = personTree.findNodeByValue(personId[0].ID);
            if (node == null) {
                ClearPersonSearchData();
                SearchPerson();
            }
            var selectedNode = personTree.findNodeByValue(personId[0].ID);
            if (selectedNode == null) {
                param = {};
                param.personId = personId[0].ID;
                var personData = getData(param, 'getPersonData');
                if (personData[0].LEAVING != null) {
                    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBPersonnelnumber").set_textBoxValue(personData[0].PERSONNELNUMBER);
                    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_CBShowFormer").set_checked(true);
                }
                SearchPerson();
                ClearPersonSearchData();
                selectedNode = personTree.findNodeByValue(personId[0].ID);
                SetPersonDataDetail(personId[0].ID);
            }
            selectedNode.set_selected(true);
            selectedNode.scrollIntoView();
        }
        else {
            ClearPersonData();
            if (!(personTree.get_selectedNode() == null)) {
                personTree.get_selectedNode().set_selected(false);
            }
        }
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("job").set_selected(true);


        $(".JobTable").css('display', 'block');
    }
}

function OETreeColapseAll() {
    var nodes = tree.get_allNodes();
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].get_nodes() != null) {
            nodes[i].collapse();
        }
    }
}

function GetOrgentity(jobId) {
    var parameter = {};
    parameter.jobId = jobId;
    var orgentity = getData(parameter, 'getOrgentityFromJob');
    if (orgentity.length == 0) {
        return 0;
    }
    else {
        return orgentity[0].ID;
    }
    return orgentity[0].ID;
}

function setOrgData(id) {
    ClearOEData();
    if (!id == 0) {
        var parameter = {};
        parameter.id = id;
        orgData = getData(parameter, 'getOrgData');
        $("#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId")[0].value = id;
        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DivisionData")[0].value = orgData[0].TITLE;
        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_MnemonicData")[0].value = orgData[0].MNEMONIC;
        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DescriptionData")[0].value = orgData[0].DESCRIPTION;
        if (!(orgData[0].CLIPBOARD_ID == null)) {
            $find('ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_ClipboardButton').set_text(orgData[0].CLIPBOARD_TITLE);
            $find('ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_ClipboardButton').set_value(orgData[0].CLIPBOARD_ID);
        }
        SetJOECommandOn();
    }
    else {
        SetOECommandOff();
    }
}

function RenameOrgentity(sender, args) {
    setOrgData(node.get_value());
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("orgentity").set_selected(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DivisionData").focus();
}

function OETreeNodeDropping(sender, args) {
    var destNode = args.get_destNode();
    var sourceNode = args.get_sourceNode();
    var parameter = {};
    if (destNode != null && destNode.get_treeView().get_id() == 'ctl00_ContentPlaceHolder1_AdminContentLayout_ctl00_OETree') {
        var sourceTyp = sourceNode.get_attributes().getAttribute('TYP');
        var destTyp = destNode.get_attributes().getAttribute('TYP');
        if (sourceTyp == 'JOB' && (destTyp == 'ORGENTITY' || destTyp == 'FIRM')) {
            parameter.sourceJobId = sourceNode.get_value();
            parameter.targetOeId = destNode.get_value();
            result = getData(parameter, 'MoveJob');

            if (result[0].RESULT == 'ok') {
                var nodetmp = new Telerik.Web.UI.RadTreeNode();
                var attributes = sourceNode.get_attributes();
                for (i = 0; i < attributes.get_count(); i++) {
                    key = attributes._keys[i];
                    value = attributes.getAttribute(key);
                    nodetmp.get_attributes().setAttribute(key, value);
                }
                nodetmp.set_value(sourceNode.get_value());
                nodetmp.set_imageUrl(sourceNode.get_imageElement().src);
                nodetmp.set_text(sourceNode.get_text());
                tree.trackChanges();
                sourceNode.get_parent().get_nodes().remove(sourceNode);
                var nodeIndex = GetNewNodePositionInNodes(destNode.get_nodes(), nodetmp.get_text());
                destNode.get_nodes().insert(nodeIndex, nodetmp);
                tree.commitChanges();
                nodetmp.select();
                destNode.expand();
                nodetmp.scrollIntoView();
            }
        }
        if (sourceTyp == 'ORGENTITY' && (destTyp == 'ORGENTITY' || destTyp == 'FIRM')) {
            parameter.sourceOeId = sourceNode.get_value();
            parameter.targetOeId = destNode.get_value();
            result = getData(parameter, 'MoveOrgentity');
            if (result[0].ERROR == 'ok') {
                var sourceSubNodes = sourceNode.get_allNodes();
                var attributes = sourceNode.get_attributes();
                var nodetmp = new Telerik.Web.UI.RadTreeNode();
                for (i = 0; i < attributes.get_count(); i++) {
                    key = attributes._keys[i];
                    value = attributes.getAttribute(key);
                    nodetmp.get_attributes().setAttribute(key, value);
                }
                nodetmp.set_value(sourceNode.get_value());
                nodetmp.set_imageUrl(sourceNode.get_imageElement().src);
                nodetmp.set_text(sourceNode.get_text());

                sourceSubNodes.forEach(function (node) {
                    if (node.get_parent().get_value() === sourceNode.get_value()){
                    nodetmp.get_nodes().add(node);
                    }
                    else{
                        var parentNode = nodetmp.get_allNodes().find(function (node1) {
                            return node.get_parent().get_value() === node1.get_value();

                        });
                        parentNode.get_nodes().add(node);
                    }
                });

                sourceNode.get_parent().get_nodes().remove(sourceNode);
                var nodeIndex = GetNewNodePositionInNodes(destNode.get_nodes(), nodetmp.get_text());
                destNode.get_nodes().insert(nodeIndex, nodetmp);

            }

        }
    }
    args.set_cancel(true);
}

function ClearOEData() {
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DivisionData")[0].value = "";
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_MnemonicData")[0].value = "";
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DescriptionData")[0].value = "";
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_ClipboardButton').set_text(Translate('createClipboard', 'global')[0].Text);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_ClipboardButton').set_value(null);
}

function SetOECommandOff() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_SaveOEImageButton").set_enabled(false);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DelteOEImageButton").set_enabled(false);
    SetOEFieldsInactive();
}

function SetJOECommandOn() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_SaveOEImageButton").set_enabled(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DelteOEImageButton").set_enabled(true);
    SetOEFieldsActive();
}

function SetOEFieldsInactive() {
    $.each($(".OrganiationTable .riTextBox"), function (key, value) {
        $find(value.id).disable();
    });
    $.each($(".OrganiationTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".OrganiationTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".OrganiationTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });

}

function SetOEFieldsActive() {
    $.each($(".OrganiationTable .riTextBox"), function (key, value) {
        $find(value.id).enable();
    });
    $.each($(".OrganiationTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".OrganiationTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".OrganiationTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });

}

function SaveOEClick() {
    var parameter = {};
    parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId")[0].value;
    parameter.title = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DivisionData")[0].value;
    parameter.mnemonic = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_MnemonicData")[0].value;
    parameter.description = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DescriptionData")[0].value;
    orgData = getData(parameter, 'SaveOrgentity');
    tree.findNodeByValue(parameter.id).get_textElement().innerHTML = parameter.title;
}

function DeleteOEClick() {
    var oeId = $("#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId")[0].value;
    var node = tree.findNodeByValue(oeId);
    DeleteOrgentity(node);
}

function DeleteOrgentity(node) {
    var parameter = {};
    var deleteConfirmText = Translate('deleteOEConfirm', 'organisation');
    var confirmWindow = radconfirm(deleteConfirmText[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            $find("ctl00_ProgressWindow").show();
            var parameter1 = {};
            parameter1.id = node.get_value();
            var status = getData(parameter1, 'DeleteOrgentity');
            tree.trackChanges();
            node.get_parent().get_nodes().remove(node);
            tree.commitChanges();
            $find("ctl00_ProgressWindow").close();
        }
    });
}

function ClearOeTree() {
    OETreeColapseAll();
    tree.unselectAllNodes();
}

function setJopData(id) {
    ClearJobData();
    if (id > 0) {
        var parameter = {};
        parameter.id = id;
        jobData = getData(parameter, 'getJobData');
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_FunktionData").findItemByValue(jobData[0].FUNKTION_ID).select();
        $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value = jobData[0].JOB_ID;
        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_JobTitleData")[0].value = jobData[0].JOB_TITLE;
        $("#ContentPlaceHolder1_AdminContentLayout_ctl02_OeData")[0].textContent = jobData[0].ORGENTITY_TITLE;
        
        if (jobData[0].EMPLOYMENT_TITLE == null) {
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EmploymentButton").set_text(Translate('VACANT', 'EMPLOYMENT')[0].Text + ' ');
        }
        else {
            $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EmploymentButton")[0].textContent = jobData[0].EMPLOYMENT_TITLE;
        }
        if (jobData[0].PROXY_NAME != null) {
            $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteButton")[0].textContent = jobData[0].PROXY_NAME;
            $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteButton")[0].value = jobData[0].PROXY_PERSON_ID;
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteSetVacant").set_enabled(true);
        }
        else {
            $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteButton")[0].textContent = 'vakant';
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteSetVacant").set_enabled(false);
        }
        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EngagementData")[0].value = jobData[0].ENGAGEMENT;
        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_DescriptionJobData")[0].value = jobData[0].JOB_DESCRIPTION;
        if (jobData[0].FROM_DATE != null) {
            $("#ContentPlaceHolder1_AdminContentLayout_ctl02_FromData")[0].innerText = GetDateDEFromJSON(jobData[0].FROM_DATE);
        }
        if (jobData[0].TO_DATE != null) {
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_ToData_dateInput").set_selectedDate(new Date(GetDateFromJson(jobData[0].TO_DATE)));
        }

        $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_MnemonicJobData")[0].value = jobData[0].JOb_MNEMONIC;
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_TypeData").findItemByValue(jobData[0].TYP).select();
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_MainFunctionData").set_checked(jobData[0].HAUPTFUNKTION);

        SetJobCommandOn();
    }
    else {
        SetJobCommandOff();
    }
}

function FunktionSelected(sender, args) {
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_JobTitleData")[0].value = args.get_item().get_text();
}

function SaveJobClick() {
    var parameter = {};
    parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value;
    parameter.jobTitle = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_JobTitleData")[0].value;
    parameter.funktionId = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_FunktionData").get_selectedItem().get_value();
    if (!($find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_ToData_dateInput").get_selectedDate() == null)) {
        parameter.toDate = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_ToData_dateInput").get_selectedDate().format("dd.MM.yyyy");
    }
    else {
        parameter.toDate = null;
    }
    parameter.mnemonic = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_MnemonicJobData")[0].value;
    parameter.bg = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EngagementData").get_editValue();
    parameter.description = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_DescriptionJobData")[0].value;
    parameter.typ = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_TypeData").get_selectedItem().get_value();
    parameter.mainfunction = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_MainFunctionData").get_checked();

    jobData = getData(parameter, 'SaveJob');

    node = tree.findNodeByValue(parameter.id);
    jobNodeText = parameter.jobTitle;
    if (node.get_attributes().getAttribute('VAKANT') == 'FALSE') {
        var parameter1 = {};
        parameter1.jobId = parameter.id;
        person = getData(parameter1, 'GetPersonFromJob');
        jobNodeText += ' ' + personTree.findNodeByValue(person[0].ID).get_text();
    }
    node.get_textElement().innerHTML = jobNodeText;
}

function DeleteJobClick() {
    var jobId = $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value;
    var node = tree.findNodeByValue(jobId);
    DeleteJob(node);
}

function DeleteJob(node) {
    var parameter = {};
    var deleteConfirmText = Translate('deleteJobConfirm', 'organisation');
    var confirmWindow = radconfirm(deleteConfirmText[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            var parameter1 = {};
            parameter1.id = node.get_value();
            var status = getData(parameter1, 'DeleteJob');
            tree.trackChanges();
            node.get_parent().get_nodes().remove(node);
            tree.commitChanges();
            ClearJobData();
            ClearOEData();
        }
    });




}

function RenameJob(node) {
    setJopData(node.get_value());
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("job").set_selected(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_JobTitleData").focus();
}

function ClearJobData() {
    $("#ContentPlaceHolder1_AdminContentLayout_ctl02_Jobs")[0].Value = 0;
    $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].Value = 0;
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_JobTitleData")[0].value = '';
    $("#ContentPlaceHolder1_AdminContentLayout_ctl02_OeData")[0].textContent = '';
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_FunktionData").findItemByValue(0).select();
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EmploymentButton")[0].textContent = 'vakant';
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteButton")[0].textContent = 'vakant';
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteSetVacant").set_enabled(false);
    $("#ContentPlaceHolder1_AdminContentLayout_ctl02_FromData")[0].innerText = '';
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_MnemonicJobData")[0].innerText = '';
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EngagementData")[0].value = '';
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_DescriptionJobData")[0].value = '';
    $("#ContentPlaceHolder1_AdminContentLayout_ctl02_FromData")[0].innerText = '';
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_ToData_dateInput").clear();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_TypeData").findItemByValue(0).select();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_MainFunctionData").set_checked(false);
    SetJobCommandOff();
}

function GetJobsPerson(personId) {
    var parameter = {};
    parameter.personId = personId;
    return getData(parameter, 'GetJobsPerson');
}

function GetMainJobId() {
    var jobs = $("#ContentPlaceHolder1_AdminContentLayout_ctl02_Jobs")[0].Value;
    for (i = 0; i < jobs.length; i++) {
        if (jobs[i].HAUPTFUNKTION === 1) {
            return jobs[i]['ID'];
        }
    }
    return 0;
}

function JobSetVacant(sender, args) {
    var treeNode = tree.findNodeByValue($("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value);
    var nodesFromParent = treeNode.get_parent().get_allNodes();
    var nodesFromParentTmp = {};
    nodesFromParentTmp.nodes = [];
    var parameter = {};
    var parameter1 = {};
    parameter.jobId = $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value;
    var job = getData(parameter, 'JobSetVacant');

    var index = 0;

    var CopyOfTreeNodes = GetCopyTreeNodesFromParent(tree, $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value);


    CopyOfTreeNodes.nodes.forEach(function (node) {
        if (node.get_value() == job[0].ID) {
            node.get_attributes().setAttribute("VAKANT", "TRUE");
            node.set_imageUrl(node.get_imageUrl().replace('og_stelle.gif', 'og_stelle_vakant.gif'));
            node.set_text(job[0].TITLE);
            ClearJobData();
            SetJobCommandOff();
        }

    });

    treeNode.get_parent().get_nodes().clear();
    tree.trackChanges();
    CopyOfTreeNodes.nodes.forEach(function (node) {
        tree.findNodeByValue(node.get_attributes().getAttribute("PARENTID")).get_nodes().add(node);
    });
    tree.commitChanges();
    args.set_cancel(true);

}

function EmploymentClicking(sender, args) {
    args.set_cancel(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow").show();
    var employment = GetEmploymentData($("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value);
    $('#ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow_C_ctl00_employmentId')[0].value = employment[0].ID;
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow_C_ctl00_EmploymentTitleData")[0].value = employment[0].TITLE;
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow_C_ctl00_EmploymentEngagementData")[0].textContent = employment[0].ENGAGEMENT + ' %';
}

function GetEmploymentData(jobId) {
    var parameter = {};
    parameter.jobId = jobId;
    return getData(parameter, 'GetEmployment');
}

function UpdateEmploymentClicking(sender, args) {
    var parameter = {};
    parameter.employmentId = $('#ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow_C_ctl00_employmentId')[0].value;
    parameter.employmentTitle = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow_C_ctl00_EmploymentTitleData")[0].value;
    $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_EmploymentButton')[0].textContent = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_EmploymentWindow_C_ctl00_EmploymentTitleData")[0].value;
    return getData(parameter, 'SetEmployment');

}

function SetJobCommandOff() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SaveJobImageButton").set_enabled(false);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_DelteJobImageButton").set_enabled(false);
    SetJobFieldsInactive();
}

function SetJobCommandOn() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SaveJobImageButton").set_enabled(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_DelteJobImageButton").set_enabled(true);
    SetJobFieldsActive();
}

function SetJobFieldsInactive() {
    $.each($(".JobTable .riTextBox"), function (key, value) {
        $find(value.id).disable();
    });
    $.each($(".JobTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".JobTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".JobTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });

}

function SetJobFieldsActive() {
    $.each($(".JobTable .riTextBox"), function (key, value) {
        $find(value.id).enable();
    });
    $.each($(".JobTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".JobTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".JobTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });

}

function SearchPersonClick() {
    SearchPerson();
}

function SearchPerson() {
    var node = personTree.findNodeByValue("1");
    node.collapse();
    node.get_treeView().trackChanges();
    node._children.clear();
    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
    node.get_treeView().commitChanges();

    var nodes = personTree.get_allNodes();
    for (var i = 0; i < nodes.length; i++) {

        if (nodes[i].get_nodes() != null) {
            nodes[i].expand();
            do {
                // alert("stop");
            }
            while (nodes[i]._expanding == true)

        }
    }

    ClearPersonData();
}

function ClearPersonSearchData() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBName").set_textBoxValue('');
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBFirstName").set_textBoxValue('');
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBMNEMO").set_textBoxValue('');
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBPersonnelnumber").set_textBoxValue('');
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_DDOrgentity").findItemByValue("0").select();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_CBShowFormer").set_checked(false);
}

function ClearPersonData() {
    $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value = 0;
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_FirmData").findItemByValue("0").select();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_ClipboardData").set_value(0);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_ClipboardData").set_text("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_NameData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_FirstnameData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MNEMOData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_TitleData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PersonnelnumberData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SexData").findItemByValue("0").select();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MartialData").findItemByValue("0").select();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_DateOfBirthData").clear();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EntryData").clear();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeavingData").clear();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LoginData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PasswordData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EMailData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PhoneData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MobileData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PhotoData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SalutationAddressData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SalutationLetterData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_BeschGradData").set_value(0);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_BerufserfahrungData").set_value(0);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeaderShipData").set_checked(false);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_CommentData").set_value("");
    SetPersonCommandOff();
}

function PersonTreeNodePopulating(NodeData, args) {
    args.get_context()['name'] = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBName")[0].value;
    args.get_context()['firstname'] = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBFirstName")[0].value;
    args.get_context()['mnemo'] = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBMNEMO")[0].value;
    args.get_context()['personnelnumber'] = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_TBPersonnelnumber")[0].value;
    args.get_context()['orgentity'] = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_DDOrgentity").get_selectedItem().get_value();
    args.get_context()['showformer'] = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl04_CBShowFormer")._checked;
}

function SavePersonClick() {
    ValidatorEnable('PersonTable');
    var isValid = Page_ClientValidate();
    ValidatorDisable('PersonTable');
    if (isValid) {
        var parameter = {};
        parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;

        var parameter1 = {};
        parameter1.pNr = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PersonnelnumberData").get_value();
        parameter1.id = parameter.id;
        var isPersonnelnumberInDb = getData(parameter1, 'ExistPersonnelnumber');
        if (isPersonnelnumberInDb.length > 0) {
            radalert(isPersonnelnumberInDb[0].ERROR_MESSAGE, 400, 200);
            return;
        }

        var personNode = null;
        var nameSurnameOld;
        if (parameter.id !== '0') {
            personNode = personTree.findNodeByValue(parameter.id);
            if (personNode !== null) {
                nameSurnameOld = personNode.get_text();
            }
        }
        parameter.firm_id = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_FirmData").get_selectedItem().get_value();
        parameter.name = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_NameData")._value;
        parameter.firstname = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_FirstnameData")._value;
        parameter.mnemo = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MNEMOData")._value;
        parameter.title = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_TitleData")._value;
        parameter.personnelnumber = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PersonnelnumberData")._value;
        parameter.sex = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SexData").get_selectedItem().get_value();
        parameter.martial = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MartialData").get_selectedItem().get_value();
        if (!($find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_DateOfBirthData").get_dateInput().get_selectedDate() == null)) {
            parameter.dateofbirth = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_DateOfBirthData").get_dateInput().get_selectedDate().format("dd.MM.yyyy");
        }
        else {
            parameter.dateofbirth = null;
        }
        if (!($find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EntryData").get_dateInput().get_selectedDate() == null)) {
            parameter.entry = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EntryData").get_dateInput().get_selectedDate().format("dd.MM.yyyy");
        }
        else {
            parameter.entry = null;
        }
        if (!($find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeavingData").get_dateInput().get_selectedDate() == null)) {
            parameter.leaving = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeavingData").get_dateInput().get_selectedDate().format("dd.MM.yyyy");
        }
        else {
            parameter.leaving = null;
        }
        parameter.login = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LoginData")._value;
        parameter.password = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PasswordData")._value;
        parameter.email = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EMailData")._value;
        parameter.phone = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PhoneData")._value;
        parameter.mobile = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MobileData")._value;
        parameter.photo = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PhotoData")._value;
        parameter.salutationaddress = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SalutationAddressData")._value;
        parameter.salutationletter = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SalutationLetterData")._value;
        parameter.beschgrad = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_BeschGradData")._value;
        parameter.berufserfahrung = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_BerufserfahrungData")._value;
        parameter.leadership = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeaderShipData").get_checked();
        parameter.comment = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_CommentData").get_value();

        personId = getData(parameter, 'SavePerson');

        if (personNode == null) {
            personTree.findNodeByValue(1).set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
            personTree.findNodeByValue(1).expand();
            personTree.findNodeByValue(1).set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(parameter.name + " " + parameter.firstname);
            node.set_value(personId);
            var nodeIndex = GetNewNodePositionInNodes(personTree.findNodeByValue(1).get_nodes(), node.get_text());
            personTree.findNodeByValue(1).get_nodes().insert(nodeIndex, node);
            personTree.findNodeByValue(personId).select();
            node.scrollIntoView();
            $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value = personId;
        }
        else {
            if (personNode !== null) {
                personNode.set_text(parameter.name + " " + parameter.firstname);
                var selectedJobs = tree.get_selectedNodes();
                selectedJobs.forEach(function (jobNode) {
                    jobNode.set_text(jobNode.get_text().replace(nameSurnameOld, parameter.name + " " + parameter.firstname));
                });
            }
        }
    }
}

function DeletePersonClick(sender, args) {
    var personId = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    if (!personId == 0) {
        var parameter = {};
        parameter.id = personId;
        var success = getData(parameter, "deletePerson");
        if (success[0].ERROR == "ok") {
            var node = personTree.findNodeByValue(personId);
            node.get_parent().get_nodes().remove(node);
            ClearPersonData();
        }
    }

}

function AddPersonClick(sender, args) {
    ClearPersonData();
    ClearJobData();
    ClearOEData();
    ClearOeTree();
    personTree.unselectAllNodes();
    SetPersonCommandOn();
}

function SetPersonCommandOff() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SavePersonImageButton").set_enabled(false);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_DeltePersonImageButton").set_enabled(false);
    //$find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_AddPersonImageButton").set_enabled(false);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_NewAddressPersonImageButton").set_enabled(false);
    SetPersonFieldsInactive();
}

function SetPersonCommandOn() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SavePersonImageButton").set_enabled(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_DeltePersonImageButton").set_enabled(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_AddPersonImageButton").set_enabled(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_NewAddressPersonImageButton").set_enabled(true);
    SetPersonFieldsActive();
}

function SetPersonFieldsInactive() {
    $.each($(".PersonTable .riTextBox"), function (key, value) {
        $find(value.id).disable();
    });
    $.each($(".PersonTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".PersonTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".PersonTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".PersonTable .FieldValidator"), function (key, value) {
        ValidatorEnable($("#" + value.id)[0].id, false);
    });

}

function SetPersonFieldsActive() {
    $.each($(".PersonTable .riTextBox"), function (key, value) {
        $find(value.id).enable();
    });
    $.each($(".PersonTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".PersonTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".PersonTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".PersonTable .FieldValidator"), function (key, value) {
        ValidatorEnable($("#" + value.id)[0].id, true);
    });

}

function PersonTreeNodeClicked(NodeData, args) {
    if (args._node.get_value() > 1) {
        personId = args._node._properties._data.value;
        SetPersonData(personId);
    }
}

function SetPersonData(personId) {
    SetPersonDataDetail(personId);

    var jobs = GetJobsPerson(personId);
    $("#ContentPlaceHolder1_AdminContentLayout_ctl02_Jobs")[0].Value = jobs;
    var mainJobId = GetMainJobId();
    if (mainJobId > 0) {
        setJopData(mainJobId);
        setOrgData(GetOrgentity(mainJobId));
    }
    else {
        if (jobs.length > 0) {
            setJopData(jobs[0].ID);
            setOrgData(GetOrgentity(jobs[0].ID));
        }
    }
    OETreeColapseAll();
    tree.unselectAllNodes();
    tree.set_multipleSelect(true);
    if (jobs.length > 0) {
        jobs.forEach(function (job, index) {
            var node = tree.findNodeByValue(job.ID);
            if (node != null) {
                node.select();
                while (node != null) {
                    if (node.expand) {
                        node.expand();
                    }

                    node = node.get_parent();
                }
            }
        });
        tree.set_multipleSelect(false);
        var selectedNode = tree.findNodeByValue(mainJobId);
        selectedNode.scrollIntoView();
    }
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("person").set_selected(true);
}

function SetPersonDataDetail(personId) {
    ClearPersonData();
    var parameter = {};
    parameter.personId = personId;
    var personData = getData(parameter, "getPersonData");
    $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value = personId;
    if (personData[0].FIRM_ID != null) {
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_FirmData").findItemByValue(personData[0].FIRM_ID).select();
    }
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_ClipboardData").set_value(personData[0].CLIPBOARD_ID);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_ClipboardData").set_text(personData[0].CLIPBOARD_TITLE);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_NameData").set_value(personData[0].PNAME);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_FirstnameData").set_value(personData[0].FIRSTNAME);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MNEMOData").set_value(personData[0].MNEMO);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_TitleData").set_value(personData[0].TITLE);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PersonnelnumberData").set_value(personData[0].PERSONNELNUMBER);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SexData").findItemByValue(personData[0].SEX).select();
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MartialData").findItemByValue(personData[0].MARTIAL).select();
    if (personData[0].DATEOFBIRTH != null) {
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_DateOfBirthData").set_selectedDate(new Date(GetDateFromJson(personData[0].DATEOFBIRTH)));
    }
    if (personData[0].ENTRY != null) {
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EntryData").set_selectedDate(new Date(GetDateFromJson(personData[0].ENTRY)));
    }
    if (personData[0].LEAVING != null) {
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeavingData").set_selectedDate(new Date(GetDateFromJson(personData[0].LEAVING)));
    }
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LoginData").set_value(personData[0].LOGIN);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PasswordData").set_value(personData[0].PASSWORD);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_EMailData").set_value(personData[0].EMAIL);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PhoneData").set_value(personData[0].PHONE);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_MobileData").set_value(personData[0].MOBILE);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_PhotoData").set_value(personData[0].PHOTO);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SalutationAddressData").set_value(personData[0].SALUTATION_ADDRESS);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_SalutationLetterData").set_value(personData[0].SALUTATION_LETTER);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_BeschGradData").set_value(personData[0].BESCH_GRAD);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_BerufserfahrungData").set_value(personData[0].BERUFSERFAHRUNG);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_LeaderShipData").set_checked(personData[0].LEADERSHIP);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ctl03_CommentData").set_value(personData[0].COMMENTS);

    SetPersonCommandOn();
}

function PersonNodeDragging(NodeData, args) {

}

function PersonNodeDropping(NodeData, args) {
    if (args._htmlElement.id == "ctl00_ContentPlaceHolder1_AdminContentLayout_ctl02_SubstituteButton") {
        var parameter = {};
        parameter.personId = args._sourceNodes[0]._properties._data.value;
        parameter.jobId = $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value;
        jobData = getData(parameter, 'setSubstitute');
        setJopData($("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value);
    }
    if (!(args.get_destNode() == null)) {
        var sourceNode = args.get_sourceNode();
        var targetNode = args.get_destNode();
        var typ = targetNode.get_attributes().getAttribute("TYP");
        var vakant = targetNode.get_attributes().getAttribute("VAKANT");
        parameter = {};

        if (typ == 'JOB' && vakant == 'TRUE') {
            parameter.personId = sourceNode.get_value();
            parameter.jobId = targetNode.get_value();
            result = getData(parameter, "SetJobOwner");
            var CopyOfTreeNodes = GetCopyTreeNodesFromParent(tree, targetNode.get_value());
            CopyOfTreeNodes.nodes.forEach(function (node) {
                if (node.get_value() == targetNode.get_value()) {
                    node.get_attributes().setAttribute("VAKANT", "FALSE");
                    node.set_imageUrl(node.get_imageUrl().replace('og_stelle_vakant.gif', 'og_stelle.gif'));
                    node.set_text(node.get_text() + " " + sourceNode.get_text());
                    setJopData(targetNode.get_value());
                    SetJobCommandOn();
                    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("job").set_selected(true);
                    targetNode.get_parent().get_nodes().clear();
                    tree.trackChanges();
                    CopyOfTreeNodes.nodes.forEach(function (node) {
                        tree.findNodeByValue(node.get_attributes().getAttribute("PARENTID")).get_nodes().add(node);
                    });
                    tree.commitChanges();

                }

            });
        }
        if (typ == 'FIRM' || typ == 'ORGENTITY') {
            parameter.oeId = targetNode.get_value();
            parameter.personId = sourceNode.get_value();
            result = getData(parameter, "AddJob");
            tree.trackChanges();
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_value(result[0].ID);
            node.set_text(result[0].TITLE + ' ' + sourceNode.get_text());
            node.set_imageUrl(imageUrl + 'og_stelle.gif');
            node.get_attributes().setAttribute("TYP", "JOB");
            node.get_attributes().setAttribute("VAKANT", "FALSE");
            targetNode.get_nodes().add(node);
            tree.commitChanges();
            node.set_selected(true);
            setJopData(result[0].ID);
            setOrgData(node.get_parent().get_value());
            SetPersonDataDetail(parameter.personId);
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_TabStripCenter").findTabByValue("job").set_selected(true);

        }

    }

    args.set_cancel(true);
}

function SubstituteSetVacantClicking(senderr, args) {
    var parameter = {};
    parameter.jobId = $("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value;
    var proxiPersonID = getData(parameter, 'SubstituteSetVacant');
    setJopData($("#ContentPlaceHolder1_AdminContentLayout_ctl02_JobId")[0].value);
    args.set_cancel(true);
}

function PersonClipboardClicking(sender, args) {
    if (!clipboardTree.get_allNodes().length == 0) {
        clipboardTree.findNodeByAttribute("IsRoot", "true").get_parent().get_nodes().clear();
    }
    clipboardTree.trackChanges();
    var node = new Telerik.Web.UI.RadTreeNode();
    node.set_text(sender.get_text());
    node.set_value(sender.get_value());
    node.get_attributes().setAttribute('IsRoot', 'true');
    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
    clipboardTree.get_nodes().add(node);
    clipboardTree.commitChanges();
    ClearClipboardData();
    var parameter = {};
    parameter.clipboardId = sender.get_value();
    parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    parameter.typ = 'person';
    var folderId = getData(parameter, 'GetClipboardFolderId')[0].folderId;
    if (!(folderId == "ERROR")) {
        SetFolderData(folderId);
        clipboardTree.findNodeByValue(sender.get_value()).select();
        clipboardTree.findNodeByValue(sender.get_value()).set_text($find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNameData").get_textBoxValue());
        clipboardTree.findNodeByValue(sender.get_value()).set_value(folderId);
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow").show();
        if (clipboardTree.get_selectedNode().get_attributes().getAttribute("IsRoot") == "true") {
            SetPersonDataDetail(parameter.id);
        }
        $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_Typ')[0].value = 'person'
        args.set_cancel(true);
    }
}

function ClipboardTreeNodeClicking(sender, args) {
    ClearClipboardData();
    var FolderId = 0;
    if (args._node.get_attributes().getAttribute("IsRoot") == "true") {
        var parameter = {};
        parameter.clipboardId = args.get_node().get_value();
        parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
        parameter.typ = $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_Typ')[0].value;

        FolderId = getData(parameter, 'GetClipboardFolderId')[0].folderId;
    }
    else {
        FolderId = args.get_node().get_value();
    }
    SetFolderData(FolderId);
    SetClipboardCommandOn();
    args._node.set_selected(true);
    args._node.expand();
    args.set_cancel(true);
}

function ClipboardMenuClicked(sender, args) {
    var itemValue = args.get_menuItem().get_value();
    switch (itemValue) {
        case "Rename":
            $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNameData").focus();
            SetClipboardCommandOn();
            break;
        case "NewOrgentity":
            NewFolder();
            break;
        case "Delete":
            DeleteFolder();
    }
}

function ClipboardOeClicking(sender, args) {
    if (!clipboardTree.get_allNodes().length == 0) {
        clipboardTree.findNodeByAttribute("IsRoot", "true").get_parent().get_nodes().clear();
    }
    clipboardTree.trackChanges();
    var node = new Telerik.Web.UI.RadTreeNode();
    node.set_text($('#ctl00_ContentPlaceHolder1_AdminContentLayout_ctl01_DivisionData')[0].value + ' ' + $('#ContentPlaceHolder1_AdminContentLayout_ctl01_ClipboardTitle').text());
    node.set_value($('#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId')[0].value);
    node.get_attributes().setAttribute('IsRoot', 'true');
    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
    clipboardTree.get_nodes().add(node);
    clipboardTree.commitChanges();
    ClearClipboardData();
    var parameter = {};
    if (!(sender.get_value() == null)) {
        parameter.clipboardId = sender.get_value();
    }
    else {
        parameter.clipboardId = 0;
    }
    parameter.id = $('#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId')[0].value;
    parameter.typ = 'orgentity';
    var folderId = getData(parameter, 'GetClipboardFolderId')[0].folderId;
    if (!(folderId == "ERROR")) {
        SetFolderData(folderId);
        clipboardTree.findNodeByValue($('#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId')[0].value).select();
        clipboardTree.findNodeByValue($('#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId')[0].value).set_text(node.get_text());
        clipboardTree.findNodeByValue($('#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId')[0].value).set_value(folderId);
        $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow").show();
        $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_Typ')[0].value = 'orgentity'
    }
    args.set_cancel(true);
}

function NewFolder() {
    ClearClipboardData();
    var parameter = {};
    parameter.parentFolderId = clipboardTree.get_selectedNode().get_value();
    parameter.rootId = clipboardTree.findNodeByAttribute('IsRoot', 'true').get_value();
    parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    parameter.typ = $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_Typ')[0].value;
    var folderDat = getData(parameter, 'GetNewFolder');
    SetFolderData(folderDat[0].ID);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNameData").focus();

    clipboardTree.trackChanges();
    var node = new Telerik.Web.UI.RadTreeNode();
    node.set_text(folderDat[0].TITLE);
    node.set_value(folderDat[0].ID);
    node.set_imageUrl(folderDat[0].IMAGEURL);
    node.get_attributes().setAttribute('IsRoot', 'false');

    clipboardTree.findNodeByValue(parameter.parentFolderId).get_nodes().add(node);
    clipboardTree.commitChanges();
    clipboardTree.get_selectedNode().expand();
    clipboardTree.findNodeByValue(folderDat[0].ID).select();

}

function SetFolderData(FolderId) {
    var parameter = {};
    parameter.FolderId = FolderId;
    var folderDat = getData(parameter, 'GetFolderData');
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_FolderId")[0].value = FolderId;
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNameData").set_value(folderDat[0].TITLE);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardDescriptionData").set_value(folderDat[0].DESCRIPTION);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardCreatethData_dateInput").set_selectedDate(new Date(GetDateFromJson(folderDat[0].CREATED)));
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNumVersionsData").findItemByValue(folderDat[0].NUMOFDOCVERSIONS).select();
    SetClipboardCommandOn();

}

function ClearClipboardData() {
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_FolderId")[0].value = 0;
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNameData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardDescriptionData").set_value("");
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardCreatethData_dateInput").clear();
}

function SetClipboardFieldsInactive() {
    $.each($(".ClipboardTable .riTextBox"), function (key, value) {
        $find(value.id).disable();
    });
    $.each($(".ClipboardTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".ClipboardTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".ClipboardTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(false);
    });
    $.each($(".ClipboardTable .FieldValidator"), function (key, value) {
        ValidatorEnable($("#" + value.id)[0].id, false);
    });

}

function SetClipboardFieldsActive() {
    $.each($(".ClipboardTable .riTextBox"), function (key, value) {
        $find(value.id).enable();
    });
    $.each($(".ClipboardTable .RadDropDownList"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".ClipboardTable .RadImageButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".ClipboardTable .RadButton"), function (key, value) {
        $find(value.id).set_enabled(true);
    });
    $.each($(".ClipboardTable .FieldValidator"), function (key, value) {
        ValidatorEnable($("#" + value.id)[0].id, true);
    });

}

function SetClipboardCommandOff() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_SaveClipboardImageButton").set_enabled(false);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_DelteClipboardImageButton").set_enabled(false);
    SetClipboardFieldsInactive();
}

function SetClipboardCommandOn() {
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_SaveClipboardImageButton").set_enabled(true);
    $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_DelteClipboardImageButton").set_enabled(true);
    SetClipboardFieldsActive();
}

function UpdateFolderClicking(sender, args) {
    if (Page_ClientValidate()) {
        var parameter = {};
        parameter.folderId = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_FolderId")[0].value;
        parameter.title = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNameData").get_value();
        parameter.description = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardDescriptionData").get_value();
        if (!($find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardCreatethData_dateInput").get_selectedDate() == null)) {
            parameter.created = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardCreatethData_dateInput").get_selectedDate().format("dd.MM.yyyy");
        }
        else {
            parameter.created = null;
        }
        parameter.numofdocversions = $find("ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_ClipboardNumVersionsData").get_selectedItem().get_value();

        getData(parameter, "UpdateFolder");

        var selectedNode = clipboardTree.get_selectedNode();
        selectedNode.set_text(parameter.title);

        SetClipboardCommandOff();
    }
}

function DeleteFolderClicking(sender, args) {
    DeleteFolder();
}

function DeleteFolder() {
    var typ = $('#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_Typ')[0].value;
    var parameter = {};
    parameter.folderId = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_ClipboardWindow_C_ctl01_FolderId")[0].value;
    if (typ == 'person') {
        parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    }
    if (typ == 'orgentity') {
        parameter.id = $("#ContentPlaceHolder1_AdminContentLayout_ctl01_OeId")[0].value;
    }
    parameter.typ = typ;
    getData(parameter, "DeleteFolder");

    var selectedNode = clipboardTree.get_selectedNode();

    ClearClipboardData();
    SetClipboardCommandOff();

    clipboardTree.trackChanges();
    if (selectedNode.get_attributes().getAttribute("IsRoot") == "true") {
        if (typ == 'person') {
            SetPersonDataDetail(parameter.id);
        }
        if (typ == 'orgentity') {
            setOrgData(id);
        }

        selectedNode.get_parent().get_nodes().clear();
    }
    else {
        selectedNode.get_parent().get_nodes().remove(selectedNode);
    }
    clipboardTree.commitChanges();
}

function AuthorisationFolderClicking(sender, args) {
    var id = $("#ctl00_ContentPlaceHolder1_AdminContentLayout_AuthorisationWindow_C_Id")[0].value = clipboardTree.get_selectedNode().get_value();
    $("#ctl00_ContentPlaceHolder1_AdminContentLayout_AuthorisationWindow_C_AuthorisationTyp")[0].value = 'FOLDER';
    var typ = 'FOLDER';
    AuthorisationWindowShow(id, typ);
}

function AddAddressPersonClick(sender, args) {
    SetAddressDetail();
}

function SetAddressDetail() {
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow').show();
    ClearAddressDetail();
    parameter = {};
    parameter.personId = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    var address = getData(parameter, 'GetAddress');
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressData1').set_value(address[0].ADDRESS1);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressData2').set_value(address[0].ADDRESS2);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressData3').set_value(address[0].ADDRESS3);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataZip').set_value(address[0].ZIP);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataCity').set_value(address[0].CITY);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddessDataCountry').findItemByValue(address[0].COUNTRY);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataPhone').set_value(address[0].PHONE);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataFax').set_value(address[0].FAX);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataMobil').set_value(address[0].MOBILE);
    $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataEMail').set_value(address[0].EMAIL_PRIVATE);
}

function SaveAddressClick(sender, args) {
    parameter = {};
    parameter.personId = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    parameter.address1 = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressData1').get_value();
    parameter.address2 = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressData2').get_value();
    parameter.address3 = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressData3').get_value();
    parameter.zip = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataZip').get_value();
    parameter.city = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataCity').get_value();
    parameter.country = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddessDataCountry').get_selectedItem().get_value();
    parameter.phone = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataPhone').get_value();
    parameter.fax = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataFax').get_value();
    parameter.mobil = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataMobil').get_value();
    parameter.email = $find('ctl00_ContentPlaceHolder1_AdminContentLayout_AddressWindow_C_ctl00_AddressDataEMail').get_value();
    getData(parameter, 'SaveAddress');
}

function DeleteAddressClick(sender, args) {
    parameter = {};
    parameter.personId = $("#ContentPlaceHolder1_AdminContentLayout_ctl03_PersonId")[0].value;
    getData(parameter, "DeleteAddress");
    ClearAddressDetail();
}

function ClearAddressDetail() {
    $('.AddressTable .Textbox').each(function () { $(this)[0].value = '' });
}



