<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClipboardDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.ClipboardDetailCtrl" %>
<asp:HiddenField ID="FolderId" runat="server" />
<asp:HiddenField ID="Typ" runat="server" />

<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveClipboardImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="UpdateFolderClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteClipboardImageButton" runat="server" Image-Url="../Images/delete_enable.gif" Image-DisabledUrl="../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteFolderClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AuthorisationAdmin" runat="server" Image-Url="../Images/authorisations_enabled.gif" Image-DisabledUrl="../images/authorisations_disabled.gif" Height="20px" OnClientClicking="AuthorisationFolderClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="height: 100%; width:100%; overflow: auto">
    <div style="display: table; width: 100%;" class="ClipboardTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="ClipboardTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
       <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="ClipboardName" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="ClipboardNameData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="ClipboardNameDataValidator" runat="server" Display="Dynamic" ControlToValidate="ClipboardNameData" ErrorMessage="Eingabe erforderlich!" Enabled="false" CssClass="FieldValidator"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="ClipboardDecriptionTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="ClipboardDescriptionData" runat="server" CssClass="DescriptionField" Resize="Both" TextMode="MultiLine"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="ClipboardCreatetTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadDatePicker ID="ClipboardCreatethData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="ClipboardNumVersionsTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadDropDownList ID="ClipboardNumVersionsData" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
    </div>
</div>
