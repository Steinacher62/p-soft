using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Objective.
    /// </summary>
    public class Objective : DBObject {
        private int _tasklistEnable = -1;
        // context 
        public const string ORGANISATION = "ORGANISATION";
        public const string PERSON = "PERSON";
        public const string PROJECT = "PROJECT";
        public const string SUPERVISOR = "SUPERVISOR";
        public const string SEARCH = "SEARCH";
        public const string SELECTION = "SELECTION";

        public const int RED    = 0;
        public const int ORANGE = 1;
        public const int GREEN  = 2;
        public const int GRAY   = 3;

        public const int UNDEFINED_TYP = 0;
        public const int ORGANISATION_TYP = 1;
        public const int ORGENTITY_TYP = 2;
        public const int JOB_TYP = 3;
        public const int PROJECT_TYP = 4;
        public const int PERSON_TYP = 5;
        public static int[] ALL_TYP = {0,1,2,3,4,5};
        public const string UNDEFINED_TYP_S = "0";
        public const string ORGANISATION_TYP_S = "1";
        public const string ORGENTITY_TYP_S = "2";
        public const string JOB_TYP_S = "3";
        public const string PROJECT_TYP_S = "4";
        public const string PERSON_TYP_S = "5";
        public const string ALL_TYP_S = "0,1,2,3,4,5";

        public enum State {
            DRAFTED = 0,
            RELEASED = 1,
            CONDITION = 2,
            ACCEPTED = 3,
            REFUSED = 4,
            NOTACTIVE = 5,
            DELETE = 6
        };

        public static int[] StateMask = {0x0001,0x0002,0x0004,0x0008,0x0010,0x0020,0x0040,0x0080,0x0100};
        public const int AllStateMask   = 0x0000FFFF;
        public const int CriticalMask   = 0x00010000;

        public Objective(DBData db, HttpSessionState session) : base (db,session) {}

        /// <summary>
        /// Has authorisation
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public bool hasAuthorisation(int access) {
            return _db.hasTableAuthorisation(access,"OBJECTIVE",true);
        }
        /// <summary>
        /// Get Objective by semaphore
        /// </summary>
        /// <param name="semaphore">0: red, 1: orange, 2: green, 3: gray</param>
        /// <returns></returns>
        public string getObjectiveBySemaphore(int semaphore) {
            string retValue = "";
            DataTable table = _db.getDataTable("select O.*,MT.NUMBER ISNUMBER from OBJECTIVEV O left join MEASUREMENT_TYPE MT on O.MEASUREMENT_TYPE_ID = MT.ID");
            int sem;
            int percent;

            foreach (DataRow row in table.Rows) {
                getSemaphore(row,out sem, out percent);
                if (sem == semaphore){
                    if (retValue != "") retValue += ",";
                    retValue += row["ID"].ToString();
                }
            }

            return retValue;
        }
        /// <summary>
        /// Get semaphore
        /// </summary>
        /// <param name="id">Objective id</param>
        /// <param name="sem">0: red, 1: orange, 2: green, 3: gray</param>
        /// <param name="percent">greater equal 0: percent</param>
        public void getSemaphore(long id, out int sem, out int percent) {
            DataTable table = _db.getDataTable("select O.*,MT.NUMBER ISNUMBER from OBJECTIVEV O left join MEASUREMENT_TYPE MT on O.MEASUREMENT_TYPE_ID = MT.ID where O.ID ="+id);

            sem = GRAY;
            percent = 100;
            if (table.Rows.Count > 0) getSemaphore(table.Rows[0], out sem, out percent);
        }
        private void getSemaphore(DataRow row, out int sem, out int percent) {
            sem = GRAY;
            percent = -1;
            try {
                object current = row["CURRENTVALUE"];
                bool isNumber = DBColumn.GetValid(row["ISNUMBER"],0) == 1;
                double currentValue = isNumber ? double.Parse(DBColumn.GetValid(current,"0")) : 0;
                object target = row["TARGETVALUE"];
                double targetValue = isNumber ? double.Parse(DBColumn.GetValid(target,"0")) : 0;
                object critical = row["CRITICALVALUE"];
                double critValue = isNumber ? double.Parse(DBColumn.GetValid(critical,"0")) : 0;
                int critDays = DBColumn.GetValid(row["CRITICALDAYS"],0);
                DateTime endDate;
                DateTime critDate;

                if (isNumber) percent = 0;
                if (isNumber && (targetValue * currentValue) != 0.0) percent = Math.Min(100,(int) (100.0 / targetValue * currentValue));

                if (DBColumn.IsNull(row["DATEOFREACHING"])) {
                    if (isNumber) sem = currentValue >= targetValue ? GRAY : GREEN;
                    else if (current != null && target != null) sem = current.Equals(target) ? GRAY : GREEN;
                }
                else if (isNumber && currentValue >= targetValue) sem = GRAY;
                else if (!isNumber && (current == null || target == null || current.Equals(target))) sem = GRAY;
                else {
                    endDate = (DateTime) row["DATEOFREACHING"];
                    if (DateTime.Now > endDate) sem = RED;
                    else  {
                        critDate = DateTime.Now.AddDays(critDays);
                        if (isNumber) {
                            if (critDate < endDate || currentValue >= critValue) sem = GREEN;
                            else sem = ORANGE;
                        }
                        else if (critDate < endDate) sem = GREEN;
                        else sem = ORANGE;
                    }
                }
                if (sem == GRAY && percent < 0) percent = 100;
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
        }

        /// <summary>
        /// Get turn ID
        /// </summary>
        public long turnId {
            get { return Validate.GetValid(_db.lookup("wert", "property", "gruppe='mbo' and title='turn'",false),0L); }
        }

        /// <summary>
        /// Is tasklist enable ?
        /// </summary>
        /// <returns></returns>
        public bool tasklistEnable {
            get {
                if (_tasklistEnable < 0 && Global.Config.isModuleEnabled("tasklist")) {
                    _tasklistEnable = _db.lookup("wert", "property", "gruppe='mbo' and title='task'",false) == "1" ? 1 : 0;
                }
                return _tasklistEnable > 0;
            }
        }

        /// <summary>
        /// Get types
        /// </summary>
        public DataTable types { 
            get { return _db.getDataTable("select id,"+_db.langAttrName("MEASUREMENT_TYPE","TITLE")+" from MEASUREMENT_TYPE order by "+_db.langAttrName("MEASUREMENT_TYPE","TITLE")); }
        }

        public DataTable organsationObjectives
        {
            get { return _db.getDataTable("select id, title from OBJECTIVE where TYP = 1 and OBJECTIVE_TURN_ID = " +  turnId.ToString()); }
        }

        /// <summary>
        /// Get type mnemonics
        /// </summary>
        public DataTable mnemonics { 
            get { return _db.getDataTable("select id,"+_db.langAttrName("MEASUREMENT_TYPE","MNEMONIC")+" from MEASUREMENT_TYPE order by "+_db.langAttrName("MEASUREMENT_TYPE","MNEMONIC")); }
        }

        /// <summary>
        /// get arguments
        /// </summary>
        public DataTable arguments { 
            get { return _db.getDataTable("select id,"+_db.langAttrName("OBJECTIVE_ARGUMENT","TITLE")+" from OBJECTIVE_ARGUMENT order by "+_db.langAttrName("OBJECTIVE_ARGUMENT","TITLE")); }
        }
        /// <summary>
        /// get turns
        /// </summary>
        public DataTable turns { 
            get { return _db.getDataTable("select id,"+_db.langAttrName("OBJECTIVE_TURN","TITLE")+" from OBJECTIVE_TURN order by "+_db.langAttrName("OBJECTIVE_TURN","TITLE")); }
        }

        /// <summary>
        /// Workflow enable ?
        /// </summary>
        public bool workflowEnable {
            get {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='workflow'"),"0");
                return val == "1";
            }
        }

        /// <summary>
        /// Workflow  state enable ?
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool workflowStateEnable(State state) {
            if (workflowEnable) {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='"+state.ToString().ToLower()+"'"),"0");
                return val == "1";
            }
            return false;
        }

        /// <summary>
        /// Get elapse days
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int workflowStateDays(State state) {
            if (workflowStateEnable(state)) {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='"+state.ToString().ToLower()+"_days'"),"0");
                return int.Parse(val);
            }
            return -1;
        }

        /// <summary>
        /// Get target tasklist flag
        /// </summary>
        /// <param name="state"></param>
        /// <returns>(bitset) 1: user, 2: supervisor</returns>
        public int workflowStateTarget(State state) {
            if (workflowStateEnable(state)) {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='"+state.ToString().ToLower()+"_target'"),"0");
                return int.Parse(val);
            }
            return 0;
        }

        /// <summary>
        /// Get critical tasklist target
        /// </summary>
        public int criticalTarget {
            get {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='critical_target'"),"0");
                return int.Parse(val);
            }
        }

        /// <summary>
        /// Get validation tasklist target
        /// </summary>
        public int validationTarget {
            get {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='validation_target'"),"0");
                return int.Parse(val);
            }
        }
        /// <summary>
        /// Get validation from
        /// </summary>
        public DateTime validationFrom {
            get {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='validation_from'"),"");
                if (val == "") return DateTime.MinValue;
                return DateTime.Parse(val,_db.dbColumn.UserCulture);
            }
        }
        /// <summary>
        /// Get validation to
        /// </summary>
        public DateTime validationTo {
            get {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='validation_to'"),"");
                if (val == "") return DateTime.MaxValue;
                return DateTime.Parse(val,_db.dbColumn.UserCulture);
            }
        }

        /// <summary>
        /// Get objective filter
        /// </summary>
        public string objectiveFilter {
            get {
                string val = DBColumn.GetValid(_db.lookup("wert","property","gruppe='mbo' and title='objectiveFilter'"),ALL_TYP_S);
                return val;
            }
        }

        /// <summary>
        /// Is objective filter exact person ?
        /// </summary>
        public bool isPersonFilterOnly {
            get { return objectiveFilter == PERSON_TYP.ToString(); }
        }

        /// <summary>
        /// Get competence of login user
        /// </summary>
        /// <param name="row">OBJECTIVE row</param>
        /// <returns>(bitset) 1: Verantwortlicher, 2: Vorgesetzter, 4: Stellenleiter, else 0</returns>
        public int getCompetence(DataRow row) {
            return getCompetence(row,_db.userId);
        }
        /// <summary>
        /// Get competence of the user for the objective
        /// </summary>
        /// <param name="row">OBJECTIVE row</param>
        /// <param name="user">user id</param>
        /// <returns>(bitset) 1: Verantwortlicher, 2: Ueberwachender, 4: Leiter, else 0</returns>
        public int getCompetence(DataRow row, long user) {
            int ret = 0;
            long id = 0;
            long jobId = 0;
            long oeId = 0;
            int typ = -1;

            if (!DBColumn.IsNull(row["PERSON_ID"])) {
                id = (long) row["PERSON_ID"];
                
                // read real rights from database, no calculations in the code / 21.06.10 / mkr
                // get id of main job (JOB_ID not set by caller)
                Int32 mainJobId = Convert.ToInt32(_db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID",
                                                  "EMPLOYMENT.PERSON_ID = " + id + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", mainJobId, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
                {
                    ret = 2;
                }
                
                //if (id == user) ret = 1;
                //if (_db.Person.getLeader(id,0,0,true) == user) ret += 6;
                //else if (_db.Person.isLeaderOfPerson (user,id,true)) ret += 4;
            }
            else if (!DBColumn.IsNull(row["JOB_ID"])) {
                jobId = (long) row["JOB_ID"];
                object[] ids = _db.lookup(new string[] {"person_id","orgentity_id","typ"},"jobemploymentv","id = "+ jobId);
                id = DBColumn.GetValid(ids[0],0L);
                if (id == user) ret = 1;
                oeId = DBColumn.GetValid(ids[1],0L);
                typ = DBColumn.GetValid(ids[2],0);
                if (typ == 0) {
                    if (_db.Person.getLeader(id,jobId,0,true) == user) ret += 6;
                    else if (_db.Person.isLeaderOfPerson (user,id,true)) ret += 4;
                }
                else if (typ == 1) {
                    if (id == user) ret += 4;
                    if (_db.Person.getLeader(id,jobId,oeId,true) == user) ret += 2;
                }
            }
            else if (!DBColumn.IsNull(row["ORGENTITY_ID"])) {
                oeId = DBColumn.GetValid(row["ORGENTITY_ID"],0L);
                id = DBColumn.GetValid(_db.lookup("person_id","jobemploymentv","orgentity_id = "+ oeId+" and typ = 1 and person_id = "+user),0L);
                if (id == user) ret = 5;
                if (_db.Person.getLeader(id,0,oeId,true) == user) ret += 2;
            }
            return ret;
        }
        /// <summary>
        /// Delete objective
        /// </summary>
        /// <param name="id">(Sub)root id</param>
        /// <param name="cascade">true: delete cascade</param>
        /// <returns>#deleted rows</returns>
        public override int delete(long id, bool cascade) {
            int num = 0;
            string sql = "";

            if (cascade) {
                sql = "select id from objective where parent_id = "+id;
                DataTable table = _db.getDataTable(sql,Logger.VERBOSE);

                foreach (DataRow row in table.Rows) {
                    num += delete((long) row[0],true);
                }
            }
            sql = "delete from objective where id = "+id;
            return num+_db.execute(sql);
        }

        /// <summary>
        /// Add objective to OE
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oeId"></param>
        /// <returns></returns>
        public int addObjectiveToOE(long id, long oeId) {
            string sql = "update objective set person_id=null,job_id=null,orgentity_id = "+oeId+" where id = "+id;
            return _db.execute(sql);
        }

        /// <summary>
        /// Remove objective from oe
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oeId"></param>
        /// <returns></returns>
        public int removeObjectiveFromOE(long id, long oeId) {
            string sql = "update objective set orgentity_id = null where id = "+id+(oeId > 0 ? " and orgentity_id="+oeId : "");
            return _db.execute(sql);
        }


        /// <summary>
        /// Get Orgentity id
        /// </summary>
        /// <param name="objId">objective id</param>
        /// <returns>oe id</returns>
        public long getOE(long objId) {
            object[] ids = _db.lookup(new string[] {"orgentity_id","job_id"},"objective","id="+objId);

            if (!DBColumn.IsNull(ids[0])) return (long) ids[0];
            else if (!DBColumn.IsNull(ids[1])) return DBColumn.GetValid(_db.lookup("orgentity_id","job","id="+ids[1]),0L);
            return 0;
        }

        /// <summary>
        /// Get Job by personid and objective id
        /// </summary>
        /// <param name="persId"></param>
        /// <param name="objId"></param>
        /// <returns>job id</returns>
        public long getJobByPersonObjective(long persId, long objId) {
            long id = DBColumn.GetValid(_db.lookup("job.ID","(job inner join employment emp on job.employment_id = emp.id) inner join objective obj on job.id = obj.job_id","emp.person_id="+persId+" and obj.id="+objId),0L);
            if (id <= 0) id = DBColumn.GetValid(_db.lookup("job.ID","(job inner join employment emp on job.employment_id = emp.id) inner join (orgentity oe inner join objective obj on oe.id = obj.orgentity_id) on job.orgentity_id = oe.id","emp.person_id="+persId+" and obj.id="+objId),0L);
            return id;
        }

        /// <summary>
        /// Get responsible person id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>person id</returns>
        public long getResponsiblePerson(long objId) {
            object[] ids = _db.lookup(new string[] {"job_id","orgentity_id","person_id","typ"},"objective","id="+objId);
            int typ =  DBColumn.GetValid(ids[3],0);
            long id = 0;

            switch (typ) {
            case Objective.PERSON_TYP:
                return DBColumn.GetValid(ids[2],0L);
            case Objective.JOB_TYP:
                id = DBColumn.GetValid(ids[0],0L);
                return DBColumn.GetValid(_db.lookup("emp.person_id","job inner join employment emp on job.employment_id = emp.id","job.id="+id),0L);
            case Objective.ORGENTITY_TYP:
                id = DBColumn.GetValid(ids[1],0L);
                return DBColumn.GetValid(_db.lookup("emp.person_id","job inner join employment emp on job.employment_id = emp.id","job.typ = 1 and job.orgentity_id="+id),0L);
            }
            return 0;
        }
        /// <summary>
        /// Get supervisor person id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>person id</returns>
        public long getSupervisorPerson(long objId) {
            object[] ids = _db.lookup(new string[] {"job_id","orgentity_id","person_id","typ"},"objective","id="+objId);
            int typ =  DBColumn.GetValid(ids[3],0);
            long id = 0;

            switch (typ) {
            case Objective.PERSON_TYP:
                id = DBColumn.GetValid(ids[2],0L);
                return DBColumn.GetValid(_db.lookup("p2.id","oepersonv p1 inner join oepersonv p2 on p1.oe_id = p2.oe_id","p2.job_typ = 1 and p1.id="+id),0L);
            case Objective.JOB_TYP:
                id = DBColumn.GetValid(ids[0],0L);
                return DBColumn.GetValid(_db.lookup("p.id","job inner join oepersonv p on job.orgentity_id = p.oe_id","p.job_typ = 1 and job.id="+id),0L);
            case Objective.ORGENTITY_TYP:
                id = DBColumn.GetValid(ids[1],0L);
                return DBColumn.GetValid(_db.lookup("p.id","orgentity oe inner join oepersonv p on oe.parent_id = p.oe_id","p.job_typ = 1 and oe.id="+id),0L);
            }
            return 0;
        }
    }
}
