using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.RSR;
using ch.appl.psoft.TPC;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web.SessionState;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// DLA - dezentrale Lohnanpassung - Teil im dezentralen Lohnmodul, welcher nur via
    /// Kundenmodule (zB rsr) zugänglich ist.
    /// 
    /// Aktuelle Zugriffsregelung für DLA:
    ///  - Nur Administratoren steht Administrationsmenü zur Verfügung
    ///  - Die Chefs haben nur auf ihre Daten und auf die Daten der untergeordneten OEs
    ///    Zugriff.
    /// 
    /// Page-Klassen (Namen und Strukturierung von Psoft 2 übernommen):
    ///  - Main         OE-Liste (eigene oder die untergeordneten einer OE)
    ///                 Eigene werden nur angezeigt, wenn es mehr als eine sind,
    ///                 sonst wird BudgetAllocation, SalaryAdjustment, Approvement 
    ///                 bzw BudgetCheck geöffnet
    ///  - BudgetAllocation Budgetverteilung. 
    ///  - SalaryAdjustment Übersicht der Lohndaten. 
    ///                 Multifunktional: Eingabe der Massnahmen und Kommentare
    ///                 oder reine Information mit Excel-Output-Möglichkeit
    ///  - Approvement  Genehmigung 
    ///  - BudgetCheck  Budgetkontrolle 
    ///  - PersonDetail Anzeige der History
    ///  - CreatePeronDetail Drucken der Lohndaten
    ///  - BudgetDoneCheck Prompt fürs Abschliessen der Lohnmassnahme
    ///  - ShowReport   Anzeigen des Excel-Outputs
    ///  - Admin        Administration der Stati
    ///  - ImportExport Import- und Exportmenü
    ///  
    ///  Kundenspezialitäten:
    ///  Pro DLA-Kunde gibt es ein Modul mit wie üblich einer Bezeichnung (KundenModuleName).
    ///  Die DLA-Funktionalitäten verhalten sich kundenspezifisch auf Grund dieser Bezeichnung,
    ///  welche in der Session festgehalten wird. Dh jedesmal, wenn DLA zum Einsatz kommt,
    ///  muss
    ///     KundenModuleName
    ///  gesetzt werden.
    ///  
    /// Summationen über Hierarchie:
    /// In DBObjects.Tree ist ein allgemeines Summieren implementiert. Der delegate Tree.NodeValue
    /// wird fürs Bestimmen des zu summierenden Wertes pro Knoten verwendet. Diese werden hier
    /// implementiert (Aufwand, BudgetVerfuegbar, NumberOfBerechtigte, PositiveAbweichungSum)
    /// und beim Aufruf von Orgentity.GetHierarchicOrgentityTreeSum() übergeben
    /// </summary>
    public class LohnModule : psoftModule 
    {
        public const string ModuleName = "lohn";
        private static HttpSessionState _session = null; // Fürs Ablegen von DLA-Session-Informationen
        private static CultureInfo _userCulture = new CultureInfo("de-DE", true); // Kultur für OGS-Import/Export

        static LohnModule()
        {
            _userCulture = new CultureInfo(Global.Config.languageCode + "-" + Global.Config.regionCode);
        }

        public LohnModule()
        {
            m_NameMnemonic = ModuleName;
            m_IsVisible = false;
            m_SubNavMenuURL = "../Lohn/SubNavMenu.aspx";
        }

        /// <summary>
        /// Das Jahr wird aus der Hauptvariante genommen. Beim ersten Import eines Jahres
        /// muss es manuell korrigiert werden.
        /// </summary>
        public static string DefaultDBName
        {
            get 
            {   string configDBName = Global.Config.getModuleParam("lohn", "dbName", "");
                int year = GetSalaryYear();
                return configDBName.Replace("YYYY", "" + year);
            }
        }
        
        /// <summary>
        /// Budgetmodus gemäss config
        /// </summary>
        public static int BudgetModusConfig
        {
            get
            {
                int moduleParameter = Validate.GetValid(
                        Global.Config.getModuleParam("lohn", "budgetModus", ""),
                        (int)BudgetModus.budgetImport
                    );

                switch (moduleParameter)
                {
                    case (int)BudgetModus.budgetImportSofortigeFreigabe:
                    case (int)BudgetModus.budgetImport:
                    case (int)BudgetModus.budgetOhneImport:
                    case (int)BudgetModus.budgetAusBedarf:
                        return moduleParameter;
                    default:
                        return (int)BudgetModus.ohneBudget;
                }
            }
        }

        /// <summary>
        /// Genehmigungsverfahren ja/nein gemäss config
        /// </summary>
        public static bool MitGenehmigungsverfahren
        {
            get
            {
                return Global.Config.getModuleParam(
                        "lohn",
                        "mitGenehmigungsverfahren",
                        "1"
                    ) == "1";
            }
        }

        /// <summary>
        /// Liefert die Id's für die lohnrelevanten Komponenten der Variante
        /// </summary>
        /// <param name="varianteId"></param>
        /// <returns></returns>
        public static string GetSalaryKomponenteIdListForSQL(long varianteId)
        {
            string returnValue = "";
            Transfer transfer = getNewTransfer(KundenModuleName);

            if (transfer != null)
            {
                DBData db = DBData.getDBData();
                db.connect();

                try
                {
                    for (int counter = 0; counter < transfer.SalaryComponentList.Length; counter++)
                    {
                        returnValue += (counter == 0 ? "" : ", ");
                        returnValue += db.lookup(
                                "ID",
                                "KOMPONENTE",
                                "VARIANTE_ID = " + varianteId
                                    + " and BEZEICHNUNG = '" + transfer.SalaryComponentList[counter] + "'",
                                "0"
                            );
                    }
                }
                finally
                {
                    db.disconnect();
                }
            }

            return returnValue;
        }

        public static int GetSalaryYear()
        {
            int returnValue = DateTime.Now.Year + 1;
            DBData db = DBData.getDBData();
            db.connect();

            try
            {
                returnValue = DBColumn.GetValid(
                        db.lookup(
                            "datepart(yyyy, LR_GUELTIG_AB)",
                            "VARIANTE_LOHNRUNDE_V",
                            "HAUPTVARIANTE = 1"
                        ),
                        returnValue
                    );
            }
            finally
            {
                db.disconnect();
            }

            return returnValue;
        }

        /// <summary>
        /// Nur in HttpSession verwendbar!
        /// </summary>
        public static string KundenModuleName
        {
            get {return SessionData.getStringValue(_session, "DLAKundenModuleName");}
            set
            {
                // HACK: Session neu erstellen wenn nicht vorhanden
                if (_session == null)
                {
                    _session = System.Web.HttpContext.Current.Session;
                }
                _session["DLAKundenModuleName"] = value;
            }
        }

        /// <summary>
        /// Nur in HttpSession verwendbar!
        /// </summary>
        public static string [] EditableComponentList
        {
            get
            {
                Transfer transfer = getNewTransfer(KundenModuleName);

                if (transfer == null)
                {
                    return new string [] {};
                }
                else
                {
                    return transfer.EditableComponentList;
                }
            }
        }

        /// <summary>
        /// Bezeichnungen der für den Lohn relevanten Komponenten
        /// Nur in HttpSession verwendbar!
        /// </summary>
        public static Hashtable SalaryComponentTable
        {
            get
            {
                Hashtable returnValue = new Hashtable();
                Transfer transfer = getNewTransfer(KundenModuleName);

                if (transfer != null)
                {
                    for (int counter = 0; counter < transfer.SalaryComponentList.Length; counter++)
                    {
                        returnValue.Add(transfer.SalaryComponentList[counter], "");
                    }
                 }

                return returnValue;
            }
        }

        /// <summary>
        /// Adäquate Import/Export-Klasse
        /// null, falls keine definiert ist
        /// </summary>
        /// <param name="kundenModuleName"></param>
        /// <returns></returns>
        public static Transfer getNewTransfer(string kundenModuleName) {
            return getNewTransfer(kundenModuleName,null);
        }

        /// <summary>
        /// Adäquate Import/Export-Klasse
        /// null, falls keine definiert ist
        /// </summary>
        /// <param name="kundenModuleName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Transfer getNewTransfer(string kundenModuleName, DBData db) {
            Transfer returnValue = null;

            switch (kundenModuleName) {
            case "rsr":
                returnValue = (db == null ? new RSRTransfer() : new RSRTransfer(db));
                break;
            case "tpc":
                returnValue = (db == null ? new TPCTransfer() : new TPCTransfer(db));
                break;
            }

            returnValue.DBColumnUserCulture = _userCulture;
            return returnValue;
        }

        /// <summary>
        /// Bearbeitungsstatus des Budgets.
        /// </summary>
        public enum EditingState
        {
            Pending = 0, // ausstehend
            Done = 1,    // abgeschlossen
            Approved = 2 // genehmigt
        }

        /// <summary>
        /// Freigabestatus des Budgets.
        /// </summary>
        public enum ClearingState
        {
            Disabled = 0,            // gesperrt
            allocationEnabled = 1,   // verteilung freigegeben
            Enabled = 2              // freigegeben
        }

        /// <summary>
        /// salaryKind gemäss OGS-Schnittstelle
        /// </summary>
        public enum SalaryKind
        {
            annualWage = 0,
            monthlyWage = 1,
            hourlyWage = 2,
            dailyWage = 3
        }

        public enum BudgetModus
        {
            ohneBudget = 0,
            budgetImportSofortigeFreigabe = 1,
            budgetImport = 2,
            budgetOhneImport = 3,
            budgetAusBedarf = 4
        }

