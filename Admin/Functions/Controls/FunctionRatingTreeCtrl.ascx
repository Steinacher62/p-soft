<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionRatingTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionRatingTreeCtrl" %>

<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveFunctionRatingClicking" AutoPostBack="false"></telerik:RadImageButton>
         <telerik:RadImageButton ID="AddImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddFunctionRatingClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteFunctionRatingClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="FunctionRatingnTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="FunctionRatingnTree_NodeDataBound" OnClientContextMenuItemClicked="FunctionRatingTreeMenuClicked" OnClientNodeClicked="FunctionRatingTreeNodeClicked" OnClientContextMenuShowing="FunctionRatingTreeContextMenuShowing" OnClientNodePopulating="FunctionRatingTreePopulating" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" OnClientNodeDragStart="FunctionRatingTreeNodeDragStart" OnClientNodeDropping="FunctionRatingTreeNodeDropping">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="RatingContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="Add" Text="Funktionsbewertung hinzufügen" ImageUrl="../../Images/add_enabled.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>
