<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SimplePageLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.SimplePageLayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="simpleLayoutTable" runat="server" BorderWidth="0px" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow ID="rowError">
        <asp:TableCell>
            <asp:Label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow ID="simpleRow" VerticalAlign="Top">
        <asp:TableCell ID="simpleCell"></asp:TableCell>
    </asp:TableRow>
</asp:Table>
