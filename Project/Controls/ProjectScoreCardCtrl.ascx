<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ProjectScoreCardCtrl.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.ProjectScoreCardCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table width="100%">
    <tr align="left" width="100%">
        <td>
			<asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
        </td>
    </tr>
    <tr align="left" width="100%">
        <td>
            <%=BuildImageMap()%>
            <img id="navigationImage" runat="server" border="0" usemap="#TreeMap" align="middle">
        </td>
    </tr>
</table>
