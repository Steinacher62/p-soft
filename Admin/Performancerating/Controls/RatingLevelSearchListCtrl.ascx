<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RatingLevelSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.RatingLevelSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="RatingLevelTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="RatingLevelListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="RatingLevelListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>