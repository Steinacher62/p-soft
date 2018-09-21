<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddEditUserDetail.ascx.cs" Inherits="ch.appl.psoft.SBS.Controls.AddEditUserDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadFormDecorator runat="server" DecorationZoneID="grid" DecoratedControls="All" EnableRoundedCorners="false" />

<script type="text/javascript" src="SBSUserScripts.js"></script>
<script src="../Scripts/jquery.signalR-2.2.3.min.js"></script>
<script src="../signalr/hubs"></script>
<script src="UploadScript.js" type="text/javascript"></script>

<link href="../Style/SBS.css" rel="stylesheet" />
<div class="UserAdministration">
    <input type="hidden" id="showImportErrors" value="<%= this.showImportErrorWindow %>" />
    <div style="padding: 10px;">
        <asp:Label ID="Label1" runat="server" Text="Aktives Seminar:  "></asp:Label><telerik:RadDropDownList runat="server" ID="ddSeminars" Width="350px" OnClientSelectedIndexChanged="ddSeminarsItemSelected" EnableViewState="false"></telerik:RadDropDownList>
        <telerik:RadButton ID="btnCardSeminar" runat="server" Text="Karten zuordnen" OnClientClicked="btnCardSeminarClicked" AutoPostBack="false" CssClass="btnUserSeminar"></telerik:RadButton>
        <telerik:RadButton ID="btnImportUser" runat="server" Text="User importieren" OnClientClicked="btnImportUserClicked" AutoPostBack="false" CssClass="btnUserSeminar"></telerik:RadButton>
    </div>

    <div id="grid">
        <telerik:RadGrid ID="GridUser" runat="server" ClientDataSourceID="ClientDataSource" AllowPaging="True" AllowSorting="True" AllowFilteringByColumn="true" EnableHeaderContextFilterMenu="false" Culture="de-DE" GroupPanelPosition="Top" ClientSettings-Scrolling-AllowScroll="true" AutoGenerateColumns="false" CellSpacing="-1" GridLines="Both" Width="95%" ClientSettings-AllowKeyboardNavigation="true">
            <MasterTableView ClientDataKeyNames="ID" CommandItemDisplay="Top" EditMode="Batch">
                <CommandItemSettings ShowRefreshButton="false" AddNewRecordText="Neuer Benutzer hinzufügen" CancelChangesText="Änderungen rückgängig machen" SaveChangesText="Änderungen speichern" />
                <Columns>
                    <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" Display="false" HeaderStyle-Width="0px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PNAME" HeaderText="Name" ColumnEditorID="GridTextBoxEditor140" FilterControlWidth="80%" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FIRSTNAME" HeaderText="Vorname" ColumnEditorID="GridTextBoxEditor140" FilterControlWidth="80%" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PERSONNELNUMBER" HeaderText="RG-Nr." ColumnEditorID="GridTextBoxEditor80" AllowFiltering="false" HeaderStyle-Width="80px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FIRM" HeaderText="Firma" ColumnEditorID="GridTextBoxEditor140" FilterControlWidth="80%" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TITLE" HeaderText="Titel" ColumnEditorID="GridTextBoxEditor100" AllowFiltering="false" HeaderStyle-Width="100px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FUNKTION" HeaderText="Funktion" ColumnEditorID="GridTextBoxEditor140" AllowFiltering="false" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="EMAIL" HeaderText="eMail" ColumnEditorID="GridTextBoxEditor140" AllowFiltering="false" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LOGIN" HeaderText="Login" ColumnEditorID="GridTextBoxEditor100" AllowFiltering="false" HeaderStyle-Width="100px">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PASSWORD" HeaderText="Passwort" ColumnEditorID="GridTextBoxEditor100" AllowFiltering="false" HeaderStyle-Width="100px">
                    </telerik:GridBoundColumn>
                    <telerik:GridClientDeleteColumn HeaderText="löschen" ButtonType="ImageButton">
                        <HeaderStyle Width="70px" />
                    </telerik:GridClientDeleteColumn>
                    <telerik:GridTemplateColumn UniqueName="SeminarTemplateColumn" ItemStyle-Width="75px" HeaderStyle-Width="100px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Button ID="btnSeminarAdmin" runat="server" OnClientClick="btnSeminarAdmin(this.id); return false;" Text="Seminare" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="MatrixTemplateColumn" ItemStyle-Width="75px" HeaderStyle-Width="100px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Button ID="btnMatrixAdmin" runat="server" OnClientClick="btnMatrixAdmin(this.id); return false;" Text="Karten" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <ClientEvents OnUserAction="UserAction" />
            </ClientSettings>
            <ExportSettings>
                <Pdf PageWidth="">
                </Pdf>
            </ExportSettings>
            <ClientSettings>

                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
            </ClientSettings>
        </telerik:RadGrid>
        <telerik:GridTextBoxColumnEditor ID="GridTextBoxEditor140" runat="server" TextBoxStyle-Width="140px">
        </telerik:GridTextBoxColumnEditor>
        <telerik:GridTextBoxColumnEditor ID="GridTextBoxEditor100" runat="server" TextBoxStyle-Width="100px">
        </telerik:GridTextBoxColumnEditor>
        <telerik:GridTextBoxColumnEditor ID="GridTextBoxEditor80" runat="server" TextBoxStyle-Width="80px">
        </telerik:GridTextBoxColumnEditor>
    </div>

    <telerik:RadClientDataSource ID="ClientDataSource" runat="server" AllowBatchOperations="true">
        <ClientEvents OnCustomParameter="ParameterMap" OnDataParse="Parse" />
        <DataSource>
            <WebServiceDataSourceSettings>
                <Select Url="GetUser" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                <Update Url="UpdateUser" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get" />
                <Insert Url="InsertUser" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get" />
                <Delete Url="DeleteUser" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get" />
            </WebServiceDataSourceSettings>
        </DataSource>
        <Schema>
            <Model ID="ID">
                <telerik:ClientDataSourceModelField FieldName="ID" />
                <telerik:ClientDataSourceModelField FieldName="PERSONNELNUMBER" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="FIRM" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="TITLE" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="PNAME" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="FIRSTNAME" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="FUNKTION" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="EMAIL" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="LOGIN" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="PASSWORD" DataType="String" />
            </Model>
        </Schema>
    </telerik:RadClientDataSource>
