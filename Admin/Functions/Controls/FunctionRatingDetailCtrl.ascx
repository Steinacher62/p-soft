<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionRatingDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Functions.Controls.FunctionRatingDetailCtrl" %>


<style type="text/css">
    div.RadGrid .rgDataDiv {
        overflow-y: scroll!important;
    }
</style>


<div style="height: 100%; overflow: auto">
    <div style="display: table; width: 100%;" class="functionRatingTable">
        <div style="display: table-row;">
            <div style="display: table-cell;">
                <telerik:RadGrid ID="RatingHistoryGrid" runat="server" ClientSettings-Selecting-AllowRowSelect="true" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" ClientSettings-Scrolling-ScrollHeight="150px" ClientSettings-ClientEvents-OnRowSelected="RatingHistoryGridRowSelected" ItemStyle-Height="10" AlternatingItemStyle-Height="10">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
                            <telerik:GridBoundColumn DataField="FUNKTIONSWERT" HeaderText="Funktionswert" DataType="System.Single">
                                <HeaderStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="GUELTIG_AB" HeaderText="Gültig ab" DataType="System.DateTime" DataFormatString="{0:d.M.yyyy}">
                                <HeaderStyle Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="GUELTIG_BIS" HeaderText="Gültig bis" DataType="System.DateTime" DataFormatString="{0:d.M.yyyy}">
                                <HeaderStyle Width="100px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>

                    <ClientSettings>
                        <ClientEvents OnCommand="function(){}" />
                    </ClientSettings>

                </telerik:RadGrid>
            </div>
            <div style="display: table-cell;">
                <telerik:RadGrid ID="RatingItemsGrid" runat="server" ClientSettings-Selecting-AllowRowSelect="true" ClientSettings-Scrolling-AllowScroll="true"  ClientSettings-Scrolling-UseStaticHeaders="true" ClientSettings-Scrolling-ScrollHeight="150px" ClientSettings-ClientEvents-OnRowSelected="RatingItemsGridRowSelected" ItemStyle-Height="10" AlternatingItemStyle-Height="10" >
                    <MasterTableView>
                        <Columns>
                            <telerik:GridBoundColumn DataField="Anforderrung_ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
                            <telerik:GridBoundColumn DataField="BEZEICHNUNG" HeaderText="Bewertungsmerkmal" DataType="System.String">
                                <HeaderStyle Width="250px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Punkte" HeaderText="Punkte" DataType="System.Single">
                                <HeaderStyle Width="100px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <ClientEvents OnCommand="function(){}" />
                    </ClientSettings>
                </telerik:RadGrid>
            </div>
        </div>
        <div style="display: table-row;">
            <div style="display: table-cell;">
            </div>
            <div style="display: table-cell;">
            </div>
        </div>

    </div>
</div>
