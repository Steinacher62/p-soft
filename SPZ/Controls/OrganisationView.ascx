<%@ Control Language="c#" AutoEventWireup="True" Codebehind="OrganisationView.ascx.cs" Inherits="ch.appl.psoft.SPZ.Controls.OrganisationView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	function redrawOrganisation() {
		var w = document.getElementById("organisationFrame");
		w.contentWindow.location.href = "OrganisationTree.aspx?rootId=<%=rootId%>&id=<%=id%>";
	}
</script>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3" >
	<tr height="30" valign="top">
		<td style="PADDING-LEFT:10px">
			<asp:Label ID="oe" Runat="server" CssClass="Detail_Label"></asp:Label><span>&nbsp;&nbsp;&nbsp;</span>
			<select id="selectOE" Runat="server" onchange="javascript: redrawOrganisation();"></select>
		</td>
	</tr>
	<tr>
		<td valign="top" style="PADDING-LEFT:10px" >
			<iframe id="organisationFrame" align="top" marginwidth="0"  frameborder="no" height="100%" width="100%" src="OrganisationTree.aspx?rootId=<%=rootId%>&id=<%=id%>"></iframe>
		</td>
	</tr>
</table>
