<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CopySearchView.ascx.cs" Inherits="ch.appl.psoft.MbO.Controls.CopySearchView" %>
<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="100%" border="0">
    <tr>
        <td height="20"></td>
    </tr>
    <tr>
        <td><asp:Label ID="title" Runat="server" CssClass="section_title"></asp:Label></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:table id="searchTab" runat="server" CssClass="InputMask" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
    </tr>
</TABLE>