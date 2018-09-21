<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ExecutionCtrl.ascx.cs" Inherits="ch.appl.psoft.Survey.Controls.ExecutionCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
    <tr height="30">
        <td valign="bottom">
            <asp:Label ID="stepTitle" Runat="server" CssClass="surveyStepTitle"></asp:Label>
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:Label ID="stepDescription" Runat="server" CssClass=""></asp:Label>
        </td>
    </tr>
    <tr>
        <td valign="top">
            <asp:table id="questionTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
        </td>
    </tr>
    <tr valign="bottom">
        <td>
            <input type="button" ID="back" Runat="server" class="ButtonL" onserverclick="back_Click">&nbsp;<asp:Button ID="next" Runat="server" CssClass="ButtonL" onclick="next_Click"></asp:Button>
        </td>
    </tr>
</table>
