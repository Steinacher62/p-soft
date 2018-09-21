<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionTypSearchListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionTypSearchListCtrl" %>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="FunctionTypesListTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="FunctionsTypBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="FunctionTypListBoxIndexChanged">
        
    </telerik:RadListBox>
</div>

