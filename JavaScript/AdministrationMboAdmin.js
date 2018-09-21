
$(document).ready(function () {
    SetDetailData();
});


function SetDetailData() {
    var parameter = {};
    var detailData = getData(parameter, 'GetDetailDataConfiguration');
    if (detailData[3].WERT == '5') {
        $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_ObjectiveFilterData').findItemByValue("0").select();
    }
    else {
        $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_ObjectiveFilterData').findItemByValue("1").select();
    }
    $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_ObjectiveRoundData').findItemByValue(detailData[0].WERT).select();
    $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_StartFromData').set_selectedDate(new Date(detailData[1].WERT));
    $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_EndToData').set_selectedDate(new Date(detailData[2].WERT));
}

function SaveClick() {
    var parameter = {};
    parameter.turn = $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_ObjectiveRoundData').get_selectedItem().get_value();
    parameter.validation_from = $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_StartFromData').get_selectedDate().format("dd.MM.yyyy");
    parameter.validation_to = $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_EndToData').get_selectedDate().format("dd.MM.yyyy");
    parameter.objectiveFilter = $find('ctl00_ContentPlaceHolder1_L_Layout_ctl00_ObjectiveFilterData').get_selectedItem().get_value();
    var detailData = getData(parameter, 'SaveDetailDataConfiguration');

}