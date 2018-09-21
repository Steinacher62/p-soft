var competenceList;

$(document).ready(function () {
    competenceList = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_CompetenceListBox');
    PaneRUResized();
    ClearSearchData();
    ClearDetailData();
});

function PaneLResized() {

}

function PaneROResized() {

}

function PaneRUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LRORU_Layout_PaneRU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LRORU_Layout_ctl02_CompetenceListBox').css("height", paneRUHeight + 'px');

}

function CompetenceSearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBName').get_value();
    parameter.titleShort = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl01_TBNameShort').get_value();
    competenceList.get_items().clear();
    detailData = getData(parameter, 'GetCompetenceList');
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        competenceList.get_items().add(item);
    });

}

function CompetenceListBoxIndexChanged(sender, args) {
    parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetCompetenceLevelDetail');
    ClearDetailData();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_numberData').set_value(detailData[0].NUMBER);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleShortData').set_value(detailData[0].MNEMO);
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').set_value(detailData[0].DESCRIPTION);
}

function SaveCompetenceClick() {
    ValidatorEnable('CompetenceTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('CompetenceTable');
    if (isValid) {
        parameter = {};
        if (competenceList.get_selectedItem() === null) {
            parameter.id = 0;
        }
        else {
            parameter.id = competenceList.get_selectedItem().get_value();
        }
        parameter.number = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_numberData').get_value();
        parameter.title = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleData').get_value();
        parameter.mnemo = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_titleShortData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_descriptionData').get_value();

        getData(parameter, 'SaveCompetenceLevelDetail');
        CompetenceSearchClick();
    }
}

function DeleteCompetenceClick() {
    if (competenceList.get_selectedItem() !== null) {
        var confirmWindow = radconfirm(Translate('DeleteCompetenceLevelConfirm', 'competences')[0].Text, function (args) {
            if (!args) {
                return;
            }
            else {
                confirmWindow.Close();
                parameter = {};
                parameter.id = competenceList.get_selectedItem().get_value();
                getData(parameter, 'DeleteCompetenceLevel');
                ClearDetailData();
                CompetenceSearchClick();
            }
        });
    }
}

function AddCompetenceClick() {
    if (competenceList.get_selectedItem() !== null) {
        competenceList.get_selectedItem().set_selected(false);
    }
    ClearDetailData();
    $find('ctl00_ContentPlaceHolder1_LRORU_Layout_ctl00_numberData').focus();
}

function ClearDetailData() {
    $('.CompetenceTable .Textbox').each(function () { $(this)[0].value = ''; });
    $('.CompetenceTable .TextboxMultiLine').each(function () { $(this)[0].value = ''; });
}

function ClearSearchData() {
    $('.SearchTable .Textbox').each(function () { $(this)[0].value = ''; });
}