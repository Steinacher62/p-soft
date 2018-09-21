using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for AverageOEPerformanceReport.
    /// </summary>
    public class AverageOEPerformanceReport : PsoftPDFReport
    {
        protected DataTable _competenceLevels = null;
        protected long _personID = -1;
        protected string _period = "";
        protected string _orgentityIDs = "";

        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public AverageOEPerformanceReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

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

        public void createReport(int personID, string period, string orgentityIDs)
        {
            _personID = personID;
            _period = period;
            _orgentityIDs = orgentityIDs;

            writeHeaderAndFooter(_map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_AVERAGEEOPERFORMANCE), DateTime.Now.ToString("d"));

            appendVSpace(10);

            // tableheader
            _nTable = AppendPerformanceTable();
            _nTable.Attributes.Append(createClassAttribute("detailValue"));
            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_EMPLOYMENT_PERSON);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_DATE);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_PERFORMANCE_PROPORTION);

            appendVSpace(10);
            appendHLine(520);
            appendVSpace(10);

            // build table foreach OE person is leader of 
            // list employees per OEtable
            double prSummAll = 0.0;
            int prCntAll = 0;
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

                string sql = "select ID,EMPLOYMENT_REF,RATING_DATE from PERFORMANCERATING where RATING_DATE like '%" + _period + "%' and IS_SELFRATING = 0 and EMPLOYMENT_REF in ( select distinct EMPLOYMENT_ID from JOB where ORGENTITY_ID=" + oeID + " ) and EMPLOYMENT_REF in(" + _db.Performance.getRateableEmploymentIDs() + ")";
                DataTable performanceTable = _db.getDataTable(sql);

                DataTable employmentTable = _db.getDataTable("select e.ID as ID, p.ID as pID, e." + _db.langAttrName("EMPLOYMENT", "TITLE") + " + ', ' + '(' + RTRIM(isnull(p.MNEMO,'')) + ') ' + p.PNAME + ' ' + isnull(p.FIRSTNAME,'') as PERS_EMP from PERSON p, EMPLOYMENT e where p.ID = e.PERSON_ID and p.ID in ( select distinct ID from PERSONOEV where OE_ID in (" + oeIDs + ")) and e.ID in(" + _db.Performance.getRateableEmploymentIDs() + ")");

                double prSumm = 0.0;
                int prCnt = 0;
                foreach (DataRow row in performanceTable.Rows)
                {  
               
                    string employmentRef = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "EMPLOYMENT_REF")].ToString(), "");
                    string employment = employmentRef;

                    foreach (DataRow eRow in employmentTable.Rows)
                    {
                        string tmp = ch.psoft.Util.Validate.GetValid(eRow[_db.langAttrName(eRow.Table.TableName, "ID")].ToString(), "");
                        if (ch.psoft.Util.Validate.GetValid(eRow[_db.langAttrName(eRow.Table.TableName, "ID")].ToString(), "") == employmentRef)
                        {
                            employment = ch.psoft.Util.Validate.GetValid(eRow[_db.langAttrName(eRow.Table.TableName, "PERS_EMP")].ToString(), "");
                        }
                    }
                    _nTable = AppendPerformanceTable();
                    _nTable.Attributes.Append(createClassAttribute("detailValue"));
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.InnerText = employment;
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.InnerText = DBColumn.GetValid(row[_db.langAttrName(row.Table.TableName, "RATING_DATE")], DateTime.MinValue).ToShortDateString();
                    _nCell = _nRow.AppendChild(createCell());
                    double pr = performanceProportion(ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "ID")].ToString(), "-1"));
                    if (!pr.Equals(Double.NaN))
                    {
                        prSumm += pr;
                        prCnt++;
                        _nCell.InnerText = format(PerformanceModule.recalcToBase(pr)) + " %";
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
                    _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_AVERAGEEOPERFORMANCE_PROPORTION);
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
                _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,PerformanceModule.LANG_MNEMO_REP_AVERAGEEOPERFORMANCE_PROPORTION);
                _nCell = _nRow.AppendChild(createCell());
                if (!prAvgAll.Equals(Double.NaN))
                {
                    _nCell.InnerText = format(PerformanceModule.recalcToBase(prAvgAll)) + " %";
                }
            }
        }

        private double performanceProportion(string prId) 
        {
            string sql = "select avg(relativ_weight) PERFORMANCE,"
				+ " avg(CRITERIA_WEIGHT) CRITERIA_WEIGHT"
				+ " from performancerating_items"
				+ " where relativ_weight >= 0 and criteria_weight > 0"
				+ " and performancerating_ref = " + prId
				+ " group by criteria_ref_persisting";
            double _prRating = 0.0;
            DataTable table = _db.getDataTable(sql);
            double sum = 0.0;
            double sumCriteriaWeight = 0.0;

            foreach (DataRow row in table.Rows) 
            {
                double pw = DBColumn.GetValid(row["PERFORMANCE"],0.0);
                double cw = DBColumn.GetValid(row["CRITERIA_WEIGHT"],0.0);
                sumCriteriaWeight += cw;
                double r = pw * cw;
                sum += r;
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
