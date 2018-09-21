using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Text;

namespace ch.appl.psoft.Project.Controls
{
    public partial class BillingEditCtrl : PSOFTInputViewUserControl
    {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";
        public const string PARAM_PROJECT_BILLING_ID = "PARAM_PROJECT_BILLING_ID";

        protected DataTable _table;
        protected DBData _db = null;


        public long ProjectID
        {
            get { return GetLong(PARAM_PROJECT_ID); }
            set { SetParam(PARAM_PROJECT_ID, value); }
        }
        public long BillingID
        {
            get { return GetLong(PARAM_PROJECT_BILLING_ID); }
            set { SetParam(PARAM_PROJECT_BILLING_ID, value); }
        }

        public static string Path
        {
            get { return Global.Config.baseURL + "/Project/Controls/BillingEditCtrl.ascx"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

 

        protected override void DoExecute() {
            base.DoExecute ();

            InputType = InputMaskBuilder.InputType.Edit;

            if (!IsPostBack)
            {
               apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
            string sql = "select * from PROJECT_BILLING where ID = " + BillingID;

            _db.connect();
            try
            {
                _table = _db.getDataTableExt(sql, "PROJECT_BILLING");

                LoadInput(_db, _table, addTab);
            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally
            {
                _db.disconnect();
            }

        }


        private void apply_Click(object sender, System.EventArgs e)
        {
            if (!base.checkInputValue(_table, addTab))
            {
                return;
            }

            _db.connect();
            try
            {
                _db.beginTransaction();
                StringBuilder sb = getSql(_table, addTab, true);
                string sql = base.endExtendSql(sb);

                if (sql != "")
                {
                    _db.execute(sql);
                    _db.ProjectBilling.updateExternalProjectCosts(this.ProjectID);
                    _db.commit();
                    Response.Redirect(psoft.Project.ProjectDetail.GetURL("context", "billing", "ID", ProjectID, "billingID", this.BillingID), false);
                }
                else
                {
                    _db.rollback();
                }
            }
            catch (Exception ex)
            {
                _db.rollback();
                DoOnException(ex);
            }
            finally
            {
                _db.disconnect();
            }

        }

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            base.OnInit(e);
            mapControls();
        }

        private void mapControls()
        {
            apply.Click += new System.EventHandler(apply_Click);
        }
    }
}