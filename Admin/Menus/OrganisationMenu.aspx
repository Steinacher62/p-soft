<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="OrganisationMenu.aspx.cs" Inherits="ch.appl.psoft.Admin.Menus.OrganisationMenu" %>

<%@ Register Src="~/LayoutControls/LMRORU_Layout.ascx" TagPrefix="uc1" TagName="LMRORU_Layout" %>
<%@ Register Src="~/Admin/Authorisation/Controls/AuthorisationUserCtrl.ascx" TagPrefix="uc1" TagName="AuthorisationUserCtrl" %>
<%@ Register Src="~/Admin/Authorisation/Controls/AuthorisationPermissionCtrl.ascx" TagPrefix="uc1" TagName="AuthorisationPermissionCtrl" %>
<%@ Register Src="~/Admin/Authorisation/Controls/AccessorCtrl.ascx" TagPrefix="uc1" TagName="AccessorCtrl" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../JavaScript/AdminHelper.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationMenus.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationPermission.js"></script>
    <link href="../../Style/admin.css" rel="stylesheet" />
    <uc1:LMRORU_Layout runat="server" ID="LMRORU_Layout" />
    <telerik:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="AuthorisationWindow" runat="server" Width="500px" Height="800px" Modal="true">
                <ContentTemplate>
                    <asp:HiddenField ID="Id" runat="server" />
                    <asp:HiddenField ID="AuthorisationTyp" runat="server" />
                    <telerik:RadSplitter runat="server" ID="AuthorisationSplitter" Width="100%" Height="100%" Orientation="Horizontal">
                        <telerik:RadPane ID="AuthorisationPaneTop" runat="server" CssClass="PaneTop" Scrolling="None" Height="45%">
                            <div>
                                <uc1:AuthorisationUserCtrl runat="server" ID="ctl00" />
                            </div>
                        </telerik:RadPane>
                        <telerik:RadSplitBar ID="RadSplitBar1" runat="server"></telerik:RadSplitBar>
                        <telerik:RadPane ID="AuthorisationPaneBottom" runat="server" CssClass="PanePottom">
                                <uc1:AuthorisationPermissionCtrl runat="server" ID="ctl01" />
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="AccessorWindow" runat="server" Width="500px" Height="800px" Modal="true" CssClass="AccessorWindow" OnClientResizeEnd="AccessorWindowResized" OnClientBeforeShow="AccessorWindowResized" OnClientClose="AccessorWindowClose">
                <ContentTemplate>
                    <uc1:AccessorCtrl runat="server" ID="AccessorCtrl" />
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
