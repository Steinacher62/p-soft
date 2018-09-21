using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Basics
{
    public partial class changePassword : System.Web.UI.Page
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            if (_map == null)
                _map = LanguageMapper.getLanguageMapper(Application);
            ChangePassword1.DisplayUserName = true;
            ((Label)ChangePassword1.ChangePasswordTemplateContainer.FindControl("TitleLabel")).Text = _map.get("titleChangePassword");
            ((Label)ChangePassword1.ChangePasswordTemplateContainer.FindControl("UsernameText")).Text = _map.get("loginUsername");
            ((Label)ChangePassword1.ChangePasswordTemplateContainer.FindControl("CurrentPasswordLabel")).Text = _map.get("currentPassword");
            ((Label)ChangePassword1.ChangePasswordTemplateContainer.FindControl("NewPasswordLabel")).Text = _map.get("newPassword");
            ((Label)ChangePassword1.ChangePasswordTemplateContainer.FindControl("ConfirmNewPasswordLabel")).Text = _map.get("confirmNewPassword");
            ((RequiredFieldValidator)ChangePassword1.ChangePasswordTemplateContainer.FindControl("UserNameRequired")).Text = _map.get("userNameRequired");
            ((RequiredFieldValidator)ChangePassword1.ChangePasswordTemplateContainer.FindControl("CurrentPasswordRequired")).Text = _map.get("passwordRequired");
            ((RequiredFieldValidator)ChangePassword1.ChangePasswordTemplateContainer.FindControl("NewPasswordRequired")).Text = _map.get("newPasswordRequired");
            ((RequiredFieldValidator)ChangePassword1.ChangePasswordTemplateContainer.FindControl("ConfirmNewPasswordRequired")).Text = _map.get("confirmNewPasswordRequired");
            ((CompareValidator)ChangePassword1.ChangePasswordTemplateContainer.FindControl("NewPasswordCompare")).Text = _map.get("newPasswordCompare");
            ((Button)ChangePassword1.ChangePasswordTemplateContainer.FindControl("ChangePasswordPushButton")).Text = _map.get("titleChangePassword");
            ((Button)ChangePassword1.ChangePasswordTemplateContainer.FindControl("CancelPushButton")).Text = _map.get("cancel");


        }

        protected void ChangePassword_ChangingPassword(object sender, LoginCancelEventArgs e)
        {
            string userName = ChangePassword1.UserName;
            string currentPassword = ChangePassword1.CurrentPassword;
            string newPassword = ChangePassword1.NewPassword;
            ((Literal)ChangePassword1.ChangePasswordTemplateContainer.FindControl("FailureText")).Text = "";


            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                long userId = db.lookup("ID", "PERSON", "LOGIN = '" + userName + "'", 0L);
                if (userId == 0)
                {
                    throw new System.InvalidOperationException(_map.get("checkUsername"));

                }
                if (!db.lookup("PASSWORD", "PERSON", "ID=" + userId, "").ToString().Equals(currentPassword))
                {
                    throw new System.InvalidOperationException(_map.get("checkPassword"));
                }

                db.execute("UPDATE PERSON SET PASSWORD = '" + newPassword + "' WHERE LOGIN = '" + userName + "'");

                e.Cancel = true;

                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(this.GetType(), "myalert", "alert('" + _map.get("changePasswordSuccessful") + "'); window.location = \"" + "login.aspx" + "\";", true);

            }

            catch (Exception ex)
            {
                ((Literal)ChangePassword1.ChangePasswordTemplateContainer.FindControl("FailureText")).Text = ex.Message;
                e.Cancel = true;
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }
        }

        protected void ChangePassword_CancelButtonClick(object sender, EventArgs e)
        {
            Response.Redirect("login.aspx");
        }
    }
}