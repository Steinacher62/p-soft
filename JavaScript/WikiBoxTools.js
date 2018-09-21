// used by wikicontrol (WikiBoxCtrl)

function endsWith(str, end, ignoreCase) {
	if(ignoreCase) {
		str = str.toLowerCase();
		end = end.toLowerCase();
	}
	if((str.length - end.length) < 0){
		return false;
	}
	return str.lastIndexOf(end) == str.length - end.length;
}

function insertTags(tagOpen, tagClose, sampleText, ctrlIDpstfx) {
	var txtarea;/*= document.getElementById(controlID);*/
	var areas = document.getElementsByTagName('textarea');
	var tmp;
	for (var i=0; i < areas.length; i++){
		var y = areas[i].id;
		if (endsWith(y,ctrlIDpstfx,true)){
			txtarea = areas[i];
			break;
		}
	}

	// IE
	if (document.selection) {
		var theSelection = document.selection.createRange().text;
		if (!theSelection) {
			theSelection=sampleText;
		}
		txtarea.focus();
		if (theSelection.charAt(theSelection.length - 1) == " ") { // exclude ending space char, if any
			theSelection = theSelection.substring(0, theSelection.length - 1);
			document.selection.createRange().text = tagOpen + theSelection + tagClose + " ";
		} else {
			document.selection.createRange().text = tagOpen + sampleText + '|' + theSelection + tagClose;
		}
	
	// Mozilla
	} else if(txtarea.selectionStart || txtarea.selectionStart == '0') {
		var replaced = false;
		var startPos = txtarea.selectionStart;
		var endPos = txtarea.selectionEnd;
		if (endPos-startPos)
			replaced = true;
		var scrollTop = txtarea.scrollTop;
		var myText = (txtarea.value).substring(startPos, endPos);
		if (!myText)
			myText=sampleText;
		if (myText.charAt(myText.length - 1) == " ") { // exclude ending space char, if any
			subst = tagOpen + myText.substring(0, (myText.length - 1)) + tagClose + " ";
		} else {
			subst = tagOpen + myText + tagClose;
		}
		txtarea.value = txtarea.value.substring(0, startPos) + subst +
			txtarea.value.substring(endPos, txtarea.value.length);
		txtarea.focus();
		//set new selection
		if (replaced) {
			var cPos = startPos+(tagOpen.length+myText.length+tagClose.length);
			txtarea.selectionStart = cPos;
			txtarea.selectionEnd = cPos;
		} else {
			txtarea.selectionStart = startPos+tagOpen.length;
			txtarea.selectionEnd = startPos+tagOpen.length+myText.length;
		}
		txtarea.scrollTop = scrollTop;

	// All other browsers get no toolbar.
	}
	// reposition cursor if possible
	if (txtarea.createTextRange)
		txtarea.caretPos = document.selection.createRange().duplicate();
}

