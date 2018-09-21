<%@ Page MasterPageFile="~/Framework.Master" Language="C#" AutoEventWireup="true" CodeBehind="UploadExcel.aspx.cs" Inherits="ch.appl.psoft.MKS.UploadExcel" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 
    <script src="../JavaScript/UploadScript.js" type="text/javascript"></script>
       <span class="allowed-attachments" id="errorMsg" runat="server"  style="color: #FF0000; visibility: inherit; display: none;">
           Die Datei konnte nicht verarbeitet werden.
    <br />
           Ein Fehler trat in der Zeile <%=RowNumber %> auf.
    <br />
    Überprüfen Sie die Datei und Starten Sie den Vorgang erneut.<br />
              </span>
    Wählen Sie die gewünschte Excel Datei aus.<br />
    

         
    Erlaubte Formate: (<%= String.Join( ",", AsyncUpload1.AllowedFileExtensions ) %>)
           
 
                       
    <asp:Button ID="btnDummy" runat="server" onclick="btnDummy_Click" style="display: none;"  />

      
     <telerik:RadAsyncUpload runat="server" ID="AsyncUpload1"
                            HideFileInput="true"
                            Skin="Telerik"
                            AllowedFileExtensions=".xls,.xlsx,.xlsm" 
                            OnFileUploaded="AsyncUpload1_FileUploaded"
                            OnClientFileUploaded="onFileUploaded"
                            PostbackTriggers="btnDummy"
                           
        >
         <Localization Cancel="" DropZone="" Remove="" Select="Wählen..." />
    </telerik:RadAsyncUpload>
      
 
                       

</asp:Content>
