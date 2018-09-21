<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TeamMemberEditCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.TeamMemberEditCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="0" cellPadding="0">
    <tr>
        <td height="20"></td>
    </tr>
    <tr>
        <td valign="top">
            <asp:table id="addTab" runat="server" CssClass="InputMask" BorderWidth="0">
                <asp:TableRow>
                    <asp:TableCell ID="personLabelCell" Runat="server" CssClass="InputMask_Label_NotNull"></asp:TableCell>
                    <asp:TableCell ID="personValueCell" Runat="server" CssClass="inputMask_Value"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ID="functionLabelCell" Runat="server" CssClass="InputMask_Label_NotNull"></asp:TableCell>
                    <asp:TableCell ID="functionValueCell" Runat="server" CssClass="inputMask_Value"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ID="oeLabelCell" Runat="server" CssClass="InputMask_Label_NotNull"></asp:TableCell>
                    <asp:TableCell ID="oeValueCell" Runat="server" CssClass="inputMask_Value"></asp:TableCell>
                </asp:TableRow>
            </asp:table>
        </td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
    </tr>
</table>
