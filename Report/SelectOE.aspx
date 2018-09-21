<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectOE.aspx.cs" Inherits="ch.appl.psoft.Report.SelectOE" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
    <table><tr><td>
    OE w&auml;hlen:&nbsp;</td>
        <td><asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList><br /></td></tr>
    <tr><td>Jahr w&auml;hlen:&nbsp;</td>
        <td><asp:DropDownList ID="lstJahr" runat="server"></asp:DropDownList></td></tr>
    </table>
            <asp:CheckBox ID="chkSubOEs" runat="server" 
                Text="auch untergeordnete OEs auflisten" Visible="False" />
            <br />
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />        
    </div>
  </asp:Content>
