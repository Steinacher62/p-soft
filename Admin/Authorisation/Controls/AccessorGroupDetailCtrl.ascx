<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessorGroupDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AccessorGroupDetailCtrl" %>

<div>
    <telerik:RadTabStrip ID="TabStrip" runat="server" MultiPageID="MultiPageAccessorDetail" Width="100%"> 
        <Tabs>
            <telerik:RadTab PageViewID="GroupMembers"></telerik:RadTab>
            <telerik:RadTab PageViewID="MemberOf"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="MultiPageAccessorDetail" runat="server">
        <telerik:RadPageView ID="GroupMembers" runat="server">
            <div style="font-weight: bold; padding-left: 5px">
                <div style="display: table-row;">
                    <div style="display: table-cell; padding-right: 5px">
                        <asp:Label ID="GroupMembersEditableTitle" runat="server"></asp:Label>
                    </div>
                    <div style="display: table-cell;">
                        <asp:Label ID="GroupMembersAccessorTitle" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
            <telerik:RadListBox ID="GroupMembersListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" CheckBoxes="true" OnClientItemChecking="AccessorListBoxChecking" OnClientContextMenu="AccessorGroupDetailShowContextMenu" Width="100%" Height="100px"></telerik:RadListBox>
            <telerik:RadContextMenu ID="GroupMembersListContextMenu" runat="server" OnClientItemClicked="GroupMembersListBoxContextMenuItemClicked">
                <Items>
                    <telerik:RadMenuItem></telerik:RadMenuItem>
                </Items>
            </telerik:RadContextMenu>
        </telerik:RadPageView>
        <telerik:RadPageView ID="MemberOf" runat="server">
            <div style="font-weight: bold; padding-left: 5px">
                <div style="display: table-row;">
                    <div style="display: table-cell; padding-right: 5px">
                        <asp:Label ID="MemberOfEditableTitle" runat="server"></asp:Label>
                    </div>
                    <div style="display: table-cell;">
                        <asp:Label ID="MemberOfAccessorTitle" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
            <telerik:RadListBox ID="MemberOfListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" CheckBoxes="true" OnClientItemChecking="AccessorListBoxChecking" Width="100%" Height="100px"></telerik:RadListBox>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</div>

