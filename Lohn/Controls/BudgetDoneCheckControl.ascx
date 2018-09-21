<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BudgetDoneCheckControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.BudgetDoneCheckControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table cellspacing="10" cellpadding="0" border="0" height="100%" width="100%">
    <tr>
    </tr>
    <tr height="50">
        <td align="middle">
            <asp:Label ID="textLabel" Runat="server" CssClass="section_title" Width="500"></asp:Label>
        </td>
    </tr>
    <tr height="20">
        <td align="middle">
            <asp:button id="yes" runat="server" CssClass="Button"></asp:button>&nbsp;
            <asp:button id="no" runat="server" CssClass="Button"></asp:button>
        </td>
    </tr>
    <tr>
    </tr>
</table>

