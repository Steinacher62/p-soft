<%@ Page Language="c#" CodeBehind="AccessorSelect.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Common.AccessorSelect" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title><%=accessorSelectTitle.Text%></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="../Style/Psoft.css" type="text/css" rel="stylesheet">
    <script type='text/javascript' type="text/javascript" src="../JavaScript/Common.js"></script>
    <script type='text/javascript' src="../Scripts/jquery-3.3.1.min.js"></script>
    <script type='text/javascript'>
        $(document).ready(function () {
             if (window.innerWidth <= 550) {
                window.innerWidth = 550;
            }
        });

        function erasePropertyBox() {
        }

        function drawPropertyBox(properties) {
        }
        function NewUser_Click(sender, args) {
            $find('AddUserWindow_C_TBUsername').set_value('');
            $('#AddUserWindow_C_ErrorMessage')[0].innerText = '';
           
            $find('AddUserWindow').show();

            args.set_cancel(true);
        }

        function AddUserWindowActivate(sender, args) {
            sender.center();
        }

    </script>

</head>
<body>

    <form id="AccessorSelect" method="post" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
        <table id="Table1" cellspacing="0" cellpadding="2" border="0" height="100%" width="100%">
            <tr>
                <td colspan="2" valign="top" height="20">
                    <asp:Label ID="accessorSelectTitle" runat="server" CssClass="section_title"></asp:Label></td>
            </tr>
            <tr valign="top" height="100%">
                <td colspan="2">
                    <div class="ListVariable">
                        <asp:Table ID="accessorTab" runat="server" CssClass="List" CellPadding="0" CellSpacing="1"></asp:Table>
                    </div>
                </td>
            </tr>
            <tr>
            </tr>

            <tr class="List" align="center" valign="bottom">
                <td align="left">
                    <telerik:RadButton ID="NewUserButton" runat="server" CssClass="Button" Width="220px" OnClientClicking="NewUser_Click"></telerik:RadButton>
                </td>
                <td align="left">
                    <telerik:RadButton ID="BackButton" runat="server" CssClass="Button" OnClick="BackButton_Click" Width="100px"></telerik:RadButton>
                </td>
                <td align="left">
                    <telerik:RadButton ID="SelectButton" runat="server" CssClass="Button" OnClick="SelectButton_Click"></telerik:RadButton>
                </td>
            </tr>
            <tr class="List" align="center" valign="bottom">
                <td>
                    <asp:Label ID="errorText" CssClass="error" runat="server" Visible="False"></asp:Label></td>
            </tr>
        </table>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
            <Windows>
                <telerik:RadWindow ID="AddUserWindow" runat="server" Width="500px" Height="250px" Modal="true" OnClientActivate="AddUserWindowActivate">
                    <ContentTemplate>
                        <div style="display: table; width: 400px;" class="NewUserTable">
                            <div style="display: table-row;">
                                <div style="display: table-cell; padding-top:20px">
                                    <telerik:RadLabel ID="LabelUsername" runat="server"></telerik:RadLabel>
                                </div>
                                <div style="display: table-cell;">
                                    <telerik:RadTextBox ID="TBUsername" runat="server" Width="300px"></telerik:RadTextBox>
                                </div>
                            </div>
                            <div style="display: table-row;" >
                                <div style="display: table-cell; padding-top:20px">
                                    <telerik:RadButton ID="ButtonNewUser" runat="server" Text="RadButton" OnClick="ButtonNewUser_Click"></telerik:RadButton>
                                </div>
                            </div>
                            <div style="display: table-row;" >
                                <div style="display: table-cell; padding-top:20px">
                                   
                                </div>
                            </div>
                        </div>
                        <telerik:RadLabel ID="ErrorMessage" runat="server" Width="450px" ForeColor="Red"></telerik:RadLabel>
                    </ContentTemplate>
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>
    </form>
</body>
</html>

