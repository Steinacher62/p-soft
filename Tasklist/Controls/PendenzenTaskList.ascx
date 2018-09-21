<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PendenzenTaskList.ascx.cs" Inherits="ch.appl.psoft.Tasklist.Controls.PendenzenTaskList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "<%=DeleteURL%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "TASKLIST", rowId, dbId)
    }
</script>

<table class="List" border="0" Width="100%" CellSpacing="0" CellPadding="3">
	<tr>
		<td>
			<asp:label id="TasklistListTitle" runat="server" CssClass="section_title"></asp:label>
		</td>
		<td align="right"><asp:checkbox id="CBShowDone" runat="server" AutoPostBack="True"></asp:checkbox></td>
	</tr>
	<tr>
		<td colspan="2" Height="10"></td>
	</tr>
	<tr valign="top">
		<td colspan="2">
			<div class="ListVariable">
				<asp:Table id="tasklistList" runat="server" BorderWidth="0" CssClass="List" CellSpacing="0" CellPadding="2"></asp:Table>
			</div>
		</td>
	</tr>
	<tr class="List" valign="bottom">
		<td colspan="2">
			<asp:Button ID="next" Runat="server" CssClass="Button" Visible="False"></asp:Button>
		</td>
	</tr>
</table>
