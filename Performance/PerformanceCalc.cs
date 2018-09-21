using ch.appl.psoft.db;
using System;
using System.Data;

namespace ch.appl.psoft.Performance
{
    public class PerformanceCalc
    {
        // calculations for performance reports (triangle and boxes) / 20.04.10 / mkr
        private string ratingId = "";
        private string jobId = "";
        private string personId = "";
        private DBData db = null;

        private int job_expectation = 0;
        private double job_expectation_weight = 0;

        private int skills = 0;
        private int skills_notrated = 0;
        private double skills_weight = 0;

        private int minRating = 0;
        private int maxRating = 0;
        private double avgRating = 0;
        private double skillsFactor = 0;
        private bool skillsAvailable = false;

        private int mbo = 0;
        private int mbo_notrated = 0;
        private double mbo_weight = 0;

        private double mboAvgRating = 0;
        private double mboFactor = 0;
        private bool mboAvailable = false;

        private int year = 0;

        private string itemTitleCol;

        private double avgRelativWeight = 0;

        private int numOfCrit = 0;

        private double sum = 0;

        private string critTitleCol = "";
        private DataTable critTable = null;

        private DataRow calcRow = null;

        public PerformanceCalc(DBData DB, string ratingID, bool includeSkillsMbO)
        {
            this.db = DB;
            this.ratingId = ratingID;

            // get settings from db
            this.jobId = db.lookup("JOB_ID", "PERFORMANCERATING", "ID = " + ratingId).ToString();
            this.personId = db.lookup("PERSON_ID", "PERFORMANCERATING", "ID = " + ratingId).ToString();

            //no job available, show only performance rating
            if (jobId == "")
            {
                job_expectation = 1;
                job_expectation_weight = 100;
            }

            if (jobId != "" && db.lookup("PERFORMANCERATING_INHERITANCE", "JOB", "ID = " + jobId).ToString() == "0")
            {
                //set by job
                job_expectation = (int)db.lookup("PERFORMANCERATING_JOB_EXPECTATION", "JOB", "ID = " + jobId);
                job_expectation_weight = (double)db.lookup("PERFORMANCERATING_JOB_EXPECTATION_WEIGHT", "JOB", "ID = " + jobId);
                skills = (int)db.lookup("PERFORMANCERATING_SKILLS", "JOB", "ID = " + jobId);
                skills_weight = (double)db.lookup("PERFORMANCERATING_SKILLS_WEIGHT", "JOB", "ID = " + jobId);
                mbo = (int)db.lookup("PERFORMANCERATING_MBO", "JOB", "ID = " + jobId);
                mbo_weight = (double)db.lookup("PERFORMANCERATING_MBO_WEIGHT", "JOB", "ID = " + jobId);
            }
            else if (jobId != "")
            {
                //iterate through OEs
                long oeId = (long)db.lookup("ORGENTITY_ID", "JOB", "ID = " + jobId);
                System.Collections.ArrayList oeIds = db.Tree("ORGENTITY").GetPath(oeId, true);
                oeIds.Reverse();

                foreach (long aktOe in oeIds)
                {
                    if (db.lookup("PERFORMANCERATING_INHERITANCE", "ORGENTITY", "ID = " + aktOe).ToString() == "0")
                    {
                        //set by this OE
                        job_expectation = (int)db.lookup("PERFORMANCERATING_JOB_EXPECTATION", "ORGENTITY", "ID = " + aktOe);
                        job_expectation_weight = (double)db.lookup("PERFORMANCERATING_JOB_EXPECTATION_WEIGHT", "ORGENTITY", "ID = " + aktOe);
                        skills = (int)db.lookup("PERFORMANCERATING_SKILLS", "ORGENTITY", "ID = " + aktOe);
                        skills_weight = (double)db.lookup("PERFORMANCERATING_SKILLS_WEIGHT", "ORGENTITY", "ID = " + aktOe);
                        mbo = (int)db.lookup("PERFORMANCERATING_MBO", "ORGENTITY", "ID = " + aktOe);
                        mbo_weight = (double)db.lookup("PERFORMANCERATING_MBO_WEIGHT", "ORGENTITY", "ID = " + aktOe);

                        break;
                    }
                }
            }

            //get year of rating
            year = (int)db.lookup("YEAR(RATING_DATE) AS JAHR", "PERFORMANCERATING", "ID = " + ratingId);

            //get latest skill-rating if needed
            if (skills == 1)
            {
                int appraisalId = Convert.ToInt32(db.lookup("TOP 1 ID", "SKILLS_APPRAISAL", "PERSON_ID = " + personId + " AND YEAR(APPRAISALDATE) = " + year + " ORDER BY APPRAISALDATE DESC"));

                if (appraisalId > 0)
                {
                    skillsAvailable = true;

                    minRating = Convert.ToInt32(db.lookup("MIN(PERCENTAGE)", "RATING_LEVEL", ""));
                    maxRating = Convert.ToInt32(db.lookup("MAX(PERCENTAGE)", "RATING_LEVEL", ""));
                    avgRating = Convert.ToDouble(db.lookup("AVG(RATING_LEVEL_PERCENTAGE)", "SKILL_RATING", "SKILLS_APPRAISAL_ID = " + appraisalId));

                    skillsFactor = (maxRating - minRating) / 100;
                }
                else
                {
                    // not rated, disable
                    skills_notrated = 1;
                    skills = 0;
                }
            }

            //get latest MbO-rating if needed
            if (mbo == 1)
            {
                mboFactor = 2;

                mboAvgRating = Convert.ToInt32(db.lookup("TOP 1 Bewertung", "Objectiv_Rating", "Person_ID = " + personId + " AND YEAR(Bewertungsdatum) IN (" + year + ", " + (year - 1) + ") ORDER BY Bewertungsdatum DESC"));

                if (mboAvgRating > 0)
                {
                    mboAvailable = true;
                }
                else
                {
                    // not rated, disable
                    mbo_notrated = 1;
                    mbo = 0;
                }
            }

            // adjust weight if not all criterias are rated
            if (skills_notrated == 1 && mbo_notrated == 1)
            {
                job_expectation_weight = 100;
            }
            if (skills_notrated == 1 && mbo_notrated == 0)
            {
                double total = job_expectation_weight + mbo_weight;
                job_expectation_weight += (job_expectation_weight / total) * skills_weight;
                mbo_weight += (mbo_weight / total) * skills_weight;
            }
            if (skills_notrated == 0 && mbo_notrated == 1)
            {
                double total = job_expectation_weight + skills_weight;
                job_expectation_weight += (job_expectation_weight / total) * mbo_weight;
                skills_weight += (mbo_weight / total) * mbo_weight;
            }

            itemTitleCol = db.langAttrName("PERFORMANCERATING_ITEMS", "CRITERIA_TITLE");

            avgRelativWeight = db.lookup("avg(relativ_weight)", "performance_level", "", avgRelativWeight);

            string sql = "select " + itemTitleCol + ",avg(relativ_weight) PERFORMANCE, avg(criteria_weight) CRITERIA from performancerating_items where relativ_weight >= 0 and criteria_weight > 0 and performancerating_ref = " + ratingId + " group by criteria_ref," + itemTitleCol;

            DataTable table = db.getDataTable(sql);
            numOfCrit = table.Rows.Count;

            foreach (DataRow row in table.Rows)
            {
                sum += (double)row["CRITERIA"] * job_expectation_weight / 100;
            }

            if (skills == 1 && includeSkillsMbO)
            {
                sum += skills_weight;
            }
            if (mbo == 1 && includeSkillsMbO)
            {
                sum += mbo_weight;
            }

            critTitleCol = db.langAttrName("PERFORMANCE_CRITERIA", "TITLE");
            critTable = db.getDataTable("select ID, " + critTitleCol + " from PERFORMANCE_CRITERIA");

            if (skills == 1 && includeSkillsMbO)
            {
                critTable.Rows.Add(new object[] { "1234", "Skills" });
            }
            if (mbo == 1 && includeSkillsMbO)
            {
                critTable.Rows.Add(new object[] { "1235", "MbO" });
            }


        }

