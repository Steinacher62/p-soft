<%@ Page Title="" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="ch.appl.psoft.Basics.AddUser" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1"  runat="server">
    <script src="../Scripts/jquery-3.3.1.min.js"></script>
    <script src="../JavaScript/jquery.json-2.2.min.js"></script>
    <link href="../Style/login.css" rel="stylesheet" />
    <div id="addUserZone" class="addUser" style="width:670px">
        <div class="table" style="border-collapse: collapse;">
            <div style="margin: 10px; width: 650px">
                <div class="titleCell">
                    <asp:Label ID="TitleLabel" runat="server" EnableViewState="False" Text="Neues Konto einrichten"></asp:Label>
                </div>
            </div>
            <div>
                <div class="table" >
                    <div class="row">
                        <div class="cell">
                            <asp:Label ID="SalutationLabel" runat="server" AssociatedControlID="DropDownListSalutation">Anrede:</asp:Label>
                        </div>
                        <div class="dropDowncell">
                            <telerik:RadDropDownList ID="DropDownListSalutation" runat="server" CssClass="dropDownSalutation"></telerik:RadDropDownList>
                        </div>
                        <div class="cell">
                            <asp:RequiredFieldValidator ID="SalutationRequired" runat="server" ControlToValidate="DropDownListSalutation" ErrorMessage="Anrede ist erforderlich." CssClass="rtf" ></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="row">
                        <div class="cell">
                            <asp:Label ID="NameLabel" runat="server" AssociatedControlID="Name">Name:</asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="Name" runat="server" Width="300px"></asp:TextBox>
                        </div>
                        <div class="cell">
                            <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name" ErrorMessage="Der Name ist erforderlich." CssClass="rtf"  ></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <asp:Label ID="FirstNameLabel" runat="server" AssociatedControlID="Firstname">Vorname:</asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="FirstName" runat="server" Width="300px"></asp:TextBox>
                        </div>
                        <div class="cell">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FirstName" ErrorMessage="Der Vorname ist erforderlich." CssClass="rtf" ></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <asp:Label ID="EMailLabel" runat="server" AssociatedControlID="UserName">E-Mail:</asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="UserName" runat="server" Width="300px"></asp:TextBox>
                        </div>
                        <div class="cell">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="UserName" ErrorMessage="Die E-Mail Adresse ist erforderlich." CssClass="rtf" ></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <asp:Label ID="ConfirmEMailLabel" runat="server" AssociatedControlID="ConfirmEMail">E-Mail bestätigen:</asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="ConfirmEMail" runat="server" Width="300px"></asp:TextBox>
                        </div>
                        <div class="cell">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ConfirmEMail" ErrorMessage="Die E-Mail Adresse ist erforderlich." CssClass="rtf" ></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="table">
            <div class="row">
                <div class="failureText">
                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                </div>
            </div>
        </div>
        <div class="table">
            <div class="row">
                <div class="cell" style="text-align: left;">
                    <asp:Button ID="BtnAddUser" runat="server" Text="Benutzer erstellen"  CausesValidation="true"   OnClientClick="AddUser();  return false;"  />
                </div>
                <div class="cell" style="text-align: left;">
                    <asp:Button ID="CancelButton" runat="server" CausesValidation="False" Text="Abbrechen" OnClientClick="return CancelClick()" />
                </div>
                <div class="cell">
                </div>
            </div>
        </div>

    </div>
    <script type="text/javascript">

        function AddUser() {
            if (Page_ClientValidate()) {
                $(".failureText").empty();
                var params = {};
                params.salutation = $("#ctl00_ContentPlaceHolder1_DropDownListSalutation")[0].value;
                params.name = $("#ContentPlaceHolder1_Name")[0].value;
                params.firstname = $("#ContentPlaceHolder1_FirstName")[0].value;
                params.userName = $("#ContentPlaceHolder1_UserName")[0].value;
                var formData = $.toJSON(params);
                var ret;

                $.ajax({
                    type: "POST",
                    url: "UserService.asmx/AddUser",
                    cache: false,
                    async: false,
                    data: formData,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (msg) {
                        ret = msg.d; //JSON object mit eigenschaft "d"
                    },
                    error: function (result, errortype, exceptionobject) {
                        alert('Error:' + result.responseText);
                    }
                });
                if (ret != "ok") {
                    $(".failureText").append(ret);
                }
                else
                {
                    alert('Benutzer wurde erflogreich hinzugefügt. Eine E-Mail mit den Zugangsdaten wurde verschickt.');
                    window.location = 'login.aspx';
                
                }
                return ret;
            }
        }

        function CancelClick() {
            window.location = 'login.aspx';
            return false;

        }



    </script>

</asp:Content>

