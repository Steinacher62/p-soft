<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditView.ascx.cs" Inherits="ch.appl.psoft.SPZ.Controls.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	function checkFlags() {
		if (typeof(setCheckFlags) == 'function') setCheckFlags('<%=_checkFlags%>','<%=_id%>');
	}
</script>
<input type="hidden" id="checkFld" runat="server" NAME="checkFld">
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<tr>
		<td valign="top">
			<asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
		</td>
	</tr>
	<tr>
		<td>
			<asp:button id="apply" CssClass="Button" runat="server"></asp:button>
		</td>
	</tr>
</table>
