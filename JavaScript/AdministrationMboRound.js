var mboRoundListBox;

$(document).ready(function () {
    mboRoundListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_MboRoundListBox');
    PaneRUResized();
    PaneLResized();
    ClearDetail();
});

function PaneLResized() {

}

function PaneROResized() {

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_MboRoundListBox').css("height", paneRUHeight - 22 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetMboRoundList');
    mboRoundListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        mboRoundListBox.get_items().add(item);
    });
}

function RoundListBoxIndexChanged(sender, args) {
    ClearDetail();
    var parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetRoundDetail');
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_RoundId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_DescriptionData').set_value(detailData[0].DESCRIPTION);
    if (detailData[0].STARTDATE !== null) {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_StartFromData').set_selectedDate(new Date(GetDateFromJson(detailData[0].STARTDATE)));
    }
    if (detailData[0].ENDDATE !== null) {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_EndToData').set_selectedDate(new Date(GetDateFromJson(detailData[0].ENDDATE)));
    }

}

function AddClick() {
    ClearDetail();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').focus();
}

function SaveClick() {
    ValidatorEnable('MboRound');
    var isValid = Page_ClientValidate();
    ValidatorDisable('MboRound');
    if (isValid) {
        var parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_RoundId')[0].value;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_DescriptionData').get_value();
        parameter.start = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_StartFromData').get_selectedDate().format("dd.MM.yyyy");
        parameter.end = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_EndToData').get_selectedDate().format("dd.MM.yyyy");
        detailData = getData(parameter, 'SaveRoundDetail');
        if (parameter.id !== "0") {
            mboRoundListBox.get_selectedItem().set_text(detailData[0].TITLE);
        }
        else {
            var item = new Telerik.Web.UI.RadListBoxItem;
            item.set_text(detailData[0].TITLE);
            item.set_value(detailData[0].ID);
            mboRoundListBox.get_items().add(item);
            mboRoundListBox.findItemByValue(detailData[0].ID).set_selected(true);
        }


    }

}

function DeleteClick() {
    if ($('#ContentPlaceHolder1_LRORU_Layout_ctl00_RoundId')[0].value !== "0") {
        var confirmWindow = radconfirm(Translate('deleteRoundConfirm', 'mbo')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                var parameter = {};
                parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_RoundId')[0].value;
                var detailData = getData(parameter, 'DeleteRound');
                mboRoundListBox.get_items().remove(mboRoundListBox.findItemByValue(parameter.id));
                ClearDetail();
            }
        });
    }
}

function ClearDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_RoundId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_DescriptionData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_StartFromData').clear();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_EndToData').clear();
}