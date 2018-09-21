<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ApprovementControl.ascx.cs" Inherits="ch.appl.psoft.Lohn.Controls.ApprovementControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript'>
    function listTabOnLoad()
    {
        <%=_onloadString%>
    }
</script>
<asp:Table Runat="server" id="Table1" Height="100%">
    <asp:TableRow>
        <asp:TableCell>
            <div id="oeContainer"  runat="server" style="DISPLAY: none; Z-INDEX: 100; LEFT: 100px; WIDTH: 800px; POSITION: absolute; TOP: 10px; HEIGHT: 400px; BACKGROUND-COLOR: #f8f8f8">
                <asp:Table Height="100%" Width="100%" BorderWidth="1" BorderStyle="Solid" BorderColor="#f8f8f8" Runat="server" >
                    <asp:TableRow Height="100%">
                        <asp:TableCell>
                            <div class="ListVariable">
                                <asp:Table id="oeList" BorderWidth="0" runat="server" Width="100%"></asp:Table>
                            </div>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                            <input id="cancel" type="button" onclick="javascript: document.getElementById('_pl__cl__detail_oeContainer').style.display = 'none';" class="Button" runat="server">&nbsp;
		                    <asp:Button ID="closeApprove" runat="server" CssClass="ButtonL"></asp:Button>&nbsp;
		                    <asp:Button ID="approve" runat="server" CssClass="Button" Visible="False"></asp:Button>
		                </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:Label ID="pageTitle" Runat="server" CssClass="section_title"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow Height="100%">
        <asp:TableCell>
            <div class="ListVariable">
                <asp:Table id="listTab" BorderWidth="0" runat="server" Width="100%"></asp:Table>
            </div>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:Button ID="approveAll" runat="server" CssClass="Button"></asp:Button>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
