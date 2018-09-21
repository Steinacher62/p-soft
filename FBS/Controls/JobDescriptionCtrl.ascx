<%@ Control Language="c#" AutoEventWireup="True" Codebehind="JobDescriptionCtrl.ascx.cs" Inherits="ch.appl.psoft.FBS.Controls.JobDescriptionCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "JobDescription.aspx?&jobID=<%=JobID%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "DUTY_COMPETENCE_VALIDITY", rowId, dbId)
    }
</script>
<asp:Table id="jobDescTable" runat="server" Width="100%" Height="100%">
	<asp:TableRow  Height="30" VerticalAlign="Bottom" ID="fktValueRow" runat="server" Visible="False">
		<asp:TableCell CssClass="Detail_Label" ID="fktValueLbl" Runat="server"></asp:TableCell>
		<asp:TableCell Wrap="False" CssClass="Detail_Value" ID="fktValue" Runat="server"></asp:TableCell>
		<asp:TableCell Width="100%"></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow Height="30" VerticalAlign="Top" ID="salaryValueRow" runat="server" Visible="False">
		<asp:TableCell CssClass="Detail_Label" ID="salaryValueLbl" runat="server" ></asp:TableCell>
		<asp:TableCell Wrap="False" CssClass="Detail_Value" ID="salaryValue" runat="server" ></asp:TableCell>
		<asp:TableCell Width="100%"></asp:TableCell>
	</asp:TableRow>
    <asp:TableRow Width="100%" ID="ablageRow" Height="100%">
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell" ColumnSpan="3">
            <asp:Table id="competenceTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="CompetenceListTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="Right">
                        <asp:checkbox id="CBShowValidDutyCompOnly" runat="server" AutoPostBack="True"></asp:checkbox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Height="10" ColumnSpan="2"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top" Height="100%">
                    <asp:TableCell Width="100%" ColumnSpan="2">
                        <div class="ListVariable">
                            <asp:Table id="competenceList" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
