<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionDescriptionTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionDescriptionTreeCtrl" %>

<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveDutyImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveDutyOrGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteDutyImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteDutyOrGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="FunctionDescriptionTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="FunctionDescriptionTree_NodeDataBound" OnClientContextMenuItemClicked="FunctionDescriptionTreeMenuClicked" OnClientNodeClicked="FunctionDescriptionTreeNodeClicked" OnClientContextMenuShowing="FunctionDescriptionTreeContextMenuShowing" OnClientNodePopulating="FunctionDescriptionTreePopulating" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" OnClientNodeDragStart="FunctionDescriptionTreeNodeDragStart" OnClientNodeDropping="FunctionDescriptionTreeNodeDropping">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="DutyContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="RefFunctionsView" Text="Funktionen mit dieser Aufgabe anzeigen">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../../WebService/AdminService.asmx" Method="GetFunctionDescriptionDutiyGroups" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>
