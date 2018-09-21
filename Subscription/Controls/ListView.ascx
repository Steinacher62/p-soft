<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ListView.ascx.cs" Inherits="ch.appl.psoft.Subscription.Controls.ListView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "<%=_backURL%>";

    function deleteRowConfirm(table,rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, table, rowId, dbId)
    }
    
</script>
<asp:Table Runat="server" BorderWidth="0" id="Table2" cellspacing="2" cellpadding="0" Height="100%" Width="500">
	<asp:TableRow>
		<asp:TableCell Height="32" VerticalAlign="Top">
			<asp:Label ID="titleLbl" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
		<asp:TableCell Height="32" VerticalAlign="Top" Width="100%" HorizontalAlign="Right">
			<asp:CheckBox ID="all" Runat="server" AutoPostBack="True" ></asp:CheckBox>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Height="10"></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow  Height="100%" VerticalAlign="Top">
		<asp:TableCell ColumnSpan="2">
			<div class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" Runat="server" Width="100%" CellSpacing="0" CellPadding="1" ></asp:Table>
			</div>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
