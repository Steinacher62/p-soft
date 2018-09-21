<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TasklistTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Tasklist.Controls.TasklistTreeCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	COPY = copy;
	MOVE = move;
	
	function move(sourceType, sourceId, targetId)
	{
		if (sourceType == "TreeID") {
			wsMoveRow("PARENT_ID", targetId, "TASKLIST", sourceId);
			location.reload(true);
		}
		else if (sourceType == "MeasureID") {
			wsMoveRow("TASKLIST_ID", targetId, "MEASURE", sourceId);
		}
		else return false;

		location.reload(true);
		return false;
	}
	
	function copy(sourceType, sourceId, targetId)
	{
		if (sourceType == "TreeID") {
			wsCopyRow("TASKLIST", targetId, "TASKLIST", sourceId, true);
		}
		else if (sourceType == "MeasureID") {
			wsCopyRow("TASKLIST", targetId, "MEASURE", sourceId, true);
		}
		else return null;
		location.reload(true);
		return null;
	}
</script>
<%=buildTree%>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
	<tr id="detailDataRow" runat="server">
		<td id="treeCell" valign="top" runat="server">
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
                                        highlightNode(<%=SelectedID%>);
                                        break;
                                    }
                                }
                            }
						</script>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
