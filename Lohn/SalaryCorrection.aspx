<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="SalaryCorrection.aspx.cs" Inherits="ch.appl.psoft.Lohn.SalaryCorrection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function rowDblClick(sender, eventArgs) {
                sender.get_masterTableView().editItem(eventArgs.get_itemIndexHierarchical());
            }
        </script>
    </telerik:RadCodeBlock>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="TableSalaryCorrection" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div>
        <b>
            <asp:Label ID="SolllohnkorrekturLabel" runat="server" Text=""></asp:Label></b><br />
        <br />

        <telerik:RadGrid ID="TableSalaryCorrection" runat="server" Height="350px" Width="100%" AutoGenerateColumns="false" AllowFilteringByColumn="True" AllowPaging="true" OnNeedDataSource="TableSalaryCorrection_NeedDataSource" OnUpdateCommand="TableSalaryCorrection_UpdateCommand">
            <MasterTableView DataKeyNames="PERSONNELNUMBER">

                <Columns>
                    <telerik:GridBoundColumn DataField="PERSONNELNUMBER" HeaderText="P-Nr." ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="JobTitel" HeaderText="Stellenbezeichnung" ReadOnly="true" FilterControlWidth="150px" />
                    <telerik:GridBoundColumn DataField="PNAME" HeaderText="Name" ReadOnly="true" />
                    <telerik:GridBoundColumn DataField="Firstname" HeaderText="Vorname" ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="ISTLOHN" HeaderText="Ist-Lohn" ReadOnly="true" AllowFiltering="false" />
                    <telerik:GridNumericColumn DataField="Korr1" UniqueName="Korr1" DataFormatString="{0:F}" AllowFiltering="false">
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="Korr2" UniqueName="Korr2" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="Korr3" UniqueName="Korr3" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                    </telerik:GridNumericColumn>
                    <telerik:GridNumericColumn DataField="Korr4" UniqueName="Korr4" DataFormatString="{0:F}" DefaultInsertValue="0" AllowFiltering="false">
                    </telerik:GridNumericColumn>
                    <telerik:GridCheckBoxColumn DataField="fix" UniqueName="fix" DefaultInsertValue="0" AllowFiltering="false">
                    </telerik:GridCheckBoxColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <ClientEvents OnRowDblClick="rowDblClick" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        #Submit1 {
            width: 57px;
        }
    </style>
</asp:Content>