</div>

<telerik:RadWindow ID="ExcelImport" runat="server" Title="Benutzer aus Excel importieren" Visible="true" Width="450px" Height="500px" Modal="true">
    <ContentTemplate>
        <div style="display: table">
            <div style="display: table-row">
                <div style="display: table-cell">
                    <label style="font-size: larger">Kursteilnehmende aus Excel importieren </label>
                </div>
            </div>
            <div style="display: table-row">
                <div style="display: table-cell">
                    <telerik:RadAsyncUpload runat="server" ID="AsyncUpload1"
                        HideFileInput="true"
                        AllowedFileExtensions=".xls,.xlsx,.xlsm"
                        OnFileUploaded="AsyncUpload1_FileUploaded"
                        OnClientFileUploaded="onFileUploaded"
                        PostbackTriggers="btnDummy">
                        <Localization Cancel="" DropZone="" Remove="" Select="Wählen..." />
                    </telerik:RadAsyncUpload>
                </div>
            </div>
            <div style="display: table-row">
                <div style="display: table-cell">
                    <span class="allowed-attachments" id="errorMsg" runat="server" style="color: #FF0000; visibility: inherit; display: none;">Die Datei konnte nicht verarbeitet werden.
    <br />
                        Überprüfen Sie die Datei und Starten Sie den Vorgang erneut.<br />
                    </span>
                    Wählen Sie die gewünschte Excel Datei aus.<br />

                    Erlaubte Formate: (<%= String.Join( ",", AsyncUpload1.AllowedFileExtensions ) %>)
                </div>
            </div>
            <div style="display: table-row">
                <div style="display: table-cell">
                    <asp:Button ID="btnDummy" runat="server" OnClick="btnDummy_Click" Style="display: none;"/> 
                </div>
            </div>
        </div>
    </ContentTemplate>
