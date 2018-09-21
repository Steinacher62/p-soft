<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Controls.PersonSearchCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="SeachImageButton" runat="server" Image-Url="../Images/search_enable.gif" Height="20px" OnClientClicking="SearchPersonClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>

<div style="height: 100%; width: 100%">
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
                <telerik:RadLabel ID="LabelName" runat="server" AssociatedControlID="TBName"></telerik:RadLabel>

            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBName" runat="server"  CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelFirstname" runat="server" AssociatedControlID="TBFirstName"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBFirstName" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelTBMNEMO" runat="server" AssociatedControlID="TBMNEMO"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBMNEMO" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelPersonnelnumber" runat="server" AssociatedControlID="TBPersonnelnumber"></telerik:RadLabel>

            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBPersonnelnumber" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelOrgentity" runat="server" AssociatedControlID="DDOrgentity"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadDropDownList ID="DDOrgentity" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelShowFormer" runat="server" AssociatedControlID="TBName"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadCheckBox ID="CBShowFormer" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
            </div>
        </div>
    </div>

</div>

