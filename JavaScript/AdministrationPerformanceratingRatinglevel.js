
var ratingLevelListBox;

$(document).ready(function () { 
    ratingLevelListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_RatingLevelListBox');
    PaneRUResized();
    ClearDetail();
});

function RatingLevelSearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetRatingLevelList');
    ratingLevelListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        ratingLevelListBox.get_items().add(item);
    });

}

function RatingLevelListBoxIndexChanged(sender, args) {
    parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetRatingLevelDetail');
    SetDetailData(detailData);
    
}

function SetDetailData(detailData) {
    ClearDetail();
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_LevelId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value(detailData[0].DESCRIPTION);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_weightData').set_value(detailData[0].RELATIV_WEIGHT);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_vaidData').get_items().getItem(detailData[0].VALID).select();
}

function SaveClick() {
    ValidatorEnable('RatingLevelTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('RatingLevelTable');
    if (isValid) {
        parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_LevelId')[0].value;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').get_value();
        parameter.relativeWeight = parseFloat($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_weightData').get_value());
        parameter.valid = parseInt($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_vaidData').get_selectedItem().get_value()) - 1;
        if (parameter.valid === -1){
            parameter.valid = 0;
        }
        detailData = getData(parameter, 'SaveRatinglevel');
        if (parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_LevelId')[0].value === '0') {
            var item = new Telerik.Web.UI.RadListBoxItem();
            item.set_value(detailData[0].ID);
            item.set_text(detailData[0].TITLE);
            ratingLevelListBox.get_items().add(item);
        }
        else {
            ratingLevelListBox.get_selectedItem().set_text(detailData[0].TITLE);
        }
        SetDetailData(detailData);
    }

}

function DeleteClick() {
    if (ratingLevelListBox.get_selectedItem() !== null) {
        var confirmWindow = radconfirm(Translate('deleteRatingLevelConfirm', 'performanceRating')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                confirmWindow.Close();
                parameter = {};
                parameter.id = ratingLevelListBox.get_selectedItem().get_value();
                getData(parameter, 'DeleteRatingLevel');
                ClearDetail();
                RatingLevelSearchClick();
            }
        });
    }
}

function AddClick() {
    if (ratingLevelListBox.get_selectedItem() !== null) {
        ratingLevelListBox.get_selectedItem().set_selected(false);
    }
    ClearDetail();
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_LevelId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').focus();

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_RatingLevelListBox').css("height", paneRUHeight - 10 + 'px');
}

function PaneLResized() {

}

function PaneROResized() {

}

function ClearDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_LevelId')[0].value = '';
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_weightData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_vaidData').get_items().getItem(0).select();
    
}