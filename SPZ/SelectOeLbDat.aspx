<%@ Page  MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectOeLbDat.aspx.cs" Inherits="ch.appl.psoft.SPZ.SelectOeLbDat" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    

    <div>
        <asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label> <br /> <br />


    <table><tr><td>
    <asp:Label ID="lblOE" runat="server" Text="OE w&auml;hlen "></asp:Label>:&nbsp;
        <asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList><br /></td></tr>
        <tr><td class="auto-style1"><asp:CheckBox ID="chkSubOEs" runat="server" 
                Text="auch untergeordnete OEs auflisten" /></td></tr>
    <tr><td><asp:Label ID="lblDateRange" runat="server" Text="Zeitraum w&auml;hlen "></asp:Label>:&nbsp;
       <br /> <asp:Label ID="lblFrom" runat="server" Text="von"></asp:Label><telerik:RadDatePicker ID="txtVon" runat="server"></telerik:RadDatePicker>
        <asp:Label ID="lblTo" runat="server" Text="bis"></asp:Label><telerik:RadDatePicker ID="txtBis" runat="server"></telerik:RadDatePicker>
    </table>
            
            <br />
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />        
    </div>
 </asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style1 {
            height: 25px;
        }
    </style>
</asp:Content>
