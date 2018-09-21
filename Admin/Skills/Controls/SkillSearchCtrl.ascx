<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkillSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Skills.Controls.SkillSearchCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="SeachImageButton" runat="server" Image-Url="../../Images/search_enable.gif" Height="20px" AutoPostBack="false" OnClientClicked="SkillSearchClick"></telerik:RadImageButton>
    </div>
</div>
<div style="height: 100%; width: 100%">
    <div style="display: table; width: 100%;" class="SearchTable">

        <div style="display: table-row;">
            <div style="display: table-cell; padding-top: 10px" class="searchLabelCell searchLabelCellTitle">
                <telerik:RadLabel ID="LabelTitle" runat="server"></telerik:RadLabel>

            </div>
            <div style="display: table-cell;">
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <telerik:RadLabel ID="LabelName" runat="server" AssociatedControlID="TBName"></telerik:RadLabel>

            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="TBName" runat="server" CssClass="Textbox" ViewStateMode="Disabled"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <telerik:RadLabel ID="LabelDescriptionSearch" runat="server" AssociatedControlID="TBDescriptionSearch"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="TBDescriptionSearch" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine" ViewStateMode="Disabled" EnableViewState="false"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>
