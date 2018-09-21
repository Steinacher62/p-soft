<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkillDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Skills.Controls.SkillDetailCtrl" %>
<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 100%;" class="DutyTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="detailTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="numberLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadNumericTextBox runat="server" ID="numberData" Type="Number" NumberFormat-DecimalDigits="0" CssClass="Textbox" ViewStateMode="Disabled"></telerik:RadNumericTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="titleLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="titleData" runat="server" CssClass="Textbox" ViewStateMode="Disabled"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="titleDataDataValidator" runat="server" Display="Dynamic" ControlToValidate="titleData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="descriptionLabel" runat="server"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="descriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine" ViewStateMode="Disabled"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="fromLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadDatePicker ID="fromData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="toLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadDatePicker ID="toData" runat="server" MinDate="1950.01.01" MaxDate="9999.12.31">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
    </div>
</div>
