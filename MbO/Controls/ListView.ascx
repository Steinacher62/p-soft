<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ListView.ascx.cs" Inherits="ch.appl.psoft.MbO.Controls.ListView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=_deleteMessage%>";
    var deleteURL = "<%=_deleteURL%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "OBJECTIVE", rowId, dbId)
    }
    
    function openArgument(url) {
		var x = window.screen.availWidth/2;
		var y = window.screen.availHeight/2;
		window.open(url,"argument","menubar=0,status=0,titlebar=0,toolbar=0,height=400,width=500,left="+x+",top="+y);
    }
</script>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="8">
	<TBODY>
		<tr>
			<td align="left" >
				<asp:Label id="title" runat="server" CssClass="section_title"></asp:Label>
			</td>
		</tr>
		<tr valign="top" Height="100%">
			<td colspan="2">
				<asp:Table id="listTab" runat="server" BorderWidth="0" CssClass="List" CellSpacing="0" CellPadding="2"></asp:Table>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Button id="next" runat="server" CssClass="Button"></asp:Button>
				<!--<asp:Button id="apply" runat="server" CssClass="Button"></asp:Button>-->
				<asp:Button id="rating" runat="server" CssClass="Button"></asp:Button>
			</td>
		</tr>
	</TBODY>
</table>
