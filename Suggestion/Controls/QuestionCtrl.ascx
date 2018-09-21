<%@ Control Language="c#" AutoEventWireup="True" Codebehind="QuestionCtrl.ascx.cs" Inherits="ch.appl.psoft.Suggestion.Controls.QuestionCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="questionTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
    <asp:TableRow Height="10">
        <asp:TableCell>
            <asp:Label></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow CssClass="suggestionQuestion" Width="100%" HorizontalAlign="Left">
        <asp:TableCell VerticalAlign="Top" HorizontalAlign="Left">
            <asp:Label ID="questionNr" Runat="server" CssClass="suggestionQuestionNr"></asp:Label>
        </asp:TableCell>
        <asp:TableCell VerticalAlign="Middle" HorizontalAlign="Left" Width="100%">
            <asp:Label ID="question" Runat="server" CssClass="suggestionQuestion"></asp:Label>
            <br>
            <asp:Label ID="hint" Runat="server" CssClass="suggestionHint"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell ColumnSpan="2" VerticalAlign="top" ID="validatorCell" Runat="server"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell ColumnSpan="2" VerticalAlign="top">
            <asp:Table ID="questionElementTab" Runat="server" CssClass="List" BorderWidth="0" CellSpacing="0" CellPadding="3"></asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
