<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PersonControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.PersonControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    function personControlOnLoad()
    {
        <%=_onloadString%>
    }
</script>
<asp:Table Runat="server" id="Table1" Height="100%" BorderWidth="0">
	<asp:TableRow VerticalAlign="Top">
		<asp:TableCell>
			<asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow VerticalAlign="Top">
		<asp:TableCell>
            <asp:Table Runat="server">
	            <asp:TableRow>
		            <asp:TableCell CssClass="InputMask_Label">
                        <asp:Label ID="showAllLbl" Runat="server" CssClass="InputMask_Label"></asp:Label>
		            </asp:TableCell>
		            <asp:TableCell>
                        <asp:checkbox id="showAllCb" runat="server"></asp:checkbox>
		            </asp:TableCell>
	            </asp:TableRow>
	            <asp:TableRow>
		            <asp:TableCell CssClass="InputMask_Label">
                        <asp:Label ID="pnameLbl" Runat="server" CssClass="InputMask_Label"></asp:Label>
		            </asp:TableCell>
		            <asp:TableCell>
                        <asp:TextBox id="pname" runat="server" CssClass="InputMask_Value" AutoPostBack="False"></asp:TextBox>
		            </asp:TableCell>
	            </asp:TableRow>
	            <asp:TableRow ID="compRow" Runat="server" >
		            <asp:TableCell CssClass="InputMask_Label">
                        <asp:Label ID="compLbl" Runat="server" CssClass="InputMask_Label"></asp:Label>
		            </asp:TableCell>
		            <asp:TableCell>
		                <asp:CheckBoxList ID="compList" runat="server" CssClass="InputMask_Value" RepeatDirection="Horizontal">
		                    <asp:ListItem Selected="false" Value="1"></asp:ListItem>
		                    <asp:ListItem Selected="false" Value="2"></asp:ListItem>
		                </asp:CheckBoxList>
		            </asp:TableCell>
	            </asp:TableRow>
	            <asp:TableRow>
		            <asp:TableCell ColumnSpan="2">
                        <asp:Button id="search" runat="server" CssClass="Button"></asp:Button>&nbsp;
                        <asp:Button ID="export" Runat="server" CssClass="Button"></asp:Button>
		            </asp:TableCell>
	            </asp:TableRow>
            </asp:Table>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow id="listTabRow" Height="100%">
		<asp:TableCell>
			<DIV class="ListVariable">
				<asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
			</DIV>
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow id="oeListTabRow">
		<asp:TableCell ID="oeListTab" ColumnSpan="3"></asp:TableCell>
	</asp:TableRow>
</asp:Table>
