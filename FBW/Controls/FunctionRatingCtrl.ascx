<%@ Control Language="c#" AutoEventWireup="True" Codebehind="FunctionRatingCtrl.ascx.cs" Inherits="ch.appl.psoft.FBW.Controls.FunctionRatingCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="mainTable" runat="server" Width="100%" Height="100%">
    <asp:TableRow Width="100%" ID="Row1" Height="100%">
        <asp:TableCell VerticalAlign="Top" Width="50%" HorizontalAlign="Left" ID="listCell" ColumnSpan="3">
            <asp:Table id="contentTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" Width="100%" CellSpacing="0" CellPadding="3">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:label id="contentListTitle" runat="server" CssClass="section_title"></asp:label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Height="10"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top" Height="100%">
                    <asp:TableCell Width="100%">
                        <div class="ListVariable">
                            <asp:Table id="contentList" runat="server" BorderWidth="0" Width="100%"></asp:Table>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
