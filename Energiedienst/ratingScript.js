/// <reference path="../JavaScript/colorMenu.js" />

$(document).ready(function () {
    $(".RatingBox").focusout(function () {
        calculateAverage();
    });
    $(".WeightBox").focusout(function () {
        calculateSum();
    });
    calculateAverage();
    calculateSum();
    var textareas = $("textarea");
    for (var i = 0 ; i < textareas.length; i++) {
        if (textareas[i].scrollHeight > 40) {
            textareas[i].style.height = (textareas[i].scrollHeight) + "px";
        } else {
            textareas[i].style.height = "100%";
        }
    }
    equalTextBoxHeight()
    $("textarea").keyup(function () {
        var textareas = $("textarea");
        for (var i = 0 ; i < textareas.length; i++) {
            textareas[i].style.height = "100%";
            if (textareas[i].scrollHeight > 40) {
                textareas[i].style.height = (textareas[i].scrollHeight-2) + "px";
            } else {
                textareas[i].style.height = "100%";
            }
        }
        equalTextBoxHeight();
    });
});
function equalTextBoxHeight() {
    var rows = $("#objectiveTable tr");
    for (var i = 0 ; i < rows.length; i++) {
        var max = 0;
        var cells = $(rows[i]).children("td").children("textarea");
        for (var j = 0; j < cells.length; j++) {
            if (max < $(cells[j]).height()) {
                max = $(cells[j]).height();
            }
        }
        for (var j = 0; j < cells.length; j++) {
            cells[j].style.height= max+"px";
        }
    }
}
function calculateAverage() {
    var average = 0;
    var ratingBoxes = $(".RatingBox");
    var weightBoxes = $(".WeightBox");
    for (var i = 0; i < 5; i++) {
        if (ratingBoxes.length > 0 && !isNaN(Number(ratingBoxes[i].value)) && ratingBoxes[i].value <= 120 && ratingBoxes[i].value >= 0) {
            if (!isNaN(Number(weightBoxes[i].value))) {
                if (average != "Fehler") {
                    average += (Number(ratingBoxes[i].value) * Number(weightBoxes[i].value)) / 100;
                }
                $(weightBoxes[i]).removeClass("HasMistake");
                $(ratingBoxes[i]).removeClass("HasMistake");
            } else {
                $(weightBoxes[i]).addClass("HasMistake");
                average = "Fehler";
            }
        } else {
            $(ratingBoxes[i]).addClass("HasMistake");
            average = "Fehler";          
        }
    }
    if (average == "Fehler") {
        $("#ratingSum").addClass("HasMistake");
    } else {
        $("#ratingSum").removeClass("HasMistake");
        average = Math.round(average);
    }
    $("#ratingSum").text(average);

}

function calculateSum() {
    var sum = 0;
    var weightBoxes = $(".WeightBox");
    for (var i = 0; i < 5; i++) {
        if (weightBoxes.length > 0 && !isNaN(Number(weightBoxes[i].value))) {
            if (sum != "Fehler") {
                sum += Number(weightBoxes[i].value);
            }
            $(weightBoxes[i]).removeClass("HasMistake");
          } else {
                $(weightBoxes[i]).addClass("HasMistake");
                sum = "Fehler";
                
          }      
    }
    if (sum == "Fehler" || (sum != 100 && sum != 0)) {
        $("#weightSum").addClass("HasMistake");
    } else {
        $("#weightSum").removeClass("HasMistake");
    }
    $("#weightSum").text(sum);
    calculateAverage();
}