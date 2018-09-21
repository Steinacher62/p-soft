<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="Seminars.aspx.cs" Inherits="ch.appl.psoft.SBS.Seminars" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="detail" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <link href="../Style/SBS.css" rel="stylesheet" />
    <script type="text/javascript" src="SBSSeminarScripts.js"></script> 
    <telerik:RadClientDataSource ID="ClientDataSourceDataForm" runat="server" AllowPaging="true" EnableViewState="true">
        <ClientEvents OnCustomParameter="ParameterMap" OnDataParse="Parse" />
        <DataSource>
            <WebServiceDataSourceSettings>
                <Select Url="GetData" DataType="JSON" ContentType="application/json; charset=utf-8" RequestType="Post" />
            </WebServiceDataSourceSettings>
        </DataSource>
        <Schema>
            <Model ID="UID">
                <telerik:ClientDataSourceModelField FieldName="UID" DataType="Number" />
                <telerik:ClientDataSourceModelField FieldName="ID" DataType="Number" />
                <telerik:ClientDataSourceModelField FieldName="URL" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="NAME" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="DESCRIPTION" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="SEMINAR_ID" DataType="Number" />
                <telerik:ClientDataSourceModelField FieldName="PARENT_ID" DataType="Number" />
                <telerik:ClientDataSourceModelField FieldName="AUTHOR" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="DOZENT" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="FOLDER_ID" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="ROOT_ID" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="DATUM" DataType="String" />
                <telerik:ClientDataSourceModelField FieldName="RELEASE" DataType="Date" />
            </Model>
        </Schema>
    </telerik:RadClientDataSource>

    <asp:HiddenField ID="seminarId" runat="server"/>
    <asp:HiddenField ID="seminarUID" runat="server"/>

    <table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
        <tr>
            <td height="20"></td>
        </tr>
        <tr>

            <td>
                <asp:Label ID="LabelSeminars" runat="server" Text="Zu bearbeitendes Seminar:"></asp:Label>
                <telerik:RadComboBox ID="CBSeminars" runat="server" DataTextField="NAME" DataValueField="ID" Width="350px" ViewStateMode="Enabled" OnClientSelectedIndexChanged="CBSeminarsIndexChanged">

                </telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td>
                <table id="inputExportTab" border="0" cellspacing="0" cellpadding="2">
                    <tr>
                        <td class="CellFileExplorer">
                            <telerik:RadFileExplorer ID="FileExplorer" runat="server" DisplayUpFolderItem="True" Language="de-DE" EnableCreateNewFolder="true" Configuration-AllowMultipleSelection="false" Configuration-MaxUploadFileSize="104857600" FilterTextBoxLabel="Filter By" EnableOpenFile="true" ClientIDMode="AutoID" EnableAsyncUpload="true" OnClientFolderChange="FileExplorerFolderChange" OnClientCreateNewFolder="FileExplorerNewFolder" OnClientItemSelected="FileExplorer_ItemSelected" OnClientDelete="FileExplorerItemDelete" OnClientMove="FileExplorerItemMove" OnClientFolderLoaded="FileExplorerFolderLoaded" OnClientLoad="FileExplorerClientLoad" EnableViewState="true" ViewStateMode="Enabled" Width="1000px" TreePaneWidth="300px">
                                <Configuration SearchPatterns="*.PDF, *.HTML" AllowMultipleSelection="False" MaxUploadFileSize="104857600"></Configuration>
                                
                            </telerik:RadFileExplorer>
                        </td>
                        <td>
                            <div id="dataForms">
                                <telerik:RadDataForm ItemPlaceholderID="itemPlaceholder1" RenderWrapper="true" ID="DataFormSeminar" ClientDataSourceID="ClientDataSourceDataForm" runat="server" CssClass="DataForms1">
                                    <ClientSettings>
                                        <ClientEvents OnDataFormCreated="DataFormSeminarCreated" OnSetValues="DataFormOnSetValues" OnGetValues="DataFormOnGetValues" />
                                     </ClientSettings>
                                    <LayoutTemplate>
                                        <div class="SeminarEdit">
                                            <div class="sbsHeader ">
                                                <div class="sbsHeaderInner">Seminar</div>
                                            </div>
                                            <div id="itemPlaceholder1" runat="server">
                                            </div>
                                        </div>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <fieldset id="View" class="sbsFieldset sbsBorders">
                                            <div class="sbsFieldset sbsBorders">
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewUID" runat="server" Enabled="false"></asp:Label>
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewFolder" runat="server" CssClass="sbsLabel sbsBlock" Text="Name:"></asp:Label>
                                                    <asp:Label ID="ViewFolderText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewTitle" runat="server" CssClass="sbsLabel sbsBlock" Text="Titel:"></asp:Label>
                                                    <asp:Label ID="ViewTitleText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewDatum" runat="server" CssClass="sbsLabel sbsBlock" Text="Durchführung am:"></asp:Label>
                                                    <asp:Label ID="ViewDatumText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewRelease" runat="server" CssClass="sbsLabel sbsBlock" Text="Freigabe:"></asp:Label>
                                                    <asp:Label ID="ViewReleaseText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewDescription" runat="server" CssClass="sbsLabel sbsBlock" Text="Beschreibung:"></asp:Label>
                                                    <asp:Label ID="ViewDescriptionText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewAuthor" runat="server" CssClass="sbsLabel sbsBlock" Text="Author:"></asp:Label>
                                                    <asp:Label ID="ViewAuthorText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsRow">
                                                    <asp:Label ID="ViewDozent" runat="server" CssClass="sbsLabel sbsBlock" Text="Dozent:"></asp:Label>
                                                    <asp:Label ID="ViewDozentText" runat="server" CssClass="sbsFieldValue" />
                                                </div>
                                                <div class="sbsCommandButtons">
                                                    <hr class="sbsHr" />
                                                    <telerik:RadButton ID="EditButton" runat="server" ButtonType="SkinnedButton" CausesValidation="False" OnClientClicked="EditButton_Click" Text="Anpassen" ToolTip="Seminardaten ändern" AutoPostBack="false" />
                                                </div>
                                            </div>
                                        </fieldset>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <fieldset id="Edit" class="sbsFieldset sbsBorders">
                                            <legend class="sbsLegend"></legend>
                                            <div class="sbsRow">
                                                <asp:Label ID="EditUID" runat="server" Enabled="false"></asp:Label>
                                            </div>
                                            <div class="rdfRow">
                                                <asp:Label ID="EditFolder" runat="server" CssClass="sbsLabel sbsBlock" Text="Name:"></asp:Label>
                                                <asp:Label ID="EditFolderText" runat="server" CssClass="sbsFieldValue" />
                                            </div>
                                            <div class="sbsRow">
                                                <asp:Label ID="EditTitle" runat="server" AssociatedControlID="EditTitleTextBox" CssClass="sbsLabel sbsBlock" Text="Titel:"></asp:Label>
                                                <telerik:RadTextBox ID="EditTitleTextBox" runat="server" WrapperCssClass="sbsInput" Width="100%"   />
                   
                                            </div>
                                            <div class="sbsRow">
                                                <asp:Label ID="EditDatum" runat="server" AssociatedControlID="EditDatumTextBox" CssClass="sbsLabel sbsBlock" Text="Durchführung am:"></asp:Label>
                                                <telerik:RadTextBox ID="EditDatumTextBox" runat="server" WrapperCssClass="sbsInput" Width="100%" />
                                            </div>
                                            <div class="sbsRow">

                                                <asp:Label ID="EditRelease" runat="server" AssociatedControlID="EditReleaseTextBox" CssClass="sbsLabel sbsBlock" Text="Freigabe am:"></asp:Label>
                                                <telerik:RadDateInput ID="EditReleaseTextBox" runat="server" WrapperCssClass="sbsInput" DateFormat="g" />


                                            </div>
                                            <div class="sbsRow">
                                                <asp:Label ID="EditDescription" runat="server" AssociatedControlID="EditDescriptionTextBox" CssClass="sbsLabel sbsBlock" Text="Beschreibung:"></asp:Label>
                                                <telerik:RadTextBox ID="EditDescriptionTextBox" runat="server" WrapperCssClass="sbsInput" Width="100%" Resize="Both" TextMode="MultiLine" />
                                            </div>
                                            <div class="sbsRow">
                                                <asp:Label ID="EditAuthor" runat="server" AssociatedControlID="EditAuthorTextBox" CssClass="sbsLabel sbsBlock" Text="Author:"></asp:Label>
                                                <telerik:RadTextBox ID="EditAuthorTextBox" runat="server" WrapperCssClass="sbsInput" Width="100%" />
                                            </div>
                                            <div class="sbsRow">
                                                <asp:Label ID="EditDozent" runat="server" AssociatedControlID="EditDozentTextBox" CssClass="sbsLabel sbsBlock" Text="Dozent:"></asp:Label>
                                                <telerik:RadTextBox ID="EditDozentTextBox" runat="server" WrapperCssClass="sbsInput" Width="100%" />
                                            </div>
                                            <div class="rdfCommandButtons">
                                                <hr class="rdfHr" />
                                                <telerik:RadButton ID="UpdateButton" runat="server" ButtonType="SkinnedButton" OnClientClicked="function(){dataform1.updateItem();}" Text="Speichern" ToolTip="Anpassungen speichern" AutoPostBack="false" />
                                                <telerik:RadButton ID="CancelButton" runat="server" ButtonType="SkinnedButton" OnClientClicked="DataUpdateClickedCancel" CausesValidation="False" Text="Abbrechen" ToolTip="Abbrechen" AutoPostBack="false" />
                                            </div>
                                        </fieldset>
                                    </EditItemTemplate>

                                </telerik:RadDataForm>
                            </div>
                            <telerik:RadWindow ID="WindowFolder" runat="server" VisibleOnPageLoad="false" Title="Ordner"></telerik:RadWindow>
                            <telerik:RadWindow ID="WindowFile" runat="server" VisibleOnPageLoad="false" Title="Dokument"></telerik:RadWindow>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td height="10"></td>
        </tr>


    </table>

    <telerik:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="WindowNewSeminar" runat="server" VisibleOnPageLoad="false" Title="Neues Seminar" Width="500px" Height="200px" Modal="true">
                <ContentTemplate>
                    <asp:Table runat="server">
                        <asp:TableRow Height="30px"></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                            <asp:Label runat="server"  Text="Seminarname:" />
                            </asp:TableCell><asp:TableCell>
                                <asp:TextBox ID="TbNewSeminar" Width="250px" runat="server"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow Height="30px"></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:Button ID="ButtonSaveSeminar" runat="server" Text="Speichern" OnClientClick="ButtonSaveSeminarClicked(); return false;" UseSubmitBehavior="false" />
                            </asp:TableCell><asp:TableCell>
                                <asp:Button ID="ButtonCancelNewSeminar" runat="server" Text="Abbrechen" OnClientClick="ButtonCancelNewSeminarClick(); return false;" UseSubmitBehavior="false" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadWindow ID="WindowStartpage" runat="server" Title="Vorschau Navigation" Width="1000px" Height="600px" Modal="true" ReloadOnShow="true" EnableViewState="false" ShowContentDuringLoad="false" VisibleOnPageLoad="false">
        <ContentTemplate>
            <asp:PlaceHolder ID="startPageMenue" runat="server">
                <link href="SBSSheet.css" rel="stylesheet" />
                <link href="SBSSeminarTable.css" rel="stylesheet" type="text/css" />
                <script src="SBSSeminarTableScript.js" type="text/javascript"></script>
            </asp:PlaceHolder>
        </ContentTemplate>
    </telerik:RadWindow>


</asp:Content>



