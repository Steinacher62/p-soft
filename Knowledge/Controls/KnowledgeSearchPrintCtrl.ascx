<%@ Control Language="c#" AutoEventWireup="True" Codebehind="KnowledgeSearchPrintCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.KnowledgeSearchPrintCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script language="javascript">
	STARTALLOPEN = 0;
	USEICONS = 1;
	WRAPTEXT = 0;
	PERSERVESTATE = 0;
	PERSERVEHIGHLIGHT = 0;
	ICONPATH = "../images/tree/";
	DRAGNODE = 0;
	DROPNODE = 0;
	HIGHLIGHT_DRAGDROP = 0;
	HIGHLIGHT = 1;
	HIGHLIGHT_COLOR = "#374979";
	HIGHLIGHT_BG = "<%=ch.appl.psoft.Global.HighlightColorRGB%>";
	TREE_DEBUG = 0;
    CLOSELINK = 1;
    TARGET_FRAME = "_self";
</script>
<script language="javascript">
	function viewIsActiveDropDown(selfid,targetid) {
		var self = document.getElementById(selfid);
		var target = document.getElementById(targetid);
		if(self.selectedIndex == 2) {
    		target.parentElement.parentElement.style.display = 'block';
			target.disabled = false;
		} else {
			target.parentElement.parentElement.style.display = 'none';
			target.disabled = true;
			target.selectedIndex = 0;
		}
	}
</script>
<table class="List" border="0" Height="40%" CellSpacing="0" CellPadding="3">
    <tr id="detailDataRow">
        <td valign="top" style="HEIGHT: 67px">
            <asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
        </td>
        <td id="registryCell" valign="top" runat="server" style="WIDTH: 119px; HEIGHT: 67px">
            <table class="Tree" border="0" cellSpacing="0" cellPadding="2">
                <tr>
                    <td style="PADDING-LEFT:10px" vAlign="top">
                        <script language="javascript">
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
        <td vAlign="top">
            <asp:CheckBox ID="checkBoxKnowledge" Text="Wissenelemente" Runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button>
        </td>
    </tr>
</table>
