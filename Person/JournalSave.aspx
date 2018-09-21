<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JournalSave.aspx.cs" Inherits="ch.appl.psoft.Person.JournalSave" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" ToolPanelView="None" Width="1000" Height="750" BestFitPage="True" />
        <asp:Label ID="lblOutput" runat="server" Text=""></asp:Label>
    </div>
    </div>
    </form>
</body>
</html>
