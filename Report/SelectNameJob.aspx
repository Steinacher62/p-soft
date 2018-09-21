<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectNameJob.aspx.cs" Inherits="ch.appl.psoft.Report.SelectNameJob" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
    Parameter festlegen<br />
    <br />
    Mitarbeiter w&auml;hlen:&nbsp;
        <asp:DropDownList ID="lstMA" runat="server">
        </asp:DropDownList><br />
        <br />
        <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" /><br />
        <asp:Label ID="lblOutput" runat="server" Text=""></asp:Label>
    </div>
   </asp:Content>
