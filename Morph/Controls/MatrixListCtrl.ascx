<%@ Control Language="c#" AutoEventWireup="True" Codebehind="MatrixListCtrl.ascx.cs" Inherits="ch.appl.psoft.Morph.Controls.MatrixListCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:table id="Table1" Height="100%" Runat="server">
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
    <asp:TableRow VerticalAlign="Bottom" ID="ButtonRow" Visible="True" Runat="server">
        <asp:TableCell>
            <asp:Button ID="next" Runat="server" CssClass="Button" Enabled="False" Visible="False"></asp:Button>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell Height="10"></asp:TableCell>
    </asp:TableRow>
</asp:table>