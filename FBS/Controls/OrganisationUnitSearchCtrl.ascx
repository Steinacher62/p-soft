<%@ Control Language="c#" AutoEventWireup="True" Codebehind="OrganisationUnitSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.FBS.Controls.OrganisationUnitSearchCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script  type='text/javascript'>
</script>
<asp:Table ID="listTab" Runat="server" CssClass="Tree" Width="100%">
    <asp:TableRow>
        <asp:TableCell>
            <asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>