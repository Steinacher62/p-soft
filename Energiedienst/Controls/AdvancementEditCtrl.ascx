<%@ Control Language="c#" AutoEventWireup="True" Codebehind="AdvancementEditCtrl.ascx.cs" Inherits="ch.appl.psoft.Energiedienst.Controls.AdvancementEditCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Table id="advancementTab" runat="server" CssClass="List" BorderWidth="0" Height="100%" CellSpacing="0" CellPadding="3">
    <asp:TableRow>
        <asp:TableCell>
            <asp:label id="advancementDetailTitle" runat="server" CssClass="section_title"></asp:label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell Height="10"></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="Top">
        <asp:TableCell ColumnSpan="2" Width="100%">
            <asp:checkbox id="CBTreeFlag" runat="server" AutoPostBack="True"></asp:checkbox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="Top">
        <asp:TableCell ColumnSpan="2" Width="100%">
            <asp:Table id="advancementEdit" runat="server" BorderWidth="0" Width="100%"></asp:Table>
        </asp:TableCell>
    </asp:TableRow>
    </asp:Table>
<asp:Table id="Table1" runat="server" Width="100%">
     <asp:TableRow>
        <asp:TableCell Width="150px">
            <asp:button id="apply" CssClass="Button" Runat="server" onclick="apply_Click"></asp:button>
        </asp:TableCell>
         <asp:TableCell Width ="150px">
            <asp:button id="releaseButton" Text="Freigeben" CssClass="Button" Runat="server" onclick="release_Click"></asp:button>
        </asp:TableCell>
         <asp:TableCell HorizontalAlign ="Left">
             <asp:CheckBox ID="VIEWED_FLAG" runat="server" AutoPostBack="True" Text="Besprochen und eingesehen"  />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow VerticalAlign="Top">
        
    </asp:TableRow>
</asp:Table>
