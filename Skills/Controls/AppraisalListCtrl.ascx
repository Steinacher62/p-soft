<%@ Control Language="c#" AutoEventWireup="True" Codebehind="AppraisalListCtrl.ascx.cs" Inherits="ch.appl.psoft.Skills.Controls.AppraisalListCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "AppraisalDetail.aspx?personID=<%=PersonID%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "SKILLS_APPRAISAL", rowId, dbId)
    }
</script>
<asp:Table Runat="server" id="Table1">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell>
			<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>