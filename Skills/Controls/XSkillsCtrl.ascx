<%@ Control Language="c#" AutoEventWireup="True" Codebehind="XSkillsCtrl.ascx.cs" Inherits="ch.appl.psoft.Skills.Controls.XSkillsCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "XSkills.aspx?jobID=<%=JobID%>&personID=<%=PersonID%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "SKILL_LEVEL_VALIDITY", rowId, dbId)
    }
</script>
<asp:Table id="xSkillsTable" runat="server" Width="100%" Height="100%">
    <asp:TableRow Width="100%" ID="ablageRow" Height="100%">
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell">
            <asp:Table id="skillLevelTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="SkillLevelListTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Right">
                        <asp:checkbox id="CBShowValidSkillLevelOnly" runat="server" AutoPostBack="True"></asp:checkbox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Height="10" ColumnSpan="2"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top" Height="100%">
                    <asp:TableCell Width="100%" ColumnSpan="2">
                        <div class="ListVariable">
                            <asp:Table id="skillLevelList" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
