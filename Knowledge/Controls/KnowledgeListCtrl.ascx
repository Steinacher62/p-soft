<%@ Control Language="c#" AutoEventWireup="True" Codebehind="KnowledgeListCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.KnowledgeListCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<tr>
		<td>
			<asp:label id="pageTitle" runat="server" CssClass="section_title"></asp:label>
		</td>
	</tr>
	<tr>
		<td colspan="2" Height="0"></td>
	</tr>
	<tr valign="top" Height="100%">
		<td colspan="2">
			<!-- <div class="ListVariable"> -->
				<asp:Table id="listTable" EnableViewState="False" runat="server" BorderWidth="0" CssClass="List" CellSpacing="0" CellPadding="2"></asp:Table>
			<!-- </div> -->
		</td>
	</tr>
	<tr class="List" valign="top">
		<td colspan="2" valign="top">
			<asp:Button ID="next" Runat="server" CssClass="Button" Visible="False" onclick="next_Click"></asp:Button>
		</td>
	</tr>
</table>
