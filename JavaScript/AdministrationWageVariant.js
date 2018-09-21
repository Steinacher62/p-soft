


var variantListBox;

$(document).ready(function () {
    variantListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_VariantListBox');
    ClearDetail();
    PaneRUResized();
});

function PaneLResized() {

}

function PaneROResized() {

}

function AddClick() {
    ClearDetail();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').focus();
}

function SaveClick() {
    ValidatorEnable('WageVariant');
    var isValid = Page_ClientValidate();
    ValidatorDisable('WageVariant');
    if (isValid) {
        var parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_VariantId')[0].value;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').get_value();
        parameter.hauptvariante = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ActiveData').get_checked();
        parameter.fix_point_value = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ValuePointFixData').get_value();
        parameter.austrittbeschraenkung = '';
        if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionToData').get_selectedDate() !== null) {
            parameter.austrittbeschraenkung = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionToData').get_selectedDate().format("dd.MM.yyyy");
        }
        parameter.eintrittbeschraenkung = '';
        if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionFromData').get_selectedDate() !== null) {
            parameter.eintrittbeschraenkung = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionFromData').get_selectedDate().format("dd.MM.yyyy");
        }
        detailData = getData(parameter, 'SaveWageVariant');

        $('#ContentPlaceHolder1_LRORU_Layout_ctl00_VariantId')[0].value = detailData[0].ID;
        if (parameter.id !== "0") {
            variantListBox.get_selectedItem().set_text(detailData[0].TITLE);
        }
        else {
            var item = new Telerik.Web.UI.RadListBoxItem;
            item.set_text(detailData[0].TITLE);
            item.set_value(detailData[0].ID);
            variantListBox.get_items().add(item);
            variantListBox.findItemByValue(detailData[0].ID).set_selected(true);
        }
    }
}

function DeleteClick() {
    var parameter = {};
    parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_VariantId')[0].value;
    var detailData = getData(parameter, 'DeleteWageVariant');
    ClearDetail();
    variantListBox.get_items().remove(variantListBox.findItemByValue(parameter.id));
}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_VariantListBox').css("height", paneRUHeight - 22 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetWageVariantList');
    variantListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        variantListBox.get_items().add(item);
    });
}

function VariantListBoxIndexChanged(sender, args) {
    ClearDetail();
    var parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetWageVariantDetail');

    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_VariantId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value(detailData[0].TITLE);
    if (detailData[0].HAUPTVARIANTE === 1) {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ActiveData').set_checked(true);
    }
    else {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ActiveData').set_checked(false);
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ValuePointFixData').set_value(detailData[0].FIX_POINT_VALUE);
    if (detailData[0].EINTRITTBESCHRAENKUNG != null) {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionFromData').set_selectedDate(new Date(GetDateFromJson(detailData[0].EINTRITTBESCHRAENKUNG)));
    }
    if (detailData[0].AUSTRITTBESCHRAENKUNG != null) {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionToData').set_selectedDate(new Date(GetDateFromJson(detailData[0].AUSTRITTBESCHRAENKUNG)));
    }
}

function ClearDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_VariantId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ActiveData').set_checked(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ValuePointFixData').set_value(0);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionFromData').clear();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_ExclusionToData').clear();
}