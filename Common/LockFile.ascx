<%@ Control Language="c#" AutoEventWireup="True" Codebehind="LockFile.ascx.cs" Inherits="ch.appl.psoft.Common.LockFile" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script  type='text/javascript' event="processFinished()" for="LockFile">
    if (window.confirm(checkInConfirmMessage))
    {
        var retValue = checkIn(lfDocumentID);
        if (lfAfterAutoCheckIn)
            lfAfterAutoCheckIn(retValue);
    }
</SCRIPT>
<script  type='text/javascript'>
    lfAfterAutoCheckIn = null;
    
    lfDocumentID='';
    checkInConfirmMessage = '<%=_checkInConfirmMessage%>';
    
    function getErrorMessage(errorID)
    {
        switch (errorID)
        {
            case 0: //everithing is fine
                return '';
                break;

            case 1: //user canceled
                return '';
                break;

            case 2: //generic failure
                return '<%=_lfErrorGeneric%>';
                break;

            case 3: //download failed
                return '<%=_lfErrorDownloadFile%>';
                break;

            case 4: //FTP Connection failed
                return '<%=_lfErrorFTPConnection%>';
                break;

            case 5: //FTP Directory does not exist
                return '<%=_lfErrorFTPDirectory%>';
                break;

            case 6: //process started but no handle; maybe the handling application was already running.
                return '<%=_lfErrorStartProcessNoHandle%>';
                break;

            case 7: //process is already running, document is already open
                return '<%=_lfErrorProcessAlreadyRunning%>';
                break;

            case 8: //starting of process failed, opening of document failed
                return '<%=_lfErrorStartProcess%>';
                break;

            case 9: //upload failed
                return '<%=_lfErrorUploadFile%>';
                break;

            case 101: //check-out failed
                return '<%=_lfErrorCheckOutFailed%>';
                break;

            case 102: //check-in failed
                return '<%=_lfErrorCheckInFailed%>';
                break;

            case 103: //undo check-out failed
                return '<%=_lfErrorCheckOutUndoFailed%>';
                break;
        }
    }
    
    function checkOut(documentID, openDocument, afterAutoCheckIn)
    {
        var retValue = 2;

        lfDocumentID = documentID;
        lfAfterAutoCheckIn = afterAutoCheckIn;
        
        if (wsCheckOutDocument(documentID))
        {
            var obj = document.getElementById("LockFile");
            retValue = obj.editFile(openDocument);
            if (retValue != 0 && retValue != 6)
                wsCheckOutUndoDocument(documentID);  //undo the checkout in case of failure
        }
        else
            retValue = 101;

        return retValue;        
    }

    function checkIn(documentID)
    {
        var obj = document.getElementById("LockFile");
        var retValue = obj.putFile();

        if (retValue == 0)
        {
            if (!wsCheckInDocument(documentID))
                retValue = 102
        }

        return retValue;        
    }

    function checkOutUndo(documentID)
    {
        var retValue = 0;

        if (!wsCheckOutUndoDocument(documentID))
            retValue = 103

        return retValue;        
    }

    function getDocument(openDocument)
    {
        var obj = document.getElementById("LockFile");
        return obj.getFile(openDocument);
    }
    
    function activexDisableConditional(){
        if(typeof activexDisable!="undefined"){
            activexDisable();
        }
    }
</script>
<OBJECT id="LockFile" onerror="window.setTimeout('activexDisableConditional()',500)" style="WIDTH: 1px; HEIGHT: 1px" codeBase="<%=_codeBase%>" border="0" classid="clsid:EE4531DB-432D-4C1B-915E-B61D2E53BE51" VIEWASTEXT>
    <PARAM NAME="_Version" VALUE="65536">
    <PARAM NAME="_ExtentX" VALUE="26">
    <PARAM NAME="_ExtentY" VALUE="26">
    <PARAM NAME="_StockProps" VALUE="0">
    <PARAM NAME="LocalFileName" VALUE="">
    <PARAM NAME="LocalPath" VALUE="">
    <PARAM NAME="OverwriteLocalFile" VALUE="-1">
    <PARAM NAME="ServerFileName" VALUE="">
    <PARAM NAME="Password" VALUE="">
    <PARAM NAME="ServerPath" VALUE="">
    <PARAM NAME="ServerPort" VALUE="0">
    <PARAM NAME="Protocol" VALUE="ftp">
    <PARAM NAME="ServerName" VALUE="">
    <PARAM NAME="Username" VALUE="">
</OBJECT>
