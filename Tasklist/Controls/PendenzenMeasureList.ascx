<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PendenzenMeasureList.ascx.cs" Inherits="ch.appl.psoft.Tasklist.Controls.PendenzenMeasureList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "<%=DeleteURL%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "MEASURE", rowId, dbId)
    }

    function listDragStart(context) {        
	    if (document.getElementById && window.event.srcElement)
	    {
            var dragData = window.event.dataTransfer;
		    var sourceImg = window.event.srcElement;
		    
		    var id = sourceImg.id;
		    var obj = document.getElementById(id);
			
            if (dragData && obj) {
				//alert("obj="+id);
                dragData.setData('Text', context+"ID="+id);
                dragData.effectAllowed = 'copyMove';
            }
            
	    }
    }
    
    function listDragEnd() {
        window.event.dataTransfer.clearData();
    }

</script>
<table class="List" border="0" Width="100%" CellSpacing="0" CellPadding="3">
	<tr>
		<td>
			<table>
				<tr>
					<td><asp:label id="MeasureListTitle" runat="server" CssClass="section_title"></asp:label></td>
				</tr>
				<tr>
				<td>
					<div class="ListVariable">
						<asp:Table id="measureList" runat="server" BorderWidth="0"></asp:Table>
					</div>
				</td>
				</tr>
			</table>
		</td>
		<td valign="top" align="right">
			<table>
				<tr>
					<td align="left"><asp:checkbox id="CBShowDone" runat="server" AutoPostBack="True" oncheckedchanged="CBShowDone_CheckedChanged"></asp:checkbox></td>
				</tr>
				<tr id="CBShowSubsTR" runat="server">
					<td align="left"><asp:checkbox id="CBShowSubs" runat="server" AutoPostBack="True" oncheckedchanged="CBShowSubs_CheckedChanged"></asp:checkbox></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr class="List" valign="bottom">
		<td colspan="2">
			<asp:Button ID="next" Runat="server" CssClass="Button" Visible="False"></asp:Button>
		</td>
	</tr>
</table>
