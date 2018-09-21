<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="OrganisationTree.aspx.cs" Inherits="ch.appl.psoft.Organisation.OrganisationTree" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadTreeView ID="OETree" runat="server" OnNodeDataBound="OETree_NodeDataBound"   ></telerik:RadTreeView>
</asp:Content>
