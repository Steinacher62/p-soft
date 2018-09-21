<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminContentLayout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.AdminContentLayout" %>
<script src="../JavaScript/Administration.js"></script>
<script src="../JavaScript/AdministrationPermission.js"></script>
<script src="../JavaScript/AdminHelper.js"></script>
<div class="MainContainerTitle">
    <asp:Label ID="MainContainerTitle" runat="server" Text="Label"></asp:Label>
</div>
<div class="AdminOrganisationContent" >
    <asp:HiddenField ID="imageUrl" runat="server" />
    <telerik:RadSplitter ID="Splitter" runat="server" CssClass="SplitterAdmin" SplitBarsSize="" Orientation="Vertical" RenderMode="Lightweight" LiveResize="True" ResizeMode="Proportional" Width="98%" Height="100%">
        <telerik:RadPane ID="PaneLeft" runat="server" CssClass="PaneLeft" Scrolling="None" OnClientResized="PaneLeftResized">
            <telerik:RadMultiPage runat="server" ID="MultiPageLeft" SelectedIndex="0" ScrollBars="Hidden">
                <telerik:RadPageView ID="PageViewOrganisation" runat="server" EnableViewState="false">
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="Splitbar1" runat="server">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PaneCenter" runat="server" CssClass="PaneCenter" Scrolling="None">
            <telerik:RadTabStrip ID="TabStripCenter" runat="server" MultiPageID="MultiPageCenter" Width="100%"></telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="MultiPageCenter" Height="100%">
                <telerik:RadPageView ID="PageviewPersonDetail" runat="server" EnableViewState="false" TabIndex="0">
                </telerik:RadPageView>
                <telerik:RadPageView ID="PageViewOrganisationDetail" runat="server" EnableViewState="false" TabIndex="1">
                </telerik:RadPageView>
                <telerik:RadPageView ID="PageViewJobDetail" runat="server" EnableViewState="false" TabIndex="2">
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="Splitbar2" runat="server">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PaneRight" runat="server" CssClass="PaneRight" Scrolling="None" Height="100%">
            <telerik:RadSplitter ID="SplitterRight" runat="server"  CssClass="SplitterRight" Orientation="Horizontal" RenderMode="Lightweight" LiveResize="True">
                <telerik:RadPane ID="PaneRightTop" CssClass="PaneRight" runat="server" Scrolling="none">
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBar3" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneRightBottom" CssClass="PaneRight PaneRightBottom" Scrolling="None" runat="server" OnClientResized="PaneRightBottomResized">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRightBottom" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewPersonTree" runat="server" EnableViewState="false" >
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
    </telerik:RadSplitter>

    <telerik:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="ClipboardWindow" runat="server" Width="900px" Height="500px" Modal="true">
                <ContentTemplate>
                    <telerik:RadSplitter runat="server" ID="ClipboardSplitter" Width="100%">
                        <telerik:RadPane ID="ClipboardPaneLeft" runat="server" CssClass="PaneLeft" Scrolling="None" Width="30%">
                        </telerik:RadPane>
                        <telerik:RadSplitBar ID="SplitBar" runat="server"></telerik:RadSplitBar>
                        <telerik:RadPane ID="ClipboardPaneRight" runat="server" CssClass="PaneCenter">
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="EmploymentWindow" runat="server" Width="800px" Height="500px" Modal="true">
            </telerik:RadWindow>
            <telerik:RadWindow ID="AuthorisationWindow" runat="server" Width="500px" Height="650px" Modal="true" >
                <ContentTemplate>
                    <asp:HiddenField ID="Id" runat="server" />
                    <asp:HiddenField ID="AuthorisationTyp" runat="server" />
                    <telerik:RadSplitter runat="server" ID="AuthorisationSplitter" Width="100%" Height="100%" Orientation="Horizontal">
                        <telerik:RadPane ID="AuthorisationPaneTop" runat="server" CssClass="PaneTop" Scrolling="None" Height="43%">
                        </telerik:RadPane>
                        <telerik:RadSplitBar ID="RadSplitBar1" runat="server"></telerik:RadSplitBar>
                        <telerik:RadPane ID="AuthorisationPaneBottom" runat="server" CssClass="PanePottom">
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="AccessorWindow" runat="server" Width="500px" Height="600px" Modal="true"  CssClass="AccessorWindow" OnClientResizeEnd="AccessorWindowResized" OnClientBeforeShow="AccessorWindowResized" OnClientClose="AccessorWindowClose">
            </telerik:RadWindow>
            <telerik:RadWindow ID="AddressWindow" runat="server" Width="800px" Height="650px" Modal="true">
                <ContentTemplate>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</div>
