<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LOLUMOMURORU_Layout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.LOLUMOMURORU_Layout" %>
<div class="LOLUMOMURORUContent">
    <telerik:RadSplitter ID="Splitter" runat="server" CssClass="SplitterPage" SplitBarsSize="" Orientation="Vertical" LiveResize="True" ResizeMode="Proportional">
        <telerik:RadPane ID="PaneL" runat="server" CssClass="PaneLeft" Scrolling="None">
            <telerik:RadSplitter ID="SplitterL" runat="server" Width="100%" Height="100%" CssClass="SplitterL" Orientation="Horizontal" RenderMode="Lightweight" LiveResize="True">
                <telerik:RadPane ID="PaneLO" CssClass="PaneLO" runat="server" Scrolling="none" Height="40%">
                    <telerik:RadMultiPage runat="server" ID="MultiPageLO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewLO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarL" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneLU" CssClass="PaneLU" Scrolling="None" Height="100%" runat="server">
                    <telerik:RadMultiPage runat="server" ID="MultiPageLU" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewLU" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="SplitbarLeft" runat="server">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PaneM" runat="server" CssClass="PaneM" Scrolling="None">
            <telerik:RadSplitter ID="SplitterM" runat="server" Width="100%" Height="100%" CssClass="SplitterM" Orientation="Horizontal" LiveResize="True">
                <telerik:RadPane ID="PaneMO" CssClass="PaneMO" runat="server" Scrolling="none" Height="40%">
                    <telerik:RadMultiPage runat="server" ID="MultiPageMO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewMO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarM" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneMU" CssClass="PaneMU" Scrolling="None" Height="100%" runat="server">
                    <telerik:RadMultiPage runat="server" ID="MultiPageMU" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewMU" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="SplitbarRight" runat="server">
        </telerik:RadSplitBar>
         <telerik:RadPane ID="PaneR" runat="server" CssClass="PaneR" Scrolling="None">
            <telerik:RadSplitter ID="SplitterR" runat="server" Width="100%" Height="100%" CssClass="SplitterR" Orientation="Horizontal" LiveResize="True">
                <telerik:RadPane ID="PaneRO" CssClass="PaneRO" runat="server" Scrolling="none" Height="40%">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarR" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneRU" CssClass="PaneRU" Scrolling="None" Height="100%" runat="server">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRU" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRU" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>

