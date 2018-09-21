<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganisationDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.OrganisationDetailCtrl" %>
 <asp:HiddenField ID="OeId" Value="0" runat="server" />
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveOEImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SaveOEClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteOEImageButton" runat="server" Image-Url="../Images/delete_enable.gif" Image-DisabledUrl="../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteOEClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="height: 100%; overflow:auto">
    <div style="display: table;  width: 100%;" class="OrganiationTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell; " class="adminTitleCell">
                <asp:Label ID="OrgentityTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; ">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="DivisionTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; overflow:visible; " class="dataLabelCell">
                <telerik:RadTextBox ID="DivisionData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="MnemonicTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; overflow:visible; " class="dataLabelCell">
                <telerik:RadTextBox ID="MnemonicData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="DecriptionTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; overflow:visible; " class="dataLabelCell">
                <telerik:RadTextBox ID="DescriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="ClipboardTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadButton ID="ClipboardButton" runat="server" OnClientClicking="ClipboardOeClicking"></telerik:RadButton>
            </div>
        </div>
    </div>
</div>
