<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PendenzenMeasureDetail.ascx.cs" Inherits="ch.appl.psoft.Tasklist.Controls.PendenzenMeasureDetail" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script  type='text/javascript'>
    var deleteMeasureConfirmMessage = "<%=deleteMessage%>";

    function deleteMeasureConfirm(dbId)
    {
        if (window.confirm(deleteMeasureConfirmMessage))
        {
            wsDeleteRow("MEASURE",dbId);
            location.reload(true);
        }
    }
</script>
<table class="List" border="0" Width="100%" CellSpacing="0" CellPadding="3">
    <tr id="detailDataRow" runat="server">
        <td>
            <asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
        </td>
    </tr>
</table>
