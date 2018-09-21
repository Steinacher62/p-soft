<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ProjectSummaryCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.ProjectSummaryCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    function SwapDisplay(controlID, anchor)
    {
        var Control = document.getElementById(controlID);
        var Images = null;
        if (Control != null &&  Control.style && anchor != null)
        {
            if (Control.style.display == "none")
            {
                Control.style.display = "inline";
                Images = anchor.getElementsByTagName("img");
                Images[0].src = "<%=BaseURL%>/images/minus.gif";
            }
            else
            {
                Control.style.display = "none";
                Images = anchor.getElementsByTagName("img");
                Images[0].src = "<%=BaseURL%>/images/plus.gif";
            }
        }
    }
</script>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<tr id="detailDataRow" runat="server">
		<td valign="top">
			<asp:table id="SummaryTable" CssClass="InputMask" CellPadding="3" CellSpacing="0" runat="server" BorderWidth="0" style="WORD-WRAP:break-word"></asp:table>
		</td>
	</tr>
</table>
