<%@ Page MasterPageFile="~/Framework.Master" language="c#" Codebehind="PrintJobDescription.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Laufenburg.PrintJobDescription" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" ToolPanelView="None" Width="1000" Height="750" BestFitPage="True" />
    </div>

  </asp:Content>