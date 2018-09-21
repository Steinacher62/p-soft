<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TrainingCatalogDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Training.Controls.TrainingCatalogDetailCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<asp:Table id="trainingCatalogDetailTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
    <asp:TableRow>
        <asp:TableCell>
            <asp:label id="trainingCatalogDetailTitle" runat="server" CssClass="section_title"></asp:label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell Height="10"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="Top" Height="100%">
        <asp:TableCell Width="100%">
            <div class="ListVariable">
                <asp:Table id="trainingCatalogDetail" runat="server" BorderWidth="0" Width="100%"></asp:Table>
            </div>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>

