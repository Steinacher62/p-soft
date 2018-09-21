using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;

namespace ch.appl.psoft
{
    /// <summary>
    /// Summary description for AppraisalAdd.
    /// </summary>
    public partial class AppraisalAdd : PsoftContentPage
	{
        protected long _personID = -1;
        protected long _skillGroupID = -1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _personID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], _personID);
            _skillGroupID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillGroupID"], _skillGroupID);

            DBData db = DBData.getDBData(Session);
            db.connect();

            string nextPage = "";
            try
            {
                db.beginTransaction();
                
                // create new appraisal...
                long appraisalID = db.newId("SKILLS_APPRAISAL");
                db.execute("insert into SKILLS_APPRAISAL (ID, PERSON_ID, APPRAISALDATE) values (" + appraisalID + ", " + _personID + ", GetDate())");

                // create skill-ratings based on existing function and job-skills...
                ArrayList IDs = db.lookup(new string[] {"ID", "FUNKTION_ID"}, "JOBPERSONV", "PERSON_ID=" + _personID, "", false);
                string jobIDs = "";
                string funktionIDs = "";
                for (int i=0; i<IDs.Count; i++){
                    if (i>0){
                        jobIDs += ",";
                        funktionIDs += ",";
                    }
                    jobIDs += ((string[])IDs[i])[0];
                    funktionIDs += ((string[])IDs[i])[1];
                }

                IDs = db.lookup(new string[] {"ID"}, "SKILL_LEVEL__SKILL_VALIDITY_V", "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and SKILL_VALIDITY_VALID_FROM<=GetDate() and SKILL_VALIDITY_VALID_TO>=GetDate() and (JOB_ID in (" + jobIDs + ") or FUNKTION_ID in (" + funktionIDs + "))", "", false);

                string sql = "";
                string attributes = "";
                string values = "";
                for (int i=0; i<IDs.Count; i++){
                    attributes = "SKILLS_APPRAISAL_ID"; values = appraisalID.ToString();
                    attributes += ",SKILLGROUP_ID"; values += ", SKILLGROUP_ID";
                    attributes += ",SKILL_NUMBER"; values += ", NUMBER";
                    attributes += ",RATING_LEVEL_PERCENTAGE"; values += ", -1";
                    attributes += "," + db.langAttrName("SKILL_RATING", "RATING_LEVEL_TITLE"); values += ", '-'";
                    attributes += "," + db.langAttrName("SKILL_RATING", "RATING_LEVEL_DESCRIPTION"); values += ", '-'";
                    attributes += "," + db.langExpand("SKILL_TITLE%LANG%", "SKILL_RATING", "SKILL_TITLE"); values += "," + db.langExpand("TITLE%LANG%", "SKILL_LEVEL__SKILL_VALIDITY_V", "TITLE");
                    attributes += "," + db.langExpand("SKILL_DESCRIPTION%LANG%", "SKILL_RATING", "SKILL_DESCRIPTION"); values += "," + db.langExpand("DESCRIPTION%LANG%", "SKILL_LEVEL__SKILL_VALIDITY_V", "DESCRIPTION");
                    attributes += ",DEMAND_LEVEL_NUMBER"; values += ", DEMAND_LEVEL_NUMBER";
                    attributes += "," + db.langExpand("DEMAND_LEVEL_TITLE%LANG%", "SKILL_RATING", "DEMAND_LEVEL_TITLE"); values += "," + db.langExpand("DEMAND_LEVEL_TITLE%LANG%", "SKILL_LEVEL__SKILL_VALIDITY_V", "DEMAND_LEVEL_TITLE");
                    attributes += "," + db.langExpand("DEMAND_LEVEL_DESCRIPTION%LANG%", "SKILL_RATING", "DEMAND_LEVEL_DESCRIPTION"); values += "," + db.langExpand("DEMAND_LEVEL_DESCRIPTION%LANG%", "SKILL_LEVEL__SKILL_VALIDITY_V", "DEMAND_LEVEL_DESCRIPTION");

                    sql = "insert into SKILL_RATING (" + attributes + ") select " + values + " from SKILL_LEVEL__SKILL_VALIDITY_V where ID=" + ((string[])IDs[i])[0];
                    db.execute(sql);
                }

                db.commit();

                nextPage = "AppraisalEdit.aspx?appraisalID=" + appraisalID + "&skillGroupID=" + _skillGroupID + "&personID=" + _personID;
            }
            catch (Exception ex)
            {
                db.rollback();
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
                if (nextPage == ""){
                    RedirectToPreviousPage();
                }
                else{
                    Response.Redirect(nextPage);
                }
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
