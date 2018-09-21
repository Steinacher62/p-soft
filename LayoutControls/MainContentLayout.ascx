<%@ Control Language="c#" AutoEventWireup="True" Codebehind="MainContentLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.MainContentLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:table id="mainContentTable" Width="100%" Height="100%" BorderWidth="0px" CellSpacing="1" CellPadding="1" runat="server">
	<asp:TableRow Width="100%" VerticalAlign="Middle" HorizontalAlign="Center" ID="row1" Height="30%">
		<asp:TableCell VerticalAlign="Middle" HorizontalAlign="Center" ID="linksCell"></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Width="100%" Height="70%" VerticalAlign="Top" HorizontalAlign="Center">
		<asp:TableCell VerticalAlign="Top" HorizontalAlign="Center" ID="listeCell" CssClass="listeCell"></asp:TableCell>
	</asp:TableRow>
</asp:table>
