<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthorisationUserCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AuthorisationUserCtrl" %>
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
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveAuthorisationImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicked ="SaveAuthorisationClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteAuthorisationImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteAuthorisationClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AddAuthorisationImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddAuthorisationClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="font-weight:bold; padding-left:5px">
    <asp:Label ID="AuthorisationTitle" runat="server" Width="100%"></asp:Label>
</div>
<div>
<telerik:RadListBox ID="AuthorisationListBox" runat="server" Culture="de-DE"  EnableLoadOnDemand="true" Width="100%" Height="200px" OnClientSelectedIndexChanged="AuthorisationListBoxIndexChanged">
</telerik:RadListBox>
    </div>

