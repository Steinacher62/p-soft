<%@ Control Language="c#" AutoEventWireup="True" Codebehind="JournalList.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.JournalList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	//cst, 090713: removed journalID from deleteURL
    var deleteURL = "JournalDetail.aspx?contactID=<%=ContactID%>&OrderColumn=<%=OrderColumn%>&OrderDir<%=OrderDir%>";
    
    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "JOURNAL_CONTACT_V", rowId, dbId)
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