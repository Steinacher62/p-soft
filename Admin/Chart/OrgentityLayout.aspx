﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="OrgentityLayout.aspx.cs" Inherits="ch.appl.psoft.Admin.Chart.OrgentityLayout" %>
<%@ Register Src="~/LayoutControls/LRORU_Layout.ascx" TagPrefix="uc1" TagName="LRORU_Layout" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../JavaScript/AdminHelper.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationChartOrgentityLayout.js"></script>
    <uc1:LRORU_Layout runat="server" ID="LRORU_Layout" />
</asp:Content>
