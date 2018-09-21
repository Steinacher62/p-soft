<%@ Page MasterPageFile="~/Framework.Master" language="c#" Codebehind="OrganisationTree.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.MbO.OrganisationTree" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<script  type='text/javascript' type='text/javascript' src='../JavaScript/Tree/ua.js'></script>
	<script  type='text/javascript' type='text/javascript' src='../JavaScript/Tree/ftiens4.js'></script>
	<script  type='text/javascript' >
		STARTALLOPEN = 0;
		USEICONS = 1;
		WRAPTEXT = 0;
		PERSERVESTATE = 1;
		PERSERVEHIGHLIGHT = 0;
		ICONPATH = "images/tree/OE/";
		DRAGNODE = 0;
		DROPNODE = 1;
		HIGHLIGHT_DRAGDROP = 1;
		HIGHLIGHT = 1;
		HIGHLIGHT_COLOR = "#374979";
		HIGHLIGHT_BG = "<%=ch.appl.psoft.Global.HighlightColorRGB%>";
		TREE_DEBUG = 0;
		CLOSELINK = 1;
		TARGET_FRAME = "_self";
		MOVE = addObjective
	    
		var backURL = "<%=encodeBackURL%>";
	    
		function addObjective(sourceType,source,targetId) {
			//alert("MOVE-"+sourceType+": "+source);
			if (sourceType == "TreeID") {
				backURL += source;
				window.location.href = "ObjectiveToOE.aspx?id="+source+"&oeId="+targetId+"&backURL="+backURL;
			}
			return false;
		}
	</script>
	<%=buildTree%>

			<script  type='text/javascript'>
                if (typeof(nodes) != "undefined")
                {
                    for (var nodeIndex = 0; nodeIndex < nodes.length; nodeIndex++)
                    {
                        if (typeof(nodes[nodeIndex]) != "undefined")
                        {
							iconsLoaded = false;
                            initializeDocument(nodes[nodeIndex]);
                            highlightNode(<%=_highlightOEId%>);
                            break;
                        }
                    }
                }
			</script>
</asp:Content>
