<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DokAblageCheckIn.ascx.cs" Inherits="ch.appl.psoft.Document.Controls.DokAblageCheckIn" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="0" cellPadding="2">
    <tr>
        <td valign="top">
            <table border="0" cellSpacing="0" cellPadding="2">
                <tr>
                    <td>
                        <asp:table id="documentTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:button id="apply" CssClass="Button" Runat="server"></asp:button>
                        <div style="DISPLAY: none">
                            <asp:textbox id="targetFileName" Runat="server"></asp:textbox>
                            <asp:textbox id="targetXFileName" Runat="server"></asp:textbox>
                            <asp:textbox id="targetFileState" Runat="server"></asp:textbox>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>