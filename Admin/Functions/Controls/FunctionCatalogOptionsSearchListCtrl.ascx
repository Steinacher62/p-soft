<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionCatalogOptionsSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionCatalogOptionsSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="FunctionCatalogOptionTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="FunctionsCatalogOptionListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="FunctionCatalogOptionListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>