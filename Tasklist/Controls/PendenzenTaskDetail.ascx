<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PendenzenTaskDetail.ascx.cs" Inherits="ch.appl.psoft.Tasklist.Controls.PendenzenTaskDetail" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script  type='text/javascript'>
    var deleteTasklistConfirmMessage = "<%=deleteMessage%>";

    function deleteTasklistConfirm(dbId)
    {
        if (window.confirm(deleteTasklistConfirmMessage))
        {
            wsDeleteRow("TASKLIST",dbId);
            location.reload(true);
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
