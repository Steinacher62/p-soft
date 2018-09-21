var dutyTree;
var functionDescriptionTree;
var dutyDetailview;
var functionDescriptionDetail;
var functionDescriptionDutyDetail;
var functionDescriptionDetailMultiPageview;

$(document).ready(function () {
    functionDescriptionTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_FunctionDescriptionTree');
    dutyTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DutyTree');
    dutyDetailview = $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM')[0];
    dutyGroupDetailview = $('#ContentPlaceHolder1_LMRORU_Layout_PageViewM1')[0];
    functionDescriptionDetailMultiPageview = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_MultiPageM');
    PaneLResized();
    PaneRUResized();
    ClearDetailData();
});

function PaneLResized() {
    var paneLHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("height").replace("px", "");
    var paneLWidth = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL").css("width").replace("px", "");
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_FunctionDescriptionTree").css("height", paneLHeight - 37 + 'px');
    $("#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_FunctionDescriptionTree").css("width", paneLWidth - 10 + 'px');
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DutyTree').css("height", paneRUHeight + 'px');

}

function DutySearchClick() {
    $find("ctl00_ProgressWindow").show();
    parameter = {};
    parameter.name = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_TBName').get_value();
    parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_TBDescriptionSearch').get_value();
    dutyTreeData = getData(parameter, "GetDutyTree");
    var rootNodeAdded = false;
    dutyTree.get_nodes().clear();
    dutyTreeData.forEach(function (node) {
        var item = new Telerik.Web.UI.RadTreeNode();
        item.set_text(node.TITLE);
        item.set_value(node.ID);
        item.set_imageUrl(node.IMAGE);
        item.get_attributes().setAttribute("TYP", node.TYP);
        item.get_attributes().setAttribute("ORDNUMBER", node.ORDNUMBER);
        if (node.PARENT_ID === null && rootNodeAdded === false) {
            dutyTree.get_nodes().add(item);
            rootNodeAdded = true;
        } else {
            if (rootNodeAdded && node.PARENT_ID > 0) {
                dutyTree.findNodeByValue(node.PARENT_ID).get_nodes().add(item);
            }
        }
    });
    if (parameter.name.length > 0 || parameter.description.length > 0) {
        var allNodes = dutyTree.get_allNodes();
        allNodes.forEach(function (node) {
            if (node.get_attributes().getAttribute('TYP') === "DUTYGROUP" && node.get_nodes().get_count() === 0) {
                var parentNode = node.get_parent();
                parentNode.get_nodes().remove(node);
                while (parentNode.get_nodes().get_count() === 0) {
                    var removeNode = parentNode;
                    parentNode = removeNode.get_parent();
                    parentNode.get_nodes().remove(removeNode);
                }
            }
        });
    }

    $find("ctl00_ProgressWindow").close();
}

function DutyTreeNodeClicked(sende, args) {
    if (args.get_node().get_attributes().getAttribute('TYP') === 'DUTY')
        SetDutyDetail(args.get_node().get_value(), false);
}

function SaveDutyOrGroupClicking() {
    parameter = {};
    if (functionDescriptionDetailMultiPageview.get_selectedIndex() === 0) {
        parameter.functionId = functionDescriptionTree.get_selectedNode().get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_descriptionData').get_value();
        getData(parameter, 'SaveFunctionDescription');
    }
    else {
        parameter.competences = [];
        parameter.dutyCompetenceId = functionDescriptionTree.get_selectedNode().get_value();
        parameter.functionId = functionDescriptionTree.get_selectedNode().get_parent().get_parent().get_value();
        var competencesCheckboxes = $('#ContentPlaceHolder1_LMRORU_Layout_ctl04_competenceTable .RadCheckBox').toArray();
        var i = 0;
        competencesCheckboxes.forEach(function (cb) {
            parameter.competences[i] = {};
            parameter.competences[i].competenceId = $find(cb.id).get_value();
            parameter.competences[i].checked = $find(cb.id).get_checked();
            i += 1;
        });

        getData(parameter, 'SaveCompetences');
    }

}

function DeleteDutyOrGroupClicking() {

    if (functionDescriptionTree.get_selectedNode() !== undefined) {
        var selectedNode = functionDescriptionTree.get_selectedNode();
        var nodeId = selectedNode.get_value();
        var typ = selectedNode.get_attributes().getAttribute('TYP');
        switch (typ) {
            case 'DUTY':
                DeleteDutyFunctiondescription(nodeId);
                break;
            case 'FUNCTION':
                DeteteFunctiondescription(nodeId);
                break;
        }
    }
}

