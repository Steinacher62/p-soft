<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectBonus.aspx.cs" Inherits="ch.appl.psoft.FoamPartner.SelectBonus" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <div>
        <asp:Label ID="pageTitle" runat="server" CssClass="section_title"></asp:Label>
        <br />
        <br />


        <table>
            <tr>
                <td>
                    <asp:Label ID="lblOE" runat="server" Text="OE w&auml;hlen "></asp:Label>:&nbsp;
        <asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList><br />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                    <asp:CheckBox ID="chkSubOEs" runat="server"
                        Text="auch untergeordnete OEs auflisten" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDateRange" runat="server" Text="Zeitraum w&auml;hlen "></asp:Label>:&nbsp;
                    <asp:Label ID="lblFrom" runat="server" Text="von"></asp:Label><telerik:RadDatePicker ID="Von" runat="server">
                        <DateInput runat="server" ID="txtVon" DateFormat="dd.MM.yyyy"></DateInput>
                    </telerik:RadDatePicker>
                    :&nbsp;
                    <asp:Label ID="lblTo" runat="server" Text="bis"></asp:Label><telerik:RadDatePicker ID="Bis" runat="server">
                        <DateInput runat="server" ID="txtBis" DateFormat="dd.MM.yyyy"></DateInput>
                    </telerik:RadDatePicker>
                    :&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Korrekturfaktor "></asp:Label><telerik:RadNumericTextBox ID="korrekturfaktor" runat="server" Value="1.00"></telerik:RadNumericTextBox>
                </td>
            </tr>
        </table>

        <br />
        <br />
        <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen"
            OnClick="cmdOk_Click" />
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .auto-style1 {
            height: 25px;
        }
    </style>
</asp:Content>
