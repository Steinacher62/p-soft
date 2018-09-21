namespace ch.appl.psoft.Skills.Controls
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
    ///		Summary description for AddSkillLevelValidityCtrl.
    /// </summary>
    public partial class AddSkillLevelValidityCtrl : PSOFTInputViewUserControl
	{
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_SKILL_ID = "PARAM_SKILL_ID";
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_SKILL_LEVEL_VALIDITY_ID = "PARAM_SKILL_LEVEL_VALIDITY_ID";




        protected System.Web.UI.WebControls.Table _cbTable;


		protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;
        protected long _funktionID = -1;
        protected static string[] ICONS = {"funktion.gif","stelle.gif","user.gif"};
        protected DataTable _competenceLevels = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Skills/Controls/AddSkillLevelValidityCtrl.ascx";}
		}

		#region Properities
        public long JobID
        {
            get {return GetLong(PARAM_JOB_ID);}
            set {SetParam(PARAM_JOB_ID, value);}
        }
        public long SkillID
        {
            get {return GetLong(PARAM_SKILL_ID);}
            set {SetParam(PARAM_SKILL_ID, value);}
        }
        public long PersonID 
        {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }
        public long SkillLevelValidityID
        {
            get {return GetLong(PARAM_SKILL_LEVEL_VALIDITY_ID);}
            set {SetParam(PARAM_SKILL_LEVEL_VALIDITY_ID, value);}
        }

        public string HighLightNode {
            get {
                string retValue = "";
                if (SkillID > 0){
                    retValue = "highlightNode(" + SkillID + ");";
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

			DutyTreeTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CT_SKILL_TREE);
            
            _db = DBData.getDBData(Session);
            
            try
			{
                _db.connect();

                if (!IsPostBack)
                {
                    CompetenceTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS,SkillsModule.LANG_MNEMO_CT_SKILL);
                    apply.Text = _mapper.get("apply");
                    apply.Visible = false;
                    _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + JobID, false), -1);
                }
                DataTable dutyTable = _db.getDataTable("select * from SKILL_VALIDITY where SKILL_ID=" + SkillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate()", "SKILL_VALIDITY");
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
                    if (SkillLevelValidityID > 0)
                    {
                        isEditable = false;
                        isFunctionCompetence = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "SKILL_LEVEL_VALIDITY", "ID=" + SkillLevelValidityID, false), -1) > 0;
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

                    _table = _db.getDataTableExt("select * from SKILL_LEVEL_VALIDITY where ID=" + SkillLevelValidityID, "SKILL_LEVEL_VALIDITY");
                    View = "SKILL_LEVEL_VALIDITY";                   

                    _table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["VALID_FROM"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["DEMAND_LEVEL_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                
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
				Common.Tree tree = new Common.Tree("SKILLGROUP", "SKILLV", "SKILLGROUP_ID", Response, "", "AddSkillLevelValidity.aspx?jobID=" + JobID + "&personID=" + PersonID + "&skillID=%ID");
                tree.extendLeafNode += new ExtendLeafNodeHandler(this.extendLeafNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                tree.LeafToolTipColum = "DESCRIPTION";
                DBData db = DBData.getDBData(Session);

				Response.Write("<script language=\"javascript\">\n");
				try
				{
					db.connect();
                    string [] roots = {db.lookup("ID", "SKILLGROUP", "PARENT_ID is null", false)};
					long [] iRoots = new long[roots.Length];
					string [] rootNames = new string[roots.Length];
					int i;
					for (i=0; i<roots.Length; i++)
					{
						iRoots[i] = long.Parse(roots[i]);
						rootNames[i] = "skillGroupTree" + i;
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
            object[] objs = _db.lookup(new string[] {"FUNKTION_ID","JOB_ID","PERSON_ID"},"SKILL_LEVEL_VALIDITY","SKILL_ID="+DBColumn.GetValid(row["ID"],0L)+"and (FUNKTION_ID="+_funktionID+" or JOB_ID="+JobID+" or PERSON_ID="+PersonID+")");
            if (!DBColumn.IsNull(objs[0]))
                image = ICONS[0];
            else if (!DBColumn.IsNull(objs[1]))
                image = ICONS[1];
            else if (!DBColumn.IsNull(objs[2]))
                image = ICONS[2];
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
                        SkillLevelValidityID = _db.newId("SKILL_LEVEL_VALIDITY");
                        extendSql(sqlB, _table, "ID", SkillLevelValidityID);
                        if (JobID > 0)
                            extendSql(sqlB, _table, "JOB_ID", JobID);
                        if (PersonID > 0)
                            extendSql(sqlB, _table, "PERSON_ID", PersonID);
                        extendSql(sqlB, _table, "SKILL_ID", SkillID);

                        long demandLevelId = -1;
                        DataTable demandLevels = _db.getDataTable("select ID from DEMAND_LEVEL order by NUMBER");
                        foreach (DataRow row in demandLevels.Rows)
                        {
                            demandLevelId = ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L);
                            break;
                        }
                        extendSql(sqlB, _table, "DEMAND_LEVEL_ID", demandLevelId);
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
                Response.Redirect(Global.Config.baseURL + "/Skills/EditSkillLevelValidity.aspx?jobID=" + JobID + "&personID=" + PersonID + "&skillID=" + SkillID + "&skillLevelValidityID=" + SkillLevelValidityID);
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
