var functionTypList;

$(document).ready(function () {
    functionTypList = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionsTypBox');
    PaneRUResized();
    ClearDetail();
});

function FunctionTypSearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetFunctionTypList');
    functionTypList.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.BEZEICHNUNG);
        functionTypList.get_items().add(item);
    });

}

function SaveFunctionTypClicking() {
    ValidatorEnable('FunctionTypTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('FunctionTypTable');
    if (isValid) {
        parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value;
        parameter.bezeichnung = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').get_value();
        parameter.beschreibung = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').get_value();
        detailDat = getData(parameter, 'SaveUpdateFunctionTyp');
        if ($('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value === '0') {
            var item = new Telerik.Web.UI.RadListBoxItem();
            item.set_value(detailDat[0].ID);
            item.set_text(detailDat[0].BEZEICHNUNG);
            functionTypList.get_items().add(item);
            functionTypList.findItemByValue(detailDat[0].ID).set_selected(true);
        }
    }
}

function AddFunctionTypClick() {
    ClearDetail();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').focus(true);
}

function DeleteFunctionTypClicking() {
    if ($('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value !== '0') {
        parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value;
        getData(parameter, 'DeleteFunctionTyp');
        functionTypList.get_items().remove(functionTypList.findItemByValue($('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value));
        ClearDetail();
    }
}

function FunctionTypListBoxIndexChanged(sender, args) {
    SetFunctionTypDetail(args.get_item().get_value());
}

function SetFunctionTypDetail(typId) {
    parameter = {};
    parameter.id = typId;
    detailDat = getData(parameter, 'GetFunctionTypDetail');
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value = detailDat[0].ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value(detailDat[0].BEZEICHNUNG);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value(detailDat[0].BESCHREIBUNG);

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionsTypBox').css("height", paneRUHeight - 10 + 'px');
}

function PaneLResized() {

}

function PaneROResized() {

}

function ClearDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TypId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value('');
}