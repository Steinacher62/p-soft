<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PsoftLinksControl.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.PsoftLinksControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="Psoft" TagName="LinkGroupControl" Src="LinkGroupControl.ascx" %>
<asp:Table id="tblLinks" runat="server" CellPadding="2" CellSpacing="0" Width="100%">
	<asp:TableRow Width="100%" VerticalAlign="Top" HorizontalAlign="Left" ID="rowFunc">
		<asp:TableCell VerticalAlign="Top" Width="100%" HorizontalAlign="Left" ID="celFunc">
			<Psoft:LinkGroupControl id="lLinks1" runat="server"></Psoft:LinkGroupControl>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Width="100%" VerticalAlign="Top" HorizontalAlign="Left" ID="rowLinks">
		<asp:TableCell VerticalAlign="Top" Width="100%" HorizontalAlign="Left" ID="celLinks">
			<Psoft:LinkGroupControl id="lLinks2" runat="server"></Psoft:LinkGroupControl>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Width="100%" VerticalAlign="Top" HorizontalAlign="Left" ID="rowNews">
		<asp:TableCell VerticalAlign="Top" Width="100%" HorizontalAlign="Left" ID="celNews">
			<Psoft:LinkGroupControl id="lLinks3" runat="server"></Psoft:LinkGroupControl>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
