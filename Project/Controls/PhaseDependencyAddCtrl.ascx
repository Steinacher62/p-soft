<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PhaseDependencyAddCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.PhaseDependencyAddCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="0" cellPadding="0">
    <tr>
        <td height="20"></td>
    </tr>
    <tr>
        <td valign=top>
            <asp:table id="addMasterTab" runat="server" CssClass="InputMask" BorderWidth="0" />
        </td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td valign=top>
            <asp:table id="addSlaveTab" runat="server" CssClass="InputMask" BorderWidth="0" />
        </td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:button id="apply" CssClass="Button" Runat="server" ></asp:button></td>
    </tr>
</table>
