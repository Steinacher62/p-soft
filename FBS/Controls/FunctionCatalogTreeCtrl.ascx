<%@ Control Language="c#" AutoEventWireup="True" Codebehind="FunctionCatalogTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.FBS.Controls.FunctionCatalogTreeCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	STARTALLOPEN = 0;
	USEICONS = 1;
	WRAPTEXT = 0;
	PERSERVESTATE = 1;
	PERSERVEHIGHLIGHT = 0;
	ICONPATH = "../images/tree/";
	DRAGNODE = 1;
	DROPNODE = 1;
	HIGHLIGHT_DRAGDROP = 1;
	HIGHLIGHT = 1;
	HIGHLIGHT_COLOR = "#374979";
	HIGHLIGHT_BG = "<%=ch.appl.psoft.Global.HighlightColorRGB%>";
	TREE_DEBUG = 0;
    CLOSELINK = 1;
    TARGET_FRAME = "_self";
</script>
<%=buildTree%>
<asp:Table ID="treeTab" Runat="server" CssClass="Tree" Width="100%">
    <asp:TableRow>
        <asp:TableCell>
            <script  type='text/javascript'>
                var nodeIndex;
                for (nodeIndex=0; nodeIndex<nodes.length; nodeIndex++)
                {
                    if (typeof(nodes[nodeIndex]) != "undefined")
                    {
                        initializeDocument(nodes[nodeIndex]);
                        highlightNode(<%=_highlightId%>);
                    }
                }
            </script>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>