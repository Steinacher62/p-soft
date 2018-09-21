<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeistungsWerteUnterdurchschnListCtrl.ascx.cs" Inherits="ch.appl.psoft.Report.Controls.LeistungsWerteUnterdurchschnListCtrl" %>
<script  type='text/javascript'>
</script>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<TBODY>
		<tr valign="top" Height="100%">
			<td colspan="2">
				<asp:Table id="listTab" runat="server" BorderWidth="0" CssClass="List" CellSpacing="0" CellPadding="2"></asp:Table>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Button id="next" runat="server" CssClass="Button"></asp:Button>
			</td>
		</tr>
	</TBODY>
</table>