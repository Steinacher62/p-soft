using ch.appl.psoft.db;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for PersonObjectiveReport.
    /// </summary>
    public class PersonObjectiveReport : PsoftPDFReport {
        private long _personId = 0;
        private string _dateFormat = "";
        private DataTable _data = null;

        public PersonObjectiveReport(HttpSessionState Session, string imagePath) : base(Session, imagePath) {
        }

        public void writeReport(long personId, DataTable data, string turnid) {
            _personId = personId;
            _data = data;

            _dateFormat = _db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern;

            string header = _map.get("mbo", "personalObjectives");
            base.writeHeaderAndFooter(header, DateTime.Now.ToString(_dateFormat));

            writeContent(turnid);

        }

        private void writeContent(string turnid) {
            string personName = _db.lookup("pname","person","id="+_personId,false);
            string firstName = _db.lookup("firstname","person","id="+_personId,false);
            DataTable jobData = null;
            DataRow jobRow = null;
            DataColumn col = null;
            string name = "";
            string val = "";
            double weightSum = 0;
            double ratingSum = 0;

                        
            XmlNode table = base.AppendTable("5cm,4cm,4cm,*");
            XmlNode row = table.AppendChild(base.createRow());
            XmlNode cell = row.AppendChild(createCell());

            table.Attributes.Append(base.createAttribute("border-width-bottom","1"));

            // Job
            cell.Attributes.Append(createClassAttribute("detailName"));
            cell.Attributes.Append(base.createAttribute("font-size","12"));
            cell.InnerText = _map.get("mbo","reportstellenbez");
            jobData = _db.Person.getMOJobs(_personId);
            foreach (DataRow r in jobData.Rows) {
                if (jobRow == null) jobRow = r;
                cell = row.AppendChild(createCell());
                cell.Attributes.Append(createClassAttribute("detailValue"));
                cell.Attributes.Append(base.createAttribute("font-size","12"));
                cell.Attributes.Append(createAttribute("colspan", "3"));
                cell.InnerText = r[_db.langAttrName("JOB","TITLE")].ToString();
                row = table.AppendChild(base.createRow());
                row.AppendChild(createCell());
            }

            // person name
            row = table.AppendChild(base.createRow());
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailName"));
            cell.Attributes.Append(base.createAttribute("font-size","12"));
            cell.InnerText = _map.get("mbo","reportpname");
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailValue"));
            cell.Attributes.Append(base.createAttribute("font-size","12"));
            cell.InnerText = personName;
            // person firstname
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailName"));
            cell.Attributes.Append(base.createAttribute("font-size","12"));
            cell.InnerText = _map.get("mbo","reportfirstname");
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailValue"));
            cell.Attributes.Append(base.createAttribute("font-size","12"));
            cell.InnerText = firstName;

            // create date
            row = table.AppendChild(base.createRow());
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailName"));
            cell.InnerText = _map.get("mbo","reportcreatedate");
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailValue"));
            col = _data.Columns["STATEDATE"];
            col.ExtendedProperties["Format"] = _dateFormat;
            if (_data.Rows.Count > 0) cell.InnerText = _db.dbColumn.GetDisplayValue(col,_data.Rows[0]["RATING_DATE"],false);
            // year
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailName"));
            cell.InnerText = _map.get("mbo","reportyear");
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailValue"));
            cell.InnerText = _db.lookup("TITLE", "OBJECTIVE_TURN", "ID=" + turnid, "");
            //cell.InnerText = _db.lookup("TITLE","OBJECTIVE_TURN","ID="+_db.Objective.turnId,"");
            cell.Attributes.Append(createAttribute("padding-bottom", "10"));

            base.appendVSpace(20);

            foreach (DataRow r in _data.Rows) {
                table = base.AppendTable("6cm,*");
                table.Attributes.Append(base.createAttribute("keep-together","true"));
                table.Attributes.Append(base.createAttribute("border-width-bottom","1"));

                col = _data.Columns["TITLE"];
                val = _db.dbColumn.GetDisplayValue(col, r[col], false);
                name = _map.get("mbo", "reportobjective");
                writeProperty(table, name, val, 1, 20);

                col = _data.Columns["PARENT_ID"];
                val = _db.dbColumn.GetDisplayValue(col, r[col], false);
                if (!val.Equals(""))
                {
                    name = _map.get("OBJECTIVE", "PARENT_ID");
                    val = _db.lookup("TITLE", "OBJECTIVE", "ID=" + val).ToString();
                    writeProperty(table, name, val, 2, 20);
                }

                col = _data.Columns["STARTDATE"];
                col.ExtendedProperties["Format"] = _dateFormat;
                val = _db.dbColumn.GetDisplayValue(col, r[col], false);
                name = _map.get("mbo", "reportstartdate");
                writeProperty(table, name, val, 2, 20);
                

                col = _data.Columns["DATEOFREACHING"];
                col.ExtendedProperties["Format"] = _dateFormat;
                val = _db.dbColumn.GetDisplayValue(col, r[col], false);
                name = _map.get("mbo", "reportdateofreaching");
                writeProperty(table, name, val, 2, 20);

                col = _data.Columns["DESCRIPTION"];
                val = _db.dbColumn.GetDisplayValue(col, r[col], false);
				if( !val.Equals(""))
                {
				name = _map.get("mbo","description");
				writeProperty(table,name,val.ToString(),1,20);
                }
               

                col = _data.Columns["ACTIONNEED"];
                val = _db.dbColumn.GetDisplayValue(col,r[col],false);
                if (!val.Equals(""))
                {
                    name = _map.get("mbo", "reportactionneed");
                    writeProperty(table, name, val, 1, 20);
                }

                col = _data.Columns["MEASUREKRIT"];
                val = _db.dbColumn.GetDisplayValue(col,r[col],false);
                if (!val.Equals(""))
                {
                    name = _map.get("mbo", "reportmeasurekrit");
                    writeProperty(table, name, val, 1, 20);
                }

                col = _data.Columns["MEMO"];
                val = _db.dbColumn.GetDisplayValue(col, r[col], false);
                if (!val.Equals(""))
                {
                    name = _map.get("mbo", "reportmemo");
                    writeProperty(table, name, val, 1, 20);
                }

                col = _data.Columns["RATING_WEIGHT"];
                double weight = DBColumn.GetValid(r[col],0.0);
                weightSum += weight;
                val = _db.dbColumn.GetDisplayValue(col,weight,false)+" %";
                name = _map.get("mbo","reportweight");
                writeProperty(table,name,val,2,40);

                col = _data.Columns["RATING"];
                int rating = DBColumn.GetValid(r[col],0);
                ratingSum += rating*weight;
                val = _db.dbColumn.GetDisplayValue(col,rating,false)+" %";
                name = _map.get("mbo","reportrating");
                cell = writeProperty(table,name,val,2,40);
                cell.Attributes.Append(createAttribute("padding-bottom", "10"));

                base.appendVSpace(20);
            }   
            table = base.AppendTable("6cm,*");
            table.Attributes.Append(base.createAttribute("keep-together","true"));
            //if (Global.isModuleEnabled("lohn") && Global.isModuleEnabled("performance") && jobRow != null) {
            // Jahreslohn
            //col = jobData.Columns["EMPLOYMENT_ID"];
            //double istLohn = _db.Lohn.getIstLohn((long)jobRow[col]);
            //val = (Math.Round(istLohn/0.05,0)*0.05).ToString("0.00")+" CHF";
            //name = _map.get("mbo","reportysalary");
            //cell = writeProperty(table,name,val,2,20);
            //// Anteil org
            //val = "100.0 %";
            //name = _map.get("mbo","reportmboorg");
            //cell = writeProperty(table,name,val,2,20);
            //// Anteil Stelle
            //col = jobData.Columns["ID"];
            //double jobWeight = _db.Performance.getRatingWeightForJob((long)jobRow[col], (int) Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.MboRating);
            //val = jobWeight.ToString("0.0")+" %";
            //name = _map.get("mbo","reportjobweight");
            //cell = writeProperty(table,name,val,2,20);
            // Erfüllungsgrad
            double mboRating = ratingSum / weightSum;
            val = mboRating.ToString("0.0") + " %";
            name = _map.get("mbo", "reportratingtot");
            cell = writeProperty(table, name, val, 2, 20);
            //// MbO Antail
            //double mbo = istLohn*jobWeight/100*mboRating/100;
            //val = (Math.Round(mbo/0.05,0)*0.05).ToString("0.00")+" CHF";
            //name = _map.get("mbo","reportmboweight");
            //cell = writeProperty(table,name,val,2,20);
            //}
            base.appendVSpace(40);

            // Visum
            table = base.AppendTable("4cm,3cm,6cm");
            table.Attributes.Append(base.createAttribute("keep-together","true"));
            table.Attributes.Append(base.createAttribute("padding-left","20"));
            row = table.AppendChild(base.createRow());
            cell = row.AppendChild(createCell());
            cell = row.AppendChild(createCell());
            cell.InnerText = _map.get("mbo","reportdate");
            cell = row.AppendChild(createCell());
            cell.InnerText = _map.get("mbo","reportsign");;
            row = table.AppendChild(base.createRow());
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createAttribute("colspan", "3"));

            table = cell.AppendChild(base.CreateTable("4cm,3cm,6cm"));
            table.Attributes.Append(base.createAttribute("border-width-all","1"));
            table.Attributes.Append(base.createAttribute("padding-all","5"));
            row = table.AppendChild(base.createRow());
            cell = row.AppendChild(createCell());
            cell.InnerText = _map.get("mbo","reportchef");;
            cell = row.AppendChild(createCell());
            cell = row.AppendChild(createCell());
            row = table.AppendChild(base.createRow());
            cell = row.AppendChild(createCell());
            cell.InnerText = _map.get("mbo","reportemployee");;
            cell = row.AppendChild(createCell());
            cell = row.AppendChild(createCell());

        }
        private XmlNode writeProperty(XmlNode table,string name, string val, int cols, int padding) {
            XmlNode row = table.AppendChild(base.createRow());
            XmlNode cell = row.AppendChild(createCell());

            if (padding > 0) cell.Attributes.Append(createAttribute("padding-left", padding.ToString()));
            cell.Attributes.Append(createClassAttribute("detailName"));
            cell.InnerText = name;
            if (cols == 1) {
                cell.Attributes.Append(createAttribute("colspan", "2"));
                row = table.AppendChild(base.createRow());
            }
            cell = row.AppendChild(createCell());
            cell.Attributes.Append(createClassAttribute("detailValue"));
            addCellText(cell, val);
            if (cols == 1) {
                cell.Attributes.Append(createAttribute("colspan", "2"));
                if (padding > 0) cell.Attributes.Append(createAttribute("padding-left", padding.ToString()));
            }
            return cell;
        }
        
    }
}
