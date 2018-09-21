<%@ Control language="c#" Codebehind="ManualMailCtrl.ascx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Dispatch.Controls.ManualMailCtrl" %>
<script  type='text/javascript'>
    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this,window.location.href,deleteConfirmMessage,"ATTACHMENT",rowId,dbId)
    }

    function startLoading()
    {
        if (parent.frames && parent.frames.loading)
        {
            if (parent.frames.document && parent.frames.document.body)
                parent.frames.document.body.rows = "0,*";
            parent.frames.loading.document.location.href='../Common/loading.aspx?mnemo=<%=_loadingMnemo%>&interval=100';
        }
    }
    
    function showMe()
    {
        if (parent.frames && parent.frames.loading)
        {
            if (parent.frames.document && parent.frames.document.body)
                parent.frames.document.body.rows = parent.firstRowSize + ",0";
            if (parent.frames.loading.StopAnimation)
                parent.frames.loading.StopAnimation();
        }
    }
</script>
<table id="mergeMailTable" cellSpacing="1" cellPadding="3" border="0" runat="server">
	<tr>
		<td height="20"></td>
	</tr>
    <tr id="rowTarget" runat="server">
        <td><asp:label id="LabelTarget" CssClass="section_title" Runat="server"></asp:label></td>
        <td></td>
        <td><asp:label id="ValueTarget" CssClass="detail_Value" Runat="server"></asp:label></td>
        <td><asp:button id="selectTarget" CssClass="button" Runat="server" onclick="popubButton_Click"></asp:button></td>
    </tr>
    <tr id="rowSeparator2" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
    <tr id="rowFrom" runat="server">
        <td><asp:label id="LabelFrom" CssClass="section_title" Runat="server"></asp:label></td>
        <td></td>
        <td><asp:textbox id="TBFrom" CssClass="detail_value" Runat="server" Width="200"></asp:textbox></td>
        <td></td>
    </tr>
    <tr id="rowSeparator3" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
    <tr id="rowTo" runat="server">
        <td><asp:label id="LabelTo" CssClass="section_title" Runat="server"></asp:label></td>
        <td></td>
        <td colSpan="2"><asp:label id="ValueTo" CssClass="detail_value" Runat="server"></asp:label></td>
    </tr>
    <tr id="rowSeparator4" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
    <tr id="rowSubject" runat="server">
        <td><asp:label id="LabelSubject" CssClass="section_title" Runat="server"></asp:label></td>
        <td></td>
        <td colSpan="2"><asp:textbox id="TBSubject" CssClass="detail_value" Runat="server" Width="400"></asp:textbox></td>
    </tr>
    <tr id="rowSeparator5" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
    <tr id="rowMailTemplate" runat="server">
        <td><asp:label id="LabelMailTemplates" CssClass="section_title" Runat="server"></asp:label></td>
        <td><asp:label id="LabelMailTemplate" CssClass="detail_Label" Runat="server"></asp:label></td>
        <td><asp:label id="ValueMailTemplate" CssClass="detail_Value" Runat="server"></asp:label></td>
        <td><asp:button id="selectMailTemplate" CssClass="button" Runat="server" onclick="popubButton_Click"></asp:button><asp:button id="addMailTemplate" CssClass="button" Runat="server" onclick="popubButton_Click"></asp:button></td>
    </tr>
    <tr id="rowLetterTemplate1" runat="server">
        <td><asp:label id="LabelLetterTemplates1" CssClass="section_title" Runat="server"></asp:label></td>
        <td><asp:label id="LabelLetterTemplate1" CssClass="detail_Label" Runat="server"></asp:label></td>
        <td colSpan="2"><asp:checkbox id="cbUseSame" Runat="server" Checked="True" AutoPostBack="True" oncheckedchanged="cbUseSame_CheckedChanged"></asp:checkbox></td>
    </tr>
    <tr id="rowLetterTemplate2" runat="server">
        <td><asp:label id="LabelLetterTemplates2" CssClass="section_title" Runat="server"></asp:label></td>
        <td></td>
        <td><asp:label id="ValueLetterTemplate" CssClass="detail_Value" Runat="server"></asp:label></td>
        <td><asp:button id="selectLetterTemplate" CssClass="button" Runat="server" onclick="popubButton_Click"></asp:button><asp:button id="addLetterTemplate" CssClass="button" Runat="server" onclick="popubButton_Click"></asp:button></td>
    </tr>
    <tr id="rowSeparator6" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
    <tr id="rowAttachments" runat="server">
        <td vAlign="top"><asp:label id="LabelAttachments" CssClass="section_title" Runat="server"></asp:label></td>
        <td></td>
        <td vAlign="top"><asp:table id="attachmentTab" Runat="server" CellSpacing="0" CellPadding="3"></asp:table></td>
        <td vAlign="top"><asp:button id="addAttachment" CssClass="button" Runat="server" onclick="popubButton_Click"></asp:button></td>
    </tr>
    <tr id="rowSeparator7" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
    <tr id="rowTestEmail" runat="server">
        <td><asp:label id="LabelTest" CssClass="section_title" Runat="server"></asp:label></td>
        <td colSpan="3"><asp:checkbox id="CBTestEmailAddress" CssClass="detail_Label" Runat="server" AutoPostBack="True" oncheckedchanged="CBTestEmailAddress_CheckedChanged"></asp:checkbox>&nbsp;<asp:textbox id="TBTestEmailAddress" Runat="server" Width="200"></asp:textbox></td>
    </tr>
    <tr id="rowSeparator8" bgColor="#edf0ff" runat="server" width="100%">
        <td colSpan="4" height="2"></td>
    </tr>
	<tr>
		<td height="20"></td>
	</tr>
    <tr valign="bottom">
        <td colSpan="4"><asp:button id="backMerge" CssClass="button" Runat="server" onclick="backMerge_Click"></asp:button>&nbsp;<asp:button id="send" CssClass="button" Runat="server" onclick="send_Click"></asp:button>
        </td>
    </tr>