//        public override void refreshAccessRightsTable(SQLDB db)
//        {
//            DBData.AUTHORISATION [] allTableAuthorisation = 
//            {
//                DBData.AUTHORISATION.READ,
//                DBData.AUTHORISATION.INSERT,
//                DBData.AUTHORISATION.UPDATE,
//                DBData.AUTHORISATION.DELETE,
//                DBData.AUTHORISATION.ADMIN
//            };
//
//            // public rights
//            db.grantTableAuthorisations(allTableAuthorisation, Global.ACCESSORGROUP.ALL, "EMP");
//            db.grantTableAuthorisations(allTableAuthorisation, Global.ACCESSORGROUP.ALL, "BUDG");
//            db.grantTableAuthorisations(allTableAuthorisation, Global.ACCESSORGROUP.ALL, "ORGBUD");
//            db.grantTableAuthorisations(allTableAuthorisation, Global.ACCESSORGROUP.ALL, "EMPPER");
//        }
//                
//        public override void refreshAccessRights(DBData db) 
//        {
//            // application rights
//            db.grantApplicationAuthorisation(DBData.AUTHORISATION.EXECUTE, Global.ACCESSORGROUP.ADMINISTRATORS, Global.APPLICATIONRIGHT.ADMINISTER_LAP);
//        }

        public static string toCurrency(double dValue)
        {
            string retValue = "-";
                        
            if (!double.IsInfinity(dValue) && ! double.IsNaN(dValue))
            {
                retValue = dValue.ToString("c");
                
                if (retValue.IndexOf(" ") >= 0)
                    retValue = retValue.Substring(retValue.IndexOf(" ") + 1);
                else
                    retValue = retValue.Substring(retValue.IndexOf("-"));
            }
            
            return retValue;
        }

        /// <summary>
        /// Anzahl Lohnkomponenten zu Budget
        /// (Berechtigte sind gekennzeichnet durch eine Lohnkomponente zum Budget)
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="parameterList">{komponente-Id}</param>
        /// <returns></returns>
        private static double NumberOfBerechtigte(long orgentityId, params object[] parameterList)
        {
            double returnValue = 0;

            if (parameterList.Length > 0)
            {
                DBData db = DBData.getDBData();
                db.connect();

                try
                {
                    returnValue = DBColumn.GetValid(
                            db.lookup(
                                "count(*)",
                                "BUDGET B, LOHNKOMPONENTE L",
                                "B.ORGENTITY_ID = " + orgentityId
                                    + " and B.KOMPONENTE_ID = " + parameterList[0]
                                    + " and L.BUDGET_ID = B.ID"
                            ),
                            0
                        );
                }
                finally
                {
                    db.disconnect();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Anzahl Berechtigter bezogen auf Komponente (hierarchisch summiert)
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="komponenteId"></param>
        /// <returns></returns>
        public static double GetNumberOfBerechtigte(long orgentityId, long komponenteId)
        {
            return Orgentity.GetHierarchicOrgentityTreeSum(
                    orgentityId,
                    new Tree.NodeValue(NumberOfBerechtigte),
                    komponenteId
                );
        }

        /// <summary>
        /// Summe der positive Differenzen (nur Berechtigte berücksichtigt!)
        ///   Lohnvorschlag - Istlohn
        ///  (anders als beim Sternenhimmel definiert! Der Richtlohn wird aber vom Import nicht eingefüllt)
        /// für die OE, die Komponente und optional den Budgettyp
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="parameterList">{Komponente-Ids (lohnrelevant), Budgettyp-Id}</param>
        /// <returns></returns>
        private static double PositiveAbweichungSum(long orgentityId, params object[] parameterList)
        {
            double returnValue = 0;

            if (parameterList.Length > 0)
            {
                string select = "select L.LOHN_ID from LOHNKOMPONENTE L, BUDGET B"
                        + " where B.ORGENTITY_ID = " + orgentityId
                        + " and B.KOMPONENTE_ID in(" + parameterList[0] + ")"
                        + " and L.BUDGET_ID = B.ID";

                if (parameterList.Length > 1) // mit Budgettyp
                {
                    select += " and isnull(B.BUDGETTYP_ID, -1) = " + parameterList[1];
                }

                DBData db = DBData.getDBData();
                db.connect();

                try
                {
                    returnValue = DBColumn.GetValid(
                            db.lookup(
                                "sum(case when isnull(LOHNVORSCHLAG, 0) - isnull(ISTLOHN, 0) > 0"
                                    + " then isnull(LOHNVORSCHLAG, 0) - isnull(ISTLOHN, 0)"
                                    + " else 0"
                                    + " end" 
                                    + ")",
                                "LOHN",
                                "ID in(" + select + ")"
                            ),
                            (double)0
                        );
                }
                finally
                {
                    db.disconnect();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Summe der positive Differenzen
        /// für die OE, Budgettyp und Variante
        /// wahlweise hierarchisch summiert
        /// Nur Berechtigte für lohnrelevante Komponenten werden berücksichtigt
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="varianteId"></param>
        /// <param name="budgettypId">-1, falls ohne budgettyp</param>
        /// <param name="subOEIncluded"></param>
        /// <returns></returns>
        public static double GetPositiveAbweichung(
            long orgentityId, 
            long varianteId, 
            long budgettypId, 
            bool subOEsIncluded
        )
        {
            double returnValue = 0;
            string komponenteIdList = GetSalaryKomponenteIdListForSQL(varianteId);
            object [] parameterList;

            if (budgettypId == -1)
            {
                parameterList = new object [] {komponenteIdList};
            }
            else
            {
                parameterList = new object [] {komponenteIdList, budgettypId};
            }

            if (subOEsIncluded)
            {
                returnValue = Orgentity.GetHierarchicOrgentityTreeSum(
                        orgentityId,
                        new Tree.NodeValue(PositiveAbweichungSum),
                        parameterList
                    );
            }
            else
            {
                returnValue = PositiveAbweichungSum(orgentityId, parameterList);
            }
 
            return returnValue;
        }

        /// <summary>
        /// Verfügbares Budget der OE, der Komponente und optional des Budgettyp
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="parameterList">{komponente-Id, Budgettyp-Id}</param>
        /// <returns></returns>
        private static double BudgetVerfuegbar(long orgentityId, params object[] parameterList)
        {
            double returnValue = 0;

            if (parameterList.Length > 0)
            {
                DBData db = DBData.getDBData();
                db.connect();

                try
                {
                    returnValue = DBColumn.GetValid(
                            db.lookup(
                                "BETRAG",
                                "BUDGET",
                                "ORGENTITY_ID = " + orgentityId
                                    + " and KOMPONENTE_ID = " + parameterList[0]
                                    + (parameterList.Length < 2 
                                            ? ""
                                            : (" and BUDGETTYP_ID = " + parameterList[1])
                                        )
                            ),
                            (double)0
                        );
                }
                finally
                {
                    db.disconnect();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Verfügbares Budget
        /// wahlweise hierarchisch summiert
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="komponenteId"></param>
        /// <param name="budgettypId"></param>
        /// <param name="subOEsIncluded"></param>
        /// <returns></returns>
        public static double GetBudgetVerfuegbar(
            long orgentityId, 
            long komponenteId, 
            long budgettypId, 
            bool subOEsIncluded
        )
        {
            double returnValue = 0;
            object [] parameterList;

            if (budgettypId == -1)
            {
                parameterList = new object [] {komponenteId};
            }
            else
            {
                parameterList = new object [] {komponenteId, budgettypId};
            }

            if (subOEsIncluded)
            {
                returnValue = Orgentity.GetHierarchicOrgentityTreeSum(
                        orgentityId,
                        new Tree.NodeValue(BudgetVerfuegbar),
                        parameterList
                    );
            }
            else
            {
                returnValue = BudgetVerfuegbar(orgentityId, parameterList);
            }

            return returnValue;
        }

        /// <summary>
        /// Verbrauch des Budgets der OE, der Komponente und optional des Budgettyp
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="parameterList">{komponente-Id, Budgettyp-Id}</param>
        /// <returns></returns>
        private static double Aufwand(long orgentityId, params object[] parameterList)
        {
            double returnValue = 0;

            if (parameterList.Length > 0)
            {
                DBData db = DBData.getDBData();
                db.connect();

                try
                {
                    returnValue = DBColumn.GetValid(
                            db.lookup(
                                "isnull(sum(isnull(L.BETRAG, 0)), 0)",
                                "LOHNKOMPONENTE L, BUDGET B",
                                "B.ORGENTITY_ID = " + orgentityId
                                    + " and B.KOMPONENTE_ID = " + parameterList[0]
                                    + (parameterList.Length < 2 
                                            ? ""
                                            : (" and BUDGETTYP_ID = " + parameterList[1])
                                        )
                                    + " and L.BUDGET_ID = B.ID"
                            ),
                            (double)0
                        );
                }
                finally
                {
                    db.disconnect();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Verfügbares Budget
        /// wahlweise hierarchisch summiert
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="komponenteId"></param>
        /// <param name="budgettypId"></param>
        /// <param name="subOEsIncluded"></param>
        /// <returns></returns>
        public static double GetAufwand(
            long orgentityId, 
            long komponenteId, 
            long budgettypId, 
            bool subOEsIncluded
        )
        {
            double returnValue = 0;
            object [] parameterList;

            if (budgettypId == -1)
            {
                parameterList = new object [] {komponenteId};
            }
            else
            {
                parameterList = new object [] {komponenteId, budgettypId};
            }

            if (subOEsIncluded)
            {
                returnValue = Orgentity.GetHierarchicOrgentityTreeSum(
                        orgentityId,
                        new Tree.NodeValue(Aufwand),
                        parameterList
                    );
            }
            else
            {
                returnValue = Aufwand(orgentityId, parameterList);
            }

            return returnValue;
        }

        /// <summary>
        /// Summe der verteilbaren Budget der direkt unterstellten OE
        /// plus verfügbares der OE (orgentityId).
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="komponenteId"></param>
        /// <param name="budgettypId">-1, falls keine Budgettypen verwendet werden.</param>
        /// <returns></returns>
        public static double GetBudgetVerteilt(
            long orgentityId, 
            long komponenteId, 
            long budgettypId
        )
        {
            double returnValue = 0;
            DBData db = DBData.getDBData();
            db.connect();

            try
            {
                returnValue = DBColumn.GetValid(
                        db.lookup(
                            "BETRAG",
                            "BUDGET",
                            "ORGENTITY_ID = " + orgentityId
                                + " and KOMPONENTE_ID = " + komponenteId
                                + (budgettypId == -1
                                    ? ""
                                    : (" and isnull(BUDGETTYP_ID, -1) = " + budgettypId)
                                )
                        ),
                        returnValue
                    );
                returnValue += DBColumn.GetValid(
                        db.lookup(
                            "sum(isnull(B.BETRAG_VERTEILBAR, 0))",
                            "ORGENTITY O, BUDGET B",
                            "O.PARENT_ID = " + orgentityId
                                + " and B.ORGENTITY_ID = O.ID"
                                + " and B.KOMPONENTE_ID = " + komponenteId
                                + (budgettypId == -1
                                    ? ""
                                    : (" and isnull(B.BUDGETTYP_ID, -1) = " + budgettypId)
                                )
                        ),
                        (double)0
                    );
            }
            finally
            {
                db.disconnect();
            }

            return returnValue;
        }

        /// <summary>
        /// Hierarchisches Zurücksetzen des Freigabestatus
        /// Zurücksetzten bedeutet, nur grössere Werte werden, auf den angegebenen
        /// Wert gesetzt.
        /// Die angegebene OE ist nicht betroffen, sondern die unterstellten
        /// </summary>
        /// <param name="orgentityId"></param>
        /// <param name="komponenteId"></param>
        /// <param name="budgettypId">-1, falls kein Budgettyp verwendet wird</param>
        /// <param name="state"></param>
        /// <param name="hierarchic"></param>
        public static void resetClearingState(
            long orgentityId,
            long komponenteId,
            long budgettypId,
            int state,
            bool hierarchic
        )
        {
            if (state == (int)ClearingState.allocationEnabled
                || state == (int)ClearingState.Disabled
            )
            {
                DBData db = DBData.getDBData();
                db.connect();

                try
                {
                    DataTable table = db.getDataTable(
                            "select ID from ORGENTITY where PARENT_ID = " + orgentityId
                        );
                    
                    foreach (DataRow row in table.Rows)
                    {
                        db.execute(
                            "update BUDGET set FREIGABESTATUS = " + state
                                + " where ORGENTITY_ID = " + row[0]
                                + " and KOMPONENTE_ID = " + komponenteId
                                + " and BUDGETTYP_ID = " + budgettypId
                                + " and FREIGABESTATUS > " + state
                        );

                        if (hierarchic)
                        {
                            resetClearingState(
                                DBColumn.GetValid(row[0], (long)-1),
                                komponenteId,
                                budgettypId,
                                state,
                                true
                            );
                        }
                    }
                }
                finally
                {
                    db.disconnect();
                }
            }
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Lohn/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        /// <summary>
        /// Called when session starts.
        /// </summary>
        /// <param name="session">The new session</param>
        public override void Session_Start(HttpSessionState session)
        {
            session["DBAShowAllHierarchic"] = true;
            _session = session;

        }
    }
}
