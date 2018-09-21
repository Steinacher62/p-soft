<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BudgetListControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.BudgetListControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table Runat="server" id="Table1" Height="100%">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Height="100%">
		<asp:TableCell>
			<DIV class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
			</DIV>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Height="25" VerticalAlign="Top">
		<asp:TableCell>
            <asp:Button ID="apply" CssClass="Button" Runat="server"></asp:Button>&nbsp;
            <asp:Button ID="clear" CssClass="Button" Runat="server"></asp:Button>&nbsp;
            <asp:Button ID="clearAllocationAll" CssClass="ButtonL" Runat="server"></asp:Button>&nbsp;
            <asp:Button ID="clearAll" CssClass="ButtonL" Runat="server"></asp:Button>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
