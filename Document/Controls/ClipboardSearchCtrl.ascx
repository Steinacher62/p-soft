<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ClipboardSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.ClipboardSearchCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	STARTALLOPEN = 0
	USEICONS = 1
	WRAPTEXT = 0
	PERSERVESTATE = 0
	ICONPATH = "../images/tree/" 
	DRAGNODE = 0;
	DROPNODE = 0;
	HIGHLIGHT_DRAGDROP = 0;
	HIGHLIGHT = 1;
	TREE_DEBUG = 0;
    CLOSELINK = 1;
</script>
<%=buildTree%>
<input type="hidden" id="registryFlags" runat="server" NAME="registryFlags">
<TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
	<tr>
		<td height="20"></td>
	</tr>
	<tr>
		<td valign="top"><asp:table id="searchTab" CssClass="InputMask" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
		<td valign="top">
			<table class="Tree" border="0" cellSpacing="0" cellPadding="2">
				<tr>
					<td style="PADDING-LEFT:10px" vAlign="top">
						<script  type='text/javascript'>
                            if (typeof(nodes) != "undefined")
                            {
                                for (var nodeIndex = 0; nodeIndex < nodes.length; nodeIndex++)
                                {
                                    if (typeof(nodes[nodeIndex]) != "undefined")
                                    {
                                        initializeDocument(nodes[nodeIndex]);
                                    }
                                }
                            }
						</script>
					</td>
					<td vAlign="top">
						<asp:RadioButtonList ID="registryRelation" Runat="server" RepeatDirection="Vertical"></asp:RadioButtonList>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
	<tr>
		<td><asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button></td>
	</tr>
</TABLE>
