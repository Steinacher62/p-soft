<%@ Page Title="" Language="C#" MasterPageFile="~/Framework.Master" AutoEventWireup="true" CodeBehind="Pay.aspx.cs" Inherits="ch.appl.psoft.Payment.Pay" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--<link href="../Style/Psoft.css" rel="stylesheet" />--%>
    <script src="../JavaScript/Payment.js"></script>
    <script src="https://js.braintreegateway.com/js/braintree-2.24.1.min.js"></script>

    <div class="payZone">
        <asp:HiddenField ID="productId" runat="server"></asp:HiddenField>
        <div class="table">
            <div class="row">
                <div class="titleCell">
                    <asp:Label ID="TitleLabelLogin" runat="server" EnableViewState="False" CssClass="titleLabel">Sie haben folgendes Produkt gewählt:</asp:Label>
                </div>
            </div>
            <div>
                <div class="table" style="margin: 10px; width: 80%">
                    <div class="row">
                        <div class="cellTitle">
                            <asp:Literal ID="Product" runat="server" EnableViewState="False">Produkt:</asp:Literal>
                        </div>
                        <div class="cellDescription">
                            <asp:Literal ID="ProductTitle" runat="server" EnableViewState="False">Titel</asp:Literal>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cellTitle">
                            <asp:Literal ID="Description" runat="server" EnableViewState="False">Beschreibung:</asp:Literal>
                        </div>
                        <div class="cellDescription">
                            <asp:Literal ID="Productdescription" runat="server" EnableViewState="False"></asp:Literal>
                        </div>
                    </div>
                    <div class="row">
                        <div class="cellTitle">
                            <asp:Literal ID="Price" runat="server" EnableViewState="False">Preis:</asp:Literal>
                        </div>
                        <div class="cellDescription">
                            <asp:Literal ID="ProductPrice" runat="server" EnableViewState="False"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div class="table">
            <div class="row">
                <div class="titleCell">
                    <asp:Label ID="Paytitle" runat="server" EnableViewState="False" CssClass="titleLabel">Sicher bezahlen</asp:Label>
                </div>
            </div>

        </div>
        <div class="table">
            <div class="row">
                <div class="cellTitle">
                    <asp:Literal ID="PayWithCreditCardText" runat="server" EnableViewState="False" Text="Mit Kreditkarte bezahlen"></asp:Literal>
                </div>
                <div class="cellTitle">
                    <asp:Literal ID="PayWithPaypal" runat="server" EnableViewState="False" Text="Mit Paypal bezahlen"></asp:Literal>
                </div>
            </div>
            <div class="row">
                <div class="cellTitle" style="cursor: pointer">
                    <asp:ImageButton ID="ImageCreditCards" runat="server" OnClientClick="imgPayCreditCardClicked(); return false" />
                </div>
                <div class="cellTitle">
                    <div id="paypal-container" style="display: none"></div>
                    <asp:ImageButton ID="ImagePaypal" runat="server" OnClientClick="imgPayPaypalClicked(); return false"  />
                </div>

                <div class="cellTitle">
                </div>
                <div class="cellChooseProduct">
                </div>
            </div>

            <div class="row">
                <div class="cellTitle">
                </div>
                <div class="cellChooseProduct">
                </div>
            </div>

            <div class="row">
                <div class="cellTitle">
                    <asp:Literal ID="PayWithInvoice" runat="server" EnableViewState="False" Text="Vorauskasse"></asp:Literal>
                </div>
            </div>
            <div class="row">
                <div class="cellTitle">
                    <asp:ImageButton ID="Imageprepayment" runat="server" OnClientClick="imgInvoceClicked(); return false" />
                </div>
                <div class="cellTitle">
                </div>
                <div class="cellChooseProduct">
                </div>
            </div>

        </div>
        <div class="table">
            <div class="row">
                <div class="cellTitle" style="width: 50px">
                    <telerik:RadLinkButton ID="RadLinkButton1" runat="server" Text="AGB" OnClientClicked="AGBClicked"></telerik:RadLinkButton>
                </div>
                <div class="cellTitle" style="width: auto">
                    <telerik:RadCheckBox ID="AGBCheckBox" runat="server" Text="Ich habe die AGB gelesen und akzeptiert." AutoPostBack="false" Font-Bold="false"></telerik:RadCheckBox>
                </div>

            </div>
        </div>
    </div>

    <telerik:RadWindow ID="RadWindowAGB" runat="server" Title="AGB" Width="800px" Height="500px" Modal="true" NavigateUrl="AGBp-soft.htm" VisibleStatusbar="false" VisibleOnPageLoad="false" EnableViewState="false">
    </telerik:RadWindow>

    <telerik:RadWindow ID="RadWindowPayCreditcard" runat="server" Title="Bezahlen" Width="600px" Height="600px" Modal="true" VisibleStatusbar="false" VisibleOnPageLoad="false" EnableViewState="false" OnClientActivate="windowPayActivate">
        <ContentTemplate>
            <div class="table">
                <div class="row">
                    <div class="titleCell">
                        <asp:Label ID="SystemOfPaymentCard" runat="server" EnableViewState="False" CssClass="titleLabel">Mit Kreditkarte bezahlen</asp:Label>
                    </div>
                </div>
            </div>
            <div class="table">
                <div class="row">
                    <div class="cellTitle">
                        <asp:Label ID="Carddat" runat="server" EnableViewState="False">Kartendaten:</asp:Label>
                    </div>
                    <div class="cellTitle payment-method-icon">
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitle">
                        <asp:Label ID="lbName" runat="server" EnableViewState="False">Name</asp:Label>
                    </div>
                    <div class="cellTitle">
                        <telerik:RadTextBox ID="tbName" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" CssClass="CardValidate"
                            ControlToValidate="tbName"
                            SetFocusOnError="True" ValidationGroup="CardValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitle">
                        <asp:Label ID="lbFirstname" runat="server" EnableViewState="False">Vorname</asp:Label>
                    </div>
                    <div class="cellTitle">
                        <telerik:RadTextBox ID="tbFirstname" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="CardValidate"
                            ControlToValidate="tbFirstname"
                            SetFocusOnError="True" ValidationGroup="CardValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitle">
                        <asp:Label ID="lbCarnumber" runat="server" EnableViewState="False">Kartennummer</asp:Label>
                    </div>
                    <div class="cellTitle">
                        <telerik:RadMaskedTextBox ID="tbCardnumber" runat="server" onkeyup="validatecardnumber(this.value)"
                            Mask="#### #### #### ####" ValidationGroup="CardValidate" />
                        <asp:RequiredFieldValidator ID="cardnumbervalidator" runat="server" CssClass="CardValidate"
                            ControlToValidate="tbCardnumber"
                            SetFocusOnError="True" ValidationGroup="CardValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>

                </div>
                <div class="row">
                    <div class="cellTitle">
                        <asp:Label ID="lbCVV" runat="server" EnableViewState="False">Prüfnummer</asp:Label>
                    </div>
                    <div class="cellTitle">
                        <telerik:RadMaskedTextBox ID="TBCVV" runat="server"
                            Mask="###" ValidationGroup="CardValidate" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" CssClass="CardValidate"
                            ControlToValidate="TBCVV"
                            SetFocusOnError="True" ValidationGroup="CardValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitle">
                        <asp:Label ID="lbExpirationDate" runat="server" EnableViewState="False">Ablaufdatum</asp:Label>
                    </div>
                    <div class="cellTitle">
                        <telerik:RadMonthYearPicker ID="ExpirationDate" runat="server" DateInput-DateFormat="MM/yyyy" DateInput-DisplayDateFormat="MM/yyyy" Culture="de-DE"></telerik:RadMonthYearPicker>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" CssClass="CardValidate"
                            ControlToValidate="ExpirationDate"
                            SetFocusOnError="True" ValidationGroup="CardValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>

            <div class="messagePayment">
            </div>


            <div class="table">
                <div class="row">
                    <div class="cellTitle" style="text-align: left;">
                        <telerik:RadButton ID="BtnPayCreditCard" runat="server" Text="Bezahlen" OnClientClicked="BtnPayClicked" AutoPostBack="false" />
                    </div>
                    <div class="cellTitle" style="text-align: left;">
                        <telerik:RadButton ID="CancelButton" runat="server" CausesValidation="False" Text="Abbrechen" OnClientClicked="CancelClick" AutoPostBack="false" />
                    </div>
                    <div class="cell">
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="RadWindowAddress" runat="server" Title="Rechnungsadresse" Width="600px" Height="450px" Modal="true" VisibleStatusbar="false" VisibleOnPageLoad="false" EnableViewState="false">
        <ContentTemplate>
            <div class="table">
                <div class="row">
                    <div class="titleCell">
                        <asp:Label ID="lblTitleAddress" runat="server" EnableViewState="False" CssClass="titleLabel">Rechnungsadresse</asp:Label>
                    </div>
                </div>
            </div>
            <div class="table">
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="lblNameAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Name</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadTextBox ID="tbNameAddress" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" CssClass="AddressValidate"
                            ControlToValidate="tbNameAddress"
                            SetFocusOnError="True" ValidationGroup="AddressValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="lblFirstnameAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Vorname</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadTextBox ID="tbFirstnameAddress" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" CssClass="AddressValidate"
                            ControlToValidate="tbFirstnameAddress"
                            SetFocusOnError="True" ValidationGroup="AddressValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="LblFirmAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Firma</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadTextBox ID="tbFirmAddress" runat="server"></telerik:RadTextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="lblStreetAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Strasse Nr.</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadTextBox ID="tbStreetAddress" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" CssClass="AddressValidate"
                            ControlToValidate="tbStreetAddress"
                            SetFocusOnError="True" ValidationGroup="AddressValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="lblZipCodeAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Postleitzahl</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadTextBox ID="tbZipCodeAddress" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" CssClass="AddressValidate"
                            ControlToValidate="tbZipCodeAddress"
                            SetFocusOnError="True" ValidationGroup="AddressValidate">*Benötigt</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
                            ControlToValidate="tbZipCodeAddress"
                            ValidationExpression="^[0-9]+$" ValidationGroup="AddressValidate">*Ungültig
                        </asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="lblCityAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Ort</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadTextBox ID="tbCityAddress" runat="server"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" CssClass="AddressValidate"
                            ControlToValidate="tbCityAddress"
                            SetFocusOnError="True" ValidationGroup="AddressValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="cellTitleLabelAddress">
                        <asp:Label ID="lblCountryAddress" runat="server" EnableViewState="False" CssClass="lblAddress">Land</asp:Label>
                    </div>
                    <div class="cellDataAddress">
                        <telerik:RadDropDownList ID="tbCountryAddress" runat="server"></telerik:RadDropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" CssClass="AddressValidate"
                            ControlToValidate="tbCountryAddress"
                            SetFocusOnError="True" ValidationGroup="AddressValidate">*Benötigt</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="messagePayment">
                </div>
                <div class="table">
                    <div class="row">
                        <div class="cellTitle" style="text-align: left;">
                            <telerik:RadButton ID="buttonAddressSave" runat="server" Text="Weiter" OnClientClicked="BtnAddressClicked" AutoPostBack="false" />
                        </div>
                        <div class="cellTitle" style="text-align: left;">
                            <telerik:RadButton ID="buttonAddressCancel" runat="server" CausesValidation="False" Text="Abbrechen" OnClientClicked="AddressCancelClicked" AutoPostBack="false" />
                        </div>
                        <div class="cell">
                        </div>
                    </div>
                </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="paypalSuccesfull" runat="server" Title="Zahlungsinformationen" Width="500px" Height="150px" Modal="true" VisibleStatusbar="false" VisibleOnPageLoad="false" EnableViewState="false">
        <ContentTemplate>
            <div class="table">
                <div class="row">
                    <div class="titleCell">
                        <asp:Label ID="Label1" runat="server" EnableViewState="False" CssClass="titleLabel">Zahlung erfolgraich</asp:Label>
                    </div>
                </div>
            </div>
            <div class="table">
                <div class="row">
                    <asp:Label ID="Label2" runat="server" EnableViewState="False">Die Zahlung wurde erfolgreich abgeschlossen. Eine E-Mail mit dem Rechnungsbeleg wird in den nächsten Minuten verschickt. </asp:Label>
                </div>
            </div>
            <div class="table">
                <div class="row">
                    <div class="cellTitle" style="text-align: left;">
                        <telerik:RadButton ID="btnPaypalSucessfull" runat="server" Text="Weiter" OnClientClicked="BtnAPaypalSucessfullyClicked" AutoPostBack="false" />
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="Invoice" runat="server" Title="Zahlungsinformationen" Width="500px" Height="150px" Modal="true" VisibleStatusbar="false" VisibleOnPageLoad="false" EnableViewState="false">
        <ContentTemplate>
            <div class="table">
                <div class="row">
                    <div class="titleCell">
                        <asp:Label ID="Label3" runat="server" EnableViewState="False" CssClass="titleLabel">Ihre Rechnung</asp:Label>
                    </div>
                </div>
            </div>
            <div class="table">
                <div class="row">
                    <asp:Label ID="Label4" runat="server" EnableViewState="False">Die Datenerfassung ist abgeschlossen. Eine E-Mail mit dem Rechnungsbeleg wird in den nächsten Minuten verschickt. </asp:Label>
                </div>
            </div>
            <div class="table">
                <div class="row">
                    <div class="cellTitle" style="text-align: left;">
                        <telerik:RadButton ID="RadButton1" runat="server" Text="Weiter" OnClientClicked="BtnAPaypalSucessfullyClicked" AutoPostBack="false" />
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>
