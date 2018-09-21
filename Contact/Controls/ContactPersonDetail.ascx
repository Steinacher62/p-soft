<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ContactPersonDetail.ascx.cs" Inherits="ch.appl.psoft.Contact.Controls.ContactPersonDetail" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!-- Person Table -->
<TABLE cellSpacing="0" cellPadding="1" width="100%" border="0">
    <TR height="10" runat="server" align="left" valign="top">
        <TD id="photoCell" style="WIDTH: 157px" align="left" valign="top"><asp:image id="PHOTO" runat="server" Height="170px"></asp:image></TD>
        <TD vAlign="top" align="left">
            <!-- Hier Adressetabelle -->
            <asp:table CellSpacing="0" CellPadding="1" Width="100%" Runat="server" id="addressTable">
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="Top">
                        <asp:Table CellSpacing="0" CellPadding="1" Runat="server" ID="companyAddressTable"></asp:table>
                    </asp:TableCell>
                    <asp:TableCell VerticalAlign="Top">
                        <asp:Table CellSpacing="0" CellPadding="1" Runat="server" ID="personDetailTable"></asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:table>
        </TD>
    </TR>
</TABLE> <!-- End of Person Table -->