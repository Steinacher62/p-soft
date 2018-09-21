<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Training.Controls.CatalogSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="TrainingCatlogListBoxTitle" runat="server" Width="100%"></asp:Label>
</div>    
<telerik:RadListBox ID="TrainingCatlogListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanging="TrainingCatlogListBoxIndexChanging">
        
    </telerik:RadListBox>