</table>
<table id="testTable" cellSpacing="0" cellPadding="3" border="0" runat="server">
	<tr>
		<td height="20"></td>
	</tr>
    <tr vAlign="top">
        <td><asp:label id="testDocTitle" CssClass="section_title" Runat="server"></asp:label></td>
    </tr>
    <tr vAlign="top">
        <td><asp:label id="testDocText" CssClass="detail_value" Runat="server"></asp:label></td>
    </tr>
    <tr vAlign="top">
        <td><asp:hyperlink id="testDocLink" Runat="server" Target="_blank"></asp:hyperlink></td>
    </tr>
	<tr>
		<td height="20"></td>
	</tr>
    <tr>
        <td><asp:button id="back" CssClass="button" Runat="server" onclick="back_Click"></asp:button>&nbsp;
            <asp:button id="ok" CssClass="button" Runat="server" onclick="ok_Click"></asp:button></td>
    </tr>
</table>
<table id="mergeDocTable" cellSpacing="0" cellPadding="3" border="0" runat="server">
	<tr>
		<td height="20"></td>
	</tr>
    <tr vAlign="top">
        <td><asp:label id="mergeDocTitle" CssClass="section_title" Runat="server"></asp:label><asp:label id="mailFinishedTitle" CssClass="section_title" Runat="server"></asp:label></td>
    </tr>
    <tr vAlign="top">
        <td><asp:label id="mergeDocText" CssClass="detail_value" Runat="server"></asp:label><asp:label id="mailFinishedText" CssClass="detail_value" Runat="server"></asp:label></td>
    </tr>
    <tr vAlign="top">
        <td><asp:hyperlink id="mergeDocLink" Runat="server" Target="_blank"></asp:hyperlink></td>
    </tr>
	<tr>
		<td height="20"></td>
	</tr>
    <tr>
        <td><asp:button id="next" Runat="server" CssClass="button" onclick="next_Click"></asp:button>&nbsp;</td>
    </tr>
</table>
