<%@ Control Language="c#" AutoEventWireup="True" Codebehind="RegistryEntriesCtrl.ascx.cs" Inherits="ch.appl.psoft.Common.Controls.RegistryEntriesCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
    <%=_onloadString%>
</script>
<%=buildTree%>
<input type="hidden" id="registryFlags" runat="server" NAME="registryFlags">
<table border="0" cellSpacing="0" cellPadding="0" height="100%" width="100%">
    <tr height="100%">
        <td>
			<div class="ListVariable">
                <table class="Tree" border="0" width="100%" cellSpacing="0" cellPadding="2">
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
                    </tr>
                </table>
            </div>
        </td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
    </tr>
</table>
