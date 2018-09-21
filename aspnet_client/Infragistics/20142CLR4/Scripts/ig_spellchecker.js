/*
* ig_spellchecker.js
* Version 14.2.20142.1028
* Copyright(c) 2001-2014 Infragistics, Inc. All Rights Reserved.
*/


function ig_CreateWebSpellChecker(props)
{
	if(!ig_WebControl.prototype.isPrototypeOf(ig_WebSpellChecker.prototype))
    {
        ig_WebSpellChecker.prototype = new ig_WebControl();
        ig_WebSpellChecker.prototype.constructor = ig_WebSpellChecker;
        ig_WebSpellChecker.prototype.base=ig_WebControl.prototype;
        
        ig_WebSpellChecker.prototype.init = function(props)
        {						
			
            this._isInitializing = true;
            this._initControlProps(props);
            var o; 
			if(ig_all)
				o = ig_all[this.getClientID()]; 
            this.base.init.apply(this,[this.getClientID()]);
            this._isInitializing = false;
            this._spellChecker = this;
            
            var id = this.getButtonId();
			if(id != null && id.length > 0)
			{
				var button = document.getElementById(id);
				if(button != null)
				{
					button._spellChecker = this;
					ig_shared.addEventListener(button, "click", this._buttonClick, true);
				}
			}
        }
        
        ig_WebSpellChecker.prototype._onUnload = function(src, evnt)
        {
			if(this._window != null)
				this._window.close();
        }

        ig_WebSpellChecker.prototype._buttonClick = function(evnt)
        {
        	var elem = evnt.target;
        	if (evnt.srcElement)
        		elem = evnt.srcElement
        	elem._spellChecker.checkTextComponent();
        	

        	if (!ig_shared.IsIE7Compat) ig_cancelEvent(evnt);
        }
        ig_WebSpellChecker.prototype.checkTextComponent = function(textComponentId)
        {
			if(textComponentId == null)
				textComponentId = this._spellChecker.getTextComponentId()
				
			var textToBeChecked = this._spellChecker._escQuotes(this._spellChecker._escEntities(this._spellChecker.getText(textComponentId)));
			
			this._spellChecker.checkSpelling(textToBeChecked, null, textComponentId);
        }
        
		ig_WebSpellChecker.prototype.checkSpelling = function(textToBeChecked, returnFunc, textComponentId)
		{
			
			var me = this._spellChecker;
			if(!textToBeChecked || !textToBeChecked.length || me._onBeforeSpellCheckBegins(textToBeChecked))
				return;
			
			textToBeChecked = escape(textToBeChecked.replace(/\+/g,"%2B"));
			me._returnFunc = returnFunc;
			textComponentId = textComponentId || "";
			
			if(typeof iged_getById == 'function') if (iged_getById(textComponentId))
				this._htmlEditor = true;
			var doc = "<html><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><body style='margin:0px;overflow:hidden;'>";
			doc += "<font face='arial, helvetica' size=2>"+ me.getSpellCheckText()+"</font>";
			
			var dialogUrl = me.getWebSpellCheckerDialogPage() || document.forms[0].action;
			doc += "<form accept-charset='UTF-8' action='" + dialogUrl + "' method='post'>";
			doc += "<input type='hidden' name='textToCheck' value=\""+textToBeChecked+"\">";
			doc += "<input type='hidden' name='UserDictionaryFile' value='"+me.getUserDictionaryFile()+"'>";
			doc += "<input type='hidden' name='SuggestionsMethod' value='"+me.getSuggestionsMethod()+"'>";
			doc += "<input type='hidden' name='LanguageParser' value='"+me.getLanguageParser()+"'>";
			doc += "<input type='hidden' name='SeparateHyphenWords' value='"+me.getSeparateHyphenWords()+"'>";
			doc += "<input type='hidden' name='AllowWordsWithDigits' value='"+me.getAllowWordsWithDigits()+"'>"; 
			doc += "<input type='hidden' name='SuggestSplitWords' value='"+me.getSuggestSplitWords()+"'>";
			doc += "<input type='hidden' name='CheckCompoundWords' value='"+me.getCheckCompoundWords()+"'>";
			doc += "<input type='hidden' name='WebSpellCheckerId' value='"+me.getClientID()+"'>";
			doc += "<input type='hidden' name='AllowMixedCase' value='"+me.getAllowMixedCase()+"'>"; 
			doc += "<input type='hidden' name='IncludeUserDictionaryInSuggestions' value='"+me.getIncludeUserDictionaryInSuggestions()+"'>";
			doc += "<input type='hidden' name='CheckHyphenatedText' value='"+me.getCheckHyphenatedText()+"'> ";
			doc += "<input type='hidden' name='AllowXML' value='"+me.getAllowXML()+"'>";
			doc += "<input type='hidden' name='AllowCapitalizedWords' value='"+me.getAllowCapitalizedWords()+"'>";
			doc += "<input type='hidden' name='AllowCaseInsensitiveSuggestions' value='"+me.getAllowCaseInsensitiveSuggestions()+"'>";
			doc += "<input type='hidden' name='ConsiderationRange' value='"+me.getConsiderationRange()+"'>";
			doc += "<input type='hidden' name='Dictionary' value='"+me.getDictionary()+"'>";
			doc += "<input type='hidden' name='TextComponentId' value ='" + textComponentId + "'>";
			doc += "<input type='hidden' name='SplitWordThreshold' value='"+me.getSplitWordThreshold()+"'>";
			doc += "<input type='hidden' name='FormId' value='"+window.document.forms[0].id+"'>";
			
			doc += "<input type='hidden' name='StyleSetName' value='"+me._props[34]+"'>";
			doc += "<input type='hidden' name='StyleSetPath' value='"+me._props[35]+"'>";
			doc += "<input type='hidden' name='EnableAppStyling' value='"+me._props[36]+"'>";
			doc += "<input type='hidden' name='ImageDirectory' value='"+me._props[37]+"'>";
			doc += "</form></body>"; 
			doc += "</html>"; 
			var left = Math.max(0, me.getWindowX()), top = Math.max(0, me.getWindowY()), width = Math.max(0, me.getWindowWidth()), height = Math.max(0, me.getWindowHeight());
			var url = me.getWebSpellCheckerDialogPage() || '';
			var modal = me.getModal();
			if (!modal || !ig_shared.IsIE || !window.showModalDialog)
			{
				var opts = (modal ? 'modal=yes,' : '') + (left ? 'left=' + left + ',' : '') + (top ? 'top=' + top + ',' : '') + (width ? 'width=' + width + ',' : '') + (height ? 'height=' + height + ',' : '') + 'resizable=yes,scrollbars=auto,dependent=yes,toolbar=no,status=no,location=no,menubar=no';
				var name = modal || !me.getAllowMultipleDialogs() ? '' : 'WebSpellChecker';
				me._openNonModalWindow(url, name, opts, doc);
				return;
			}
			if(!url.length)
			{
				
				url = document.forms[0].action;
				
				if (url.indexOf("?") == -1)
				    url += "?";
				else
				    url += "&";
				url += "Modal=" + window.document.forms[0].id;
			}
			showModalDialog(url, [me, doc, document], (height ? "dialogHeight:" + (height + 5) + "px;" : "") + (width ? "dialogWidth:" + (width + 5) + "px;" : "") + (left ? "dialogLeft:" + left + "px;" : "") + (top ? "dialogTop:" + top + "px;" : "") + "center:yes;help:no;resizable:no;status:no;scroll:no;");
		}
        
        


		ig_WebSpellChecker.prototype._fireFoxFireFinishedServerEvent = function()
		{
			this._ffFinishedTimerId = setInterval(ig_createCallback(this._delayCallbackHandler, this, null), 500);
		}
		ig_WebSpellChecker.prototype._delayCallbackHandler = function()
		{
			clearTimeout(this._ffFinishedTimerId);
			this._fireSpellCheckComplete();
		}
		
		ig_WebSpellChecker.prototype._fireSpellCheckComplete = function()
		{
			
			var tempUniqueId = this.getUniqueID().replace(/\$/g, "_");
			var hiddenInput = document.getElementById(tempUniqueId);
			if(hiddenInput == null)
			{
				hiddenInput = document.createElement("input");
				hiddenInput.type = "hidden";
				hiddenInput.id = tempUniqueId; 
				hiddenInput.name = tempUniqueId; 
				hiddenInput.value = "";
				var element = document.getElementById(this.getClientID()+ "_Data"); 
				element.parentNode.appendChild(hiddenInput);
			}
			this.fireServerEvent("SpellCheck", "Finished");
		}
        
        ig_WebSpellChecker.prototype._openNonModalWindow = function(url, name, params, doc)
		{ 
			this._window = window.open(url, name, params); 
			this._window.focus();  
			this._window.document.open();  
			this._window.document.write(doc); 
			this._window.document.close(); 
			this._window.document.forms[0].submit();
		}
		
		ig_WebSpellChecker.prototype.getText = function(textComponentId, doNotFireEvnt)
		{
			var text = "";

			var textComponentDelegate = this._getTextComponentObjectDelegate()

			if (textComponentId == null)
				textComponentId = this.getTextComponentId();

			if (textComponentDelegate == null || textComponentDelegate.length == 0)
			{
				var textElement = null;
				if (typeof igedit_getById == 'function')
				{
					textElement = igedit_getById(textComponentId);
				}
				if(!textElement && typeof iged_getById == 'function')
				{
					textElement = iged_getById(textComponentId);
		        	
					if (textElement)
						this._htmlEditor = true;
				}

				if (textElement != null)
					text = textElement.getText();
				else
				{
					textElement = document.getElementById(textComponentId);

					if (textElement != null)
						text = textElement.value;
				}
				var getText = doNotFireEvnt ? "" : this._onGetText(text);

				if (getText == "")
					return text;
				else
					return getText;
			}
			else
			{
				var getTextDelegate = this._getTextComponentGetTextDelegate();
				if (getTextDelegate != null && getTextDelegate.length > 0)
				{
					textComponentDelegate = eval(textComponentDelegate);
					if (textComponentDelegate != null)
					{
						var textElement = textComponentDelegate(textComponentId);

						if (textElement != null)
						{
							getTextDelegate = eval("textElement." + getTextDelegate);
							if (getTextDelegate != null)
							{
								textElement.__mygettextfunc = getTextDelegate;
								var text = textElement.__mygettextfunc();
								
								if (!doNotFireEvnt)
									this._onGetText(text);
								return text;
							}
						}
					}
				}
				return "";
			}
		}
        
        ig_WebSpellChecker.prototype.setText = function(textComponentId, value)
        {
			
			if(textComponentId)
			{
				
				var old = this.getText(textComponentId, true);
				if(old === value)
					return;
			}
			if(!this._onSetText(value))
			{
				var textComponentDelegate = this._getTextComponentObjectDelegate()
				
				if(textComponentId == null)
						textComponentId = this.getTextComponentId();
						
				if(textComponentDelegate == null || textComponentDelegate.length == 0)
				{
					var textElement = null;
					if (typeof igedit_getById == 'function')
					{
						textElement = igedit_getById(textComponentId);
					}
					if(!textElement && typeof iged_getById == 'function')
					{
						textElement = iged_getById(textComponentId);
			        	
						if (textElement)
							this._htmlEditor = true;
					}
					
					if(textElement != null)
						textElement.setText(value);
					else
					{
						textElement = document.getElementById(textComponentId);
				
						if(textElement != null) 
							textElement.value = value;
					}
				}
				else
				{
					var setTextDelegate = this._getTextComponentSetTextDelegate();
					if(setTextDelegate != null && setTextDelegate.length > 0)
					{
						textComponentDelegate = eval(textComponentDelegate);
						if(textComponentDelegate != null)
						{
							var textElement = textComponentDelegate(textComponentId);
							
							if(textElement != null)
							{
								setTextDelegate = eval("textElement."+setTextDelegate);
								if(setTextDelegate != null)
								{
									textElement.__mysettextfunc = setTextDelegate;
									textElement.__mysettextfunc(value);
								}
							}
						}
					}
				}
			}
        }
        
        ig_WebSpellChecker.prototype.getCheckCompoundWords = function() 
        {
          return this._props[2];
		}
        ig_WebSpellChecker.prototype.getConsiderationRange = function() 
        {
          return this._props[3];
		}
        ig_WebSpellChecker.prototype.getAllowCapitalizedWords = function() 
        {
          return this._props[4];
		}
        ig_WebSpellChecker.prototype.getAllowWordsWithDigits = function() 
        {
          return this._props[5];
		}
        ig_WebSpellChecker.prototype.getAllowXML = function() 
        {
          return this._props[6];
		}
        ig_WebSpellChecker.prototype.getIncludeUserDictionaryInSuggestions = function() 
        {
          return this._props[7];
		}
        ig_WebSpellChecker.prototype.getLanguageParser = function() 
        {
          return this._props[8];
		}
        ig_WebSpellChecker.prototype.getCheckHyphenatedText = function() 
        {
          return this._props[9];
		}
        ig_WebSpellChecker.prototype.getModal = function() 
        {
          return this._props[10];
		}
        ig_WebSpellChecker.prototype.getWebSpellCheckerDialogPage = function() 
        {
          return this._props[11];
		}
        ig_WebSpellChecker.prototype.setWebSpellCheckerDialogPage = function(value) 
        {
		  this._props[11] = value;
        }
          ig_WebSpellChecker.prototype.getSeparateHyphenWords = function() 
        {
          return this._props[12];
		}
        ig_WebSpellChecker.prototype.getShowFinishedMessage = function() 
        {
          return this._props[13];
		}
        ig_WebSpellChecker.prototype.getShowNoErrorsMessage = function() 
        {
          return this._props[14];
		}
        ig_WebSpellChecker.prototype.getSpellCheckText = function() 
        {
          return this._props[15];
		}
        ig_WebSpellChecker.prototype.getSuggestionsMethod = function() 
        {
          return this._props[16];
		}
        ig_WebSpellChecker.prototype.getSuggestSplitWords = function() 
        {
          return this._props[17];
		}
        ig_WebSpellChecker.prototype.getTextComponentId = function() 
        {
          return this._props[18];
		}
        ig_WebSpellChecker.prototype.setTextComponentId = function(value) 
        {
		  this._props[18] = value;
        }
        ig_WebSpellChecker.prototype.getUserDictionaryFile = function() 
        {
          return this._props[19];
		}
        ig_WebSpellChecker.prototype.getWindowHeight = function() 
        {
          return this._props[20];
		}
        ig_WebSpellChecker.prototype.getWindowWidth = function() 
        {
          return this._props[21];
		}
        ig_WebSpellChecker.prototype.getWindowX = function() 
        {
          return this._props[22];
		}
        ig_WebSpellChecker.prototype.getWindowY = function() 
        {
          return this._props[23];
		}
        ig_WebSpellChecker.prototype.getAllowMixedCase = function() 
        {
          return this._props[24];
		}
        ig_WebSpellChecker.prototype.getAutoPostBackSpellCheckComplete = function() 
        {
          return this._props[25];
		}
        ig_WebSpellChecker.prototype.getAllowMultipleDialogs = function() 
        {
          return this._props[26];
		}
        ig_WebSpellChecker.prototype.getButtonId = function() 
        {
          return this._props[27];
		}
		ig_WebSpellChecker.prototype.getDictionary = function() 
        {
          return this._props[28];
		}
		ig_WebSpellChecker.prototype.getAllowCaseInsensitiveSuggestions = function() 
        {
          return this._props[29];
		}
		ig_WebSpellChecker.prototype._getTextComponentObjectDelegate = function() 
        {
          return this._props[30];
		}
		ig_WebSpellChecker.prototype._getTextComponentGetTextDelegate = function() 
        {
          return this._props[31];
		}
		ig_WebSpellChecker.prototype._getTextComponentSetTextDelegate = function() 
        {
          return this._props[32];
		}
		ig_WebSpellChecker.prototype.getSplitWordThreshold = function() 
        {
          return this._props[33];
		}
		ig_WebSpellChecker.prototype._escQuotes = function(text)
		{  
			var rx = new RegExp("\"", "g"); 
			return text.replace(rx,"&#34;");
		}
		ig_WebSpellChecker.prototype._escEntities = function(text)
		{
			var rx = new RegExp("&", "g"); 
			return text.replace(rx,"&amp;");
		}
        ig_WebSpellChecker.prototype._onGetText = function(text)
        {
        	return this._fireEvent("GetText", null, text);
        }
        ig_WebSpellChecker.prototype._onSetText = function(text)
        {
        	return this.fireEvent("SetText", null, text);
        }
        ig_WebSpellChecker.prototype._onSpellCheckComplete = function(oldText, correctedText)
        {
        	return this.fireEvent("SpellCheckComplete", null, oldText, correctedText);
        }
        ig_WebSpellChecker.prototype._onWordCorrected = function(oldWord, newWord)
        {
        	return this._fireEvent("WordCorrected", null, oldWord, newWord);
        }
        ig_WebSpellChecker.prototype._onBeforeSpellCheckBegins = function(text)
        {
        	return this.fireEvent("BeforeSpellCheckBegins", null, text);
        }
        ig_WebSpellChecker.prototype._fireEvent = function(name, evnt)
        {
			if(!name || this._isInitializing || !this._clientEvents)
				return false;
			this._postRequest = 0;
			var evt, evts = this._clientEvents[name];
			var cancel = false, post = 0, i = (evts == null) ? 0 : evts.length;
			if(i == 0)
				return false;
			if(evnt == "check")
				return true;
			var args = this._fireEvent.arguments;
			var returnVal = "";
			while(i-- > 0)
			{
				if(evts[i] == null)
					continue;
				evt = evts[i]._event;
				evt.reset();
				evt.event=evnt;
				evt.needPostBack=evts[i]._autoPostBack;
				try
				{
					returnVal = evts[i]._handler(this, evt, args[2], args[3], args[4], args[5], args[6]);
				}catch(ex){continue;}
			}
			if(returnVal == null)
				returnVal = "";
			return returnVal;
        }
     }
     return new ig_WebSpellChecker(props);
}

function ig_WebSpellChecker(props)
{
   if(arguments.length != 0)
       this.init(props);
}



