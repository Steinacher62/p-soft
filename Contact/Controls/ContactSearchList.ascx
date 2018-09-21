<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactSearchList.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactSearchList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table Runat="server" id="Table1" Height="100%" Width="100%">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow VerticalAlign="Top" Height="100%">
		<asp:TableCell ID="cellList">
			<div class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" runat="server" CssClass="List" CellPadding="2" CellSpacing="0"></asp:Table>
			</div>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow VerticalAlign="Bottom">
		<asp:TableCell>
			<asp:Button ID="next" Runat="server" CssClass="Button"></asp:Button>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
