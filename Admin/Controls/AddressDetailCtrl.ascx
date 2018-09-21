<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.AddressDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SaveAddressClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../Images/delete_enable.gif" Image-DisabledUrl="../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteAddressClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<div style="height: 100%; overflow: auto">
    <div style="display: table;  width: 100%;" class="AddressTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell; " class="adminTitleCell">
                <asp:Label ID="AddressTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; ">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabel1" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressData1" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabel2" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressData2" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
         <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabel3" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressData3" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
         <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelZip" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressDataZip" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelCity" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressDataCity" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelCountry" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadDropDownList ID="AddessDataCountry" runat="server" CssClass="DropDown" OnClientItemSelected="FunktionSelected"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelPhone" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressDataPhone" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelFax" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressDataFax" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelMobil" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressDataMobil" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell; " class="titleLabelCell">
                <asp:Label ID="AddressLabelEMail" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell; " class="dataLabelCell">
                <telerik:RadTextBox ID="AddressDataEMail" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>

