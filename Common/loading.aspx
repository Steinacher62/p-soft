<%@ Register TagPrefix="Psoft" Namespace="ch.appl.psoft.Common" Assembly="p-soft" %>
<%@ Page language="c#" Codebehind="loading.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.loading" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>loading</title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="../Style/Psoft.css" type="text/css" rel="stylesheet">
        <script  type='text/javascript'>
                var TimerID = null;
                var Done = 100;
                function Animate()
                {
                    if (Done >= 100)
                        Done = 0;
                    else
                        Done += 5;
                    setLoadingBar(Done);
                }
                
                function StartAnimation()
                {
                    Done = 100;
                    Animate();
                    TimerID = window.setInterval("Animate()",<%=_interval%>);
                }
                
                function StopAnimation()
                {
                    window.clearInterval(TimerID);
                    TimerID = null;
                }
        </script>
    </HEAD>
    <body onload="<%=_onloadString%>" onbeforeunload="StopAnimation()" MS_POSITIONING="FlowLayout">
        <form id="loading" method="post" runat="server">
            <table width="100%" height="100%" border="0">
                <tr height="100%" width="100%" valign="middle">
                    <td align="middle">
                        <P><asp:Label id="LoadingLabel" runat="server"></asp:Label></P>
                        <P>
                            <Psoft:ProgressBar id="LoadingBar" runat="server" Width="300" Height="20px" DonePercentage="0" BackColor="DarkGray" BorderColor="Black" BorderWidth="0px" CellSpacing="1"></Psoft:ProgressBar></P>
                         <input type="button" ID="close" Runat="server" onclick="if (top.hideProgressBar) top.hideProgressBar();">
                    </td>
                </tr>
            </table>
        </form>
    </body>
</HTML>
