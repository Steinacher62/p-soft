using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace ch.appl.psoft.Basics
{
    public partial class forgotPassword : System.Web.UI.Page
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            if (_map == null)
                _map = LanguageMapper.getLanguageMapper(Application);
            ((Label)PasswordRecovery.UserNameTemplateContainer.FindControl("TitleLabel")).Text = _map.get("titlePasswordRecovery");
            ((Label)PasswordRecovery.UserNameTemplateContainer.FindControl("UserNameLabel")).Text = _map.get("loginUsername");
            ((RequiredFieldValidator)PasswordRecovery.UserNameTemplateContainer.FindControl("UserNameRequired")).Text = _map.get("userNameRequired");
            ((Literal)PasswordRecovery.UserNameTemplateContainer.FindControl("UsernameNeeded")).Text = _map.get("usernameNeeded");
            ((Button)PasswordRecovery.UserNameTemplateContainer.FindControl("SubmitButton")).Text = _map.get("submit");


        }

        protected string eMailAddress;
        protected string newPassword;
        protected void PasswordRecovery_VerifyingUser(object sender, LoginCancelEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                long userId = Convert.ToInt32(db.lookup("ID", "PERSON", "LOGIN ='" + PasswordRecovery.UserName.Trim() + "'",0L));
                if (userId == 0)
                {
                    throw new System.InvalidOperationException(_map.get("checkUsername"));
                }
                eMailAddress = db.lookup("EMail", "PERSON", "LOGIN ='" + PasswordRecovery.UserName.Trim()+"'", "").ToString();
                if (eMailAddress.Equals(""))
                {
                    throw new System.InvalidOperationException(_map.get("missEmail"));
                }

                newPassword = Membership.GeneratePassword(7, 1);

                string newPasswordRecoveryMessage = _map.get("newPasswordRecoveryMessage").Replace("newPassword", newPassword).Replace("ApplicationName", Global.Config.ApplicationName);
                sendMail(db, "Neues Passwort", newPasswordRecoveryMessage, eMailAddress, userId);


                db.execute("UPDATE PERSON SET PASSWORD ='" + newPassword + "' WHERE ID =" + userId.ToString());

                e.Cancel = true;


            }
            catch (Exception ex)
            {
                ((Literal)PasswordRecovery.UserNameTemplateContainer.FindControl("FailureText")).Text = ex.Message;
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
                e.Cancel = true;
            }
        }


        protected void sendMail( DBData _db, string subject, string message, string toAddress, long userId)
        {
            _db.connect();
            
            MailMessage myMessage = new MailMessage();

            myMessage.From = new MailAddress(Global.Config.getModuleParam("dispatch", "UserName", ""));
            myMessage.To.Add(toAddress);
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

            string senderName = "Applikation";
            string receiverName = _db.lookup("PNAME", "PERSON", "ID = " + userId).ToString() + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = " + userId).ToString();
            message = message.Replace("sender", senderName);
            message = message.Replace("receiver", receiverName);

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
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(this.GetType(), "myalert", "alert('"+ _map.get("passwordRecoverySuccessful")+"'); window.location = \"" + "changePassword.aspx" + "\";", true);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
        }


    }
}