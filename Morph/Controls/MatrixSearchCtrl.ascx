<%@ Control Language="c#" AutoEventWireup="True" Codebehind="MatrixSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Morph.Controls.MatrixSearchCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:Panel ID="Panel1" runat="server" DefaultButton="apply">
<TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
    <tr>
        <td height="20"></td>
    </tr>
    <tr>
        <td><asp:table id="searchTab" CssClass="InputMask" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button></td>
    </tr>
</TABLE>
    </asp:Panel>