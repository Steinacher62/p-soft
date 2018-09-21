<%@ Control language="c#" Codebehind="History.ascx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Document.Controls.History" %>
<script  type='text/javascript'>
    var deleteURL = "<%=_deleteURL%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "DOCUMENT_HISTORY", rowId, dbId)
    }
</script>
<asp:Table id="historyTab" runat="server" CssClass="List" BorderWidth="0" CellSpacing="0" CellPadding="3">
    <asp:TableRow>
        <asp:TableCell>
            <asp:label id="HistoryListTitle" runat="server" CssClass="section_title"></asp:label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell Height="10"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
                <asp:Table id="historyList" runat="server" BorderWidth="0" CssClass="List"></asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
