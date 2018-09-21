<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessorSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AccessorSearchCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="SeachImageButton" runat="server" Image-Url="../../Images/search_enable.gif" Height="20px" AutoPostBack="false" OnClientClicked="SetAccessorList"></telerik:RadImageButton>
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
                <telerik:RadLabel ID="LabelTyp" runat="server" AssociatedControlID="TDTyp"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadDropDownList ID="DDTyp" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelAccessor" runat="server" AssociatedControlID="TBAccessor"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBAccessore" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelIsEditable" runat="server" AssociatedControlID="DDIsEditable"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadDropDownList ID="DDIsEditable" runat="server"></telerik:RadDropDownList>

            </div>
        </div>
    </div>

</div>

