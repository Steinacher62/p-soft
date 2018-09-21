<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PerformanceRatingList.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.PerformanceRatingList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<asp:Table Runat="server" id="Table1">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right">
            <asp:checkbox id="CBShowDone" runat="server" AutoPostBack="True"></asp:checkbox>
        </asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell ColumnSpan="2" Width="100%" Height="10"></asp:TableCell>
    </asp:TableRow>
	<asp:TableRow>
		<asp:TableCell ColumnSpan="2" Width="100%">
			<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>