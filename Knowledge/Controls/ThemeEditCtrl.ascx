<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ThemeEditCtrl.ascx.cs" Inherits="ch.appl.psoft.Knowledge.Controls.ThemeEditCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<!-- ACHTUNG 
  diese Variablen werden im FCKEditor benötigt um die korrekten Bilder , bzw Objektlinks einzufügen
-->
  
<script type="text/javascript">
  var KnowledgeUID = <%= knowledgeUID %>;
  var ThemeUID = <%= knowledgeUID %>;
</script>



<table cellSpacing=0 cellPadding=0 border=0 width="800px">
  <tr>
    <td height=20></TD></TR>
  <tr>
    <td vAlign=top><asp:table id="editTab" Width="100%" BorderWidth="0" CssClass="InputMask" runat="server"></asp:table></TD></TR>
  <tr>
    <td height=10></TD>
  </TR>
    <tr>
    <td vAlign=top><asp:table id="colorationTab" BorderWidth="0" CssClass="InputMask" runat="server"></asp:table></TD></TR>
  <tr>
      
    <td>
        <asp:button id="save" CssClass="Button" Runat="server"></asp:button>
	    <asp:button id="apply" CssClass="Button" Runat="server"></asp:button>
	</td>
  </tr>
</TABLE>


   