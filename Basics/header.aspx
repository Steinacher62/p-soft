<%@ Page CodeBehind="header.aspx.cs" Language="c#" AutoEventWireup="True" Inherits="ch.appl.psoft.Basics.header" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<HTML>
    <HEAD>
        <title>.: p-soft :.</title>
        <LINK href="../Style/Psoft.css" type="text/css" rel="STYLESHEET">
            <script  type='text/javascript'>
        var elementIDs = new Array();
        <%=_elementsScript%>
        
        function HighlightModule(module)
        {
            var i = 0;
            var ele = null;

            for (i in elementIDs)
            {
                ele = document.getElementById(elementIDs[i]);
                if (ele)
                    ele.className="header";
            }
            
            ele = document.getElementById(module + "Cell");
            if (ele)
                ele.className = "header_selected";

            ele = document.getElementById(module + "Link");
            if (ele)
                ele.className = "header_selected";
        }
            </script>
    </HEAD>
    <body bgColor="#ffffff">
        <form id="formula" method="post" runat="server">
            <table cellSpacing="0" cellPadding="0" border="0" width="100%" height="100%">
                <tr>
                    <td vAlign="top" height="100%" width="100%">
                        <img src="<%=_backgroundImage%>" alt="" border=0>
                    </td>
                </tr>
            </table>
            <div style="LEFT: 32px; POSITION: absolute; TOP: 2px; HEIGHT: 60px">
                <a href="../default.html"><IMG height="58" width="70" alt="" src="../images/transparent.gif" border="0"></a>
            </div>
            <div style="RIGHT: 10px; POSITION: absolute; TOP: 0px; HEIGHT: 60px" align="center">
                <table height="100%" border="0">
                    <tr valign="middle">
                        <td valign="middle" height="100%">
                            <a href="<%=LogoURL%>" target="_blank"><IMG alt="" src="<%=Logo%>" border="0"></a>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="LEFT: 0px; WIDTH: 100%; POSITION: absolute; TOP: 60px" runat="server" id="headerDIV">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr class="header">
                        <td align="left">
                            <asp:table id="headerTable" Runat="server" BorderWidth="0" cellpadding="0" cellspacing="0" Width="100%">
                                <asp:TableRow VerticalAlign="Middle" Runat="server" ID="Tablerow1">
                                    <asp:TableCell ID="Tablecell1" Runat="server" BorderWidth="0" VerticalAlign="Middle" HorizontalAlign="Left">
                                        <asp:table id="Table2" Runat="server" BorderWidth="0" cellpadding="0" cellspacing="0">
                                            <asp:TableRow id="headerRow" VerticalAlign="Middle" Runat="server" Height="16">
                                                <asp:TableCell ID="spacerCell" Runat="server" BorderWidth="0" VerticalAlign="Middle"></asp:TableCell>
                                            </asp:TableRow>
                                        </asp:table>
                                    </asp:TableCell>
                                    <asp:TableCell ID="loginCell" Runat="server" BorderWidth="0" VerticalAlign="Middle" HorizontalAlign="Right">
                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td ID="languageSelector" runat="server" valign="middle" class="header" height=16>
                                                    <asp:LinkButton ID="langGerman" Runat="server">DE</asp:LinkButton>
                                                    <asp:LinkButton ID="langFrench" Runat="server">FR</asp:LinkButton>
                                                    <asp:LinkButton ID="langItalian" Runat="server">IT</asp:LinkButton>
                                                    <asp:LinkButton ID="langEnglish" Runat="server">EN</asp:LinkButton>
                                                </td>
                                                <td valign="middle" class="header">&nbsp;<%=LoginInfo%>&nbsp;
                                                </td>
                                                <td valign="middle" class="header">&nbsp;<asp:HyperLink ID="logoutLink" runat="server"></asp:HyperLink>&nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:table>
                        </td>
                    </tr>
                    <tr>
                        <td height="1">
                            <IMG height="1" alt="" src="../images/transparent.gif" width="970" border="0">
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </body>
</HTML>
