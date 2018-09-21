<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactAddCtrl.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactAddCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="0" cellPadding="0">
	<tr>
		<td height="20"></td>
	</tr>
	<TR>
		<TD style="LEFT: 2px; POSITION: relative"><asp:label id="pageTitle" CssClass="section_title" runat="server"></asp:label></TD>
	</TR>
	<tr>
		<td style="LEFT: 2px; POSITION: relative"><asp:Label ID="contactTabTitle" Runat="server" CssClass="section_subTitle"></asp:Label></td>
		<td style="LEFT: 2px; POSITION: relative"><asp:Label ID="addressTabTitle" Runat="server" CssClass="section_subTitle"></asp:Label></td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
	<tr>
		<td valign="top"><asp:table id="contactTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table></td>
		<td valign="top"><asp:table id="addressTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table></td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
	<tr>
		<td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
	</tr>
</table>
