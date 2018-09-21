<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LRORMRU_Layout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.LRORMRU_Layout1" %>
<div class="MainContainerTitle">
    <asp:Label ID="MainContainerTitle" runat="server" Text="Label"></asp:Label>
</div>
<div class="AdminContent">
    <telerik:RadSplitter ID="Splitter" runat="server" CssClass="SplitterPage" SplitBarsSize="" Orientation="Vertical" LiveResize="True" ResizeMode="Proportional" Width="98%" Height="100%">
        <telerik:RadPane ID="PaneL" runat="server" CssClass="PaneLeft" Scrolling="None" OnClientResized="PaneLResized">
            <telerik:RadMultiPage runat="server" ID="MultiPageL" SelectedIndex="0" ScrollBars="Hidden">
                <telerik:RadPageView ID="PageViewL" runat="server" EnableViewState="false">
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="SplitbarRight" runat="server">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PaneR" runat="server" CssClass="PaneR" Scrolling="None" Width="550px">
            <telerik:RadSplitter ID="SplitterR" runat="server" Width="100%" Height="100%" CssClass="SplitterR" Orientation="Horizontal" LiveResize="True">
                <telerik:RadPane ID="PaneRO" CssClass="PaneRO" runat="server" Scrolling="none" Height="30%" OnClientResized="PaneROResized">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarRO" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneRM" CssClass="PaneRM" runat="server" Scrolling="none" Height="20%" OnClientResized="PaneRMResized">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRM" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRM" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarRU" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneRU" CssClass="PaneRU" Scrolling="None" Width="100%" Height="100%" runat="server" OnClientResized="PaneROResized">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRU" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRU" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>
