<%@ Control Language="c#" AutoEventWireup="True" Codebehind="AddSkillLevelValidityHandsFreeCtrl.ascx.cs" Inherits="ch.appl.psoft.Skills.Controls.AddSkillLevelValidityHandsFreeCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="jobDescTable" runat="server" Width="100%">
    <asp:TableRow Width="100%" ID="ablageRow">
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell">
            <asp:Table id="competenceTab" runat="server" CssClass="List" BorderWidth="0" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2">
                        <asp:label id="CompetenceTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell Height="10"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2" Width="100%">
                        <asp:Table id="competenceCB" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2" Width="100%">
                        <asp:Table id="competenceEdit" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell height="10"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2">
                        <asp:button id="apply" CssClass="Button" Runat="server" onclick="apply_Click"></asp:button>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
