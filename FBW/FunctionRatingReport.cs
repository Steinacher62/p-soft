using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.FBW
{
    /// <summary>
    /// Summary description for FunctionRatingReport.
    /// </summary>
    public class FunctionRatingReport : PsoftPDFReport
	{
        protected long _fbwID = -1;

        public FunctionRatingReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}
        
        public void createReport(int fbwID)
        {
            _fbwID = fbwID;
            //11.04.19 Datum revision in Report auf Wunsch Habasit entfernt 
            DataTable table = _db.getDataTableExt("select ID, FUNKTION_ID, FUNKTIONSWERT, GUELTIG_AB, GUELTIG_BIS from FUNKTIONSBEWERTUNGFUNKTIONV where ID=" + _fbwID, "FUNKTIONSBEWERTUNGFUNKTIONV");
            if (table.Rows.Count > 0){
                long funktionID = ch.psoft.Util.Validate.GetValid( table.Rows[0]["FUNKTION_ID"].ToString(), -1L);

                writeHeaderAndFooter(_map.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_REP_FUNCTIONRATING).Replace("#1", _db.lookup("TITLE", "FUNKTION", "ID=" + funktionID, false)), DateTime.Now.ToString("d"));               

                // function rating detail
                XmlNode nTable = AppendDetailTable();
                addDetail(_db, nTable, table);
                XmlNode nRow = nTable.AppendChild(createRow());
                nRow.Attributes.Append(createAttribute("padding-top", "10"));
                XmlNode nCell = nRow.AppendChild(createCell());
                appendHLine(520);

                //criterias
                nTable = AppendTable("25,495");
                DataTable criteriasTable = _db.getDataTable("select ID, BEZEICHNUNG from FBW_KRITERIUM order by ORDNUMBER asc");
                int baseIndex = 1;
                bool isFirst = true;
                foreach (DataRow row in criteriasTable.Rows){
                    appendCriteria(nTable, baseIndex++, ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), row[1].ToString(), isFirst);
                    if (isFirst){
                        isFirst = false;
                    }
                }
            }
        }

        protected void appendCriteria(XmlNode nTable, int baseIndex, long criteriaID, string bezeichnung, bool isFirst){
            // Kriterium Bezeichnung
            XmlNode nRow = nTable.AppendChild(createRow());
            if (isFirst){
                nRow.Attributes.Append(createAttribute("padding-top", "10"));
            }
            else {
                nRow.Attributes.Append(createAttribute("padding-top", "30"));
            }
            XmlNode nCell = nRow.AppendChild(createCell());
            nRow = nTable.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("title"));
            nCell.InnerText = baseIndex.ToString();
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("title"));
            nCell.InnerText = bezeichnung;

            // Kriterium Bewertung
            nRow = nTable.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("padding-top", "10"));
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("subTitle"));
            nCell.InnerText = baseIndex.ToString() + ".1";
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("subTitle"));
            nCell.InnerText = _map.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_REP_RATING);

            nRow = nTable.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("detailValue"));
            nCell.Attributes.Append(createAttribute("padding-left", "0"));
            DataTable table = _db.getDataTableExt("select * from FBWKOMMENTARV where FUNKTIONSBEWERTUNG_ID=" + _fbwID + " and FBW_KRITERIUM_ID=" + criteriaID, "FBWKOMMENTARV");
            //Einstufung entfernt 2013.05.24 MSr
            table.Columns.Remove("Bezeichnung");
            
            XmlNode nDetailTable = CreateDetailTable();
            nCell.AppendChild(nDetailTable);
            addDetail(_db, nDetailTable, table, "FBWKOMMENTARV", false);


            // Anforderungen
            DataTable anforderungTable = _db.getDataTable("select * from ANFORDERUNGDETAILV where FUNKTIONSBEWERTUNG_ID=" + _fbwID + " and KRITERIUM_ID=" + criteriaID + " ORDER BY FBW_ARGUMENT_KATALOG_ID");
            int subIndex = 2;
            foreach (DataRow row in anforderungTable.Rows) {
                appendAnforderung(nTable, baseIndex, subIndex++, row);
            }
        }

        protected void appendAnforderung(XmlNode nTable, int baseIndex, int subIndex, DataRow anforderungRow){
            // Bezeichnung (mit Katalog-Pfad)
            XmlNode nRow = nTable.AppendChild(createRow());
            nRow.Attributes.Append(createAttribute("padding-top", "10"));
            XmlNode nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("subTitle"));
            nCell.InnerText = baseIndex.ToString() + "." + subIndex.ToString();
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("subTitle"));
            nCell.InnerText = FBWModule.getCatalogPath(_db, ch.psoft.Util.Validate.GetValid(anforderungRow["FBW_ARGUMENT_KATALOG_ID"].ToString(), -1L), false) + anforderungRow["BEZEICHNUNG"].ToString();
            string anpassung = ch.psoft.Util.Validate.GetValid(anforderungRow["ELEMENT_BEZEICHNUNG"].ToString(), "");
			string punktezahl = ch.psoft.Util.Validate.GetValid(anforderungRow["STUFE_PUNKTEZAHL"].ToString(), "");
            if (anpassung != ""){
                nCell.InnerText += " (" + anpassung + ")";
            }
			if (punktezahl != "")
			{
				nCell.InnerText += " [" + punktezahl + "]";
			}

            // Beschreibung
            nRow = nTable.AppendChild(createRow());
            nCell = nRow.AppendChild(createCell());
            nCell = nRow.AppendChild(createCell());
            nCell.Attributes.Append(createClassAttribute("detailValue"));
            addCellText(nCell, anforderungRow["BESCHREIBUNG"].ToString());
        }
    }
}
