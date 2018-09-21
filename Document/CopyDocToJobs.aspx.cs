using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;

namespace ch.appl.psoft.Document
{
    public partial class CopyDocToJobs : System.Web.UI.Page
    {  
        protected void Page_Load(object sender, EventArgs e)
        {
            // apply language
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }

            DBData db = DBData.getDBData(Session);
            db.connect();
            String clipBoardId=Request["ClipboardID"];
            long ownerId = db.lookup("id", "person", "clipboard_id=" + clipBoardId, 0L);
            string jobTitle = db.lookup ("JOB.TITLE_DE","PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "(JOB.HAUPTFUNKTION = 1) AND (PERSON.CLIPBOARD_ID = " + clipBoardId +")", " ");

            DataTable tblJobs = db.getDataTableExt("SELECT PERSON.CLIPBOARD_ID FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE(JOB.HAUPTFUNKTION = 1) AND (JOB.TITLE_DE = '" + jobTitle + "') AND (PERSON.ID <> " + ownerId + ")", new object[0]);
            DataTable tbldoc = db.getDataTableExt("SELECT * FROM DOCUMENT WHERE ID= '" + Request["DocumentId"] + "'", new object[0]);
            
            DataRow rowdoc = tbldoc.Rows[0];

            foreach (DataRow aktJob in tblJobs.Rows)
            {
                long folderRootId = db.lookup("folder_id", "clipboard", "id =" + aktJob[0].ToString() , 0L);
                long folder_id = db.lookup("id", "folder", "(root_id ='" + folderRootId + "') and (Title = 'Stellenbeschreibungen')", 0L);
                string sql = "insert into document (FOLDER_ID,INHERIT,TITLE,DESCRIPTION,AUTHOR,FILENAME,XFILENAME,VERSION,CREATED,CHECKOUT_STATE,CHECKIN_PERSON_ID,TYP,NUMOFDOCVERSIONS)  VALUES("+ folder_id +",'"+ rowdoc[3] +"','"+rowdoc[4] +"','" +rowdoc[5] +"','" +rowdoc[6] +"','" +rowdoc[7] +"','"+rowdoc[8] +"','" + rowdoc[9] +"','"+Convert.ToDateTime(rowdoc[10]).ToString("yyyy-MM-dd HH:mm:ss")+ "','" +rowdoc[11]+"','"+rowdoc[12]+"','"+rowdoc[16]+"','" +rowdoc[20]+"')";
                db.execute(sql);

            }
            db.disconnect();       
        }
    }
}