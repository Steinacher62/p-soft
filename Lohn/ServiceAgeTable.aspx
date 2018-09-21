<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="ServiceAgeTable.aspx.cs" Inherits="ch.appl.psoft.Lohn.ServiceAgeTable" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <b><asp:Label ID="ServiceAgeLabel" runat="server" Text=""></asp:Label></b>
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
            <asp:Label ID="AgeTableLabel" runat="server" Text=""></asp:Label></b>

        <telerik:RadGrid ID="ServiceAgeGrid" runat="server" Height="350px" Width="400px" AutoGenerateColumns="false" AllowPaging="true" OnNeedDataSource="ServiceAgeGrid_NeedDataSource" OnUpdateCommand="ServiceAgeGrid_UpdateCommand">
            <MasterTableView DataKeyNames="SCHLUESSEL">

                <Columns>
                    <telerik:GridBoundColumn DataField="SCHLUESSEL" HeaderText="Schlüssel" ReadOnly="true" />
                    <telerik:GridNumericColumn DataField="WERT" HeaderText="Wert"  DataFormatString="{0:F}" >
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
        <asp:EntityDataSource ID="AgeTableLabelDataSource" runat="server"></asp:EntityDataSource>
        <br />


    </div>
</asp:Content>

