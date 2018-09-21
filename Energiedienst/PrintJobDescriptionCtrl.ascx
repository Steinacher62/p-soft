<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrintJobDescriptionCtrl.ascx.cs" Inherits="ch.appl.psoft.Energiedienst.Controls.PrintJobDescriptionCtrl" %>
    <div>

        <iframe id ="pdfFrame"  width="1000px" height="750px"  runat="server"/>

    </div>
    <asp:CheckBox ID="FBSGeprueft" runat="server" AutoPostBack="True" />
