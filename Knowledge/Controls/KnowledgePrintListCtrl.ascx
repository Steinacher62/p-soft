<%@ Control Language="c#" AutoEventWireup="True" Codebehind="KnowledgePrintListCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.KnowledgePrintListCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="List" border="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="2">
    <tr>
        <td>
            <asp:label id="pageTitle" runat="server" CssClass="section_title"></asp:label>
        </td>
    </tr>
    <tr>
        <td colspan="2" Height="8"></td>
    </tr>
    <tr valign="top" Height="100%">
        <td colspan="2">
            <div class="ListVariable">
                <asp:Table id="listTable" EnableViewState="False" runat="server" BorderWidth="0" CssClass="List" CellSpacing="0" CellPadding="2"></asp:Table>
            </div>
        </td>
    </tr>
    <tr class="List" valign="bottom">
        <td colspan="2" Height="10">
            <asp:Button id="next" CssClass="Button" Visible="True" Text="Print Selection" Runat="server" onclick="next_Click"></asp:Button>&nbsp;
        </td>
    </tr>
    <tr class="List" Height="2">
        <td></td>
    </tr>
</table>
