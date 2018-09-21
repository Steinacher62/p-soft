using ch.psoft.Util;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for AveragePerformanceReport.
    /// </summary>
    public class AveragePerformanceReport : PReport
    {
        protected DataTable _competenceLevels = null;
        protected long _employmentID = -1;
        protected long _personID = -1;
        protected long _performanceRatingID = -1;
        protected string _pyramidName = "";

        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public AveragePerformanceReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendPerformanceTable()
        {
            return AppendTable("520");
        }

        public XmlNode AppendMemoTable()
        {
            return AppendTable("1500");
        }

        private XmlNode AppendSignTable()
        {
            XmlNode nTable = createTable();
            if (Global.isModuleEnabled("ahb"))
            {
                nTable.Attributes.Append(createAttribute("widths", "100,80,100,200,100"));
            }
            else
            {
                nTable.Attributes.Append(createAttribute("widths", "110,80,110,110"));
            }
           
           
            nTable.Attributes.Append(createAttribute("keep-together", "false"));
            nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
            nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
            nTable.Attributes.Append(createAttribute("padding-all", "2"));
            nTable.Attributes.Append(createAttribute("align", "left"));
            nTable.Attributes.Append(createClassAttribute("detailValue"));

            return _rootNode.AppendChild(nTable);
        }

        private void createPyramid()
        {
            PRPyramidImage image = new PRPyramidImage(_db,_map,1400,1000);
            image.leftSpace = 28;
            FileInfo file = new FileInfo(_pyramidName);
            Stream stream = file.OpenWrite();

            image.resolution = 192;

            try 
            {
                Bitmap map = image.draw(_performanceRatingID);

                map.Save(stream, ImageFormat.Jpeg);
                map.Dispose();

                addImageToCatalog(_pyramidName, "pyramid");
            }
            catch (Exception ex) 
            {
                Logger.Log(ex,Logger.ERROR);
            }
            stream.Close();
        }

        public override void createReport(object[] parameter)
        {
			_employmentID = int.Parse(parameter[0].ToString());//employmentID;
			_performanceRatingID = int.Parse(parameter[1].ToString());//performanceRatingID;
            _pyramidName = parameter[2].ToString();//pyramidName;

            createPyramid();

            _personID = ch.psoft.Util.Validate.GetValid((_db.lookup("PERSON_ID","EMPLOYMENT","ID="+_employmentID)).ToString(),-1L);
            string type = "leaderR";
            if (ch.psoft.Util.Validate.GetValid((_db.lookup("IS_SELFRATING","PERFORMANCERATING","ID="+_performanceRatingID)).ToString(),(int)-1) > 0)
            {
                type = "selfR";
            }
            if (type.Equals("selfR"))
            {
                writeHeaderAndFooter(_map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_AVERAGEPERFORMANCESELF), DateTime.Now.ToString("d"));
            }
            else {
                writeHeaderAndFooter(_map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_AVERAGEPERFORMANCE), DateTime.Now.ToString("d"));
            }
            // person detail
            _nTable = AppendDetailTable();
            DataTable table = _db.getDataTableExt("select ID, FIRSTNAME, PNAME, PERSONNELNUMBER, DATEOFBIRTH from PERSON where ID=" + _personID, "PERSON");
            addDetail(_db, _nTable, table, "MA", true);
            
            // employment
            _nTable = AppendDetailTable();
            table = _db.getDataTableExt("select " + _db.langAttrName("PERFORMANCERATING", "JOB_TITLE") + 
                                        " from PERFORMANCERATING where ID=" + _performanceRatingID, "PERFORMANCERATING");

            addDetail(_db, _nTable, table, "", true);

            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-top", "10"));
            _nCell = _nRow.AppendChild(createCell());

            appendHLine(520);
            appendVSpace(10);

            // rating
            _nTable = AppendPerformanceTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("title"));
            _nCell = _nRow.AppendChild(createCell());
            appendVSpace(10);
            if (type.Equals("selfR"))
            {
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_CLERK_RATING);
            }
            else if (type.Equals("leaderR"))
            {
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_SUPERVISOR_RATING);
            }

            appendVSpace(10);
            _nTable = AppendDetailTable();
            table = _db.getDataTableExt("select RATING_DATE from PERFORMANCERATING where ID=" + _performanceRatingID, "PERFORMANCERATING");
            addDetail(_db, _nTable, table, "", true);
            appendVSpace(10);
            
            // pyramid
            _nTable = AppendPerformanceTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.AppendChild(createShowImage("pyramid"));

            appendVSpace(10);

            // Leistungsgrad
            //double leistungsgrad = _db.lookup("Leistungsgrad", "Rep_LeistungsDreieck", "employmentID = " + _employmentID + " AND performanceRatingID = " + _performanceRatingID, 0.0);
            double leistungsgrad = _db.lookup("Leistungsgrad", "Rep_LeistungsDreieck", "performanceRatingID = " + _performanceRatingID, 0.0);
            double multiplikator = double.Parse(Global.Config.getModuleParam("performance", "performanceRatingBase", "100")) / 100;

            //leistungsgrad = leistungsgrad * multiplikator;

            // Leistungsgrad auch für MbO und Skills berechnen
            //get settings of job expectation, skills and mbo
            string jobId = _db.lookup("JOB_ID", "PERFORMANCERATING", "ID = " + _performanceRatingID).ToString();
            string personId = _db.lookup("PERSON_ID", "PERFORMANCERATING", "ID = " + _performanceRatingID).ToString();

            int job_expectation = 0;
            double job_expectation_weight = 0;

            int skills = 0;
            int skills_notrated = 0;
            double skills_weight = 0;

            int minRating = 0;
            int maxRating = 0;
            double avgRating = 0;
            double skillsFactor = 0;
            bool skillsAvailable = false;

            int mbo = 0;
            int mbo_notrated = 0;
            double mbo_weight = 0;

            int mboMinRating = 0;
            int mboMaxRating = 200;
            double mboAvgRating = 0;
            double mboFactor = 0;
            bool mboAvailable = false;
            //int maxRating = Convert.ToInt32(Global.Config.getModuleParam("performance", "performanceRatingBase", "100"));

            //no job available, show only performance rating
            if (jobId == "")
            {
                job_expectation = 1;
                job_expectation_weight = 100;
            }

            if (jobId != "" && _db.lookup("PERFORMANCERATING_INHERITANCE", "JOB", "ID = " + jobId).ToString() == "0")
            {
                //set by job
                job_expectation = (int)_db.lookup("PERFORMANCERATING_JOB_EXPECTATION", "JOB", "ID = " + jobId);
                job_expectation_weight = (double)_db.lookup("PERFORMANCERATING_JOB_EXPECTATION_WEIGHT", "JOB", "ID = " + jobId);
                skills = (int)_db.lookup("PERFORMANCERATING_SKILLS", "JOB", "ID = " + jobId);
                skills_weight = (double)_db.lookup("PERFORMANCERATING_SKILLS_WEIGHT", "JOB", "ID = " + jobId);
                mbo = (int)_db.lookup("PERFORMANCERATING_MBO", "JOB", "ID = " + jobId);
                mbo_weight = (double)_db.lookup("PERFORMANCERATING_MBO_WEIGHT", "JOB", "ID = " + jobId);
            }
            else if (jobId != "")
            {
                //iterate through OEs
                long oeId = (long)_db.lookup("ORGENTITY_ID", "JOB", "ID = " + jobId);
                System.Collections.ArrayList oeIds = _db.Tree("ORGENTITY").GetPath(oeId, true);
                oeIds.Reverse();

                foreach (long aktOe in oeIds)
                {
                    if (_db.lookup("PERFORMANCERATING_INHERITANCE", "ORGENTITY", "ID = " + aktOe).ToString() == "0")
                    {
                        //set by this OE
                        job_expectation = (int)_db.lookup("PERFORMANCERATING_JOB_EXPECTATION", "ORGENTITY", "ID = " + aktOe);
                        job_expectation_weight = (double)_db.lookup("PERFORMANCERATING_JOB_EXPECTATION_WEIGHT", "ORGENTITY", "ID = " + aktOe);
                        skills = (int)_db.lookup("PERFORMANCERATING_SKILLS", "ORGENTITY", "ID = " + aktOe);
                        skills_weight = (double)_db.lookup("PERFORMANCERATING_SKILLS_WEIGHT", "ORGENTITY", "ID = " + aktOe);
                        mbo = (int)_db.lookup("PERFORMANCERATING_MBO", "ORGENTITY", "ID = " + aktOe);
                        mbo_weight = (double)_db.lookup("PERFORMANCERATING_MBO_WEIGHT", "ORGENTITY", "ID = " + aktOe);

                        break;
                    }
                }
            }

            //get year of rating
            int year = (int)_db.lookup("YEAR(RATING_DATE) AS JAHR", "PERFORMANCERATING", "ID = " + _performanceRatingID);

            //get latest skill-rating if needed
            if (skills == 1)
            {
                int appraisalId = Convert.ToInt32(_db.lookup("TOP 1 ID", "SKILLS_APPRAISAL", "PERSON_ID = " + personId + " AND YEAR(APPRAISALDATE) = " + year + " ORDER BY APPRAISALDATE DESC"));

                if (appraisalId > 0)
                {
                    skillsAvailable = true;

                    minRating = Convert.ToInt32(_db.lookup("MIN(PERCENTAGE)", "RATING_LEVEL", ""));
                    maxRating = Convert.ToInt32(_db.lookup("MAX(PERCENTAGE)", "RATING_LEVEL", ""));
                    avgRating = Convert.ToDouble(_db.lookup("AVG(RATING_LEVEL_PERCENTAGE)", "SKILL_RATING", "SKILLS_APPRAISAL_ID = " + appraisalId));

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

                mboAvgRating = Convert.ToInt32(_db.lookup("TOP 1 Bewertung", "Objectiv_Rating", "Person_ID = " + personId + " AND YEAR(Bewertungsdatum) IN (" + year + ", " + (year - 1) + ") ORDER BY Bewertungsdatum DESC"));

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

            leistungsgrad = leistungsgrad / 100 * job_expectation_weight;

            if (skills == 1)
            {
                if (skillsAvailable)
                {
                    leistungsgrad += (avgRating - minRating) / 100 * skills_weight;
                }
                else
                {
                    leistungsgrad += 50 / 100 * skills_weight;
                }
            }

            if (mbo == 1)
            {
                if (mboAvailable)
                {
                    leistungsgrad += (mboAvgRating / 2) / 100 * mbo_weight;
                }
                else
                {
                    leistungsgrad += 50 / 100 * mbo_weight;
                }
            }

            leistungsgrad = leistungsgrad * multiplikator;

            // Ende Leistungsgrad

            _nTable = AppendDetailTable();
            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createClassAttribute("detailName"));
            if (PerformanceModule.showPyramidFooter)
            {  
                _nCell.InnerText = "Leistungsgrad";
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createClassAttribute("detailValue"));
                _nCell.InnerText = Math.Round(leistungsgrad, 0) + "%";
            }
            if (skills_notrated == 1)
            {
                _nCell.InnerText += " Skills nicht bewertet";
            }
            if (mbo_notrated == 1)
            {
                _nCell.InnerText += " MbO nicht bewertet";
            }

            appendVSpace(10);

            

            // signbox

            if (!Global.isModuleEnabled("ahb"))
            {
                _nTable = AppendSignTable();
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_DATE);
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_SIGN);
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_SUPERVISOR);
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                if (PerformanceModule.ShowSign2Up)
                {
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                    _nCell.InnerText = "2UP";
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                }
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_CLERK);
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            }


            if (Global.isModuleEnabled("ahb"))
            {
                _nTable = AppendSignTable();
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_DATE);
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = "Name";
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "center"));
                _nCell.InnerText = "Visum";
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createAttribute("padding-bottom", "15"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_SUPERVISOR);
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createAttribute("padding-bottom", "15"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_CLERK);
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            }
            appendVSpace(20);
           
            _rootNode.AppendChild(createElement("new-page"));

            DataTable tableCriteria = _db.getDataTableExt("select distinct CRITERIA_REF, " + _db.langAttrName("PERFORMANCERATING_ITEMS", "CRITERIA_TITLE") + " from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF="+_performanceRatingID, "PERFORMANCERATING_ITEMS");            

            foreach (DataRow rowCriteria in tableCriteria.Rows) 
            {
                long criteriaID = ch.psoft.Util.Validate.GetValid(rowCriteria["CRITERIA_REF"].ToString(), -1L);
                    //conditions removed (SEEKIII-275)
                    DataTable tableItems = _db.getDataTableExt("select ID from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF="+_performanceRatingID+" and CRITERIA_REF="+criteriaID, "PERFORMANCERATING_ITEMS");
                    if (tableItems.Rows.Count > 0) {
                        appendVSpace(20);
                        _nTable = AppendPerformanceTable();
                        _nRow = _nTable.AppendChild(createRow());
                        _nRow.Attributes.Append(createClassAttribute("title"));
                        _nCell = _nRow.AppendChild(createCell());
                        _nCell.InnerText = ch.psoft.Util.Validate.GetValid(rowCriteria[_db.langAttrName(rowCriteria.Table.TableName, "CRITERIA_TITLE")].ToString(), "");

                        foreach (DataRow itemsRow in tableItems.Rows) {                 

                            String itemsID = ch.psoft.Util.Validate.GetValid(itemsRow[_db.langAttrName(itemsRow.Table.TableName, "ID")].ToString(), "");
                            DataTable detailTable = _db.getDataTableExt("select " + _db.langAttrName("PERFORMANCERATING_ITEMS", "EXPECTATION_DESCRIPTION") + " from PERFORMANCERATING_ITEMS where ID="+itemsID, "PERFORMANCERATING_ITEMS");  

                            appendVSpace(10);
                            _nTable = AppendDetailTable();
                            addDetail(_db, _nTable, detailTable, "", false);

                            //add Bewertung (SEEKIII-275)
                            DataTable bewArgs = _db.getDataTableExt("select " + _db.langAttrName("PERFORMANCERATING_ITEMS", "LEVEL_TITLE") + " from PERFORMANCERATING_ITEMS where ID=" + itemsID, "PERFORMANCERATING_ITEMS");  
                            appendVSpace(2);
                            _nTable = AppendDetailTable();
                            addDetail(_db, _nTable, bewArgs, "", false);

							//add motivation
							DataTable tableArgs = _db.getDataTableExt("select * from PERFORMANCERATING_ARGUMENTS where PERFORMANCERATING_REF="+_performanceRatingID+" and PERFORMANCERATING_CRITERIA_REF="+criteriaID + " and PERFORMANCERATING_ITEM_ID="+itemsID, "PERFORMANCERATING_ARGUMENTS");            
							appendVSpace(2);
							_nTable = AppendDetailTable();
							addDetail(_db, _nTable, tableArgs, "", false);



                    
                        }
                    }
            }

            appendVSpace(30);

            if (Global.isModuleEnabled("frauenfeld"))
            {
                _nTable = AppendMemoTable();
                _nTable.Attributes.Append(createAttribute("font-size", 8.ToString()));
                _nRow = _nTable.AppendChild(createRow());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.InnerText += " Eine Lohnsenkung wird in der Regel erst vorgenommen, wenn nach Ablauf eines Jahres die zweite Leistungsbewertung immer noch tiefer ist als der Wert vor zwei Jahren. Für die Lohnsenkung ist der Wert der zweiten Leistungsbeurteilung massgebend. Allfällige besondere Umstände sind zu berücksichtigen. (Art. 17 Abs. 2 und 3 Besoldungsreglement).";
            }
        }
     }


}
