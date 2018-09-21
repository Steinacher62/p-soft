<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChartDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Chart.Controls.ChartDetailCtrl" %>
<div class="CommandRow">
    <div style="display: table-cell;" class="CommandCell">
        <telerik:RadImageButton ID="DelteImageButton" runat="server" Image-Url="../../Images/delete_enable.gif" Image-DisabledUrl="../../images/delete_disable.gif" Height="20px" OnClientClicking="DeleteClick" AutoPostBack="false"></telerik:RadImageButton>
        <telerik:RadImageButton ID="AddImageButton" runat="server" Image-Url="../../Images/add_enabled.gif" Image-DisabledUrl="../../images/add_disabled.gif" Height="20px" OnClientClicking="AddClick" AutoPostBack="false"></telerik:RadImageButton>
    </div>
</div>
<table style="width: 100%;">
        <tr style="text-align:center"; width: 100%;">
        <td>
            <asp:Label ID="Label1" runat="server" Font-Bold="true"></asp:Label>
        </td>
    </tr>
    <tr style="align-items: center; width: 100%;">
        <td>
            <div style="overflow: auto; text-align: center" id="imageDiv">
                <img id="navigationImage" runat="server" border="0" usemap="#TreeMap" align="middle">
            </div>
            <map name="TreeMap" id="TreeMapId">
            </map>
        </td>
    </tr>
</table>

<telerik:RadContextMenu ID="ContextMenuChartItem" runat="server" OnClientShowing="ContextMenuChartItemShowing" OnClientItemClicked="ContextMenuChartItemClientItemClicked">
    <Items>
        <telerik:RadMenuItem Value="Delete"></telerik:RadMenuItem>
        <telerik:RadMenuItem Value="DeleteSubOe"></telerik:RadMenuItem>
        <telerik:RadMenuItem Value="MoveBefore"></telerik:RadMenuItem>
        <telerik:RadMenuItem Value="MoveAfter"></telerik:RadMenuItem>
        <telerik:RadMenuItem Value="InsertMissigItems"></telerik:RadMenuItem>
    </Items>
    <Targets>
        <telerik:ContextMenuTagNameTarget TagName="map" />
    </Targets>
</telerik:RadContextMenu>

