<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="MatrixDetailCtrl.ascx.cs" Inherits="ch.appl.psoft.Morph.Controls.MatrixDetailCtrl" %>
<link href="../Style/Table.css" rel="stylesheet" type="text/css" />
<!-- <script src="../Scripts/jquery-3.3.1.min.js" type="text/javascript"></script> -->
<!-- <script src="../JavaScript/jquery.json-2.2.min.js" type="text/javascript"></script> -->
<script src="../JavaScript/colorMenu.js" type="text/javascript"></script>
<%--<script src="../JavaScript/jquery.svg.min.js" type="text/javascript"></script>--%>
<script src="../Scripts/svg.min.js"></script>
<%--<script src="../Scripts/jquery.signalR-2.2.1.min.js"></script>
<script src="../signalr/hubs"></script>--%>
<script src="../JavaScript/MatrixCommunication.js"></script>
<script src="../JavaScript/DragDropTouch.js"></script>
<telerik:RadWindowManager ID="RadWindowManager1" runat="server">
    <Windows>
        <telerik:RadWindow ID="CopyOptionWindow" runat="server" Title="Optionen kopieren" Visible="true" Width="365px" Height="200px" Modal="true" CssClass="CopyWindow">
            <ContentTemplate>
                <div>
                    <telerik:RadLabel ID="TitleOptionWindow" runat="server" Text="Optionen Sokratesskarte kopieren" Font-Bold="true" Font-Size="Large"></telerik:RadLabel>
                </div>
                <div>
                    <telerik:RadCheckBox ID="copyColoration" runat="server" Text="Einfärbungen kopieren" AutoPostBack="false" Checked="true"></telerik:RadCheckBox>
                </div>
                <div>
                    <telerik:RadCheckBox ID="copyWirkungspaket" runat="server" Text="Wirkungselemente kopieren" AutoPostBack="false" Checked="true"></telerik:RadCheckBox>
                </div>
                <div>
                    <telerik:RadButton ID="CopyOptionWindowButton" runat="server" Text="Weiter" OnClientClicked="CopyOptionWindowButtonClicked" AutoPostBack="false"></telerik:RadButton>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
        <telerik:RadWindow ID="PdfWindow" runat="server" Title="Sokratreskarte drucken" Visible="true" Width="600px" Height="600px" Modal="true">
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
