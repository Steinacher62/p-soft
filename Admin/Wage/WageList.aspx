<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="WageList.aspx.cs" Inherits="ch.appl.psoft.Admin.Wage.WageList" %>

<%@ Register Src="~/LayoutControls/L_Layout.ascx" TagPrefix="uc1" TagName="L_Layout" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../JavaScript/AdminHelper.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationWageList.js"></script>
    <uc1:L_Layout runat="server" ID="L_Layout" />
</asp:Content>
