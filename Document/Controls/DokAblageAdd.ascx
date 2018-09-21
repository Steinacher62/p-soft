<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DokAblageAdd.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.DokAblageAdd" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<script type="text/javascript">
    wsUserSetFileProperties = setFileProperties;
    
    function setFileProperties(values) {
        var ele = document.getElementById("<%=ClientID%>_Input-DOCUMENT-TITLE");
        if (ele && !(ele.value && ele.value != ""))
            ele.value = values[0];
        ele = document.getElementById("<%=ClientID%>_Input-DOCUMENT-DESCRIPTION");
        if (ele && !(ele.value && ele.value != ""))
            ele.value = values[1];
        ele = document.getElementById("<%=ClientID%>_Input-DOCUMENT-AUTHOR");
        if (ele && !(ele.value && ele.value != ""))
            ele.value = values[2];
        ele = document.getElementById("<%=ClientID%>_Input-DOCUMENT-CREATED");
        if (ele && !(ele.value && ele.value != ""))
            ele.value = values[3];
    }
    
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
<table border="0" cellSpacing="0" cellPadding="2">
	<tr>
		<td valign="top">
			<asp:table id="addTab" runat="server" CssClass="InputMask" BorderWidth="0">
				<asp:TableRow>
					<asp:TableCell>
						<asp:table id="documentTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table>
					</asp:TableCell>
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell>
						<asp:button id="apply" CssClass="Button" Runat="server"></asp:button>
					</asp:TableCell>
				</asp:TableRow>
			</asp:table>
			<br>
			<div style="display:none;">
				<asp:textbox id="targetFileName" Runat="server"></asp:textbox>
				<asp:textbox id="targetFileState" Runat="server"></asp:textbox>
			</div>
		</td>
		<td id="registryCell" valign="top" runat="server">
			<table class="Tree" border="0" cellSpacing="0" cellPadding="2">
				<tr>
					<td style="PADDING-LEFT:10px" vAlign="top">
						<script type="text/javascript">
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
				</tr>
			</table>
		</td>
	</tr>
</table>
