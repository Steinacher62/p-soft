<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingListCtrl.ascx.cs"
    Inherits="ch.appl.psoft.Project.Controls.BillingListCtrl" %>

<script  type='text/javascript'>
    var deleteURL = "<%=PostDeleteURL%>";

    function deleteRowConfirm(rowId,dbId) {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "PROJECT_BILLING", rowId, dbId)
    }

</script>

<asp:Table runat="server" ID="Table1" Height="100%">
    <asp:TableRow>
        <asp:TableCell>
            <asp:Label ID="pageTitle" runat="server" CssClass="section_title"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Height="100%">
        <asp:TableCell>
            <div class="ListVariable">
                <asp:Table ID="listTab" BorderWidth="0" runat="server" Width="100%">
                </asp:Table>
            </div>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:CheckBox ID="subprojects" CssClass="CheckBox" runat="server"></asp:CheckBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:Button ID="apply" CssClass="Button" runat="server"></asp:Button>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
    </asp:TableRow>
</asp:Table>
