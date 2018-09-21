<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TreeView.ascx.cs" Inherits="ch.appl.psoft.MbO.Controls.TreeView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
	STARTALLOPEN = 0;
	USEICONS = 1;
	WRAPTEXT = 0;
	PERSERVESTATE = 1;
	PERSERVEHIGHLIGHT = 0;
	ICONPATH = "images/tree/";
	DRAGNODE = 1;
	DROPNODE = 0;
	HIGHLIGHT_DRAGDROP = 0;
	HIGHLIGHT = 1;
	HIGHLIGHT_COLOR = "#374979";
	HIGHLIGHT_BG = "<%=ch.appl.psoft.Global.HighlightColorRGB%>";
	TREE_DEBUG = 0;
    CLOSELINK = 1;
    TARGET_FRAME = "_self";
</script>
<script  type='text/javascript'>
	function getCheckFlags(inputFldId) {
		var fld = document.getElementById(inputFldId);
		var first = true;
		fld.value = "";
		for (var i=0; i<document.forms[0].elements.length; i++) {
			var ele = document.forms[0].elements[i];
			if (ele.id && ele.id.substr(0,2) == "cb") {
				var id = parseInt(ele.id.substr(2));
				if ((ele.checked && id > 0)|| (!ele.checked && id < 0)) {
					if (!first) fld.value += ",";
					first = false;
					fld.value += id.toString();
				}
			}
		}
		//alert("getCheckFlags="+fld.value);
	}
	function setCheckFlags(checkFlags,disableFlag) {
		//alert("setCheckFlags="+checkFlags+"/"+disableFlag);
		if (checkFlags == "" && disableFlag == "") return;
		var values = disableFlag.split(",");
		for (var i=0; i<values.length; i++) {
			var node = findObj(values[i]);
			if (node) node.forceOpeningOfAncestorFolders();
			var ele = document.getElementById("cb"+values[i]);
			if (ele) ele.disabled = true;
		}
		values = checkFlags.split(",");
		for (var i=0; i<values.length; i++) {
			var node = findObj(values[i]);
			if (node) node.forceOpeningOfAncestorFolders();
			var ele = document.getElementById("cb"+values[i]);
			if (ele) {
				ele.checked = true;
				ele.id = "cb-"+values[i];
			}
		}
	}
</script>
<%=buildTree%>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="0" style="PADDING-LEFT:10px">
	<tr>
		<td valign="top">
			<asp:Label ID="title" Runat="server" Visible="False" CssClass="page_title"></asp:Label>
		</td>
	</tr>
	<tr height="100%">
		<td valign="top">
			<script  type='text/javascript'>
                if (typeof(nodes) != "undefined")
                {
                    for (var nodeIndex = 0; nodeIndex < nodes.length; nodeIndex++)
                    {
                        if (typeof(nodes[nodeIndex]) != "undefined")
                        {
							iconsLoaded = false;
                            initializeDocument(nodes[nodeIndex]);
                            highlightNode(<%=_highlightId%>);
                            break;
                        }
                    }
                }
			</script>
		</td>
	</tr>
</table>
