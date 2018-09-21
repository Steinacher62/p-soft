namespace ch.appl.psoft.SPZ.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class DetailView : PSOFTDetailViewUserControl {
        protected long _id = 0;
        protected string _backURL = "";
        protected string _deleteDetailMessage = "";
        protected string _deleteDetailURL = "";


        public static string Path {
            get {return Global.Config.baseURL + "/SPZ/Controls/DetailView.ascx";}
        }

		#region Properities

        /// <summary>
        /// Get/Set current id
        /// </summary>
        public long id {
            get {return _id;}
            set {
                _id = value;
            }
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            DBData db = DBData.getDBData(Session);

            db.connect();
            try
            {
                DataTable table = db.getDataTableExt("select * from OBJECTIVEV where ID=" + _id, "OBJECTIVEV");
                if (table.Rows.Count == 0) return;

                int typ = (int)table.Rows[0]["TYP"];

            //    switch (typ) {
            //    case Objective.PERSON_TYP:
            //    case Objective.PROJECT_TYP:
            //    case Objective.JOB_TYP:
            //        table.Columns["VALUEIMPLICIT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        break;
            //    default:
            //        table.Columns["VALUEIMPLICIT"].ExtendedProperties["DetailControlType"] = typeof(CheckBox);
            //        break;
            //    }
            //    switch (typ) {
            //    case Objective.ORGENTITY_TYP:
            //        table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
            //        table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = db.Orgentity.orgentities; 
            //        break;
            //    case Objective.JOB_TYP:
            //        table.Columns["JOB_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
            //        table.Columns["JOB_ID"].ExtendedProperties["In"] = db.Orgentity.jobs; 
            //        break;
            //    case Objective.PERSON_TYP:
            //        table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
            //        table.Columns["MEMO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
            //        table.Columns["PERSON_ID"].ExtendedProperties["In"] = db.Person.getWholeNameMATable(true);
            //        table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
            //        table.Columns["CURRENTVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["CRITICALDAYS"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["CRITICALVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["ARGUMENT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["TASKLIST"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["CURRENTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["argument_id"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            //        table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

            //        break;
            //    }

                table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                table.Columns["MEMO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                table.Columns["PERSON_ID"].ExtendedProperties["In"] = db.Person.getWholeNameMATable(true);
                table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                table.Columns["CURRENTVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["CRITICALDAYS"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["CRITICALVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["VALUEIMPLICIT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;  
                table.Columns["ARGUMENT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["TASKLIST"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["CURRENTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["argument_id"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                
                //if (!db.Objective.tasklistEnable) {
                //    table.Columns["TASKLIST"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                //}
                
                table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo","typ",false));
                table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo","state",false));
                table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["In"] = db.Objective.types;
                table.Columns["ARGUMENT_ID"].ExtendedProperties["In"] = db.Objective.arguments;
                table.Columns["OBJECTIVE_TURN_ID"].ExtendedProperties["In"] = db.Objective.turns;
                
                detailTab.Rows.Clear();
                base.View = "OBJECTIVE_OE";
                base.LoadDetail(db, table, detailTab);

                _deleteDetailURL = Global.Config.baseURL;
                if (DBColumn.GetValid(table.Rows[0]["PARENT_ID"],0L) > 0) _deleteDetailURL += "/SPZ/Detail.aspx?view=detail&id="+table.Rows[0]["PARENT_ID"];
                else _deleteDetailURL += "/SPZ/Search.aspx";
                _deleteDetailMessage = PSOFTConvert.ToJavascript(_mapper.get("mbo", "deleteObjective"));
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
