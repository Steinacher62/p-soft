using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.psoft.Util;
using System;
using System.Data;
using System.Timers;
using System.Web;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for MBO.
    /// </summary>
    public class MBOModule : psoftModule {
        private LanguageMapper _map = null;
        private static Timer TasklistServer = null;
        private ElapsedEventHandler _tasklistHandler = null;
        private DBData _db = null;

        public MBOModule() : base()  {
            m_NameMnemonic = "mbo";
            // auf unsichtbar gesetzt wegen neuem Menu
            m_IsVisible = false;
            m_StartURL = "../MbO/Search.aspx";
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "MbO/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public override void Application_Start(HttpApplicationState application) {
            _map = LanguageMapper.getLanguageMapper(application);
            if (TasklistServer == null) {
                TasklistServer = new Timer();
                // Set the Interval to 5 min.
                TasklistServer.Interval = 5 * 60 * 1000;
                //TasklistServer.Interval = 10 * 1000;
                TasklistServer.AutoReset = true;
            }
            if (_tasklistHandler == null) _tasklistHandler = new ElapsedEventHandler(executeTask);
            TasklistServer.Elapsed += _tasklistHandler;
            TasklistServer.Enabled = Global.Config.isModuleEnabled("tasklist");
        }
        public override void Application_End(HttpApplicationState application) {
            if (TasklistServer != null) {
                TasklistServer.Enabled = false;
                TasklistServer.Elapsed -= _tasklistHandler;
            }
        }
        private void executeTask(object source, ElapsedEventArgs e) {
            _db = DBData.getDBData();

            _db.connect();
            try {
                string sql = "";

                if (!_db.Objective.tasklistEnable) return;

                int target = 0;
                sql = "select * from OBJECTIVE where FLAG > 0";
                DataTable table = _db.getDataTable(sql);

                foreach (DataRow row in table.Rows) {
                    int st = DBColumn.GetValid(row["STATE"],0);
                    Objective.State state = (Objective.State) st;
                    int flag = DBColumn.GetValid(row["FLAG"],0);
                    int targetFlag = flag;
                    int mask = Objective.StateMask[(int)state];
                    string tasklistSql = "insert into measure (tasklist_id,trigger_uid,title,responsible_person_id) values (";
                    tasklistSql += row["TASKLIST_ID"];
                    tasklistSql += ",";
                    string uid = row["UID"].ToString();
                    tasklistSql += uid;
                    tasklistSql += ",'%T',%P)";
                    long userId = 0;
                    long supervisorId = 0;

                    switch (state) {
                    case Objective.State.DRAFTED:
                    case Objective.State.RELEASED:
                        if ((flag & mask) > 0) {
                            int days = _db.Objective.workflowStateDays(state);
                            target = days >= 0 ? _db.Objective.workflowStateTarget(state) : 0;
                            if (days >= 0 && target > 0) {
                                DateTime stateDate = (DateTime) row["STATEDATE"];
                                stateDate = stateDate.AddDays(days);
                                DateTime date = DateTime.Now.AddDays(days);

                                if (stateDate <= DateTime.Now) {
                                    sql = tasklistSql.Replace("%T",DBColumn.toSql(_map.get("mbo","tasklist"+state)));
                                    if ((target & 1) > 0) userId = _db.Objective.getResponsiblePerson((long) row["ID"]);
                                    if ((target & 2) > 0) supervisorId = _db.Objective.getSupervisorPerson((long) row["ID"]);
                                    if (userId > 0 && userId != supervisorId) _db.execute(sql.Replace("%P",userId.ToString()));
                                    if (supervisorId > 0) _db.execute(sql.Replace("%P",supervisorId.ToString()));
                                    targetFlag &= ~mask;
                                }
                            }
                        }
                        userId = 0;
                        supervisorId = 0;
                        break;
                    default:
                        break;
                    }
                    //
                    if ((flag & Objective.CriticalMask) > 0) {
                        int sem = Objective.RED;
                        int percent = 0;
                        _db.Objective.getSemaphore((long) row["ID"],out sem,out percent);

                        if (sem == Objective.ORANGE) {
                            target = _db.Objective.criticalTarget;
                            sql = tasklistSql.Replace("%T",DBColumn.toSql(_map.get("mbo","tasklistCritical")));
                            if ((target & 1) > 0) userId = _db.Objective.getResponsiblePerson((long) row["ID"]);
                            if ((target & 2) > 0) supervisorId = _db.Objective.getSupervisorPerson((long) row["ID"]);
                            if (userId > 0 && userId != supervisorId) _db.execute(sql.Replace("%P",userId.ToString()));
                            if (supervisorId > 0) _db.execute(sql.Replace("%P",supervisorId.ToString()));
                        }
                        if (sem != Objective.GREEN) {
                            targetFlag &= ~Objective.CriticalMask;
                            userId = 0;
                            supervisorId = 0;
                        }
                    }
                    //
                    // validation
                    DateTime from = _db.Objective.validationFrom;
                    DateTime to = _db.Objective.validationTo;

                    if (DateTime.Now >= from && DateTime.Now <= to) {
                        target = _db.Objective.validationTarget;
                        sql = tasklistSql.Replace("%T",DBColumn.toSql(_map.get("mbo","tasklistValidation")));
                        sql = sql.Replace(","+uid+",",",null,");
                        if ((target & 1) > 0) userId = _db.Objective.getResponsiblePerson((long) row["ID"]);
                        if ((target & 2) > 0) supervisorId = _db.Objective.getSupervisorPerson((long) row["ID"]);
                        if (userId > 0 && userId != supervisorId) {
                            if (DBColumn.GetValid(_db.lookup("objective_flag","person","id="+userId),0) == 1) {
                                _db.execute(sql.Replace("%P",userId.ToString()));
                                sql = "update PERSON set OBJECTIVE_FLAG=0 where id = "+userId;
                                _db.execute(sql);
                            }
                        }
                        if (supervisorId > 0 && DBColumn.GetValid(_db.lookup("objective_flag","person","id="+userId),0) == 1) {
                            _db.execute(sql.Replace("%P",supervisorId.ToString()));
                            sql = "update PERSON set OBJECTIVE_FLAG=0 where id = "+supervisorId;
                            _db.execute(sql);
                        }
                        userId = 0;
                        supervisorId = 0;
                    }
                    if (targetFlag != flag) {
                        sql = "update OBJECTIVE set flag="+targetFlag+" where id = "+row["ID"];
                        _db.execute(sql);
                    }
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }
    }
}
