<%@ Control Language="c#" AutoEventWireup="True" Codebehind="MailingListDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.MailingListDetailCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script type="text/javascript"> 
	function getElement(aID) { 
		return (document.getElementById) ? document.getElementById(aID) : document.all[aID]; 
	} 
	
	function getIFrameDocument(aID){ 
		var rv = null; 
		var frame=getElement(aID); 
		// if contentDocument exists, W3C compliant (e.g. Mozilla) 
		if (frame.contentDocument) 
			rv = frame.contentDocument; 
		else // bad IE ;) 
			rv = document.frames[aID].document; 
		
		return rv; 
	} 
	
	function adjustIFrameHeight(frameID) { 
		var frame = getElement(frameID); 
		var frameDoc = getIFrameDocument(frameID); 
		frame.height = frameDoc.body.offsetHeight; 
	} 
	
	function ReloadIFrame (frameID,exchangeID,messageID,style) {
		var f = document.getElementById(frameID);
		var src = "../Document/MailShowIFrame.aspx?contextID=" + messageID + "&xID=" + exchangeID + "&style=" + style;
		f.src = src;
	}
</script>
<table border="0" cellSpacing="0" cellPadding="0" style="height:100%;width:100%">
	<tr>
		<td height="20"></td>
	</tr>
	<tr>
		<td valign="top"><asp:table id="splitTab" runat="server" CssClass="InputMask" BorderWidth="0" Height="100%"></asp:table></td>
		<td valign="top"><asp:table id="attachementTab" BorderWidth="0" runat="server" Width="100%"></asp:table></td>
	</tr>
</table>
