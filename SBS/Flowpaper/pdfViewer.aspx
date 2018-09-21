<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pdfViewer.aspx.cs" Inherits="ch.appl.psoft.SBS.pdfViewer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="js/jquery.min.js"></script>
   <%-- <script src="js/jquery.json-2.2.min.js"></script>--%>
    <script src="js/jquery.extensions.min.js"></script>

    <script src="js/pdfOpen.js"></script>

    <script src="js/flowpaper.js"></script>
    <script src="js/flowpaper_handlers.js"></script>
    <script src="js/FlowPaperViewer.js"></script>

    <%--<script src="js/pdf.min.js"></script>+
    <script src="js/pdf.worker.min.js"></script>--%>
    
    <link href="css/flowpaper.css" rel="stylesheet" />
    <link href="css/flowpaper_flat.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>


    <div id="documentViewer" class="flowpaper_viewer" style="position: absolute; left: 1px; top: 1px; width: 99%; height: 99%"></div>



</body>
</html>
