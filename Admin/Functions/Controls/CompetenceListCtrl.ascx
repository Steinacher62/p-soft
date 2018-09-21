<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenceListCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.CompetenceListCtrl" %>
<div>
    <telerik:RadListBox ID="CompetenceListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="380px" OnClientSelectedIndexChanged="CompetenceListBoxIndexChanged">
        <HeaderTemplate>
            <asp:Label runat="server" ID="CompetenceListBoxHeadertitle" Font-Bold="true" ></asp:Label>
        </HeaderTemplate>
    </telerik:RadListBox>
</div>
