<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganisationTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.OrganisationTreeCtrl" %>

<div style="height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="OrganisationTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>

        <div style="display: table-row;">
            <div style="display: table-cell; padding: 1px;">
                <telerik:RadTreeView ID="OETree" CssClass="Tree" runat="server" OnNodeDataBound="OETree_NodeDataBound" AllowNodeEditing="False" EnableDragAndDrop="True" OnClientContextMenuShowing="OrgClientContextMenuShowing" OnClientNodeClicking="NodeClicking" OnClientNodeDropping="OETreeNodeDropping" Height="500px" OnClientContextMenuItemClicking="MenuOrgClicking">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="MainContextMenu" runat="server"
                            enderMode="Lightweight">
                            <Items>
                                <telerik:RadMenuItem Value="Rename" Text="Umbenennen" ImageUrl="../Images/edit.gif"
                                    PostBack="false">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewOrgentity" Text="Neue Abteilung" ImageUrl="../Images/og_abteilung.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewJob" Text="Neue Stelle" ImageUrl="../Images/og_stelle.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Authorisation" Text="Berechtigungen" ImageUrl="../Images/authorisations_enabled.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getOrganistionTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>
