<%@ Page MasterPageFile="~/Login.Master" Language="c#" CodeBehind="login.aspx.cs" AutoEventWireup="True" Inherits="ch.appl.psoft.Basics.login" %>

<asp:Content ID="LoginContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
            
    <link href="../Style/login.css" rel="stylesheet" />
    <div id="loginZone" class="loginZone">
        <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" CssClass="loginControl">
            <LayoutTemplate>
                
                <div class="table">
                    <div style="margin:10px; width:650px">
                        <div class="titleCell">
                            <asp:Label ID="TitleLabelLogin" runat="server" EnableViewState="False"></asp:Label>
                        </div>
                    </div>
                    <div>
                        <div class="table" style="height: 100px">
                            <div class="row">
                                <div class="cell">
                                    <asp:Label ID="UserNameLabel" runat="server" CssClass="label" AssociatedControlID="UserName"></asp:Label>
                                </div>
                                <div class="cell">
                                    <asp:TextBox ID="UserName" runat="server" Width="300px"></asp:TextBox>
                                </div>
                                <div class="cell">
                                    <asp:RequiredFieldValidator ID="UserNameRequired" CssClass="rtf" runat="server" ControlToValidate="UserName"
                                        ErrorMessage="User Name is required." ValidationGroup="Login1"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="cell">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                </div>
                                <div class="cell">
                                    <asp:TextBox ID="Password" runat="server" Width="300px" TextMode="Password"></asp:TextBox>
                                </div>
                                <div class="cell">
                                    <asp:RequiredFieldValidator ID="PasswordRequired" CssClass="rtf" runat="server" ControlToValidate="Password"
                                        ErrorMessage="Password is required." ValidationGroup="Login1"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="failureText">
                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                        </div>

                        <div class="table">
                            <div class="row">
                                <div class="cell">
                                    <asp:HyperLink ID="forgotPasswordLink" NavigateUrl="forgotPassword.aspx" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="cell">
                                    <asp:HyperLink ID="changePasswordLink" NavigateUrl="changePassword.aspx" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="cell">
                                    <asp:HyperLink ID="newAccountLink" NavigateUrl="AddUser.aspx" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="cell" style="text-align: left; height: 50px">
                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Login" ValidationGroup="Login1" UseSubmitBehavior="true"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>
    </div>
</asp:Content>






