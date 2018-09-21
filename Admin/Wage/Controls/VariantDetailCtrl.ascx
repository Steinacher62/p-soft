<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VariantDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Wage.Controls.VariantDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="AddImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div class="WageVariant">
    <asp:HiddenField ID="VariantId" runat="server" />
    <div style="display: table; height: 30px; width: 100%">
        <div style="display: table-row; padding-left: 10px" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="VariantTitle" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
    </div>
    <div style="display: table; width: 100%;" class="tableNodeLinks">
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px; width: 200px">
                <telerik:RadLabel ID="NameTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadTextBox ID="NameData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="ItemNameValidator" runat="server" Display="Dynamic" ControlToValidate="NameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="ActiveTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell; vertical-align:middle;" class="nameTitle">
                <telerik:RadCheckBox ID="ActiveData" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="ValuePointFixTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadNumericTextBox ID="ValuePointFixData" runat="server" NumberFormat-DecimalDigits="3" CssClass="Textbox"></telerik:RadNumericTextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="ValuePointFixData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="ExclusionFromTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadDatePicker ID="ExclusionFromData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="ExclusionToTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadDatePicker ID="ExclusionToData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
    </div>
</div>
