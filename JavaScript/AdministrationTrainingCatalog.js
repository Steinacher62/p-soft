var trainingSearchList;
var trainigTree;

$(document).ready(function () {
    trainingSearchList = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_TrainingCatlogListBox');
    trainigTree = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_CatalogTree');
    PaneRUResized();
    PaneLResized();
    ClearTrainigDetail();
    $('.TrainigCatalogDetail').css('visibility', 'collapse');
    $('.TrainigDetail').css('visibility', 'visible');
});

function PaneLResized() {
    var paneLwidth = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL').css('width').replace('px', '');
    var paneLheight = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneL').css('height').replace('px', '');
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_CatalogTree').css('width', paneLwidth - 10 + 'px');
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl00_CatalogTree').css('height', paneLheight - 35 + 'px');

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_LMRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl02_TrainingCatlogListBox').css("height", paneRUHeight - 22 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBName').get_value();
    parameter.place = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBPlace').get_value();
    parameter.trainer = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl01_TBTrainer').get_value();
    detailData = getData(parameter, 'GetTrainingList');
    trainingSearchList.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        trainingSearchList.get_items().add(item);
    });
}

function TrainingCatlogListBoxIndexChanging(sender, args) {
    if (args.get_domEvent() !== null) {
        SetTrainingDetailData(args.get_item().get_value());
        trainigTree.findNodeByValue(args.get_item().get_value()).set_selected(true);
    }
    
}

function SaveClick() {
    if ($('.TrainigCatalogDetail').css('visibility') === 'visible') {
        SaveTrainingGroupDetail();
    }
    if ($('.TrainigDetail').css('visibility') === 'visible') {
        SaveTrainingDetail();
    }
}

function SaveTrainingDetail() {
    var parameter = {};
    parameter.id = $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingId')[0].value;
    parameter.parentId = 0;
    if (trainigTree.get_selectedNode() !== null)
    {
        parameter.parentId = trainigTree.get_selectedNode().get_value();
    }
    parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NameData').get_value();
    parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DescriptionData').get_value();
    parameter.validFrom = '';
    if ($find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidFromData_dateInput').get_selectedDate() !== null) {
        parameter.validFrom = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidFromData_dateInput').get_selectedDate().format("dd.MM.yyyy");
    }
    parameter.validTo = '';
    if ($find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidTo_dateInput').get_selectedDate() !== null) {
        parameter.validTo = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidTo_dateInput').get_selectedDate().format("dd.MM.yyyy");
    }
    parameter.costExternal = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CostExternData').get_value();
    parameter.costInternal = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CostInternData').get_value();
    parameter.location = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_PlaceData').get_value();
    parameter.participantNumber = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NumberParticipantData').get_value();
    parameter.instructor = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainerData').get_value();
    var detailData = getData(parameter, 'SaveTrainingDetail');
    if (parameter.id > 0) {
        trainigTree.findNodeByValue(detailData[0].ID).set_text(detailData[0].TITLE);
        trainigTree.findNodeByValue(detailData[0].ID).set_selected(true);
        if (trainingSearchList.findItemByValue(detailData[0].ID) !== null) {
            trainingSearchList.findItemByValue(detailData[0].ID).set_text(detailData[0].TITLE);
            trainingSearchList.findItemByValue(detailData[0].ID).set_selected(true);
        }
    }
    else {

        var imagePath = GetImagePath();

        var node = new Telerik.Web.UI.RadTreeNode();
        node.set_text(detailData[0].TITLE);
        node.set_value(detailData[0].ID);
        node.get_attributes().setAttribute('TYP', 'ITEM');
        node.set_imageUrl(imagePath + 'm_menue.gif');
        trainigTree.findNodeByValue(parameter.parentId).get_nodes().add(node);
        trainigTree.findNodeByValue(detailData[0].ID).set_selected(true);

        var item = new Telerik.Web.UI.RadListBoxItem;
        item.set_text(detailData[0].TITLE);
        item.set_value(detailData[0].ID);
        trainingSearchList.get_items().add(item);
        trainingSearchList.findItemByValue(detailData[0].ID).set_selected(true);
    }

    

    $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingId')[0].value = detailData[0].ID;
}

function SaveTrainingGroupDetail() {
    var parameter = {};
    parameter.id = $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingGroupId')[0].value;
    parameter.parentId = trainigTree.get_selectedNode().get_value();
    parameter.title = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupTitleData').get_value();
    parameter.description = $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupDescriptionData').get_value();
    var detailData = getData(parameter, 'SaveTrainingGroupDetail');
    if (parameter.id === '0') {
        var imagePath = GetImagePath();

        var node = new Telerik.Web.UI.RadTreeNode();
        node.set_text(detailData[0].TITLE);
        node.set_value(detailData[0].ID);
        node.get_attributes().setAttribute('TYP', 'GROUP');
        node.set_imageUrl(imagePath + 'm_menuegruppe.gif');
        trainigTree.findNodeByValue(parameter.parentId).get_nodes().add(node);
        trainigTree.findNodeByValue(detailData[0].ID).set_selected(true);
    }

    trainigTree.get_selectedNode().set_text(detailData[0].TITLE);

}

function CatalogMenuClicked(sender, args) {
    var nodeId = args.get_node().get_value();
    var menu = args.get_menuItem().get_value();
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    switch (menu) {
        case 'NewTraining':
            AddTraining();
            break;
        case 'NewTrainingCategory':
            AddTrainingFolder(nodeId);
            break;
        case 'Delete':
            DeleteFolderOrTraining(nodeId, typ);
            break;
    }
}

