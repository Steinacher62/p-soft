<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectKnowledgeCtrl.ascx.cs" Inherits="ch.appl.psoft.Report.Controls.SelectKnowledgeCtrl" %>
<div style="padding-top:20px; padding-left:20px;">
    <table >
        <tr>
             <td><asp:Label ID="lbSelectKnowledge" runat="server"> </asp:Label></td>
            <td><telerik:RadDropDownList ID="lstKnowledge" runat="server" Width="400px"></telerik:RadDropDownList></td>
        </tr>
        <tr>
            <td style="padding-top:10px"><telerik:RadButton ID="cmdOk" runat="server" Text="Report anzeigen" OnClick="cmdOk_Click"></telerik:RadButton></td>
            <td></td>
        </tr>
    </table>
</div>
