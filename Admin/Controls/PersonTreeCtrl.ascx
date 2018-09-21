<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.PersonTreeCtrl" %>


<div style="display: table;">
    
    <div style="display: table-row;">
        <div style="display: table-cell; padding: 10px;">
            <telerik:RadTreeView ID="PersonTree" CssClass="Tree" runat="server" OnClientNodePopulating="PersonTreeNodePopulating" AllowNodeEditing="False" EnableDragAndDrop="True" OnClientNodeClicking="PersonTreeNodeClicked" OnClientNodeDragging="PersonNodeDragging" OnClientNodeDropping="PersonNodeDropping" Height="500px">
                <ContextMenus>
                    <telerik:RadTreeViewContextMenu ID="MainContextMenu" runat="server"
                        enderMode="Lightweight">
                        <Items>
                        </Items>
                        <CollapseAnimation Type="none"></CollapseAnimation>
                    </telerik:RadTreeViewContextMenu>
                </ContextMenus>
                <Nodes>
                    <telerik:RadTreeNode runat="server" Text="Personen" ExpandMode="WebService"
                        Value="1">
                    </telerik:RadTreeNode>
                </Nodes>

                <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getPersonTreeData" />
            </telerik:RadTreeView>
        </div>
    </div>
</div>
