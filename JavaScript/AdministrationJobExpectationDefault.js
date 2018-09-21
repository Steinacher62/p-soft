

var ratingItemListBox;

$(document).ready(function () {
    ratingItemListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_PerformanceratingItemListBox');
    PaneRUResized();
    ClearDetail();
}); 

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_PerformanceratingItemListBox').css("height", paneRUHeight - 10 + 'px');
}

function PaneLResized() {

}

function PaneROResized() {

}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetRatingItemList');
    ratingItemListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        ratingItemListBox.get_items().add(item);
    });
}

function PerformanceratingItemListBoxIndexChanged(sender, args) {
    parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetJobExpectationDefaultDetail');
    SetDetailData(detailData);
}

function SetDetailData(detailData) {
    ClearDetail();
    if (detailData.length > 0) {
        $('#ContentPlaceHolder1_LRORU_Layout_ctl00_DefaultJobexpectationId')[0].value = detailData[0].CRITERIA_REF;
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value(detailData[0].TITLE);
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value(detailData[0].DESCRIPTION);
    }
}

function SaveClick() {
    parameter = {};
    parameter.id = parseInt($('#ContentPlaceHolder1_LRORU_Layout_ctl00_DefaultJobexpectationId')[0].value);
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').get_value();
    parameter.description = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').get_value();
    getData(parameter, 'SavejobExpectationDefault');

}

function DeleteClick() {
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value('');
    SaveClick();
}

function ClearDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_DefaultJobexpectationId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value('');
}