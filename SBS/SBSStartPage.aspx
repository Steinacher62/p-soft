<%@  MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true"CodeBehind="SBSStartPage.aspx.cs" Inherits="ch.appl.psoft.SBS.SBSStartPage" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:PlaceHolder ID="startPageMenue" runat="server">
    <link href="SBSSheet.css" rel="stylesheet" />
    <link href="SBSSeminarTable.css" rel="stylesheet" type="text/css" />
    <script src="SBSSeminarTableScript.js"  type="text/javascript"></script>
        </asp:PlaceHolder>    
</asp:Content>
