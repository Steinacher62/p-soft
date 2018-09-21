﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LRORU_Layout.ascx.cs" Inherits="ch.appl.psoft.LayoutControls.LRORU_Layout" %>
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
        <telerik:RadPane ID="PaneR" runat="server" CssClass="PaneR" Scrolling="None" Width ="550px" OnClientResized="PaneROResized">
            <telerik:RadSplitter ID="SplitterR" runat="server" Width="100%" Height="100%" CssClass="SplitterR" Orientation="Horizontal" LiveResize="True">
                <telerik:RadPane ID="PaneRO" CssClass="PaneRO" runat="server" Scrolling="none" Height="30%" OnClientResized="PaneROResized">
                    <telerik:RadMultiPage runat="server" ID="MultiPageRO" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRO" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="SplitBarR" runat="server"></telerik:RadSplitBar>
                <telerik:RadPane ID="PaneRU" CssClass="PaneRU" Scrolling="None"  Width="100%" Height="100%" runat="server" OnClientResized="PaneRUResized"  >
                    <telerik:RadMultiPage runat="server" ID="MultiPageRU" SelectedIndex="0" ScrollBars="Hidden">
                        <telerik:RadPageView ID="PageViewRU" runat="server" EnableViewState="false">
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>
