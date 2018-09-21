<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.JobDetailCtrl" %>
<asp:HiddenField ID="JobId" runat="server" />
<asp:HiddenField ID="Jobs" runat="server" />
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveJobImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SaveJobClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteJobImageButton" runat="server" Image-Url="../Images/delete_enable.gif" Image-DisabledUrl="../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteJobClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="height: 100%; overflow: auto">
    <div style="display: table; overflow:auto"class="JobTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell; " class="adminTitleCell">
                <asp:Label ID="JobFormTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; ">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="JobTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="JobTitleData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="OeTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <asp:Label ID="OeData" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="FunktionTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadDropDownList ID="FunktionData" runat="server" CssClass="DropDown" OnClientItemSelected="FunktionSelected"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="EmploymentTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadImageButton ID="EmploymentButton" runat="server" CssClass="JobImageButton" OnClientClicking="EmploymentClicking"></telerik:RadImageButton>
                <telerik:RadButton ID="JobSetVacant" runat="server" OnClientClicking="JobSetVacant"></telerik:RadButton>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="SubstituteTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadImageButton ID="SubstituteButton" runat="server" CssClass="JobImageButton" AutoPostBack="false"></telerik:RadImageButton>
                <telerik:RadButton ID="SubstituteSetVacant" runat="server" OnClientClicking="SubstituteSetVacantClicking"></telerik:RadButton>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="FromTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <asp:Label ID="FromData" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="ToTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadDatePicker ID="ToData" runat="server"></telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="MnemonicJobTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="MnemonicJobData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="EngagementTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadNumericTextBox ID="EngagementData" runat="server" NumberFormat-DecimalDigits="2" MinValue="0" MaxValue="100" Type="Percent" CssClass="NumericTextBox"></telerik:RadNumericTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="DescriptionJobTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="DescriptionJobData" runat="server" CssClass="DescriptionField" TextMode="MultiLine"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="TypeTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadDropDownList ID="TypeData" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="MainFunctionTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadCheckBox ID="MainFunctionData" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
    </div>
</div>
