<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactSearch.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactSearch" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
	<tr>
		<td height="20"></td>
	</tr>
    <tr>
        <td style="LEFT: 2px; POSITION: relative"><asp:Label ID="contactTitle" Runat="server" CssClass="section_subTitle"></asp:Label></td>
        <td style="LEFT: 2px; POSITION: relative"><asp:Label ID="journalTitle" Runat="server" CssClass="section_subTitle"></asp:Label></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td valign="top"><asp:table id="searchTabContact" CssClass="InputMask" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
        <td valign="top"><asp:table id="searchTabJournal" CssClass="InputMask" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button></td>
    </tr>
</TABLE>
