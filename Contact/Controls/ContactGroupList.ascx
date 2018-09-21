<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactGroupList.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactGroupList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteURL = "ContactGroupDetail.aspx?personID=<%=PersonID%>&OrderColumn=<%=OrderColumn%>&OrderDir<%=OrderDir%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "CONTACT_GROUP", rowId, dbId)
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