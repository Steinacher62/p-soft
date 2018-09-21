<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeistungsWerteUnterdurchschnCtrl.ascx.cs" Inherits="ch.appl.psoft.Report.Controls.LeistungswerteUnterdurchschnCtrl" %>
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