</telerik:RadWindow>
<telerik:RadWindow ID="ImportError" runat="server" Title="Fehler Datenimport" Width="700px" Height="500px" Modal="true">
    <ContentTemplate>
        <telerik:RadGrid ID="ErrorTable" runat="server"></telerik:RadGrid>
    </ContentTemplate>
</telerik:RadWindow>


<telerik:RadWindow ID="SeminarManagement" runat="server" VisibleOnPageLoad="false" Title="Seminare zuordnen" Width="800px" Height="600px" Modal="true">
    <ContentTemplate>
        <telerik:RadClientDataSource ID="ClientDataSourceSeminars" runat="server" AllowBatchOperations="true">
            <ClientEvents OnCustomParameter="ParameterMapSeminars" OnDataParse="ParseSeminars" />
            <DataSource>
                <WebServiceDataSourceSettings>
                    <Select Url="GetUserSeminars" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                    <Update Url="UpdateSeminars" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get"/>
                </WebServiceDataSourceSettings>
            </DataSource>
            <Schema>
                <Model ID="SEMINAR_ID">
                    <telerik:ClientDataSourceModelField FieldName="SEMINAR_ID" />
                    <telerik:ClientDataSourceModelField FieldName="SEMINAR_TITLE" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="ISACTIVE" DataType="Boolean" />
                    <telerik:ClientDataSourceModelField FieldName="PERSON_ID" DataType="Number" />
                </Model>
            </Schema>
        </telerik:RadClientDataSource>

        <table style="width: 100%;">
            <tr>
                <td colspan="2">Seminarverwaltung für:</td>
            </tr>
            <tr>
                <td>Name:</td>
                <td>
                    <asp:Label ID="SeminarverwaltungUserName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td>Vorname:</td>
                <td>
                    <asp:Label ID="SeminarverwaltungUserFirstname" runat="server"></asp:Label></td>
            </tr>
        </table>

        <telerik:RadGrid ID="GridSeminars" runat="server" AllowPaging="True" AllowSorting="True" AllowFilteringByColumn="true" EnableHeaderContextFilterMenu="false" Culture="de-DE" GroupPanelPosition="Top" ClientSettings-Scrolling-AllowScroll="true" AutoGenerateColumns="false" CellSpacing="-1" GridLines="Both" Width="100%"  ClientSettings-AllowKeyboardNavigation="true" ClientDataSourceID="ClientDataSourceSeminars" ClientSettings-ClientEvents-OnDataBound="UserSeminarBound">
            <MasterTableView ClientDataKeyNames="SEMINAR_ID" CommandItemDisplay="Top" EditMode="Batch">
                <CommandItemSettings ShowRefreshButton="false" ShowAddNewRecordButton="false" ShowCancelChangesButton="false" CancelChangesText="Änderungen rückgängig machen" SaveChangesText="Speichern" />
                <Columns>
                    <telerik:GridBoundColumn DataField="SEMINAR_ID" UniqueName="SeminarId" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PERSON_ID" UniqueName="SeminarPersonId" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SEMINAR_TITLE" UniqueName="SeminarTitle" HeaderText="Seminar" ReadOnly="false" InsertVisiblityMode="AlwaysHidden" ColumnEditorID="GridTextBoxEditor140" FilterControlWidth="100px" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn ItemStyle-Width="50px" HeaderStyle-Width="50px" DataField="ISACTIVE" DataType="System.Boolean" UniqueName="CBActive">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB1" CssClass="CB1Class" runat="server" Enabled="true" Checked='<%# Eval("ISACTIVE")==DBNull.Value ? false : Eval("ISACTIVE") %>' onclick="changeEditor(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB2" CssClass="CB2Class" runat="server" Checked='<%# Bind("ISACTIVE") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow ID="MatrixManagement" runat="server" VisibleOnPageLoad="false" Title="Sokrateskarten zuordnen" Width="800px" Height="600px" Modal="true">
    <ContentTemplate>
        <telerik:RadClientDataSource ID="ClientDataSourceMatrix" runat="server" AllowBatchOperations="true" AutoSync="false">
            <ClientEvents OnCustomParameter="ParameterMapMatrix" OnDataParse="ParseMatrix" />
            <DataSource>
                <WebServiceDataSourceSettings>
                    <Select Url="GetMatrix" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                    <Update Url="UpdateMatrix" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get" />
                </WebServiceDataSourceSettings>
            </DataSource>
            <Schema>
                <Model ID="MATRIX_ID">
                    <telerik:ClientDataSourceModelField FieldName="MATRIX_ID" DataType="Number" />
                    <telerik:ClientDataSourceModelField FieldName="PERSON_ID" DataType="Number" />
                    <telerik:ClientDataSourceModelField FieldName="MATRIX_TITLE" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="ISACTIVE" DataType="Boolean" />
                    <telerik:ClientDataSourceModelField FieldName="READWRITE" DataType="Boolean" />
                </Model>
            </Schema>
        </telerik:RadClientDataSource>

        <table style="width: 100%;">
            <tr>
                <td colspan="2">Verwaltung Sokrateskarten für:</td>
            </tr>
            <tr>
                <td>Name:</td>
                <td>
                    <asp:Label ID="MatrixverwaltungUserName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td>Vorname:</td>
                <td>
                    <asp:Label ID="MatrixverwaltungUserFirstname" runat="server"></asp:Label></td>
            </tr>
        </table>

        <telerik:RadGrid ID="GridMatrix" runat="server" ClientDataSourceID="ClientDataSourceMatrix" AllowPaging="True" AllowSorting="True" AllowFilteringByColumn="true" EnableHeaderContextFilterMenu="false" Culture="de-DE" GroupPanelPosition="Top" ClientSettings-Scrolling-AllowScroll="true" AutoGenerateColumns="false" CellSpacing="-1" GridLines="Both" Width="95%" ClientSettings-AllowKeyboardNavigation="true" ClientSettings-ClientEvents-OnDataBound="UserMatrixBound">
            <MasterTableView ClientDataKeyNames="MATRIX_ID" CommandItemDisplay="Top" EditMode="Batch">
                <CommandItemSettings ShowRefreshButton="false" ShowAddNewRecordButton="false" ShowCancelChangesButton="false" SaveChangesText="Speichern" />
                <Columns>
                    <telerik:GridBoundColumn DataField="MATRIX_ID" UniqueName="MatrixId" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PERSON_ID" UniqueName="PersonId" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="MATRIX_TITLE" HeaderText="Sokrateskarte" ReadOnly="false" InsertVisiblityMode="AlwaysHidden" ColumnEditorID="GridTextBoxEditor140" FilterControlWidth="100px" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Aktiv" ItemStyle-Width="50px" HeaderStyle-Width="50px" DataField="ISACTIVE" DataType="System.Boolean" UniqueName="CBActive">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB3" CssClass="CB3Class" runat="server" Enabled="true" Checked='<%# Eval("ISACTIVE")==DBNull.Value ? false : Eval("ISACTIVE") %>' onclick="btnISActiveCardClicked(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB4" CssClass="CB4Class" runat="server" Checked='<%# Bind("ISACTIVE") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Schreibrechte" ItemStyle-Width="50px" HeaderStyle-Width="50px" DataField="READWRITE" DataType="System.Boolean" UniqueName="CBReadWrite">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB5" CssClass="CB5Class" runat="server" Enabled="true" Checked='<%# Eval("READWRITE")==DBNull.Value ? false : Eval("READWRITE") %>' onclick="btnReadWriteCardClicked(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB6" CssClass="CB6lass" runat="server" Checked='<%# Bind("READWRITE") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow ID="MatrixManagementSeminar" runat="server" VisibleOnPageLoad="false" Title="Sokrateskarten zuordnen" Width="700px" Height="500px" Modal="true">
    <ContentTemplate>
        <telerik:RadClientDataSource ID="ClientDataSourceMatrixSeminar" runat="server" AllowBatchOperations="true" AutoSync="false" ClientEvents-OnCommand="SeminaMatrixOnCommand">
            <ClientEvents OnCustomParameter="ParameterMapMatrixSeminar" OnDataParse="ParseMatrixSeminar" />
            <DataSource>
                <WebServiceDataSourceSettings>
                    <Select Url="GetMatrixSeminar" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
                    <Update Url="UpdateMatrixSeminar" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Get" />
                </WebServiceDataSourceSettings>
            </DataSource>
            <Schema>
                <Model ID="MATRIX_ID1">
                    <telerik:ClientDataSourceModelField FieldName="MATRIX_ID1" DataType="Number" OriginalFieldName="MATRIX_ID" />
                    <telerik:ClientDataSourceModelField FieldName="SEMINAR_REF" DataType="Number" />
                    <telerik:ClientDataSourceModelField FieldName="MATRIX_TITLE" DataType="String" />
                    <telerik:ClientDataSourceModelField FieldName="ISACTIVE" DataType="Boolean" />
                    <telerik:ClientDataSourceModelField FieldName="READWRITE" DataType="Boolean" />
                </Model>
            </Schema>
        </telerik:RadClientDataSource>

        <table style="width: 100%;">
            <tr>
                <td colspan="2">Verwaltung Sokrateskarten für:</td>
            </tr>
            <tr>
                <td>Seminar:</td>
                <td>
                    <asp:Label ID="lblSeminar" runat="server"></asp:Label></td>
            </tr>

        </table>

        <telerik:RadGrid ID="GridMatrixSeminar" runat="server" ClientDataSourceID="ClientDataSourceMatrixSeminar" AllowPaging="True" AllowSorting="True" AllowFilteringByColumn="true" EnableHeaderContextFilterMenu="false" Culture="de-DE" GroupPanelPosition="Top" ClientSettings-Scrolling-AllowScroll="true" AutoGenerateColumns="false" CellSpacing="-1" GridLines="Both" Width="90%" Height="90%" ClientSettings-AllowKeyboardNavigation="true" ClientSettings-ClientEvents-OnDataBound="SeminarMatrixBound">
            <MasterTableView ClientDataKeyNames="MATRIX_ID1" CommandItemDisplay="Top" EditMode="Batch">
                <CommandItemSettings ShowRefreshButton="false" ShowAddNewRecordButton="false" ShowCancelChangesButton="false" SaveChangesText="Speichern" />
                <Columns>
                    <telerik:GridBoundColumn DataField="MATRIX_ID1" UniqueName="MatrixId1" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SEMNAR_REF" UniqueName="SeminarRef" Display="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="MATRIX_TITLE" HeaderText="Sokrateskarte" ReadOnly="false" InsertVisiblityMode="AlwaysHidden" ColumnEditorID="GridTextBoxEditor140" FilterControlWidth="100px" HeaderStyle-Width="140px">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Aktiv" ItemStyle-Width="50px" HeaderStyle-Width="50px" DataField="ISACTIVE" DataType="System.Boolean" UniqueName="CBActive">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB7" CssClass="CB7Class" runat="server" Enabled="true" Checked='<%# Eval("ISACTIVE")==DBNull.Value ? false : Eval("ISACTIVE") %>' onclick="btnISActiveCardSeminarClicked(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB8" CssClass="CB8Class" runat="server" Checked='<%# Bind("ISACTIVE") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Schreibrechte" ItemStyle-Width="50px" HeaderStyle-Width="50px" DataField="READWRITE" DataType="System.Boolean" UniqueName="CBReadWrite">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB9" CssClass="CB9Class" runat="server" Enabled="true" Checked='<%# Eval("READWRITE")==DBNull.Value ? false : Eval("READWRITE") %>' onclick="btnReadWriteCardSeminarClicked(this, event)" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:CheckBox ID="CB10" CssClass="CB10lass" runat="server" Checked='<%# Bind("READWRITE") %>' />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadNotification ID="CopyMapNotification" runat="server" Position="Center" Title="Bitte warten" Width="400px" Height="80px" LoadContentOn="FirstShow" UpdateInterval="0" AutoCloseDelay="0">
    <ContentTemplate>
        <asp:Literal ID="lblMessage" Text="Karten für Benutzer werden bereitgestellt.." runat="server"></asp:Literal>
    </ContentTemplate>
</telerik:RadNotification>



