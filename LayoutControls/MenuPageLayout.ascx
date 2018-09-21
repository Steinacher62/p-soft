<%@ Control Language="c#" AutoEventWireup="True" Codebehind="MenuPageLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.MenuPageLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="menuLayoutTable" runat="server" BorderWidth="0px" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow ID="rowError">
        <asp:TableCell>
            <asp:Label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow ID="menuRow" VerticalAlign="Top">
        <asp:TableCell ID="menuCell" Width="100%"></asp:TableCell>
    </asp:TableRow>
</asp:Table>
