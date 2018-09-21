using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Novis.Controls
{
    public partial class CreateNovisReportCtrl : PSOFTUserControl
    {
        // Fields
        
        protected Button apply;


        // Methods
        private void apply_Click(object sender, EventArgs e)
        {
            if (this.chooseMaster.SelectedIndex >0)
            {
                String matrixId = chooseMaster.SelectedValue; 
                base.Response.Redirect("../Morph/MatrixDetail.aspx?matrixID=Novis"+matrixId);
            }
          
        }

        private void mapControls()
        {
            this.apply.Click += new EventHandler(this.apply_Click);
        }


        private void InitializeComponent()
        {
        }


        private void fillChooseMaster()
        {
            if (!IsPostBack)
            {
                this.apply.Text = "erstellen";
                DBData db = DBData.getDBData(base.Session);
                db.connect();
                DataTable masters = db.getDataTable("SELECT TITLE, ID FROM MATRIX WHERE NOVIS_ROOT_ID = ID AND IS_NOVIS_TEMPLATE = 1");
                this.chooseMaster.Items.Add(new RadComboBoxItem("", ""));
                for (int i = 0; i < masters.Rows.Count; i++)
                {
                    this.chooseMaster.Items.Add(new RadComboBoxItem(masters.Rows[i][0].ToString(), masters.Rows[i][1].ToString()));
                }
                this.chooseMaster.DataBind();
                db.disconnect();
            }
        }

        

        protected override void OnInit(EventArgs e)
        {
            
            this.InitializeComponent();
            fillChooseMaster();
            mapControls();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private string sqlAppendWhere(string sql, string clause)
        {
            sql = sql + ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }

        // Properties
      

        public static string Path
        {
            get
            {
                return (Global.Config.baseURL + "/Novis/Controls/CreateNovisReportCtrl.ascx");
            }
        }
    }
}