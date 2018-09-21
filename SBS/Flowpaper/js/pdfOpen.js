$(document).ready(function () {
    jQuery.get((!window.isTouchScreen) ? 'UI_flowpaper_desktop_flat.html' : 'UI_flowpaper_mobile_flat.html',
               function (toolbarData) {
                   $('#documentViewer').FlowPaperViewer(
                           {
                               config: {

                                   //SWFFile: "../" + window.location.search.substring(1).split("pdfPath=")[1].replace('.pdf', '.swf'),
                                   IMGFiles: "../" + window.location.search.substring(1).split("pdfPath=")[1] .replace('.pdf','_{page}.png'),
                                   JSONFile: "../" + window.location.search.substring(1).split("pdfPath=")[1].replace('.pdf', '.js'),
                                   PDFFile: "../" + window.location.search.substring(1).split("pdfPath=")[1],
                                   key: "@0a4d3572058d36ff11c$5057b73be3ce426a57a",
                                   Scale: 1,
                                   ZoomTransition: 'easeOut',
                                   ZoomTime: 0.5,
                                   ZoomInterval: 0.2,
                                   FitPageOnLoad: true,
                                   FitWidthOnLoad: false,
                                   FullScreenAsMaxWindow: false,
                                   ProgressiveLoading: true,
                                   MinZoomSize: 0.2,
                                   MaxZoomSize: 5,
                                   SearchMatchAll: false,
                                   Toolbar: toolbarData,
                                   BottomToolbar: 'UI_flowpaper_annotations.html',
                                   InitViewMode: 'Portrait',
                                   RenderingOrder: 'html5,html',
                                   StartAtPage: '',

                                   PrintPaperAsBitmap:false,
                                   ViewModeToolsVisible: true,
                                   ZoomToolsVisible: true,
                                   NavToolsVisible: true,
                                   CursorToolsVisible: true,
                                   SearchToolsVisible: true,
                                   
                                   localeDirectory: 'locale/',
                                   cssDirectory: 'css/',
                                   localeChain: 'de_DE',
                                   jsDirectory: 'js/'
                               }
                           }
                   );
               });
});

function MarkCreated(mark) {
    var documentViewer = window.getDocViewer("documentViewer");

    var params = mark;
    params.pdfPath = window.location.search.substring(1).split("pdfPath=")[1];
    var formData = $.toJSON(params);
    var params2 = {};
    params2.mark = formData;
    formData = $.toJSON(params2);
    $.ajax({
        type: "POST",
        url: "../../WebService/SBSService.asmx/CreateMark",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {

        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });
}

function MarkDeleted(mark) {
    var documentViewer = window.getDocViewer("documentViewer");

    var params = {};
    params.id = mark.id;
    var formData = $.toJSON(params);

    $.ajax({
        type: "POST",
        url: "../../WebService/SBSService.asmx/DeleteMark",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {

        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });

}

function MarkChanged(mark) {

   
    var documentViewer = window.getDocViewer("documentViewer");

    var params = mark;
    params.pdfPath = window.location.search.substring(1).split("pdfPath=")[1];
    var formData = $.toJSON(params);
    var params2 = {};
    params2.mark = formData;
    formData = $.toJSON(params2);

    $.ajax({
        type: "POST",
        url: "../../WebService/SBSService.asmx/ChangeMark",
        cache: false,
        async: false,
        dataType: 'json',
        data: formData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data, TextStatus, XHR) {

        },
        error: function (XHR, TextDescription, ErrorThrown) {
            var responseText = JSON.parse(XHR.responseText);

        }
    });
}

