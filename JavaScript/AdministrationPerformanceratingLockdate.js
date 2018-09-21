
$(document).ready(function () {
    setLockdate();
});

function setLockdate() {
    parameter = {};
    detailDat = getData(parameter, 'GetLockdate');
    GetDateDEFromJSON(detailDat[0].DATUM_WERT);
    if (detailDat[0].DATUM_WERT !== null) {
        $find("ctl00_ContentPlaceHolder1_L_Layout_ctl00_lockDateData").set_selectedDate(new Date(GetDateFromJson(detailDat[0].DATUM_WERT)));
    }
}

function SaveLockdateClicking() {
    parameter = {};
    if ($find("ctl00_ContentPlaceHolder1_L_Layout_ctl00_lockDateData").get_selectedDate() !== null) {
        parameter.lockdate = $find("ctl00_ContentPlaceHolder1_L_Layout_ctl00_lockDateData").get_selectedDate().format("dd.MM.yyyy");
    }
    else {
        parameter.lockdate = null;
    }
    getData(parameter, 'SaveLockdate');

}