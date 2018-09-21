<%@ Page language="c#" Codebehind="PropertyBox.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.PropertyBox" %>
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
   	            var resize = window.document.getElementById("propertyTab");
   	            var w = parent.document.body.offsetHeight;
   	            var x = 1*parent.document.getElementById("propertyBoxX").value; // force number
   	            var y = 1*parent.document.getElementById("propertyBoxY").value; // force number
	            var newHeight = resize.offsetHeight;
	            var newWidth = resize.offsetWidth;
	            
	            window.resizeTo(newWidth, newHeight);
	            y += newHeight;
	            if (y > w) {
	                y = w - newHeight;
	                //window.moveTo(x, Math.max(0,y));
	            }
            }
            catch (e) {
            }
	    }
    </script>
    <body onload="resizeToFit();" leftmargin="0" topmargin="0">
        <form id="PropertyBox" method="post" runat="server">
            <asp:Table ID="propertyTab" Runat="server" CssClass="PropertyBox" BorderWidth="1" ></asp:Table>
        </form>
    </body>
</HTML>
