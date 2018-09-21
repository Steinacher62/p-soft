<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SkillsCatalogTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Skills.Controls.SkillsCatalogTreeCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	STARTALLOPEN = 0;
	USEICONS = 1;
	WRAPTEXT = 0;
	PERSERVESTATE = <%=PerserveState%>;
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
            <asp:label id="SkillGroupTreeTitle" runat="server" CssClass="section_title"></asp:label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell Height="10"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <script  type='text/javascript'>
                var nodeIndex;
                for (nodeIndex=0; nodeIndex<nodes.length; nodeIndex++)
                {
                    if (typeof(nodes[nodeIndex]) != "undefined")
                    {
                        initializeDocument(nodes[nodeIndex]);
                    }
                }
                <%=HighLightNode%>
            </script>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>