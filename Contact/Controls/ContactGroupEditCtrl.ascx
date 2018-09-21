<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactGroupEditCtrl.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactGroupEditCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="0" cellPadding="0">
    <TR>
        <TD style="LEFT: 2px; POSITION: relative"><asp:label id="pageTitle" CssClass="section_title" runat="server"></asp:label></TD>
    </TR>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td valign=top><asp:table id="contactGroupTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
    </tr>
</table>
