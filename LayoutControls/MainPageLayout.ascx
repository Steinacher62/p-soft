<%@ Control Language="c#" AutoEventWireup="True" Codebehind="MainPageLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.MainPageLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="mainLayoutTable" runat="server" BorderWidth="0px" CellPadding="2" CellSpacing="2" Height="100%" Width="100%">
	<asp:TableRow ID="rowError" Height="1%" Width="100%" VerticalAlign="Middle" HorizontalAlign="Justify">
		<asp:TableCell>
			<asp:Label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Width="100%" VerticalAlign="Top" HorizontalAlign="Justify" ID="mainRow" Height="99%">
		<asp:TableCell VerticalAlign="Top" HorizontalAlign="Justify" ID="mainCell">
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
