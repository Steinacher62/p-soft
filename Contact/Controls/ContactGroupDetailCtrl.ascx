<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactGroupDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactGroupDetailCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="0" cellPadding="0">
    <TR>
        <TD style="LEFT: 2px; POSITION: relative"><asp:label id="pageTitle" CssClass="section_title" runat="server"></asp:label></TD>
    </TR>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td valign=top><asp:table id="contactGroupTab" runat="server" CssClass="Detail" BorderWidth="0"></asp:table></td>
    </tr>
</table>
