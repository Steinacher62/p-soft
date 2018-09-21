using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Net.Mail;
using System.ServiceModel.Activation;
using System.Web.Security;
using System.Web.Services;

namespace ch.appl.psoft.Basics
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für UserService
    /// </summary>
    [WebService(Namespace = "http://p-soft.ch/webservices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public string AddUser(string salutation, string name, string firstname, string userName)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string ret = "";
            try
            {

                if (Convert.ToInt16(db.lookup("COUNT(EMAIL)", "PERSON", "EMAIL = '" + userName.Trim() + "'", 0)) > 0)
                {
                    throw new System.InvalidOperationException("Ein Benutzer mit dieser E-Mail Adresse ist bereits vorhanden");
                }

                int sex;
                string mailSalutation;
                LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
                if (_map == null)
                    _map = LanguageMapper.getLanguageMapper(Application);

                if (salutation.Equals("Herr"))
                {
                    sex = 1; //männlich
                    mailSalutation = _map.get("salutationMale");
                }
                else
                {
                    sex = 2; //weiblich
                    mailSalutation = _map.get("salutationFemale");
                }
                string password = Membership.GeneratePassword(7, 1);

                db.execute("INSERT INTO PERSON (PNAME, FIRSTNAME, LOGIN, EMAIL, PASSWORD, SEX, TYP ) VALUES('" + name.Replace("'", "''") + "','" + firstname.Replace("'", "''") + "','" + userName.Trim().Replace("'", "''") + "','" + userName.Trim() + "','" + password.Replace("'", "''") + "','" + sex + "',1)");
                string userId = db.lookup("max(ID)", "PERSON", "").ToString();
                //db.execute("insert into accessor (id, visible, tablename, row_id) values ( " + userId + ", 0, 'PERSON', " + userId + ")");
                //db.execute("insert into ACCESSOR_GROUP_ASSIGNMENT (ACCESSOR_MEMBER_ID, ACCESSOR_GROUP_ID) values (" + userId + ", 1)");



                string newUserMessage = _map.get("yourAccessData").Replace("salutation", mailSalutation + " " + name).Replace("ApplicationName", Global.Config.ApplicationName).Replace("userName", userName.Trim().Replace("'", "''")).Replace("password", password);
                sendMail(_map.get("yourAccessDataTitel"), newUserMessage, userName.Trim());

                ret = "ok";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
                //e.Cancel = true;
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }

            return ret;

        }

        protected string sendMail(string subject, string message, string toAddress)
        {
            string ret = "";
            MailMessage myMessage = new MailMessage();

            myMessage.From = new MailAddress(Global.Config.getModuleParam("dispatch", "UserName", ""));
            myMessage.To.Add(toAddress);
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

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
    }
}
