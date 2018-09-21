<%@ Control Language="c#" AutoEventWireup="True" Codebehind="KnowledgeDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.KnowledgeDetailCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script language="javascript">    
    var deleteSearch      = "<%=_searchURL%>";    
    var deleteLatest      = "<%=_latestURL%>";
    var deletePrevious    = "<%=_previousURL%>";
    var deleteCurrent     = "<%=_currentURL%>";    
    var currentID         =  <%=_currentId%>;
    var latestID          =  <%=_latestId%>;
    var confirmDeleteAll  = "<%=_confirmAllElements%>";
    var errorDeleteEntry  = "<%=_errorDeleteEntry%>";

    function deleteRowConfirm(rowId, dbId, table)
    {
      if(table == "KNOWLEDGE")
      {
	       if(dbId == latestID)
	       {
	         wsDeleteRowConfirmExt(this, deleteSearch, deleteCurrent, deleteConfirmMessage, errorDeleteEntry, table, rowId, dbId)       
	       }
	       else if(dbId != currentID )
	       {
	         wsDeleteRowConfirm(this, deleteCurrent, deleteConfirmMessage, table, rowId, dbId)
	       }
	       else
	       {
		     wsDeleteRowConfirm(this, deleteLatest, deleteConfirmMessage, table, rowId, dbId)       
	       }
      }
      else
      {
	     wsDeleteRowConfirm(this, deleteCurrent, deleteConfirmMessage, table, rowId, dbId)      
      }
    }        
</script>
<DIV class="ListVariable">
	<asp:Table id="tableThemes" BorderWidth="0" runat="server" Width="100%" CellPadding="0" CellSpacing="2">
		<asp:TableRow>
			<asp:TableCell ID="cellDescription" Runat="server" CssClass="themeText" Width="100%" ColumnSpan="4">
				<asp:Label ID="labelDescription" Runat="server" CssClass="themeText"></asp:Label>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<asp:Table id="tableAdditionals" Runat="server" BorderWidth="0" Width="100%" CellPadding="0" CellSpacing="2">
		<asp:TableRow>
			<asp:TableCell CssClass="themeTitle" ColumnSpan="4">
				<asp:Label ID="labelDocuments" Runat="server" CssClass="themeTitle"></asp:Label>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ColumnSpan="4">
				<asp:Table id="tableDocuments" BorderWidth="0" runat="server" CellPadding="0" CellSpacing="2"></asp:Table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="themeTitle" ColumnSpan="4">
				<asp:Label ID="labelHistory" Runat="server" CssClass="themeTitle"></asp:Label>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ColumnSpan="4">
				<asp:Table id="tableHistory" BorderWidth="0" runat="server" CellPadding="0" CellSpacing="2"></asp:Table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="themeTitle" ColumnSpan="4">
				<asp:Label ID="labelRegistry" Runat="server" CssClass="themeTitle"></asp:Label>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ColumnSpan="4">
				<asp:Table id="tableRegistry" BorderWidth="0" runat="server" CellPadding="0" CellSpacing="2"></asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</DIV>
