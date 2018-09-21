using ch.appl.psoft.db;
using ch.appl.psoft.Organisation;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for GlobalPerformanceReport.
    /// </summary>
    public class GlobalPerformanceReport : PReport
    {
        protected DataTable _competenceLevels = null;
        protected long _employmentID = -1;
        protected long _personID = -1;
        protected long _performanceRatingID = -1;
        protected string _graphName = "";
        protected long _mboID = -1;
        protected long _skillsAppraisalID = -1;
		// Gewichte für Gesamtbewertung
		protected double _performanceRatingWeight = 0.0;
		protected double _skillsWeight = 0.0;
		protected double _mboWeight = 0.0;

        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;

        public GlobalPerformanceReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendPerformanceTable()
        {
            return AppendTable("420,100");
        }

        private XmlNode AppendSignTable()
        {
            XmlNode nTable = createTable();
            nTable.Attributes.Append(createAttribute("widths", "420,100"));
            nTable.Attributes.Append(createAttribute("keep-together", "false"));
            nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
            nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
            nTable.Attributes.Append(createAttribute("padding-all", "2"));
            nTable.Attributes.Append(createAttribute("align", "left"));
            nTable.Attributes.Append(createClassAttribute("detailValue"));

            return _rootNode.AppendChild(nTable);
        }

        private void createGraph()
        {
            PROverallImage image = new PROverallImage(_db,_map,1200,1200);
            FileInfo file = new FileInfo(_graphName);
            Stream stream = file.OpenWrite();

            image.resolution = 192;

            try 
            {
                Bitmap map = image.draw(
						_mboID,
						_performanceRatingID,
						_skillsAppraisalID,
						_mboWeight,
						_performanceRatingWeight,
						_skillsWeight
					);

                map.Save(stream, ImageFormat.Jpeg);
                map.Dispose();

                addImageToCatalog(_graphName, "graph");
            }
            catch (Exception ex) 
            {
                Logger.Log(ex,Logger.ERROR);
            }
            stream.Close();
        }

		/*	public override void createReport(long employmentID, long performanceRatingID, string graphName, long mboID, long skillsAppraisalID)
        */
		public override void createReport(object[] parameter)
        {
			_employmentID = long.Parse(parameter[0].ToString());
            _performanceRatingID = long.Parse(parameter[1].ToString());
			_performanceRatingWeight = _db.Performance.getRatingWeight(
					_employmentID,
					(int)ch.appl.psoft.Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.PerformanceRating
				);
            _graphName = parameter[2].ToString();;
            _mboID = long.Parse(parameter[3].ToString());
			_mboWeight = _db.Performance.getRatingWeight(
					_employmentID,
					(int)ch.appl.psoft.Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.MboRating
				);
			_skillsAppraisalID = long.Parse(parameter[4].ToString());
			_skillsWeight = _db.Performance.getRatingWeight(
					_employmentID,
					(int)ch.appl.psoft.Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.SkillsRating
				);

            createGraph();

            _personID = ch.psoft.Util.Validate.GetValid((_db.lookup("PERSON_ID","EMPLOYMENT","ID="+_employmentID)).ToString(),-1L);

            writeHeaderAndFooter(_map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_GLOBALPERFORMANCE), DateTime.Now.ToString("d"));

            // person detail
            _nTable = AppendDetailTable();
            DataTable table = _db.getDataTableExt("select ID, FIRSTNAME, PNAME, PERSONNELNUMBER, PHONE, EMAIL from PERSON where ID=" + _personID, "PERSON");
            addDetail(_db, _nTable, table, "MA", true);
            
            // employment / job determination
			long jobID = -1;
			table = _db.getDataTableExt(
					"select " + _db.langAttrName("JOB", "TITLE")
						+ " from JOB where ID = " + jobID,
					"JOB"
				);

			DataColumn column = table.Columns[0];
			DBColumn.Visibility visi = (DBColumn.Visibility)column.ExtendedProperties["Visibility"];
			object dbViews = column.ExtendedProperties["Views"];

			// Soll die Angabe angezeigt werden?
			if (visi >= DBColumn.Visibility.DETAIL
				&& DBColumn.IsInView("JOB", dbViews)
				&& _db.hasColumnAuthorisation(
					DBData.AUTHORISATION.READ,
					table,
					column.ColumnName,
					true,
					true
				)
			)
			{
				// Zeile erstellen
				_nTable = AppendDetailTable();
				XmlNode nRow = _nTable.AppendChild(createRow());
				XmlNode nCell = nRow.AppendChild(createCell());
				nCell.Attributes.Append(createClassAttribute("detailName"));
				nCell.InnerText = _map.get("JOB", column.ColumnName);

				nCell = nRow.AppendChild(createCell());
				nCell.Attributes.Append(createClassAttribute("detailValue"));
				nCell.InnerText = "";

				// Jobs gemäss allgemeiner Regel einfügen
				int jobCount = 0;

				Hashtable jobPartsTable = OrganisationModule.jobParts(_db, _employmentID);
			
				foreach (object key in jobPartsTable.Keys)
				{
					if (Validate.GetValid(jobPartsTable[key].ToString(), (double)0) > 0)
					{
						jobID = Validate.GetValid(key.ToString(), (long)-1);
						table = _db.getDataTableExt(
								"select " + _db.langAttrName("JOB", "TITLE")
									+ " from JOB where ID = " + jobID,
								"JOB"
							);

						if (table.Rows.Count > 0)
						{
							if (jobCount > 0)
							{
								nCell.InnerText += ", ";
							}

							nCell.InnerText += table.Rows[0][0].ToString();
							jobCount++;
						}
					}
				}

				if (jobCount == 0)
				{
					Logger.Log(_map.get("organisation", "noHauptfunktionFound"), Logger.ERROR);
					nCell.InnerText += _map.get("organisation", "noHauptfunktionFound");
				}
			}

            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-top", "10"));
            _nCell = _nRow.AppendChild(createCell());

            appendHLine(520);
            appendVSpace(10);

            // rating-dates
            appendVSpace(10);
            _nTable = AppendDetailTable();
            table = _db.getDataTableExt("select RATING_DATE from OBJECTIVE_PERSON_RATING where ID=" + _mboID, "OBJECTIVE_PERSON_RATING");
            addDetail(_db, _nTable, table, "OBJECTIVE_OE", false);
            _nTable = AppendDetailTable();
            table = _db.getDataTableExt("select APPRAISALDATE from SKILLS_APPRAISAL where ID=" + _skillsAppraisalID, "SKILLS_APPRAISAL");
            addDetail(_db, _nTable, table, "REPORTVIEW", false);
            _nTable = AppendDetailTable();
            table = _db.getDataTableExt("select RATING_DATE from PERFORMANCERATING where ID=" + _performanceRatingID, "PERFORMANCERATING");
            addDetail(_db, _nTable, table, "REPORTVIEW", false);
            appendVSpace(10);
            
            // graph
            _nTable = AppendPerformanceTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.AppendChild(createShowImage("graph"));

            appendVSpace(10);
            // signbox
            _nTable = AppendSignTable();
            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell = _nRow.AppendChild(createCell());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_DATE);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_SIGN);
            _nCell = _nRow.AppendChild(createCell());
            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_SUPERVISOR);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            _nCell = _nRow.AppendChild(createCell());
            _nRow = _nTable.AppendChild(createRow());
            _nCell = _nRow.AppendChild(createCell());
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            _nCell.InnerText = _map.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_REP_CLERK);
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.Attributes.Append(createAttribute("border-width-all", "1"));
            _nCell = _nRow.AppendChild(createCell());


            appendVSpace(20);



        }

     }


}
