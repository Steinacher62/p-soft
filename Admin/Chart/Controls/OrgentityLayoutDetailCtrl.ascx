<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgentityLayoutDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Chart.Controls.OrgentityLayoutDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AddImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<asp:HiddenField ID="OrgentityLayoutId" runat="server" />

<div style="display: table; height: 30px; width: 100%">
    <div style="display: table-row; padding-left: 10px" class="titleRow">
        <div style="display: table-cell;" class="adminTitleCell">
            <asp:Label ID="OrgentityLayoutTitle" runat="server" Text="Label"></asp:Label>
        </div>
    </div>
</div>
<div style="display: table" class="tableNodeLinks">
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="NameTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadTextBox ID="NameData" runat="server" ></telerik:RadTextBox>
            <asp:RequiredFieldValidator ID="ItemNameValidator" runat="server" Display="Dynamic" ControlToValidate="NameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="PictureTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadTextBox ID="PictureData" runat="server" ></telerik:RadTextBox>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="WidthTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell" >
            <telerik:RadNumericTextBox ID="WidthData" runat="server" NumberFormat-DecimalDigits="0"  ></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="HeightTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell" >
            <telerik:RadNumericTextBox ID="HeightData" runat="server" NumberFormat-DecimalDigits="0" ></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="PaddingTopTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell" >
            <telerik:RadNumericTextBox ID="PaddingTopData" runat="server" NumberFormat-DecimalDigits="0" ></telerik:RadNumericTextBox>
        </div> 
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="PaddingLeftTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell" >
            <telerik:RadNumericTextBox ID="PaddingLeftData" runat="server" NumberFormat-DecimalDigits="0" ></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="PaddingRightTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell" >
            <telerik:RadNumericTextBox ID="PaddingRightData" runat="server" NumberFormat-DecimalDigits="0" ></telerik:RadNumericTextBox>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="LineWidthTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell" >
            <telerik:RadNumericTextBox ID="LineWidthData" runat="server" NumberFormat-DecimalDigits="0" ></telerik:RadNumericTextBox>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="LineColorTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadColorPicker ID="LineColorData" runat="server" ShowIcon="true" EnableCustomColor="true"></telerik:RadColorPicker>
        </div>
    </div>
     <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="BackgroundColorTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadColorPicker ID="BackgroundColorData" runat="server" ShowIcon="true" EnableCustomColor="true"></telerik:RadColorPicker>
        </div>
    </div>
</div>
