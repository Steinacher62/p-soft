/// <reference path="../JavaScript/colorMenu.js" />

/*script to calculate averages*/

$(document).ready(function () {
    calculateAverage();
    $(".Radiobutton input").change(function(){
        calculateAverage();
       
    });
    $(".InfoIcon").mouseover(function(){
        $(".descDiv").html($(this).attr("desc").replace(/-/g, "</br> -").replace("</br>",""));
        

        //set the height, top position
        var height = $(this).height();
        var top = $(this).offset().top + 10;


        //set  the left position
        var left = $(this).offset().left + 10;

        $('.descDiv').css({
            'left': left,
            'top': top
        });


        $('.descDiv').show();
    });
    $(".InfoIcon").mouseleave(function () {
        $('.descDiv').hide();
    });
    $('.descDiv').hide();


    
});

function calculateAverage() {
    $(".Active").text("");

    $(".Active").removeClass("Active");
    var tables = $(".criteriaTable");
    for (var i = 0 ; i < tables.length - 1 ; i++) {
        var table = tables[i];
        var rows = $(table).children().children("tr");
        var sum = 0;
        for (var rownumber = 1 ; rownumber < rows.length; rownumber++) {
            var radioboxes = $(rows[rownumber]).children("td").children("span").children("input");
            var selectedValue = $(radioboxes).filter(":checked").parent().parent().index() - 2;
            if (selectedValue >= 0) {
                sum += selectedValue;
            } else {
                sum = -1;
                break;
            }
        }
        if (sum != -1) {
            sum = Math.round(sum / (rows.length - 1));
            $($(rows[0]).children(".TotalBoxCriteria")[sum]).addClass("Active");
            $($(rows[0]).children(".TotalBoxCriteria")[sum]).text(new Array("E", "D", "C", "B", "A")[sum]);
        }

    }

    if ($(".Active").length == $(".criteriaTable").length - 1) {
        var totalSum = 0;
        for (var i = 0 ; i < tables.length - 1 ; i++) {
            var row = $(tables[i]).children().children("tr:first-child");
            var newValue = $(row).children(".Active").index();
            if ($(".criteriaTable").length - 1 == 3) {
                switch (i) {
                    case 0:
                        newValue *= 0.4;
                        break;
                    case 1:
                        newValue *= 0.3;
                        break;
                    case 2:
                        newValue *= 0.3;
                        break;
                    default:
                        break;

                }

            } else {
                switch (i) {
                    case 0:
                        newValue *= 0.3;
                        break;
                    case 1:
                        newValue *= 0.2;
                        break;
                    case 2:
                        newValue *= 0.2;
                        break;
                    case 3:
                        newValue *= 0.3;
                        break;
                    default:
                        break;
                }
            }
            totalSum += newValue;
        }

        totalSum = Math.round(totalSum);
        $($(tables[tables.length - 1]).children().children().children()[totalSum]).addClass("Active").text(new Array("E", "D", "C", "B", "A")[totalSum - 2]);
    }


}