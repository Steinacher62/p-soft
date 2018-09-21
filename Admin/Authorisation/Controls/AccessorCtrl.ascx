<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessorCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AccessorCtrl" %>
<style type="text/css">
    .auto-style1 {
        left: 0px;
        top: 3px;
        width: 129px;
        height: 47px;
    }
</style>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="AddAccessorImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddAccessorClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="font-weight: bold; padding-left: 5px">
    <asp:Label ID="AccessorTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
    <telerik:RadListBox ID="AccessorListBox" runat="server" Culture="de-DE" CssClass="auto-style1" EnableLoadOnDemand="true" Width="100%" Height="780px">
    </telerik:RadListBox>
</div>


