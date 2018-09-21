<%@ Control Language="c#" AutoEventWireup="True" Codebehind="JobDescriptionCtrl.ascx.cs" Inherits="ch.appl.psoft.Energiedienst.Controls.JobDescriptionCtrl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<script  type='text/javascript'>
    var deleteConfirmMessage = "<%=deleteMessage%>";
    var deleteURL = "JobDescription.aspx?&jobID=<%=JobID%>";

    function deleteRowConfirm(rowId,dbId)
    {
        wsDeleteRowConfirm(this, deleteURL, deleteConfirmMessage, "DUTY_COMPETENCE_VALIDITY", rowId, dbId)
    }
</script>

<iframe id ="pdfFrame"  width="1000px" height="750px"  runat="server"/>
<div  style ="padding-top:30px; vertical-align:top; display:inline-block;">
 
 <p style="line-height: .5em;" />
 <telerik:RadButton ID="btnDownload"  runat="server" Text="Dokument zur Bearbeitung herunteladen" OnClick="btnDownload_Click" />
     <p style="line-height: .5em;" />
      <asp:Label ID="Label1" runat="server" style="line-height: .5em;"  Text="Nach dem Herunterladen können Sie das Word-Dokument bearbeiten"></asp:Label>
     <p style="line-height: .5em; height:30px;" />
      <asp:Label ID="Label2" runat="server" style="line-height: .5em;"  Text="und an einem beliebigen Ort zwischenspeichern."></asp:Label>
     <p style="line-height: .5em;" />
      <asp:Label ID="Label3" runat="server" style="line-height: .5em;"  Text="Das Word-Dokument kann geschlossen und anschliessend über die "></asp:Label>
     <p style="line-height: .5em;" />
      <asp:Label ID="Label4" runat="server" style="line-height: .5em;"  Text="unten stehenden Buttons wieder hochgeladen werden."></asp:Label>
 <p style="line-height: .5em;" />
 <asp:FileUpload   ID="FileUploadControl" runat="server" />
      
        <p style="line-height: .5em; height:50px;" />

<telerik:RadButton ID="btnUpload" runat="server"  Text="Bearbeitetes Dokument hochladen" OnClick="btnUpload_Click" />
     <p style="line-height: .5em;" />
      <asp:Label ID="lblMessage" runat="server" style="line-height: .5em;"  Text=""></asp:Label>
       <p style="line-height: .5em; height:50px;" />
 <telerik:RadButton ID="FBSFreigabe" runat="server" Text="Freigabe" OnClick="FBW_RELEASE_CLICKED" /> 
   <p style="line-height: .5em;" />
    <asp:CheckBox ID="FBSGeprueft" runat="server" AutoPostBack="True" />
</div>
