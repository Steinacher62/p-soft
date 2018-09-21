<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="FunctionsListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="FunctionsListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="FunctionListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>


