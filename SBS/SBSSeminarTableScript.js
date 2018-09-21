/// <reference path="../Scripts/jquery-3.3.1.min.js" />
/// <reference path="../Javascript/jquery.json-2.2.min.js" />

$(document).ready(function () {
   
    $('.ClickableFolder').click(function () {
        $(".SubDocs").hide();
        $(".DocumentRow").hide().insertAfter($(this.parentElement)).slideDown('400', function () {
            if (!isScrolledIntoView($('.DocumentRow')[0])) {
                $('.DocumentRow')[0].scrollIntoView(false);
            }
        });
        if (!$(this).hasClass("DescriptionActive")) {
            $(".Description").removeClass("Active");
            $(".DescriptionActive").removeClass("DescriptionActive");
        }
        $('.ClickableFolder').removeClass("ActiveFolder");
            
        $(this).addClass("ActiveFolder");
        var id = $(this).attr("id").split("FolderCell")[1];
        $(".DocumentsList li").addClass("HiddenDocument");
        $(".DocumentsList li.FolderEntry" + id).removeClass("HiddenDocument");
        
    });
    $('.SubFolder').click(function () { openSubfolder(this)});
    $('.ClickableDescription').click(function () { showDescription(this) });
});

function showDescription(cell) {
   
    if ($(cell).hasClass("DescriptionActive")) {
        $(".Description").removeClass("Active");
        $(".DescriptionActive").removeClass("DescriptionActive");
    } else {
        $(".Description").removeClass("Active");
        $(".DescriptionActive").removeClass("DescriptionActive");
        var id = $(cell).attr("id");
        var descCell = $("#description" + id);
        var top = 0;
        var left = 0;
        if (id.search("Document") == 0) {
            top = $(cell).offset().top + $(cell).height()+2;
             left = $(cell).offset().left + $(cell).width();
        } else {
             top = $(cell).offset().top;
             left = $(cell).offset().left + $(cell).width()+10;
        }
        $(descCell).css({
            'left': left,
            'top': top
        });
        $(descCell).addClass("Active");
        $(cell).addClass("DescriptionActive");
    }
}

function isScrolledIntoView(elem) {
    var $elem = $(elem);
    var $window = $(window);

    var docViewTop = $window.scrollTop();
    var docViewBottom = docViewTop + $window.height();

    var elemTop = $elem.offset().top;
    var elemBottom = elemTop + $elem.height();

    return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
}

function openSubfolder(button) {
    $(".Description").removeClass("Active");
    
    if ($("#subDocs_" + $(button).attr('id').split("_")[1]).is(":visible")) {
        $(".SubDocs").hide();
    } else {
        $(".SubDocs").hide();
        $("#subDocs_" + $(button).attr('id').split("_")[1]).show();
    }
}