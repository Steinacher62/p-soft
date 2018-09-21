<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="Chart.aspx.cs" Inherits="ch.appl.psoft.Admin.Chart.Chart" %>

<%@ Register Src="~/LayoutControls/LRORU_Layout.ascx" TagPrefix="uc1" TagName="LRORU_Layout" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../JavaScript/AdminHelper.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationChart.js"></script>
    <uc1:LRORU_Layout runat="server" ID="LRORU_Layout" />
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
        <Windows>
            <telerik:RadWindow runat="server" ID="WindowChartDetail" Visible="true" Width="1000px" Height="800px" Modal="true">
                <ContentTemplate>
                    <div class="CommandRow">
                        <div style="display: table-cell;" class="CommandCell">
                            <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveChartDetailImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SaveChartDetailClick" AutoPostBack="false"></telerik:RadImageButton>
                        </div>
                    </div>
                    <div class="LOROUContent">
                        <telerik:RadSplitter ID="Splitter" runat="server" CssClass="SplitterPage" Orientation="Horizontal" LiveResize="True" ResizeMode="Proportional" Width="100%" Height="830px">
                            <telerik:RadPane ID="PaneO" runat="server" CssClass="PaneLeft" Scrolling="None">
                                <telerik:RadSplitter ID="SplitterOben" runat="server" Width="100%" CssClass="SplitterL" Orientation="Vertical" RenderMode="Lightweight" LiveResize="True">
                                    <telerik:RadPane ID="PaneOL" CssClass="PaneOL" runat="server" Scrolling="none">
                                        <telerik:RadMultiPage runat="server" ID="MultiPageLO" SelectedIndex="0" ScrollBars="Hidden">
                                            <telerik:RadPageView ID="PageViewOL" runat="server" EnableViewState="false">

                                                <div style="display: table; width: 100%;">
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;""  class="searchLabelCell searchLabelCellTitle">
                                                            <telerik:RadLabel ID="LabelTitle" runat="server" class="titleLabelCell"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;">
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell" >
                                                            <telerik:RadLabel ID="OrganisationTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadLabel ID="OrganisationData" runat="server"></telerik:RadLabel>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;"  class="titleLabelCell">
                                                            <telerik:RadLabel ID="LayoutTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="LayoutData" runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="TextLayoutTitle" runat="server" ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="TextLayoutData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;"  class="titleLabelCell">
                                                            <telerik:RadLabel ID="AligmentTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;"  class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="AligmentData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;"  class="titleLabelCell">
                                                            <telerik:RadLabel ID="NameTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadTextBox ID="NameData" runat="server" ViewStateMode="Disabled" ></telerik:RadTextBox>
                                                            <asp:RequiredFieldValidator ID="titleDataDataValidator" runat="server" Display="Dynamic" ControlToValidate="NameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                </div>
                                            </telerik:RadPageView>
                                        </telerik:RadMultiPage>
                                    </telerik:RadPane>
                                    <telerik:RadSplitBar ID="SplitBarL" runat="server"></telerik:RadSplitBar>
                                    <telerik:RadPane ID="PaneOR" CssClass="PaneOR" Scrolling="None" Height="100%" runat="server">
                                        <telerik:RadMultiPage runat="server" ID="MultiPageLU" SelectedIndex="0" ScrollBars="Hidden">
                                            <telerik:RadPageView ID="PageViewOR" runat="server" EnableViewState="false">
                                                <asp:HiddenField ID="NodeId" runat="server" />
                                                <div style="display: table; width: 100%;">
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;"  class="adminTitleCell">
                                                            <telerik:RadLabel ID="LabelTitleChart" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;">
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="ChartTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadLabel ID="ChartData" runat="server"></telerik:RadLabel>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell" >
                                                            <telerik:RadLabel ID="OrgentityTitle" runat="server" ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadLabel ID="OrgentityData" runat="server"></telerik:RadLabel>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="NodeLayoutTitle" runat="server" class="titleLabelCell" ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="NodeLayoutData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;"class="titleLabelCell">
                                                            <telerik:RadLabel ID="SubNodeLayoutTitle" runat="server" ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="SubNodeLayoutData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="NodeAligmentTitle" runat="server"  ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;"  class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="NodeAligmentData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="NodeTextLayoutTitle" runat="server" ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="NodeTextLayoutData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell" >
                                                            <telerik:RadLabel ID="NodeTypTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadDropDownList ID="NodeTypData"  runat="server"></telerik:RadDropDownList>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell" >
                                                            <telerik:RadLabel ID="NodeShowPersonTitle" runat="server"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadCheckBox ID="NodeShowPersonData" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="NodeOffsetVerticalLineTitle" runat="server" Visible="false"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;"  class="dataLabelCell">
                                                            <telerik:RadNumericTextBox runat="server" ID="NodeOffsetVerticalLineData" Type="Number" NumberFormat-DecimalDigits="0" CssClass="Textbox" ViewStateMode="Disabled" Display="false"></telerik:RadNumericTextBox>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;"  class="titleLabelCell">
                                                            <telerik:RadLabel ID="NodeHorizontalSpaceTitle" runat="server" Visible="false" ></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadNumericTextBox runat="server" ID="NodeHorizontalSpaceData" Type="Number" NumberFormat-DecimalDigits="0" ViewStateMode="Disabled" Display="false"></telerik:RadNumericTextBox>
                                                        </div>
                                                    </div>
                                                    <div style="display: table-row;">
                                                        <div style="display: table-cell;" class="titleLabelCell">
                                                            <telerik:RadLabel ID="NodeVerticalSpaceTitle" runat="server" Visible="false"></telerik:RadLabel>
                                                        </div>
                                                        <div style="display: table-cell;" class="dataLabelCell">
                                                            <telerik:RadNumericTextBox runat="server" ID="NodeVerticalSpaceData" Type="Number" NumberFormat-DecimalDigits="0" CssClass="Textbox" ViewStateMode="Disabled" Display="false"></telerik:RadNumericTextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </telerik:RadPageView>
                                        </telerik:RadMultiPage>
                                    </telerik:RadPane>
                                </telerik:RadSplitter>
                            </telerik:RadPane>
                            <telerik:RadSplitBar ID="SplitBarU" runat="server"></telerik:RadSplitBar>
                            <telerik:RadPane ID="RadPaneU" runat="server" CssClass="PaneLeft" Scrolling="None">
                                <telerik:RadTabStrip ID="TabStrip" runat="server" MultiPageID="MultiPageU" Width="100%">
                                    <Tabs>
                                        <telerik:RadTab PageViewID="PageviewTexts"></telerik:RadTab>
                                        <telerik:RadTab PageViewID="PageviewIcons"></telerik:RadTab>
                                    </Tabs>
                                </telerik:RadTabStrip>
                                <telerik:RadMultiPage runat="server" ID="MultiPageU" SelectedIndex="0" ScrollBars="Hidden">
                                    <telerik:RadPageView ID="PageviewTexts" runat="server" EnableViewState="false">
                                        <telerik:RadGrid ID="TextLinkGrid" runat="server" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" Width="100%" CssClass="GridChartLink" ClientSettings-ClientEvents-OnRowContextMenu="ContextMenuLinkShow" EnableHeaderContextMenu="true" ClientSettings-Selecting-AllowRowSelect="true">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
                                                    <telerik:GridBoundColumn DataField="TEXT" HeaderText="Text" DataType="System.String">
                                                        <HeaderStyle Width="45%" />
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="LINK" HeaderText="Verknüpfung" DataType="System.String">
                                                        <HeaderStyle Width="45%" />
                                                    </telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                            <ClientSettings>
                                                <ClientEvents OnCommand="function(){}" />
                                            </ClientSettings>
                                        </telerik:RadGrid>
                                        <telerik:RadContextMenu ID="contextMenuLink" runat="server" EnableRoundedCorners="true" OnClientItemClicked="ContextMenuLinkClicked">
                                            <Items>
                                                <telerik:RadMenuItem Text="AddLink" Value="AddLink">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="EditLink" Value="EditLink">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="DeleteLink"  Value="DeleteLink">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="MoveUpLink" Value="MoveUpLink">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="MoveDownLink" Value="MoveDownLink">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="CopyLink" Value="CopyLink">
                                                </telerik:RadMenuItem>
                                            </Items>
                                        </telerik:RadContextMenu>
                                    </telerik:RadPageView>
                                    <telerik:RadPageView ID="PageviewIcons" runat="server" EnableViewState="false">
                                        <telerik:RadGrid ID="IconLinkGrid" runat="server" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" Width="100%" CssClass="GridChartLink" ClientSettings-ClientEvents-OnRowContextMenu="ContextMenuPiktoShow" EnableHeaderContextMenu="true" ClientSettings-Selecting-AllowRowSelect="true">
                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
                                                    <telerik:GridBoundColumn DataField="TEXT" HeaderText="Text" DataType="System.String">
                                                        <HeaderStyle Width="33%" />
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="LINK" HeaderText="Verknüpfung" DataType="System.String">
                                                        <HeaderStyle Width="33%" />
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="ICON" HeaderText="Piktogramm" DataType="System.String">
                                                        <HeaderStyle Width="33%" />
                                                    </telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                            <ClientSettings>
                                                <ClientEvents OnCommand="function(){}" />
                                            </ClientSettings>
                                        </telerik:RadGrid>
                                        <telerik:RadContextMenu ID="contextMenuPikto" runat="server" EnableRoundedCorners="true" OnClientItemClicked="ContextMenuPiktoClicked">
                                            <Items>
                                                <telerik:RadMenuItem Text="AddPikto" Value="AddPikto">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="EditPikto" Value="EditPikto">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="DeletePikto" Value="DeletePikto">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="MoveUpPikto" Value="MoveUpPikto">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="MoveDownPikto" Value="MoveDownPikto">
                                                </telerik:RadMenuItem>
                                                <telerik:RadMenuItem Text="CopyPikto" Value="CopyPikto">
                                                </telerik:RadMenuItem>
                                            </Items>
                                        </telerik:RadContextMenu>
                                    </telerik:RadPageView>
                                </telerik:RadMultiPage>
                            </telerik:RadPane>
                        </telerik:RadSplitter>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow runat="server" ID="NewChartWindow" Visible="true" Width="600px" Height="800px" Modal="true">
                <ContentTemplate>
                    <telerik:RadSplitter ID="NewChartSplitter" runat="server" CssClass="SplitterPage" Orientation="Horizontal" LiveResize="True" ResizeMode="Proportional" Width="100%" Height="100%">
                        <telerik:RadPane ID="OEPane" runat="server" CssClass="PaneLeft" Scrolling="None" OnClientResizing="OEPaneResizing">
                            <div class="CommandRow">
                                <div style="display: table-cell;" class="CommandCell">
                                    <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveNewChartImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SaveNewChartClick" AutoPostBack="false"></telerik:RadImageButton>
                                </div>
                            </div>
                            <div style="display: table; height: 100%; width: 100%">
                                <div style="display: table-row;" class="titleRow">
                                    <div style="display: table-cell;" class="adminTitleCell">
                                        <asp:Label ID="TreeTitle" runat="server" Text="Label"></asp:Label>
                                    </div>
                                    <div style="display: table-cell;">
                                    </div>
                                </div>

                                <div style="display: table-row;">
                                    <div style="display: table-cell; padding: 1px;">
                                        <telerik:RadTreeView ID="OETree1" CssClass="Tree" runat="server" AllowNodeEditing="False" OnNodeDataBound="OETree_NodeDataBound" OnClientNodeClicking="OETreeNodeClicking" Height="303px" OnClientNodeExpanded="OETree1NodeExpanded">
                                        </telerik:RadTreeView>
                                    </div>
                                </div>
                            </div>

                        </telerik:RadPane>
                        <telerik:RadSplitBar ID="NewChartSplitBar" runat="server"></telerik:RadSplitBar>
                        <telerik:RadPane ID="NewChartDetailPane" runat="server" CssClass="PaneLeft" Scrolling="None">
                            <div style="display: table; width: 100%;" class="tableNewChart">
                                <div style="display: table-row;">
                                    <div style="display: table-cell;"   class="adminTitleCell">
                                        <telerik:RadLabel ID="LabelTitleNewChart" runat="server" class="titleLabelCell"></telerik:RadLabel>
                                    </div>
                                    <div style="display: table-cell;">
                                    </div>
                                </div>
                                <div style="display: table-row;">
                                    <div style="display: table-cell;"  class="titleLabelCell">
                                        <telerik:RadLabel ID="NewChartTitle" runat="server"></telerik:RadLabel>
                                    </div>
                                    <div style="display: table-cell;"  class="dataLabelCell">
                                        <telerik:RadLabel ID="NewChartData" runat="server"></telerik:RadLabel>
                                    </div>
                                </div>
                                <div style="display: table-row;">
                                    <div style="display: table-cell;"  class="titleLabelCell">
                                        <telerik:RadLabel ID="NewChartNodeLayoutTitle" runat="server"></telerik:RadLabel>
                                    </div>
                                    <div style="display: table-cell;" class="dataLabelCell">
                                        <telerik:RadDropDownList ID="NewChartNodeLayoutData"  runat="server"></telerik:RadDropDownList>
                                    </div>
                                </div>
                                <div style="display: table-row;">
                                    <div style="display: table-cell;"  class="titleLabelCell">
                                        <telerik:RadLabel ID="NewChartNodeTextLayoutTitle" runat="server"></telerik:RadLabel>
                                    </div>
                                    <div style="display: table-cell;"  class="dataLabelCell">
                                        <telerik:RadDropDownList ID="NewChartNodeTextLayoutData"  runat="server"></telerik:RadDropDownList>
                                    </div>
                                </div>
                                <div style="display: table-row;">
                                    <div style="display: table-cell;"  class="titleLabelCell">
                                        <telerik:RadLabel ID="NewChartTextAligmnetTitle" runat="server"></telerik:RadLabel>
                                    </div>
                                    <div style="display: table-cell;"  class="dataLabelCell">
                                        <telerik:RadDropDownList ID="NewChartTextAligmnetData"  runat="server"></telerik:RadDropDownList>
                                    </div>
                                </div>
                                <div style="display: table-row;">
                                    <div style="display: table-cell;"  class="titleLabelCell">
                                        <telerik:RadLabel ID="NewChartNameTitle" runat="server"></telerik:RadLabel>
                                    </div>
                                    <div style="display: table-cell;"  class="dataLabelCell">
                                        <telerik:RadTextBox ID="NewChartNameData" runat="server" CssClass="Textbox" ViewStateMode="Disabled" ></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator ID="NewChartNameDataDataValidator" runat="server" Display="Dynamic" ControlToValidate="NewChartNameData" ErrorMessage="Eingabe erforderlich!" CssClass="FieldValidator" Enabled="false"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </ContentTemplate>
            </telerik:RadWindow>

        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
