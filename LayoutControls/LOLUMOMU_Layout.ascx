<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LOLUMOMU_Layout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.LOLUMOMU_Layout" %>

<div class="AdminContent">
    <telerik:RadSplitter ID="Splitter" runat="server" CssClass="SplitterPage" SplitBarsSize="" Orientation="Vertical" LiveResize="True" ResizeMode="Proportional" Width="98%" Height="100%">
        <telerik:RadPane ID="PaneL" runat="server" CssClass="PaneLeft" Scrolling="None" Width="70%" >
            <telerik:RadSplitter ID="SplitterL" runat="server" CssClass="SplitterL" Orientation="Horizontal" RenderMode="Lightweight" LiveResize="True">
                <telerik:RadPane ID="PaneLO" CssClass="PaneLO" runat="server" Scrolling="none">
                    <telerik:RadMultiPage runat="server" ID="MultiPageLO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewLO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarL" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneLU" CssClass="PaneLU" Scrolling="None" runat="server" OnClientResized="PaneLUResized" Height="75%">
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
            <telerik:RadSplitter ID="SplitterM" runat="server" CssClass="SplitterM" Orientation="Horizontal" LiveResize="True">
                <telerik:RadPane ID="PaneMO" CssClass="PaneMO" runat="server" Scrolling="none">
                    <telerik:RadMultiPage runat="server" ID="MultiPageMO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewMO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarM" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneMU" CssClass="PaneMU" Scrolling="None" runat="server" OnClientResized="PaneMUResized" Height="75%">
                    <telerik:RadMultiPage runat="server" ID="MultiPageMU" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewMU" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>
