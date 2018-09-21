<%@ Control language="c#" Codebehind="ReportLayoutSelectCtrl.ascx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Report.Controls.ReportLayoutSelectCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table cellSpacing="0" cellPadding="3" border="0">
    <tr>
        <td>
            <asp:Label ID="Title" Runat="server" CssClass="section_title"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Table ID="listTab" Runat="server"></asp:Table>
        </td>
    </tr>
</table>
