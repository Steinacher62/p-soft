<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PersonJournalList.ascx.cs" Inherits="ch.appl.psoft.Person.Controls.PersonJournalList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>



<script language=javascript>
    var deleteURL = "PersonJournalDetail.aspx?contextID=<%=_contextID%>&OrderColumn=<%=OrderColumn%>&OrderDir<%=OrderDir%>";
    
    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "PERSON_JOURNAL", rowId, dbId)
    }
</script>
<asp:table id=Table1 Height="100%" Runat="server">
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
