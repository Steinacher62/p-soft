<%@ Page Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="SelectNameOE.aspx.cs" Inherits="ch.appl.psoft.Report.SelectNameOE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <table>
        <tr>
            <td colspan="2"><asp:Label ID="lblTitle" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblOE" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
            <td><asp:DropDownList ID="lstOE" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="2"><asp:CheckBox ID="chkSubOEs" runat="server" Text="chkSubOEs" /></td>
        </tr>
        <tr>
            <td colspan="2"><asp:Label ID="lblOR" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblPerson" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
            <td><asp:DropDownList ID="lstPerson" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="2"><asp:Label ID="lblNotFound" runat="server" Text=""></asp:Label></td>
        </tr>
    </table>
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />        
    </div>

</asp:Content>