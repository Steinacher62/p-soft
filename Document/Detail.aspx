<%@ Page MasterPageFile="~/Framework.Master" language="c#" Codebehind="Detail.aspx.cs" enableSessionState= "True" AutoEventWireup="True" Inherits="ch.appl.psoft.Document.Detail" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<asp:Content ID="Content1" runat="server" 
    contentplaceholderid="ContentPlaceHolder1">
        <asp:HiddenField ID="goCopy" runat="server" />
    <asp:HiddenField ID="lblConfirmText" runat="server" />

    <script type='Javascript'>        __doPostBack('__Page', 'test');</script>




</asp:Content>




