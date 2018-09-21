<%@ Page language="c#" Codebehind="Authorisations.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.Authorisations" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>
            <%=authorisationsTitle.Text%>
        </title>
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <LINK href="../Style/Psoft.css" type="text/css" rel="stylesheet">
        <script  type='text/javascript' type="text/javascript" src="../JavaScript/PopupWindow.js"></script>
    </HEAD>
    <body onload="<%=_onloadString%>">
        <div id="resizeDIV">
            <form id="Authorisations" method="post" runat="server">
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
                <TABLE id="Table1" cellSpacing="0" cellPadding="2" border="0" width="100%">
                    <TR>
                        <TD><asp:label id="authorisationsTitle" runat="server" CssClass="section_title"></asp:label></TD>
                    </TR>
                    <TR>
                        <TD colspan="2"><asp:label id="LabelAccessors" runat="server" CssClass="detail_header"></asp:label></TD>
                    </TR>
                    <TR>
                        <TD colspan="2">
                            <TABLE id="Table2" cellSpacing="0" cellPadding="2" border="0" width="100%">
                                <TR>
                                    <TD width="100%"><asp:listbox id="accessors" runat="server" AutoPostBack="True" Rows="10" BorderStyle="None" Width="100%" onselectedindexchanged="accessors_SelectedIndexChanged"></asp:listbox></TD>
                                    <TD align="left"><telerik:RadButton id="AddAccessorButton" runat="server" CssClass="Button" Enabled="False" onclick="AddAccessorButton_Click"></telerik:RadButton><BR>
                                        <BR>
                                        <telerik:RadButton id="RemoveAccessorButton" runat="server" CssClass="Button" Enabled="False" onclick="RemoveAccessorButton_Click"></telerik:RadButton></TD>
                                </TR>
                            </TABLE>
                        </TD>
                    </TR>
                    <TR>
                        <TD colspan="2"><asp:label id="LabelAuthorisations" runat="server" CssClass="detail_header"></asp:label></TD>
                    </TR>
                    <TR>
                        <TD colspan="2">
                            <TABLE id="Table3" cellSpacing="0" cellPadding="1" border="0">
                                <TR>
                                    <TD><asp:label id="LabelRead" runat="server" CssClass="detail_label"></asp:label></TD>
                                    <TD><asp:checkbox id="CBRead" runat="server" Enabled="False" oncheckedchanged="CBRead_CheckedChanged"></asp:checkbox></TD>
                                    <TD><asp:checkbox id="CBReadEffective" runat="server" Enabled="False"></asp:checkbox></TD>
                                </TR>
                                <TR>
                                    <TD><asp:label id="LabelInsert" runat="server" CssClass="detail_label"></asp:label></TD>
                                    <TD><asp:checkbox id="CBInsert" runat="server" Enabled="False" oncheckedchanged="CBInsert_CheckedChanged"></asp:checkbox></TD>
                                    <TD><asp:checkbox id="CBInsertEffective" runat="server" Enabled="False"></asp:checkbox></TD>
                                </TR>
                                <TR>
                                    <TD><asp:label id="LabelUpdate" runat="server" CssClass="detail_label"></asp:label></TD>
                                    <TD><asp:checkbox id="CBUpdate" runat="server" Enabled="False" oncheckedchanged="CBUpdate_CheckedChanged"></asp:checkbox></TD>
                                    <TD><asp:checkbox id="CBUpdateEffective" runat="server" Enabled="False"></asp:checkbox></TD>
                                </TR>
                                <TR>
                                    <TD><asp:label id="LabelDelete" runat="server" CssClass="detail_label"></asp:label></TD>
                                    <TD><asp:checkbox id="CBDelete" runat="server" Enabled="False" oncheckedchanged="CBDelete_CheckedChanged"></asp:checkbox></TD>
                                    <TD><asp:checkbox id="CBDeleteEffective" runat="server" Enabled="False"></asp:checkbox></TD>
                                </TR>
                                <TR>
                                    <TD><asp:label id="LabelAdmin" runat="server" CssClass="detail_label"></asp:label></TD>
                                    <TD><asp:checkbox id="CBAdmin" runat="server" Enabled="False" oncheckedchanged="CBAdmin_CheckedChanged"></asp:checkbox></TD>
                                    <TD><asp:checkbox id="CBAdminEffective" runat="server" Enabled="False"></asp:checkbox></TD>
                                </TR>
                                <TR>
                                    <TD><asp:label id="LabelExecute" runat="server" CssClass="detail_label"></asp:label></TD>
                                    <TD><asp:checkbox id="CBExecute" runat="server" Enabled="False" oncheckedchanged="CBExecute_CheckedChanged"></asp:checkbox></TD>
                                    <TD><asp:checkbox id="CBExecuteEffective" runat="server" Enabled="False"></asp:checkbox></TD>
                                </TR>
                            </TABLE>
                            <asp:checkbox id="CBInheritance" runat="server" Enabled="False" oncheckedchanged="CBInheritance_CheckedChanged"></asp:checkbox>
                        </TD>
                    </TR>
                    <TR>
                        <TD colspan="2">
                            <asp:label id="errorText" CssClass="error" Runat="server" Visible="False"></asp:label></TD>
                    </TR>
                </TABLE>
            </form>
        </div>
    </body>
</HTML>
