<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="WageTable.aspx.cs" Inherits="ch.appl.psoft.Lohn.WageTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function WageUserAction(sender, args) {
                if (sender.get_batchEditingManager().hasChanges(sender.get_masterTableView()) &&
                    !confirm("Bei dieser Operation gehen alle Änderungen verloren. Wollen Sie die Operation trotzdem durchführen?")) {
                    args.set_cancel(true);
                }
            }
            function WageParameterMap(sender, args) {
                //If you want to send a parameter to the select call you can modify the if 
                //statement to check whether the request type is 'read':
                //if (args.get_type() == "read" && args.get_data()) {
                if (args.get_type() != "read" && args.get_data()) {
                    args.set_parameterFormat({ wageJSON: kendo.stringify(args.get_data().models) });
                }
            }

            function WageParse(sender, args) {
                if (sender._kendoDataSource._data.length == 0) {
                    var response = args.get_response().d;
                    if (response) {
                        args.set_parsedData(JSON.parse(response));
                    }

                }
                else {
                    var dataSourceTmp = $find("ctl00_ContentPlaceHolder1_WageGrid").get_masterTableView()._dataSource;
                    $.each(dataSourceTmp, function (index) { $(".CB1Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION_WAGE });
                    $.each(dataSourceTmp, function (index) { $(".CB3Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION });
                }

            }
            function changeEditor(sender, args) {
                var batchManager = $telerik.toGrid($find("ctl00_ContentPlaceHolder1_WageGrid")).get_batchEditingManager();
                //if (batchManager.get_currentlyEditedRow() == null) {
                  batchManager.openRowForEdit(sender.parentElement.parentElement.parentElement.parentElement);
                //}
                var cb = $("#" + sender.id);
                batchManager.openCellForEdit(sender.parentElement.parentElement);
                sender.checked = !sender.checked;
                setTimeout(function () {
                    batchManager._tryCloseEdits(document);
                }, 500);

            }

            function WageOnDataBound() {
                var dataSourceTmp = $find("ctl00_ContentPlaceHolder1_WageGrid").get_masterTableView()._dataSource;
                $.each(dataSourceTmp, function (index) { $(".CB1Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION_WAGE });
                $.each(dataSourceTmp, function (index) { $(".CB3Class")[index].children[0].checked = dataSourceTmp[index].EXCLUSION });
            }

            function OnKeyPress(sender, args) {

                if (args._keyCode == 13) {
                    args._domEvent.stopPropagation();
                    args._domEvent.preventDefault();
                }
            }

            function RequestFailed(sender, args) {
                alert("Bei der Operation ist ein Fehler aufgetreten! Bitte laden Sie die Seite mittels F5 neu.");
            }
        </script>
    </telerik:RadCodeBlock>



    <div>
        <b>
            <asp:Label ID="WageGridLabel" runat="server" Text=""></asp:Label></b><br />
        <br />
        <telerik:RadGrid ID="WageGrid" runat="server" ClientDataSourceID="ClientDataSource" Width="100%" AutoGenerateColumns="false" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-ClientEvents-OnDataBound="WageOnDataBound" Culture="de-DE" MasterTableView-BatchEditingSettings-EditType="Row" MasterTableView-AllowPaging="true" MasterTableView-PagerStyle-PageSizeControlType="None" AllowSorting="true" AllowFilteringByColumn="true" PageSize="50" AllowAutomaticUpdates="true">
            <MasterTableView DataKeyNames="PERSON_ID" ClientDataKeyNames="PERSON_ID" EditMode="Batch" CommandItemDisplay="Top">
                <CommandItemSettings SaveChangesText="Änderungen speichern" ShowAddNewRecordButton="false" ShowSaveChangesButton="true" ShowCancelChangesButton="false" />
                <Columns>
                    <telerik:GridBoundColumn DataField="PERSON_ID" HeaderText="Person_ID" AllowFiltering="false" Display="false" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="ORGENTITY" UniqueName="ORGENTITY" ReadOnly="true" AllowFiltering="true" />
                    <telerik:GridBoundColumn DataField="PERSONNELNUMBER" UniqueName="PERSONNELNUMBER" ReadOnly="true" AllowFiltering="false" HeaderStyle-Width="100px" />
                    <telerik:GridBoundColumn DataField="NAME" UniqueName="NAME" ReadOnly="true" AllowFiltering="true" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="FIRSTNAME" UniqueName="FIRSTNAME" ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="JOB" UniqueName="JOB" ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridNumericColumn DataField="WAGE" UniqueName="WAGE" ReadOnly="false" DefaultInsertValue="0" AllowFiltering="false" HeaderStyle-Width="100px" DataType="System.Decimal" NumericType="Number" AllowRounding="true" ItemStyle-HorizontalAlign="Right">
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
                <ClientEvents OnUserAction="WageUserAction" OnKeyPress="OnKeyPress" />
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                <KeyboardNavigationSettings AllowSubmitOnEnter="false" />
            </ClientSettings>
        </telerik:RadGrid>


        <telerik:RadClientDataSource ID="ClientDataSource" runat="server" AllowBatchOperations="true" EnableViewState="false" ViewStateMode="Disabled" ClientEvents-OnRequestFailed="RequestFailed">
            <ClientEvents OnCustomParameter="WageParameterMap" OnDataParse="WageParse" />
            <DataSource>
                <WebServiceDataSourceSettings>
                    <Select Url="GetWages" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                    <Update Url="UpdateWages" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get" />
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
</asp:Content>





