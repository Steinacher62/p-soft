<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PersonDetailView.ascx.cs" Inherits="ch.appl.psoft.Person.Controls.PersonDetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!-- Person Table -->
<TABLE id="PersonTable" cellSpacing="0" cellPadding="1" width="100%" border="0">
    <TR height="10">
        <TD colSpan="2"><asp:label id="TITLE_VALUE" runat="server" CssClass="detail_value"></asp:label><asp:label id="HEADER" runat="server" CssClass="section_title"></asp:label></TD>
    </TR>
    <TR height="10">
        <TD colSpan="2"><asp:label id="ORG_INFO" runat="server" CssClass="detail_value"></asp:label></TD>
    </TR>
    <TR height="10" align="left" valign="top">
        <TD id="photoCell" style="WIDTH: 157px" align="left" valign="top" runat="server"><asp:image id="PHOTO" runat="server" Height="170px"></asp:image></TD>
        <TD Align="top" align="left">
            <!-- Hier Adressetabelle -->
            <asp:Table CellSpacing="0" CellPadding="1" Width="100%" Runat="server" id="addressTable">
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
