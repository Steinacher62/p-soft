	var popUpWindow;

	function openPopupWindow(url, width, height)
	{
	    var properties = 'locationbar=0,menubar=0,titlebar=0,status=0,toolbar=0,resizable=1';
	    if (width && height)
	    {
		    var x = (window.screen.availWidth - width) / 2;
		    var y = (window.screen.availHeight - height) / 2;
		    properties += ',height='+height+',width='+width+',top='+y+',left='+x;
		}
        popUpWindow = window.open(url,'_blank',properties);
	}
	
	function center()
	{
	    if (top.location == self.location)
	    {
		    var x = (window.screen.availWidth - window.document.body.clientWidth) / 2;
		    var y = (window.screen.availHeight - window.document.body.clientHeight) / 2;
		    window.moveTo(x,y);
		}
	}
	
	function resizeToFit()
	{
	    if (top.location == self.location)
	    {
	        try{
    	        var resizeDIV = window.document.getElementById("resizeDIV");
    	        if (!resizeDIV)
    	            return;
	            var newHeight = resizeDIV.offsetHeight + 60;
	            var newWidth = resizeDIV.offsetWidth + 40;
	            if(!newHeight || !newWidth)
	                return;
	            window.resizeTo(newWidth, newHeight);
            }
            catch (e){
            }
	    }
	}
	
    function popup(w,h,site) {
		x=screen.availWidth/2-w/2;
		y=screen.availHeight/2-h/2;
		popUpWindow=window.open('','','width='+w+',height='+h+',left='+x+',top='+y+',screenX='+x+',screenY='+y);
		popUpWindow.document.write('<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">');
		popUpWindow.document.write(site);
	}

