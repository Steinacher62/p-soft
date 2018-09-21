<%@ Register tagprefix="PSOFT" tagname="lockFile" src="../../Common/LockFile.ascx" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DokAblageTreeview.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.DokAblageTreeview" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<script src="../JavaScript/Tree/ua.js"></script>
<script src="../JavaScript/Tree/ftiens4.js"></script>
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
    DRAGITEM = 1;
    DROPITEM = 1;
	COPY = copy;
	MOVE = move;
	DOWNLOAD_ENABLE = 0;
    TARGET_FRAME = "_self"
    
	function move(sourceType, sourceId, targetId)
	{
		var ownerColumn = "PARENT_ID";
		var tableName = "FOLDER";
		
		if (sourceType == "ListID")
		{
		    var idx = sourceId.indexOf("_");
		    tableName = sourceId.substring(0, idx);
		    sourceId = sourceId.substring(idx+1);
		    if (tableName == "DOCUMENT")
		        ownerColumn = "FOLDER_ID";
		}

		wsMoveRow(ownerColumn, targetId, tableName, sourceId);
		location.reload(true);

		return false;
	}
	
	function copy(sourceType, sourceId, targetId)
	{
		var tableName = "FOLDER";
		
		if (sourceType == "ListID")
		{
		    var idx = sourceId.indexOf("_");
		    tableName = sourceId.substring(0, idx);
		    sourceId = sourceId.substring(idx+1);
		}
		
		wsCopyRow("FOLDER", targetId, tableName, sourceId, true);
		location.reload(true);

		return null;
	}
	
	// activeX disable onerror
    function nop(){}
    function activexDisable() {
         linkDiasable(document.getElementById('getDocumentLink'));
    }
    function linkDiasable(obj){
        var tooltip = '<%=ActiveXErrorTooltip%>';
        if (obj != null){
            obj.disabled = true;
            obj.href = 'javascript: nop()';
            obj.title = tooltip;
         }
    }
</script>
<%=buildTree%>
<script  type='text/javascript'>
    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, "Clipboard.aspx?id=<%=ClipboardID%>&ownerTable=<%=OwnerTable%>&selectedFolderID=<%=SelectedFolderID%>&orderColumn=<%=OrderColumn%>&orderDir=<%=OrderDir%>&documentAddEnable=<%=DocumentAddEnable%>&registryEnable=<%=RegistryEnable%>", deleteConfirmMessage, "FOLDERDOCUMENTV", rowId, dbId)
    }
    
    function listDragStart()
    {
	    var copyEle = window.event.ctrlKey;
        
	    if (document.getElementById && window.event.srcElement)
	    {
            var dragData = window.event.dataTransfer;
		    var sourceImg = window.event.srcElement;
		    var id = sourceImg.id;
		    var obj = document.getElementById(id);

            if (dragData && obj)
            {
                dragData.setData('Text', "ListID="+id);
                dragData.effectAllowed = 'copyMove';
            }
	    }
    }
    
    function listDragEnd()
    {
        window.event.dataTransfer.clearData();
    }
</script>
<!-- <PSOFT:lockFile runat="server" id="LockFile1"/> -->
<asp:Table id="ablageTable" runat="server" Width="100%" Height="100%">
	<asp:TableRow Width="100%" ID="ablageRow" Height="100%">
		<asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="treeCell" RowSpan="2">
            <asp:Table ID="treeTab" Runat="server" CssClass="Tree" Width="100%">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="ShelfTreeTitle" runat="server" CssClass="section_title"></asp:label>
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
                                    highlightNode(<%=_highlightId%>);
                                }
                            }
                        </script>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
		</asp:TableCell>
		<asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell">
            <asp:Table id="documentTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="ShelfListTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Height=10>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top" Height="100%">
                    <asp:TableCell>
                        <div class="ListVariable">
                            <asp:Table id="documentList" runat="server" BorderWidth="0"></asp:Table>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow CssClass="List" VerticalAlign="Bottom">
                    <asp:TableCell HorizontalAlign="Center">
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
