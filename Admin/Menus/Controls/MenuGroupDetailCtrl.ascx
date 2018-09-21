<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuGroupDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Menus.Controls.MenuGroupDetailCtrl" %>
<div style="height: 100%; width: 100%" class="detailDataGroup">
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
                <telerik:RadLabel ID="LabelGroupName" runat="server"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                 <telerik:RadTextBox ID="TBGroupName" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="GroupNameValidator" runat="server" ControlToValidate="TBGroupName" Display="Dynamic" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelShortName" runat="server" AssociatedControlID="TBShortName"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBShortName" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>
