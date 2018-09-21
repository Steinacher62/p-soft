<%@ Control Language="c#" AutoEventWireup="True" Codebehind="CatalogCtrl.ascx.cs" Inherits="ch.appl.psoft.FBW.Controls.CatalogCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	STARTALLOPEN = 0
	USEICONS = 1
	WRAPTEXT = 0
	PERSERVESTATE = 1
	PERSERVEHIGHLIGHT = 0
	ICONPATH = "../images/tree/" 
	DRAGNODE = 0;
	DROPNODE = 0;
	HIGHLIGHT_DRAGDROP = 1;
	HIGHLIGHT = 1;
	HIGHLIGHT_COLOR = "#374979";
	HIGHLIGHT_BG = "<%=ch.appl.psoft.Global.HighlightColorRGB%>";
	TREE_DEBUG = 0;
    CLOSELINK = 1;
    DRAGITEM = 0;
    DROPITEM = 0;
	DOWNLOAD_ENABLE = 0;
    TARGET_FRAME = "_self"
</script>
<%=buildTree%>
<asp:Table id="catalogTable" runat="server" Width="100%" Height="100%">
    <asp:TableRow Width="100%" ID="ablageRow" Height="100%">
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="treeCell" RowSpan="2">
            <asp:Table ID="treeTab" Runat="server" CssClass="Tree" Width="100%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="treeTitle" runat="server" CssClass="section_title"></asp:label>
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
        </asp:TableCell>
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell">
            <asp:Table id="detailMainTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="detailTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Height="10"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top" Height="100%">
                    <asp:TableCell Width="100%">
                        <div class="ListVariable">
                            <asp:Table id="detailTab" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
