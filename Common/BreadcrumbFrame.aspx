<%@ Page language="c#" Codebehind="BreadcrumbFrame.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.BreadcrumbFrame" EnableViewState="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>breadcrumbFrame</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="../Style/Psoft.css" type="text/css" rel="stylesheet">
        <script  type='text/javascript'>
            function setContentFrameFocusCtrl(){
                if (top.frames && top.frames.contentFrame && top.frames.contentFrame.setPsoftFocusCtrl){
                    try{
                        top.frames.contentFrame.setPsoftFocusCtrl();
                    }
                    catch (e){
                    }
                }
            }
        </script>
	</HEAD>
	<body onload="setContentFrameFocusCtrl();">
		<form id="breadcrumbFrame" method="post" runat="server">
            <table cellSpacing="0" cellPadding="0" border="0" width="100%" height="100%">
                <tr>
                    <td vAlign="top" height="100%" width="100%">
                        <img src="../images/header_bottom.jpg" alt="" border=0>
                    </td>
                </tr>
            </table>
            <div style="LEFT: 10px; WIDTH: 100%; HEIGHT: 100%; POSITION: absolute; TOP: 0px" runat="server" id="headerDIV">
			    <asp:Table id="breadcrumbTable" runat="server" CellPadding="0" CellSpacing="2" BorderWidth="0px" Height="100%" Width="100%" CssClass="subNav">
				    <asp:TableRow Width="100%" VerticalAlign="Middle" HorizontalAlign="Left" ID="breadcrumbRow" Height="100%">
					    <asp:TableCell ID="breadcrumbCell" CssClass="subNav" VerticalAlign="Middle"></asp:TableCell>
				    </asp:TableRow>
			    </asp:Table>
			 </div>
		</form>
	</body>
</HTML>
