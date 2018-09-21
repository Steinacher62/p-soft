<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="SelectApplRight.aspx.cs" Inherits="ch.appl.psoft.Report.SelectApplRight" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <table><tr><td>
        <asp:Label ID="lblSelPerson" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
        <td><asp:DropDownList ID="lstSelPerson" runat="server">
        </asp:DropDownList><br /></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblShowFor" runat="server" Text="Label"></asp:Label></td></tr>
    <tr><td><asp:Label ID="lblOE" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
        <td><asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList></td></tr>
    <tr><td colspan="2"><asp:Label ID="lblOR" runat="server" Text="Label"></asp:Label></td></tr>
    <tr><td><asp:Label ID="lblPerson" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
        <td><asp:DropDownList ID="lstPerson" runat="server">
        </asp:DropDownList></td></tr>
    </table>
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />        
    </div>

</asp:Content>
