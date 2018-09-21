<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgentityLayoutSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Chart.Controls.OrgentityLayoutSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="OrgentityLayoutListBoxTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="OrgentityLayoutListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanging="OrgentityLayoutListBoxIndexChanging">
        
    </telerik:RadListBox>
</div>
