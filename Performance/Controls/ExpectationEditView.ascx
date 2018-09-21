<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ExpectationEditView.ascx.cs" Inherits="ch.appl.psoft.Performance.Controls.ExpectationEditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<TABLE id="Table1" cellSpacing="0" cellPadding="2" border="0">
	<TR>
		<TD><asp:label id="TITLE_VALUE" CssClass="section_title" runat="server"></asp:label></TD>
	</TR>
</TABLE>
<asp:table id="editTab" CssClass="InputMask" runat="server" BorderWidth="0"></asp:table>
<script type="text/javascript">
    function confirmCopy(confirmText) {
        //show confirmation to copy expectation to other jobs
        return confirm(document.getElementById(confirmText).value);
    }
</script>
<asp:HiddenField ID="lblConfirmText" runat="server" />
<asp:button id="apply" CssClass="Button" Runat="server"></asp:button>