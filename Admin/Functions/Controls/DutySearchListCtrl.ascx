<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DutySearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.DutySearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="ListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="DutyListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="DutyListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>
