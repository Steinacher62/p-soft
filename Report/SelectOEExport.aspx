<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectOEExport.aspx.cs" Inherits="ch.appl.psoft.Report.wordexport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
    <table><tr><td>
    OE w&auml;hlen:&nbsp;</td>
        <td><asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList><br /></td></tr>
    </table>
            <asp:CheckBox ID="chkSubOEs" runat="server" 
                Text="auch untergeordnete OEs auflisten" />
            <br />
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Export starten" 
            onclick="cmdOk_Click" />        
    </div>

  </asp:Content>