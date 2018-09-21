<%@ Page Title="" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="chooseProduct.aspx.cs" Inherits="ch.appl.psoft.Basics.chooseProduct" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../Style/login.css" rel="stylesheet" />
    <div id="productZone">
        <div class="table">
            <div style="margin: 10px; width: 580px">
                <div class="titleCell">
                    <asp:Label ID="TitleLabelLogin" runat="server" EnableViewState="False">Produktauswahl</asp:Label>
                </div>
            </div>
            <div>
                <div class="table">
                    <div class="row">
                        <div class="cellChooseProduct" style="width: 80px">
                            <asp:Literal ID="HeaderPlan" runat="server" EnableViewState="False">Plan</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="HeaderOperationalTime" runat="server" EnableViewState="False">Laufzeit</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="HeaderMaps" runat="server" EnableViewState="False">Maps</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="HeaderPrice" runat="server" EnableViewState="False">Preis CHF</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                        </div>
                        <div class="cellChooseProduct">
                        </div>
                    </div>

                    <div class="row">
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Plan1" runat="server" EnableViewState="False">Micro</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="OperationalTime1" runat="server" EnableViewState="False">5 Jahre</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="MapQuantity1" runat="server" EnableViewState="False">5</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Price1" runat="server" EnableViewState="False">9</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Description1" runat="server" Text="Beschreibung" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Description_Click" CommandArgument="1" CommandName="Description" />
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Oder1" runat="server" Text="Bestellen" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Oder_Click" CommandArgument="1" CommandName="Order" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Plan2" runat="server" EnableViewState="False">Small</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="OperationalTime2" runat="server" EnableViewState="False">5 Jahre</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="MapQuantity2" runat="server" EnableViewState="False">10</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Price2" runat="server" EnableViewState="False">18</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Description2" runat="server" Text="Beschreibung" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Description_Click" CommandArgument="2" CommandName="Description" />
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Oder2" runat="server" Text="Bestellen" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Oder_Click" CommandArgument="2" CommandName="Order 2" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Plan3" runat="server" EnableViewState="False">Medium</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="OperationalTime3" runat="server" EnableViewState="False">5 Jahre</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="MapQuantity3" runat="server" EnableViewState="False">20</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Price3" runat="server" EnableViewState="False">29</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Description3" runat="server" Text="Beschreibung" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Description_Click" CommandArgument="3" CommandName="Description" />
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Oder3" runat="server" Text="Bestellen" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Oder_Click" CommandArgument="3" CommandName="Order" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Plan4" runat="server" EnableViewState="False">Large</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="OperationalTime4" runat="server" EnableViewState="False">5 Jahre</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="MapQuantity4" runat="server" EnableViewState="False">50</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Literal ID="Price4" runat="server" EnableViewState="False">49</asp:Literal>
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Description4" runat="server" Text="Beschreibung" CssClass=" rfdSkinnedButton buttonChooseProduct" OnClick="Description_Click" CommandArgument="4" CommandName="Description" />
                        </div>
                        <div class="cellChooseProduct">
                            <asp:Button ID="Oder4" runat="server" Text="Bestellen" CssClass="rfdSkinnedButton buttonChooseProduct" OnClick="Oder_Click" CommandArgument="4" CommandName="Order" />
                        </div>
                    </div>
                    <div class="table">
                        <div class="row">
                            <div class="cellChooseProduct" style="text-align: left; height: 50px">
                                <asp:Button ID="CancelButton" runat="server" Text="Abbrechen" OnClick="CancelButton_Click"></asp:Button>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
