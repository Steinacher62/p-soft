<%@ Control Language="c#" AutoEventWireup="True" Codebehind="AddUIDAssignmentsCtrl.ascx.cs" Inherits="ch.appl.psoft.Common.Controls.AddUIDAssignmentsCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellSpacing="2" cellPadding="0" class="Detail">
    <tr>
        <td height="20"></td>
    </tr>
    <tr class="InputMask">
        <td valign="top" class="InputMask_Label">
            <asp:Label ID="lblObjectTypes" Runat="server" CssClass="InputMask_Label"></asp:Label>
        </td>
        <td class="inputMask_Value">
            <asp:RadioButtonList ID="rbListObjectTypes" Runat="server" RepeatDirection="Vertical" CssClass="inputMask_Value" CellPadding="0" CellSpacing="0"></asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td valign="top" class="InputMask_Label">
            <asp:Label ID="lblTyp" Runat="server" CssClass="InputMask_Label"></asp:Label>
        </td>
        <td class="inputMask_Value">
            <asp:DropDownList ID="ddTyp" Runat="server" CssClass="inputMask_Value"></asp:DropDownList>
        </td>
    </tr>
    <tr id="structureRow" runat="server" class="InputMask">
        <td valign="top" class="InputMask_Label">
            <asp:Label ID="lblStructure" Runat="server" CssClass="InputMask_Label"></asp:Label>
        </td>
        <td class="inputMask_Value">
            <asp:DropDownList ID="ddStructure" Runat="server" CssClass="inputMask_Value"></asp:DropDownList>
        </td>
    </tr>
    <tr class="InputMask">
        <td valign="top" class="InputMask_Label">
            <asp:Label ID="lblPersonal" Runat="server" CssClass="InputMask_Label"></asp:Label>
        </td>
        <td class="inputMask_Value">
            <asp:CheckBox ID="cbPersonal" Runat="server" CssClass="inputMask_Value"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td><asp:button id="next" CssClass="Button" Runat="server"></asp:button></td>
    </tr>
</table>
