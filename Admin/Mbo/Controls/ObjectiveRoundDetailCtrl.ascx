﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObjectiveRoundDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Mbo.Controls.ObjectiveRoundDetailCtrl" %>
<div class="CommandRow"> 
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="AddImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div class="MboRound">
    <asp:HiddenField ID="RoundId" runat="server" />
    <div style="display: table; height: 30px; width: 100%">
        <div style="display: table-row; padding-left: 10px" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="RoundDetailTitle" runat="server" Text="Label"></asp:Label>
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
                <telerik:RadLabel ID="DesacriptionTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadTextBox ID="DescriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine" ViewStateMode="Disabled" Width="350px"></telerik:RadTextBox>
            </div>
        </div>
         <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="StartFromTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadDatePicker ID="StartFromData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ControlToValidate="StartFromData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="EndToTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadDatePicker ID="EndToData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="EndToData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
</div>
