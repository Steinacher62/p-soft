using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;

namespace ch.appl.psoft.Organisation
{
    /// <summary>
    /// Summary description for OrganisationModule.
    /// </summary>
    public class OrganisationModule : psoftModule {
        public OrganisationModule() : base() {
            m_NameMnemonic = "organisation";
            m_SubNavMenuURL = "../Organisation/SubNavMenu.aspx";
            m_IsVisible = Global.Config.organisationModuleVisible;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Organisation/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        /// <summary>
        /// Anteile (als Dezimalbruch) der Jobs zu einer Anstellung (employmentID) gemäss
        /// allgemeiner Regel dh auf Grund der Angaben
        /// 
        /// - organisation.hauptfunktion_verwenden: ja (= 1)/ nein 
        /// - organisation.hauptfunktion_automatisch:
        ///     0 gemäss job.hauptfunktion 
        ///     1 mit grösstem Beschäftigungsgrad (job.engagement)
        ///     2 mit grösstem Funktionswert
        /// - job.hauptfunktion: ja (= 1)/ nein
        /// - job.engagement
        /// 
        /// Annahme: alle Jobs einer Anstellung gehören zur gleichen Organisation!!
        /// </summary>
        /// <param name="db">DBData-Object für DB-Zugriff</param>
        /// <param name="employmentID"></param>
        /// <returns></returns>
        public static Hashtable jobParts(DBData db, long employmentID) {
            Hashtable returnTable = new Hashtable();
            int hauptfunktionVerwenden = -1;
            int hauptfunktionAutomatisch = -1;
            int hauptfunktion = -1;
            long jobID = -1;
            double maxValue = 0;
            double part = 0;
            double funktionswert = -1;
            double partSum = 0;

            try {
                db.connect();

                int numberOfJobs = DBColumn.GetValid(
                    db.lookup(
                    "count(*)",
                    "JOB",
                    "EMPLOYMENT_ID = " + employmentID
                    ),
                    0
                    );

                if (numberOfJobs == 1) { // nur einen Job! Dieser ist Hauptfunktion
                    jobID = DBColumn.GetValid(
                        db.lookup(
                        "ID",
                        "JOB",
                        "EMPLOYMENT_ID = " + employmentID
                        ),
                        (long)-1
                        );
					
                    if (jobID > -1) {
                        part = 1;
                        returnTable.Add(jobID, part);
                    }
                }
                else {
                    string sql = "select JOB.ID,"
                        + " JOB.HAUPTFUNKTION,"
                        + " JOB.ENGAGEMENT,"
                        + " O.HAUPTFUNKTION_VERWENDEN,"
                        + " O.HAUPTFUNKTION_AUTOMATISCH"
                        + " from ORGANISATION O, ORGENTITY OE, JOB"
                        + " where JOB.EMPLOYMENT_ID = " + employmentID
                        + " and OE.ID = JOB.ORGENTITY_ID"
                        + " and O.ID = OE.ROOT_ID";
                    DataTable table = db.getDataTable(sql, Logger.DEBUG);
				
                    foreach (DataRow row in table.Rows) {
                        if (hauptfunktionVerwenden == -1) {
                            hauptfunktionVerwenden = DBColumn.GetValid(
                                row["HAUPTFUNKTION_VERWENDEN"],
                                0
                                );
                            hauptfunktionAutomatisch = DBColumn.GetValid(
                                row["HAUPTFUNKTION_AUTOMATISCH"],
                                0
                                );

                            // gegebenenfalls Maximum bestimmen
                            if (hauptfunktionVerwenden == 1) {
                                switch (hauptfunktionAutomatisch) {
                                case 1:
                                    maxValue = DBColumn.GetValid(
                                        db.lookup(
                                        "max(ENGAGEMENT)",
                                        "JOB",
                                        "EMPLOYMENT_ID = " + employmentID.ToString()
                                        ),
                                        (double)0
                                        );
                                    break;
                                case 2:
                                    maxValue = DBColumn.GetValid(
                                        db.lookup(
                                        "max(F.FUNKTIONSWERT)",
                                        "JOB inner join FUNKTIONSBEWERTUNG F"
                                        + " on JOB.FUNKTION_ID = F.FUNKTION_ID",
                                        "EMPLOYMENT_ID = " + employmentID
                                        + " and isnull(GUELTIG_BIS, getdate())"
                                        + " >= dateadd(Day, -1, getdate())"
                                        ),
                                        (double)0
                                        );
                                    break;
                                default: // nicht automatisch
                                    break;
                                }
                            }
                        }

                        jobID = DBColumn.GetValid(row["ID"], (long)-1);

                        if (jobID > -1) {
                            if (hauptfunktionVerwenden == 1) {
                                switch (hauptfunktionAutomatisch) {
                                case 1:
                                    if (hauptfunktion == 1) { // höchstens 1 Hauptfunktion
                                        hauptfunktion = 0;
                                    }
                                    else {
                                        part = DBColumn.GetValid(
                                            row["ENGAGEMENT"],
                                            (double)0
                                            );
										
                                        if (part < maxValue) {
                                            hauptfunktion = 0;
                                        }
                                        else {
                                            hauptfunktion = 1;
                                        }
                                    }

                                    returnTable.Add(jobID, hauptfunktion);
                                    break;
                                case 2:
                                    if (hauptfunktion == 1) { // höchstens 1 Hauptfunktion
                                        hauptfunktion = 0;
                                    }
                                    else {
                                        funktionswert = DBColumn.GetValid(
                                            db.lookup(
                                            "max(FUNKTIONSWERT)",
                                            "JOB inner join FUNKTIONSBEWERTUNG F"
                                            + " on JOB.FUNKTION_ID = F.FUNKTION_ID",
                                            "JOB.ID = " + jobID
                                            + " and isnull(GUELTIG_BIS, getdate())"
                                            + " >= dateadd(Day, -1, getdate())"
                                            ),
                                            (double)0
                                            );
										
                                        if (funktionswert < maxValue) {
                                            hauptfunktion = 0;
                                        }
                                        else {
                                            hauptfunktion = 1;
                                        }
                                    }

                                    returnTable.Add(jobID, hauptfunktion);
                                    break;
                                default: // nicht automatisch
                                    if (hauptfunktion == 1) { // höchstens 1 Hauptfunktion
                                        hauptfunktion = 0;
                                    }
                                    else {
                                        hauptfunktion = DBColumn.GetValid(
                                            row["HAUPTFUNKTION"],
                                            (int)0
                                            );
                                    }

                                    returnTable.Add(jobID, hauptfunktion);
                                    break;
                                }
                            }
                            else {
                                part = DBColumn.GetValid(
                                    row["ENGAGEMENT"],
                                    (double)0
                                    );
                                partSum += part;

                                returnTable.Add(jobID, part);
                            }
                        }
                    }

                    if (hauptfunktionVerwenden != 1 && partSum > 0) {
                        Hashtable tempTable = returnTable;
                        returnTable = new Hashtable();

                        foreach (object key in tempTable.Keys) {
                            part = Validate.GetValid(tempTable[key].ToString(), (double)0)
                                / partSum;
                            returnTable.Add(key, part);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Logger.Log(ex, Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

            return returnTable;
        }

        /// <summary>
        /// Wahr, falls bei allen Anstellungen (employments) der Organisation
        /// in jobParts() die Summe 1 ist
        /// </summary>
        /// <param name="db">DBData-Object für DB-Zugriff</param>
        /// <param name="organisationID"></param>
        /// <returns></returns>
        public static bool jobPartsAreDeterminable(DBData db, long organisationID) {
            bool returnValue = true;
            object jobID = null;

            try {
                db.connect();

                object[] values = db.lookup(
                    new string[] {
                                "HAUPTFUNKTION_VERWENDEN",
                                "HAUPTFUNKTION_AUTOMATISCH"
                            },
                    "ORGANISATION",
                    "ID = " + organisationID.ToString()
                    );
                int hauptfunktionVerwenden = DBColumn.GetValid(values[0], 0);
                int hauptfunktionAutomatisch = DBColumn.GetValid(values[1], 0);

                string sql = "select JOB.EMPLOYMENT_ID, count(*) ANZAHL"
                    + " from JOB, ORGENTITY"
                    + " where ORGENTITY.ROOT_ID = " + organisationID
                    + " and JOB.ORGENTITY_ID = ORGENTITY.ID"
                    + " and JOB.EMPLOYMENT_ID is not null"
                    + " group by JOB.EMPLOYMENT_ID";
                DataTable table = db.getDataTable(sql, Logger.DEBUG);
				
                foreach (DataRow row in table.Rows) {
                    // mehr als einen Job
                    if (DBColumn.GetValid(row[1], 0) > 1) {
                        if (hauptfunktionVerwenden == 1) {
                            switch (hauptfunktionAutomatisch) {
                            case 1:
                                jobID = db.lookup(
                                    "ID",
                                    "JOB",
                                    "EMPLOYMENT_ID = " + row[0].ToString()
                                    + " and ENGAGEMENT > 0",
                                    false
                                    );
                                break;
                            case 2:
                                jobID = db.lookup(
                                    "ID",
                                    "JOB inner join FUNKTIONSBEWERTUNG F"
                                    + " on JOB.FUNKTION_ID = F.FUNKTION_ID",
                                    "EMPLOYMENT_ID = " + row[0].ToString()
                                    + " and F.FUNKTIONSWERT > 0"
                                    + " and isnull(GUELTIG_BIS, getdate())"
                                    + " >= dateadd(Day, -1, getdate())",
                                    false
                                    );
                                break;
                            default: // nicht automatisch
                                jobID = db.lookup(
                                    "ID",
                                    "JOB",
                                    "EMPLOYMENT_ID = " + row[0].ToString()
                                    + " and HAUPTFUNKTION = 1",
                                    false
                                    );
                                break;
                            }
                        }
                        else {
                            jobID = db.lookup(
                                "ID",
                                "JOB",
                                "EMPLOYMENT_ID = " + row[0].ToString()
                                + " and ENGAGEMENT > 0",
                                false
                                );
                        }

                        if (DBColumn.IsNull(jobID)) {
                            returnValue = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                Logger.Log(ex, Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

            return returnValue;
        }
    
        /// <summary>
        /// Bestimmt zu einer Anstellung, die Id der OE, welche gemäss Regel für den Lohn
        /// zuständig ist.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="employmentId"></param>
        /// <returns>orgentity_id für employment (zuständig für Lohn); -1, falls Regel versagt.</returns>
        public static long getStandardOrgentityId(DBData db, long employmentId) {
            long returnValue = -1;
		
            long jobId = getMaximalPartJobId(db, employmentId, true);
		
            if (jobId != -1) {
                returnValue = getOrgentityIdWithBudget(db, jobId);
            }
		
            return returnValue;
        }
    
        /// <summary>
        /// Stellt fest, ob die Zuständigkeit für Lohn einer Anstellung gemäss
        /// Regel ist.
        /// Falls zuständig für Lohn leer oder die Regel nicht eindeutig ist, wird
        /// true zurückgegeben
        /// ACHTUNG: Einsatz nur, falls employment.Orgentity_id vorhanden ist
        /// </summary>
        /// <param name="db"></param>
        /// <param name="employmentId"></param>
        /// <returns></returns>
        public static bool orgentityIdIsStandard(DBData db, long employmentId) {
            long orgentityId = DBColumn.GetValid(
                db.lookup(
                "ORGENTITY_ID",
                "EMPLOYMENT",
                "ID = " + employmentId
                ),
                (long)-1
                );
            long orgentityIdStandard = getStandardOrgentityId(db, employmentId);
		
            return orgentityId == -1 
                || orgentityIdStandard == -1
                || orgentityId == orgentityIdStandard;
        }
    
        /// <summary>
        /// Bestimmt Id des Jobs mit maximalem Anteil
        /// Falls unique = true, wird -1 zurückgegeben, falls der Job nicht eindeutig
        /// ist.
        /// Falls unique = false, wird irgendeine Job-Id mit maximalem Anteil zurück-
        /// gegeben.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="employmentId"></param>
        /// <param name="unique"></param>
        /// <returns>id des Jobs</returns>
        public static long getMaximalPartJobId(DBData db, long employmentId, bool unique) {
            long returnValue = -1;
		
            Hashtable jobPartList = jobParts(db, employmentId);
            long jobId = -1;
            double part = 0;
            double maximalPart = 0;

            foreach (object key in jobPartList.Keys) {
                part = Validate.GetValid(jobPartList[key].ToString(), 0.0);

                if (maximalPart < part) {
                    jobId = Validate.GetValid(key.ToString(), (long)-1);
                    maximalPart = part;
                }
            }
		
            if (jobId != -1) {
                if (unique) {
                    // Gibt es nur einen Job mit maximalem Anteil?
                    int count = 0;

                    foreach (object key in jobPartList.Keys) {
                        part = Validate.GetValid(jobPartList[key].ToString(), 0.0);

                        if (maximalPart <= part) {
                            count++;
                        }
                    }
        		
                    if (count == 1) {
                        returnValue = jobId;
                    }
                }
                else {
                    returnValue = jobId;
                }
            }
        
            return returnValue;
        }
    
        /// <summary>
        /// Bestimmt zu Job Id die OE mit Budget gemäss Regel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="jobId"></param>
        /// <returns>id der OE</returns>
        public static long getOrgentityIdWithBudget(DBData db, long jobId) {
            long returnValue = -1;

            if (jobId != -1) {
                // Start-OE bestimmen (Leiter ja oder nein)
                long startOrgentityId = -1;
                object[] values = db.lookup(
                    new string[] {"O.ID", "O.PARENT_ID", "TYP"},
                    "ORGENTITY O inner join JOB J"
                    + " on O.ID = J.ORGENTITY_ID",
                    "J.ID = " + jobId
                    );
                if (DBColumn.GetValid(values[2], 0) == 1 // Leiter
                    && !DBColumn.IsNull(values[1]) // es gibt eine Parent-OE
                    ) {
                    startOrgentityId = DBColumn.GetValid(values[1], (long)-1);
                }
                else {
                    startOrgentityId = DBColumn.GetValid(values[0], (long)-1);
                }
			
                returnValue = getNextOrgentityIdWithBudget(db, startOrgentityId);
            }

            return returnValue;
        }
    
        /// <summary>
        /// Bestimmt Id der nächsten OE hierarchieaufwärts mit Budget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgentityId"></param>
        /// <returns></returns>
        public static long getNextOrgentityIdWithBudget(DBData db, long orgentityId) {
            long returnValue = -1;
            object[] values = null;
        
            values = db.lookup(
                new string[] {"ID", "PARENT_ID", "HAT_BUDGET"},
                "ORGENTITY",
                "ID = " + orgentityId
                );

            if (DBColumn.GetValid(values[2], 0) == 1) { // hat Budget
                returnValue = DBColumn.GetValid(values[0], (long)-1);
            }
            else {
                // Noch nicht gefunden und PARENT_ID nicht NULL
                while (returnValue == -1 && !DBColumn.IsNull(values[1])) {
                    values = db.lookup(
                        new string[] {"ID", "PARENT_ID", "HAT_BUDGET"},
                        "ORGENTITY",
                        "ID = " + values[1]
                        );

                    if (DBColumn.GetValid(values[2], 0) == 1) { // hat Budget
                        returnValue = DBColumn.GetValid(values[0], (long)-1);
                    }
                }
            }
        
            if (returnValue == -1) { // noch nicht gefunden
                // zuletzt bestimmte ID, also jene der Root-OE
                returnValue = DBColumn.GetValid(values[0], (long)-1);
            }
        
            return returnValue;
        }

        /// <summary>
        /// Für die Anzeige der Funktionsbezeichnung in DLA
        /// mit Alternative Job- statt Funktionsbezeichnung 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="employmentId"></param>
        /// <param name="alternative">"": aus FUNKTION, "JOB": aus JOB</param>
        /// <returns></returns>
        public static string getFunktionTitle(DBData db, long employmentId, string alternative) {
            string returnValue = "";
            string titleAttribute;
            db.connect();

            try {
                long jobId = getMaximalPartJobId(db, employmentId, false);

                switch (alternative) {
                case "JOB":
                    titleAttribute = db.langAttrName("JOB", "TITLE");
                    returnValue = db.lookup(
                        titleAttribute,
                        "JOB",
                        "ID = " + jobId,
                        ""
                        );
                    break;
                default:
                    titleAttribute = db.langAttrName("FUNKTION", "TITLE");
                    returnValue = db.lookup(
                        "F." + titleAttribute,
                        "FUNKTION F inner join JOB J on F.ID = J.FUNKTION_ID",
                        "J.ID = " + jobId,
                        ""
                        );
                    break;
                }
            }
            finally {
                db.disconnect();
            }

            return returnValue;
        }

        /// <summary>
        /// Für die Anzeige der Funktion external ref in DLA
        /// mit Alternative Job- statt Funktionsbezeichnung 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="employmentId"></param>
        /// <param name="alternative"></param>
        /// <returns></returns>
        public static string getFunktionEXTREF(DBData db, long employmentId, string alternative) {
            string returnValue = "";
            db.connect();

            try {
                long jobId = getMaximalPartJobId(db, employmentId, false);

                switch (alternative) {
                case "JOB":
                    returnValue = db.lookup(
                        "EXTERNAL_REF",
                        "JOB",
                        "ID = " + jobId,
                        ""
                        );
                    break;
                default:
                    returnValue = db.lookup(
                        "F.EXTERNAL_REF",
                        "FUNKTION F inner join JOB J on F.ID = J.FUNKTION_ID",
                        "J.ID = " + jobId,
                        ""
                        );
                    break;
                }
            }
            finally {
                db.disconnect();
            }

            return returnValue;
        }
    }
}
