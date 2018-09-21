<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Training.Controls.CatalogDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div class="TrainigDetail">
    <asp:HiddenField ID="TrainingId" runat="server" />
    <div style="display: table; height: 30px; width: 100%">
        <div style="display: table-row; padding-left: 10px" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="TrainigDetailTitle" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
    </div>
    <div style="display: table; width: 100%; overflow-y:auto;" class="tableNodeLinks">
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
                <telerik:RadLabel ID="ValidFromTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadDatePicker ID="ValidFromData" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="ValidToTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadDatePicker ID="ValidTo" runat="server" MinDate="1950.01.01">
                    <DateInput DateFormat="dd.MM.yyyy" runat="server" CssClass="TextBoxDateInput"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="CostExternTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadNumericTextBox ID="CostExternData" runat="server" NumberFormat-DecimalDigits="0" CssClass="Textbox"></telerik:RadNumericTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="CostInternTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadNumericTextBox ID="CostInternData" runat="server" NumberFormat-DecimalDigits="0" CssClass="Textbox"></telerik:RadNumericTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="PlaceTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadTextBox ID="PlaceData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="NumberParticipantTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadNumericTextBox ID="NumberParticipantData" runat="server" NumberFormat-DecimalDigits="0" CssClass="Textbox"></telerik:RadNumericTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="TrainerTite" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadTextBox ID="TrainerData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>

<div class="TrainigCatalogDetail">
    <asp:HiddenField ID="TrainingGroupId" runat="server" />
    <div style="display: table; height: 30px; width: 100%">
        <div style="display: table-row; padding-left: 10px" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="TrainingGroupTitle" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
    </div>
    <div style="display: table; width: 100%;" class="tableNodeLinks">
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px; width: 200px">
                <telerik:RadLabel ID="GroupTitletitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadTextBox ID="GroupTitleData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="GroupTitleDataValidator" runat="server" Display="Dynamic" ControlToValidate="GroupTitleData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px">
                <telerik:RadLabel ID="GrouDescriptionTitle" runat="server" CssClass="searchLabelCell"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="nameTitle">
                <telerik:RadTextBox ID="GroupDescriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine" ViewStateMode="Disabled" Width="350px"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>
