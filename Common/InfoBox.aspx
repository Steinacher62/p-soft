<%@ Page language="c#" Codebehind="InfoBox.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.InfoBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>Info</title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="../Style/Psoft.css" type="text/css" rel="stylesheet">
    </HEAD>
    <script  type='text/javascript' type="text/javascript">
	    function resizeToFit()
	    {
	        try {
    	        var resize = window.document.getElementById("infoTab");
	            var newHeight = resize.offsetHeight;
	            var newWidth = resize.offsetWidth;
	            
	            window.resizeTo(newWidth, newHeight);
            }
            catch (e) {
            }
	    }
    </script>
    <body onload="resizeToFit();" leftmargin="0" topmargin="0">
        <form id="InfoBox" method="post" runat="server">
            <asp:Table ID="infoTab" Runat="server" BorderWidth="1" CssClass="message"></asp:Table>
        </form>
    </body>
</HTML>
