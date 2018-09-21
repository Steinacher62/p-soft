<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WageCorrectionDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Wage.Controls.WageCorrectionDetailCtrl" %>
<telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="TableSalaryCorrection" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<div class="AdminContent" style="height: 800px">
    <b>
        <asp:Label ID="SolllohnkorrekturLabel" runat="server" Text=""></asp:Label></b><br />
    <br />

    <telerik:RadGrid ID="TableSalaryCorrection" runat="server" ClientDataSourceID="ClientDataSource" Width="98%" AutoGenerateColumns="false" AllowFilteringByColumn="True" AllowPaging="false"  MasterTableView-BatchEditingSettings-EditType="Row" MasterTableView-AllowPaging="false" MasterTableView-PagerStyle-PageSizeControlType="None" AllowAutomaticDeletes="true">
        <MasterTableView DataKeyNames="ID" ClientDataKeyNames="ID" EditMode="Batch" CommandItemDisplay="Top" Width="100%">
            <CommandItemSettings SaveChangesText="Änderungen speichern" ShowAddNewRecordButton="false" ShowSaveChangesButton="true" ShowCancelChangesButton="false" />
            <Columns>
                <telerik:GridBoundColumn DataField="ID" UniqueName="ID" HeaderText="P-Nr." ReadOnly="true" AllowFiltering="false" Visible="false" />
                <telerik:GridBoundColumn DataField="PERSONNELNUMBER" UniqueName="PERSONNELNUMBER" HeaderText="P-Nr." ReadOnly="true" AllowFiltering="false" />
                <telerik:GridBoundColumn DataField="NAME" UniqueName="NAME" HeaderText="Name" ReadOnly="true" />
                <telerik:GridBoundColumn DataField="FIRSTNAME" UniqueName="FIRSTNAME" HeaderText="Vorname" ReadOnly="true" AllowFiltering="false" />
                 <telerik:GridBoundColumn DataField="JOB" UniqueName="JOB" HeaderText="Stellenbezeichnung" ReadOnly="true" FilterControlWidth="150px" />
                <telerik:GridBoundColumn DataField="WAGE" UniqueName="WAGE" HeaderText="Ist-Lohn" ReadOnly="true" DataFormatString="{0:N2}"  AllowFiltering="false" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridNumericColumn DataField="KORR1" UniqueName="KORR1" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                </telerik:GridNumericColumn>
                <telerik:GridNumericColumn DataField="KORR2" UniqueName="KORR2" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                </telerik:GridNumericColumn>
                <telerik:GridNumericColumn DataField="KORR3" UniqueName="KORR3" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                </telerik:GridNumericColumn>
                <telerik:GridNumericColumn DataField="KORR4" UniqueName="KORR4" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                </telerik:GridNumericColumn>
                <telerik:GridCheckBoxColumn DataField="FIX" UniqueName="FIX" DefaultInsertValue="0" AllowFiltering="false">
                </telerik:GridCheckBoxColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <ClientEvents  OnKeyPress="OnKeyPress" />
            <Scrolling AllowScroll="True" UseStaticHeaders="True" />
            <KeyboardNavigationSettings AllowSubmitOnEnter="false" />
        </ClientSettings>
    </telerik:RadGrid>
</div>

<telerik:RadClientDataSource ID="ClientDataSource" runat="server" AllowBatchOperations="true" EnableViewState="false" ViewStateMode="Disabled" ClientEvents-OnRequestFailed="RequestFailed">
    <ClientEvents OnCustomParameter="WageParameterMap" OnDataParse="WageParse" />
    <DataSource>
        <WebServiceDataSourceSettings>
            <Update Url="UpdateWagesCorrection" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get"/>
            <Select Url="GetWagesCorrection" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
        </WebServiceDataSourceSettings>
    </DataSource>
    <Schema>
        <Model ID="ID">
            <telerik:ClientDataSourceModelField FieldName="ID" DataType="Number" />
            <telerik:ClientDataSourceModelField FieldName="PERSONNELNUMBER" DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="NAME" DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="FIRSTNAME" DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="JOB" DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="WAGE"  DataType="Number" />
            <telerik:ClientDataSourceModelField FieldName="KORR1"  DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="KORR2"  DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="KORR3"  DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="KORR4" DataType="String" />
            <telerik:ClientDataSourceModelField FieldName="FIX"  DataType="Boolean" />
        </Model>
    </Schema>
</telerik:RadClientDataSource>


<style type="text/css">
    #Submit1 {
        width: 57px;
    }
</style>

