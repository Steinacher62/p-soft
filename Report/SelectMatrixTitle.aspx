<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SelectMatrixTitle.aspx.cs" Inherits="ch.appl.psoft.Report.SelectMatrixTitle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
        Sokrateskarte in Listform<br />
    <br />
        Sokrateskarte auswählen:&nbsp; 
        <asp:DropDownList ID="lstMatrix" runat="server">
        </asp:DropDownList><br />
        <br />
        <asp:Button ID="cmdOk" runat="server" Text="Report anzeigen" 
            onclick="cmdOk_Click" />
    </div>
    </asp:Content>
