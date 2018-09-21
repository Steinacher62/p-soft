<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KnowledgeMenuTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Menus.Controls.KnowledgeMenuTreeCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveMenuImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveMenuClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteMenuImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteMenuClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AuthorisationMenu" runat="server" Image-Url="../../Images/authorisations_enabled.gif" Image-DisabledUrl="../../images/authorisations_disabled.gif" Height="20px" OnClientClicking="AuthorisationMenuClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="MenuTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="MenuTree_NodeDataBound" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" OnClientNodeDragStart="MenuTreeDragStart" OnClientNodeDropping="MenuTreeNodeDropping"  OnClientContextMenuItemClicked="MenuTreeContextMenuClicked" OnClientNodeClicked="MenuTreeNodeClicked" OnClientContextMenuShowing="MenuTreeContextMenuShowing">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="MenuContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="NewMenu" Text="Neuer Menüeintrag" ImageUrl="../../Images/m_menue.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewMenugroup" Text="Neue Menügruppe" ImageUrl="../../Images/m_menuegruppe.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Rename" Text="Umbenennen" ImageUrl="../../Images/edit.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../../Images/delete_enable.gif">
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
