<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobExpectationDefaultSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.JobExpectationDefaultSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="PerformanceratingItemListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="PerformanceratingItemListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="PerformanceratingItemListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>