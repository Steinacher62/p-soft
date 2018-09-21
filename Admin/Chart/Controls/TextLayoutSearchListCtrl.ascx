<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextLayoutSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Chart.Controls.TextLayoutSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="ChartsTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="TextLayoutListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanging="TextLayoutListBoxIndexChanging">
        
    </telerik:RadListBox>
</div>