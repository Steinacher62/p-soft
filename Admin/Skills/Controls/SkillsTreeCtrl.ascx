<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkillsTreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Skills.Controls.SkillsTreeCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveDutyImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveSkillOrGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteDutyImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteSkillClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="height: 100%; width: 100%">
    <div style="display: table; height: 100%; width: 100%">
        <div style="display: table-row;">
            <div style="display: table-cell; padding: 10px;">
                <telerik:RadTreeView ID="SkillTree" CssClass="Tree" runat="server" Height="200px" OnNodeDataBound="SkillTree_NodeDataBound" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" OnClientNodeDropping="SkillTreeNodeDropping" OnClientContextMenuItemClicked="SkillMenuClicked" OnClientNodeClicked="SkillTreeNodeClicked" OnClientContextMenuShowing="SkillTreeContextMenuShowing" OnClientNodePopulating="SkillTreeNodePopulating">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="SkillContextMenu" runat="server" RenderMode="Lightweight" DataFieldParentID="PARENT_ID" DataFieldID="ID" DataTextField="TITLE">
                            <Items>
                                <telerik:RadMenuItem Value="NewItem" Text="Neuer Skill" ImageUrl="../../Images/fb_aufgaben.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="NewGroup" Text="Neue Skillsgruppe" ImageUrl="../../Images/fb_aufgabengruppe.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Rename" Text="Umbenennen" ImageUrl="../../Images/edit.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Löschen" ImageUrl="../../Images/delete_enable.gif">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                    <WebServiceSettings Path="../WebService/AdminService.asmx" Method="getSkillTreeData" />
                </telerik:RadTreeView>
            </div>
        </div>
    </div>
</div>