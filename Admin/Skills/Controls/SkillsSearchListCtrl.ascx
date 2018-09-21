<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkillsSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Skills.Controls.SkillsSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="ListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="SkillsListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="SkillListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>
