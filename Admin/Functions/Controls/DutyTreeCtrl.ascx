<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DutyTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.DutyTreeCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveDutyImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveDutyOrGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteDutyImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteDutyClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="DutyTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="DutyTree_NodeDataBound" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" OnClientNodeDropping="DutyTreeNodeDropping" OnClientContextMenuItemClicked="DutyMenuClicked" OnClientNodeClicked="DutyTreeNodeClicked" OnClientContextMenuShowing="DutyTreeContextMenuShowing" OnClientNodePopulating="DutyTreeNodePopulating">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="DutyContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="NewItem" Text="Neue Aufgabe" ImageUrl="../../Images/fb_aufgaben.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewGroup" Text="Neue Aufgabengruppe" ImageUrl="../../Images/fb_aufgabengruppe.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Rename" Text="Umbenennen" ImageUrl="../../Images/edit.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getDutyTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>