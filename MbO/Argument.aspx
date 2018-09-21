<%@ Page MasterPageFile="~/Framework.Master" language="c#" Codebehind="Argument.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.MbO.Argument" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
			<table class="argumentTab" border="0" Width="100%" CellSpacing="0" CellPadding="3">
				<tr>
					<td>
						<asp:DropDownList ID="argumentList" CssClass="inputMask_Value" Runat="server"></asp:DropDownList>
					</td>
				</tr>
				<tr height="100%">
					<td>
						<asp:TextBox TextMode="MultiLine" CssClass="inputMask_Value" Width="100%" Rows="20" Runat="server" ID="argumentText"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:button id="apply" CssClass="Button" runat="server"></asp:button>
					</td>
				</tr>
			</table>
</asp:Content>