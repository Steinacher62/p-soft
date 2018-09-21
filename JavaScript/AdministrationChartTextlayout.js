
var textLayoutListBox;

$(document).ready(function () {
    textLayoutListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_TextLayoutListBox');
    PaneRUResized();
    ClearChartDetail();
});

function PaneROResized() {

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_TextLayoutListBox').css("height", paneRUHeight - 22 + 'px');
}

function PaneLResized() {
    var paneLwidth = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL').css('width').replace('px', '');
    var paneLheight = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL').css('height').replace('px', '');
    $('#imageDiv').css('width', paneLwidth - 3 + 'px');
    $('#imageDiv').css('height', paneLheight - 3 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetTextLayoutList');
    textLayoutListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        textLayoutListBox.get_items().add(item);
    });
    ClearChartDetail();
}

function DeleteClick() {
    if ($('#ContentPlaceHolder1_LRORU_Layout_ctl00_TextLayoutId')[0].value > 0) {
        var confirmWindow = radconfirm(Translate('deleteTextLayoutConfirm', 'charttext')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                var parameter = {};
                parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TextLayoutId')[0].value;
                var detailData = getData(parameter, "DeleteTextLayout");
                textLayoutListBox.get_items().remove(textLayoutListBox.get_selectedItem());
                ClearChartDetail();
            }
        });
    }
}

function AddClick() {

    if (textLayoutListBox.get_selectedItem() !== null) {
        textLayoutListBox.get_selectedItem().set_selected(false);
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').focus();
    ClearChartDetail();
}

function SaveClick() {
    ValidatorEnable('nameTitle');
    var isValid = Page_ClientValidate();
    ValidatorDisable('nameTitle');
    if (isValid) {
        parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TextLayoutId')[0].value;
        parameter.name = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').get_value();
        parameter.align = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_AlignmentData').get_selectedItem().get_value();
        parameter.fontFamily = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontData').get_selectedItem().get_text();
        parameter.fontsize = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontsizeData').get_selectedItem().get_text();
        parameter.bold = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontBoldData').get_checked();
        parameter.italic = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontItalicData').get_checked();
        parameter.color = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontColorData').get_selectedColor();
        detailData = getData(parameter, 'SaveTextLayoutDetail');
        if (parameter.id === "0") {
            $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TextLayoutId')[0].value = detailData[0].ID;
            var item = new Telerik.Web.UI.RadListBoxItem;
            item.set_text(detailData[0].TITLE);
            item.set_value(detailData[0].ID);
            var itemIndex = GetNewItemPositionInItems(textLayoutListBox.get_items(), item.get_text());
            textLayoutListBox.get_items().insert(itemIndex, item);
            textLayoutListBox.findItemByValue(detailData[0].ID).select();
            item.scrollIntoView();
        }
        else {
            textLayoutListBox.get_selectedItem().set_text(detailData[0].TITLE);
        }
    }
}

function TextLayoutListBoxIndexChanging(sender, args) {
    parameter = {};
    parameter.id = args.get_item().get_value();
    var detailData = getData(parameter, 'GetTextlayoutDetail');
    SetDetailData(detailData[0]);
}

function SetDetailData(detailData) {
    ClearChartDetail();
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TextLayoutId')[0].value = detailData.ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value(detailData.TITLE);
    if (detailData.HORIZONTAL_ALIGN === null) {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_AlignmentData').findItemByValue(0).select();
    }
    else {
        $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_AlignmentData').findItemByValue(detailData.HORIZONTAL_ALIGN).select();
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontData').findItemByText(detailData.FONTFAMILY).select();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontsizeData').findItemByValue(detailData.FONTSIZE).select();

    switch (detailData.FONTSTYLE) {
        case (1):
            $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontBoldData').set_checked(true);
            break;
        case (2):
            $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontItalicData').set_checked(true);
            break;
        case (3):
            $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontBoldData').set_checked(true);
            $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontItalicData').set_checked(true);
            break;
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontColorData').set_selectedColor(detailData.FONTCOLOR);
    SetExampleText();
}

function SetExampleText() {
    var fontFamiliy = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontData').get_selectedItem().get_text();
    var fontSize = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontsizeData').get_selectedItem().get_value();
    var fontColor = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontColorData').get_selectedColor();
    var bold = "";
    var italic = "";
    if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontBoldData').get_checked()) {
        bold = 'bold';
    }
    if ($find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontItalicData').get_checked()) {
        italic = 'italic';
    }

    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontExampleData').css({ 'font-family': fontFamiliy, 'font-size': fontSize +'px', 'color': fontColor, 'font-weight': bold, 'font-style': italic});
}

function ClearChartDetail() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_TextLayoutId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontBoldData').set_checked(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontItalicData').set_checked(false);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_FontColorData').set_selectedColor('#000000');
}

function FontsizeDataIndexChanged(sender, args) {
    SetExampleText();
}

function FontDataIndexChanged(sender, args) {
    SetExampleText();
}

function FontBoldDataCheckedChanged(sender, args) {
    SetExampleText();
}

function FontItalicDatCheckedChanged(sender, args) {
    SetExampleText();
}

function FontColorDataColorChange(sender, args) {
    SetExampleText();
}

   