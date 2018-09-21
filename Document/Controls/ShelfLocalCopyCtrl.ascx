<%@ Control language="c#" Codebehind="ShelfLocalCopyCtrl.ascx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Document.Controls.ShelfLocalCopyCtrl" %>
<%@ Register TagPrefix="Psoft" Namespace="ch.appl.psoft.Common" Assembly="p-soft" %>
<%@ Register tagprefix="PSOFT" tagname="lockFile" src="../../Common/LockFile.ascx" %>
<script  type='text/javascript' type="text/javascript" event='getFileFinished()' for='lockFile'>
    if (!copyNextContext())
        setTimeout("Back()", 1500);
</script>

<script language='javascript'>
    var cancel = false;
    var localBasePath = '';
    var contextArray = new Array();
    var curIndex = 0;

    function Cancel()
    {
        cancel = true;
        document.forms[0].<%=ClientID%>_cancelButton.disabled = true;
    }

    function Back()
    {
        window.history.back();
    }

    function FDContext(localPath, localFileName, serverFileName, isFolder)
    {
        this.localPath = localPath;
        this.localFileName = localFileName;
        this.serverFileName = serverFileName;
        this.isFolder = isFolder;
    }

    function copyNextContext()
    {
        if (curIndex < contextArray.length)
        {
            var fdContext = contextArray[curIndex];
            LockFileObject.LocalPath = localBasePath + fdContext.localPath;
            if (fdContext.isFolder)
            {
                if (SetProgress(curIndex++, fdContext.localPath))
                    return false;
                LockFileObject.createLocalPath();
                return copyNextContext();
            }
            else
            {
                if (SetProgress(curIndex++, fdContext.localPath + fdContext.localFileName))
                    return false;
                LockFileObject.LocalFileName = fdContext.localFileName;
                LockFileObject.ServerFileName = fdContext.serverFileName;
                LockFileObject.getFileAsync(false);
                return true;
            }
        }
        else
        {
            SetProgress(contextArray.length, '');
            <%=ClientID%>_LoadingLabel.innerText = '<%=_loadingFinished%>';
            document.forms[0].<%=ClientID%>_cancelButton.disabled = true;
            return false;
        }
    }

    function SetProgress(fileNr, fileName)
    {
        var percentage = fileNr * 100 / contextArray.length;
        <%=ClientID%>_FileNameLabel.innerText = fileName;
        <%=ClientID%>_PercentageLabel.innerText = Math.round(percentage) + '%';
        setLoadingBar(percentage);
        return cancel;
    }

    function CopyLocal()
    {
        LockFileObject.ServerName = '<%=_ftpDocumentServer%>';
        LockFileObject.ServerPath = '<%=_ftpDocumentSaveDirectory%>';
        LockFileObject.LocalFileName = '<%=_title%>';
        LockFileObject.selectLocalPath();
        localBasePath = LockFileObject.LocalPath;
        if (localBasePath == '')
            return false;
        LockFileObject.createLocalPath();

        <%=_copyScript%>

        copyNextContext();
        return true;
    }

    function Go()
    {
        LockFileObject = document.getElementById("LockFile");
        document.forms[0].<%=ClientID%>_cancelButton.disabled = false;
        if (!CopyLocal())
            Back();
    }
</script>
<PSOFT:lockFile runat="server" id="LockFile1" />
<table border="0" width="100%" height="100%" align="center">
    <tr valign="middle">
        <td align="center" width="100%">
            <table border="0" cellpadding="0" cellspacing="2">
                <tr>
                    <td colspan="2" align="middle"><asp:label id="LoadingLabel" runat="server"></asp:label></td>
                </tr>
                <tr valign="bottom">
                    <td align="left" height="30" width="250"><asp:label id="FileNameLabel" runat="server"></asp:label></td>
                    <td align="right" width="50"><asp:label id="PercentageLabel" runat="server"></asp:label></td>
                </tr>
                <tr>
                    <td colspan="2" align="middle"><Psoft:progressbar id="LoadingBar" runat="server" CellSpacing="1" BorderWidth="0px" BorderColor="Black" BackColor="DarkGray" DonePercentage="0" Height="20px" Width="300px" NrOfCells="25"></Psoft:progressbar></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label Visible="False" ID="errorText" Runat="server" CssClass="error"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="middle">
                        <input type="button" id="cancelButton" class="Button" runat="server" onclick="Cancel()" NAME="cancelButton">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
