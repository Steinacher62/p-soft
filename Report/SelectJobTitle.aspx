<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectJobTitle.aspx.cs" Inherits="ch.appl.psoft.Report.SelectJobTitle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


    <div>
    Stellenbeschreibung<br />
    <br />
    Funktion w&auml;hlen:&nbsp;
        <asp:DropDownList ID="lstFunktion" runat="server">
        </asp:DropDownList><br />
        <br />
        <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />
    </div>

 </asp:Content>