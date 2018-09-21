<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingAddCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.BillingAddCtrl" %>

<table border="0" cellSpacing="0" cellPadding="2">
    <tr>
        <td valign="top">
            <table border="0" cellSpacing="0" cellPadding="0">
                <tr>
                    <td height="20"></td>
                </tr>
                <tr>
                    <td valign=top>
                        <asp:table id="addTab" runat="server" CssClass="InputMask" BorderWidth="0" />
                    </td>
                </tr>
                <tr>
                    <td height="10"></td>
                </tr>
                <tr>
                    <td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
                </tr>
            </table>
        </td>
    </tr>
</table>
