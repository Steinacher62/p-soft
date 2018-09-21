
var orgentityLayoutListBox;

$(document).ready(function () {
    orgentityLayoutListBox = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_OrgentityLayoutListBox');
    PaneRUResized();
    ClearDetailData();
});

function PaneROResized() {

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_OrgentityLayoutListBox').css("height", paneRUHeight - 22 + 'px');
}

function PaneLResized() {
    var paneLwidth = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL').css('width').replace('px', '');
    var paneLheight = $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneL').css('height').replace('px', '');
    $('#imageDiv').css('width', paneLwidth - 3 + 'px');
    $('#imageDiv').css('height', paneLheight - 55 + 'px');
}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    detailData = getData(parameter, 'GetOrgentityLayoutList');
    orgentityLayoutListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        orgentityLayoutListBox.get_items().add(item);
    });
}

function OrgentityLayoutListBoxIndexChanging(sender, args) {
    var parameter = {};
    parameter.id = args.get_item().get_value();
    var detailData = getData(parameter, 'GetOrgentityLayoutDetail');
    SetDetaiData(detailData[0]);
}

function SetDetaiData(detailData) {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OrgentityLayoutId')[0].value = detailData.ID;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value(detailData.TITLE);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PictureData').set_value(detailData.IMAGE);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_WidthData').set_value(detailData.NODEWIDTH);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_HeightData').set_value(detailData.NODEHEIGHT);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingTopData').set_value(detailData.PADDING_TOP);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingLeftData').set_value(detailData.PADDING_LEFT);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingRightData').set_value(detailData.PADDING_RIGHT);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_LineWidthData').set_value(detailData.LINEWIDTH);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_LineColorData').set_selectedColor(detailData.LINECOLOR);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_BackgroundColorData').set_selectedColor(detailData.BACKGROUNDCOLOR);
}

function SaveClick() {
    ValidatorEnable('nameTitle');
    var isValid = Page_ClientValidate();
    ValidatorDisable('nameTitle');
    if (isValid) {
        var parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OrgentityLayoutId')[0].value;
        parameter.name = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').get_value();
        parameter.picture = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PictureData').get_value();
        parameter.width = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_WidthData').get_value();
        parameter.height = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_HeightData').get_value();
        parameter.paddingTop = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingTopData').get_value();
        parameter.paddingLeft = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingLeftData').get_value();
        parameter.paddingRight = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingRightData').get_value();
        parameter.lineWidth = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_LineWidthData').get_value();
        parameter.lineColor = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_LineColorData').get_selectedColor();
        parameter.backgroundcolor = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_BackgroundColorData').get_selectedColor();
        var detailData = getData(parameter, 'SaveNodeLayout');

        if (parameter.id === "0") {
            $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OrgentityLayoutId')[0].value = detailData[0].ID;
            var item = new Telerik.Web.UI.RadListBoxItem;
            item.set_text(detailData[0].TITLE);
            item.set_value(detailData[0].ID);
            var itemIndex = GetNewItemPositionInItems(orgentityLayoutListBox.get_items(), item.get_text());
            orgentityLayoutListBox.get_items().insert(itemIndex, item);
            orgentityLayoutListBox.findItemByValue(detailData[0].ID).select();
            item.scrollIntoView();
        }
        else {
            orgentityLayoutListBox.get_selectedItem().set_text(detailData[0].TITLE);
        }
    }
}

function DeleteClick() {
    if ($('#ContentPlaceHolder1_LRORU_Layout_ctl00_OrgentityLayoutId')[0].value > 0) {
        var confirmWindow = radconfirm(Translate('deleteConfirm', 'chartnodelayout')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                var parameter = {};
                parameter.id = $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OrgentityLayoutId')[0].value;
                var detailData = getData(parameter, "DeleteNodeLayout");
                orgentityLayoutListBox.get_items().remove(orgentityLayoutListBox.get_selectedItem());
                ClearDetailData();
            }
        });
    }
}

function AddClick() {
    if (orgentityLayoutListBox.get_selectedItem() !== null) {
        orgentityLayoutListBox.get_selectedItem().set_selected(false);
    }
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').focus();
    ClearDetailData();
}

function ClearDetailData() {
    $('#ContentPlaceHolder1_LRORU_Layout_ctl00_OrgentityLayoutId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_NameData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PictureData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_WidthData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_HeightData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingTopData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingLeftData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_PaddingRightData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_LineWidthData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_LineColorData').set_selectedColor('#000000');
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_BackgroundColorData').set_selectedColor('#FFFFFF');
}