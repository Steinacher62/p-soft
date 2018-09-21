using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Habasit
{
    /// <summary>
    /// Summary description for AveragePerformanceReport.
    /// </summary>
    public class AveragePerformanceReport : ch.appl.psoft.Performance.PReport
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
			return AppendTable("420,100");
		}

		private XmlNode AppendSignTable()
		{
			XmlNode nTable = createTable();
			nTable.Attributes.Append(createAttribute("widths", "25%,25%,50%"));
			nTable.Attributes.Append(createAttribute("keep-together", "false"));
			nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
			nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
			nTable.Attributes.Append(createAttribute("padding-all", "2"));
			nTable.Attributes.Append(createAttribute("align", "left"));
			nTable.Attributes.Append(createClassAttribute("detailValue"));

			return _rootNode.AppendChild(nTable);
		}

		private XmlNode AppendRemarksTable()
		{
			XmlNode nTable = createTable();
			nTable.Attributes.Append(createAttribute("widths", "100%"));
			nTable.Attributes.Append(createAttribute("keep-together", "false"));
			nTable.Attributes.Append(createAttribute("border-width-inner", "0"));
			nTable.Attributes.Append(createAttribute("border-width-outer", "0"));
			nTable.Attributes.Append(createAttribute("padding-all", "2"));
			nTable.Attributes.Append(createAttribute("align", "left"));
			nTable.Attributes.Append(createClassAttribute("detailValue"));

			return _rootNode.AppendChild(nTable);
		}



		public override void createReport(object[] parameter)
		{
			_employmentID = int.Parse(parameter[0].ToString());//employmentID;
			_performanceRatingID = int.Parse(parameter[1].ToString());//performanceRatingID;


			_personID = ch.psoft.Util.Validate.GetValid((_db.lookup("PERSON_ID","EMPLOYMENT","ID="+_employmentID)).ToString(),-1L);

			writeHeaderAndFooter(_map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_AVERAGEPERFORMANCE), DateTime.Now.ToString("d"));

			// person detail
			_nTable = AppendDetailTable();
			DataTable table = _db.getDataTableExt("select ID, FIRSTNAME, PNAME, PERSONNELNUMBER from PERSON where ID=" + _personID, "PERSON");
			//            table..Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
			addDetail(_db, _nTable, table, "MA", true);
            
			// employment
			_nTable = AppendDetailTable();
			table = _db.getDataTableExt("select distinct " + _db.langAttrName("PERFORMANCERATING", "JOB_TITLE") + " from PERFORMANCERATING where ID=" + _performanceRatingID, "PERFORMANCERATING");
			addDetail(_db, _nTable, table, "", true);

			_nTable = AppendDetailTable();
			table = _db.getDataTableExt("select RATING_DATE from PERFORMANCERATING where ID=" + _performanceRatingID, "PERFORMANCERATING");
			addDetail(_db, _nTable, table, "", true);

			_nRow = _nTable.AppendChild(createRow());
			_nRow.Attributes.Append(createAttribute("padding-top", "10"));
			_nCell = _nRow.AppendChild(createCell());

			appendHLine(520);
			appendVSpace(10);


			DataTable tableCriteria = _db.getDataTableExt("select distinct CRITERIA_REF, " + _db.langAttrName("PERFORMANCERATING_ITEMS", "CRITERIA_TITLE") + " from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF="+_performanceRatingID, "PERFORMANCERATING_ITEMS");            

			foreach (DataRow rowCriteria in tableCriteria.Rows) 
			{
				long criteriaID = ch.psoft.Util.Validate.GetValid(rowCriteria["CRITERIA_REF"].ToString(), -1L);
				string title =  ch.psoft.Util.Validate.GetValid(rowCriteria[_db.langAttrName(rowCriteria.Table.TableName, "CRITERIA_TITLE")].ToString(), "");
				DataTable tableItems = _db.getDataTableExt("select ID from PERFORMANCERATING_ITEMS where PERFORMANCERATING_REF="+_performanceRatingID+" and CRITERIA_REF="+criteriaID, "PERFORMANCERATING_ITEMS");            
				if (tableItems.Rows.Count > 0) 
				{
					appendVSpace(20);
					_nTable = AppendPerformanceTable();
					_nRow = _nTable.AppendChild(createRow());
					_nRow.Attributes.Append(createClassAttribute("title"));
					_nCell = _nRow.AppendChild(createCell());
					_nCell.Attributes.Append(createAttribute("colspan", "2"));
					_nCell.InnerText = title;

					foreach (DataRow itemsRow in tableItems.Rows)
					{                 

						String itemsID = ch.psoft.Util.Validate.GetValid(itemsRow[_db.langAttrName(itemsRow.Table.TableName, "ID")].ToString(), "");
						DataTable detailTable = _db.getDataTableExt("select " + _db.langAttrName("PERFORMANCERATING_ITEMS", "LEVEL_TITLE")+ "," + _db.langAttrName("PERFORMANCERATING_ITEMS", "EXPECTATION_DESCRIPTION") + " from PERFORMANCERATING_ITEMS where ID="+itemsID, "PERFORMANCERATING_ITEMS");  

						appendVSpace(10);
						_nTable = AppendDetailTable();
						addDetail(_db, _nTable, detailTable, "", false);
                    
					}

					DataTable tableArgs = _db.getDataTableExt("select * from PERFORMANCERATING_ARGUMENTS where PERFORMANCERATING_REF="+_performanceRatingID+" and PERFORMANCERATING_CRITERIA_REF="+criteriaID, "PERFORMANCERATING_ARGUMENTS");            
					appendVSpace(10);
					_nTable = AppendDetailTable();
					addDetail(_db, _nTable, tableArgs, "", false);

				}
			}

			appendVSpace(10);
			// remarksbox
			_nTable = AppendRemarksTable();
			_nRow = _nTable.AppendChild(createRow());
			_nRow.Attributes.Append(createAttribute("leading", "150"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell.InnerText = _map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_EMPL_REMARKS);

			appendVSpace(10);
			// signbox
			_nTable = AppendSignTable();
			_nRow = _nTable.AppendChild(createRow());
			_nCell = _nRow.AppendChild(createCell());
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_DATE);
			_nCell = _nRow.AppendChild(createCell());
			_nCell.InnerText = _map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_SIGN);
			_nRow = _nTable.AppendChild(createRow());
			_nRow.Attributes.Append(createAttribute("leading", "100"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell.InnerText = _map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_CLERK);
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nRow = _nTable.AppendChild(createRow());
			_nRow.Attributes.Append(createAttribute("leading", "100"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell.InnerText = _map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_SUPERVISOR);
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nRow = _nTable.AppendChild(createRow());
			_nRow.Attributes.Append(createAttribute("leading", "100"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell.InnerText = _map.get(HabasitModule.LANG_SCOPE_HABASIT, HabasitModule.LANG_MNEMO_REP_HIGHERSUPERVISOR);
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));
			_nCell = _nRow.AppendChild(createCell());
			_nCell.Attributes.Append(createAttribute("border-width-all", "1"));


		}

	}


}
