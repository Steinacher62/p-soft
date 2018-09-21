<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateNovisReportCtrl.ascx.cs" Inherits="ch.appl.psoft.Novis.Controls.CreateNovisReportCtrl" %>

<TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
    <tr>
        <td height="20"></td>
    </tr>
    <tr>
        <td><asp:table id="searchTab" CssClass="InputMask" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
    </tr>
    <tr>
        <td>
            Template-Karte wählen: <telerik:RadComboBox ID="chooseMaster" runat="server" Width="400px"></telerik:RadComboBox>
        </td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:Button ID="apply" Runat="server" CssClass="Button">
            </asp:Button></td>
    </tr>
</TABLE>
