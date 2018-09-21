

    function setFocus(elementName)
    {
        var element = document.getElementById(elementName);
        if (element) element.focus();
    }
    
    function setPsoftFocusCtrl(element){
        if (element){
            if (element && element.PsoftFocusCtrl && element.PsoftFocusCtrl=="1" && !element.isDisabled){
                element.focus();
                element.scrollIntoView();
                return true;
            }
            else{
                if (element.childNodes){
                    var children = element.childNodes;
                    var i;
                    for (i=0; i<children.length; i++){
                        if (setPsoftFocusCtrl(children.item(i))){
                            return true;
                        }
                    }
                }
            }
        }
        else{
            setPsoftFocusCtrl(document);
        }
        return false;
    }
	
    function loadURLinFrame(target,url,close)
    {
        var targetWindow = null;
        switch (target)
        {
            case "_self":
                targetWindow = window;
                break;
            case "_top":
                targetWindow = top;
                break;
            case "_opener":
                targetWindow = top.opener;
                break;
            case "_parent":
                targetWindow = parent;
                break;
            case "_blank":
                window.open(url);
                break;
            default:
                if (parent.frames[target] != null)
                    targetWindow = parent.frames[target];
                else if (top.frames[target] != null)
                    targetWindow = top.frames[target];
                else if (frames[target] != null)
                    targetWindow = frames[target];
                else if (parent.parent && parent.parent.frames[target] != null)
                    targetWindow = parent.parent.frames[target];
                break;
        }
        
        if (targetWindow != null && url != null)
            targetWindow.location.href = url;
            
        if (close)
            top.close();
    }

    // HighLighter
	var currentHighlightEle = null;
	var currentHighlightBackColor = null;
	var currentHighlightClassNames = null;
    function highlightElement(target,url,eleId)
    {
        var ele;
        if (currentHighlightEle && currentHighlightClassNames)
        {
            var i=0;

            if (currentHighlightEle.className && currentHighlightEle.className != "")
                currentHighlightEle.className = currentHighlightClassNames[i++];
            else
                currentHighlightEle.style.backgroundColor = currentHighlightBackColor;
                
            for (var child in currentHighlightEle.childNodes)
            {
				ele = currentHighlightEle.childNodes[child];
                if (ele && ele.className && ele.className != "")
                    ele.className = currentHighlightClassNames[i++];
            }
            currentHighlightEle = null;
            currentHighlightBackColor = null;
            currentHighlightClassNames = null;
        }
        
        currentHighlightEle = document.getElementById(eleId);
        
        if (currentHighlightEle)
        {
            var i=0;
            currentHighlightClassNames = new Array();
            
            if (currentHighlightEle.className && currentHighlightEle.className != "")
            {
                currentHighlightClassNames[i++] = currentHighlightEle.className;
                currentHighlightEle.className += "_selected";
            }
            else
            {
                currentHighlightBackColor = currentHighlightEle.style.backgroundColor;
                currentHighlightEle.style.backgroundColor = top.highlightColor;
            }
                
            for (var child in currentHighlightEle.childNodes)
            {
				ele = currentHighlightEle.childNodes[child];
                if (ele && ele.className && ele.className != "")
                {
                    currentHighlightClassNames[i++] = ele.className;
                    ele.className += "_selected";
                }
            }
        }
        
        loadURLinFrame(target, url, false);
    }
    
    var currentActualPage = 0;
    function showActualPage(table, actualPage, rowsPerPage, hasHeader)
    {
        if (top.hideProgressBar)
            top.hideProgressBar();
            
        var offset = 0;
        if (hasHeader)
            offset = 1;
        if (table && actualPage >= 0 && rowsPerPage > 0 && actualPage <= Math.floor((table.rows.length-offset-2)/rowsPerPage))
        {
            currentActualPage = actualPage;
            var j;
            var obj;
            for (j=0; j<table.rows[table.rows.length-1].cells[0].childNodes.length; j++)
            {
                obj = table.rows[table.rows.length-1].cells[0].childNodes[j];
                if (obj.id.indexOf("actualPage" + actualPage + "_") > 0)
                {
                    obj.className = "selected";
                    obj.href = "#";
                }
                else
                {
                    obj.className = "";
                    if (obj.hrefOrig)
                        obj.href = obj.hrefOrig;
                }
            }
            var i;
            for (i=0; i<table.rows.length-1-offset; i++)
            {
                if (i < ((actualPage)*rowsPerPage) || i >= (actualPage+1)*rowsPerPage)
                    table.rows[i+offset].style.display = "none";
                else
                    table.rows[i+offset].style.display = "";
            }
            
            if (typeof(actualPagePrev) != "undefined")
                actualPagePrev.disabled = actualPage == 0;
            if (typeof(actualPageNext) != "undefined")
                actualPageNext.disabled = actualPage == Math.floor((table.rows.length-offset-2)/rowsPerPage);
        }
    }
    
    function showNextPage(table, rowsPerPage, hasHeader)
    {
        showActualPage(table, currentActualPage+1, rowsPerPage, hasHeader);
    }
    
    function showPreviousPage(table, rowsPerPage, hasHeader)
    {
        showActualPage(table, currentActualPage-1, rowsPerPage, hasHeader);
    }
    
    /*
    * InputMaskBuilder sets onkeydown attribute to dropdownlist
    */

    var quickSearchDropDownPressedKeys = "";
    var quickSearchDropDownPressedKeyTimerID = null;
    function quickSearchDropDownPressed(){
        var key=String.fromCharCode(window.event.keyCode);
        //var reg = /[a-zA-Z0-9_ \-]/; 
        var reg = /[\w \u00e4\u00f6\u00fc\u00e9\u00e0\u00e8]/; // umlaute recognition depends on system configuration
        if (reg.test(key)){ // test keys
            if (quickSearchDropDownPressedKeyTimerID != null){
                window.clearTimeout(quickSearchDropDownPressedKeyTimerID);
                quickSearchDropDownPressedKeyTimerID = null;
            }
            quickSearchDropDownPressedKeyTimerID = window.setTimeout("quickSearchDropDownPressedTimeout()", 1000);
            quickSearchDropDownPressedKeys += key.toLowerCase();
            var sObj = window.event.srcElement;
            var len = sObj.options.length;
            var idx = sObj.selectedIndex;
            for (var i=0; i<len; i++){
                if (sObj.options[i].text.toLowerCase().substr(0,quickSearchDropDownPressedKeys.length) == quickSearchDropDownPressedKeys
                    || (quickSearchDropDownPressedKeys == ' ' && sObj.options[i].text == '')){
                    idx = i;
                    break;
                }
            }        
            sObj.options[idx].selected = true;
            event.returnValue=false; // prevent further processing
        }
    }

    function quickSearchDropDownPressedTimeout(){
        quickSearchDropDownPressedKeyTimerID = null;
        quickSearchDropDownPressedKeys = "";
    }


    // Called when deleting a row directly from a list (ListBuilder).
    // It checks for the existence of a function called deleteRowConfirm() - if it exists,
    // it gets called, otherwise the webservice-stub gets called.
    function listDeleteRowConfirm(rowId, dbId, tablename)
    {
        if (typeof (deleteRowConfirm) != "undefined") {
            deleteRowConfirm(rowId, dbId, tablename);
        }
        else {
            wsDeleteRowConfirm(this, document.location.href, deleteConfirmMessage, tablename, rowId, dbId)
        }
    }
    
    // Replaces the inner HTML of an element.
    // This function can be used to work around a problem introduced through a Microsoft Update of IE in April 2006
    // which forces the user to activate an ActiveX control to allow user-interaction. When creating the object through 
    // an external script, it doesn't need to be activated by the user.
    function replaceInnerHTML(elementID, newInnerHTML){
        var element = document.getElementById(elementID);
        element.innerHTML = newInnerHTML;
    }
    
    // Shows the element with the given id.
    function showElement(id,x,y) {
	    var obj = document.getElementById(id);
	    if (obj != null) {
	        obj.style.display = '';
	        if (x) obj.style.left = x;
	        if (y) obj.style.top = y;
	        //alert ("x,y="+x+","+y);
		    // For elements which support the showMe flag, do only display
		    // them if the flag is set to true.
		    if (obj.showMe != null) obj.showMe = true;
	    }
    }
    function hideParentElement(id) {
	    var obj = parent.document.getElementById(id);
	    //alert("obj="+obj);
	    if (obj != null) {
		    obj.style.display = 'none';
		    // Set showMe flag to false for all elements which support it.
		    if (obj.showMe != null) obj.showMe = false;
	    }
    }    
    function hideElement(id) {
	    var obj = document.getElementById(id);
	    if (obj != null) {
		    obj.style.display = 'none';
		    // Set showMe flag to false for all elements which support it.
		    if (obj.showMe != null) obj.showMe = false;
	    }
    }    
    function hideAllElements(name) {
	    var obj = document.getElementsByName(name);
	    if (obj != null) {
	        for (var o=0; o<obj.length; o++) {
		        obj[o].style.display = 'none';
		        // Set showMe flag to false for all elements which support it.
		        if (obj[o].showMe != null) obj[o].showMe = false;
	        }
	    }
    }
    function showInfo(infoURL) {
	    var obj = document.getElementById("infoBox");
	    if (obj != null) {
	        obj.src = infoURL;
	        var x = window.event.x+10;
	        var y = window.event.y-20;
	        var X = document.getElementById("infoBoxX");
	        var Y = document.getElementById("infoBoxY");
	        
	        X.value = x;
	        Y.value = y;
	        showElement("infoBox",x,y);
	    }
    }
    function hideInfo() {
	    hideElement("infoBox");
    }
    
    function showCalendarBox(url,initValId) {
	    var obj = document.getElementById("calendarBox");
	    var init = document.getElementById(initValId);
	    try {
	        if (init) url += "&initValue="+init.value;
	        obj.src = url;
	        var x = window.event.x+10;
	        var y = window.event.y;
	        showElement("calendarBox",x,y);
	        
	        var X = document.getElementById("calendarBoxX");
	        var Y = document.getElementById("calendarBoxY");
	        X.value = x;
	        Y.value = y;
	    }
	    catch (e) {
	    }
    }
    function hideCalendarBox(parent) {
	    if (parent) hideParentElement("calendarBox");
	    else hideElement("calendarBox");
    }
