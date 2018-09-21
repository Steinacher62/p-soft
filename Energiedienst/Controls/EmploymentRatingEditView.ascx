<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EmploymentRatingEditView.ascx.cs" Inherits="ch.appl.psoft.Energiedienst.Controls.EmploymentRatingEditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<link href="../Energiedienst/PerformanceEditStyle.css" rel="stylesheet" />
<script src="../Energiedienst/PerformanceRatingEditScript.js"></script>

<TABLE id="Table1" cellSpacing="0" cellPadding="2" border="0">
	<TR>
		<TD><asp:label id="TITLE_VALUE" CssClass="section_title" runat="server"></asp:label></TD>
	</TR>
</TABLE>
<asp:table id="editTab" CssClass="InputMask" runat="server" BorderWidth="1" GridLines=Both></asp:table>
<asp:Table id="listTab" CssClass="InputMask"  BorderWidth="0" runat="server" Width="100%"></asp:Table>
<asp:table id="argumentTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
<asp:button id="apply" CssClass="Button" Runat="server" onclick="apply_Click"></asp:button>
