var catalogOptionList;

$(document).ready(function () {
    catalogOptionList = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionsCatalogOptionListBox');
    PaneRUResized();
    ClearDetailData();
});

function PaneLResized() {

}

function PaneROResized() {

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_FunctionsCatalogOptionListBox').css("height", paneRUHeight + 'px');

}

function FunctionCatalogOptionsSearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    catalogOptionList.get_items().clear();
    detailData = getData(parameter, 'GetOptionList');
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.BEZEICHNUNG);
        catalogOptionList.get_items().add(item);
    });
}

function FunctionCatalogOptionListBoxIndexChanged(sender, args) {
    ClearDetailData();
    parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetCatalogOptionDetail');
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OptionId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_averageTypData').get_items().getItem(detailData[0].FBW_MITTELUNGSART_FLAG).select();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_maxValueData').set_value(detailData[0].FBW_PUNKTMAXIMUM);

}

function SaveFunctionCatalogOptionClicking() {
    if ($('#ContentPlaceHolder1_LRORU_Layout_ctl00_OptionId')[0].value > "0") {
        parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OptionId')[0].value;
        parameter.averageTyp = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_averageTypData').get_selectedItem().get_value();
        parameter.maxValue = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_maxValueData').get_value();
        detailDat = getData(parameter, 'SaveCatalogOption');
    }
}

function ClearDetailData() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OptionId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_averageTypData').get_items().getItem(0).select();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_maxValueData').set_value(0);
}