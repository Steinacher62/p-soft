<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="CrystalReportViewer.aspx.cs" Inherits="ch.appl.psoft.Report.CrystalReportViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" ToolPanelView="None" Width="1000" Height="750" BestFitPage="True" />
        <asp:Label ID="lblOutput" runat="server" Text=""></asp:Label>
    </div>

    </asp:Content>