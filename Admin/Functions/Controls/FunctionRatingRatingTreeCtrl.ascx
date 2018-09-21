<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionRatingRatingTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionRatingRatingTreeCtrl" %>

<div style="overflow: auto; height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="FunctionRatingnRatingTree" CssClass="Tree" runat="server" Height="200px" OnClientContextMenuItemClicked="FunctionRatingRatingTreeMenuClicked" OnClientNodeClicked="FunctionRatingRatingTreeNodeClicked" OnClientContextMenuShowing="FunctionRatingRatingTreeContextMenuShowing" OnClientContextMenuItemClicking="FunctionRatingRatingTreeContextMenuItemClicking" OnClientNodePopulating="FunctionRatingRatingTreePopulating" EnableDragAndDrop="true" OnClientNodeDragStart="FunctionRatingRatingTreeNodeDragStart" OnClientNodeDropping="FunctionRatingRatingTreeNodeDropping">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="RatingContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="Add" Text="Merkmal hinzufügen" ImageUrl="../../Images/add_enabled.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Remove" Text="Merkmal entfernen" ImageUrl="../../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="ShowReference" Text="Funktionsbewertungen mit diesem Merkmal anzeigen" ImageUrl="../../Images/document.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Description" Text="Beschreibung anzeigen" ImageUrl="../../Images/info.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../../WebService/AdminService.asmx" Method="GetFunctionRatingRatingTreeNodes" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>
