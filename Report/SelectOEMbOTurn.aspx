<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectOEMbOTurn.aspx.cs" Inherits="ch.appl.psoft.Report.SelectOEMbOTurn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


    <div>
    <table>
        <tr>
            <td><asp:Label ID="lblTurn" runat="server" Text="Label"></asp:Label>:&nbsp;</td>
            <td><asp:DropDownList ID="lstTurn" runat="server"></asp:DropDownList></td>
        </tr>
        <tr><td>
     
        <asp:Label ID="lblOE" runat="server" Text="OE w&auml;hlen"></asp:Label>:&nbsp;</td>
       
        <td><asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList><br /></td></tr>
    
    </table>
            <asp:CheckBox ID="chkSubOEs" runat="server" 
                Text="auch untergeordnete OEs auflisten" />
            <br />
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />        
    </div>
 </asp:Content>

