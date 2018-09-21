<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ExpectationAddView.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.ExpectationAddView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td style="LEFT: 2px; POSITION: relative"><asp:label id="pageTitle" CssClass="section_title" runat="server"></asp:label></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <tr>
        <td valign=top><asp:table id="addTab" runat="server" CssClass="InputMask" BorderWidth="0"></asp:table></td>
    </tr>
    <tr>
        <td height="10"></td>
    </tr>
    <script type="text/javascript">
    function confirmCopy(confirmText) {
        //show confirmation to copy expectation to other jobs
        return confirm(document.getElementById(confirmText).value);
    }
    </script>
    <asp:HiddenField ID="lblConfirmText" runat="server" />
    
    <tr>
        <td><asp:button id="apply" CssClass="Button" Runat="server" ></asp:button></td>
    </tr>
</table>