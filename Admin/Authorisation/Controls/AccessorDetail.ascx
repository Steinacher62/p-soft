<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessorDetail.ascx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.Controls.AccessorDetail" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveAccessorImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicked="SaveAccesorGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="DelteAccessorImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteAccessorGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AddAccessorGroupImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddAccessorGroupClicking" AutoPostBack="false"></telerik:RadImageButton>
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
                <telerik:RadLabel ID="LabelGroup" runat="server" AssociatedControlID="TDTyp"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBGroup" runat="server" CssClass="Textbox" ReadOnly="true"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="searchLabelCell">
                <telerik:RadLabel ID="LabelAccessor" runat="server" AssociatedControlID="TBAccessor"></telerik:RadLabel>
            </div>
            <div style="display: table-cell;" class="searchData">
                <telerik:RadTextBox ID="TBAccessor" runat="server" CssClass="Textbox" ReadOnly="true"></telerik:RadTextBox>
            </div>
        </div>
    </div>
</div>

<telerik:RadWindow ID="NewAccessorgroupWindow" runat="server" Width="800px" Height="150px" Modal="true">
    <ContentTemplate>
        <div class="CommandRow">
            <div style="display: table-cell;" class="CommandCell">
                <telerik:RadImageButton ID="RadImageButton1" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicked="AddAccessorGroup" AutoPostBack="false"></telerik:RadImageButton>
            </div>
        </div>
        <div padding-left: 5px">
            <div style="display: table-row;">
                <div style="display: table-cell; padding-right: 5px; padding-top:15px; font-weight: bold;">
                    <asp:Label ID="LabelNewGroupTitle" runat="server"></asp:Label>
                </div>
                <div style="display: table-cell;">
                    <telerik:RadTextBox ID="TBNewGroupTitle" runat="server" CssClass="Textbox"></telerik:RadTextBox>
                </div>
            </div>
        </div>
    </ContentTemplate>
</telerik:RadWindow>


