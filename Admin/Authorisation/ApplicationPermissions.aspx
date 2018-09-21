<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="ApplicationPermissions.aspx.cs" Inherits="ch.appl.psoft.Admin.Authorisation.ApplicationPermissions" %>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../JavaScript/AdminHelper.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationPermission.js"></script>
    <link href="../../Style/admin.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            $find('ctl00_ContentPlaceHolder1_ApplicationAuthorisationWindow').show();
        });
    </script>
    <telerik:RadWindow ID="ApplicationAuthorisationWindow" runat="server" Width="500px" Height="800px" Modal="true" OnClientShow="ApplicationAuthorisationWindowShow">
        <ContentTemplate>
            <asp:HiddenField ID="Id" runat="server" />
            <asp:HiddenField ID="AuthorisationTyp" runat="server" />
            <telerik:RadSplitter runat="server" ID="AuthorisationSplitter" Width="100%" Height="100%" Orientation="Horizontal">
                <telerik:RadPane ID="AuthorisationPaneTop" runat="server" CssClass="PaneTop" Scrolling="None" Height="40%">
                </telerik:RadPane>
                <telerik:RadSplitBar ID="RadSplitBar1" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="AuthorisationPaneBottom" runat="server" CssClass="PanePottom">
                </telerik:RadPane>
            </telerik:RadSplitter>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="AccessorWindow" runat="server" Width="500px" Height="800px" Modal="true" CssClass="AccessorWindow" OnClientResizeEnd="AccessorWindowResized" OnClientBeforeShow="AccessorWindowResized" OnClientClose="AccessorWindowClose">
        <ContentTemplate>

        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>
