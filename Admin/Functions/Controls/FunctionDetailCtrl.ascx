<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionDetailCtrl" %>
<div style="height: 100%; width: 100%" class="detailData">
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
                <telerik:RadLabel ID="LabelGroup" runat="server"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                 <telerik:RadLabel ID="LabelGroupData" runat="server" AssociatedControlID="TDTyp"></telerik:RadLabel>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelName" runat="server" AssociatedControlID="TBName"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBName" runat="server" CssClass="Textbox" ></telerik:RadTextBox>
                 <asp:RequiredFieldValidator ID="FunctionNameDataValidator" runat="server" Display="Dynamic" ControlToValidate="TBName" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelNameShort" runat="server" AssociatedControlID="TBNameShort"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBNameShort" runat="server" CssClass="Textbox" MaxLength="20"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelDescription" runat="server" AssociatedControlID="TBDescription"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBDescription" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelDefault" runat="server" AssociatedControlID="CBdefault"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadCheckBox ID="CBdefault" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
         <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelTyp" runat="server" AssociatedControlID="TypeData"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadDropDownList ID="TypeData" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelFBWRevision" runat="server" AssociatedControlID="FBWRevisionData"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadDatePicker ID="FBWRevisionData" runat="server" MinDate="1950.01.01">
                        <DateInput DateFormat="dd.MM.yyyy" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelBonusPart" runat="server" AssociatedControlID="TypeData"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadNumericTextBox ID="TBBonusPart" runat="server" NumberFormat-DecimalDigits="2" MinValue="0" MaxValue="100" Type="Percent" CssClass="NumericTextBox"></telerik:RadNumericTextBox>
            </div>
        </div>
    </div>
</div>
