using ch.appl.psoft.Lohn;
using ch.appl.psoft.Organisation;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.TPC
{
    public class TPCPersonDetailReport : PersonDetailReport
    {

        public TPCPersonDetailReport(HttpSessionState session)
            : base(session)
        {
        }

        public void createPersonDetailReport(long lohnId, string salaryKomponent)
        {
            _db.connect();
            try
            {
                DataTable dataTable = _db.getDataTableExt(String.Concat("select * from TPC_LOHNDATEN_V where ID = ", lohnId), new object[] { "TPC_LOHNDATEN_V" });
                string[] strs = _map.getEnum("lohn", "salaryType", true);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow1 = dataTable.Rows[0];
                    long l1 = SQLColumn.GetValid(dataRow1["VARIANTE_ID"], (long)-1);
                    int i1 = SQLColumn.GetValid(_db.lookup("LR_GUELTIG_AB", "VARIANTE_LOHNRUNDE_V", String.Concat("ID = ", l1)), DateTime.Now.AddYears(1)).Year;
                    double d1 = SQLColumn.GetValid(dataRow1["ISTLOHN"], 0.0);
                    double d2 = SQLColumn.GetValid(dataRow1["LOHNVORSCHLAG"], 0.0);
                    long l2 = SQLColumn.GetValid(dataRow1["PERSON_ID"], (long)-1);
                    string str1 = String.Concat(_db.Person.getWholeName(l2.ToString(), false), " ", _map.get("tpc", "salarySituation").Replace("#1", String.Concat(i1)));
                    base.writeHeaderAndFooter(str1, DateTime.Now.ToString("d"));
                    base.appendVSpace(20);
                    XmlNode xmlNode1 = _rootNode.AppendChild(base.createTable());
                    xmlNode1.Attributes.Append(base.createAttribute("widths", "150,175,175"));
                    xmlNode1.Attributes.Append(base.createAttribute("keep-together", "true"));
                    xmlNode1.Attributes.Append(base.createAttribute("border-width-inner", "0"));
                    xmlNode1.Attributes.Append(base.createAttribute("border-width-outer", "0"));
                    xmlNode1.Attributes.Append(base.createAttribute("padding-all", "2"));
                    xmlNode1.Attributes.Append(base.createAttribute("align", "left"));
                    DataColumn dataColumn1 = dataTable.Columns["COSTCENTER_NUMBER"];
                    DataColumn dataColumn2 = dataTable.Columns["COSTCENTER_DESCRIPTION"];
                    XmlNode xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "COSTCENTER_NUMBER")));
                    xmlNode2.AppendChild(base.createDetailValueCell(String.Concat(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), " - ", _db.GetDisplayValue(dataColumn2, dataRow1[dataColumn2], false)), 2));
                    dataColumn1 = dataTable.Columns["EMPLOYMENTKIND"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "EMPLOYMENTKIND")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    dataColumn1 = dataTable.Columns["EMPLOYMENTTYP"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "EMPLOYMENTTYP")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    dataColumn1 = dataTable.Columns["ENTRY"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "ENTRY")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    dataColumn1 = dataTable.Columns["AGE"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "AGE")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    dataColumn1 = dataTable.Columns["SUPERIOR"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "SUPERIOR")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    long l3 = SQLColumn.GetValid(dataRow1["EMPLOYMENT_ID"], (long)-1);
                    DataRow dataRow2 = dataTable.Rows[0];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("FUNKTION", _db.langAttrName("FUNKTION", "TITLE", new string[0]))));
                    string str2 = _db.lookup("F.EXTERNAL_REF", "FUNKTION F inner join JOB J on F.ID = J.FUNKTION_ID", String.Concat("J.ID = ", OrganisationModule.getMaximalPartJobId(_db, l3, false)), "");
                    if (str2 != "")
                    {
                        str2 = String.Concat(str2.TrimStart(new char[] { '0' }), " - ");
                    }
                    str2 = String.Concat(str2, OrganisationModule.getFunktionTitle(_db, l3, "JOB"));
                    xmlNode2.AppendChild(base.createDetailValueCell(str2, 2));
                    dataColumn1 = dataTable.Columns["FUNKTIONSWERT"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "FUNKTIONSWERT")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    dataColumn1 = dataTable.Columns["FUNKTION_AB"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "FUNKTION_AB")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], false), 2));
                    _salaryDivisor = 1.0;
                    int j1 = SQLColumn.GetValid(dataRow1["LOHNART"], 1);
                    int k1 = SQLColumn.GetValid(_db.lookup("L.ANZAHL_MONATSLOEHNE", "LOHNRUNDE L inner join VARIANTE V on L.ID = V.LOHNRUNDE_ID", String.Concat("V.ID = ", l1)), 1);
                    int i2 = SQLColumn.GetValid(dataRow1["ANZAHL_LOEHNE"], k1);
                    int j2 = SQLColumn.GetValid(dataRow1["ANZAHL_STUNDEN"], 1);
                    int k2 = SQLColumn.GetValid(dataRow1["ANZAHL_TAGE"], 1);
                    dataColumn1 = dataTable.Columns["ENGAGEMENT"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "ENGAGEMENT")));
                    XmlNode xmlNode3 = xmlNode2.AppendChild(base.createDetailValueCell("", "right"));
                    XmlNode xmlNode4 = null;
                    if (j1 > 1)
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell("", "right"));
                    }
                    switch (j1)
                    {
                        case 1:
                            _salaryDivisor = 1.0;
                            xmlNode3.InnerText = String.Concat(_db.GetDisplayValue(dataColumn1, dataRow1["ENGAGEMENT"], false), " %");
                            break;

                        case 2:
                            _salaryDivisor = i2;
                            xmlNode3.InnerText = String.Concat(_db.GetDisplayValue(dataColumn1, dataRow1["ENGAGEMENT"], false), " %");
                            break;

                        case 3:
                            _salaryDivisor = k2;
                            xmlNode4.InnerText = String.Concat(_db.GetDisplayValue(dataColumn1, dataRow1["ENGAGEMENT"], false), " %");
                            xmlNode3.InnerText = String.Concat(k2, " ", _map.get("lohn", "days"));
                            break;

                        case 4:
                            _salaryDivisor = j2;
                            xmlNode4.InnerText = String.Concat(k2, " ", _map.get("lohn", "days"));
                            xmlNode3.InnerText = String.Concat(j2, " ", _map.get("lohn", "hours"));
                            break;
                    }
                    dataColumn1 = dataTable.Columns["ANFORDERUNGSZULAGE"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "ANFORDERUNGSZULAGE")));
                    xmlNode2.AppendChild(base.createDetailValueCell(SQLColumn.GetValid(dataRow1[dataColumn1], 0.0).ToString("c"), "right"));
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "LOHNART")));
                    xmlNode2.AppendChild(base.createDetailValueCell(strs[j1], "right"));
                    if (j1 == 1)
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(strs[2], "right"));
                    }
                    else
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(strs[1], "right"));
                    }
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "ISTLOHN")));
                    xmlNode2.AppendChild(base.createDetailValueCell(base.getPartSalary(d1).ToString("c"), "right"));
                    double d3 = 0.0;
                    if (j1 == 1)
                    {
                        d3 = d1 / i2;
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d3.ToString("c"), "right"));
                    }
                    else
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d1.ToString("c"), "right"));
                    }
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "LOHNVORSCHLAG")));
                    xmlNode2.AppendChild(base.createDetailValueCell(base.getPartSalary(d2).ToString("c"), "right"));
                    if (j1 == 1)
                    {
                        d3 = d2 / i2;
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d3.ToString("c"), "right"));
                    }
                    else
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d2.ToString("c"), "right"));
                    }
                    double d4 = d2 - d1;
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("tpc", "proposedSalaryDifference")));
                    xmlNode2.AppendChild(base.createDetailValueCell(base.getPartSalary(d4).ToString("c"), "right"));
                    if (j1 == 1)
                    {
                        d3 = d4 / i2;
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d3.ToString("c"), "right"));
                    }
                    else
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d4.ToString("c"), "right"));
                    }
                    double d5 = Math.Round(d4 / d2 * 100.0, 2);
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("tpc", "proposedSalaryPercentageDifference")));
                    xmlNode2.AppendChild(base.createDetailValueCell((!Double.IsNaN(d5) && !Double.IsInfinity(d5)) ? String.Concat(d5.ToString(), " %") : "-", "right"));
                    double d6 = SQLColumn.GetValid(dataRow1["NEUER_LOHN"], 0.0);
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "NEUER_LOHN")));
                    xmlNode2.AppendChild(base.createDetailValueCell(base.getPartSalary(d6).ToString("c"), "right"));
                    if (j1 == 1)
                    {
                        d3 = d6 / i2;
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d3.ToString("c"), "right"));
                    }
                    else
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d6.ToString("c"), "right"));
                    }
                    double d7 = d6 - d1;
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "SALAIRE")));
                    xmlNode2.AppendChild(base.createDetailValueCell(base.getPartSalary(d7).ToString("c"), "right"));
                    if (j1 == 1)
                    {
                        d3 = d7 / i2;
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d3.ToString("c"), "right"));
                    }
                    else
                    {
                        xmlNode4 = xmlNode2.AppendChild(base.createDetailValueCell(d7.ToString("c"), "right"));
                    }
                    double d8 = Math.Round(d7 / d1 * 100.0, 2);
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("tpc", "salaryChangePercentage")));
                    xmlNode2.AppendChild(base.createDetailValueCell((!Double.IsNaN(d8) && !Double.IsInfinity(d8)) ? String.Concat(d8.ToString(), " %") : "-", "right"));
                    double d9 = Math.Round(d6 / d2 * 100.0, 2);
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("tpc", "salaryPercentageOfProposed")));
                    xmlNode2.AppendChild(base.createDetailValueCell((!Double.IsNaN(d9) && !Double.IsInfinity(d9)) ? String.Concat(d9.ToString(), " %") : "-", "right"));
                    dataColumn1 = dataTable.Columns["KOMMENTARS"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "KOMMENTARS")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], true), 2));
                    double d10 = SQLColumn.GetValid(dataRow1["PRIME"], 0.0);
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "PRIME")));
                    xmlNode2.AppendChild(base.createDetailValueCell(d10.ToString("c"), "right"));
                    dataColumn1 = dataTable.Columns["KOMMENTARP"];
                    xmlNode2 = xmlNode1.AppendChild(base.createRow());
                    xmlNode2.AppendChild(base.createDetailLabelCell(_map.get("TPC_LOHNDATEN_V", "KOMMENTARP")));
                    xmlNode2.AppendChild(base.createDetailValueCell(_db.GetDisplayValue(dataColumn1, dataRow1[dataColumn1], true), 2));
                }
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
            }
        }
    }
}
