<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionTreeCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveFunctionImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveFunctionOrGroupClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="FunctionTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="FunctionTree_NodeDataBound" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" OnClientNodeDropping="FunctionTreeNodeDropping" OnClientContextMenuItemClicked="FunctionMenuClicked" OnClientNodeClicked="FunctionTreeNodeClicked" OnClientContextMenuShowing="FunctionTreeContextMenuShowing">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="FunctionContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="NewFunction" Text="Neue Funktion" ImageUrl="../../Images/fx_funktion.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewFunctionFolder" Text="Neue Funktionsgruppe" ImageUrl="../../Images/fx_funktionsgruppe.gif" DisabledImageUrl="../../fx_funktion_inaktiv.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Rename" Text="Umbenennen" ImageUrl="../../Images/edit.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getFunctionTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>