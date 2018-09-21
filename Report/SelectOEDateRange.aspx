<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectOEDateRange.aspx.cs" Inherits="ch.appl.psoft.Report.SelectOEDateRange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="AjaxControl">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="DatVon" />
                     <telerik:AjaxUpdatedControl ControlID="DatBis" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <div>
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblOE" runat="server" Text="OE w&auml;hlen"></asp:Label>:&nbsp;</td>
                <td>
                    <telerik:RadComboBox ID="lstOE" runat="server" Width="250px" EnableViewState="false"></telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDateRange" runat="server" Text="Zeitraum w&auml;hlen"></asp:Label>:&nbsp;</td>
                <td>
                    <asp:Label ID="lblFrom" runat="server" Text="von"></asp:Label><telerik:RadDatePicker ID="DatVon" EnableViewState="false"   runat="server"></telerik:RadDatePicker>
                    <asp:Label ID="lblTo" runat="server" Text="bis"></asp:Label><telerik:RadDatePicker ID="DatBis" EnableViewState="false" runat="server"></telerik:RadDatePicker>
                </td>
            </tr>
        </table>
        <asp:CheckBox ID="chkSubOEs" runat="server"
            Text="auch untergeordnete OEs auflisten" />
        <br />
        <br />
        <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen"
            OnClick="cmdOk_Click" />
    </div>
</asp:Content>

