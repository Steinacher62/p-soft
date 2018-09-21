<%@ Control Language="c#" AutoEventWireup="True" Codebehind="AppraisalEditCtrl.ascx.cs" Inherits="ch.appl.psoft.Skills.Controls.AppraisalEditCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="aTable" runat="server" Width="100%" Height="100%">
    <asp:TableRow Width="100%" ID="ablageRow" Height="100%">
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell">
            <asp:Table id="skillRatingTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="SkillRatingListTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Height="10"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top" Height="100%">
                    <asp:TableCell Width="100%">
                        <div class="ListVariable">
                            <asp:Table id="skillRatingList" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell>
                        <asp:button id="apply" CssClass="Button" Runat="server" onclick="apply_Click"></asp:button>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
