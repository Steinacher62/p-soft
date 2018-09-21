<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ExpectationDetailView.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.ExpectationDetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table class="List" border="0" Width="100%" CellSpacing="0" CellPadding="3">
    <TR id="detailTitleRow" runat="server">
        <TD><asp:label id="detailTitle" CssClass="section_title" runat="server"></asp:label></TD>
    </TR>
    <tr id="detailDataRow" runat="server">
        <td>
            <asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
        </td>
    </tr>
</table>