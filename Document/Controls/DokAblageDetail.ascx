<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DokAblageDetail.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.DokAblageDetail" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register tagprefix="PSOFT" tagname="lockFile" src="../../Common/LockFile.ascx" %> 
<script type="text/javascript">
    documentID=<%=XID%>;
    
    function displayError(errorID, reload)
    {
        <%=ClientID%>_errorText.innerText = getErrorMessage(errorID);
        if (reload && errorID == 0){
            document.location.href = "<%=ReloadURL%>";
        }
        return errorID == 0;
    }

    function delayedRefresher(time, link){
        if(link == undefined){
            link = window.location.href;
        }
        window.setTimeout(function(){window.location.href = link}, time);
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
<!--  <PSOFT:lockFile runat="server" id="LockFile1"/>  --> 
<table id="detailTab"  class="Detail" border="0" width="100%">
    <tr>
        <td valign="top"yy>
          <asp:table id="documentTab" runat="server" CssClass="Detail" BorderWidth="0"></asp:table>  
        </td>
        
        <table id="registryCell" valign="top" runat="server">
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
        </table>
     
    </tr>
    <tr>
        <td colspan="2">
            <asp:label id="errorText" CssClass="error" Runat="server" Visible="True"></asp:label>
            <script type="text/javascript">
                if ("<%=ShowErr6Warning%>" == "True"){
                    displayError(6, false);
                }
            </script>
        </td>
    </tr>
</table>
