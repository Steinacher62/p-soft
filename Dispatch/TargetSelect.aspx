<%@ Page MasterPageFile="~/Framework.Master" language="c#" Codebehind="TargetSelect.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Dispatch.TargetSelect" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

            <table cellSpacing="0" cellPadding="3" border="0" height="100%">
                <tr height="100%">
                    <td>
                        <div class="ListVariable">
                            <asp:Table ID="listTab" Runat="server" CellPadding="3" CellSpacing="0"></asp:Table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="cbAll" Runat="server" AutoPostBack="True" oncheckedchanged="cbAll_CheckedChanged"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td><asp:button id="select" Runat="server" CssClass="button" onclick="select_Click"></asp:button>
                    </td>
                </tr>
            </table>
 </asp:Content>
