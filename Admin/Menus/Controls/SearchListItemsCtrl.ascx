<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchListItemsCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Menus.Controls.SearchListItemsCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="SearchListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="SearchListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" EnableDragAndDrop="true" OnClientDropping="SearchListBoxDropping" Width="100%" Height="380px" >
        <Items>
            
        </Items>
    </telerik:RadListBox>
</div>
