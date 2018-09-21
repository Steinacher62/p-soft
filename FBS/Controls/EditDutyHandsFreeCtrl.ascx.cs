namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Text;

    /// <summary>
    ///		Summary description for EditDutyHandsFreeCtrl.
    /// </summary>
    public partial class EditDutyHandsFreeCtrl : PSOFTInputViewUserControl
	{
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_DUTY_ID = "PARAM_DUTY_ID";




        protected System.Web.UI.WebControls.Table _cbTable;


		protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;

        protected DataTable _competenceLevels = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/FBS/Controls/EditDutyHandsFreeCtrl.ascx";}
		}

		#region Properities
        public long JobID
        {
            get {return GetLong(PARAM_JOB_ID);}
            set {SetParam(PARAM_JOB_ID, value);}
        }

        public long DutyID
        {
            get {return GetLong(PARAM_DUTY_ID);}
            set {SetParam(PARAM_DUTY_ID, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();
            _config = Global.Config;
            
            _db = DBData.getDBData(Session);
            
            try
			{
                _db.connect();

                if (!IsPostBack)
                {
                    CompetenceTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS,FBSModule.LANG_MNEMO_CT_DUTY);
                    apply.Text = _mapper.get("apply");
                    apply.Visible = true;
                }
                _table = _db.getDataTableExt("select * from DUTY_VALIDITY where DUTY_ID="+DutyID+" and VALID_FROM<=GetDate() and VALID_TO>=GetDate()", "DUTY_VALIDITY");
                _table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["VALID_FROM"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                

                InputType = InputMaskBuilder.InputType.Edit;


                LoadInput(_db, _table, competenceEdit);

                
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


        protected void apply_Click(object sender, System.EventArgs e) 
        {
            if (checkInputValue(_table, competenceEdit))
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    StringBuilder sqlB = getSql(_table, competenceEdit, true);
                    long dutyValidityID = -1;
                    foreach (DataRow row in _table.Rows)
                    {
                        dutyValidityID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                        break;
                    }
                    string sql = endExtendSql(sqlB);

                    _db.execute(sql);
                    sql = "update DUTY_VALIDITY set DUTY_VALIDITY.NUMBER = t.NUMBER, DUTY_VALIDITY."+_db.langAttrName("DUTY_VALIDITY", "TITLE")+" = t."+_db.langAttrName("DUTY_VALIDITY", "TITLE")+", DUTY_VALIDITY."+_db.langAttrName("DUTY_VALIDITY", "DESCRIPTION")+" = t."+_db.langAttrName("DUTY_VALIDITY", "DESCRIPTION") 
                        + " from (select NUMBER,"+_db.langAttrName("DUTY_VALIDITY", "TITLE")+","+_db.langAttrName("DUTY_VALIDITY", "DESCRIPTION")+" from DUTY_VALIDITY where ID="+dutyValidityID+") as t" 
                        + "  where DUTY_ID="+DutyID+" and ID<>"+dutyValidityID;
                    _db.execute(sql);

                    _db.commit();
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
                Response.Redirect(Global.Config.baseURL + "/FBS/JobDescription.aspx?jobID=" + JobID);
            }		
        }

        #region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
        }
		#endregion
	}
}
