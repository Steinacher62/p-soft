<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriteriaFunctionratingLink.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.CriteriaFunctionratingLink" %>
<div class="MainContainerTitle" style="padding-left:30px;">
    <asp:Label ID="ContainerTitle" runat="server" Text="Label" ></asp:Label>
</div>
<asp:HiddenField ID="SelectedCriteriaId" runat="server" />
<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 100%;" class="CriteriaDetailTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="FunctionRatingItemTitle" runat="server"></asp:Label>
            </div>
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="CriteriaTitle" runat="server"></asp:Label>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadListBox ID="FunctionCriteriaListBox" runat="server" Culture="de-DE" CssClass="auto-style1" Width="100%" Height="380px">
                </telerik:RadListBox>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadListBox ID="CriteriaListBoxLink" runat="server" Culture="de-DE" CssClass="auto-style1" Width="100%" Height="380px">
                    
                </telerik:RadListBox>
            </div>
        </div>
    </div>
</div>

