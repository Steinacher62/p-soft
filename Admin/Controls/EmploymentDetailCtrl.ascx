<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmploymentDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.EmploymentDetailCtrl" %>
<asp:HiddenField ID="employmentId" runat="server" />

<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveClipboardImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="UpdateEmploymentClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="height: 100%; overflow: auto">
    <div style="display: table;  width: 100%;" class="EmploymentTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell; " class="adminTitleCell">
                <asp:Label ID="Employment" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; ">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="EmploymentTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="EmploymentTitleData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="EmploymentEngagementTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <asp:Label ID="EmploymentEngagementData" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
        </div>
    </div>