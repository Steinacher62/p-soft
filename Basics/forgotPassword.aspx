<%@ Page Title="" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="forgotPassword.aspx.cs" Inherits="ch.appl.psoft.Basics.forgotPassword" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../Style/login.css" rel="stylesheet" />
    <div id="PasswordRecoveryZone">
        <asp:PasswordRecovery ID="PasswordRecovery" runat="server" CssClass="loginControl" OnVerifyingUser="PasswordRecovery_VerifyingUser">
            <UserNameTemplate>
                <div class="table">
                    <div>
                         <div style="margin: 10px; width: 580px">
                            <div class="titleCell">
                                <asp:Label ID="TitleLabel" runat="server" EnableViewState="False"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="cell" style="font-size: 1.4em; text-align: center">
                                <asp:Literal ID="UsernameNeeded" runat="server" EnableViewState="False"></asp:Literal>
                            </div>
                        </div>
                        <div>
                            <div class="table" style="height: 70px;">

                                <div class="row">
                                    <div class="cell">
                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"></asp:Label>
                                    </div>

                                    <div class="cell">
                                        <asp:TextBox ID="UserName" runat="server" Width="300px"></asp:TextBox>
                                    </div>
                                    <div class="cell">
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" CssClass="rtf" ValidationGroup="PasswordRecovery"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="failureText">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                            </div>
                            <div class="table" style="height:50px;">
                                <div class="row">
                                    <div class="cell">
                                        <asp:Button ID="SubmitButton" runat="server" ValidationGroup="PasswordRecovery" CommandName="Submit" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </UserNameTemplate>
        </asp:PasswordRecovery>
    </div>
</asp:Content>
