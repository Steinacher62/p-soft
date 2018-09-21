using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Text;

namespace ch.appl.psoft.Project.Controls
{
    public partial class BillingAddCtrl : PSOFTInputViewUserControl
    {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";

        public const string DEFAULT_VAT_VALUE = "7.6";

        protected DataTable _table;
        protected DBData _db = null;


        public long ProjectID
        {
            get { return GetLong(PARAM_PROJECT_ID); }
            set { SetParam(PARAM_PROJECT_ID, value); }
        }


        public static string Path
        {
            get { return Global.Config.baseURL + "/Project/Controls/BillingAddCtrl.ascx"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

 

        protected override void DoExecute() {
            base.DoExecute ();
            if (!IsPostBack)
            {
               apply.Text = _mapper.get("apply");
            }

            _db = DBData.getDBData(Session);
            string sql = "select * from PROJECT_BILLING where ID=-1";

            _db.connect();
            try
            {
                _table = _db.getDataTableExt(sql, "PROJECT_BILLING");

                LoadInput(_db, _table, addTab);
                //set the default VAT value
                setInputValue(_table, this.addTab, "VAT_VALUE", DEFAULT_VAT_VALUE);
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

        /// <summary>
        /// For every new bill entry the project costs are updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apply_Click(object sender, System.EventArgs e)
        {
            if (!base.checkInputValue(_table, addTab))
            {
                return;
            }

            long newID = -1;

            _db.connect();
            try
            {
                _db.beginTransaction();
                StringBuilder sb = getSql(_table, addTab, true);
                newID = _db.newId(_table.TableName);

                extendSql(sb, _table, "ID", newID);
                extendSql(sb, _table, "PROJECT_ID", ProjectID);

                //new values (currently not used)
                {
                    double inputCredit = 0;
                    double.TryParse(getInputValue(_table, this.addTab, "CREDIT_VALUE").ToString(), out inputCredit);

                    double inputCreditor = 0;
                    double.TryParse(getInputValue(_table, this.addTab, "CREDITOR_VALUE").ToString(), out inputCreditor);

                    double inputDebitor = 0;
                    double.TryParse(getInputValue(_table, this.addTab, "DEBITOR_VALUE").ToString(), out inputDebitor);
                }


                string sql = endExtendSql(sb);

                if (sql != "")
                {
                    _db.execute(sql);

                    //grant base rights... (TODO!!! ok?)
                    long[] leaders = _db.Project.getLeaderPersonIDs(this.ProjectID);
                    _db.ProjectBilling.setDefaultRights(newID, leaders);

                    //fill in new "soll/ist" values into project entry
                    _db.ProjectBilling.updateExternalProjectCosts(this.ProjectID);

                    _db.commit();

                    Response.Redirect(psoft.Project.ProjectDetail.GetURL("context", "billing", "ID", ProjectID, "billingID", newID), false);
                }
                else
                    _db.rollback();
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