function AddTraining() {
    $('.TrainigCatalogDetail').css('visibility', 'collapse');
    $('.TrainigDetail').css('visibility', 'visible');
    ClearTrainigDetail();
    var title = Translate('newTrainig', 'training');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NameData').set_value(title[0].Text);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NameData').focus();
}

function AddTrainingFolder(parentId) {
    $('.TrainigCatalogDetail').css('visibility', 'visible');
    $('.TrainigDetail').css('visibility', 'collapse');
    ClearTrainingGroupDetail();
    var title = Translate('newGroup', 'training');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupTitleData').set_value(title[0].Text);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupTitleData').focus();
}

function DeleteFolderOrTraining(id, typ) {
    var confirmWindow = radconfirm(Translate('deleteTrainigConfirm', 'training')[0].Text, function (args) {
        if (!args) {
            return;
        }
        else {
            var parameter = {};
            parameter.id = id;
            parameter.typ = typ;
            var detailData = getData(parameter, 'DeleteFolderOrTraining');
            ClearTrainigDetail();
            ClearTrainingGroupDetail();
            trainigTree.get_selectedNode().get_parent().get_nodes().remove(trainigTree.get_selectedNode());
            if (trainingSearchList.findItemByValue(id) !== null) {
                trainingSearchList.get_items().remove(trainingSearchList.findItemByValue(id));
            }
        }
    });
    
}

function CatalogTreeNodeClicked(sender, args) {
    var typ = args.get_node().get_attributes().getAttribute('TYP');
    var id = args.get_node().get_value();
    if (typ === 'GROUP') {
        SetTrainingGroupDetailData(id);
        if (trainingSearchList.get_selectedItem() !== null) {
            trainingSearchList.get_selectedItem().set_selected(false);
        }
    }
    else {
        SetTrainingDetailData(id);
        if (trainingSearchList.findItemByValue(id) !== null) {
            trainingSearchList.findItemByValue(id).set_selected(true);
        }
        else {
            if (trainingSearchList.get_selectedItem() !== null) {
                trainingSearchList.get_selectedItem().set_selected(false);
            }
        }
    }
    
}

function CatalogTreeContextMenuShowing(sender, args) {
    var node = trainigTree.findNodeByValue(args._node._properties._data.value);
    node.select();
    var typ = trainigTree.get_selectedNode().get_attributes().getAttribute('TYP');
    var id = trainigTree.get_selectedNode().get_value();

    if (typ === 'GROUP') {
        SetTrainingGroupDetailData(id)
    }
    else {
        SetTrainingDetailData(id);
    }
    setMenu(node, typ);
    
}

function setMenu(node, typ) {
    switch (typ) {
        case 'ITEM':
            node.get_contextMenu().findItemByValue("NewTraining").hide();
            node.get_contextMenu().findItemByValue("NewTrainingCategory").hide();
            node.get_contextMenu().findItemByValue("Delete").show();
            break;
        case 'GROUP':
            node.get_contextMenu().findItemByValue("NewTraining").show();
            node.get_contextMenu().findItemByValue("NewTrainingCategory").show();
            node.get_contextMenu().findItemByValue("Delete").show();
            if (node.get_level() === 0) {
                node.get_contextMenu().findItemByValue("Delete").hide();
            }
            break;
    }
    
}

function SetTrainingDetailData(id) {
    var parameter = {};
    parameter.id = id;
    detailData = getData(parameter, 'GetTrainingDetailData');

    ClearTrainigDetail();
    $('.TrainigCatalogDetail').css('visibility', 'collapse');
    $('.TrainigDetail').css('visibility', 'visible');

    $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NameData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DescriptionData').set_value(detailData[0].DESCRIPTION);
    if (detailData[0].VALID_FROM != null) {
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidFromData_dateInput').set_selectedDate(new Date(GetDateFromJson(detailData[0].VALID_FROM)));
    }
    if (detailData[0].VALID_TO != null) {
        $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidTo_dateInput').set_selectedDate(new Date(GetDateFromJson(detailData[0].VALID_TO)));
    }
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CostExternData').set_value(detailData[0].COST_EXTERNAL);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CostInternData').set_value(detailData[0].COST_INTERNAL);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_PlaceData').set_value(detailData[0].LOCATION);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NumberParticipantData').set_value(detailData[0].PARTICIPANT_NUMBER);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainerData').set_value(detailData[0].INSTRUCTOR);  
}

function SetTrainingGroupDetailData(id) {
    var parameter = {};
    parameter.id = id;
    detailData = getData(parameter, 'GetTrainingGroupDetailData');

    ClearTrainingGroupDetail();
    $('.TrainigDetail').css('visibility', 'collapse');
    $('.TrainigCatalogDetail').css('visibility', 'visible');

    $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingGroupId')[0].value = detailData[0].ID
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupTitleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupDescriptionData').set_value(detailData[0].DESCRIPTION);
}

function ClearTrainigDetail() {
    $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_DescriptionData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidFromData_dateInput').clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_ValidTo_dateInput').clear();
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CostExternData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_CostInternData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_PlaceData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_NumberParticipantData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainerData').set_value('');
}

function ClearTrainingGroupDetail() {
    $('#ContentPlaceHolder1_LMRORU_Layout_ctl03_TrainingGroupId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupTitleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LMRORU_Layout_ctl03_GroupDescriptionData').set_value('');
}