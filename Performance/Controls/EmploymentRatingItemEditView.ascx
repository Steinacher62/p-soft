<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EmploymentRatingItemEditView.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.EmploymentRatingItemEditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<TABLE id="Table1" cellSpacing="0" cellPadding="2" border="0">
	<TR>
		<TD><asp:label id="TITLE_VALUE" CssClass="section_title" runat="server"></asp:label></TD>
	</TR>
</TABLE>
<asp:table id="editTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
<asp:button id="apply" CssClass="Button" Runat="server"></asp:button>
