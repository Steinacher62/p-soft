<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PhaseDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.PhaseDetailCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	var deletePhaseConfirmMessage = "<%=deleteMessage%>";
    function deletePhaseConfirm(dbId)
    {
        if (window.confirm(deletePhaseConfirmMessage))
        {
            wsDeleteRow("PHASE",dbId);
            location.href="<%=PostDeleteURL%>";
        }
    }
</script>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<tr id="detailDataRow" runat="server">
		<td valign="top">
			<asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
		</td>
	</tr>
</table>