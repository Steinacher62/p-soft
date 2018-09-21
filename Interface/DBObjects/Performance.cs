using ch.appl.psoft.db;
using ch.appl.psoft.Organisation;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Performance.
    /// </summary>
    public class Performance : DBObject
	{
        public enum ES_PERFORMANCE_ReportType {
            NormalReport = 0,
            GlobalReport
        }

        public enum ES_PERFORMANCE_CompositionType {
            PerformanceRating = 0,
            SkillsRating,
            MboRating
        }

        /// <summary>
        /// Holds a cache-entry of the job-IDs and employment-IDs the current user has the performance-rating right
        /// </summary>
        protected class RateableJobIDsCacheEntry : DBCacheEntry{
            private string _jobIDs = "";
            private string _employmentIDs = "";
            private bool _getJustFirst = false;

            public RateableJobIDsCacheEntry(bool getJustFirst) : base(600){ // the cache-entry remains fresh for 10 minutes.
                _getJustFirst = getJustFirst;
            }

            protected override void onRefreshing(DBData db){
                _jobIDs = _employmentIDs = "";
                string sql = "select distinct JOB.ID, JOB.EMPLOYMENT_ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1";
                if (!db.hasTableAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true)){
                    sql += db.getAccessRightsRowInnerJoinSQL("JOB", DBData.AUTHORISATION.INSERT, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true);
                }
                DataTable allJobsTable = db.getDataTable(sql);
                bool isFirstJob = true;
                bool isFirstEmployment = true;
                foreach (DataRow row in allJobsTable.Rows){
                    string jobID = DBColumn.GetValid(row[0], "");
                    if (jobID.Length > 0){
                        if (isFirstJob){
                            isFirstJob = false;
                        }
                        else{
                            _jobIDs += ",";
                        }
                        _jobIDs += jobID;
                    }
                    string employmentID = DBColumn.GetValid(row[1], "");
                    if (employmentID.Length > 0){
                        if (isFirstEmployment){
                            isFirstEmployment = false;
                        }
                        else{
                            _employmentIDs += ",";
                        }
                        _employmentIDs += employmentID;
                    }
                    if (_getJustFirst){
                        break;
                    }
                }
            }

            public string getRateableJobIDs(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _jobIDs;
                }
            }

            public string getRateableEmploymentIDs(DBData db){
                lock (this){
                    refreshIfStale(db);
                    return _employmentIDs;
                }
            }
        }

        public Performance(DBData db, HttpSessionState session) : base(db, session) { }

		public override int delete(long ID, bool cascade)
		{
			return delete(ID, cascade, true);
		}

        public int delete(long ID, bool cascade, bool createMessage) {
            int numDel = 0;
            string sql = "";

            sql = "delete from PERFORMANCERATING where ID=" + ID;
            numDel += _db.execute(sql);

            return numDel;
        }

        public int deleteItem(long ID) {
            int numDel = 0;
            string sql = "";

            sql = "delete from PERFORMANCERATING_ITEMS where ID=" + ID;
            numDel += _db.execute(sql);

            return numDel;
        }


		/// <summary>
		/// Return all valid performance rating levels
		/// </summary>
		/// <returns></returns>
		public DataTable getPerformanceRatingLevels()
		{
            if (Global.isModuleEnabled("foampartner")){

                return _db.getDataTableExt("select ID, " + _db.langAttrName("PERFORMANCE_LEVEL", "TITLE") + " from PERFORMANCE_LEVEL where VALID <> 0 order by RELATIV_WEIGHT desc", "PERFORMANCE_LEVEL");
            }
            else
            {
                return _db.getDataTableExt("select ID, " + _db.langAttrName("PERFORMANCE_LEVEL", "TITLE") + " from PERFORMANCE_LEVEL where VALID <> 0 order by RELATIV_WEIGHT asc", "PERFORMANCE_LEVEL");
            }
		}


		/// <summary>
		/// Returns table (ID, TITLE) of all performance criteria relevant to a job item
		/// </summary>
		/// <returns>DataTable with performance criteria items</returns>
		public DataTable getJobCriteriaTable(long jobID)
		{
			string functionID = _db.lookup("FUNKTION_ID","JOB","ID="+jobID.ToString(),false);
			// string criteriaSql = "select CRITERIA_REF, " + _db.langAttrName("CRITERIA_FUNCTION_V","CRITERIA_TITLE") + " from CRITERIA_FUNCTION_V where FUNCTION_ID = " + functionID + " and WEIGHT > 0 order by CRITERIA_REF";
			string criteriaSql = "select CRITERIA_REF, " + _db.langAttrName("CRITERIA_FUNCTION_V","CRITERIA_TITLE") + " from CRITERIA_FUNCTION_V where FUNCTION_ID = " + functionID + " order by CRITERIA_REF";
			return _db.getDataTableExt(criteriaSql,"CRITERIA_FUNCTION_V");
		}

		/// <summary>
		/// Returns table of all job expectation item
		/// </summary>
		/// <returns>DataTable with job expectation items</returns>
		public DataTable getJobExpectationTable(long jobID, string orderCol, string orderDir)
		{
			//string   expectationSql = "select ID, " + _db.langAttrName("JOB_EXPECTATION_V","CTITLE") + ", VALID_DATE, " + _db.langAttrName("JOB_EXPECTATION_V","DESCRIPTION") + " from JOB_EXPECTATION_V where JOB_REF=" + jobID + " order by " + orderCol + " " + orderDir;
			string expectationSql = "select * from JOB_EXPECTATION_V  where JOB_REF=" + jobID.ToString() + " order by " + orderCol + " " + orderDir;
			return _db.getDataTableExt(expectationSql,"JOB_EXPECTATION_V");
		}

		/// <summary>
		/// Returns table of all job expectation item for insert process
		/// </summary>
		/// <returns>DataTable with job expectation items</returns>
		public DataTable getJobExpectationInsertTable()
		{
			string expectationInsertSql = "select * from JOB_EXPECTATION where id = -1";
			return _db.getDataTableExt(expectationInsertSql,"JOB_EXPECTATION");
		}

		/// <summary>
		/// Returns table of all job expectation item for insert process
		/// </summary>
		/// <returns>DataTable with job expectation items</returns>
		public DataTable getJobExpectationEditTable(long expectationID)
		{
			string expectationEditSql = "select * from JOB_EXPECTATION where id = " + expectationID.ToString();
			return _db.getDataTableExt(expectationEditSql,"JOB_EXPECTATION");
		}


		/// <summary>
		/// Returns all criteria title of an active performance rating
		/// </summary>
		/// <param name="performanceRatingID">performance rating record id</param>
		/// <returns></returns>
		public DataTable getRatingCriteriaTitleTable(long performanceRatingID) 
		{
			// + " order by " + _db.langAttrName("PERFORMANCERATING_ITEMS","CRITERIA_TITLE"			
			string criteriaRatingSql = "select distinct CRITERIA_REF," + _db.langAttrName("PERFORMANCERATING_ITEMS","CRITERIA_TITLE") + " from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF = " + performanceRatingID.ToString() ;
			return _db.getDataTableExt(criteriaRatingSql,"PERFORMANCERATING_ITEMS");
		}


		public long getInworkPerformanceRating(long employmentID, bool selfRating)
		{
			return ch.psoft.Util.Validate.GetValid(_db.lookup("ID","PERFORMANCERATING","EMPLOYMENT_REF = " + employmentID + " AND " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0"),false),-1);
		}

		/// <summary>
		/// Determin if there exists an inwork performance rating for a given employment
		/// </summary>
		/// <param name="employmentID">employment record id</param>
		/// <param name="selfRating">self rating = true, leader rating = false</param>
		/// <returns></returns>
		public bool hasInworkPerformanceRating(long employmentID, bool selfRating)
		{
			long ratingID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID","PERFORMANCERATING","EMPLOYMENT_REF = " + employmentID + " AND " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0"),false),-1);
			return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)","PERFORMANCERATING_ITEMS","PERFORMANCERATING_REF = " + ratingID + " AND (RELATIV_WEIGHT is null or RELATIV_WEIGHT = 0)",false),-1) > 0;
		}



		public int getEmploymentRatingState(long ratingID)
		{
			int returnValue = -1;
			int countNull = ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)","PERFORMANCERATING_ITEMS","PERFORMANCERATING_REF = " + ratingID + " AND (RELATIV_WEIGHT is null or RELATIV_WEIGHT < 0)",false),0);
			int countAll = ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)","PERFORMANCERATING_ITEMS","PERFORMANCERATING_REF = " + ratingID ,false),0);

			switch(countNull)
			{
				case 0:
					returnValue = (countAll > 0 ? 2 : 0);
					break;
				default:
					returnValue = (countAll > countNull ? 1 : 0);
					break;
			}

			return returnValue;
		}


		/// <summary>
		/// Determin if a given performance rating has job expectation records
		/// </summary>
		/// <param name="performanceRatingID">performance rating record id</param>
		/// <param name="criteriaID">criteria id</param>
		/// <returns></returns>
		public bool hasJobExpectationRecords(long performanceRatingID, long criteriaID)
		{
			return ch.psoft.Util.Validate.GetValid(_db.lookup("count(ID)","PERFORMANCERATING_ITEMS","PERFORMANCERATING_REF = " + performanceRatingID.ToString() + " AND CRITERIA_REF" + criteriaID.ToString() + " AND EXPECTATION_REF is not null",false),-1) > 0;
		}


		/// <summary>
		/// Generate a new performance rating (PERFORMANCERATING, PERFORMANCERATING_ITEMS and PERFORMANCERATING_ARGUMENTS table records)
		/// </summary>
		/// <param name="employmentID">employment record id</param>
		/// <param name="selfRating">is self rating (=true)</param>
		/// <returns></returns>
		public long createNewPerformanceRating(long employmentID, long _jobID, bool selfRating)
		{
            long ratingID = -1;
			try
			{
				_db.connect();
				_db.beginTransaction();

                //update criteria weight if necessary for Laufenburg
                if (Global.isModuleEnabled("laufenburg"))
                {
                    int ifLeadership = (int)_db.lookup("LEADERSHIP", "PERSON", "ID = (SELECT PERSON_ID FROM EMPLOYMENT WHERE ID = " + employmentID + ")");
                    long funktionId = (long)_db.lookup("FUNKTION_ID", "JOB", "ID=" + _jobID);
                    if(ifLeadership == 0)
                    {
                        _db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT = 0 WHERE FUNCTION_REF =" + funktionId + " AND CRITERIA_REF IN (SELECT ID FROM PERFORMANCE_CRITERIA WHERE TITLE_DE = 'Führungskompetenz')");
                        _db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT = 50 WHERE FUNCTION_REF =" + funktionId + " AND CRITERIA_REF IN (SELECT ID FROM PERFORMANCE_CRITERIA WHERE TITLE_DE = 'Fachkompetenz')");
                        _db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT = 25 WHERE FUNCTION_REF =" + funktionId + " AND CRITERIA_REF IN (SELECT ID FROM PERFORMANCE_CRITERIA WHERE TITLE_DE = 'Selbstkompetenz' or TITLE_DE = 'Sozialkompetenz')");
                    }
                    if (ifLeadership == 1)
                    {
                        _db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT = 25 WHERE FUNCTION_REF =" + funktionId + " AND CRITERIA_REF IN (SELECT ID FROM PERFORMANCE_CRITERIA WHERE TITLE_DE = 'Führungskompetenz')");
                        _db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT = 25 WHERE FUNCTION_REF =" + funktionId + " AND CRITERIA_REF IN (SELECT ID FROM PERFORMANCE_CRITERIA WHERE TITLE_DE = 'Fachkompetenz')");
                        _db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT = 25 WHERE FUNCTION_REF =" + funktionId + " AND CRITERIA_REF IN (SELECT ID FROM PERFORMANCE_CRITERIA WHERE TITLE_DE = 'Selbstkompetenz' or TITLE_DE = 'Sozialkompetenz')");
                    }
                }

                //insert default expectations if available
                    DataTable jobs = _db.getDataTable("SELECT ID FROM JOB WHERE EMPLOYMENT_ID = " + employmentID);
                foreach (DataRow job in jobs.Rows)
                {
                    //get criterias for job
                    DataTable criterias = _db.getDataTable("SELECT CRITERIA_FUNCTION_WEIGHT.CRITERIA_REF"
                                                           + " FROM dbo.JOB INNER JOIN"
                                                           + " FUNKTION ON dbo.JOB.FUNKTION_ID = dbo.FUNKTION.ID INNER JOIN"
                                                           + " CRITERIA_FUNCTION_WEIGHT ON dbo.FUNKTION.ID = dbo.CRITERIA_FUNCTION_WEIGHT.FUNCTION_REF"
                                                           + " WHERE JOB.ID = " + job["ID"]);

                    DataTable functionIDt = _db.getDataTable("SELECT dbo.FUNKTION.ID FROM dbo.JOB INNER JOIN FUNKTION ON dbo.JOB.FUNKTION_ID = dbo.FUNKTION.ID WHERE dbo.JOB.ID = " + job["ID"]);
                    string functionID_exp = functionIDt.Rows[0][0].ToString();

                    foreach (DataRow criteria in criterias.Rows)
                    {
                        //check if expectation exists
                        DataTable exp_exists = _db.getDataTable("SELECT ID FROM JOB_EXPECTATION WHERE CRITERIA_REF = " + criteria["CRITERIA_REF"] + " AND JOB_REF = " + job["ID"]);
                        if (exp_exists.Rows.Count < 1)
                        {
                            //expectation doesn't exist, add it if default is available
                            DataTable exp_default = _db.getDataTable("SELECT * FROM JOB_EXPECTATION_DEFAULT WHERE CRITERIA_REF = " + criteria["CRITERIA_REF"]);
                            if (exp_default.Rows.Count > 0)
                            {
                                //default criteria exists, copy it
                                string descriptionAttrs = _db.langExpand("DESCRIPTION%LANG%", "JOB_EXPECTATION", "DESCRIPTION");
                                string titleAttrs = _db.langExpand("TITLE%LANG%", "JOB_EXPECTATION", "TITLE");
                                // only add criteria if weight is over 0
                                double criteriaWeight_exp = ch.psoft.Util.Validate.GetValid(_db.lookup("WEIGHT", "CRITERIA_FUNCTION_V", "FUNCTION_ID=" + functionID_exp + " and CRITERIA_REF=" + criteria["CRITERIA_REF"], false), 0.0);
                                if (criteriaWeight_exp > 0)
                                {
                                    int row = 0;
                                    foreach (DataRow expectations in exp_default.Rows)
                                    {
                                        string exp_sql = "INSERT INTO JOB_EXPECTATION (CRITERIA_REF, JOB_REF, " + descriptionAttrs + "," + titleAttrs + ", VALID_DATE) select CRITERIA_REF, '" + job["ID"] + "'," + descriptionAttrs + "," + titleAttrs + ",getdate() from JOB_EXPECTATION_DEFAULT where ID = " + exp_default.Rows[row]["ID"];
                                        _db.execute(exp_sql);
                                        row += 1;
                                    }
                                }
                            }
                        }
                    }
                }

                //create new PERFORMANCERATING record
                ratingID         = _db.newId("PERFORMANCERATING");
				DataTable titles =  _db.getDataTable("select E." + _db.langAttrName("EMPLOYMENT","TITLE") + " AS EMPLOYMENT_TITLE,"
													 +     " J." + _db.langAttrName("JOB","TITLE") + " AS JOB_TITLE," 
					                                 +     " F." + _db.langAttrName("FUNKTION","TITLE") + " AS FUNCTION_TITLE"
					                                 +" FROM EMPLOYMENT E"
                                                     +" LEFT OUTER JOIN JOB J ON J.ID = " + _jobID
					                                 +" LEFT OUTER JOIN FUNKTION F ON F.ID = J.FUNKTION_ID"
					                                 +" WHERE E.ID = " + employmentID);
				                                     
				string sql ="insert into PERFORMANCERATING (ID, EMPLOYMENT_REF, PERSON_ID, RATING_DATE, IS_SELFRATING, RATING_PERSON_REF,JOB_ID," 
					       + _db.langAttrName("PERFORMANCERATING","EMPLOYMENT_TITLE") + ","
					       + _db.langAttrName("PERFORMANCERATING","JOB_TITLE") + ","
					       + _db.langAttrName("PERFORMANCERATING","FUNCTION_TITLE") + ") "
					       + "select " + ratingID + ", "+ employmentID + "," + personId(employmentID) + ", GetDate(), " + (selfRating ? "1" : "0") + ", " + _db.userId;
				
				sql += ","+ _jobID;              
				sql = _db.dbColumn.AddToSql(sql + "," ,titles.Columns["EMPLOYMENT_TITLE"],titles.Rows[0]["EMPLOYMENT_TITLE"]);
                sql = _db.dbColumn.AddToSql(sql + "," ,titles.Columns["JOB_TITLE"],titles.Rows[0]["JOB_TITLE"]);
				sql = _db.dbColumn.AddToSql(sql + "," ,titles.Columns["FUNCTION_TITLE"],titles.Rows[0]["FUNCTION_TITLE"]);
                
                _db.execute(sql);
                //create all PERFORMANCERATING_ITEMS records
                Hashtable jobPartsTable = OrganisationModule.jobParts(_db, employmentID);
                DataTable criteriaTable = _db.getDataTable("select * from PERFORMANCE_CRITERIA");
                foreach (DataRow row in criteriaTable.Rows){
                    long criteriaID = DBColumn.GetValid(row["ID"], -1L);
                    if (criteriaID != -1){
                        double criteriaWeight = 0.0;
                        foreach (Object key in jobPartsTable.Keys) {
                            long jobID = Validate.GetValid(key.ToString(), -1L);
                            //double jobPart = Validate.GetValid(jobPartsTable[key].ToString(), 0.0);
                            double jobPart = Double.Parse(jobPartsTable[key].ToString());
                            
                            // HACK: divide by 10 if greater than 1
                            //if (jobPart > 1)
                            //{
                            //    jobPart = jobPart / 10;
                            //}

                            if (jobID != -1 && jobPart > 0) {
                                long functionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + jobID, false), -1);
                                if (functionID != -1){
                                    criteriaWeight += ch.psoft.Util.Validate.GetValid(_db.lookup("WEIGHT", "CRITERIA_FUNCTION_V", "FUNCTION_ID=" + functionID + " and CRITERIA_REF=" + criteriaID, false).ToString().Replace(",","."), 0.0) * jobPart;
                                }
                            }
                        }

                        if (criteriaWeight > 0){
							// old
                            //DataTable expectationTable = _db.getDataTableExt("select * from JOB_EXPECTATION_V where CRITERIA_REF=" + criteriaID + " and JOB_REF in (select ID from JOB where EMPLOYMENT_ID=" + employmentID + ")", "JOB_EXPECTATION_V");
							DataTable expectationTable = _db.getDataTableExt("select * from JOB_EXPECTATION_V where CRITERIA_REF=" + criteriaID + " and JOB_REF = "+ _jobID , "JOB_EXPECTATION_V");
                            
							switch(expectationTable.Rows.Count) {
                                case 0:
                                    long ratingItemID = _db.newId("PERFORMANCERATING_ITEMS");
                                    _db.execute("insert into PERFORMANCERATING_ITEMS ("
                                        + "ID,"
                                        + "PERFORMANCERATING_REF,"
                                        + "CRITERIA_REF,"
                                        + "CRITERIA_WEIGHT,"
                                        + "CRITERIA_TITLE_DE,"
                                        + "CRITERIA_TITLE_FR,"
                                        + "CRITERIA_TITLE_IT,"
                                        + "CRITERIA_TITLE_EN)"
                                        + " values ("
                                        + ratingItemID.ToString() + ","
                                        + ratingID + ","
                                        + criteriaID + ","
                                        + "'" + DBColumn.toSql(criteriaWeight.ToString(DBColumn.DBCulture)) + "',"
                                        + "'" + DBColumn.toSql(row["TITLE_DE"].ToString()) + "',"
                                        + "'" + DBColumn.toSql(row["TITLE_FR"].ToString()) + "',"
                                        + "'" + DBColumn.toSql(row["TITLE_IT"].ToString()) + "',"
                                        + "'" + DBColumn.toSql(row["TITLE_EN"].ToString()) + "')");

									//create PERFORMANCERATING_ARGUMENTS record
									long ratingArgumentID = _db.newId("PERFORMANCERATING_ARGUMENTS");
									_db.execute("insert into PERFORMANCERATING_ARGUMENTS (ID, PERFORMANCERATING_REF, PERFORMANCERATING_CRITERIA_REF, ARGUMENTS,PERFORMANCERATING_ITEM_ID) values (" + ratingArgumentID + "," + ratingID + "," + criteriaID + ",''," +ratingItemID+ ")");


                                    break;

                                default:
                                    foreach (DataRow expRow in expectationTable.Rows) {
                                        ratingItemID = _db.newId("PERFORMANCERATING_ITEMS");
                                        _db.execute("insert into PERFORMANCERATING_ITEMS ("
                                            + "ID,"
                                            + "PERFORMANCERATING_REF,"
                                            + "EXPECTATION_REF,"
                                            + "CRITERIA_REF,"
                                            + "CRITERIA_WEIGHT,"
                                            + "CRITERIA_TITLE_DE,"
                                            + "CRITERIA_TITLE_FR,"
                                            + "CRITERIA_TITLE_IT,"
                                            + "CRITERIA_TITLE_EN,"
											+ "EXPECTATION_TITLE_DE,"
											+ "EXPECTATION_TITLE_FR,"
											+ "EXPECTATION_TITLE_IT,"
											+ "EXPECTATION_TITLE_EN,"
                                            + "EXPECTATION_DESCRIPTION_DE,"
                                            + "EXPECTATION_DESCRIPTION_FR,"
                                            + "EXPECTATION_DESCRIPTION_IT,"
                                            + "EXPECTATION_DESCRIPTION_EN,"
                                            + "EXPECTATION_VALID_DATE)"
                                            + " values ("
                                            + ratingItemID.ToString() + ","
                                            + ratingID + ","
                                            + expRow["ID"].ToString() + ","
                                            + criteriaID + ","
                                            + "'" + DBColumn.toSql(criteriaWeight.ToString(DBColumn.DBCulture)) + "',"
                                            + "'" + DBColumn.toSql(row["TITLE_DE"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(row["TITLE_FR"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(row["TITLE_IT"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(row["TITLE_EN"].ToString()) + "',"
											+ "'" + DBColumn.toSql(expRow["TITLE_DE"].ToString()) + "',"
											+ "'" + DBColumn.toSql(expRow["TITLE_FR"].ToString()) + "',"
											+ "'" + DBColumn.toSql(expRow["TITLE_IT"].ToString()) + "',"
											+ "'" + DBColumn.toSql(expRow["TITLE_EN"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(expRow["DESCRIPTION_DE"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(expRow["DESCRIPTION_FR"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(expRow["DESCRIPTION_IT"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(expRow["DESCRIPTION_EN"].ToString()) + "',"
                                            + "'" + DBColumn.toSql(((DateTime)expRow["VALID_DATE"]).ToString(DBColumn.DBCulture)) + "')");

										//create PERFORMANCERATING_ARGUMENTS record
										ratingArgumentID = _db.newId("PERFORMANCERATING_ARGUMENTS");
										_db.execute("insert into PERFORMANCERATING_ARGUMENTS (ID, PERFORMANCERATING_REF, PERFORMANCERATING_CRITERIA_REF, ARGUMENTS,PERFORMANCERATING_ITEM_ID) values (" + ratingArgumentID + "," + ratingID + "," + criteriaID + ",''," + ratingItemID + ")");

                                    }
                                    break;
                            }
                        }
                    }
                }

				_db.commit();
			}
			catch(Exception ex)
			{
				Logger.Log(ex, Logger.ERROR);
                _db.rollback();
                ratingID = -1;
			}
			finally
			{
				_db.disconnect();
			}

			return ratingID;
		}

		/// <summary>
		/// Returns all items for a given employment rating
		/// </summary>
		/// <returns>DataTable with job expectation items</returns>
		

		public DataTable getEmploymentRatingTable(long employmentRatingID, string orderCol, string orderDir)
		{
			string ratingSql = "select * from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF = " + employmentRatingID.ToString() + " order by " + orderCol + " " + orderDir;
			return _db.getDataTableExt(ratingSql,"PERFORMANCERATING_ITEMS");
		}


		/// <summary>
		/// Returns all items for a given employment rating criteria
		/// </summary>
		/// <returns>DataTable with job expectation items</returns>
		public DataTable getEmploymentRatingTable(long employmentRatingID, long criteriaID,long ratingItemId)
		{
			string criteriaRatingSql = "select * from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF = " + employmentRatingID.ToString() + " and CRITERIA_REF = " + criteriaID.ToString();
			if(ratingItemId > 0)
				criteriaRatingSql += " and ID  = " + ratingItemId.ToString();
			return _db.getDataTableExt(criteriaRatingSql,"PERFORMANCERATING_ITEMS");
		}

		public DataTable getEmploymentRatingViewTable(long employmentRatingID, long criteriaID, long ratingItemId)
		{
			string ratingSql = "select * from PERFORMANCERATING_ITEMS_V where PERFORMANCERATING_REF = " + employmentRatingID.ToString() + " and CRITERIA_REF = " + criteriaID.ToString();
			if(ratingItemId > 0)
				ratingSql += " and ID = " + ratingItemId.ToString();
			return _db.getDataTableExt(ratingSql,"PERFORMANCERATING_ITEMS_V");
		}

        public DataTable getEmploymentRatingViewTableFirstItem(long employmentRatingID)
        {
            string ratingSql = "select top(1) * from PERFORMANCERATING_ITEMS_V where PERFORMANCERATING_REF = " + employmentRatingID.ToString();
            return _db.getDataTableExt(ratingSql, "PERFORMANCERATING_ITEMS_V");
        }


        public DataTable getCriteriaArgumentTable(long employmentRatingID, long criteriaID, long itemID)
		{
			string argumentCriteriaSql = "select * from PERFORMANCERATING_ARGUMENTS where PERFORMANCERATING_REF = " + employmentRatingID.ToString() + " and PERFORMANCERATING_CRITERIA_REF = " + criteriaID.ToString();
			if(itemID > 0 )
			{
				//long itemID = DBColumn.GetValid(_db.lookup("ID","PERFORMANCERATING_ITEMS","PERFORMANCERATING_REF = " + employmentRatingID + " AND EXPECTATION_REF =" + jobexpextationID),-1L);
                argumentCriteriaSql += " AND PERFORMANCERATING_ITEM_ID = " + itemID;

			}
			return _db.getDataTableExt(argumentCriteriaSql,"PERFORMANCERATING_ARGUMENTS");
		}


		/// <summary>
        /// if jobId in -1 (unassigned) performance rating entries with JOB_ID equal to null are returned. 
		/// </summary>
		/// <returns>DataTable with job expectation items</returns>
		public DataTable getEmploymentRatingHistoryTable(long employmentID,long jobId, bool selfRating, string orderCol, string orderDir)
		{
            string jobIdExpression = "JOB_ID = " + jobId.ToString();
            if (jobId == -1)
            {
                jobIdExpression = "JOB_ID is null";
            }
            if (jobId == -2)
            {
                jobIdExpression = "(JOB_ID > 0 OR JOB_ID is null)";
            }

            // Eigene Leistungsbewertungen ausblenden wenn LB datum > Sperrdatum und im Conig Option eingeschaltet ist 
            // MSR 01.13.2011 
            
            object lockDat = _db.lookup("DATUM_WERT", "PROPERTY", "Gruppe = 'performance' and TITLE = 'lock'");

            string ratingHistorySql;
            if (!Global.isModuleEnabled("energiedienst"))
            {
                if (Global.Config.getModuleParam("performance", "hideOwnPerformanceratingByLockdate", "0") == "1" && lockDat != DBNull.Value && personId(employmentID) == _db.userId.ToString())
                {
                    ratingHistorySql = "select * from PERFORMANCERATING where " + jobIdExpression + " AND EMPLOYMENT_REF = " + employmentID.ToString() + " and " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0") + " and RATING_DATE < '" + ((DateTime)lockDat).ToString("yyyy.MM.dd") + "' order by " + orderCol + " " + orderDir;
                }
                else
                {
                    ratingHistorySql = "select * from PERFORMANCERATING where " + jobIdExpression + " AND EMPLOYMENT_REF = " + employmentID.ToString() + " and " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0") + " order by " + orderCol + " " + orderDir;
                }
            }
            else
            {
                //Eigene LB ausblenden wenn Gespräch noch nicht bestätigt ist
                if (_db.userId == (long)_db.lookup("PERSON_ID", "EMPLOYMENT", "ID =" + employmentID))
                {
                    if (Global.Config.getModuleParam("performance", "hideOwnPerformanceratingByLockdate", "0") == "1" && lockDat != DBNull.Value && personId(employmentID) == _db.userId.ToString())
                    {
                        ratingHistorySql = "select * from PERFORMANCERATING where " + jobIdExpression + " AND EMPLOYMENT_REF = " + employmentID.ToString() + " and " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0") + " and RATING_DATE < '" + ((DateTime)lockDat).ToString("yyyy.MM.dd") + "' and not INTERVIEW_DONE is NULL order by " + orderCol + " " + orderDir;
                    }
                    else
                    {
                        ratingHistorySql = "select * from PERFORMANCERATING where " + jobIdExpression + " AND EMPLOYMENT_REF = " + employmentID.ToString() + " and " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0") + "  and not INTERVIEW_DONE is NULL order by " + orderCol + " " + orderDir;
                    }
                }
                else
                {
                    if (Global.Config.getModuleParam("performance", "hideOwnPerformanceratingByLockdate", "0") == "1" && lockDat != DBNull.Value && personId(employmentID) == _db.userId.ToString())
                    {
                        ratingHistorySql = "select * from PERFORMANCERATING where " + jobIdExpression + " AND EMPLOYMENT_REF = " + employmentID.ToString() + " and " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0") + " and RATING_DATE < '" + ((DateTime)lockDat).ToString("yyyy.MM.dd") + "' order by " + orderCol + " " + orderDir;
                    }
                    else
                    {
                        ratingHistorySql = "select * from PERFORMANCERATING where " + jobIdExpression + " AND EMPLOYMENT_REF = " + employmentID.ToString() + " and " + (selfRating ? "IS_SELFRATING <> 0" : "IS_SELFRATING = 0") + " order by " + orderCol + " " + orderDir;
                    }
                }
            }
            

            return _db.getDataTableExt(ratingHistorySql,"PERFORMANCERATING");
		}


		public string getRatingPageDateTitle(long ratingID)
		{
			DateTime dt = _db.lookup("RATING_DATE","PERFORMANCERATING","ID=" + ratingID.ToString(),DateTime.Now);
            return dt.ToString("dd.MM.yyyy");
		}


		public string getPerformanceRatingJobTitle(long performanceRatingId)
		{
			return  ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("PERFORMANCERATING","JOB_TITLE"),"PERFORMANCERATING","ID=" + performanceRatingId,false),"");
		}

		public string getPerformanceRatingFunctionTitle(long performanceRatingId)
		{
			return  ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("PERFORMANCERATING","FUNCTION_TITLE"),"PERFORMANCERATING","ID=" + performanceRatingId,false),"");
		}

		public string getPerformanceRatingEmploymentTitle(long performanceRatingId)
		{
			return  ch.psoft.Util.Validate.GetValid(_db.lookup(_db.langAttrName("PERFORMANCERATING","EMPLOYMENT_TITLE"),"PERFORMANCERATING","ID=" + performanceRatingId,false),"");
		}


		public string getRatingPageNameTitle(long employmentID)
		{
			long personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "EMPLOYMENT", "ID=" + employmentID.ToString(), false), -1);
			string personName = _db.Person.getWholeName(personID);
			return personName;
		}


		/// <summary>
		/// Determine the composition parts of an employment rating
		/// 
		/// Dies wird auf Grund aller zu berücksichtigenden Jobs bestimmt
		/// </summary>
		/// <param name="employmentID">employment database record id</param>
		/// <param name="compositionPart">rating composition part [enum ES_PERFORMANCE_CompositionType]</param>
		/// <returns>true if compositionPart is defined for employment</returns>
		public bool hasCompositionPart(long employmentID, int compositionPart)
		{
			bool isPart = false;

			long jobID = -1;
			double jobPart = 0;

			Hashtable jobPartsTable = OrganisationModule.jobParts(_db, employmentID);

			foreach (Object key in jobPartsTable.Keys)
			{
				jobID = Validate.GetValid(key.ToString(), (long)-1);
				jobPart = Validate.GetValid(jobPartsTable[key].ToString(), (double)0);

				if (jobID != -1 && jobPart > 0)
				{
					if (hasCompositionPartJob(jobID, compositionPart))
					{
						isPart = true;
						break;
					}
				}
			}

			return isPart;
		}


		/// <summary>
		/// Determine the composition parts of a Job for rating
		/// </summary>
		/// <param name="employmentID">job database record id</param>
		/// <param name="compositionPart">rating composition part [enum ES_PERFORMANCE_CompositionType]</param>
		/// <returns>true if compositionPart is defined for employment</returns>
		public bool hasCompositionPartJob(long jobID, int compositionPart)
		{
			bool isPart = false;

			long jobIDLookup = ch.psoft.Util.Validate.GetValid(_db.lookup("ID","JOB","ID="+jobID.ToString(),false),-1);
			long orgID = ch.psoft.Util.Validate.GetValid(_db.lookup("ORGENTITY_ID","JOB","ID="+jobIDLookup.ToString(),false),-1);
			bool inherit = (ch.psoft.Util.Validate.GetValid(_db.lookup("PERFORMANCERATING_INHERITANCE","JOB","ID="+jobIDLookup.ToString(),false),-1) != 0);

			string partColumn = "";

			switch (compositionPart)
			{
				case (int)ES_PERFORMANCE_CompositionType.PerformanceRating:
					partColumn = "PERFORMANCERATING_JOB_EXPECTATION";
					break;
				case (int)ES_PERFORMANCE_CompositionType.SkillsRating:
					partColumn = "PERFORMANCERATING_SKILLS";
					break;
				case (int)ES_PERFORMANCE_CompositionType.MboRating:
					partColumn = "PERFORMANCERATING_MBO";
					break;
				default:
					break;
			}

			if (inherit)
			{
				if (getOrgentityIDForRatingSettings(jobIDLookup) != -1)
				{
					int rating = ch.psoft.Util.Validate.GetValid(
						_db.lookup(
							partColumn,
							"ORGENTITY",
							"ID = " + getOrgentityIDForRatingSettings(jobIDLookup).ToString(),
							false
							),
							-1
						);

					isPart = (rating != 0);
				}
			}
			else
			{
				isPart = (ch.psoft.Util.Validate.GetValid(_db.lookup(partColumn,"JOB","ID="+jobIDLookup.ToString(),false),-1) != 0);
			}

			return isPart;
		}

		/// <summary>
		/// Liefert das massgebende Gewicht für einen Bewertungsteil
		/// 
		/// Der massgebende Job wird auf Grund der allgemeinen Regelung ausgewählt oder
		/// anteilmässig berücksichtigt.
		/// In letzterem Fall ist diese Rechnung nur sinnvoll, wenn die Settings der
		/// Jobs pro Anstellung zusammenpassen:
		///  - die gleichen Teile aktiviert
		///  - die Gewichtssummen gleich (100 oder 1)
		/// </summary>
		/// <param name="employmentID"></param>
		/// <param name="compositionPart">Bewertungsteil</param>
		/// <returns></returns>
		public double getRatingWeight(long employmentID, int compositionPart)
		{
			double returnValue = 0;

			long jobID = -1;
			double part = 0;
			double partSum = 0;
			double weight = 0;

			Hashtable jobPartsTable = OrganisationModule.jobParts(_db, employmentID);

			foreach (Object key in jobPartsTable.Keys)
			{
				jobID = Validate.GetValid(key.ToString(), (long)-1);
				part = Validate.GetValid(jobPartsTable[key].ToString(), (double)0);

				if (jobID != -1)
				{
					weight = getRatingWeightForJob(jobID, compositionPart);
					partSum += part;
					returnValue += weight * part;
				}
			}

			if (partSum > 0)
			{
				returnValue = returnValue / partSum;
			}

			return returnValue;
		}

		/// <summary>
		/// Liefert das massgebende Gewicht für einen Job und einen Bewertungsteil
		/// 
		/// Falls Bewertungsteil nicht berücksichtigt werden soll oder die Bestimmung nicht
		/// vollständig durchgeführt werden konnte, wird 0 zurückgegeben
		/// </summary>
		/// <param name="jobID"></param>
		/// <param name="compositionPart">Bewertungsteil</param>
		/// <returns></returns>
		public double getRatingWeightForJob(long jobID, int compositionPart)
		{
			double returnValue = 0;
			object[] values = _db.lookup(
					new string[]
						{
							"ID",
							"PERFORMANCERATING_INHERITANCE",
							"PERFORMANCERATING_JOB_EXPECTATION",
							"PERFORMANCERATING_JOB_EXPECTATION_WEIGHT",
							"PERFORMANCERATING_SKILLS",
							"PERFORMANCERATING_SKILLS_WEIGHT",
							"PERFORMANCERATING_MBO",
							"PERFORMANCERATING_MBO_WEIGHT"
						},
					"JOB",
					"ID = " + jobID.ToString()
				);
			
			if (!DBColumn.IsNull(values[0])) // Job gefunden
			{
				if (!DBColumn.IsNull(values[1]) && (int)values[1] == 1) // Inheritance: Vererbung verwenden
				{
					values = _db.lookup(
						new string[]
						{
							"ID",
							"PERFORMANCERATING_INHERITANCE",
							"PERFORMANCERATING_JOB_EXPECTATION",
							"PERFORMANCERATING_JOB_EXPECTATION_WEIGHT",
							"PERFORMANCERATING_SKILLS",
							"PERFORMANCERATING_SKILLS_WEIGHT",
							"PERFORMANCERATING_MBO",
							"PERFORMANCERATING_MBO_WEIGHT"
						},
						"ORGENTITY",
						"ID = " + getOrgentityIDForRatingSettings((long)values[0])
						);
				}
			}

			if (!DBColumn.IsNull(values[0])) // Gewichtungsangaben gefunden
			{
				int valueIndex = -1;

				switch(compositionPart)
				{
					case (int)ES_PERFORMANCE_CompositionType.PerformanceRating:
						valueIndex = 2;
						break;
					case (int)ES_PERFORMANCE_CompositionType.SkillsRating:
						valueIndex = 4;
						break;
					case (int)ES_PERFORMANCE_CompositionType.MboRating:
						valueIndex = 6;
						break;
					default:
						break;
				}

				if (valueIndex > -1
					&& !DBColumn.IsNull(values[valueIndex])
					&& (int)values[valueIndex] == 1
					&& !DBColumn.IsNull(values[valueIndex + 1])
					) // Bewertungsteil in Gesamtbewertung berücksichtigen
				{
					returnValue = (double)values[valueIndex + 1];
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Bestimmt die massgebende Orgentity für die Angaben für die Zusammensetzung
		/// in der Gesamtbewertung:
		/// erste Orgentity die Hierarchie hinauf, die nicht auf 'vererbt' gesetzt ist
		/// (nicht PERFORMANCERATING_INHERITANCE = 1)
		/// </summary>
		/// <param name="jobID"></param>
		/// <returns></returns>
		public long getOrgentityIDForRatingSettings(long jobID)
		{
			long orgentityID = ch.psoft.Util.Validate.GetValid(
					_db.lookup("ORGENTITY_ID",
						"JOB",
						"ID = " + jobID.ToString() + " and PERFORMANCERATING_INHERITANCE = 1",
						false
					),
					-1
				);
			long returnValue = -1;

			while (orgentityID != -1)
			{
				returnValue = orgentityID;
				orgentityID = ch.psoft.Util.Validate.GetValid(
						_db.lookup(
							"PARENT_ID",
							"ORGENTITY",
							"ID = " + orgentityID.ToString() + " and PERFORMANCERATING_INHERITANCE = 1",
							false
						),
						-1
					);
			}

			return returnValue;
		}

        private RateableJobIDsCacheEntry getRateableJobIDsCacheEntry(string baseKey, bool getJustFirst){
            RateableJobIDsCacheEntry cacheEntry = null;
            lock (_db.CacheSyncRoot){
                string key = baseKey + _db.userId;
                cacheEntry = (RateableJobIDsCacheEntry) _db.getCacheEntry(key);
                if (cacheEntry == null){
                    cacheEntry = new RateableJobIDsCacheEntry(getJustFirst);
                    _db.addCacheEntry(key, cacheEntry);
                }
            }
            return cacheEntry;
        }

        /// <summary>
        /// Liefert eine kommaseparierte Liste aller Job-IDs,
        /// für welche die eingeloggte Person Bewertungen durchführen kann
        /// </summary>
        /// <returns></returns>
        public string getRateableJobIDs(){
            return getRateableJobIDsCacheEntry("RateableJobIDs_", false).getRateableJobIDs(_db);
        }

        /// <summary>
        /// Liefert eine kommaseparierte Liste aller Employment-IDs,
        /// für welche die eingeloggte Person Bewertungen durchführen kann
        /// </summary>
        /// <returns></returns>
        public string getRateableEmploymentIDs(){
            return getRateableJobIDsCacheEntry("RateableJobIDs_", false).getRateableEmploymentIDs(_db);
        }

        /// <summary>
        /// true, wenn die eingeloggte Person Bewertungen auf mindestens einem Job durchführen kann.
        /// </summary>
        /// <returns></returns>
        public bool hasRateableJobs(){
            return getRateableJobIDsCacheEntry("HasRateableJobs_", true).getRateableJobIDs(_db).Length > 0;
        }

        public void refreshCacheEntriesAsynchronous(){
            getRateableJobIDsCacheEntry("HasRateableJobs_", true).refreshAsynchronous(DBData.getDBData(_db.session));
            getRateableJobIDsCacheEntry("RateableJobIDs_", false).refreshAsynchronous(DBData.getDBData(_db.session));
        }

        /// <summary>
        /// returns the person id associated with the given employment, "null" if no
        /// association can been found.
        /// </summary>
        /// <param name="employmentID">the employment id used to finding out the person id</param>
        /// <returns>the person id associated with the given employment id</returns>
        private String personId(long employmentID)
        {
            //find out person id (new database definition but interface unchanged)
            long personIDL = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "EMPLOYMENT", "ID = " + employmentID, false), -1L);
            string personID = personIDL != -1 ? personIDL.ToString() : "null";
            return personID;
        }
    }
}