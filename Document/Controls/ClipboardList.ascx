<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ClipboardList.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.ClipboardList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteURL = "<%=PostDeleteURL%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "DOCUMENT", rowId, dbId)
    }
</script>
<asp:Table Runat="server" id="Table1" Height="100%">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Height="100%">
		<asp:TableCell>
			<DIV class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%" EnableViewState="False"></asp:Table>
			</DIV>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow VerticalAlign="Bottom" ID="ButtonRow" Visible="False" Runat="server">
		<asp:TableCell>
			<asp:Button ID="next" Runat="server" CssClass="Button"></asp:Button>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>