	var propertyBox;
	var propertyBox_Width = 10;
	var propertyBox_Height = 10;
	var propertyBox_Path = "";
	var propertyBox_Timer;
	
	function erasePropertyBox() {
		//alert(erasePropertyBox);
		clearTimeout(propertyBox_Timer);
		if (propertyBox) propertyBox.close();
		propertyBox = null;
	}
	function drawPropertyBox(properties) {
		//alert("drawPropertyBox="+properties);
		var x = Math.min(window.event.screenX+20, window.screen.width-propertyBox_Width-10);
		var y = Math.min(window.event.screenY-20, window.screen.height-propertyBox_Height-60);
		if (x < window.event.screenX+10)
		{
		    x = window.event.screenX - propertyBox_Width - 30;
		}
		propertyBox_Timer = setTimeout("drawPropertyBoxNow('"+properties+"','"+x+"','"+y+"')",500);
	}
	function drawPropertyBoxNow(properties,x,y) {
		//alert("drawPropertyBox="+properties);
		propertyBox = window.open(propertyBox_Path+"PropertyBox.aspx?"+properties,"PropertyBox","locationbar=0,menubar=0,titlebar=0,status=0,toolbar=0,height="+propertyBox_Height+",width="+propertyBox_Width+",top="+y+",left="+x);
	}
    function showPropertyBox(infoURL) {
	    var obj = document.getElementById("propertyBox");
	    try {
	        obj.src = infoURL;
	        var x = window.event.x+10;
	        var y = window.event.y-70;
	        var X = document.getElementById("propertyBoxX");
	        var Y = document.getElementById("propertyBoxY");
	        X.value = x;
	        Y.value = y;
	        //alert(x + '/' + y);
	        showElement("propertyBox",x,y);
	    }
	    catch (e) {
	    }
    }
    function hidePropertyBox() {
	    hideElement("propertyBox");
    }

