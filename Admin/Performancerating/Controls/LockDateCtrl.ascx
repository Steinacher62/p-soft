<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LockDateCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.LockDateCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveLockdateClicking" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="display: table; width: 500px;" class="FunctionDescriptionDetailTable">
    <div style="display: table-row;" class="titleRow">
        <div style="display: table-cell;" class="adminTitleCell">
            <asp:Label ID="detailTitle" runat="server" Text="Label"></asp:Label>
        </div>
        <div style="display: table-cell;">
        </div>
    </div>
    <div style="display: table-row;" class="dataRow">
        <div style="display: table-cell;" class="titleLabelCell">
            <asp:Label ID="lockDateLabel" runat="server" Text="Label"></asp:Label>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadDatePicker ID="lockDateData" runat="server" MinDate="2000.01.01">
                <DateInput DateFormat="dd.MM.yyyy" runat="server"></DateInput>
            </telerik:RadDatePicker>
        </div>
    </div>
</div>
