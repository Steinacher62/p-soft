    function readRegistryFlag(InputCtrl) {
        //alert("readRegistryFlag="+InputCtrl);
        if (InputCtrl) {
            InputCtrl.value = "";
            var first = true;
            for (var counter=0; counter < document.forms[0].elements.length; counter++) {
                var element = document.forms[0].elements[counter];
                if (element && (element.type == 'checkbox' || element.type == 'radio') && element.id.substring(0,7) == 'RegFlag') {
                    var neg = element.id.charAt(7);
                    
                    if (element.checked) {
                        if (neg != "-") {
                            if (!first) InputCtrl.value += ",";
                            InputCtrl.value += element.id.substring(7);
                            first = false;
                        }
                    }
                    else if (neg == "-") {
                        if (!first) InputCtrl.value += ",";
                        InputCtrl.value += element.id.substring(7);
                        first = false;
                    }
                }
            }
        }
    }
