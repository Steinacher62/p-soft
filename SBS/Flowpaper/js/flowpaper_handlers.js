function DocumentLoaded() {
    var params = {};
    params.pdfPath = window.location.search.substring(1).split("pdfPath=")[1];
    $.getScript("js/jquery.json-2.2.min.js").done(function (script, textStatus) {
        var formData = $.toJSON(params);

        $.ajax({
            type: "POST",
            url: "../../WebService/SBSService.asmx/GetMarks",
            cache: false,
            async: false,
            dataType: 'json',
            data: formData,
            contentType: 'application/json; charset=utf-8',
            success: function (Data, TextStatus, XHR) {
                var documentViewer = window.getDocViewer("documentViewer");
                documentViewer.addMarks(JSON.parse(Data.d));

            },
            error: function (XHR, TextDescription, ErrorThrown) {
                var responseText = JSON.parse(XHR.responseText);

            }
        });
    })
   
}

jQuery(function() {
    /**
     * Handles the event of external links getting clicked in the document.
     *
     * @example onExternalLinkClicked("http://www.google.com")
     *
     * @param String link
     */
    jQuery('#documentViewer').bind('onExternalLinkClicked',function(e,link){

        window.open(link,'_flowpaper_exturl');
    });

    /**
     * Recieves progress information about the document being loaded
     *
     * @example onProgress( 100,10000 );
     *
     * @param int loaded
     * @param int total
     */
    jQuery('#documentViewer').bind('onProgress',function(e,loadedBytes,totalBytes){

    });

    /**
     * Handles the event of a document is in progress of loading
     *
     */
    jQuery('#documentViewer').bind('onDocumentLoading',function(e){

    });

    /**
     * Handles the event of a document is in progress of loading
     *
     */
    jQuery('#documentViewer').bind('onPageLoading',function(e,pageNumber){
    });

    /**
     * Receives messages about the current page being changed
     *
     * @example onCurrentPageChanged( 10 );
     *
     * @param int pagenum
     */
    jQuery('#documentViewer').bind('onCurrentPageChanged',function(e,pagenum){

    });

    /**
     * Receives messages about the document being loaded
     *
     * @example onDocumentLoaded( 20 );
     *
     * @param int totalPages
     */
    jQuery('#documentViewer').bind('onDocumentLoaded',function(e,totalPages){
        DocumentLoaded();
    });

    /**
     * Receives messages about the page loaded
     *
     * @example onPageLoaded( 1 );
     *
     * @param int pageNumber
     */
    jQuery('#documentViewer').bind('onPageLoaded',function(e,pageNumber){
    });

    /**
     * Receives messages about the page loaded
     *
     * @example onErrorLoadingPage( 1 );
     *
     * @param int pageNumber
     */
    jQuery('#documentViewer').bind('onErrorLoadingPage',function(e,pageNumber){

    });

    /**
     * Receives error messages when a document is not loading properly
     *
     * @example onDocumentLoadedError( "Network error" );
     *
     * @param String errorMessage
     */
    jQuery('#documentViewer').bind('onDocumentLoadedError',function(e,errMessage){

    });

    /**
     * Receives error messages when a document has finished printed
     *
     * @example onDocumentPrinted();
     *
     */
    jQuery('#documentViewer').bind('onDocumentPrinted',function(e){

    });

    /**
     * Handles the event of annotations getting clicked.
     *
     * @example onMarkClicked(object)
     *
     * @param Object mark that was clicked
     */
    jQuery('#documentViewer').bind('onMarkClicked',function(e,mark){

    });

    /**
     * Handles the event of annotations getting clicked.
     *
     * @example onMarkCreated(object)
     *
     * @param Object mark that was created
     */
    jQuery('#documentViewer').bind('onMarkCreated',function(e,mark){
        MarkCreated(mark);
    });

    /**
     * Handles the event of annotations getting clicked.
     *
     * @example onMarkDeleted(object)
     *
     * @param Object mark that was deleted
     */
    jQuery('#documentViewer').bind('onMarkDeleted',function(e,mark){
        MarkDeleted(mark);
    });

    /**
     * Handles the event of annotations getting clicked.
     *
     * @example onMarkChanged(object)
     *
     * @param Object mark that was changed
     */
    jQuery('#documentViewer').bind('onMarkChanged',function(e,mark){
        MarkChanged(mark);
    });

    /**
     * Handles the event of a pdf requiring a password
     *
     * @example onPasswordNeeded(updatePassword,reason)
     *
     * @param updatePassword callback function for setting the password
     */
    jQuery('#documentViewer').bind('onPasswordNeeded',function(e,updatePassword){

    });
});


