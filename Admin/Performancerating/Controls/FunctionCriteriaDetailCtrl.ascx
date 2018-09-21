<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FunctionCriteriaDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Admin.Performancerating.Controls.FunctionCriteriaDetailCtrl" %>

<%--<telerik:RadAjaxManager runat="server" ID="RadAjaxManager4">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="FunctionCriteriaGrid">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="FunctionCriteriaGrid" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>--%>

<div style="display: table;" class="FunctionCriteriaTable">
    <div style="display: table-row;" class="titleRow">
        <div style="display: table-cell; padding-bottom: 5px; padding-left: 15px; width: 500px" class="adminTitleCell">
            <asp:Label ID="FunctionCriteriaTableTitle" runat="server" Text="Label"></asp:Label>
        </div>
    </div>
</div>
<telerik:RadGrid ID="FunctionCriteriaGrid" runat="server" ClientSettings-Scrolling-AllowScroll="true" ClientSettings-Scrolling-UseStaticHeaders="true" Width="95%" ClientSettings-ClientEvents-OnRowSelected="FunctionCriteriaGridOnRowSelected">
    <MasterTableView EditMode="Batch">
        <Columns>
            <telerik:GridBoundColumn DataField="ID" HeaderText="ID" DataType="System.Int32" Visible="false" />
            <telerik:GridBoundColumn DataField="NAME" HeaderText="Funktion" DataType="System.String" ReadOnly="true">
                <HeaderStyle Width="430px" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CRITERIA" HeaderText="Kriterium" DataType="System.String" ReadOnly="true">
                <HeaderStyle Width="430px" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="WEIGHT" HeaderText="Gewichtung" DataType="System.Double" DataFormatString="{0:N}">
            </telerik:GridBoundColumn>

        </Columns>
    </MasterTableView>

    <ClientSettings>
        <ClientEvents OnCommand="function(){}" />
        <ClientEvents OnBatchEditOpening="BatchEditOpening"/>
    </ClientSettings>

</telerik:RadGrid>
