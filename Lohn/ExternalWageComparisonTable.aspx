<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="ExternalWageComparisonTable.aspx.cs" Inherits="ch.appl.psoft.Lohn.ExternalWageComparisonTable" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <b>
            <asp:Label ID="SolllohnkorrekturLabel" runat="server" Text=""></asp:Label></b><br />
        <br />
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
                        <telerik:AjaxUpdatedControl ControlID="ServiceAgeGrid" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>

        <div>
            <b>
                <telerik:RadGrid ID="TableExternalComparison" runat="server" Height="350px" Width="400px" AutoGenerateColumns="false" AllowPaging="true" OnNeedDataSource="ExternalComparison_NeedDataSource" OnUpdateCommand="ExternalComparison_UpdateCommand">
                    <MasterTableView DataKeyNames="ID">

                        <Columns>
                            <telerik:GridBoundColumn DataField="ID" HeaderText="ID" Visible="false" />
                            <telerik:GridBoundColumn DataField="Funktion_Id" HeaderText="Funktion_Id" Visible="false" />
                            <telerik:GridBoundColumn DataField="Title_DE" HeaderText="Funktionsbezeichnung"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Extern_Funktion_Id" HeaderText="Funktionscode"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Bezeichnung" HeaderText="Bezeichnung"></telerik:GridBoundColumn>
                            <telerik:GridNumericColumn DataField="Extern_Soll1" HeaderText="Solllohn 1" DataFormatString="{0:F}">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" EnableModelErrorMessageValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Eingabe benötigt"></RequiredFieldValidator>
                                    <ModelErrorMessage BackColor="Red" />
                                </ColumnValidationSettings>
                            </telerik:GridNumericColumn>
                            <telerik:GridNumericColumn DataField="Extern_Soll2" HeaderText="Solllohn 2" DataFormatString="{0:F}">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" EnableModelErrorMessageValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Eingabe benötigt"></RequiredFieldValidator>
                                    <ModelErrorMessage BackColor="Red" />
                                </ColumnValidationSettings>
                            </telerik:GridNumericColumn>
                            <telerik:GridNumericColumn DataField="Extern_Soll3" HeaderText="Solllohn 3" DataFormatString="{0:F}">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" EnableModelErrorMessageValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Eingabe benötigt"></RequiredFieldValidator>
                                    <ModelErrorMessage BackColor="Red" />
                                </ColumnValidationSettings>
                            </telerik:GridNumericColumn>
                            <telerik:GridNumericColumn DataField="Extern_Soll4" HeaderText="Solllohn 4" DataFormatString="{0:F}">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" EnableModelErrorMessageValidation="true">
                                    <RequiredFieldValidator ForeColor="Red" ErrorMessage="Eingabe benötigt"></RequiredFieldValidator>
                                    <ModelErrorMessage BackColor="Red" />
                                </ColumnValidationSettings>
                            </telerik:GridNumericColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="rowDblClick" />
                    </ClientSettings>
                </telerik:RadGrid>
                <br />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        #Submit1 {
            width: 57px;
        }
    </style>
</asp:Content>
