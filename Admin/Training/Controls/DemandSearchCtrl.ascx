<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DemandSearchCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Training.Controls.DemandSearchCtrl" %>

<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="SeachImageButton" runat="server" Image-Url="../../Images/search_enable.gif" Height="20px" AutoPostBack="false" OnClientClicked="SearchClick"></telerik:RadImageButton>
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
                <telerik:RadTextBox ID="TBName" runat="server" CssClass="Textbox"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>