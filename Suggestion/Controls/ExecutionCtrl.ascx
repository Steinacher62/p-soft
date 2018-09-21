<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ExecutionCtrl.ascx.cs" Inherits="ch.appl.psoft.Suggestion.Controls.ExecutionCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table id="suggestionList" class="List" height="100%" cellSpacing="0" cellPadding="3" width="100%" border="0" Runat="server">
    <tr height="30">
        <td vAlign="bottom"><asp:label id="stepTitle" CssClass="suggestionStepTitle" Runat="server"></asp:label></td>
    </tr>
    <tr>
        <td vAlign="top"><asp:label id="stepDescription" CssClass="" Runat="server"></asp:label></td>
    </tr>
    <tr>
        <td vAlign="top"><asp:table id="questionTab" CssClass="InputMask" BorderWidth="0" runat="server"></asp:table></td>
    </tr>
    <tr vAlign="bottom">
        <td><input class="ButtonL" id="back" type="button" Runat="server" onserverclick="back_Click">&nbsp;<asp:button id="next" CssClass="ButtonL" Runat="server" onclick="next_Click"></asp:button>&nbsp;
            <asp:button id="sendToKnowledge" CssClass="ButtonL" Runat="server" onclick="sendToKnowledge_Click"></asp:button></td>
    </tr>
</table>
