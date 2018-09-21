<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PendenzenSearch.ascx.cs" Inherits="ch.appl.psoft.Tasklist.Controls.PendenzenSearch" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr>
		<td height="20"></td>
	</tr>
	<tr>
		<td><asp:table id="searchTab" runat="server" CssClass="InputMask" BorderWidth="0" CellSpacing="0" CellPadding="2"></asp:table></td>
		<td align="right" valign="top"><asp:checkbox id="CBShowDone" runat="server" AutoPostBack="True" oncheckedchanged="CBShowDone_CheckedChanged"></asp:checkbox></td>
	</tr>
	<tr>
		<td>
			<table cellSpacing="0" cellPadding="2" border="0">
				<tr>
					<td><asp:label id="semaphore" runat="server" CssClass="InputMask_Label"></asp:label></td>
					<td><asp:radiobutton id="rbAlle" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton></td>
					<td id="tdDone" runat="server"><asp:radiobutton id="rbDone" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imDone" Runat="server" ImageUrl="../../images/ampelGrau.gif"></asp:image></td>
					<td id="tdGreen" runat="server"><asp:radiobutton id="rbGreen" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imGreen" Runat="server" ImageUrl="../../images/AmpelGruen.gif"></asp:image></td>
					<td id="tdOrange" runat="server"><asp:radiobutton id="rbOrange" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imOrange" Runat="server" ImageUrl="../../images/AmpelOrange.gif"></asp:image></td>
					<td id="tdRed" runat="server"><asp:radiobutton id="rbRed" GroupName="semaphoreGroup" Runat="server"></asp:radiobutton><asp:image id="imRed" Runat="server" ImageUrl="../../images/AmpelRot.gif"></asp:image></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td height="10"></td>
	</tr>
	<tr>
		<td><asp:button id="apply" CssClass="Button" Runat="server"></asp:button></td>
	</tr>
</TABLE>
