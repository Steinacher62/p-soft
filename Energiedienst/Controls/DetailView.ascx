<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DetailView.ascx.cs" Inherits="ch.appl.psoft.Energiedienst.Controls.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteDetailConfirmMessage = "<%=_deleteDetailMessage%>";
    var backDetailURL = "<%=_deleteDetailURL%>";

    function deleteDetailConfirm(dbId)
    {
        if (window.confirm(deleteDetailConfirmMessage))
        {
            wsDeleteRow("OBJECTIVE",dbId);
            location.href = backDetailURL;

        }
    }
</script>
<script src="ratingScript.js"></script>
<link href="ratingStyle.css" rel="stylesheet" />
<iframe id="frameA" style="DISPLAY: none">
	<script  type='text/javascript'>
		var a = "kurt";
	</script>
</iframe>
<iframe id="frameB" style="DISPLAY: none">
	<script  type='text/javascript'>
		var a = "Wymann";
	</script>
</iframe>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<tr>
		<td valign="top">
			<asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
		</td>
	</tr>
</table>
