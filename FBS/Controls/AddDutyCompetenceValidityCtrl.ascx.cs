namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for AddDutyCompetenceValidityCtrl.
    /// </summary>
    public partial class AddDutyCompetenceValidityCtrl : PSOFTInputViewUserControl
	{
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_DUTY_ID = "PARAM_DUTY_ID";
        public const string PARAM_DUTY_COMPETENCE_VALIDITY_ID = "PARAM_DUTY_COMPETENCE_VALIDITY_ID";




        protected System.Web.UI.WebControls.Table _cbTable;


		protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;
        protected long _funktionID = -1;
        protected static string[] ICONS = {"funktion.gif","stelle.gif"};
        protected DataTable _competenceLevels = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/FBS/Controls/AddDutyCompetenceValidityCtrl.ascx";}
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

        public string HighLightNode {
            get {
                string retValue = "";
                if (DutyID > 0){
                    retValue = "highlightNode(" + DutyID + ");";
                }
                return retValue;
            }
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

			DutyTreeTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CT_DUTY_TREE);
            
            _db = DBData.getDBData(Session);
            
            try
			{
                _db.connect();

                if (!IsPostBack)
                {
                    CompetenceTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS,FBSModule.LANG_MNEMO_CT_COMPETENCE);
                    apply.Text = _mapper.get("apply");
                    apply.Visible = false;
                    _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + JobID, false), -1);
                }
                DataTable dutyTable = _db.getDataTable("select * from DUTY_VALIDITY where DUTY_ID=" + DutyID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate()", "DUTY_VALIDITY");
                if (dutyTable.Rows.Count > 0)
                {
                    DataRow dutyRow = dutyTable.Rows[0];

                    TableRow r = new TableRow();
                    competenceCB.Rows.Add(r);
                    r.VerticalAlign = VerticalAlign.Top;
                    TableCell c = new TableCell();
                    r.Cells.Add(c);
                    c.CssClass = "Detail_special";
                    c.Text = dutyRow["NUMBER"].ToString();
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.CssClass = "Detail_special";
                    c.Text = dutyRow[_db.langAttrName(dutyRow.Table.TableName, "TITLE")].ToString();

                    // Competence-levels...
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.RowSpan = 2;
                    c.VerticalAlign = VerticalAlign.Top;

                    bool isEditable = true;
                    bool isFunctionCompetence = false;
                    if (DutyCompetenceValidityID > 0)
                    {
                        isEditable = false;
                        isFunctionCompetence = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "DUTY_COMPETENCE_VALIDITY", "ID=" + DutyCompetenceValidityID, false), -1) > 0;
                    }
                    isEditable = (isFunctionCompetence)? false : isEditable;

                    //Description
                    r = new TableRow();
                    competenceCB.Rows.Add(r);
                    r.VerticalAlign = VerticalAlign.Top;
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 2;
                    c.CssClass = "Detail";
                    c.Text = dutyRow[_db.langAttrName(dutyRow.Table.TableName, "DESCRIPTION")].ToString();

                    _table = _db.getDataTableExt("select * from DUTY_COMPETENCE_VALIDITY where ID=" + DutyCompetenceValidityID, "DUTY_COMPETENCE_VALIDITY");
                    View = "DUTY_COMPETENCE_VALIDITY";                   

                    _table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["VALID_FROM"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    
                    LoadInput(_db, _table, competenceEdit);

                    if (isFunctionCompetence || !isEditable)
                    {
                        competenceEdit.Enabled = false;
                        apply.Enabled = false;
                    }

                    apply.Visible = true;
                }
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

		public string buildTree
		{
			get
			{
				Common.Tree tree = new Common.Tree("DUTYGROUP", "DUTYV", "DUTYGROUP_ID", Response, "", "AddDutyCompetenceValidity.aspx?jobID=" + JobID + "&dutyID=%ID");
                tree.extendLeafNode += new ExtendLeafNodeHandler(this.extendLeafNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                tree.LeafToolTipColum = "DESCRIPTION";
                DBData db = DBData.getDBData(Session);

				Response.Write("<script language=\"javascript\">\n");
				try
				{
					db.connect();
                    string [] roots = {db.lookup("ID", "DUTYGROUP", "PARENT_ID is null", false)};
					long [] iRoots = new long[roots.Length];
					string [] rootNames = new string[roots.Length];
					int i;
					for (i=0; i<roots.Length; i++)
					{
						iRoots[i] = long.Parse(roots[i]);
						rootNames[i] = "dutyGroupTree" + i;
					}
					tree.build(db,iRoots,rootNames);
				}
				catch (Exception ex)
				{
                    DoOnException(ex);
                }
				finally
				{
					db.disconnect();
				}
				return "</script>";
			}
		}

        private bool extendLeafNode(HttpResponse response, string nodeName,  DataRow row) 
        {
            string image = "";
            object[] objs = _db.lookup(new string[] {"FUNKTION_ID","JOB_ID"},"DUTY_COMPETENCE_VALIDITY","DUTY_ID="+DBColumn.GetValid(row["ID"],0L)+"and (FUNKTION_ID="+_funktionID+" or JOB_ID="+JobID+")");
            if (!DBColumn.IsNull(objs[0]))
                image = ICONS[0];
            else if (!DBColumn.IsNull(objs[1]))
                image = ICONS[1];
            else 
                return true;
            response.Write(nodeName+".prependHTML=\"<img align='absmiddle' src='../images/" + image + "'>&nbsp;&nbsp;\";\n");
            return true;
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
                    if (InputType == InputMaskBuilder.InputType.Add)
                    {
                        DutyCompetenceValidityID = _db.newId("DUTY_COMPETENCE_VALIDITY");
                        extendSql(sqlB, _table, "ID", DutyCompetenceValidityID);
                        extendSql(sqlB, _table, "JOB_ID", JobID);
                        extendSql(sqlB, _table, "DUTY_ID", DutyID);
                    }
                    string sql = endExtendSql(sqlB);
                    if (sql.Length > 0)
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
