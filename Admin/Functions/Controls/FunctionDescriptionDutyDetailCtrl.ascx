<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionDescriptionDutyDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionDescriptionDutyDetailCtrl" %>
<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 100%;" class="DutyTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="detailTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="numberLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadNumericTextBox runat="server" ID="numberData" Type="Number" NumberFormat-DecimalDigits="0" CssClass="Textbox" ViewStateMode="Disabled" ReadOnly="true"></telerik:RadNumericTextBox>
            </div>
        </div>
        <div style="display: table-row;" class="dataRow">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="titleLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="titleData" runat="server" CssClass="Textbox" ViewStateMode="Disabled" ReadOnly="true"></telerik:RadTextBox>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;" class="titleLabelCell">
                <asp:Label ID="descriptionLabel" runat="server"></asp:Label>
            </div>
            <div style="display: table-cell;" class="dataLabelCell">
                <telerik:RadTextBox ID="descriptionData" runat="server" CssClass="TextboxMultiLine" Resize="Both" TextMode="MultiLine" ViewStateMode="Disabled" ReadOnly="true"></telerik:RadTextBox>
            </div>
        </div>
    </div>
    <div style="display: table; width: 100%;" class="CompetenceTable" runat="server" id="competenceTable">
        <div style="display: table-row;" class="titleRow">
            <div style="display: table-cell;" class="adminTitleCell">
                <asp:Label ID="CompetenceTitle" runat="server" Text="Label"></asp:Label>
            </div>
            <div style="display: table-cell;">
            </div>
        </div>
    </div>
</div>
<div>
    <telerik:RadWindow ID="FunctionDutyWindow" runat="server" Width="800px" Height="500px">
        <ContentTemplate>
            <div style="display: table; width: 100%;" class="FunctionDutyTable">
                <div style="display: table-row;" class="titleRow">
                    <div style="display: table-cell; padding-bottom:5px; padding-left:0px" class="adminTitleCell">
                        <asp:Label ID="FunctionTableTitleLabel" runat="server" Text="Label"></asp:Label>
                    </div>
                </div>
            </div>
            <telerik:RadGrid ID="FunctionDutyGrid" runat="server" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" Width="100%">
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
                        <telerik:GridBoundColumn DataField="TITLE" HeaderText="Name" DataType="System.String">
                            <HeaderStyle Width="430px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="VALID_FROM" HeaderText="Gültig von:" DataType="System.DateTime" DataFormatString="{0:d.M.yyyy}">
                            <HeaderStyle Width="100px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="VALID_TO" HeaderText="Gültig bis:" DataType="System.DateTime" DataFormatString="{0:d.M.yyyy}">
                            <HeaderStyle Width="100px" />
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>

                <ClientSettings>
                    <ClientEvents OnCommand="function(){}" />
                </ClientSettings>

            </telerik:RadGrid>
        </ContentTemplate>
    </telerik:RadWindow>
</div>
