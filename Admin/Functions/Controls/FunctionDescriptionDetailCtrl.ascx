<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionDescriptionDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionDescriptionDetailCtrl" %>
<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 100%;" class="FunctionDescriptionDetailTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="detailTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="titleLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="titleData" runat="server" CssClass="Textbox" ReadOnly="true"></telerik:RadTextBox>
            </div>
        </div>

        <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="descriptionLabel" runat="server"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="descriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>
