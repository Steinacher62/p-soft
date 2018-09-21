
var demandListBox;

$(document).ready(function () {
    demandListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_TrainingDemandListBox');
    ClearDetail()
    PaneRUResized();
});

function PaneLResized() {

}

function PaneROResized() {

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_TrainingDemandListBox').css("height", paneRUHeight - 22 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetTrainingDemandList');
    demandListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        demandListBox.get_items().add(item);
    });

}

function AddClick() {
    ClearDetail();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').focus();
}

function DeleteClick() {
    var id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_DemandId')[0].value;
    if (id !== "0") {
        var confirmWindow = radconfirm(Translate('deleteDemandConfirm', 'training')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                var parameter = {};
                parameter.id = id;
                var detailData = getData(parameter, 'DeleteTrainingDemand');
                ClearDetail();
                demandListBox.get_items().remove(demandListBox.findItemByValue(id));
            }
        });
    }
}

function SaveClick() {
    ValidatorEnable('TrainigDemand');
    var isValid = Page_ClientValidate();
    ValidatorDisable('TrainigDemand');
    if (isValid) {
        var parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_DemandId')[0].value;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').get_value();
        parameter.ordnumber = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_OrdnumberData').get_value();
        parameter.mnemo = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_MnemoData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_DescriptionData').get_value();
        detailData = getData(parameter, "SaveDemandDetail");

        var item = new Telerik.Web.UI.RadListBoxItem;
        item.set_text(detailData[0].TITLE);
        item.set_value(detailData[0].ID);
        demandListBox.get_items().add(item);
        demandListBox.findItemByValue(detailData[0].ID).set_selected(true);
    }
}

function TrainingDemandListBoxIndexChanging(sender, args) {
    ClearDetail();
    var parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, "GetDemandDetail");
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_DemandId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_OrdnumberData').set_value(detailData[0].NUMBER);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_MnemoData').set_value(detailData[0].MNEMO);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_DescriptionData').set_value(detailData[0].DESCRIPTION);
}

function ClearDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_DemandId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_OrdnumberData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_MnemoData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_DescriptionData').set_value('');

}