function DutyTreeNodeDropping(sender, args) {
    var parameter = {};
    if (args._destNode === null || !(args._destNode.get_attributes().getAttribute('TYP') === 'FUNCTION')) {
        parameter.text = "nodeDropingError";
        parameter.scope = "error";
        errorMessage = getData(parameter, "Translate");
        radalert(errorMessage[0].Text);
        args.set_cancel(true);
    }
    else {
        parameter.targetId = args._destNode.get_value();
        parameter.sourceId = args.get_sourceNode().get_value();
        detailDat = getData(parameter, 'AddDutyFunctionsdescription');
        args._destNode.set_imageUrl(detailDat[0].IMAGE);
        //args._destNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
        newDuty = true;
        newDutyData = detailDat[0];
        dropFunctionId = parameter.targetId;
        args._destNode.expand();

        //var dutyGroup = functionDescriptionTree.findNodeByValue(dropFunctionId).get_nodes().toArray().find(function (node) {
        //    return node.get_value().toString() === newDutyData.DUTYGROUP_ID.toString();
        //});

        var dutyGroup;
        functionDescriptionTree.findNodeByValue(dropFunctionId).get_nodes().toArray().forEach(function (node) {
                if (node.get_value().toString() === newDutyData.DUTYGROUP_ID.toString()) {
                    dutyGroup = node;
                }
        });

        if (dutyGroup === undefined) {
            var sourceDutyGroup = dutyTree.get_selectedNode().get_parent();
            newGroupNode = new Telerik.Web.UI.RadTreeNode();
            newGroupNode.set_value(sourceDutyGroup.get_value());
            newGroupNode.set_text(sourceDutyGroup.get_text());
            newGroupNode.set_imageUrl(sourceDutyGroup.get_imageUrl());
            newGroupNode.get_attributes().setAttribute("TYP", "DUTYGROUP");
            newGroupNode.get_attributes().setAttribute("FUNCTIONID", newDutyData.FUNKTION_ID);
            functionDescriptionTree.findNodeByValue(newDutyData.FUNKTION_ID).get_nodes().add(newGroupNode);
            dutyGroup = newGroupNode;
        }

        dutyGroup.expand();
        newNode = new Telerik.Web.UI.RadTreeNode();
        newNode.set_value(newDutyData.ID);
        newNode.set_text(newDutyData.TITLE);
        newNode.set_imageUrl(newDutyData.IMAGE);
        newNode.get_attributes().setAttribute("TYP", "DUTY");
        newNode.get_attributes().setAttribute("DUTY_ID", newDutyData.DUTY_ID);
        dutyGroup.get_nodes().add(newNode);
        newNode.set_selected(true);
        SetDutyDetail(newDutyData.DUTY_ID, true);
    }

}

function FunctionDescriptionTreePopulating(sender, args) {
    args.get_node().get_nodes().clear();
}

function FunctionDescriptionTreeMenuClicked(sender, args) {
    var command = args.get_menuItem().get_value();
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    nodeId = args.get_node().get_value();
    switch (command) {
        case 'Delete':
            if (typ === 'DUTY') {
                DeleteDutyFunctiondescription(nodeId);
            }
            else {
                DeteteFunctiondescription(nodeId);
            }
            break;
        case 'RefFunctionsView':
            parameter = {};
            parameter.id = args.get_node().get_attributes().getAttribute('DUTY_ID');
            detailData = getData(parameter, 'GetFunctionWithDuty');
            functionTable = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_FunctionDutyWindow_C_FunctionDutyGrid');
            functionTable.get_masterTableView().set_dataSource(detailData);
            functionTable.get_masterTableView().dataBind();
            $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_FunctionDutyWindow').show();
            break;
    }
}

function DeleteDutyFunctiondescription(nodeId) {
    node = functionDescriptionTree.findNodeByValue(nodeId);
    parentNode = node.get_parent();
    functionNode = parentNode.get_parent();
    parameter = {};
    parameter.id = nodeId;
    getData(parameter, 'DeleteDutyFunctiondescription');
    node.get_parent().get_nodes().remove(node);
    if (parentNode.get_nodes().get_count() === 0) {
        functionNode.get_nodes().remove(parentNode);
    }
    if (functionNode.get_nodes().get_count() === 0) {
        functionNode.set_imageUrl(GetImagePath() + 'fx_funktion_inaktiv.gif');
    }
    ClearDetailData();
}

function DeteteFunctiondescription(nodeId) {
    var confirmWindow = radconfirm(Translate('DeleteFunctionDescriptionConfirm', 'functionDescription')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            confirmWindow.Close();
            node = functionDescriptionTree.findNodeByValue(nodeId);
            parameter = {};
            parameter.id = nodeId;
            getData(parameter, 'DeleteFunctiondescription');
            node.get_nodes().clear();
            node.set_imageUrl(GetImagePath() + 'fx_funktion_inaktiv.gif');
            ClearDetailData();
        }
    });
}