        public double getSum()
        {
            return sum;
        }

        public double getAvgRelativWeight()
        {
            return avgRelativWeight;
        }

        public DataTable getCritTable()
        {
            return critTable;
        }

        public DataRow getCalcRow(DataRow critRow)
        {
            string criteriaTitle = critRow[1].ToString();
            string sql = "select min(" + itemTitleCol + ") " + itemTitleCol + ", avg(relativ_weight) PERFORMANCE, avg(criteria_weight) CRITERIA from performancerating_items where relativ_weight >= 0 and criteria_weight > 0 and performancerating_ref = " + ratingId + " and (criteria_ref=" + critRow[0] + " or " + itemTitleCol + "='" + DBColumn.toSql(criteriaTitle) + "')";

            DataTable tempTable = db.getDataTable(sql);

            if (critRow[0].ToString() == "1234")
            {
                tempTable.Rows.Add(new object[] { "Skills", avgRating, "100" });
                tempTable.Rows.RemoveAt(0);
            }

            if (critRow[0].ToString() == "1235")
            {
                tempTable.Rows.Add(new object[] { "MbO", mboAvgRating, "100" });
                tempTable.Rows.RemoveAt(0);
            }

            if (tempTable.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return tempTable.Rows[0];
            }
        }

        public double getSkillsWeight()
        {
            return skills_weight;
        }

        public double getMboWeight()
        {
            return mbo_weight;
        }

        public double getJobExpectationWeight()
        {
            return job_expectation_weight;
        }

        public string getItemTitleCol()
        {
            return itemTitleCol;
        }

        public bool getSkillsAvailable()
        {
            return skillsAvailable;
        }

        public bool getMboAvailable()
        {
            return mboAvailable;
        }

        public int getMinRating()
        {
            return minRating;
        }

        public double getSkillsFactor()
        {
            return skillsFactor;
        }

        public double getMboFactor()
        {
            return mboFactor;
        }
    }
}
