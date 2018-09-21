<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HelpVideoPlayer.aspx.cs" Inherits="ch.appl.psoft.Admin.HelpVideoPlayer" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
<telerik:RadScriptManager ID="RadScriptManager1" Runat="server">
            </telerik:RadScriptManager>
        <div>
            <telerik:RadMediaPlayer ID="VideoPlayer" runat="server"></telerik:RadMediaPlayer>
        </div>
    </form>
</body>
</html>