function FunctionDescriptionTreeNodeClicked(sender, args) {
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    var id = args.get_node().get_value();
    var dutyId = args.get_node().get_attributes().getAttribute('DUTY_ID');
    switch (typ) {
        case 'FUNCTION':
            SetFunctionDetail(id);
            break;
        case 'DUTY':
            SetDutyDetail(dutyId, true);
            break;
    }
}

function SetFunctionDetail(id) {
    ClearDetailData();
    functionDescriptionDetailMultiPageview.set_selectedIndex(0);
    parameter = {};
    parameter.id = id;
    detailDat = getData(parameter, "GetFunctionDetail");
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_titleData').set_value(detailDat[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_descriptionData').set_value(detailDat[0].DESCRIPTION);

}

function SetDutyDetail(id, showCompetences) {
    ClearDetailData();
    functionDescriptionDetailMultiPageview.set_selectedIndex(1);
    parameter = {};
    parameter.id = id;
    detailDat = getData(parameter, "GetDutyData");
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_numberData').set_value(detailDat[0].NUMBER);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_titleData').set_value(detailDat[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl04_descriptionData').set_value(detailDat[0].DESCRIPTION);
    if (showCompetences) {
        $("#ContentPlaceHolder1_LMRORU_Layout_ctl04_competenceTable")[0].style.display = "table";
        parameter = {};
        parameter.id = functionDescriptionTree.get_selectedNode().get_value();
        var competences = getData(parameter, "GetCompetences");
        if (!(competences === "")) {
            var competencesCheckboxes = $('#ContentPlaceHolder1_LMRORU_Layout_ctl04_competenceTable .RadCheckBox').toArray();

            competencesCheckboxes.forEach(function (competence){
                var competenceId = $find(competence.id).get_value();
                for (var i = 0, len = competences.length; i < len; i++) {
                    if (competences[i].COMPETENCE_LEVEL_ID.toString() === competenceId) {
                        $find(competence.id).set_checked(true);
                        break;
                    }
                }

            });
        }
    }
    else {
        $("#ContentPlaceHolder1_LMRORU_Layout_ctl04_competenceTable")[0].style.display = "none";
    }
}

function FunctionDescriptionTreeContextMenuShowing(sender, args) {
    args.get_node().set_selected(true);
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    switch (typ) {
        case 'DUTY':
            args.get_node().get_contextMenu().findItemByValue("RefFunctionsView").show();
            args.get_node().get_contextMenu().findItemByValue("Delete").show();
            break;
        case 'DUTYGROUP':
            args.get_node().get_contextMenu().findItemByValue("RefFunctionsView").hide();
            args.get_node().get_contextMenu().findItemByValue("Delete").hide();
            break;
        case 'FUNCTION':
            args.get_node().get_contextMenu().findItemByValue("RefFunctionsView").hide();
            args.get_node().get_contextMenu().findItemByValue("Delete").show();
            break;
        default:
            args.get_node().get_contextMenu().findItemByValue("RefFunctionsView").hide();
            args.get_node().get_contextMenu().findItemByValue("Delete").hide();
            break;
    }
}

function FunctionDescriptionTreeNodeDragStart(sender, args) {
    typ = args.get_node().get_attributes().getAttribute('TYP');
    if (!(typ === 'FUNCTION')) {
        args.set_cancel(true);
    }
}

function FunctionDescriptionTreeNodeDropping(senderr, args) {
    var parameter = {};
    if (args._destNode === null || !(args._destNode.get_attributes().getAttribute('TYP') === 'FUNCTION')) {
        parameter.text = "nodeDropingFunctiondescriptionError";
        parameter.scope = "error";
        errorMessage = getData(parameter, "Translate");
        radalert(errorMessage[0].Text);
        args.set_cancel(true);
    }
    else {
        parameter.destId = args._destNode.get_value()
        parameter.sourceId = args.get_sourceNode().get_value();
        detailData = getData(parameter, "CopyFunctiondescription");
        args._destNode.set_imageUrl(getData(parameter, 'GetImagePath') + 'fx_funktion.gif');
        args._destNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
    }
}

function ClearDetailData() {
    $('.DutyTable .Textbox').each(function () { $(this)[0].value = ''; });
    $('.DutyTable .TextboxMultiLine').each(function () { $(this)[0].value = ''; });
    $('.FunctionDescriptionDetailTable .Textbox').each(function () { $(this)[0].value = ''; });
    $('.FunctionDescriptionDetailTable .TextboxMultiLine').each(function () { $(this)[0].value = ''; });
    $('.CompetenceTable .RadCheckBox').each(function () {
        $find($(this)[0].id).set_checked(false);
    });
    $('.SearchTable .Textbox').each(function () { $(this)[0].value = ''; });
    $('.SearchTable .TextboxMultiLine').each(function () { $(this)[0].value = ''; });
}




