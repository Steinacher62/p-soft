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
    ///		Summary description for AddDutyCompetenceValidityHandsFreeCtrl.
    /// </summary>
    public partial class AddDutyCompetenceValidityHandsFreeCtrl : PSOFTInputViewUserControl
	{
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_DUTY_ID = "PARAM_DUTY_ID";
        public const string PARAM_DUTY_COMPETENCE_VALIDITY_ID = "PARAM_DUTY_COMPETENCE_VALIDITY_ID";




        protected System.Web.UI.WebControls.Table _cbTable;


		protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;

        protected DataTable _competenceLevels = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/FBS/Controls/AddDutyCompetenceValidityHandsFreeCtrl.ascx";}
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

        public long DutyCompetenceValidityID
        {
            get {return GetLong(PARAM_DUTY_COMPETENCE_VALIDITY_ID);}
            set {SetParam(PARAM_DUTY_COMPETENCE_VALIDITY_ID, value);}
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
                    CompetenceTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS,FBSModule.LANG_MNEMO_CT_COMPETENCE);
                    apply.Text = _mapper.get("apply");
                    apply.Visible = true;
                }
                _table = _db.getDataTableExt("select * from DUTY_VALIDITY where ID=-1", "DUTY_VALIDITY");
                _table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["VALID_FROM"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;


                InputType = InputMaskBuilder.InputType.Add;


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
                    DutyCompetenceValidityID = _db.newId("DUTY_COMPETENCE_VALIDITY");
                    DutyID = _db.newId("DUTY");
                    extendSql(sqlB, _table, "DUTY_ID", DutyID);
                    long id = _db.newId("DUTY_VALIDITY");
                    extendSql(sqlB, _table, "ID", id);

                    string sql = endExtendSql(sqlB);
                    if (sql.Length > 0)
                    {
                        _db.execute("insert into DUTY (ID) values ("+DutyID+")");
                        _db .execute("insert into DUTY_COMPETENCE_VALIDITY (ID,DUTY_ID,JOB_ID) values ("+DutyCompetenceValidityID+","+DutyID+","+JobID+")");
                        _db.execute(sql);
                    }

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
                Response.Redirect(Global.Config.baseURL + "/FBS/EditDutyCompetenceValidity.aspx?jobID=" + JobID + "&dutyID=" + DutyID + "&dutyCompetenceValidityID=" + DutyCompetenceValidityID);
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
