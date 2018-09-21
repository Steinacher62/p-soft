<%@ Page language="c#" Codebehind="DateSelector.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.DateSelector" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title><%=_pageTitle%></title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="../Style/Psoft.css" type="text/css" rel="stylesheet">
    </HEAD>
    <body onload="<%=_onloadString%>">
        <DIV align="center">
            <form id="DateSelector" method="post" runat="server">
                <P>&nbsp;</P>
                <P>
                    <asp:Calendar id="calendar" runat="server"></asp:Calendar></P>
                <P>
                    <asp:Button id="selectButton" runat="server" Text="Select" Enabled="true" onclick="selectButton_Click"></asp:Button></P>
            </form>
        </DIV>
    </body>
</HTML>
