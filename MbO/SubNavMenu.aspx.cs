using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for SubNavMenu.
    /// </summary>
    public partial class SubNavMenu : PsoftMenuPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            string context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"],"");
            long contextId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"],0L);
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl)this.LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get("mbo","objective");
            ctrl.addMenuItem(null, "search", _mapper.get("mbo","objectiveSearch"), Global.Config.baseURL + "/mbo/Search.aspx");
            DBData db = DBData.getDBData(Session);
            string url = Global.Config.baseURL + "/mbo/Detail.aspx?view=add";
            url += context == Objective.SEARCH ? "&typ=" : "&context="+context+"&contextId="+contextId+"&typ=";
            db.connect();
            try {
                int typ = db.Orgentity.getEmployment(db.userId);
                if (!db.Objective.isPersonFilterOnly) {
                    if (db.Objective.hasAuthorisation(DBData.AUTHORISATION.UPDATE)) {
                        ctrl.addMenuItem(null, "addObjective", _mapper.get("mbo","addObjective"), url+Objective.ORGANISATION_TYP);
                    }
                    else if (typ == 1) {
                        if (db.Orgentity.hasOEs(db.userId)) ctrl.addMenuItem(null, "addOEObjective", _mapper.get("mbo","addOEObjective"), url+Objective.ORGENTITY_TYP);
                        if (db.Orgentity.hasJobs(db.userId)) ctrl.addMenuItem(null, "addJobObjective", _mapper.get("mbo","addJobObjective"), url+Objective.JOB_TYP);
                    }
                }
                if (typ >= 0 && db.Orgentity.hasPersons(db.userId)) ctrl.addMenuItem(null, "addPersonObjective", _mapper.get("mbo","addPersonObjective"), url+Objective.PERSON_TYP);
                //TODO: an neues Projekt-Modell anpassen...
                //long id = DBColumn.GetValid(db.lookup("id","project","leader_person_id = "+db.userId),0L);
                //if (id > 0) ctrl.addMenuItem(null, "addObjective", _mapper.get("mbo","addProjectObjective"), url+((int) Objective.Typ.PROJECT));
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }
            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
        }
		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
