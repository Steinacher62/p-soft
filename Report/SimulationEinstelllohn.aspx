<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SimulationEinstelllohn.aspx.cs" Inherits="ch.appl.psoft.Report.SimulationEinstelllohn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
    <asp:Label ID="titleLabel" runat="server" Text=""></asp:Label><br /><br />
    <table>
    <tr>
    <td>OE:&nbsp;<asp:DropDownList ID="lstOE" runat="server"></asp:DropDownList></td>
    <td><asp:CheckBox ID="chkSubOEs" runat="server" Checked="True" Text="Sub-OEs" /></td>
    </tr>
    <tr>
    <td colspan="2"><asp:Label ID="jobDescriptionLabel" runat="server" Text=""></asp:Label>:&nbsp;<asp:DropDownList ID="lstJobs" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
    <td><asp:Label ID="nameLabel" runat="server" Text=""></asp:Label></td>
    <td><asp:TextBox ID="txtName" runat="server" Width="150"></asp:TextBox></td>
    </tr>
    <tr>
    <td><asp:Label ID="vornameLabel" runat="server" Text=""></asp:Label></td>
    <td><asp:TextBox ID="txtVorname" runat="server" Width="150"></asp:TextBox></td>
    </tr>
    <tr>
    <td><asp:Label ID="lohnvorstellungLabel" runat="server" Text=""></asp:Label></td>
    <td><asp:TextBox ID="txtLohnvorstellung" runat="server" Width="150">0</asp:TextBox>&nbsp;CHF</td>
    </tr>
    <tr>
    <td><asp:Label ID="leistungsanteilLabel" runat="server" Text=""></asp:Label></td>
    <td><asp:TextBox ID="txtLeistungsanteil" runat="server" Width="150">0</asp:TextBox>&nbsp;%</td>
    </tr>
    <tr>
    <td><asp:Label ID="alterLabel" runat="server" Text=""></asp:Label></td>
    <td><asp:TextBox ID="txtAlter" runat="server" Width="150">0</asp:TextBox>&nbsp;Jahre</td>
    </tr>
    <tr><td>&nbsp;</td><td>&nbsp;</td></tr>
    </table>
    <br />
    <asp:Label ID="lblFehler" runat="server" Text="" ForeColor="Red" Font-Bold="True"></asp:Label>
    <br />
        <asp:Button ID="cmdOk" runat="server" Text="Anzeigen" onclick="cmdOk_Click" />
    </div>
    </asp:Content>