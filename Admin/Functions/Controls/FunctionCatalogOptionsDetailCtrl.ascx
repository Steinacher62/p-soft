<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionCatalogOptionsDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionCatalogOptionsDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveFunctionCatalogOptionClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<asp:HiddenField ID="OptionId" runat="server" />
<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 100%;" class="FunctionTypTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="detailTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="averageTypTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadDropDownList ID="averageTypData" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="maxValueTitle" runat="server"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadNumericTextBox ID="maxValueData" runat="server" NumberFormat-DecimalDigits="2" CssClass="NumericTextBox" Type="Number"></telerik:RadNumericTextBox>
            </div>
        </div>
    </div>
</div>