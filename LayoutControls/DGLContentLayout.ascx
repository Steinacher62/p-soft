<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DGLContentLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.DGLContentLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:table id="dglTable" Width="100%" Height="100%" BorderWidth="0px" CellSpacing="0" CellPadding="0" runat="server">
    <asp:TableRow ID="detailRow" Runat="server" Width="100%" VerticalAlign="Top" HorizontalAlign="Left" Height="50%">
        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" ID="detailCell">
            <div ID="detailCellDiv" runat="server" class="ListVariable"></div>
        </asp:TableCell>
        <asp:TableCell VerticalAlign="Middle" ID="linksMinimizerCell" RowSpan="2" Width="5">
            <asp:Image ID="linksMinimizerImg" Runat="server" style="cursor:hand;"></asp:Image>
        </asp:TableCell>
        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Right" ID="linksCell" RowSpan="2" Width="20%">
            <div ID="linksCellDiv" runat="server" class="ListVariable"></div>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow ID="groupRow" Runat="server" Width="100%" VerticalAlign="Top" HorizontalAlign="Left" CssClass="groupRow">
        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Left" ID="groupCell" BorderWidth="1px" CssClass="groupCell">
            <p align="center" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; CURSOR: pointer; PADDING-TOP: 0px"><asp:Image ID="groupMinimizerImg" Runat="server"></asp:Image></p>
            <div ID="groupCellDiv" runat="server" class="ListVariable"></div>
        </asp:TableCell>
    </asp:TableRow>
</asp:table>
