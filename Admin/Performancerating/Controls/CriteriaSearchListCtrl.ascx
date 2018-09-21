<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.CriteriaSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="CriteriaListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="CriteriaListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="CriteriaItemListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>