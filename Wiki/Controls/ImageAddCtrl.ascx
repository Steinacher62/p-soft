<%@ Control language="c#" Codebehind="ImageAddCtrl.ascx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Wiki.Controls.ImageAddCtrl" %>
<table cellSpacing="0" cellPadding="3" border="0">
    <tr>
        <td colspan="2"><asp:label id="title" CssClass="section_title" Runat="server"></asp:label></td>
    </tr>
    <tr></tr>
    <tr>
        <td class="InputMask_Label">
            <asp:Label ID="lblTitle" Runat="server"></asp:Label>
        </td>
        <td class="inputMask_Value">
            <asp:TextBox ID="tbTitle" Runat="server" Width="400"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="InputMask_Label">
            <asp:Label ID="lblDescription" Runat="server"></asp:Label>
        </td>
        <td class="inputMask_Value">
            <asp:TextBox ID="tbDescription" Runat="server" TextMode="MultiLine" Rows="5" Width="400"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="InputMask_Label">
            <asp:Label ID="lblImage" Runat="server"></asp:Label>
        </td>
        <td>
            <input id="newImage" type="file" runat="server" style="WIDTH:400px">
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="ok" Runat="server" CssClass="button" onclick="ok_Click"></asp:Button>
        </td>
    </tr>
</table>
