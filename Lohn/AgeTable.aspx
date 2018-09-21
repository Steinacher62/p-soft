<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="AgeTable.aspx.cs" Inherits="ch.appl.psoft.Lohn.AgeTable" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                    <telerik:AjaxUpdatedControl ControlID="AgeTableGrid" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <div>
        <b>
            <asp:Label ID="AgeTableLabel" runat="server" Text=""></asp:Label></b>

        <telerik:RadGrid ID="AgeTableGrid" runat="server" Height="350px" Width="400px" AutoGenerateColumns="false" AllowPaging="true" OnNeedDataSource="AgeTableGrid_NeedDataSource" OnUpdateCommand="AgeTableGrid_UpdateCommand">
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
        <br />


    </div>
</asp:Content>
