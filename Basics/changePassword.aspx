<%@ Page Title="" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="changePassword.aspx.cs" Inherits="ch.appl.psoft.Basics.changePassword" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../Style/login.css" rel="stylesheet" />
    <div id="ChangePasswordZone">
        <asp:ChangePassword ID="ChangePassword1" runat="server" CssClass="loginControl" OnCancelButtonClick="ChangePassword_CancelButtonClick" OnChangingPassword="ChangePassword_ChangingPassword">
            <ChangePasswordTemplate>
                <div class="table" style="border-collapse: collapse;">
                    <div style="margin: 10px; width: 650px">
                        <div class="titleCell">
                            <asp:Label ID="TitleLabel" runat="server" EnableViewState="False"></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cell">
                            <div class="table" style="height: 200px;">
                                <div class="row">
                                    <div class="cell">
                                        <asp:Label ID="UsernameText" runat="server" AssociatedControlID="UserName"></asp:Label>
                                    </div>
                                    <div class="cell">
                                        <asp:TextBox ID="UserName" runat="server" Width="300px"></asp:TextBox>
                                    </div>
                                    <div class="cell">
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" CssClass="rtf" ValidationGroup="ChangePassword1"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="cell">
                                        <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Kennwort:</asp:Label>
                                    </div>
                                    <div class="cell">
                                        <asp:TextBox ID="CurrentPassword" runat="server" Width="300px" TextMode="Password"></asp:TextBox>
                                    </div>
                                    <div class="cell">
                                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword" CssClass="rtf" ValidationGroup="ChangePassword1"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="cell">
                                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"></asp:Label>
                                    </div>
                                    <div class="cell">
                                        <asp:TextBox ID="NewPassword" runat="server" Width="300px" TextMode="Password"></asp:TextBox>
                                    </div>
                                    <div class="cell">
                                        <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword" CssClass="rtf" ValidationGroup="ChangePassword1"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="cell">
                                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"></asp:Label>
                                    </div>
                                    <div class="cell">
                                        <asp:TextBox ID="ConfirmNewPassword" runat="server" Width="300px" TextMode="Password"></asp:TextBox>
                                    </div>
                                    <div class="cell">
                                        <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword" CssClass="rtf" ValidationGroup="ChangePassword1">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                </div>
                            </div>
                            <div>
                                <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" CssClass="rtf" ControlToValidate="ConfirmNewPassword" Display="Dynamic" ValidationGroup="ChangePassword1"></asp:CompareValidator>
                            </div>
                            <div class="failureText">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False" Text=""></asp:Literal>
                            </div>
                            <div class="table">
                                <div class="row">
                                    <div class="cell" style="text-align: left;">
                                        <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword" ValidationGroup="ChangePassword1" CausesValidation="true" />
                                    </div>
                                    <div class="cell" style="text-align: left; height: 50px">
                                        <asp:Button ID="CancelPushButton" runat="server" CausesValidation="False" CommandName="Cancel" />
                                    </div>
                                    <div class="cell">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ChangePasswordTemplate>
        </asp:ChangePassword>
    </div>

</asp:Content>


