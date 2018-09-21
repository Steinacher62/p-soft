
var criteriaListBox;
var criteriaListBoxLink;
var functionCriteriaListBox;

$(document).ready(function () {
    criteriaListBox = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_CriteriaListBox');
    criteriaListBoxLink = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_CriteriaListBoxLink');
    functionCriteriaListBox = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl01_FunctionCriteriaListBox');
    $('#RAD_SPLITTER_PANE_CONTENT_ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_PaneLO').css("height", "350px");
    PaneMUResized();
    ClearDetail();
}); 

function PaneMUResized() {
    var paneRUHeight = $("#RAD_SPLITTER_PANE_TR_ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_PaneMU").css("height").replace("px", "");
    $('#ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl03_CriteriaListBox').css("height", paneRUHeight - 10 + 'px');
}

function PaneLUResized() {

}

function PaneROResized() {

}

function SaveClick() {
    ValidatorEnable('CriteriaDetailTable');
    isValid = Page_ClientValidate();
    ValidatorDisable('CriteriaDetailTable');
    if (isValid) {
        parameter = {};
        parameter.id = $('#ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_CriteriaId')[0].value;
        parameter.title = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_titleData').get_value();
        parameter.description = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_descriptionData').get_value();
        parameter.functionRatingLinkId = parseInt($find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_FunctionRatingLinkDD').get_selectedItem().get_value());

        detailData = getData(parameter, 'SavePerformanceratingItem');

        criteriaListBox.get_selectedItem().set_text(detailData[0].TITLE);
        SetDetaildata(detailData);
    }
}

function DeleteClick() {

}

function SearchClick() {
    parameter = {};
    parameter.title = $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl02_TBName').get_value();
    detailData = getData(parameter, 'GetRatingItemList');
    criteriaListBox.get_items().clear();
    detailData.forEach(function (itemVal) {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_value(itemVal.ID);
        item.set_text(itemVal.TITLE);
        criteriaListBox.get_items().add(item);
    });
}

function CriteriaItemListBoxIndexChanged(sender, args) {
    parameter = {};
    parameter.id = args.get_item().get_value();
    detailData = getData(parameter, 'GetRatingCriteriaDetail');
    SetDetaildata(detailData);
    
}

function SetDetaildata(detailData) {
    ClearDetail();
    $('#ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_CriteriaId')[0].value = detailData[0].ID;
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_titleData').set_value(detailData[0].TITLE);
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_descriptionData').set_value(detailData[0].DESCRIPTION);
    if (detailData[0].FBW_KRITERIUM !== null) {
        $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_FunctionRatingLinkDD').findItemByValue(detailData[0].FBW_KRITERIUM).select();
        functionCriteriaListBox.findItemByValue(detailData[0].FBW_KRITERIUM).set_selected(true);
    }
    else {
        $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_FunctionRatingLinkDD').findItemByValue(0).select();
    }
    criteriaListBoxLink.findItemByValue(criteriaListBox.get_selectedItem().get_value()).set_selected(true);

    
    
}

function ClearDetail() {
    $('#ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_CriteriaId')[0].value = 0;
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_titleData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_descriptionData').set_value('');
    $find('ctl00_ContentPlaceHolder1_LOLUMOMU_Layout_ctl00_FunctionRatingLinkDD').findItemByValue(0).select();
    if (criteriaListBoxLink.get_selectedItem() !== null) {
        criteriaListBoxLink.get_selectedItem().set_selected(false);
    }
    if (functionCriteriaListBox.get_selectedItem() !== null){
        functionCriteriaListBox.get_selectedItem().set_selected(false);
    }
    
}