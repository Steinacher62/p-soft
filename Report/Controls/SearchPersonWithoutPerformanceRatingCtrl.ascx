<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SearchPersonWithoutPerformanceRatingCtrl.ascx.cs" Inherits="ch.appl.psoft.Report.Controls.SearchPersonWithoutPerformanceRatingCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr>
		<td height="20"></td>
	</tr>
	<tr>
		<td><asp:table id="searchTab" runat="server" CssClass="InputMask" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
		<td align="right" valign="top"><asp:checkbox id="CBShowOpposite" runat="server" AutoPostBack="True"></asp:checkbox></td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
	<tr>
		<td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
	</tr>
</TABLE>
