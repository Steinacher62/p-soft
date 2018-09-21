using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for PerformanceChangeReport.
    /// </summary>
    public class PerformanceChangeReport : PsoftPDFReport
    {
        protected DataTable _competenceLevels = null;
        protected long _personID = -1;
        protected string _period = "";
        protected string _periodII = "";
        protected string _orgentityIDs = "";

        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public PerformanceChangeReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendOETable()
        {
            return AppendTable("420,100");
        }

        public XmlNode AppendPerformanceTable()
        {
            return AppendTable("320,130,*");
        }

        public XmlNode AppendAverageProportionTable()
        {
            return AppendTable("150,300,*");
        }

        public void createReport(int personID, string period, string periodII, string orgentityIDs)
        {
            _personID = personID;
            _period = period;
            _periodII = periodII;
            _orgentityIDs = orgentityIDs;

            int p = Convert.ToInt16(_period);
            int pII = Convert.ToInt16(_periodII);
            int d = pII - p;
            bool lowerFirst = true;
            if (d < 0) 
            {
                d = d * -1;
                lowerFirst = false;
            }
            string[] periods = new string[d+1];
            periods[0] = p.ToString();           
            for (int i=1; i<=d; i++)
            {
                int pp = ((lowerFirst)?(p+i):(p-i));
                periods[i] = pp.ToString();
            }

            writeHeaderAndFooter(_map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_PERFORMANCECHANGE), DateTime.Now.ToString("d"));

            appendVSpace(10);
            string subTitle = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_RATINGPERIODS);
            subTitle = subTitle.Replace("%1%",_period);
            subTitle = subTitle.Replace("%2%",_periodII);
            _nTable = AppendOETable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("subTitle"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("colspan", "2"));
            _nCell.InnerText = subTitle;
            appendVSpace(10);

            // tableheader
            _nTable = AppendPerformanceTable();
            _nTable.Attributes.Append(createClassAttribute("detailValue"));
            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_PERFORMANCECRITERIA);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_YEAR);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_PERFORMANCE_PROPORTION);

            appendVSpace(10);
            appendHLine(520);
            appendVSpace(10);

            // build table foreach OE person is leader of 
            // list categories per OEtable
            // list periods per category
            double prSummAll = 0.0;
            int prCntAll = 0;
            string sql = "select * from PERFORMANCE_CRITERIA";
            DataTable criteriaTable = _db.getDataTable(sql,"PERFORMANCE_CRITERIA");
            // if no oe's selected
            string oeIDs = _orgentityIDs;
            if (oeIDs == "")
            {
                oeIDs = _db.Orgentity.addAllSubOEIDs(_db.Person.getLeadingOEIDs(_personID));
            }
            string[] oeIDsL = oeIDs.Split(',');
            System.Collections.IEnumerator myEnumerator = oeIDsL.GetEnumerator();
            while ( myEnumerator.MoveNext() )
            {
                string oeID = myEnumerator.Current.ToString();
                string title = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_AVERAGEPERFORMANCES) + " " + ch.psoft.Util.Validate.GetValid((_db.lookup(_db.langAttrName("ORGENTITY", "TITLE"),"ORGENTITY","ID="+oeID)).ToString(),"");
                appendVSpace(20);
                _nTable = AppendOETable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("title"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText = title;
                appendVSpace(10);

                double prSumm = 0.0;
                int prCnt = 0;
                foreach (DataRow row in criteriaTable.Rows)
                {  
               //loop periods
                    int cnt = 0;
                    for (int i=0; i<periods.Length; i++)
                    {                        
                        double pr = performanceProportion(periods[i].ToString(), ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), "-1"), oeID);
                        if (!pr.Equals(Double.NaN))
                        {
                            cnt++;
                            _nTable = AppendPerformanceTable();
                            _nTable.Attributes.Append(createClassAttribute("detailValue"));
                            _nRow = _nTable.AppendChild(createRow());
                            _nCell = _nRow.AppendChild(createCell());
                            if (cnt == 1)
                            {
                                _nCell.InnerText = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                            }
                            _nCell = _nRow.AppendChild(createCell());
                            _nCell.InnerText = periods[i].ToString();
                            _nCell = _nRow.AppendChild(createCell());
                            _nCell.InnerText = format(PerformanceModule.recalcToBase(pr)) + " %";

                            prSumm += pr;
                            prCnt++;
                        }
                    }
                    if (cnt > 0)
                    {
                        appendVSpace(10);
                    }
                    
                }
                if (prCnt > 0)
                {
                    prSummAll += prSumm;
                    prCntAll += prCnt;
                    double prAvg = prSumm/prCnt;
                    _nTable = AppendAverageProportionTable();
                    _nTable.Attributes.Append(createClassAttribute("detailValue"));
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createAttribute("align", "right"));
                    _nCell.Attributes.Append(createAttribute("padding-right", "20"));
                    _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_TOTAL);
                    _nCell = _nRow.AppendChild(createCell());
                    if (!prAvg.Equals(Double.NaN))
                    {
                        _nCell.InnerText = format(PerformanceModule.recalcToBase(prAvg)) + " %";
                    }
                }

            }
            if (prCntAll > 0)
            {
                appendVSpace(20);
                double prAvgAll = prSummAll/prCntAll;
                _nTable = AppendAverageProportionTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("sectionHeader"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("align", "right"));
                _nCell.Attributes.Append(createAttribute("padding-right", "20"));
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_TOTAL);
                _nCell = _nRow.AppendChild(createCell());
                if (!prAvgAll.Equals(Double.NaN))
                {
                    _nCell.InnerText = format(PerformanceModule.recalcToBase(prAvgAll)) + " %";
                }
            }
        }

        private double performanceProportion(string period, string critID, string oeID) 
        {
			double _prRating = 0.0;
			double sum = 0.0;
			double sumCriteriaWeight = 0.0;

			// für Gruppierung
			long employmentIdSave = -1;
			long employmentId = -1;

			string sql = "select pr.EMPLOYMENT_REF,"
				+ " pr.rating_date,"
				+ " pr.id,"
				+ " avg(pri.relativ_weight) RELATIV_WEIGHT,"
				+ " avg(pri.criteria_weight) CRITERIA_WEIGHT"
				+ " from performancerating pr, performancerating_items pri"
				+ " where pr.id = pri.performancerating_ref"
				+ " and pri.relativ_weight >= 0 and pri.criteria_weight > 0"
				+ " and pri.criteria_ref_persisting = " + critID // nur ein Kriterium
				+ " and pr.rating_date like '%" + period + "%'"
				+ " and pr.IS_SELFRATING = 0"
				+ " and pr.EMPLOYMENT_REF in ("
				+ "  select distinct EMPLOYMENT_ID from JOB where ORGENTITY_ID = " + oeID
				+ ") and pr.EMPLOYMENT_REF in ("
				+ _db.Performance.getRateableEmploymentIDs()
				+ ")"
				+ " group by pr.EMPLOYMENT_REF, pr.rating_date, pr.id"
				+ " order by pr.EMPLOYMENT_REF, pr.rating_date desc";
            DataTable table = _db.getDataTable(sql);

            foreach (DataRow row in table.Rows) 
            {
				employmentId = DBColumn.GetValid(row["EMPLOYMENT_REF"], (long)0);

				if (employmentId != employmentIdSave) // pro Employment höchstens ein Rating letztes der Periode
				{
					employmentIdSave = employmentId;
					double pw = DBColumn.GetValid(row["RELATIV_WEIGHT"], 0.0);
					double cw = DBColumn.GetValid(row["CRITERIA_WEIGHT"], 0.0);
					sumCriteriaWeight += cw;
					double r = pw * cw;
					sum += r;
				}
            }

			if (sumCriteriaWeight > 0)
			{
				_prRating = sum / sumCriteriaWeight;
			}

            return _prRating;
        }

        private string format(double val)
        {
            val = Math.Round(val,2);           
            return val.ToString("F");
        }
    }
}
