<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClipboardTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.ClipboardTreeCtrl" %>
<div style="height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="ClipboardTree" CssClass="Tree" runat="server" OnClientNodeClicking="ClipboardTreeNodeClicking" OnClientContextMenuItemClicked="ClipboardMenuClicked">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="ClipboardContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="Rename" Text="Umbenennen" ImageUrl="../Images/edit.gif"
                                    PostBack="false">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewOrgentity" Text="Neuer Ordner" ImageUrl="../Images/ordner_zu.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getClipboardTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>
