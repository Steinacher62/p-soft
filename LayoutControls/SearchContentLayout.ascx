<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SearchContentLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.SearchContentLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:table id="slTable" Width="100%" Height="100%" BorderWidth="0px" CellSpacing="1" CellPadding="1" runat="server" style="min-height: 98%">
	<asp:TableRow Width="100%" VerticalAlign="Top" HorizontalAlign="Left" ID="searchRow" Runat="server" Height="40%">
		<asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" ID="searchCell">
            <div ID="searchCellDiv" runat="server" class="ListVariable"></div>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow id="listRow" Runat="server" Width="100%" VerticalAlign="Top" HorizontalAlign="Left">
		<asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" ID="listCell" BorderWidth="1px">
          <p align="center" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; CURSOR: pointer; PADDING-TOP: 0px"><asp:Image ID="searchMinimizerImg" Runat="server"></asp:Image></p>
		</asp:TableCell>
	</asp:TableRow>
</asp:table>
