<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ProjectSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.ProjectSearchCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
</script>
<%=buildTree%>
<input type="hidden" id="registryFlags" runat="server" NAME="registryFlags">
<table border="0" cellSpacing="0" cellPadding="2" width="100%">
	<tr>
        <td align="right" valign="top"><asp:checkbox id="CBShowInactive" runat="server" AutoPostBack="True"></asp:checkbox></td>
	</tr>
</table>
<table border="0" cellSpacing="0" cellPadding="2">
    <tr>
        <td valign="top">
            <TABLE id="Table1" cellSpacing="0" cellPadding="0" border="0">
	            <tr>
		            <td><asp:table id="searchTab" CssClass="InputMask" runat="server" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
	            </tr>
	            <tr>
		            <td>
			            <table cellSpacing="0" cellPadding="2" border="0">
				            <tr>
					            <td><asp:label id="semaphore" runat="server" CssClass="InputMask_Label"></asp:label></td>
					            <td><asp:radiobutton id="rbAlle" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton></td>
					            <td id="tdDone" runat="server"><asp:radiobutton id="rbDone" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imDone" Runat="server" ImageUrl="../../images/ampelGrau.gif"></asp:image></td>
					            <td id="tdBlue" runat="server"><asp:radiobutton id="rbBlue" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imBlue" Runat="server" ImageUrl="../../images/AmpelBlau.gif"></asp:image></td>
					            <td id="tdGreen" runat="server"><asp:radiobutton id="rbGreen" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imGreen" Runat="server" ImageUrl="../../images/AmpelGruen.gif"></asp:image></td>
					            <td id="tdOrange" runat="server"><asp:radiobutton id="rbOrange" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imOrange" Runat="server" ImageUrl="../../images/AmpelOrange.gif"></asp:image></td>
					            <td id="tdRed" runat="server"><asp:radiobutton id="rbRed" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imRed" Runat="server" ImageUrl="../../images/AmpelRot.gif"></asp:image></td>
				            </tr>
			            </table>
		            </td>
	            </tr>
	            <tr>
		            <td height="10"></td>
	            </tr>
	            <tr>
		            <td><asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button></td>
	            </tr>
            </TABLE>
        </td>
        <td id="registryCell" valign="top" runat="server">
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
</table>
