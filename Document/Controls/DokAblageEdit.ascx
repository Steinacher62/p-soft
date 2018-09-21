<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DokAblageEdit.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.DokAblageEdit" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript' event="endStore(ok)" for="storeFile">
    if (ok) {
        var obj = document.getElementById("storeFile");
        var file = obj ? obj.getTargetFileAt(0) : null;
        var ele = document.getElementById("<%=TargetFileName%>");
        var state = document.getElementById("<%=TargetFileState%>");
        if (ele && file && state) {
            //alert("file="+file);
            ele.value = file;
            state.value = "EndStore";
            if (typeof(wsUserSetFileProperties) != "undefined")
                wsFileProperties(file);
        }
    }
</SCRIPT>
<script  type='text/javascript' event="startStore()" for="storeFile">
    var state = document.getElementById("<%=TargetFileState%>");
    if (state)
        state.value = "StartStore";
</SCRIPT>
<script  type='text/javascript'>
    function clearInput() {
        var obj = document.getElementById("storeFile");
        var state = document.getElementById("<%=TargetFileState%>");
        var ele = document.getElementById("<%=TargetFileName%>");
        if (obj && typeof(obj.clear) != "undefined")
            obj.clear();
        if (state)
            state.value = "ClearFile";
        if (ele)
            ele.value = "";
    }
</SCRIPT>
<script  type='text/javascript'>
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

    function addSavedFile() {
        var obj = document.getElementById("storeFile");
        var ele = document.getElementById("<%=TargetFileName%>");
        var state = document.getElementById("<%=TargetFileState%>");
        if (obj && typeof(obj.addFile) != "undefined" && ele && ele.value && ele.value != "") obj.addFile(ele.value,1);
        if (ele) ele.value = "";
        if (state) state.value = "";
    }

    function onLoadTask() {
        addSavedFile();
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
            <table border="0" cellSpacing="0" cellPadding="2">
                <tr>
                    <td>
                        <asp:table id="documentTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:button id="apply" CssClass="Button" Runat="server"></asp:button>
                        <div style="DISPLAY: none">
                            <asp:textbox id="targetFileName" Runat="server"></asp:textbox>
                            <asp:textbox id="targetXFileName" Runat="server"></asp:textbox>
                            <asp:textbox id="targetFileState" Runat="server"></asp:textbox>
                        </div>
                    </td>
                </tr>
            </table>
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
                </tr>
            </table>
        </td>
    </tr>
</table>
