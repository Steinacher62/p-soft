<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BudgetDetailControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.BudgetDetailControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table Runat="server" id="Table1" Height="100%">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell VerticalAlign="top">
            <asp:table id="editTab" runat="server" CssClass="InputMask" BorderWidth="0" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
