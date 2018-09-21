<%@ Control Language="c#" AutoEventWireup="True" Codebehind="OrganigrammGraph.ascx.cs" Inherits="ch.appl.psoft.Organisation.Controls.OrganigrammGraph" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table width="100%">
    <tr align="center" width="100%">
        <td>
            <asp:Label id="Label1" runat="server">Label</asp:Label>
        </td>
    </tr>
    <tr align="center" width="100%">
        <td>
            <%=BuildImageMap()%>
            <img id="navigationImage" runat="server" border="0" usemap="#TreeMap" align="middle">
        </td>
    </tr>
</table>
