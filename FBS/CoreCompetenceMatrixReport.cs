using ch.appl.psoft.db;
/// TODO: language
///
using System.Collections;
using System.Data;
using System.Web.SessionState;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for DutyCatalogReport.
    /// </summary>
    public class CoreCompetenceMatrixReport 
    {

        //attributes

        const string FILE_EXTENSION = ".xls";


        protected HttpSessionState _session;
        protected string _imageDirectory;
        protected DBData _db; 

        string _filename = "";

        bool  INCLUSIVE = false;


        /// <summary>
        /// skills
        /// </summary>
        Hashtable skillGroups = new Hashtable();


        /// <summary>
        /// funktion list
        /// </summary>
        Function[] funtionList = null;
   

        MatrixReportInterface reportInterface = new MatrixReportInterface();

 
        long orgUnitID;
        string orgUnitTitle = "Alle Organisationseinheit"; 

        public string Filename
        {
            get{ return _filename;}
        }


        /// <summary>
        /// constructor
        /// </summary>        
        public CoreCompetenceMatrixReport(HttpSessionState Session, string imageDirectory, long orgUnitId) 
        {
            this._session = Session;
            this._imageDirectory = imageDirectory;
            this.orgUnitID = orgUnitId;

            _db = DBData.getDBData(Session);

            ///all function ids (inclusive in order to display unassigned functions) 
//--for exclusive query use: 
            string allfuncIdsSql = "select f.id from skill_level_validity cv, funktion f, job j where cv.funktion_id = f.id and j.funktion_id = f.id  group by f.id";
            if(orgUnitId > 0) 
            { //organisation unit restriction
                allfuncIdsSql = "select f.id from skill_level_validity cv, funktion f, job j where cv.funktion_id = f.id and j.funktion_id = f.id and j.orgentity_id = " + orgUnitId + "group by f.id";
                orgUnitTitle = (string)_db.lookup(_db.langAttrName("ORGENTITY", "TITLE"),"ORGENTITY","id = " + orgUnitId);
            }
//--inclusive query:         
            if(INCLUSIVE)  //change the sql query to inclusive 
            {
                string orgUnitRestriction = "";
                if(orgUnitId > 0)
                    orgUnitRestriction = " and j.id in (select id from job where ORGENTITY_ID = " + orgUnitId + ")"; //organisation unit restriction if required.
                allfuncIdsSql = "select f.id from funktion f, job j where f.id = j.funktion_id " + orgUnitRestriction + " group by f.id";
            }
//--

            DataTable allFunctionIdsTable = _db.getDataTableExt(allfuncIdsSql, "funktion");
            funtionList = new Function[allFunctionIdsTable.Rows.Count];
            for(int k = 0; k < allFunctionIdsTable.Rows.Count; k++)
            {
                long curFktId = (long)allFunctionIdsTable.Rows[k][0];    
                string curDes = (string)_db.lookup("title_de", "funktion","id="+curFktId);
                Function curfkt = new Function(curDes,curFktId);
                funtionList[k] = curfkt;
            }


            //skills
            string allskillsSql= "select s.id, sg.id, sg.title_de as skillgroup, sv.title_de as skill_title, sv.description_de as skill_description from skill s, skillgroup sg, skill_validity sv where s.skillgroup_id = sg.id and s.id = sv.skill_id and  s.id  in (select skill_id from skill_level_validity where funktion_id in (" + allfuncIdsSql + "))";
            DataTable allskillsTable = _db.getDataTableExt(allskillsSql, "skill");
            for(int k = 0; k < allskillsTable.Rows.Count; k++)
            {
                long skillId = (long)allskillsTable.Rows[k][0];
                long skillGroupKey = (long)allskillsTable.Rows[k][1];
                string skillGroupTitle = (string)allskillsTable.Rows[k][2];
                string skillTitle = (string)allskillsTable.Rows[k][3];
                string skillDescription = (string)allskillsTable.Rows[k][4];

                if ( !this.skillGroups.ContainsKey(skillGroupKey) ) 
                {
                    this.skillGroups.Add(skillGroupKey,new ItemGroup(skillGroupTitle,skillGroupKey));
                }
                ((ItemGroup)this.skillGroups[skillGroupKey]).addItem(new Skill(skillDescription,skillId));
            }

        }

      

        public void createReport() 
        {
            //1. step: Add title
            MatrixReportInterface.Cell[] title = new MatrixReportInterface.Cell [1];
            title[0] = new MatrixReportInterface.Cell(this.orgUnitTitle,true);
            reportInterface.addRow(title, false);
            reportInterface.addRow(new MatrixReportInterface.Cell [1], true);
   
            //2. step: Add all funtions
            MatrixReportInterface.Cell[] cells = new MatrixReportInterface.Cell [this.funtionList.Length + 1];
            //first cell is empty or contains a description
            cells[0] = new MatrixReportInterface.Cell("", true); //Kompetence/Funktion
            for(int l = 1; l < cells.Length; l++) 
            {
                cells[l] = new MatrixReportInterface.Cell(this.funtionList.GetValue(l-1).ToString(),true);
            }
            reportInterface.addRow(cells,true);

            //3. step: Add all competence and values
            foreach(DictionaryEntry entry in this.skillGroups)
            {
                ItemGroup item = (ItemGroup)entry.Value;
                //Title (Parent Aufgabe)
                MatrixReportInterface.Cell[] aufgabeTitles = new MatrixReportInterface.Cell [this.funtionList.Length + 1];
                aufgabeTitles[0] = new MatrixReportInterface.Cell(item.ToString(),true);
                for(int m = 1; m <  aufgabeTitles.Length; m++) aufgabeTitles[m] = new MatrixReportInterface.Cell("",false);
                reportInterface.addRow(aufgabeTitles,false);
                //Aufgaben
                reportItems(item.getList());

            }
         }

      void reportItems(ExcelItem[] list) 
      {
            for(int k = 0; k < list.Length; k++) 
            {
                MatrixReportInterface.Cell[] cells = new MatrixReportInterface.Cell [this.funtionList.Length + 1];
                ExcelItem cpt = (ExcelItem)list.GetValue(k);
                cells[0] = new MatrixReportInterface.Cell(cpt.ToString(), false);       

                for(int l = 0; l < this.funtionList.Length; l++) 
                {
                    cells[l+1] = new MatrixReportInterface.Cell(calculateValue( (ExcelItem)this.funtionList.GetValue(l), (ExcelItem)list.GetValue(k) ) , false );
                }
                reportInterface.addRow(cells,true);
            }

        }

        public string saveReport(string outputDirectory, string fileName) 
        {
            this._filename = fileName + "_" + ch.appl.psoft.Interface.SessionData.getSessionID(_session) + FILE_EXTENSION;
            reportInterface.printToExcel(outputDirectory + "/" + this._filename);
            return outputDirectory + "/" + this._filename;
        }

        private string calculateValue(ExcelItem funktion, ExcelItem skill) 
        {
            long fktId = funktion.ID;
            long skillId = skill.ID; 
            string ret = "";
            string sql = "select dl.* from skill_level_validity slv, demand_level dl where skill_id = " + skillId + " and funktion_id = " + fktId + " and slv.demand_level_id = dl.id";
            DataTable data = _db.getDataTableExt(sql, "demand_level");
            if(data.Rows.Count > 0) 
            {
                ret = (string)data.Rows[0][_db.langAttrName("demand_level","TITLE")];
            }
            return ret;
        }      
    }


   
 

    /// <summary>
    /// 
    /// </summary>
    class Skill: ExcelItem
    {
        public Skill(string description, long id)
            : base(description, id)
        {
        }

    }

}
