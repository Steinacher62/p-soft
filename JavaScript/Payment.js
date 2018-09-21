

var productId;

$(document).ready(function () {

    braintree.setup(getClientToken(), "custom", {
        paypal: {
            container: "paypal-container",
            locale: "de_de",
            currency: "CHF",
        },
        onPaymentMethodReceived: function (obj) {
            transactionPaypal(obj.nonce);
        },
        onAuthorizationDismissed: function () {
            transactionDismissed();
        }
    });
    

    $("#ContentPlaceHolder1_ImageCreditCards").mouseover(function () {
        $(this).css('cursor', 'pointer');
    });

    $("#ContentPlaceHolder1_Imageprepayment").mouseover(function () {
        $(this).css('cursor', 'pointer');
    });

    $("#ContentPlaceHolder1_ImagePaypal").mouseover(function () {
        $(this).css('cursor', 'pointer');
    });
});
$(document).ajaxComplete(function (event, request, settings) {
    $('*').css('cursor', 'default');
});

function waitCursor() {
    $('*').css('cursor', 'progress');
}

function BtnAddressClicked() {
    if (Page_ClientValidate("AddressValidate")) {
        waitCursor();
        var params = {};
        params.Name = $("#ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbNameAddress").val();
        params.Firstname = $("#ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbFirstnameAddress").val();
        params.Firm = $("#ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbFirmAddress ").val();
        params.Street = $("#ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbStreetAddress").val();
        params.Zip = $("#ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbZipCodeAddress").val();
        params.City = $("#ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbCityAddress").val();
        params.CountryCode = $find("ctl00_ContentPlaceHolder1_RadWindowAddress_C_tbCountryAddress")._selectedValue;
        var formData = JSON.stringify(params);
        var ret;

        $.ajax({
            type: "POST",
            url: "../Payment/BraintreeService.asmx/SaveAddress",
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

        if (ret !== null) {
            $(".messagePayment").removeClass("successText").addClass("failureText").text(ret);
        }
        else {
            switch(paymentType) {
                case "Paypal":
                    $find('ctl00_ContentPlaceHolder1_RadWindowAddress').close();
                    $("#ContentPlaceHolder1_ImagePaypal").hide();
                    $("#paypal-container").css("display", "");
                    $("#braintree-paypal-button")[0].firstChild.click();
                    break;
                case "Invoice":
                    $find('ctl00_ContentPlaceHolder1_RadWindowAddress').close();
                    transactionInvoice();
                    break;
                default:
                    $find('ctl00_ContentPlaceHolder1_RadWindowAddress').close();
                    $find("ctl00_ContentPlaceHolder1_RadWindowPayCreditcard").show();
            } 
        }
    }

}

function AGBClicked() {
    $find('ctl00_ContentPlaceHolder1_RadWindowAGB').show();
}

function AGBCheckChecked() {
    if ($find("ctl00_ContentPlaceHolder1_AGBCheckBox")._checked) {
        return  true;
    }
    else {
        window.alert("Bitte akzeptieren Sie die AGB.")
        return false;
    }
    
}


function AddressCancelClicked() {
    if ($find('ctl00_ContentPlaceHolder1_RadWindowAddress') !== null) {
        $find('ctl00_ContentPlaceHolder1_RadWindowAddress').close();
    }
}

function windowPayActivate() {
    $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_BtnPayCreditCard").show();
    $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_CancelButton")[0].innerText = "Abbrechen";
}

function transactionPaypal(nonce) {
    waitCursor();
    paymentType = "Paypal";
    var params = {};
    params.Nonce = nonce;
    params.PaymentType = paymentType;

    var formData = JSON.stringify(params);
    var ret;

    $.ajax({
        type: "POST",
        url: "../Payment/BraintreeService.asmx/Pay",
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
    $find("ctl00_ContentPlaceHolder1_paypalSuccesfull").show();
}

function transactionInvoice() {
    waitCursor();
    paymentType = "Invoice";
    var params = {};
    params.PaymentType = paymentType;

    var formData = JSON.stringify(params);
    var ret;

    $.ajax({
        type: "POST",
        url: "../Payment/BraintreeService.asmx/CreateInvoce",
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
    $find("ctl00_ContentPlaceHolder1_Invoice").show();
}

function BtnAPaypalSucessfullyClicked() {
    $find("ctl00_ContentPlaceHolder1_paypalSuccesfull").close();
    window.location.href = '../report/CrystalReportViewer.aspx?alias=Invoice';  //+ $("#ContentPlaceHolder1_productId").val();
}


var paymentType = "";

function BtnPayClicked(sender, args) {
    if (Page_ClientValidate()) {
        paymentType = $(".payment-method-icon")[0].classList[2];
        var token = getClientToken();
        var nonce = getNonce(token);
    }

}

function imgPayCreditCardClicked() {
    if (AGBCheckChecked()) {
        $find('ctl00_ContentPlaceHolder1_RadWindowAddress').show();
    }
}

function imgPayPaypalClicked() {
    if (AGBCheckChecked()) {
        paymentType = "Paypal";
        $find('ctl00_ContentPlaceHolder1_RadWindowAddress').show();
    }
}

function imgInvoceClicked() {
    if (AGBCheckChecked()) {
        paymentType = "Invoice";
        $find('ctl00_ContentPlaceHolder1_RadWindowAddress').show();
    }
}


function CancelClick() {
    if ($find('ctl00_ContentPlaceHolder1_RadWindowPayCreditcard') !== null) {
        $find('ctl00_ContentPlaceHolder1_RadWindowPayCreditcard').close();
        if ($("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_CancelButton").hasClass('report')) {
            window.location.href = '../report/CrystalReportViewer.aspx?alias=Invoice';  //+ $("#ContentPlaceHolder1_productId").val();
        }
    }
}
function getClientToken() {
    waitCursor();
    var token = "test";
    $.ajax({
        url: 'BraintreeService.asmx/GetClientToken',
        type: 'POST',
        dataType: 'json',
        cache: false,
        async: false,
        contentType: 'application/json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, status) {
            token = data.d;
        },
        error: function (result, errortype, exceptionobject) {
            alert('Error:' + result.responseText);
        }
    });

    return token;
}

function getNonce(token) {
    waitCursor();
    var client = new braintree.api.Client({ clientToken: token });
    client.tokenizeCard({
        number: $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_tbCardnumber").val(),
        cardholderName: $.trim($("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_tbFirstname").val()) + " " + $.trim($("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_tbName").val()),
        expirationDate: $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_ExpirationDate_dateInput").val(),
        cvv: $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_TBCVV").val()
    }, function (err, nonce) {
        if (err == null) {
            var params = {};
            params.Nonce = nonce;
            params.PaymentType = paymentType;
            var formData = JSON.stringify(params);
            var ret;

            $.ajax({
                type: "POST",
                url: "../Payment/BraintreeService.asmx/Pay",
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

            if (ret !== null) {
                $(".messagePayment").removeClass("successText").addClass("failureText").text(ret);
            }
            else {
                $(".messagePayment").removeClass("failureText").addClass("successText").text("Zahlung erfogreich. Ein Beleg wird per E-Mail an Sie versandt.");
                $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_BtnPayCreditCard").hide()
                $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_CancelButton")[0].innerText = 'Schliessen';
                $("#ctl00_ContentPlaceHolder1_RadWindowPayCreditcard_C_CancelButton")[0].classList.add('report');
               

            }

        }

    });
}

var cardType;
function validatecardnumber(cardnumber) {
    if (cardType == null) {
        // Strip spaces and dashes
        cardnumber = cardnumber.replace(/[ -]/g, '').replace(/[ _]/g, '');
        var match = /^(4{1,})|(5[1-5]{1,})|(6(?:011|5{1,}))|(3[47]{1,})|(3(?:0[0-5]|[36]|[38]){1,})|((?:2131|1800|35{1,}))$/.exec(cardnumber);

        if (match) {
            // List of card types, in the same order as the regex capturing groups
            var types = ['Visa', 'MasterCard', 'Discover', 'American Express',
                         'Diners Club', 'JCB'];
            for (var i = 1; i < match.length; i++) {
                if (match[i]) {
                    cardType = (types[i - 1]);
                    break;
                }
            }
            switch (cardType) {
                case 'Visa':
                    $(".payment-method-icon").removeClass("MasterCard AmEx Diners Discover JCB").addClass("Visa");
                    break;
                case 'MasterCard':
                    $(".payment-method-icon").removeClass(" Visa AmEx Diners Discover JCB").addClass("MasterCard");
                    break;
                case 'Discover':
                    $(".payment-method-icon").removeClass("MasterCard Visa AmEx Diners JCB").addClass("Discover");
                    break;
                case 'American Express':
                    $(".payment-method-icon").removeClass("MasterCard Visa Diners Discover JCB").addClass("AmEx");
                    break;
                case 'Diners Club':
                    $(".payment-method-icon").removeClass("MasterCard Visa AmEx Discover JCB").addClass("Diners");
                    break;
                case 'JCB':
                    $(".payment-method-icon").removeClass("MasterCard Visa AmEx Diners Discover").addClass("JCB");
                    break;
            }
        }
    }
}