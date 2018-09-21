<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="CopyObjectiveToOE.aspx.cs" Inherits="ch.appl.psoft.MbO.CopyObjectiveToOE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
    <table><tr><td>
        <asp:Label ID="lblSelectOE" runat="server" Text=""></asp:Label>:&nbsp;</td>
        <td><asp:DropDownList ID="lstOE" runat="server">
        </asp:DropDownList><br /></td></tr>
    </table>
            <!--<asp:CheckBox ID="chkSubOEs" runat="server" 
                Text="auch untergeordnete OEs auflisten" Visible="False" />
            <br />-->
    <br />
    <asp:Button ID="cmdOk" runat="server" Text="Ziel kopieren" 
            onclick="cmdOk_Click" />&nbsp;&nbsp;<asp:Label ID="lblDone" runat="server" Text=""></asp:Label>      
    </div>
  </asp:Content>