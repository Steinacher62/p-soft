<%@ Control Language="c#" AutoEventWireup="True" Codebehind="LinkGroupControl.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.LinkGroupControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:table id="lnkTable" runat="server" BorderWidth="1" Width="100%" BorderColor="#D6D3CE">
	<asp:TableRow ID="lnkRow">
		<asp:TableCell ID="lnkCell">
			<asp:Label id="captionLabel" runat="server" Font-Bold="true">Label</asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow ID="lnkRowList">
		<asp:TableCell ID="lnkCellList"></asp:TableCell>
	</asp:TableRow>
</asp:table>
