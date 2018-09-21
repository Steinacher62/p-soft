using ch.appl.psoft.Lohn;
using ch.appl.psoft.Organisation;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;
namespace ch.appl.psoft.RSR
{
    public class RSRPersonDetailReport : PersonDetailReport
    {
        // Methods
        public RSRPersonDetailReport(HttpSessionState session)
            : base(session)
        {
        }

        public void createPersonDetailReport(long lohnId)
        {
            base._db.connect();
            try
            {
                DataTable table = base._db.getDataTableExt("select * from RSR_LOHNDATEN_V where ID = " + lohnId, new object[] { "RSR_LOHNDATEN_V" });
                string[] strArray = base._map.getEnum("lohn", "salaryType", true);
                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    long valid = SQLColumn.GetValid(row["VARIANTE_ID"], (long)(-1L));
                    int year = SQLColumn.GetValid(base._db.lookup("LR_GUELTIG_AB", "VARIANTE_LOHNRUNDE_V", "ID = " + valid), DateTime.Now.AddYears(1)).Year;
                    int num3 = SQLColumn.GetValid(base._db.lookup("V.LR_GUELTIG_AB", "LOHN L inner join VARIANTE_LOHNRUNDE_V V on L.VARIANTE_ID = V.ID", "V.HAUPTVARIANTE = 1 and L.ID = " + lohnId), DateTime.Now.AddYears(1)).Year;
                    double salary = SQLColumn.GetValid(row["ISTLOHN"], (double)0.0);
                    double num5 = SQLColumn.GetValid(row["LOHNVORSCHLAG"], (double)0.0);
                    long num6 = SQLColumn.GetValid(row["PERSON_ID"], (long)(-1L));
                    string header = base._db.Person.getWholeName(num6.ToString(), false) + " - " + base._map.get("rsr", "salarySituation").Replace("#1", String.Concat(year));
                    base.writeHeaderAndFooter(header, DateTime.Now.ToString("d"));
                    base.appendVSpace(20);
                    XmlNode node = base._rootNode.AppendChild(base.createTable());
                    node.Attributes.Append(base.createAttribute("widths", "150,175,175"));
                    node.Attributes.Append(base.createAttribute("keep-together", "true"));
                    node.Attributes.Append(base.createAttribute("border-width-inner", "0"));
                    node.Attributes.Append(base.createAttribute("border-width-outer", "0"));
                    node.Attributes.Append(base.createAttribute("padding-all", "2"));
                    node.Attributes.Append(base.createAttribute("align", "left"));
                    XmlNode node2 = null;
                    XmlNode node3 = null;
                    if (year != num3)
                    {
                        header = base._map.get("rsr", "situation").Replace("#1", String.Concat(num3));
                        node.AppendChild(base.createRow()).AppendChild(base.createCell(base.createClassAttribute("title"), header, 2, "")).Attributes.Append(base.createAttribute("padding-top", "10"));
                    }
                    DataColumn column = table.Columns["COSTCENTER_NUMBER"];
                    DataColumn column2 = table.Columns["COSTCENTER_DESCRIPTION"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "COSTCENTER_NUMBER")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], false) + " - " + base._db.GetDisplayValue(column2, row[column2], false), 2));
                    column = table.Columns["EMPLOYMENTKIND"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "EMPLOYMENTKIND")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], false), 2));
                    column = table.Columns["EMPLOYMENTTYP"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "EMPLOYMENTTYP")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], false), 2));
                    column = table.Columns["ENTRY"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "ENTRY")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], false), 2));
                    column = table.Columns["AGE"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "AGE")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], false), 2));
                    column = table.Columns["SUPERIOR"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "SUPERIOR")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], false), 2));
                    long employmentId = SQLColumn.GetValid(row["EMPLOYMENT_ID"], (long)(-1L));
                    DataRow row1 = table.Rows[0];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("FUNKTION", base._db.langAttrName("FUNKTION", "TITLE", new string[0]))));
                    object[] objArray = base._db.lookup(new string[] { "F.EXTERNAL_REF", "F." + base._db.langAttrName("FUNKTION", "TITLE", new string[0]) }, "FUNKTION F inner join JOB J on F.ID = J.FUNKTION_ID", "J.ID = " + OrganisationModule.getMaximalPartJobId(base._db, employmentId, false));
                    string innerText = "";
                    if (objArray[0] != null)
                    {
                        innerText = objArray[0].ToString().TrimStart(new char[] { '0' }) + " - ";
                    }
                    innerText = innerText + SQLColumn.GetValid(objArray[1], "");
                    node2.AppendChild(base.createDetailValueCell(innerText, 2));
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("FUNKTION", base._db.langAttrName("FUNKTION", "TITLE", new string[0]) + "_REP")));
                    objArray = base._db.lookup(new string[] { "F.EXTERNAL_REF", "F." + base._db.langAttrName("FUNKTION", "TITLE", new string[0]) }, "FUNKTION F inner join JOB J on F.ID = J.REPFUNKTION_ID", "J.ID = " + OrganisationModule.getMaximalPartJobId(base._db, employmentId, false));
                    innerText = "";
                    if (objArray[0] != null)
                    {
                        innerText = objArray[0].ToString().TrimStart(new char[] { '0' }) + " - ";
                    }
                    innerText = innerText + SQLColumn.GetValid(objArray[1], "");
                    node2.AppendChild(base.createDetailValueCell(innerText, 2));
                    if (year != num3)
                    {
                        header = base._map.get("rsr", "salarySituation").Replace("#1", String.Concat(year));
                        node.AppendChild(base.createRow()).AppendChild(base.createCell(base.createClassAttribute("title"), header, 2, "")).Attributes.Append(base.createAttribute("padding-top", "10"));
                    }
                    column = table.Columns["REPRICHTLOHN"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "REPRICHTLOHN")));
                    node2.AppendChild(base.createDetailValueCell(SQLColumn.GetValid(row[column], (double)0.0).ToString("c"), "right"));
                    column = table.Columns["INDEMNITE_DE_FONCTION"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "INDEMNITE_DE_FONCTION")));
                    node2.AppendChild(base.createDetailValueCell(SQLColumn.GetValid(row[column], (double)0.0).ToString("c"), "right"));
                    base._salaryDivisor = 1.0;
                    int index = SQLColumn.GetValid(row["LOHNART"], 1);
                    int dflt = SQLColumn.GetValid(base._db.lookup("L.ANZAHL_MONATSLOEHNE", "LOHNRUNDE L inner join VARIANTE V on L.ID = V.LOHNRUNDE_ID", "V.ID = " + valid), 1);
                    int num10 = SQLColumn.GetValid(row["ANZAHL_LOEHNE"], dflt);
                    int num11 = SQLColumn.GetValid(row["ANZAHL_STUNDEN"], 1);
                    int num12 = SQLColumn.GetValid(row["ANZAHL_TAGE"], 1);
                    column = table.Columns["ENGAGEMENT"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "ENGAGEMENT")));
                    node3 = node2.AppendChild(base.createDetailValueCell("", "right"));
                    XmlNode node4 = null;
                    if (index > 1)
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell("", "right"));
                    }
                    switch (index)
                    {
                        case 1:
                            base._salaryDivisor = 1.0;
                            node3.InnerText = base._db.GetDisplayValue(column, row["ENGAGEMENT"], false) + " %";
                            break;

                        case 2:
                            base._salaryDivisor = num10;
                            node3.InnerText = base._db.GetDisplayValue(column, row["ENGAGEMENT"], false) + " %";
                            break;

                        case 3:
                            base._salaryDivisor = num12;
                            node4.InnerText = base._db.GetDisplayValue(column, row["ENGAGEMENT"], false) + " %";
                            node3.InnerText = num12 + " " + base._map.get("lohn", "days");
                            break;

                        case 4:
                            base._salaryDivisor = num11;
                            node4.InnerText = num12 + " " + base._map.get("lohn", "days");
                            node3.InnerText = num11 + " " + base._map.get("lohn", "hours");
                            break;
                    }
                    column = table.Columns["FORFAIT_B"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "FORFAIT_B")));
                    node2.AppendChild(base.createDetailValueCell(SQLColumn.GetValid(row[column], (double)0.0).ToString("c"), "right"));
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "LOHNART")));
                    node2.AppendChild(base.createDetailValueCell(strArray[index], "right"));
                    if (index == 1)
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(strArray[2], "right"));
                    }
                    else
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(strArray[1], "right"));
                    }
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "ISTLOHN")));
                    node2.AppendChild(base.createDetailValueCell(base.getPartSalary(salary).ToString("c"), "right"));
                    double num13 = 0.0;
                    if (index == 1)
                    {
                        num13 = salary / ((double)num10);
                        node4 = node2.AppendChild(base.createDetailValueCell(num13.ToString("c"), "right"));
                    }
                    else
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(salary.ToString("c"), "right"));
                    }
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "LOHNVORSCHLAG")));
                    node2.AppendChild(base.createDetailValueCell(base.getPartSalary(num5).ToString("c"), "right"));
                    if (index == 1)
                    {
                        num13 = num5 / ((double)num10);
                        node4 = node2.AppendChild(base.createDetailValueCell(num13.ToString("c"), "right"));
                    }
                    else
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(num5.ToString("c"), "right"));
                    }
                    double num14 = num5 - salary;
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("rsr", "proposedSalaryDifference")));
                    node2.AppendChild(base.createDetailValueCell(base.getPartSalary(num14).ToString("c"), "right"));
                    if (index == 1)
                    {
                        num13 = num14 / ((double)num10);
                        node4 = node2.AppendChild(base.createDetailValueCell(num13.ToString("c"), "right"));
                    }
                    else
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(num14.ToString("c"), "right"));
                    }
                    double d = Math.Round((double)((num14 / num5) * 100.0), 2);
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("rsr", "proposedSalaryPercentageDifference")));
                    node2.AppendChild(base.createDetailValueCell((double.IsNaN(d) || double.IsInfinity(d)) ? "-" : (d.ToString() + " %"), "right"));
                    double num16 = SQLColumn.GetValid(row["NEUER_LOHN"], (double)0.0);
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "NEUER_LOHN")));
                    node2.AppendChild(base.createDetailValueCell(base.getPartSalary(num16).ToString("c"), "right"));
                    if (index == 1)
                    {
                        num13 = num16 / ((double)num10);
                        node4 = node2.AppendChild(base.createDetailValueCell(num13.ToString("c"), "right"));
                    }
                    else
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(num16.ToString("c"), "right"));
                    }
                    double num17 = num16 - salary;
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "SALAIRE")));
                    node2.AppendChild(base.createDetailValueCell(base.getPartSalary(num17).ToString("c"), "right"));
                    if (index == 1)
                    {
                        num13 = num17 / ((double)num10);
                        node4 = node2.AppendChild(base.createDetailValueCell(num13.ToString("c"), "right"));
                    }
                    else
                    {
                        node4 = node2.AppendChild(base.createDetailValueCell(num17.ToString("c"), "right"));
                    }
                    double num18 = Math.Round((double)((num17 / salary) * 100.0), 2);
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("rsr", "salaryChangePercentage")));
                    node2.AppendChild(base.createDetailValueCell((double.IsNaN(num18) || double.IsInfinity(num18)) ? "-" : (num18.ToString() + " %"), "right"));
                    double num19 = Math.Round((double)((num16 / num5) * 100.0), 2);
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("rsr", "salaryPercentageOfProposed")));
                    node2.AppendChild(base.createDetailValueCell((double.IsNaN(num19) || double.IsInfinity(num19)) ? "-" : (num19.ToString() + " %"), "right"));
                    column = table.Columns["KOMMENTARS"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "KOMMENTARS")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], true), 2));
                    double num20 = SQLColumn.GetValid(row["PRIME"], (double)0.0);
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "PRIME")));
                    node2.AppendChild(base.createDetailValueCell(num20.ToString("c"), "right"));
                    column = table.Columns["KOMMENTARP"];
                    node2 = node.AppendChild(base.createRow());
                    node2.AppendChild(base.createDetailLabelCell(base._map.get("RSR_LOHNDATEN_V", "KOMMENTARP")));
                    node2.AppendChild(base.createDetailValueCell(base._db.GetDisplayValue(column, row[column], true), 2));
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception, Logger.ERROR);
            }
            finally
            {
                base._db.disconnect();
            }
        }
    }
}