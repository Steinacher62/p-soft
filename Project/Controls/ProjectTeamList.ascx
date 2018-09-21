<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ProjectTeamList.ascx.cs" Inherits="ch.appl.psoft.Project.Controls.ProjectTeamList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, document.location.href, deleteConfirmMessage, "JOB", rowId, dbId)
    }
</script>
<asp:Table Runat="server" id="Table1" Height="100%">
    <asp:TableRow Height="100%">
        <asp:TableCell>
            <DIV class="ListVariable">
                <br>
                <asp:Label ID="commiteeLabel" Runat="server" CssClass="section_title"></asp:Label>
                <asp:Table id="commiteeListTab" BorderWidth="0" runat="server"></asp:Table><br>
                <asp:Label ID="leaderLabel" Runat="server" CssClass="section_title"></asp:Label>
                <asp:Table id="leaderListTab" BorderWidth="0" runat="server"></asp:Table><br>
                <asp:Label ID="memberLabel" Runat="server" CssClass="section_title"></asp:Label>
                <asp:Table id="memberListTab" BorderWidth="0" runat="server"></asp:Table>
            </DIV>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>