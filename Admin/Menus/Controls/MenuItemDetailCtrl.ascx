<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuItemDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Menus.Controls.MenuItemDetailCtrl" %>
<div style="height: 100%; width: 100%" class="detailDataItem">
    <div style="display: table; width: 100%;">
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px" class="searchLabelCell searchLabelCellTitle">
                <telerik:RadLabel ID="LabelTitle" runat="server"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px" class="searchLabelCell">
                <telerik:RadLabel ID="LabelItemName" runat="server"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                 <telerik:RadTextBox ID="TBItemName" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="ItemNameValidator" runat="server" Display="Dynamic" ControlToValidate="TBItemName" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelShortcut" runat="server" AssociatedControlID="TBShortcut"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBShortcut" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelFrame" runat="server" AssociatedControlID="TBFrame"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBFrame" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>