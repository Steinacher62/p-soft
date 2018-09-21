<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ProjectDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.ProjectDetailCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	var deleteProjectConfirmMessage = "<%=deleteMessage%>";
    function deleteProjectConfirm(dbId)
    {
        if (window.confirm(deleteProjectConfirmMessage))
        {
            wsDeleteRow("PROJECT",dbId);
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
