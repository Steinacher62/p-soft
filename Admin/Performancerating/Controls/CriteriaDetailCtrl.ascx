<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.CriteriaDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<asp:HiddenField ID="CriteriaId" runat="server" />
<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 700px;" class="CriteriaDetailTable"> 
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
                <telerik:RadTextBox ID="titleData" runat="server" CssClass="Textbox" ViewStateMode="Disabled" Width="550px"></telerik:RadTextBox>
                <asp:RequiredFieldValidator ID="titleDataDataValidator" runat="server" Display="Dynamic" ControlToValidate="titleData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
         <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="descriptionLabel" runat="server"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="descriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine" ViewStateMode="Disabled" Width="550px"></telerik:RadTextBox>
            </div>
        </div>
         <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="FunctionRatingLabel" runat="server" Width="300px"></asp:Label>
            </div>
            <div style="display: table-cell; vertical-align:central;" class="dataLabelCell">
                <telerik:RadDropDownList ID="FunctionRatingLinkDD" Width="550px" runat="server"></telerik:RadDropDownList>
            </div>
        </div>
    </div>
</div>
