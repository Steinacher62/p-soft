<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WageListDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Wage.WageListDetailCtrl" %>
<div class="AdminContent" style="height:800px">
        <b>
            <asp:Label ID="WageGridLabel" runat="server" Text=""></asp:Label></b><br />
        <br />
        <telerik:RadGrid ID="WageGrid" runat="server" ClientDataSourceID="ClientDataSource" Width="100%" AutoGenerateColumns="false" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-ClientEvents-OnDataBound="WageOnDataBound" Culture="de-DE" MasterTableView-BatchEditingSettings-EditType="Row" MasterTableView-AllowPaging="false" MasterTableView-PagerStyle-PageSizeControlType="None" AllowSorting="true" AllowFilteringByColumn="true" PageSize="3000" AllowAutomaticUpdates="true">
            <MasterTableView DataKeyNames="PERSON_ID" ClientDataKeyNames="PERSON_ID" EditMode="Batch" CommandItemDisplay="Top">
                <CommandItemSettings SaveChangesText="Änderungen speichern" ShowAddNewRecordButton="false" ShowSaveChangesButton="true" ShowCancelChangesButton="false" />
                <Columns>
                    <telerik:GridBoundColumn DataField="PERSON_ID" HeaderText="Person_ID" AllowFiltering="false" Display="false" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="ORGENTITY" UniqueName="ORGENTITY" ReadOnly="true" AllowFiltering="true" />
                    <telerik:GridBoundColumn DataField="PERSONNELNUMBER" UniqueName="PERSONNELNUMBER" ReadOnly="true" AllowFiltering="false" HeaderStyle-Width="100px" />
                    <telerik:GridBoundColumn DataField="NAME" UniqueName="NAME" ReadOnly="true" AllowFiltering="true" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="FIRSTNAME" UniqueName="FIRSTNAME" ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="JOB" UniqueName="JOB" ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridNumericColumn DataField="WAGE" UniqueName="WAGE" ReadOnly="false" DefaultInsertValue="0" AllowFiltering="false" HeaderStyle-Width="100px" DataType="System.Double" DataFormatString="{0:N2}" NumericType="Number" DecimalDigits="2"  AllowRounding="true" ItemStyle-HorizontalAlign="Right">
                    </telerik:GridNumericColumn>
                    <telerik:GridTemplateColumn ItemStyle-Width="50px" HeaderStyle-Width="100px" DataField="EXCLUSION_WAGE" DataType="System.Boolean" UniqueName="EXCLUSION_WAGE">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB1" CssClass="CB1Class" runat="server" Enabled="true" Checked='<%# Eval("EXCLUSION_WAGE")==DBNull.Value ? false : Eval("EXCLUSION_WAGE") %>' onclick="changeEditor(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB2" CssClass="CB2Class" runat="server" Checked='<%# Bind("EXCLUSION_WAGE") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn ItemStyle-Width="50px" HeaderStyle-Width="100px" DataField="EXCLUSION" DataType="System.Boolean" UniqueName="EXCLUSION">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB3" CssClass="CB3Class" runat="server" Enabled="true" Checked='<%# Eval("EXCLUSION")==DBNull.Value ? false : Eval("EXCLUSION") %>' onclick="changeEditor(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB4" CssClass="CB4Class" runat="server" Checked='<%# Bind("EXCLUSION") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn DataField="EXCLUSION_TO" UniqueName="EXCLUSION_TO" ReadOnly="false" AllowFiltering="false" DataType="System.DateTime" DataFormatString="{0:dd.MM.yyyy}" HeaderStyle-Width="150px" />
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <ClientEvents  OnKeyPress="OnKeyPress" />
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                <KeyboardNavigationSettings AllowSubmitOnEnter="false" /> 
            </ClientSettings>
        </telerik:RadGrid>


        <telerik:RadClientDataSource ID="ClientDataSource" runat="server" AllowBatchOperations="true" EnableViewState="false" ViewStateMode="Disabled" ClientEvents-OnRequestFailed="RequestFailed">
            <ClientEvents OnCustomParameter="WageParameterMap" OnDataParse="WageParse" />
            <DataSource>
                <WebServiceDataSourceSettings>
                    <Select Url="GetWages" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                    <Update Url="UpdateWages" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                </WebServiceDataSourceSettings>
            </DataSource>
            <Schema>
                <Model ID="PERSON_ID">
                    <telerik:ClientDataSourceModelField FieldName="PERSON_ID" DataType="Number" />
                    <telerik:ClientDataSourceModelField FieldName="ORGENTITY" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="PERSONNELNUMBER" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="NAME" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="FIRSTNAME" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="JOB" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="WAGE" DataType="Number" />
                    <telerik:ClientDataSourceModelField FieldName="EXCLUSION_WAGE" DataType="Boolean" />
                    <telerik:ClientDataSourceModelField FieldName="EXCLUSION" DataType="Boolean" />
                    <telerik:ClientDataSourceModelField FieldName="EXCLUSION_TO" DataType="String" />
                </Model>
            </Schema>
        </telerik:RadClientDataSource>
    </div>
