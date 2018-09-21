using Braintree;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Services;


namespace ch.appl.psoft.Payment
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für BraintreeService
    /// </summary>
    [WebService(Namespace = "http://p-soft.ch/webservices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BraintreeService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string SaveAddress(string Name, string Firstname, string Firm, string Street, string Zip, string City, string CountryCode)
        {

            string error = null;
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            try
            {

                AddUserBraintree(db, (long)Session["UserId"]);

                if ((int)db.lookup("COUNT(ID)", "ADDRESS", "PERSON_ID=" + (long)Session["UserId"]) > 0)
                {
                    db.execute("UPDATE ADDRESS SET ADDRESS1 = '" + Street.Replace("'", "''") + "', ZIP ='" + Zip.Replace("'", "''") + "', CITY='" + City.Replace("'", "''") + "', COUNTRY= '" + CountryCode.Replace("'", "''") + "' WHERE PERSON_ID =" + Session["UserId"]);
                }
                else
                {
                    db.execute("INSERT INTO ADDRESS(PERSON_ID, ADDRESS1, ZIP, CITY, COUNTRY) VALUES('" + Session["UserId"] + "', '" + Street.Replace("'", "''") + "', '" + Zip.Replace("'", "''") + "', '" + City.Replace("'", "''") + "', '" + CountryCode.Replace("'", "''") + "')");
                }

                //db.execute("UPDATE PERSON SET PNAME='" + Name.Replace("'", "''") + "', FIRSTNAME ='" + Firstname.Replace("'", "''")+"'");

                if (Convert.ToInt32(db.lookup("FIRM_ID", "ADDRESS", "PERSON_ID=" + (long)Session["UserId"], 0L)) == 0)
                {
                    db.execute("INSERT INTO FIRM (TITLE) VALUES ('" + Firm.Replace("'", "''") + "')");
                    db.execute("UPDATE ADDRESS SET FIRM_ID =" + db.lookup("max(ID)", "FIRM", ""));
                }
                else
                {
                    db.execute("UPDATE FIRM SET TITLE ='" + Firm.Replace("'", "''") + "' WHERE ID = " + db.lookup("FIRM_ID", "ADDRESS", "PERSON_ID =" + Session["UserId"]));
                }
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
                error = "Fehler: Daten wurden nicht gespeichert.";
            }
            finally
            {
                db.disconnect();
            }

            error = saveOrderAddress(Name, Firstname, Firm, Street, Zip, City, CountryCode);

            return error;
        }

        private string saveOrderAddress(string Name, string Firstname, string Firm, string Address, string Zip, string City, string CountryCode)
        {
            string error = null;
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            try
            {
                RegionInfo myRI1 = new RegionInfo(CountryCode);
                string Country = myRI1.DisplayName;
                db.execute("INSERT INTO PRODUCTS_ORDER (FIRM, PERSON_ID, NAME, FIRSTNAME, ADDRESS, ZIP, CITY, COUNTRY) VALUES ('" + Firm.Replace("'", "''") + "', " + Session["UserId"] + ", '" + Name.Replace("'", "''") + "', '" + Firstname.Replace("'", "''") + "', '" + Address.Replace("'", "''") + "', " + Zip + ", '" + City.Replace("'", "''") + "', '" + Country.Replace("'", "''") + "')");
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
                error = e.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return error;
        }

        [WebMethod]
        public string GetClientToken()
        {
            return GetGateway().ClientToken.Generate();
        }

        [WebMethod(EnableSession = true)]

        public string Pay(string Nonce, string PaymentType)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            DataTable product = db.getDataTable("SELECT * FROM PRODUCTS WHERE ID =" + Session["OrderItem"]);
            var request = new TransactionRequest
            {
                Amount = (decimal)product.Rows[0]["PRICE"],
                PaymentMethodNonce = Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = GetGateway().Transaction.Sale(request);

            string error = null;
            if (result.Message != null)
            {
                error = "Fehler: Bitte prüfen Sie ihre Kartendaten.";
            }
            else
            {
                error = saveInvoiceDat(Session["OrderItem"].ToString(), Session["UserId"].ToString(), PaymentType);
            }

            db.disconnect();
            return error;
        }

        [WebMethod(EnableSession = true)]
        public string CreateInvoce(string PaymentType)
        {
            string success = saveInvoiceDat(Session["OrderItem"].ToString(), Session["UserId"].ToString(), PaymentType);
            return success;
        }
        private string saveInvoiceDat(string OrderItem, string PersonId, string PaymentType)
        {
            string error = null;
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable product = db.getDataTable("SELECT TITLE, DESCRIPTION, PRICE, CURRENCY FROM PRODUCTS WHERE ID =" + OrderItem);
            long OrderId = (long)db.lookup("max(id)", "PRODUCTS_ORDER", "PERSON_ID =" + PersonId);
            string createDateTime = String.Format("{0:MM/dd/yyyy hh:mm:ss}", System.DateTime.Now);
            try
            {
                db.execute("UPDATE PRODUCTS_ORDER SET PRODUCT_ID = " + OrderItem + ", ORDER_DAT ='" + createDateTime + "', PRODUCT_TITLE = '" + product.Rows[0]["TITLE"].ToString().Replace("'", "''") + "', PRODUCT_DESCRIPTION ='" + product.Rows[0]["DESCRIPTION"].ToString().Replace("'", "''") + "', PRICE ='" + product.Rows[0]["PRICE"].ToString().Replace(",", ".") + "', CURRENCY ='" + product.Rows[0]["CURRENCY"] + "', PAYMENT_TYPE ='" + PaymentType + "'  WHERE ID =" + OrderId);
                Session.Add("OrderId", OrderId);
                Session.Add("BuyerId", PersonId);

                Session.Add("SellerAddressId", db.lookup("ID", "ADDRESS", "PERSON_ID= (SELECT ID FROM PERSON WHERE PNAME = 'Administrator')").ToString());
                Session.Add("SellerFirmId", db.lookup("FIRM_ID", "ADDRESS", "PERSON_ID= (SELECT ID FROM PERSON WHERE PNAME = 'Administrator')").ToString());
                Session.Add("PaymentType", PaymentType);

                long matrixRef = db.lookup("MATRIX_REF", "PRODUCTS", "ID=" + OrderItem, 0L);
                int numCards = db.lookup("NUMBER_OF_CARDS", "PRODUCTS", "ID=" + OrderItem, 0);

                createProduct(matrixRef, numCards, Convert.ToInt32(PersonId));
            }

            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
                error = e.ToString();
            }

            finally
            {
                db.disconnect();
            }

            return error;
        }

        private BraintreeGateway GetGateway()
        {
            BraintreeGateway gateway = new BraintreeGateway
            {
                //Environment = Braintree.Environment.SANDBOX,
                //MerchantId = "sxsmrqjmh4xngd9y",
                //PublicKey = "pj7cvxhdbj64zbsb",
                //PrivateKey = "33a93b7bd392594c299c8d85d6702a6e"

                Environment = Braintree.Environment.PRODUCTION,
                MerchantId = "nbwg9xx6pygb88b5",
                PublicKey = "7gnrsq597pdm9qj7",
                PrivateKey = "4fd937499f3ae9d0b7a254b4b3412f19"
            };

            return gateway;
        }

        private void AddUserBraintree(DBData db, long userId)
        {
            var CustomerRequest = new CustomerSearchRequest().
            Id.Is(userId.ToString());

            ResourceCollection<Customer> collection = GetGateway().Customer.Search(CustomerRequest);

            if (collection.MaximumCount == 0)
            {

                DataTable Customer = db.getDataTable("SELECT PNAME, FIRSTNAME, ID FROM PERSON WHERE ID =" + userId);

                var request = new CustomerRequest
                {
                    FirstName = Customer.Rows[0]["PNAME"].ToString(),
                    LastName = Customer.Rows[0]["FIRSTNAME"].ToString(),
                    Id = userId.ToString()
                };
                Result<Customer> result = GetGateway().Customer.Create(request);

                bool success = result.IsSuccess();


                string customerId = result.Target.Id;
            }
        }


        [WebMethod(EnableSession = true)]
        public void sendInvoice(string path)
        {
            string subject;
            string message;
            string address;
            subject = "Rechnungsbeleg";
            message = "Vielen Dank für Ihre Bestellung bei " + Global.Config.ApplicationName + ". Den Rechnungsbeleg finden Sie im Anhang.<br> Freundliche Grüsse Administrator " + Global.Config.ApplicationName;
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            address = db.lookup("EMAIL", "PERSON", "ID=" + db.userId).ToString();
            db.disconnect();
            sendMail(subject, HttpUtility.HtmlDecode(message), address, path);
        }

        private string sendMail(string subject, string message, string toAddress, string attachmentPath)
        {
            string ret = "";
            MailMessage myMessage = new MailMessage();

            myMessage.From = new MailAddress(Global.Config.getModuleParam("dispatch", "UserName", ""));
            myMessage.To.Add(toAddress);
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;
            myMessage.Attachments.Add(new Attachment(attachmentPath));

            myMessage.Body = message;
            SmtpClient mySmtpClient = new SmtpClient();
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(Global.Config.getModuleParam("dispatch", "UserName", ""), Global.Config.getModuleParam("dispatch", "passwordFrom", ""));
            mySmtpClient.Host = Global.Config.getModuleParam("dispatch", "smtpServer", "");
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            mySmtpClient.ServicePoint.MaxIdleTime = 1;

            try
            {
                mySmtpClient.Send(myMessage);
                ret = "ok";

            }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }

            return ret;
        }

        private void createProduct(long matrixRef, int numCards, long buyerId)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            if (numCards > 0)
            {
                int existingCards = (int)db.lookup("NUMBER_OF_CARDS", "PERSON", "ID=" + buyerId);
                if (existingCards < 0)
                {
                    existingCards = 0;
                }
                int newCards = existingCards + numCards;
                db.execute("UPDATE PERSON SET NUMBER_OF_CARDS =" + newCards + " WHERE ID =" + buyerId);

            }

            db.disconnect();
        }
    }
}




