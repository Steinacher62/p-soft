<%@ Page MasterPageFile="~/Framework.Master" language="c#" Codebehind="DocumentAdd.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Dispatch.DocumentAdd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
            <table cellSpacing="0" cellPadding="3" border="0">
                <tr>
                    <td><asp:label id="title" CssClass="section_title" Runat="server"></asp:label></td>
                </tr>
                <tr>
                    <td><input id="fileNewDocument" type="file" runat="server" style="WIDTH:400px">
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="ok" Runat="server" CssClass="button" onclick="ok_Click"></asp:Button>
                    </td>
                </tr>
            </table>
      </asp:Content>