<telerik:RadWindowManager ID="RadWindowManager1" runat="server">
    <Windows>
        <telerik:RadWindow runat="server" ID="NodeLinkstWindow" Visible="true" Width="600px" Height="640px" Modal="true">
            <ContentTemplate>
                 <asp:HiddenField ID="LinkId" runat="server" />
                <div class="CommandRow">
                    <div style="display: table-cell;" class="CommandCell">
                        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveNodeLinksImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveNodeLinkClick" AutoPostBack="false"></telerik:RadImageButton>
                    </div>
                </div>
                <div style="display: table; height: 30px; width: 100%">
                    <div style="display: table-row; padding-left:10px" class="titleRow">
                        <div style="display: table-cell;" class="adminTitleCell">
                            <asp:Label ID="LinkTitle" runat="server" Text="Label"></asp:Label>
                        </div>
                    </div>
                </div>
                <div style="display: table; width: 100%;" class="tableNodeLinks">
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="LabelTextTitle" runat="server"  Width="200px"></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadRadioButtonList ID="ButtonListText" runat="server" AutoPostBack="false">
                                <Items>
                                    <telerik:ButtonListItem Value="leader" />
                                    <telerik:ButtonListItem Value="orgentity" />
                                </Items>
                            </telerik:RadRadioButtonList>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="LinkTitle1" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadRadioButtonList ID="ButtonListLink" runat="server" AutoPostBack="false" ClientEvents-OnItemClicked="ButtonListLinkClicked">
                                <Items>
                                    <telerik:ButtonListItem Value="noLink" />
                                    <telerik:ButtonListItem Value="leader" />
                                    <telerik:ButtonListItem Value="personOrgentityList" />
                                    <telerik:ButtonListItem Value="clipboardOrgentity" />
                                    <telerik:ButtonListItem Value="chart" />
                                </Items>
                            </telerik:RadRadioButtonList>
                        </div>
                    </div>
                    <div style="display: table-row; visibility:collapse" id="orgRow">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="OrganisationLinkTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadDropDownList ID="OrganisationData" Width="300px" runat="server"></telerik:RadDropDownList>
                        </div>
                    </div>
                    <div style="display: table-row; visibility:collapse" id="chartRow">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="ChartLinkTitle" runat="server" ></telerik:RadLabel>
                        </div>
                       <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadDropDownList ID="ChartData" Width="300px" runat="server"></telerik:RadDropDownList>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="OpenNewWindowTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadCheckBox ID="OpenNewWindowData" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="LayoutTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadDropDownList ID="LayoutData" Width="300px" runat="server"></telerik:RadDropDownList>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow runat="server" ID="NodePiktoWindow" Visible="true" Width="600px" Height="640px" Modal="true">
            <ContentTemplate>
                <asp:HiddenField ID="PiktoId" runat="server" />
                <div class="CommandRow">
                    <div style="display: table-cell;" class="CommandCell">
                        <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveNodePiktoImageButton" runat="server" Image-Url="../../Images/save_enable.gif" Image-DisabledUrl="../../images/save_disable.gif" Height="20px" OnClientClicking="SaveNodePicktoClick" AutoPostBack="false"></telerik:RadImageButton>
                    </div>
                </div>
                <div style="display: table; height: 30px; width: 100%">
                    <div style="display: table-row;" class="titleRow">
                        <div style="display: table-cell;"  class="adminTitleCell">
                            <asp:Label ID="PiktoTitle" runat="server" Text="Label"></asp:Label>
                        </div>
                    </div>
                </div>
                <div style="display: table; width: 100%;" class="tableNodePikto">
                     <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="FreeTextTitel" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadTextBox ID="FreeTextData" ClientEvents-OnValueChanged="FreeTextDataValueChanged" runat="server"></telerik:RadTextBox>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="LabelTextPiktoTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadRadioButtonList ID="ButtonLisPiktotText" runat="server" ClientEvents-OnItemClicking="ButtonLisPiktotTextClicking" AutoPostBack="false">
                                <Items>
                                    <telerik:ButtonListItem Value="leader" />
                                    <telerik:ButtonListItem Value="orgentity" />
                                </Items>
                            </telerik:RadRadioButtonList>
                        </div>
                    </div>

                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="LinkTitlePikto1" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadRadioButtonList ID="ButtonListPikto" runat="server" AutoPostBack="false" ClientEvents-OnItemClicked="ButtonListPiktoClicked">
                                <Items>
                                    <telerik:ButtonListItem Value="noLink" />
                                    <telerik:ButtonListItem Value="leader" />
                                    <telerik:ButtonListItem Value="personOrgentityList" />
                                    <telerik:ButtonListItem Value="clipboardOrgentity" />
                                    <telerik:ButtonListItem Value="chart" />
                                </Items>
                            </telerik:RadRadioButtonList>
                        </div>
                    </div>
                    <div style="display: table-row; visibility:collapse" id="orgRowPikto">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="OrganisationPiktoTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadDropDownList ID="OrganisationPiktoData" Width="300px" runat="server"></telerik:RadDropDownList>
                        </div>
                    </div>
                    <div style="display: table-row; visibility:collapse" id="chartRowPikto">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="ChartPiktoTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadDropDownList ID="ChartPiktoData" Width="300px" runat="server"></telerik:RadDropDownList>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="OpenNewWindowPiktoTitle" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadCheckBox ID="OpenNewWindowPiktoData" runat="server" AutoPostBack="false"></telerik:RadCheckBox>
                        </div>
                    </div>
                    <div style="display: table-row;">
                        <div style="display: table-cell;" class="titleLabelCell">
                            <telerik:RadLabel ID="PiktoTitle1" runat="server" ></telerik:RadLabel>
                        </div>
                        <div style="display: table-cell;" class="dataLabelCell">
                            <telerik:RadDropDownList ID="PiktoData1" Width="300px" runat="server"></telerik:RadDropDownList>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>

    </Windows>
</telerik:RadWindowManager>
