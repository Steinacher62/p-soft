<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SearchView.ascx.cs" Inherits="ch.appl.psoft.SPZ.Controls.SearchView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="List" border="0" Height="100%" CellSpacing="0" CellPadding="3" >
	<tr id="detailDataRow">
		<td valign="top">
			<asp:table id="detailTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
		</td>
	</tr>
	<tr>
		<td>
			<table cellSpacing="0" cellPadding="2" border="0">
				<tr>
					<td width="100"><asp:label id="semaphore" runat="server" CssClass="InputMask_Label" 
                            Visible="False"></asp:label></td>
					<td><asp:radiobutton id="rbAlle" GroupName="semaphoreGroup" Runat="server" 
                            Visible="False"></asp:radiobutton></td>
					<td id="tdGray" runat="server"><asp:radiobutton id="rbGray" 
                            GroupName="semaphoreGroup" Runat="server" Visible="False"></asp:radiobutton>
                        <asp:image id="imGray" Runat="server" ImageUrl="../../images/ampelGrau.gif" 
                            Visible="False"></asp:image></td>
					<td id="tdGreen" runat="server"><asp:radiobutton id="rbGreen" 
                            GroupName="semaphoreGroup" Runat="server" Visible="False"></asp:radiobutton>
                        <asp:image id="imGreen" Runat="server" ImageUrl="../../images/AmpelGruen.gif" 
                            Visible="False"></asp:image></td>
					<td id="tdOrange" runat="server"><asp:radiobutton id="rbOrange" 
                            GroupName="semaphoreGroup" Runat="server" Visible="False"></asp:radiobutton>
                        <asp:image id="imOrange" Runat="server" ImageUrl="../../images/AmpelOrange.gif" 
                            Visible="False"></asp:image></td>
					<td id="tdRed" runat="server"><asp:radiobutton id="rbRed" 
                            GroupName="semaphoreGroup" Runat="server" Visible="False"></asp:radiobutton>
                        <asp:image id="imRed" Runat="server" ImageUrl="../../images/AmpelRot.gif" 
                            Visible="False"></asp:image></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<asp:Button ID="apply" Runat="server" CssClass="Button"></asp:Button>
		</td>
	</tr>
</table>
