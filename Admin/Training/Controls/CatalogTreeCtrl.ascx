<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Training.Controls.CatalogTreeCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        
    </div>
</div>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="CatalogTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="CatalogTree_NodeDataBound" OnClientContextMenuItemClicked="CatalogMenuClicked" OnClientNodeClicked="CatalogTreeNodeClicked" OnClientContextMenuShowing="CatalogTreeContextMenuShowing">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="CatalogContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="NewTraining">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewTrainingCategory">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getTrainingTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>