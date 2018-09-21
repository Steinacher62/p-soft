<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextLayoutDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Chart.Controls.TextLayoutDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AddImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<asp:HiddenField ID="TextLayoutId" runat="server" />

<div style="display: table; height: 30px; width: 100%">
    <div style="display: table-row; padding-left: 10px" class="titleRow">
        <div style="display: table-cell;" class="adminTitleCell">
            <asp:Label ID="TextLayoutTitle" runat="server" Text="Label"></asp:Label>
        </div>
    </div>
</div>
<div style="display: table;" class="tableNodeLinks">
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="NameTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
        <telerik:RadTextBox ID="NameData" runat="server" CssClass="Textbox"></telerik:RadTextBox>
        <asp:RequiredFieldValidator ID="ItemNameValidator" runat="server" Display="Dynamic" ControlToValidate="NameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="AlignmentTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadDropDownList ID="AlignmentData"  runat="server"></telerik:RadDropDownList>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="FontTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
             <telerik:RadDropDownList ID="FontData"  runat="server" OnClientSelectedIndexChanged="FontDataIndexChanged"></telerik:RadDropDownList>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell" >
            <telerik:RadLabel ID="FontsizeTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
             <telerik:RadDropDownList ID="FontsizeData"  runat="server" OnClientSelectedIndexChanged="FontsizeDataIndexChanged"></telerik:RadDropDownList>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="FontBoldTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
             <telerik:RadCheckBox ID="FontBoldData" runat="server" AutoPostBack="false" OnClientCheckedChanged="FontBoldDataCheckedChanged"></telerik:RadCheckBox>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="FontItalicTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
             <telerik:RadCheckBox ID="FontItalicData" runat="server" AutoPostBack="false" OnClientCheckedChanged="FontItalicDatCheckedChanged"></telerik:RadCheckBox>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="FontColorTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
            <telerik:RadColorPicker ID="FontColorData" runat="server" ShowIcon="true" EnableCustomColor="true"  OnClientColorChange="FontColorDataColorChange"></telerik:RadColorPicker>
        </div>
    </div>
    <div style="display: table-row;">
        <div style="display: table-cell;" class="titleLabelCell">
            <telerik:RadLabel ID="FontExampleTitle" runat="server" ></telerik:RadLabel>
        </div>
        <div style="display: table-cell;" class="dataLabelCell">
             <telerik:RadLabel ID="FontExampleData"  runat="server" ></telerik:RadLabel>
        </div>
    </div>
</div>
