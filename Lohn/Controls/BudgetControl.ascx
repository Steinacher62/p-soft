<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BudgetControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.BudgetControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
            <asp:Button ID="done" CssClass="Button" Runat="server" onclick="done_Click"></asp:Button>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
