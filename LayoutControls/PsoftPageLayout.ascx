<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PsoftPageLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.PsoftPageLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="baseLayoutTable" runat="server" BorderWidth="0px" CellPadding="0" CellSpacing="0" Height="100%" Width="100%">
    <asp:TableRow ID="rowError" Height="1%" Width="100%" VerticalAlign="Middle" HorizontalAlign="Justify">
        <asp:TableCell>
            <asp:Label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
        </asp:TableCell>
        <asp:TableCell>
            <iframe id="infoBox" name="infoBox" frameborder="0" scrolling="no" style="position:absolute; z-index:100; display:none"></iframe>
            <input type="hidden" id="infoBoxX">
            <input type="hidden" id="infoBoxY">
            <iframe id="propertyBox" name="propertyBox" frameborder="0" scrolling="no" style="position:absolute; z-index:101; display:none"></iframe>
            <input type="hidden" id="propertyBoxX">
            <input type="hidden" id="propertyBoxY">
            <iframe id="calendarBox" name="calendarBox" frameborder="0" scrolling="no" width="180" height="190" style="position:absolute; z-index:101; display:none"></iframe>
            <input type="hidden" id="calendarBoxX">
            <input type="hidden" id="calendarBoxY">
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Width="100%" VerticalAlign="Middle" ID="titleRow" Height="1%">
        <asp:TableCell VerticalAlign="Top" HorizontalAlign="left" Width="80%" ID="titleCell">
            <asp:Label ID="titleLabel" Runat="server" CssClass="page_title" Height="20"></asp:Label>
        </asp:TableCell>
        <asp:TableCell VerticalAlign="Middle" HorizontalAlign="Right" Width="20%" ID="baseButtonCell">
            <asp:Label ID="titleRight" Runat="server" CssClass="page_title" Height="20" BorderWidth="0px"></asp:Label>
            <asp:ImageButton runat="server" ID="btnAuthorisations" ImageUrl="../images/icon_authorisations.gif" Visible="False"></asp:ImageButton>
            <asp:Image runat="server" ID="imageAuthorisations" ImageUrl="../images/icon_authorisations.gif" style="cursor:grabbing;" Visible="False"></asp:Image>
            <asp:ImageButton runat="server" ID="btnRegistryEntries" ImageUrl="../images/icon_registryentries.gif" Visible="False"></asp:ImageButton>
            <asp:Image runat="server" ID="imageRegistryEntries" ImageUrl="../images/icon_registryentries.gif" style="cursor:grabbing;" Visible="False"></asp:Image>
            <asp:ImageButton runat="server" ID="btnPrint" ImageUrl="../images/icon_print.gif" Visible="False"></asp:ImageButton>
            <asp:Image runat="server" ID="imagePrint" ImageUrl="../images/icon_print.gif" style="cursor:grabbing;" Visible="False"></asp:Image>
            <asp:ImageButton runat="server" ID="btnList" ImageUrl="../images/icon_list.gif" Visible="False"></asp:ImageButton>
            <asp:Image runat="server" ID="imageList" ImageUrl="../images/icon_list.gif" style="cursor:grabbing;" Visible="False"></asp:Image>
            <asp:ImageButton Runat="server" ID="btnExcel" ImageUrl="../images/icon_excel.gif" Visible="False"></asp:ImageButton>
            <asp:Image Runat="server" ID="imageExcel" ImageUrl="../images/icon_excel.gif" style="cursor:grabbing;" Visible="False"></asp:Image>
            <asp:ImageButton Runat="server" ID="btnVisio" ImageUrl="../images/icon_visio.png" Visible="False"></asp:ImageButton>
            <asp:Image Runat="server" ID="imageVisio" ImageUrl="../images/icon_visio.png" style="cursor:grabbing;" Visible="False"></asp:Image>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Width="100%" VerticalAlign="Middle" HorizontalAlign="Center" ID="dividerRow">
        <asp:TableCell ColumnSpan="2" VerticalAlign="Top" Width="100%" Height="1" ID="dividerCell" BackColor="#B4BBCF"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Width="100%" VerticalAlign="Top" HorizontalAlign="Left" ID="baseLayoutRow" Height="98%">
        <asp:TableCell ColumnSpan="2" VerticalAlign="Top" HorizontalAlign="Left" ID="baseLayoutCell"></asp:TableCell>
    </asp:TableRow>
</asp:Table>
