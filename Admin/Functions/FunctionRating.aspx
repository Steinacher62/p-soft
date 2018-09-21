<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="FunctionRating.aspx.cs" Inherits="ch.appl.psoft.Admin.Functions.FunctionRating" %>

<%@ Register Src="~/LayoutControls/LRORMRU_Layout.ascx" TagPrefix="uc1" TagName="LRORMRU_Layout" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../JavaScript/AdminHelper.js"></script>
    <script type="text/javascript" src="../../JavaScript/AdministrationFunctionRating.js"></script>
    <uc1:LRORMRU_Layout runat="server" ID="LRORMRU_Layout" />

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
        <Windows>
            <telerik:RadWindow runat="server" ID="RatingReferences" Width="500px" Height="500px" Modal="true">
                <ContentTemplate>
                    <div style="height: 95%; overflow: auto">
                        <div style="display: table; width: 95%;" class="ReferenceTable">
                            <div style="display: table-row;" class="titleRow">
                                <div style="display: table-cell;" class="adminTitleCell">
                                    <asp:Label ID="ReferenceTitle" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>
                            <div style="display: table-row;">
                                <div style="display: table-cell;">
                                    <telerik:RadGrid ID="RatingDetail" runat="server" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" ItemStyle-Height="10" AlternatingItemStyle-Height="10">
                                        <MasterTableView>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
                                                <telerik:GridBoundColumn DataField="TITLE" HeaderText="Funktionsbezeichnung" DataType="System.String">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings>
                                            <ClientEvents OnCommand="function(){}" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow runat="server" ID="RatingItemDetailWindow" Width="1000px" Height="550px" Modal="true">
                <ContentTemplate>
                    <div class="CommandRow">
                        <div style="display: table-cell;" class="CommandCell">
                            <telerik:RadImageButton CssClass="SaveImageButton" ID="SaveImageButton" runat="server" Image-Url="../Images/save_enable.gif" Image-DisabledUrl="../images/save_disable.gif" Height="20px" OnClientClicking="SaveAnforderrungDetailClick" AutoPostBack="false"></telerik:RadImageButton>
                        </div>
                    </div>
                    <asp:HiddenField ID="itemId" runat="server" />
                    <div style="height: 100%; overflow: auto">
                        <div style="display: table; width: 700px; " class="CompetenceTable">
                            <div style="display: table-row;" class="dataRow">
                                <div style="display: table-cell;" class="titleLabelCell">
                                    <asp:Label ID="itemTitleLabel" runat="server" Text="Label"></asp:Label>
                                </div>
                                <div style="display: table-cell;" class="dataLabelCell">
                                    <asp:Label ID="itemTitleData" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>
                            <div style="display: table-row;" class="dataRow">
                                <div style="display: table-cell;" class="titleLabelCell">
                                    <asp:Label ID="itemValueTitle" runat="server" Text="Label"></asp:Label>
                                </div>
                                <div style="display: table-cell;" class="dataLabelCell">
                                    <asp:Label ID="itemValueData" runat="server" Text="Label"></asp:Label>
                                </div>
                            </div>
                            <div style="display: table-row;" class="dataRow">
                                <div style="display: table-cell;" class="titleLabelCell">
                                    <asp:Label ID="itemDescriptionTitle" runat="server" Text="Label"></asp:Label>
                                </div>
                                <div style="display: table-cell;" class="dataLabelCell">
                                    <telerik:RadTextBox ID="itemDescriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" Width="500px" TextMode="MultiLine"></telerik:RadTextBox>
                                </div>
                            </div>
                            <div style="display: table-row;">
                                <div style="display: table-cell;" class="titleLabelCell">
                                    <asp:Label ID="itemExamplesTitle" runat="server"></asp:Label>
                                </div>
                                <div style="display: table-cell;" class="dataLabelCell">
                                    <telerik:RadTextBox ID="itemExamplesData" runat="server" CssClass="TextboxMultiLine" Resize="Both" Width="500px" TextMode="MultiLine" ViewStateMode="Disabled"></telerik:RadTextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

</asp:Content>
