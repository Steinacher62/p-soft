<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionGroupDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionGroupDetailCtrl" %>
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
                <telerik:RadLabel ID="LabelGroupParent" runat="server"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                 <telerik:RadLabel ID="LabelGroupParentData" runat="server" AssociatedControlID="TDTyp"></telerik:RadLabel>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelGroup" runat="server" AssociatedControlID="TBGroup"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBGroup" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="FunctionGroupValidator" runat="server" Display="Dynamic" ControlToValidate="TBGroup" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelDescription" runat="server" AssociatedControlID="TBDescription"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBDescription" runat="server" Resize="Both" TextMode="MultiLine" CssClass="TextboxMultiLine"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>
