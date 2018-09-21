using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Energiedienst.Controls;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Energiedienst
{

    /// <summary>
    /// 
    /// </summary>
    public partial class Detail : PsoftTreeViewPage {
        private string _context = "";  // see Objective
        private long _contextId = 0;
        private string _view = "detail";  // trace, detail, add, edit, copy, link, replace
        private long _id = 0; // objective
        private int _typ = Objective.UNDEFINED_TYP;
        private string _myURL = "";
        private DBData _db = null;

        public CheckBox employeeConfirmBox;
        public CheckBox interviewDoneBox;

        #region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"],_context);
            string ids = Request.QueryString["id"];
            if (ids != null) {
                _id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"], 0L);
                if (_id == 0 && ids.Length > 2) _id = ch.psoft.Util.Validate.GetValid(ids.Substring(2), 0L);
            }
            _contextId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], 0L);
            base.SubNavMenuUrl = "/MbO/SubNavMenu.aspx?context="+_context+"&contextId="+_contextId+"','addObjective','"+_id;
        }
        protected override void AppendBodyOnLoad(System.Text.StringBuilder bodyOnLoad) {
            base.AppendBodyOnLoad (bodyOnLoad);
            bodyOnLoad.Append("if (typeof(checkFlags) == 'function') checkFlags();");
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            int typ = ch.psoft.Util.Validate.GetValid(Request.QueryString["typ"], -1);
            _view = ch.psoft.Util.Validate.GetValid(Request.QueryString["view"], _view);
            _myURL = this.Request.RawUrl;
            _db = DBData.getDBData(Session);
            _db.connect();
            if(_view.Equals("copy"))
            {
                DataTable objectveTable = _db.getDataTable("SELECT * FROM OBJECTIVE WHERE OBJECTIVE_TURN_ID = " + ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], "0") + " AND PERSON_ID = " + ch.psoft.Util.Validate.GetValid(Request.QueryString["Id"], "0"));
                DataTable newObjectiveIds = _db.getDataTable("SELECT ID FROM OBJECTIVE WHERE OBJECTIVE_TURN_ID = " + ch.psoft.Util.Validate.GetValid(Request.QueryString["newTurnId"], "0") + " AND PERSON_ID = " + ch.psoft.Util.Validate.GetValid(Request.QueryString["Id"], "0"));
                int counter = 1;
                int RecNo = newObjectiveIds.Rows.Count;
                long newID;
                foreach (DataRow objective in objectveTable.Rows)
                { 
                    String sql = "";
                    if (counter > RecNo)
                    {
                        
                        sql = "INSERT INTO OBJECTIVE (NUMBER, STRATEGIE_RELATION, DESCRIPTION, ACTIONNEED, TERMIN, ARGUMENT, MEASUREKRIT, MEMO,TASKLIST, TITLE , MEASUREMENT_TYPE_ID, TARGETVALUE, STATE, TYP, FLAG, CREATOR, PERSON_ID ,OBJECTIVE_TURN_ID) " +
                                "VALUES('" + objective["NUMBER"].ToString().Replace("'", "''") + "','" + objective["STRATEGIE_RELATION"].ToString().Replace("'", "''") + "','" + objective["DESCRIPTION"].ToString().Replace("'", "''") + "','" + objective["ACTIONNEED"].ToString().Replace("'", "''") + "','" + objective["TERMIN"].ToString().Replace("'", "''") + "','" + objective["ARGUMENT"].ToString().Replace("'", "''") + "','" + objective["MEASUREKRIT"].ToString().Replace("'", "''") +
                                   "','" + objective["MEMO"].ToString().Replace("'", "''") + "','" + objective["TASKLIST"].ToString().Replace("'", "''") + "','" + objective["TITLE"].ToString().Replace("'", "''") + "','" + objective["MEASUREMENT_TYPE_ID"] + "','" + objective["TARGETVALUE"] + "','" + objective["STATE"] +
                                   "'," + objective["TYP"] + "," + objective["FLAG"] + "," + _db.userId.ToString() + "," + objective["PERSON_ID"] + "," + ch.psoft.Util.Validate.GetValid(Request.QueryString["newTurnId"], "0") + ")";
                        _db.execute(sql);
                        newID = (long)_db.lookup("max(ID)", "OBJECTIVE", "");
                        _db.execute("INSERT INTO OBJECTIVE_PERSON_RATING (ID,PERSON_ID,OBJECTIVE_ID,RATING_WEIGHT,RATING) VALUES ("+ newID + "," + objective["PERSON_ID"] +"," +newID+ ","+ _db.lookup("RATING_WEIGHT", "OBJECTIVE_PERSON_RATING", "ID=" + objective["ID"])+",0)");
                    }
                    else
                    {
                        newID = (long)newObjectiveIds.Rows[counter-1][0];
                        sql = "UPDATE OBJECTIVE SET NUMBER='" + objective["NUMBER"].ToString().Replace("'", "''") + "',STRATEGIE_RELATION='" + objective["STRATEGIE_RELATION"].ToString().Replace("'", "''") + "',DESCRIPTION='" + objective["DESCRIPTION"].ToString().Replace("'", "''") + "', ACTIONNEED='" + objective["ACTIONNEED"].ToString().Replace("'", "''") + "', TERMIN ='" + objective["TERMIN"].ToString().Replace("'", "''") + "', ARGUMENT ='" + objective["ARGUMENT"].ToString().Replace("'", "''") + "', MEASUREKRIT='" + objective["MEASUREKRIT"].ToString().Replace("'", "''") +
                                   "', MEMO='" + objective["MEMO"].ToString().Replace("'", "''") + "',TASKLIST='" + objective["TASKLIST"].ToString().Replace("'", "''") + "', TITLE='" + objective["TITLE"].ToString().Replace("'", "''") + "', MEASUREMENT_TYPE_ID='" + objective["MEASUREMENT_TYPE_ID"] + "', TARGETVALUE='" + objective["TARGETVALUE"] +
                                   "', STATE='" + objective["STATE"] + "', TYP='" + objective["TYP"] + "', FLAG='" + objective["FLAG"] + "', CREATOR='" + _db.userId.ToString() + "', OBJECTIVE_TURN_ID='" +ch.psoft.Util.Validate.GetValid(Request.QueryString["newTurnId"], "0") +"' WHERE ID =" +newID.ToString();
                        _db.execute(sql);
                        _db.execute("UPDATE OBJECTIVE_PERSON_RATING SET PERSON_ID =" + objective["PERSON_ID"] + ", OBJECTIVE_ID = " + newID + ",RATING_WEIGHT =" + _db.lookup("RATING_WEIGHT", "OBJECTIVE_PERSON_RATING", "ID=" + objective["ID"]) + " ,RATING =0 WHERE ID = " + newID.ToString());
                    }
     
                    counter += 1;
                }
                Response.Redirect(base._config.baseURL+"/Energiedienst/Detail.aspx?view=detail&context=PERSON&contextId="+ objectveTable.Rows[0]["PERSON_ID"] +"&turnid="+Request.QueryString["newTurnId"]);
            }

            base.BreadcrumbCaption = base._mapper.get("mbo", _view + "Objective");
            base.BreadcrumbName += _view; 
            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PsoftPageLayout pageLayout = PageLayoutControl as PsoftPageLayout;
            pageLayout.PageTitle = base._mapper.get("mbo", _view + "Objective");
            //



            // get turn id from querystring, if not set, use default
            string turnid = _db.Objective.turnId.ToString();
            if (Request.QueryString["turnid"] != null && Request.QueryString["turnid"] != "")
            {
                turnid = Request.QueryString["turnid"];
            }
            pageLayout.ButtonPrintAttributes.Add("onClick",
                "javascript: window.open('ObjectiveReport.aspx?context=" + _context + "&contextId=" + _contextId + "&turnid=" + turnid + "','ObjectiveReport')");
            pageLayout.ButtonPrintToolTip = _mapper.get("mbo", "personReportTP");
            pageLayout.ButtonPrintVisible = _context == Objective.PERSON && _view == "detail";
            //Authorisation
            int objectiveAccess = _db.getTableAuthorisations(_db.userId, "OBJECTIVE", true);

            DDGLContentLayout control = (DDGLContentLayout)this.LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = control;

            PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            links.LinkGroup1.Caption = _mapper.get("actions");


            // selection of turn
            links.LinkGroup2.Caption = "Zielrunden";

            string turn_link = _myURL;

            // remove id from link to leave detail view empty / 18.10.10 / mkr
            turn_link = System.Text.RegularExpressions.Regex.Replace(turn_link, @"&id=[0-9]*", "&id=0");

            if (turn_link.Contains("turnid"))
            {
                turn_link = turn_link.Substring(0, turn_link.IndexOf("&turnid="));
            }

            DataTable zielrunden = _db.getDataTable("SELECT ID, TITLE_DE FROM OBJECTIVE_TURN", new object[0]);
            foreach (DataRow zielrunde in zielrunden.Rows)
            {
                links.LinkGroup2.AddLink("", zielrunde["TITLE_DE"].ToString(), turn_link + "&turnid=" + zielrunde["ID"]);
            }
            DetailView detail = (DetailView)this.LoadPSOFTControl(DetailView.Path, "_detail");
            detail.id = _id;
            detail.backURL = _myURL;
            SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);

            // placeholder for links



            //if (_typ != Objective.JOB_TYP && _typ != Objective.PERSON_TYP && _typ != Objective.PROJECT_TYP && !_db.Objective.isPersonFilterOnly)
            //{
            //    links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "newObjective"), "/mbo/Detail.aspx?typ=0&view=add&id=" + _id);
            //    links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "copyObjective"), "/mbo/Detail.aspx?typ=0&view=copy&id=" + _id);
            //    links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "linkOE"), "/mbo/Detail.aspx?view=link&context=" + _context + "&contextId=" + _contextId + "&id=" + _id);
            //}

            //links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "editObjective"), "/mbo/Detail.aspx?view=edit&context=" + _context + "&contextId=" + _contextId + "&id=" + _id);
            //links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "deleteObjective"), "javascript:	deleteDetailConfirm(" + _id + ")");

            //// add link to copy objective / 27.09.10 / mkr
            //// Martin: Kommentar auf folgender Zeile ausblenden um Link anzuzeigen
            //links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "copyObjectiveFrom"), "/MbO/CopyObjective.aspx?contextId=" + _contextId);

            //// add link to copy objective to OE / 26.10.10 / mkr
            //links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "copyObjectiveTo"), "/MbO/CopyObjectiveToOE.aspx?Id=" + _id);

            long mainJobId = _db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _contextId.ToString() + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)", 0L);
            int actTurnName =  Convert.ToInt32( _db.lookup("TITLE_DE", "OBJECTIVE_TURN", "ID = " + turnid));
            long lastYearTurnId = _db.lookup("ID", "OBJECTIVE_TURN", "TITLE_DE = " + (actTurnName - 1).ToString(),0L);
            if((int)_db.lookup("COUNT(OBJECTIVE_TURN_ID)","OBJECTIVE","OBJECTIVE_TURN_ID < "+ turnid +" AND PERSON_ID = " + _contextId)>0  &&  _db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", mainJobId, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
            {
                if (_db.lookup("top 1 viewed", "objective", "OBJECTIVE_TURN_ID = " + turnid + " AND PERSON_ID = " + _contextId, "null").ToString() == "null")
                {
                    links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), "Ziele vom Vorjahr kopieren", "/Energiedienst/Detail.aspx?view=copy&contextId=" + lastYearTurnId.ToString() + "&id=" + ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], "0") + "&newTurnId=" + turnid.ToString());
                }
            }
            SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);

           
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    

        }
		#endregion
    }
}
