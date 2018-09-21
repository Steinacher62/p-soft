<%@ Page Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="MbOOrgentity.aspx.cs" Inherits="ch.appl.psoft.Report.MbOOrgentity" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <table>
        <tr>
            <td colspan="2"><asp:Label ID="lblTitle" runat="server" Text=""></asp:Label></td>
            
        </tr>
        <tr>
            <td><asp:Label ID="lblTurn" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
            <td><asp:DropDownList ID="lstTurn" runat="server" OnSelectedIndexChanged="lstTurnChanged"></asp:DropDownList></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblOE" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
            <td><asp:DropDownList ID="lstOE" runat="server" ></asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="2"><asp:CheckBox ID="chkSubOEs" runat="server" Text="chkSubOEs" /></td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblNotFound" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />        
    </div>

</asp:Content>