<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PhaseList.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.PhaseList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteURL = "<%=PostDeleteURL%>";

    function deleteRowConfirm(rowId,dbId) {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "PHASE", rowId, dbId)
    }

    function listDragStart(context) {        
	    if (document.getElementById && window.event.srcElement)
	    {
            var dragData = window.event.dataTransfer;
		    var sourceImg = window.event.srcElement;
		    
		    var id = sourceImg.id;
		    var obj = document.getElementById(id);
			
            if (dragData && obj) {
                dragData.setData('Text', context + "ID=" + id);
                dragData.effectAllowed = 'copyMove';
            }
	    }
    }
    
    function listDragEnd() {
        window.event.dataTransfer.clearData();
    }

</script>
<asp:Table Runat="server" id="Table1" Height="100%">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Height="100%">
		<asp:TableCell>
			<DIV class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
			</DIV>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>