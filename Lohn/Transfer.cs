using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace ch.appl.psoft.Lohn
{
    public struct UserAtributeValue {
        public string _tableName;
        public string _columnName;
        public object _value;

        public UserAtributeValue(string tableName, string columnName, object value) {
            _tableName = tableName;
            _columnName = columnName;
            _value = value;
        }
    }

    public class TransferException : Exception {
        public TransferException(string message) : base (message) {
        }
    }

    /// <summary>
    /// Läd OGS-Salary-Daten in die DB und speichert in der DB erfasste Daten in der OGS-Datenbank
    /// via COM-Schnittstelle (dll). Sie wird in den konkreten Klasse mit verschiedenen dll's 
    /// instanziert
    /// 
    /// Bemerkung:
    /// Die Organisationsstruktur kann verändert werden. Die oberste OE ABER kann nicht ersetzt werden.
    /// 
    /// Laden:     OGS-Datenbank -> Psoft-Datenbank
    /// Speichern: Psoft-Datenbank -> OGS-Datenbank
    /// </summary>
    public abstract class Transfer {
        protected DBData _db;
        protected string _attributeLanguageCode
            = Global.Config.languageCode; // für sprachabhängige Attribute beim Einfüllen der Daten (update)
 
        private enum LoadPart {
            Person,
            OE,
            Budget,
            Salary,
            PreviousSalary
        }
        
        private Hashtable _editableComponentTable = null;
        private int _loadReturnValue = 0;
        private LoadPart _loadPart = LoadPart.Person;
        private int _salaryYear = -1;
        private bool _neueLohnrunde = false;
        
        // IDs für die Budget- und Lohndaten
        private long _rootId = -1;
        private Hashtable _varianteIdTable = null;
        private string _funktion_groupId = "null"; // Funktionen-Katalog-Id
        private Hashtable _komponenteIdTable = null;
        private Hashtable _budgettypIdTable = null;
        private Hashtable _funktionIdTable = null; // für Identifikation vorhandener Funktionen

        // DataTables für DBColumn.AddToSql()
        private DataTable _jobTable;
        private DataTable _employmentTable;
        private DataTable _lohnTable;
        private DataTable _lohnkomponenteTable;

        public Transfer() : this(DBData.getDBData()) {
        }

        public Transfer(DBData db) {
            _db = db;
            _editableComponentTable = new Hashtable();

            for (int counter = 0; counter < EditableComponentList.Length; counter++) {
                _editableComponentTable.Add(EditableComponentList[counter], "");
            }
        }

        /// <summary>
        /// Das Datumformat der Default-DBColumn in _db kann gesetzt werden.
        /// Bei Aufruf durch WebService fehlt dieses Format, da es sonst in
        /// Global.Session_Start() via Session-Variabeln gesetzt wird.
        /// </summary>
        public CultureInfo DBColumnUserCulture {
            set {_db.dbColumn.UserCultureName = value.Name;}
        }

        /// <summary>
        /// Fürs Importieren von kundenspezifischen Attributen wird gecheckt, ob
        /// sie in den DB-Tabellen vorkommen
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected bool tableColumnExists(string tableName, string columnName) {
            _db.connect();
            try {
                return _db.getColumnNames(tableName,columnName) != "";
            }
            finally {
                _db.disconnect();
            }
        }

        /// <summary>
        /// Komponenten, welche auf den Lohn wirken
        /// Konvention: Teil aller Komponenten
        /// </summary>
        /// <returns></returns>
        public abstract string [] SalaryComponentList {
            get;
        }
        
        /// <summary>
        /// Komponenten, die mit DLA bearbeitet werden 
        /// Konvention: Teil aller Komponenten
        /// </summary>
        /// <returns></returns>
        public abstract string [] EditableComponentList {
            get;
        }
        
        /// <summary>
        /// Alle vorkommenden Varianten
        /// </summary>
        /// <returns></returns>
        public abstract string [] ComponentList {
            get;
        }
        /// <summary>
        /// Komponentenbezeichnung
        /// Leer, falls Code für Schnittstelle nicht definiert ist
        /// </summary>
        /// <param name="ogsCode">OGS-Schnittstellen-Code</param>
        /// <returns></returns>
        protected abstract string getComponent(int ogsCode);
 
        /// <summary>
        /// OGS-Schnittstellen-Code
        /// -1, falls Komponente für Schnittstelle nicht definiert ist
        /// </summary>
        /// <param name="component">Komponentenbezeichnung</param>
        /// <returns></returns>
        protected abstract int getOGSComponent(string component);

        // Für verschiedene COM-Schnittstellen
        protected abstract int StartTransfer(string dbName);
        protected abstract int EndTransfer();
        protected abstract string GetError(int errorCode);
        protected abstract int StartStoreSalary();
        protected abstract int EndStoreSalary();
        protected abstract int PutSalary(string salaryRef, double newSalary, string comment, int component);
        protected abstract int GetNext(ref string reference);
        protected abstract int StartLoadPerson();
        protected abstract int EndLoadPerson();
        // valueList: Kundenspezifische Personen-Attribute mit zugehörigen Werte
        protected abstract int GetPerson(
            string personRef,
            ref string initials,
            ref string password,
            ref string name,
            ref string firstname,
            ref string dateofbirth,
            ref string email,
            ref string title,
            ref string phone_intern,
            ref string mobile,
            ref string personalnumber,
            ref string entryDate,
            ref string geschlecht,
            ref int typ,
            ref ArrayList valueList
            );
        protected abstract int StartLoadOrganisation();
        protected abstract int EndLoadOrganisation();
        protected abstract int GetOrganisation(
            string organisationRef,
            ref string parentRef,            
            ref int nodelayout,
            ref string name,
            ref string mnemonic,
            ref int ordnummer
            );
        protected abstract int StartLoadBudgetTyp();
        protected abstract int EndLoadBudgetTyp();
        protected abstract int GetBudgetTyp(
            string budgettypRef,
            ref string budgettypDescription
            );
        protected abstract int StartLoadBudget();
        protected abstract int EndLoadBudget();
        protected abstract int GetBudget(
            string budgetRef,
            ref string organisationRef,            
            ref double budget,            
            ref int component,
            ref double budgetVerteilbar,
            ref string budgettypRef
            );
        protected abstract int StartLoadSalary();
        protected abstract int EndLoadSalary();
        protected abstract int StartLoadSalaryHistory();
        protected abstract int EndLoadSalaryHistory();
        // rightTable: Rechte für kundenspezifische Komponenten (key: Komponenten.bezeichnung)
        // componentValueTable: Betrag für kundenspezifische Komponenten (key: Komponenten.bezeichnung)
        // valueList: Kundenspezifische Employment-Attribute mit zugehörigen Werte

        protected abstract int GetEmployment(
            bool historyData,
            string salaryRef,
            ref string personRef,
            ref string organisationRef,
            ref int typ,  
            ref string function,
            ref string functionRef,
            ref string repFunction,
            ref string repFunctionRef,
            ref int salaryYear,
            ref ArrayList valueList
            );

        protected abstract int GetSalary(
            bool historyData,
            string salaryRef,
            ref string personRef,
            ref string organisationRef,
            ref double occupation,
            ref int hourYear,
            ref int dayYear,
            ref int monthYear,
            ref int salaryKind,
            ref double currentSalary,
            ref double proposedSalary,
            ref double newSalary,
            ref int salaryYear,
            ref string employmentOrganisationRef,
            ref string budgettypRef,
            ref Hashtable rightTable,
            ref Hashtable componentValueTable,
            ref ArrayList valueList,
            ref Hashtable componentDescrTable,
            ref Hashtable componentInfoTable
            );

        /// <summary>
        /// Store data
        /// Schreibt Lohndaten in die OGS-Datenbank: Komponente und Kommentar
        /// Bei der lohnrelevanten Komponenten wird der neue Lohn (NEUER_LOHN) geschrieben,
        /// sonst der Komponentenbetrag (BETRAG).
        /// Der Parameter orgentityId kann weggelassen werden. Dann werden die Daten zu allen OEs
        /// übermittelt. In orgentityId kann eine OE angegeben werden, für die die Daten über-
        /// mittelt werden sollen. Falls mehrere angegeben werden, wird nur die erste
        /// berücksichtigt.
        /// Bedingungen:
        ///  1. Budget gehört zu der/den OE(s)
        ///  2. Budget gehört zur Komponente. Diese gehört zur aktiven Variante (HAUPTVARIANTE = 1)
        ///  3. Die Massnahmen zum Budget sind genehmigt (BUDGET.bearbeitungsstatus = EditingState.Approved)
        /// </summary>
        /// <param name="dbName">DB-name</param>
        /// <param name="components">Komponentenbezeichnungen (a,b,..) </param>
        /// <param name="orgentityId">OE (keine oder eine)</param>
        public void storeAll(string dbName, string components, params long[] orgentityId) {
            string[] compList = components.Split(',');

            foreach (string component in compList) {
                int ogsComponent = getOGSComponent(component);

                if (ogsComponent == -1) {
                    throw new Exception(
                        "Salary Component is not accepted for storing. (Component: "
                        + component
                        + ")"
                        );
                }
                else {
                    bool transferStart = false;
                    string sql = "select isnull(L.NEUER_LOHN, 0) NEUER_LOHN," 
                        + "  isnull(LK.BETRAG, 0) BETRAG,"
                        + "  LK.KOMMENTAR,"
                        + "  L.EXTERNAL_REF,"
                        + "  K.BEZEICHNUNG"
                        + " from VARIANTE V,"
                        + "  LOHN L,"
                        + "  KOMPONENTE K,"
                        + "  BUDGET B,"
                        + "  LOHNKOMPONENTE LK,"
                        + "  ORGENTITY O,"
                        + "  EMPLOYMENT E"
                        + " where V.HAUPTVARIANTE = 1"
                        + "  and L.VARIANTE_ID = V.ID"
                        + "  and K.VARIANTE_ID = V.ID"
                        + "  and K.BEZEICHNUNG = '" + component + "'"
                        + "  and B.BEARBEITUNGSSTATUS = " + (int)LohnModule.EditingState.Approved
                        + "  and LK.LOHN_ID = L.ID"
                        + "  and LK.KOMPONENTE_ID = K.ID"
                        + "  and LK.BUDGET_ID = B.ID"
                        + "  and E.ORGENTITY_ID = O.ID"
                        + "  and L.EMPLOYMENT_ID = E.ID";

                    if (orgentityId.Length > 0) {
                        sql += " and O.ID = " + orgentityId[0];
                    }
                
                    _db.connect();

                    try {
                        assertError(StartTransfer(dbName));
                        transferStart = true;
                        assertError(StartStoreSalary());
                    
                        try {
                            string kommentar = "";
                            string salaryRef = "";
                            double val = 0;
                            DataTable table = _db.getDataTable(sql);

                            foreach (DataRow row in table.Rows) {
                                kommentar = DBColumn.GetValid(row["KOMMENTAR"]," ");
                                salaryRef = DBColumn.GetValid(row["EXTERNAL_REF"]," ");
                                val = DBColumn.GetValid(row["BETRAG"],0.0);
                            
                                // Bei Lohnkomponente neuer Lohn
                                if (LohnModule.SalaryComponentTable.Contains(component)) {
                                    val = DBColumn.GetValid(row["NEUER_LOHN"],0.0);
                                }

                                assertError(PutSalary(salaryRef, val, kommentar, ogsComponent));
                            }
                        }
                        finally {
                            EndStoreSalary();
                        }
                    }
                    finally {
                        if (transferStart) {
                            EndTransfer();
                        }

                        _db.disconnect();
                    }
                }
            }
        }


        /// <summary>
        /// Load all persons, organisations, budgets, salaries from OGS
        /// Erstellt alle Daten der Organisationsstruktur dh
        ///  - Personen
        ///  - OEs mit Budgets
        ///  - Jobs mit Funktionen
        ///  - Emlployments mit Löhnen und Verweis, welche OE für den Lohn zuständig ist
        /// gemäss spezifizierten Regeln.
        /// Ob Budgets importiert werden, ist abhängig vom Budgetmodus (wie Budget in DLA
        /// behandelt werden) und vom Stand der Bearbeitung.
        /// Es werden Lohndaten der aktuellen Lohnrunde und alle gelieferten History geladen.
        /// Pro Jahr wird eine Lohnrunde und eine zugehörige Variante erzeugt.
        /// Die Variante des aktuellen Jahres wird aktiviert (HAUPTVARIANTE = 1)
        /// Das Jahr wird auf Grund des ersten Datensatzes (GetSalary())
        /// bestimmt.
        /// Die Lohndaten der Vorjahre werden geupdatet, jene des aktuellen Jahres neu
        /// geschrieben, falls noch keine zugehörige Komponenten bearbeitet wurden.
        /// Die Verwendung von nodelayout bei den OEs ist noch offen.
        /// Das Laden erfolgt in 5 Schritten (DB-Transaktionen):
        /// - Personen         bei Abbruch nächster Schritt,
        ///                    einzelne Exceptions führen nicht zum Abbruch
        /// - OEs              einzelne Exceptions führen zum Abbruch,
        ///                    dieser führt zum vollständigen Abbruch
        /// - Budget           bei Abbruch nächster Schritt,
        ///                    einzelne Exceptions führen nicht zum Abbruch
        /// - Löhne            bei Abbruch nächster Schritt,
        ///                    einzelne Exceptions führen nicht zum Abbruch
        /// - Vorjahreslöhne   einzelne Exceptions führen nicht zum Abbruch
        /// Der zurückgegebene Ladestatus liefert für jeden Schritt Infos über mögliche
        /// Probleme in Form eines Bitmaps. Jedem Schritt in obiger Reihenfolge sind
        /// 3 Bits zugeteilt:
        /// 1. Verarbeitet aber mit E-Log-Einträgen
        /// 2. Einzelne nicht verarbeitet
        /// 3. Schritt mit Rollback abgeschlossen.
        /// </summary>
        /// <param name="dbName">OGS-DB-name</param>
        /// <returns>Ladestatus</returns>
        public int loadAll(string dbName) {
            return loadAll(dbName, -1);
        }
        /// <summary>
        /// Wie oben. Zusätzlich kann die Anzahl zu importierende History-Lohnrunden
        /// angegeben werden. Voraussetzung: pro Jahr höchstens eine
        /// </summary>
        /// <param name="dbName">OGS-DB-name</param>
        /// <param name="anzahlLohnrunden">-1: alle</param>
        /// <returns>Ladestatus</returns>
        public int loadAll(string dbName, int anzahlLohnrunden) {
            _loadReturnValue = 0;
            _loadPart = LoadPart.Person;
            string sql = "";
            bool transferStart = false;
            DBColumn column = new DBColumn();
            DataTable table;
            
            ArrayList valueList = null;
            string personRef = "";
            string organisationRef = "";
            string budgettypRef = "";
                   
            string salaryRef = "";
            int typ = 0;   
            string function = "";
            string functionRef = "";
            string repFunction = "";
            string repFunctionRef = "";

            _salaryYear = DateTime.Now.Year + 1;
            _rootId = -1;
            _db.connect();
            
            try {
                _db.beginTransaction();
                assertError(StartTransfer(dbName));
                transferStart = true;

                // load person
                string leaving = "" + DateTime.Now.Year + "0101";
                sql = "update PERSON"
                    + " set LEAVING = convert(datetime, '" + leaving + "', 112)" 
                    + " where (TYP & 1) > 0 and (login <> 'admin' or login is null)";
                _db.execute(sql);

                sql = "select * from person where id = -1";
                table = _db.getDataTableExt(sql, "PERSON");
                
                assertError(StartLoadPerson());

                try {
                    while (assertError(GetNext(ref personRef)) == 0) {
                        loadPerson(personRef,table);
                    }
                    _db.commit();
                }
                catch (Exception except) {
                    _db.rollback();
                    Logger.Log(except, Logger.ERROR);
                    _loadReturnValue |= 4; // 3. bit des Bereichs
                }
                finally {
                    EndLoadPerson();
                }

                _db.beginTransaction();
                _loadPart = LoadPart.OE;

                // load organisation
                // Eine Exception in diesem Teil führt zu einem vollständigen Abbruch!
                long organisationId = DBColumn.GetValid(
                    _db.lookup(
                    "ORGANISATION_ID",
                    "VARIANTE",
                    "HAUPTVARIANTE = 1"
                    ),
                    (long)-1
                    );
                sql = "update EMPLOYMENT set ORGENTITY_ID = null"
                    + " where ORGENTITY_ID in("
                    + " select ID from ORGENTITY where ROOT_ID = "  + organisationId
                    + ")";
                _db.execute(sql);
                sql = "update ORGENTITY set VERSION_FLAG = 'D', HAT_BUDGET = 0"
                    + " where ROOT_ID = " + organisationId;
                _db.execute(sql);
                sql = "update ORGANISATION set VERSION_FLAG = 'D'"
                    + " where id = " + organisationId;
                _db.execute(sql);

                sql = "select * from ORGENTITY where ID = -1";
                table = _db.getDataTable(sql, "ORGENTITY");
                
                assertError(StartLoadOrganisation());

                try {
                   
                    while (assertError(GetNext(ref organisationRef)) == 0) {
                        loadOrganisation(organisationRef,table);
                    }

                    if (_rootId == -1) {
                        throw new TransferException("Keine Organisationswurzel definiert");
                    }

//		sql = "select ID, PARENT_ID, EXTERNAL_REF, TITLE_DE from ORGENTITY where VERSION_FLAG = 'D'";
//		table = _db.getDataTable(sql, "ORGENTITY");
//
//		foreach (DataRow row in table.Rows) 
//		{
//			Logger.Log("Try to delete: " + row["ID"] + "; " + row["PARENT_ID"] +"; " + row["EXTERNAL_REF"] +"; " + row["TITLE_DE"], Logger.DEBUG);
//		}

                    // Löschen der OE
                    sql = "select ID from ORGENTITY where VERSION_FLAG = 'D'";
                    table = _db.getDataTable(sql, "ORGENTITY");

                    foreach (DataRow row in table.Rows) {
                        deleteOrgentity((long)row["ID"], "D");
                    }

                    // Löschen der Org
                    sql = "select ID from ORGANISATION where VERSION_FLAG = 'D'";
                    table = _db.getDataTable(sql, "ORGANISATION");

                    foreach (DataRow row in table.Rows) {
                        _db.Organisation.delete((long)row["ID"],true);
                    }
                }
                finally {
                    EndLoadOrganisation();
                }

                // aktuelles _salaryYear aus Salary-Daten
                assertError(StartLoadSalary());
                assertError(GetNext(ref salaryRef));
                assertError(
                    GetEmployment(
                    false,
                    salaryRef,
                    ref personRef,
                    ref organisationRef,
                    ref typ,  
                    ref function,
                    ref functionRef,
                    ref repFunction,
                    ref repFunctionRef,
                    ref _salaryYear,
                    ref valueList));
                EndLoadSalary();
                
                // Erzeugen der Lohnrunden, Varianten und Komponenten
                _varianteIdTable = new Hashtable();
                _komponenteIdTable = new Hashtable();

                _neueLohnrunde = setIdsForSalaryData(_salaryYear);

                sql = "update VARIANTE set HAUPTVARIANTE = 1"
                    + " where ID = " + _varianteIdTable[_salaryYear];
                _db.execute(sql);

                _db.commit();
                _db.beginTransaction();
                _loadPart = LoadPart.Budget;

                // load budgettyp (ganz am Schluss werden Budgettypen ohne Budget bzw
                // Employments wieder gelöscht)
                // Für programminternen Gebrauch wird eine Hashtable erzeugt
                bool load = true;
                sql = "select * from BUDGETTYP where ID = -1";
                table = _db.getDataTable(sql, "BUDGETTYP");

                _budgettypIdTable = new Hashtable();

                if (assertError(StartLoadBudgetTyp()) == 0) {
                    try {
                        while (assertError(GetNext(ref budgettypRef)) == 0) {
                            loadBudgetTyp(budgettypRef,table);
                        }
                        _db.commit();
                    }
                    catch (Exception except) {
                        _db.rollback();
                        Logger.Log(except, Logger.ERROR);
                        _loadReturnValue |= 4 * 64; // 3. bit des Bereichs
                        load = false; // kein Import der Budgets
                    }
                    finally {
                        EndLoadBudgetTyp();
                    }
                }

                // load budget
                // Soll Budget importiert werden und, falls ja, mit welchem Freigabestatus
                if (load) loadBudget();

                _db.beginTransaction();
                _loadPart = LoadPart.Salary;

                // load salary
                // Funktionen-Katalog bei fehlenden Funktionen
                long funktion_groupId = DBColumn.GetValid(
                    _db.lookup(
                    "ID",
                    "FUNKTION_GROUP",
                    "PARENT_ID is null"
                    ),
                    (long)-1
                    );

                if (funktion_groupId != -1) {
                    _funktion_groupId = funktion_groupId.ToString();
                }

                // Funktionstabelle für die Identifikation
                _funktionIdTable = new Hashtable();
                table = _db.getDataTable(
                    "select ID, EXTERNAL_REF from FUNKTION",
                    "FUNKTION where EXTERNAL_REF is not null"
                    );

                foreach (DataRow row in table.Rows) {
                    _funktionIdTable.Add(row[1].ToString(), (long) row[0]);
                }

                // aktuelle Löhne
                // DataTable für DBColumn.AddToSQL() bei den Inserts und Updates
                sql = "select * from EMPLOYMENT where ID = -1";
                _employmentTable = _db.getDataTableExt(sql, "EMPLOYMENT");
                sql = "select * from JOB where ID = -1";
                _jobTable = _db.getDataTableExt(sql, "JOB");
                sql = "select * from LOHN where ID = -1";
                _lohnTable = _db.getDataTableExt(sql, "LOHN");
                sql = "select * from LOHNKOMPONENTE where ID = -1";
                _lohnkomponenteTable = _db.getDataTableExt(sql, "LOHNKOMPONENTE");

                // engagement in job muss auf 0 gesetzt werden, sonst verhindert ein Check
                // im Trigger, dass ein neuer Job bei der gleichen Anstellung zugefügt
                // werden kann.
                // Da der Check im Organisationsmodul und JOB.VERSION_FLAG im Lohn-Modul
                // definiert ist, ist es zu kompliziert den Trigger anzupassen.
                sql = "update JOB set VERSION_FLAG = 'D', engagement = 0";
                _db.execute(sql, 60);

                // nur Lohndaten löschen die noch nicht bearbeitet wurden
                string editierbareKomponenten = " and K.BEZEICHNUNG in (";

                for (int counter = 0; counter < EditableComponentList.Length; counter++) {
                    editierbareKomponenten += (counter == 0 ? "'" : ", '")
                        + EditableComponentList[counter]
                        + "'";
                }

                editierbareKomponenten += ")";
                sql = "delete LOHN"
                    + " where VARIANTE_ID = " + _varianteIdTable[_salaryYear]; // aktuelle Lohndaten neu schreiben
                
                if (EditableComponentList.Length > 0) {
                    sql += " and not exists ("
                        + " select 1 from LOHNKOMPONENTE L, KOMPONENTE K"
                        + " where L.LOHN_ID = LOHN.ID"
                        + " and L.BEARBEITUNGSSTATUS != " + (int)LohnModule.EditingState.Pending
                        + " and L.KOMPONENTE_ID = K.ID"
                        + editierbareKomponenten
                        + ")";
                }

                _db.execute(sql,60);
                
                assertError(StartLoadSalary());
                
                try {
                    while (assertError(GetNext(ref salaryRef)) == 0) {
                        try {
                            loadSalaryData(salaryRef, 0); // 0 Lohnrunden vorher dh aktuelle
                        }
                        catch (Exception except) {
                            Logger.Log(except, Logger.ERROR);
                            _loadReturnValue |= 2 * 512; // 2. bit des Bereichs
                        }
                    }

                    _db.commit();
                }
                catch (Exception except) {
                    _db.rollback();
                    Logger.Log(except, Logger.ERROR);
                    _loadReturnValue |= 4 * 512; // 3. bit des Bereichs
                }
                finally {
                    EndLoadSalary();
                }

                // Löhne der Vorjahre
                if (anzahlLohnrunden != 0) {
                    _db.beginTransaction();
                    _loadPart = LoadPart.PreviousSalary;
                    assertError(StartLoadSalaryHistory());
                    
                    try {
                        while (assertError(GetNext(ref salaryRef)) == 0) {
                            try {
                                loadSalaryData(salaryRef, anzahlLohnrunden);
                            }
                            catch (Exception except) {
                                Logger.Log(except, Logger.ERROR);
                                _loadReturnValue |= 2 * 4096; // 2. bit des Bereichs
                            }
                        }

                        sql = "delete JOB where VERSION_FLAG = 'D'";
                        _db.execute(sql);
                        _db.commit();
                    }
                    catch (Exception except) {
                        _db.rollback();
                        Logger.Log(except, Logger.ERROR);
                        _loadReturnValue |= 4 * 4096; // 3. bit des Bereichs
                    }
                    finally {
                        EndLoadSalaryHistory();
                    } 
                }

                // Löschen nicht verwendeter Budgettypen
                try {
                    _db.beginTransaction();
                    sql = "delete BUDGETTYP"
                        + " where not exists ("
                        + " select 1 from EMPLOYMENT where BUDGETTYP_ID = BUDGETTYP.ID"
                        + ") and  not exists ("
                        + " select 1 from BUDGET where BUDGETTYP_ID = BUDGETTYP.ID"
                        + ")";
                    _db.execute(sql);
                    _db.commit();                       
                }
                catch (Exception except) {
                    _db.rollback();
                    Logger.Log(except, Logger.ERROR);
                }
            }
            catch (Exception except) {
                Logger.Log(except, Logger.ERROR);

                switch (_loadPart) {
                case LoadPart.Person:
                    _loadReturnValue |= 4;
                    goto case LoadPart.OE;
                case LoadPart.OE:
                    _loadReturnValue |= 4 * 8;
                    goto case LoadPart.Budget;
                case LoadPart.Budget:
                    _loadReturnValue |= 4 * 64;
                    goto case LoadPart.Salary;
                case LoadPart.Salary:
                    _loadReturnValue |= 4 * 512;
                    goto case LoadPart.PreviousSalary;
                case LoadPart.PreviousSalary:
                    _loadReturnValue |= 4 * 4096;
                    break;
                }
                Logger.Log("Transfer load code = "+_loadReturnValue,Logger.ERROR);
                throw(except);
            }
            finally {
                if (transferStart) EndTransfer();
                _db.disconnect();
                // Führt allfällige Rollbacks aus! Ein vorgängiges Rollback - wie früher -
                // führt zu einer Exception, welche die wichtigen Exeptions verdeckt
            }

            return _loadReturnValue;
        }
        
        /// <summary>
        /// Löschen aller OE's mit angegebenem Versionsflag
        /// Zugehörige Budgets und Lohnkomponenten werden ohne Rücksicht auf
        /// den Stand der Bearbeitung gelöscht!
        /// Löschen ist nur möglich, falls alle untergeordneten OEs das angegebene
        /// Versionsflag haben. Falls nicht, wird einfach nicht gelöscht.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="versionFlag"></param>
        /// <returns>OE ist gelöscht</returns>
        private bool deleteOrgentity(long id, string versionFlag) {
            bool deleted = true;

            // ist die OE noch vorhanden?
            object [] values
                = _db.lookup(new string [] {"ID", "VERSION_FLAG"}, "ORGENTITY", "ID = " + id);
            long orgentityId = DBColumn.GetValid(values[0], (long)-1);

            if (orgentityId != -1) {
                string dataVersionFlag = DBColumn.GetValid(values[1], "");

                if (dataVersionFlag == versionFlag) {
                    string sql = "select ID from ORGENTITY"
                        + " where PARENT_ID = " + orgentityId;
                    DataTable table = _db.getDataTable(sql, "ORGENTITY");

                    foreach (DataRow row in table.Rows) {
                        if (!deleteOrgentity((long)row["ID"], versionFlag)) {
                            deleted = false;
                            break;
                        }
                    }

                    if (deleted) {
                        _db.Orgentity.delete(orgentityId,true);

                    }
                }
                else deleted = false;
            }

            return deleted;
        }

        /// <summary>
        /// Fürs Importieren der Salary-Daten pro Runde (eine pro Jahr!) braucht es ein Set
        /// von Ids (Variante und Komponenten).
        /// Es wird für die DB-Einträge gesorgt und die Ids in den zugehörigen Members ein-
        /// getragen
        /// </summary>
        /// <param name="year"></param>
        /// <return>true: neue Lohnrunde</return>
        private bool setIdsForSalaryData(int year) {
            long varianteId = -1;
            string sql = "";
            long lohnrundeId = DBColumn.GetValid(
                _db.lookup("ID", "LOHNRUNDE", "datepart(yyyy, GUELTIG_AB) = " + year),
                (long)-1
                );
            bool neu = lohnrundeId == -1;

            if (lohnrundeId == -1) {
                lohnrundeId = _db.newId("LOHNRUNDE");
                sql = "insert into LOHNRUNDE ("
                    + " ID, GUELTIG_AB"
                    + ") values ("
                    + lohnrundeId + ", convert(datetime, '" + year + "0101', 112)"
                    +")";
                _db.execute(sql);
            }

            varianteId = DBColumn.GetValid(
                _db.lookup("ID", "VARIANTE", "lohnrunde_id = " + lohnrundeId),
                (long)-1
                );

            if (varianteId == -1) {
                varianteId = _db.newId("VARIANTE");
                sql = "insert into VARIANTE ("
                    + " ID, LOHNRUNDE_ID, BEZEICHNUNG, ORGANISATION_ID"
                    + ") values ("
                    + varianteId + ", " + lohnrundeId + ", 'OGS-Import', " + _rootId
                    +")";
            }
            else {
                sql = "update VARIANTE"
                    + " set ORGANISATION_ID = " + _rootId
                    + " where ID = " + varianteId;
            }

            _db.execute(sql);

            if (!_varianteIdTable.ContainsKey(year)) {
                _varianteIdTable.Add(year, varianteId);
            }

            for (int counter = 0; counter < ComponentList.Length; counter++) {
                _komponenteIdTable.Add(
                    year.ToString() + ComponentList[counter],
                    createMissingKomponente(ComponentList[counter], varianteId)
                    );
            }

            return neu;
        }

 
        private void loadBudgetTyp(string budgettypRef, DataTable table) {
            ch.psoft.db.SQLColumn column = _db.dbColumn;
            string sql = "";
            string budgettypDescription = "";

            assertError(
                GetBudgetTyp(
                budgettypRef,
                ref budgettypDescription
                )
                );
                                
            long budgettypId = DBColumn.GetValid(
                _db.lookup(
                "ID",
                "BUDGETTYP",
                "EXTERNAL_REF = '" + budgettypRef + "'"
                ),
                (long)-1
                );

            if (budgettypId == -1) {
                budgettypId = _db.newId("BUDGETTYP");
                sql = "insert into budgettyp ("
                    + "id,"
                    + "bezeichnung,"
                    + "beschreibung,"
                    + "external_ref"
                    + ") values (";
                sql = column.AddToSql(sql, table.Columns["id"], budgettypId) + ",";
                sql = column.AddToSql(sql, table.Columns["bezeichnung"], budgettypDescription) + ",";
                sql = column.AddToSql(sql, table.Columns["beschreibung"], budgettypDescription) + ",";
                sql = column.AddToSql(sql, table.Columns["external_ref"], budgettypRef) + ")";
            }
            else {
                sql = "update BUDGETTYP";
                sql = column.AddToSql(sql + " set bezeichnung =", table.Columns["bezeichnung"], budgettypDescription) + ",";
                sql = column.AddToSql(sql + "beschreibung =", table.Columns["beschreibung"], budgettypDescription);
                sql += " where id = " + budgettypId;
            }
                            
            _db.execute(sql);
            _budgettypIdTable.Add(budgettypRef, budgettypId);
        }
       
        private void loadBudget() {
            string sql = "";
            // Komponenten bestimmen, für welche die Budgets übernommen werden sollen.
            bool load = false;
            long id = -1;
            bool loadKomponente = false;
            string komponente = "";
            int freigabestatus = (int) LohnModule.ClearingState.Disabled;
            Hashtable loadKomponenteTable = new Hashtable();
            DataTable table;
            ch.psoft.db.SQLColumn column = _db.dbColumn;
            string organisationRef = "";
            string budgettypRef = "";
            string budgetRef = "";
            double budget = 0;
            double budgetVerteilbar = 0;
            int component = 0;

            for (int counter = 0; counter < EditableComponentList.Length; counter++) {
                // 1. Kein Budget darf zugehörige bearbeitete Lohnkomponenten haben.
                // Diese können zusammen mit dem Budget verschwinden
                komponente = EditableComponentList[counter];
                id = DBColumn.GetValid(
                    _db.lookup(
                    "B.ID",
                    "BUDGET B inner join LOHNKOMPONENTE L"
                    + " on B.ID = L.BUDGET_ID"
                    + " inner join KOMPONENTE K"
                    + " on L.KOMPONENTE_ID = K.ID",
                    "K.VARIANTE_ID = " + _varianteIdTable[_salaryYear]
                    + " and K.BEZEICHNUNG = '" + komponente + "'"
                    + " and L.BEARBEITUNGSSTATUS != "
                    + (int)LohnModule.EditingState.Pending
                    ),
                    (long)-1
                    );
                        
                if (id == -1) { // keine bearbeiteten Lohnkomponenten!
                    // 2. Im normalen Modus (ohne sofortige Freigabe) darf kein Budget
                    // freigegeben sein.
                    switch (LohnModule.BudgetModusConfig) {
                    case (int)LohnModule.BudgetModus.budgetImportSofortigeFreigabe:
                        loadKomponente = true;
                        break;
                    case (int)LohnModule.BudgetModus.budgetImport:
                        // Gibt es bereits freigegebene Budgets?
                        id = DBColumn.GetValid(
                            _db.lookup(
                            "B.ID",
                            "BUDGET B inner join KOMPONENTE K"
                            + " on B.KOMPONENTE_ID = K.ID",
                            "K.VARIANTE_ID = " + _varianteIdTable[_salaryYear]
                            + " and K.BEZEICHNUNG = '" + komponente + "'"
                            + " and B.FREIGABESTATUS"
                            + " != " + (int)LohnModule.ClearingState.Disabled
                            ),
                            (long)-1
                            );

                        if (id == -1) { // keine freigegebene Budget
                            loadKomponente = true;
                        }
                        else {
                            loadKomponente = false;
                        }

                        break;
                    }
                }
                else { // bearbeitete Lohnkomponenten vorhanden!
                    loadKomponente = false;
                }
                        
                loadKomponenteTable.Add(getOGSComponent(komponente), loadKomponente);

                // bei mindestens einer zu übernehmenden Komponente Budgetteil ausführen!
                if (loadKomponente) {
                    load = true;
                }
            }

            // Soll Budget importiert werden und, falls ja, mit welchem Freigabestatus
            if (load) {

                // Freigabestatus bestimmen
                switch (LohnModule.BudgetModusConfig) {
                case (int)LohnModule.BudgetModus.budgetImportSofortigeFreigabe:
                    freigabestatus = (int) LohnModule.ClearingState.Enabled;
                    break;
                case (int)LohnModule.BudgetModus.budgetImport:
                    freigabestatus = (int) LohnModule.ClearingState.Disabled;
                    break;
                }

                _db.beginTransaction();
                assertError(StartLoadBudget());

                try {
                    for (int counter = 0; counter < EditableComponentList.Length; counter++) {
                        komponente = EditableComponentList[counter];
                            
                        if ((bool)loadKomponenteTable[getOGSComponent(komponente)]) {
                            sql = "update BUDGET"
                                + " set VERSION_FLAG = 'D'"
                                + " where KOMPONENTE_ID in("
                                + "  select ID from KOMPONENTE"
                                + "  where VARIANTE_ID = " + _varianteIdTable[_salaryYear]
                                + "  and BEZEICHNUNG = '" + komponente + "'"
                                + ")";
                            _db.execute(sql);
                        }
                    }

                    sql = "select * from BUDGET where ID = -1";
                    table = _db.getDataTable(sql, "BUDGET");
                        
                    string budgetExternal_ref = "";
                    long budgetId = -1;
                    string orgentityIdString = "null";
                    long budgettypId = -1;
                    string budgettypIdString = "null";
                    long komponenteId = -1;
                    string komponenteIdString = "null";

                    while (assertError(GetNext(ref budgetRef)) == 0) {
                        try {
                            assertError(
                                GetBudget(
                                budgetRef,
                                ref organisationRef,            
                                ref budget,            
                                ref component,
                                ref budgetVerteilbar,
                                ref budgettypRef
                                )
                                );
                                    
                            if ((bool)loadKomponenteTable[component]) {
                                budgetExternal_ref = budgetRef + "_" + budgettypRef;
                                budgetId = DBColumn.GetValid(
                                    _db.lookup(
                                    "ID",
                                    "BUDGET",
                                    "EXTERNAL_REF = '" + budgetExternal_ref + "'"
                                    ),
                                    (long)-1
                                    );
                                orgentityIdString = DBColumn.GetValid(
                                    _db.lookup(
                                    "ID",
                                    "ORGENTITY",
                                    "EXTERNAL_REF = '" + organisationRef + "'"
                                    ),
                                    "null"
                                    );
                                budgettypId = getHashtableValue(
                                    _budgettypIdTable,
                                    budgettypRef,
                                    -1
                                    );
                                budgettypIdString = budgettypId == -1 ? "null" : budgettypId.ToString();
                                komponenteId = getHashtableValue(
                                    _komponenteIdTable,
                                    _salaryYear.ToString() + getComponent(component),
                                    -1
                                    );
                                komponenteIdString = komponenteId == -1 ? "null" : komponenteId.ToString();

                                if (budgetId == -1) {
                                    budgetId = _db.newId("BUDGET");
                                    sql = "insert into budget ("
                                        + "id,"
                                        + "betrag,"
                                        + "betrag_verteilbar,"
                                        + "orgentity_id,"
                                        + "komponente_id,"
                                        + "bearbeitungsstatus,"
                                        + "freigabestatus,"
                                        + "budgettyp_id,"
                                        + "external_ref"
                                        + ") values (";
                                    sql = column.AddToSql(sql, table.Columns["id"], budgetId) + ",";
                                    sql = column.AddToSql(sql, table.Columns["betrag"], budget) + ",";
                                    sql = column.AddToSql(sql, table.Columns["betrag_verteilbar"], budgetVerteilbar) + ",";
                                    sql += orgentityIdString + ","
                                        + komponenteIdString + ",";
                                    sql = column.AddToSql(sql, table.Columns["bearbeitungsstatus"], (int)LohnModule.EditingState.Pending) + ",";
                                    sql = column.AddToSql(sql, table.Columns["freigabestatus"], freigabestatus) + ",";
                                    sql += budgettypIdString + ",";
                                    sql = column.AddToSql(sql, table.Columns["external_ref"], budgetExternal_ref) + ")";
                                }
                                else {
                                    sql = "update BUDGET"
                                        + " set VERSION_FLAG = 'P'";
                                    sql = column.AddToSql(sql + ",betrag =", table.Columns["betrag"], budget);
                                    sql = column.AddToSql(sql + ",betrag_verteilbar =", table.Columns["betrag_verteilbar"], budgetVerteilbar);
                                    sql += ",komponente_id = " + komponenteIdString;
                                    sql += ",budgettyp_id = " + budgettypIdString;
                                    sql += ",bearbeitungsstatus = " + ((int)LohnModule.EditingState.Pending);
                                    sql += ",freigabestatus = " + freigabestatus;
                                    sql += " where id = " + budgetId;
                                }
                                    
                                _db.execute(sql);
                            }

                        }
                        catch (Exception except) {
                            Logger.Log(except, Logger.ERROR);
                            _loadReturnValue |= 2 * 64; // 2. bit des Bereichs
                        }
                    }

                    // Löschen der nicht mehr vorhandenen Budgets und der zugeordneten
                    // Lohnkomponenten
                    sql = "delete LOHNKOMPONENTE"
                        + " where BUDGET_ID in("
                        + " select B.ID"
                        + " from KOMPONENTE K, BUDGET B"
                        + " where B.VERSION_FLAG = 'D'"
                        + " and K.VARIANTE_ID = " + _varianteIdTable[_salaryYear]
                        + " and K.ID = B.KOMPONENTE_ID"
                        + ")";
                    _db.execute(sql);
                    sql = "delete BUDGET"
                        + " where VERSION_FLAG = 'D'"
                        + " and KOMPONENTE_ID in("
                        + " select ID from KOMPONENTE where VARIANTE_ID = " + _varianteIdTable[_salaryYear]
                        + ")";
                    _db.execute(sql);
                    _db.commit();
                }
                catch (Exception except) {
                    _db.rollback();
                    Logger.Log(except, Logger.ERROR);
                    _loadReturnValue |= 4 * 64; // 3. bit des Bereichs
                }
                finally {
                    EndLoadBudget();
                }
            }
        }

        private void loadPerson(string personRef, DataTable table) {
            ch.psoft.db.SQLColumn column = _db.dbColumn;
            ArrayList valueList = null;
            string sql = "";

            long personId = -1;
            string initials = "";
            string password = "";
            string name = "";
            string firstname = "";
            string dateofbirth = "";
            string email = "";
            string title = "";
            string phone_intern = "";
            string mobile = "";
            string personalnumber = "";
            string entryDate = "";
            string geschlecht = "";
            int typ = 0;

            try {
                assertError(
                    GetPerson(
                    personRef,
                    ref initials,
                    ref password,
                    ref name,
                    ref firstname,
                    ref dateofbirth,
                    ref email,
                    ref title,
                    ref phone_intern,
                    ref mobile,
                    ref personalnumber,
                    ref entryDate,
                    ref geschlecht,
                    ref typ,
                    ref valueList
                    )
                    );

                personId = DBColumn.GetValid(
                    _db.lookup("ID", "PERSON", "EXTERNAL_REF = '" + personRef + "'"),
                    (long)-1
                    );

                if (personId == -1) {
                    sql = "insert into person (";

                    if (valueList != null && valueList.Count > 0) {
                        foreach (UserAtributeValue val in  valueList) {
                            if (val._tableName == "PERSON") {
                                sql += val._columnName + ",";
                            }
                        }
                    }

                    sql += "login,"
                        + "password,"
                        + "pname,"
                        + "firstname,"
                        + "dateofbirth,"
                        + "email,"
                        + "title,"
                        + "phone,"
                        + "mobile,"
                        + "personnelnumber,"
                        + "external_ref,"
                        + "entry,"
                        + "sex,"
                        + "typ"
                        + ") values (";


                    if (valueList != null && valueList.Count > 0) {
                        foreach (UserAtributeValue val in  valueList) {
                            if (val._tableName == "PERSON") {
                                sql = column.AddToSql(
                                    sql,
                                    table.Columns[val._columnName],
                                    val._value
                                    ) + ",";
                            }
                        }
                    }

                    sql = column.AddToSql(sql, table.Columns["login"], initials) + ","; 
                    sql = column.AddToSql(sql, table.Columns["password"], password) + ",";
                    sql = column.AddToSql(sql, table.Columns["pname"], name) + ",";
                    sql = column.AddToSql(sql, table.Columns["firstname"], firstname) + ",";
                    sql = column.AddToSql(sql, table.Columns["dateofbirth"], dateofbirth) + ",";
                    sql = column.AddToSql(sql, table.Columns["email"], email) + ",";
                    sql = column.AddToSql(sql, table.Columns["title"], title) + ",";
                    sql = column.AddToSql(sql, table.Columns["phone"], phone_intern) + ",";
                    sql = column.AddToSql(sql, table.Columns["mobile"], mobile) + ",";
                    sql = column.AddToSql(sql, table.Columns["personnelnumber"], personalnumber) + ",";
                    sql = column.AddToSql(sql, table.Columns["external_ref"], personRef) + ",";
                    sql = column.AddToSql(sql, table.Columns["entry"], entryDate) + ",";
                    sql = column.AddToSql(sql, table.Columns["sex"], sex(geschlecht)) + ",";
                    sql += (typ == 0 ? "1)" : "2)");
                }
                else {
                    sql = "update person set leaving = null,";

                    if (valueList != null && valueList.Count > 0) {
                        foreach (UserAtributeValue val in  valueList) {
                            if (val._tableName == "PERSON") {
                                sql = column.AddToSql(
                                    sql + table.Columns[val._columnName] + "=",
                                    table.Columns[val._columnName],
                                    val._value
                                    ) + ",";
                            }
                        }
                    }

                    // Das Password wird nur beim Insert eingetragen
                    sql = column.AddToSql(sql + "login=", table.Columns["login"], initials) + ","; 
                    sql = column.AddToSql(sql + "pname=", table.Columns["pname"], name) + ",";
                    sql = column.AddToSql(sql + "firstname=", table.Columns["firstname"], firstname) + ",";
                    sql = column.AddToSql(sql + "dateofbirth=", table.Columns["dateofbirth"], dateofbirth) + ",";
                    sql = column.AddToSql(sql + "email=", table.Columns["email"], email) + ",";
                    sql = column.AddToSql(sql + "title=", table.Columns["title"], title) + ",";
                    sql = column.AddToSql(sql + "phone=", table.Columns["phone"], phone_intern) + ",";
                    sql = column.AddToSql(sql + "mobile=", table.Columns["mobile"], mobile) + ",";
                    sql = column.AddToSql(sql + "personnelnumber=", table.Columns["personnelnumber"], personalnumber) + ",";
                    sql = column.AddToSql(sql + "entry=", table.Columns["entry"], entryDate) + ",";
                    sql = column.AddToSql(sql + "sex=", table.Columns["sex"], sex(geschlecht));
                    sql += " where ID = " + personId;
                }
                            
                _db.execute(sql);
            }
            catch (Exception except) {
                Logger.Log(except, Logger.ERROR);
                _loadReturnValue |= 2; // 2. bit des Bereichs
            }
        }

        private void loadOrganisation(string organisationRef, DataTable table) {
            bool isRoot = false;
            ch.psoft.db.SQLColumn column = _db.dbColumn;
            string sql = "";

            string name = "";            
            long orgentityId = -1;
            long parentId = -1;
            string parentRef = "";            
            int nodelayout = 0;
            string mnemonic = "";
            int ordnummer = 0;
            string titleAttribute = "TITLE_" + _attributeLanguageCode;
            string mnemonicAttribute = "MNEMONIC_" + _attributeLanguageCode;

            assertError(
                GetOrganisation(
                organisationRef,
                ref parentRef,            
                ref nodelayout,
                ref name,
                ref mnemonic,
                ref ordnummer
                )
                );

            orgentityId = DBColumn.GetValid(
                _db.lookup("ID", "ORGENTITY", "EXTERNAL_REF = '" + organisationRef + "'"),
                (long)-1
                );
            parentId = DBColumn.GetValid(
                _db.lookup(
                "ID",
                "ORGENTITY",
                "EXTERNAL_REF = '" + parentRef + "' and VERSION_FLAG = 'P'"
                ),
                (long)-1
                );

            // Ist OE die Root? (höchstens eine!)
            isRoot = (parentRef == "" && _rootId == -1);

            // Bestimmen der Root-Id und für Organisation-Record sorgen.
            // _rootId wird über die restlichen Loops erhalten
            if (isRoot) {
                // Gibt es bereits einen Organisation-Record
                if (orgentityId == -1) {
                    _rootId = DBColumn.GetValid(
                        _db.lookup("ID", "ORGANISATION", "MAINORGANISATION = 1"),
                        (long)-1
                        );
                }
                else {
                    _rootId = DBColumn.GetValid(
                        _db.lookup("ID", "ORGANISATION", "ID = " + orgentityId),
                        (long)-1
                        );
                }

                if (_rootId == -1) {
                    if (orgentityId == -1) {
                        _rootId = _db.newId("ORGENTITY");
                    }
                    else {
                        _rootId = orgentityId;
                    }

                    sql = "insert into ORGANISATION ("
                        + " ID, VERSION_FLAG, TITLE_DE, MAINORGANISATION"
                        + ") values (" 
                        + _rootId + ", 'P', 'Organisation', 1"
                        + ")";
                }
                else {
                    sql = "update ORGANISATION"
                        + " set MAINORGANISATION = 1, VERSION_FLAG = 'P'"
                        + " where id = " + _rootId;
                }

                _db.execute(sql);
            }
                       
            if (orgentityId == -1) {
                // OE neu
                if (isRoot) {
                    orgentityId = _rootId;
                }
                else {
                    orgentityId = _db.newId("ORGENTITY");
                }
                            
                sql = "insert into ORGENTITY ("
                    + "id,"
                    + "title_de,"
                    + "mnemonic_de,"
                    + "ordnumber,"
                    + "external_ref,"
                    + "parent_id,"
                    + "root_id,"
                    + "VERSION_FLAG,"
                    + "INHERIT,"
                    + "HAT_BUDGET"
                    + ") values (";
                sql = column.AddToSql(sql, table.Columns["id"], orgentityId) + ",";
                sql = column.AddToSql(sql, table.Columns["title_de"], name) + ",";
                sql = column.AddToSql(sql, table.Columns["mnemonic_de"], mnemonic) + ",";
                sql = column.AddToSql(sql, table.Columns["ordnumber"], ordnummer) + ",";
                sql = column.AddToSql(sql, table.Columns["external_ref"], organisationRef) + ",";
                sql += (isRoot ? "null" : "" + (parentId == -1 ? _rootId : parentId)) + ","; // falls Parent fehlt, der root-OE unterstellen
                sql += _rootId + ",";
                sql += "'P', 1, 0)";
            }
            else {
                // OE updaten
                sql = "update ORGENTITY"
                    + " set VERSION_FLAG = 'P',"
                    + " parent_id = " + (isRoot ? "null" : "" + (parentId == -1 ? _rootId : parentId)) + "," // falls Parent fehlt, der root-OE unterstellen
                    + " root_id = " + _rootId + ",";
                sql = column.AddToSql(sql + titleAttribute + "=", table.Columns[titleAttribute], name) + ",";
                sql = column.AddToSql(sql + mnemonicAttribute + "=", table.Columns[mnemonicAttribute], mnemonic) + ",";
                sql = column.AddToSql(sql + "ordnumber=", table.Columns["ordnumber"], ordnummer) + ",";
                sql = column.AddToSql(sql + "external_ref=", table.Columns["external_ref"], organisationRef);
                sql += " where id = " + orgentityId;
            }

            _db.execute(sql);

            if (isRoot) {
                sql = "update ORGANISATION set ORGENTITY_ID = " + _rootId
                    + " where ID = " + _rootId;
                _db.execute(sql);
            }
        }

        /// <summary>
        /// Einarbeiten der Salary-Daten zu salaryRef in die DB
        /// Modus 1: anzahlLohnrunden == 0, aktuelles Jahr wird importiert, dazu muss
        ///          StartLoadSalary() ausgeführt worden sein.
        /// Modus 2: anzahlLohnrunden > 0 oder -1 alle, Historydaten werden importiert, dazu muss
        ///          StartLoadSalaryHistory() ausgeführt worden sein.
        ///          anzahlLohnrunden gibt an, wieviele Jahre (höchstens 1 Jahr pro Lohnrunde!)
        ///          importiert werden sollen. -1 bedeutet, alle History-Sätze, welche die
        ///          Schnittstelle liefert.
        /// Bemerkung:
        /// Konzeptionell war diese Methode auf eine frühere Version
        /// der COM-Schnittstelle ausgerichtet. Sie wurde aus Zeitgründen nicht um gebaut
        /// </summary>
        /// <param name="salaryRef"></param>
        /// <param name="anzahlLohnrunden">siehe obige Beschreibung</param>
        private void loadSalaryData(string salaryRef, int anzahlLohnrunden) {
            string sql = "";
            string titleAttribute = "TITLE_" + _attributeLanguageCode;
            string mnemonicAttribute = "MNEMONIC_" + _attributeLanguageCode;

            DBColumn column = new DBColumn();
            string personRef = "";
            string organisationRef = "";
            int typ = 0;   
            string function = "";
            string repFunction = "";
            double occupation = 100;
            int hourYear = 100;
            int dayYear = 365;
            int monthYear = 12;
            int salaryKind = 0;
            double currentSalary = 0;
            double newSalary = 0;
            double proposedSalary = 0;
            string functionRef = "";
            string repFunctionRef = "";
            string employmentOrganisationRef = "";
            string budgettypRef = "";
            int salaryYear = 2006;
            Hashtable rightTable = null;
            Hashtable componentValueTable = null;
            Hashtable componentDescrTable = null;
            Hashtable componentInfoTable = null;
            ArrayList valueListEmp = null;
            ArrayList valueListSal = null;
            int bitPart = 1;
            bool salaryDef = false;

            if (assertError(
                GetEmployment(
                anzahlLohnrunden != 0, // anzahlLohnrunden > 0 oder -1 (alle) bedeutet History
                salaryRef,
                ref personRef,
                ref organisationRef,
                ref typ,  
                ref function,
                ref functionRef,
                ref repFunction,
                ref repFunctionRef,
                ref salaryYear,
                ref valueListEmp)) == 0) 
            {
                salaryDef = assertError(
                    GetSalary(
                    anzahlLohnrunden != 0, 
                    salaryRef,
                    ref personRef,
                    ref organisationRef,
                    ref occupation,
                    ref hourYear,
                    ref dayYear,
                    ref monthYear,
                    ref salaryKind,
                    ref currentSalary,
                    ref proposedSalary,
                    ref newSalary,
                    ref salaryYear,
                    ref employmentOrganisationRef,
                    ref budgettypRef,
                    ref rightTable,
                    ref componentValueTable,
                    ref valueListSal,
                    ref componentDescrTable,
                    ref componentInfoTable)) >= 0;
            }
            // nichts ausführen, falls History-Daten zu früheren Jahren gehören
            if (anzahlLohnrunden != -1 && salaryYear < _salaryYear - anzahlLohnrunden) {
                return;
            }

            // Fürs Setzen der Bits im richtigen Bereich von _loadReturnValue
            if (_loadPart == LoadPart.Salary) {
                bitPart = 512;
            }
            else {
                bitPart = 4096;
            }

            if (personRef == "") {
                Logger.Log(
                    "Salary data without person reference! (salaryRef = " + salaryRef + ")",
                    Logger.ERROR
                    );
                _loadReturnValue |= bitPart; // 1. bit des Bereichs
            }
            else {
                // Variante initialisieren
                if (!_varianteIdTable.ContainsKey(salaryYear)) {
                    setIdsForSalaryData(salaryYear);
                }

                long personId = DBColumn.GetValid(
                    _db.lookup("ID", "PERSON", "EXTERNAL_REF = '" + personRef + "'"),
                    (long)-1
                    );
                
                if (personId == -1) {
                    throw new Exception("Person not found (personRef = " + personRef + ")");
                }

                long orgentityId = DBColumn.GetValid(
                    _db.lookup("ID", "ORGENTITY", "EXTERNAL_REF = '" + organisationRef + "'"),
                    (long)-1
                    );
                string employmentOrgentityIdString = DBColumn.GetValid(
                    _db.lookup("ID", "ORGENTITY", "EXTERNAL_REF = '" + employmentOrganisationRef + "'"),
                    "null"
                    );
                long budgettypId = getHashtableValue(_budgettypIdTable, budgettypRef, -1);
                string budgettypIdString = budgettypId == -1 ? "null" : budgettypId.ToString();

                // funktion
                long funktionId = -1;
                long repFunktionId = -1;
                string mnemonic = "";

                // nur bei aktuellen Salary-Daten
                if (_salaryYear == salaryYear) {
                    functionRef = functionRef == "" ? "NN" : functionRef;
                    function = function == "" ? functionRef : function;
                    mnemonic = function.Substring(0, Math.Min(function.Length, 8));

                    funktionId = getHashtableValue(_funktionIdTable, functionRef, (long)-1);

                    if (funktionId == -1) {
                        funktionId = _db.newId("FUNKTION");
                        sql = "insert into funktion ("
                            + "id,"
                            + " title_de,"
                            + " mnemonic_de,"
                            + "  external_ref,"
                            + "  funktion_group_id"
                            + ") values ("
                            + funktionId + ","
                            + " '" + function + "',"
                            + "'" + mnemonic + "',"
                            + "'" + functionRef + "',"
                            + _funktion_groupId
                            + ")";

                        _db.execute(sql);
                        _funktionIdTable.Add(functionRef, funktionId);
                    }
                    if (repFunctionRef != "") {;
                        repFunction = repFunction == "" ? repFunctionRef : repFunction;
                        mnemonic = repFunction.Substring(0, Math.Min(repFunction.Length, 8));

                        repFunktionId = getHashtableValue(_funktionIdTable, repFunctionRef, (long)-1);

                        if (repFunktionId == -1) {
                            repFunktionId = _db.newId("FUNKTION");
                            sql = "insert into funktion ("
                                + "id,"
                                + " title_de,"
                                + " mnemonic_de,"
                                + "  external_ref,"
                                + "  funktion_group_id"
                                + ") values ("
                                + repFunktionId + ","
                                + " '" + repFunction + "',"
                                + "'" + mnemonic + "',"
                                + "'" + repFunctionRef + "',"
                                + _funktion_groupId
                                + ")";

                            _db.execute(sql);
                            _funktionIdTable.Add(repFunctionRef, repFunktionId);
                        }
                    }
                }

                // employment
                long employmentId = DBColumn.GetValid(
                    _db.lookup(
                    "ID",
                    "EMPLOYMENT",
                    "PERSON_ID = " + personId + " and ORGANISATION_ID = " + _rootId
                    ),
                    (long)-1
                    );
                        
                if (employmentId == -1) {
                    employmentId = _db.newId("EMPLOYMENT");
                    sql = "insert into employment (";

                    if (valueListEmp != null) {
                        foreach (UserAtributeValue val in  valueListEmp) {
                            if (val._tableName == "EMPLOYMENT") {
                                sql += val._columnName + ",";
                            }
                        }
                    }

                    sql += "id,"
                        + "person_id,"
                        + "organisation_id,"
                        + "orgentity_id,"
                        + "budgettyp_id,"
                        + "title_de"
                        + ") values (";

                    if (valueListEmp != null) {
                        foreach (UserAtributeValue val in  valueListEmp) {
                            if (val._tableName == "EMPLOYMENT") {
                                sql = column.AddToSql(
                                    sql,
                                    _employmentTable.Columns[val._columnName],
                                    val._value
                                    ) + ",";
                            }
                        }
                    }

                    sql = column.AddToSql(sql, _employmentTable.Columns["id"], employmentId) + ",";
                    sql = column.AddToSql(sql, _employmentTable.Columns["person_id"], personId) + ",";
                    sql = column.AddToSql(sql, _employmentTable.Columns["organisation_id"], _rootId) + ",";
                    sql += employmentOrgentityIdString + ","
                        + budgettypIdString + ",";
                    sql = column.AddToSql(sql, _employmentTable.Columns["title_de"], function) + ")";
                    _db.execute(sql);
                }
                else if (_salaryYear == salaryYear) { // updates nur mit aktuellen Salary-Daten
                    sql = "update employment set ";

                    if (valueListEmp != null) {
                        foreach (UserAtributeValue val in  valueListEmp) {
                            if (val._tableName == "EMPLOYMENT") {
                                sql = column.AddToSql(
                                    sql + _employmentTable.Columns[val._columnName] + "=",
                                    _employmentTable.Columns[val._columnName],
                                    val._value
                                    ) + ",";
                            }
                        }
                    }

                    sql += "orgentity_id = " + employmentOrgentityIdString + ","
                        + "budgettyp_id = " + budgettypIdString + ",";
                    sql = column.AddToSql(sql + titleAttribute + "=", _employmentTable.Columns[titleAttribute], function);
                    sql += " where id = " + employmentId;
                    _db.execute(sql);
                }

                if (salaryDef) {
                    // eintragen von HAT_BUDGET bei zuständiger OE
                    if (employmentOrgentityIdString != "null") {
                        _db.execute(
                            "update ORGENTITY"
                            + " set HAT_BUDGET = 1"
                            + " where ID = " + employmentOrgentityIdString
                            );
                    }

                    // lohn
                    if (newSalary < 0.001) {
                        newSalary = currentSalary;
                    }

                    long komponenteId = -1;
                    long lohnId = DBColumn.GetValid(
                        _db.lookup(
                        "ID",
                        "LOHN",
                        "EXTERNAL_REF = '" + salaryRef + "'"
                        + " and VARIANTE_ID = "
                        + _varianteIdTable[salaryYear]
                        ),
                        (long)-1
                        );
                        
                    if (lohnId == -1) {
                        lohnId = _db.newId("LOHN");
                        sql = "insert into lohn (";
                            if (valueListSal != null) {
                                foreach (UserAtributeValue val in  valueListSal) {
                                    if (val._tableName == "LOHN") {
                                        sql += val._columnName + ",";
                                    }
                                }
                            }
                        sql += "id,"
                            + "employment_id,"
                            + "variante_id,"
                            + "anzahl_stunden,"
                            + "anzahl_tage,"
                            + "anzahl_loehne,"
                            + "lohnart,"
                            + "istlohn,"
                            + "lohnvorschlag,"
                            + "neuer_lohn,"
                            + "external_ref"
                            + ") values (";

                        if (valueListSal != null) {
                            foreach (UserAtributeValue val in  valueListSal) {
                                if (val._tableName == "LOHN") {
                                    sql = column.AddToSql(
                                        sql,
                                        _lohnTable.Columns[val._columnName],
                                        val._value
                                        ) + ",";
                                }
                            }
                        }
                        sql = column.AddToSql(sql, _lohnTable.Columns["id"], lohnId) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["employment_id"], employmentId) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["variante_id"], _varianteIdTable[salaryYear]) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["anzahl_stunden"], hourYear) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["anzahl_tage"], dayYear) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["anzahl_loehne"], monthYear) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["lohnart"], lohnart(salaryKind)) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["istlohn"], currentSalary) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["lohnvorschlag"], proposedSalary) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["neuer_lohn"], newSalary) + ",";
                        sql = column.AddToSql(sql, _lohnTable.Columns["external_ref"],salaryRef) + ")";
                        _db.execute(sql);
                    }
                    else {
                        // update nur, falls in der aktuellen Lohnrunde die Lohnanpassung noch
                        // nicht bearbeitet ist
                        long id = -1;
                    
                        if (_salaryYear == salaryYear && SalaryComponentList.Length > 0) {
                            string salaryComponentIdList = "";
                        
                            for (int counter = 0; counter < SalaryComponentList.Length; counter++) {
                                komponenteId = getHashtableValue(
                                    _komponenteIdTable,
                                    salaryYear.ToString() + SalaryComponentList[counter],
                                    -1
                                    );

                                if (komponenteId != -1) {
                                    salaryComponentIdList += (salaryComponentIdList == "" ? "" : ", ")
                                        + komponenteId;
                                }
                            }

                            id = DBColumn.GetValid(
                                _db.lookup(
                                "ID",
                                "LOHNKOMPONENTE",
                                "lohn_id = " + lohnId
                                + " and bearbeitungsstatus"
                                + "   != " + (int)LohnModule.EditingState.Pending
                                + " and komponente_id in(" + salaryComponentIdList + ")"
                                ),
                                (long)-1
                                );
                        }
                    
                        if (_salaryYear != salaryYear || id == -1) {
                            sql = "update lohn set ";
                            if (valueListSal != null) {
                                foreach (UserAtributeValue val in  valueListSal) {
                                    if (val._tableName == "LOHN") {
                                        sql = column.AddToSql(
                                            sql + _lohnTable.Columns[val._columnName] + "=",
                                            _lohnTable.Columns[val._columnName],
                                            val._value
                                            ) + ",";
                                    }
                                }
                            }
                            sql = column.AddToSql(sql + "employment_id=",  _lohnTable.Columns["employment_id"], employmentId) + ",";
                            sql = column.AddToSql(sql + "anzahl_stunden=",  _lohnTable.Columns["anzahl_stunden"], hourYear) + ",";
                            sql = column.AddToSql(sql + "anzahl_tage=",  _lohnTable.Columns["anzahl_tage"], dayYear) + ",";
                            sql = column.AddToSql(sql + "anzahl_loehne=",  _lohnTable.Columns["anzahl_loehne"], monthYear) + ",";
                            sql = column.AddToSql(sql + "lohnart=",  _lohnTable.Columns["lohnart"], lohnart(salaryKind)) + ",";
                            sql = column.AddToSql(sql + "istlohn=",  _lohnTable.Columns["istlohn"], currentSalary) + ",";
                            sql = column.AddToSql(sql + "lohnvorschlag=",  _lohnTable.Columns["lohnvorschlag"], proposedSalary) + ",";
                            sql = column.AddToSql(sql + "neuer_lohn=",  _lohnTable.Columns["neuer_lohn"], newSalary);
                            sql += " where id = " + lohnId;
                            _db.execute(sql);
                        }
                    }

                    // lohnkomponenten
                    int componentNumber = Math.Min(
                        Math.Min(rightTable.Count, componentValueTable.Count),
                        ComponentList.Length
                        );
                    string component = "";

                    for (int counter = 0; counter < componentNumber; counter++) {
                        component = ComponentList[counter];
                        komponenteId = getHashtableValue(
                            _komponenteIdTable,
                            salaryYear.ToString() + component,
                            -1
                            );
                    
                        if (komponenteId != -1) {
                            if ((bool)rightTable[component]) { // berechtigt
                                long id = -1;

                                // kein Überschreiben bearbeiteter Komponenten im aktuellen Jahr
                                if (_salaryYear == salaryYear && _editableComponentTable.ContainsKey(component)) {
                                    id = DBColumn.GetValid(
                                        _db.lookup(
                                        "ID",
                                        "LOHNKOMPONENTE",
                                        "LOHN_ID = " + lohnId
                                        + " and BEARBEITUNGSSTATUS"
                                        + "   != " + (int)LohnModule.EditingState.Pending
                                        + " and KOMPONENTE_ID = " + komponenteId
                                        ),
                                        (long)-1
                                        );
                                }

                                if (id == -1) {
                                    loadLohnkomponente(
                                        lohnId,
                                        komponenteId,
                                        (double)componentValueTable[component],
                                        (_editableComponentTable.ContainsKey(component) ? budgettypId : -1),
                                        (componentDescrTable == null ? null : (string)componentDescrTable[component]),
                                        (componentInfoTable == null ? null : (string)componentInfoTable[component])
                                        );
                                }
                            }
                            else if (_salaryYear == salaryYear) { // im aktuellen Jahr löschen, falls nicht berechtigt
                                sql = "delete LOHNKOMPONENTE"
                                    + " where LOHN_ID = " + lohnId
                                    + " and KOMPONENTE_ID = " + komponenteId
                                    + " and BEARBEITUNGSSTATUS"
                                    + "  = " + (int)LohnModule.EditingState.Pending;
                                _db.execute(sql);
                            }
                        }
                    }
                } // salaryDef

                // job
                if (_salaryYear == salaryYear) { // nur bei aktuellen Lohndaten
                    typ = (typ == 0 ? 1 : 0); // bei OGS ist Leiter = 0
                    long jobId = DBColumn.GetValid(
                        _db.lookup(
                        "ID",
                        "JOB",
                        "ORGENTITY_ID = " + orgentityId + " and EMPLOYMENT_ID = " + employmentId
                        ),
                        (long)-1
                        );

                    if (jobId == -1) {
                        jobId = _db.newId("JOB");
                        sql = "insert into job ("
                            + "id,"
                            + " orgentity_id,"
                            + " employment_id,"
                            + " funktion_id,"
                            + (repFunktionId > 0 ? " repfunktion_id," : "")
                            + " title_de,"
                            + " mnemonic_de,"
                            + " typ,"
                            + " engagement,"
                            + " version_flag,"
                            + " hauptfunktion"
                            + ") values (";
                        sql = column.AddToSql(sql, _jobTable.Columns["id"], jobId) + ",";
                        sql = column.AddToSql(
                            sql,
                            _jobTable.Columns["orgentity_id"],
                            orgentityId == -1 ? _rootId : orgentityId
                            ) + ",";
                        sql = column.AddToSql(sql, _jobTable.Columns["employment_id"], employmentId) + ",";
                        sql = column.AddToSql(sql, _jobTable.Columns["funktion_id"], funktionId) + ",";
                        if (repFunktionId > 0) sql = column.AddToSql(sql, _jobTable.Columns["repfunktion_id"], repFunktionId) + ",";
                        sql = column.AddToSql(sql, _jobTable.Columns["title_de"], function) + ",";
                        sql = column.AddToSql(sql, _jobTable.Columns["mnemonic_de"], mnemonic) + ",";
                        sql = column.AddToSql(sql, _jobTable.Columns["typ"], typ) + ",";
                        sql = column.AddToSql(sql, _jobTable.Columns["engagement"], occupation) + ",";
                        sql += "'P',1)";
                    }
                    else {
                        sql = "update job set version_flag = 'P', hauptfunktion = 1,";
                        sql = column.AddToSql(sql + "funktion_id=", _jobTable.Columns["funktion_id"], funktionId) + ",";
                        if (repFunktionId > 0) sql = column.AddToSql(sql+"repfunktion_id=", _jobTable.Columns["repfunktion_id"], repFunktionId) + ",";
                        sql = column.AddToSql(sql + titleAttribute + "=", _jobTable.Columns[titleAttribute], function) + ",";
                        sql = column.AddToSql(sql + mnemonicAttribute + "=", _jobTable.Columns[mnemonicAttribute], mnemonic) + ",";
                        sql = column.AddToSql(sql + "typ=", _jobTable.Columns["typ"], typ) + ",";
                        sql = column.AddToSql(sql + "engagement=", _jobTable.Columns["engagement"], occupation);
                        sql += " where id = " + jobId;
                    }

                    _db.execute(sql);
                }
            }
        }

        /// <summary>
        /// Suchen einer Komponente und gegebenenfalls eine zufügen
        /// </summary>
        /// <param name="bezeichnung"></param>
        /// <param name="varianteId"></param>
        /// <returns>Id der Komponente</returns>
        private long createMissingKomponente(string bezeichnung, long varianteId) {
            long returnValue = DBColumn.GetValid(
                _db.lookup(
                "ID", 
                "KOMPONENTE",
                "BEZEICHNUNG = '" + bezeichnung + "' and VARIANTE_ID = " + varianteId
                ),
                (long)-1
                );

            if (returnValue == -1) {
                returnValue = _db.newId("KOMPONENTE");
                string sql = "insert into KOMPONENTE ("
                    + " ID, VARIANTE_ID, BEZEICHNUNG"
                    + ") values ("
                    + returnValue + ", " + varianteId + ", '" + bezeichnung + "'"
                    +")";
                _db.execute(sql);
            }

            return returnValue;
        }

        /// <summary>
        /// Lohnkomponente in DB einfügen oder updaten
        /// </summary>
        /// <param name="lohnId"></param>
        /// <param name="komponenteId"></param>
        /// <param name="betrag"></param>
        private void loadLohnkomponente(
            long lohnId,
            long komponenteId,
            double betrag,
            long budgettypId,
            string description,
            string info
            ) {
            string sql = "";
            long orgentityId = DBColumn.GetValid(
                _db.lookup(
                "E.ORGENTITY_ID",
                "EMPLOYMENT E, LOHN L",
                "L.ID = " + lohnId
                + " and E.ID = L.EMPLOYMENT_ID"
                ),
                (long)-1
                );
            string budgetIdString = DBColumn.GetValid(
                _db.lookup(
                "ID",
                "BUDGET",
                "KOMPONENTE_ID = " + komponenteId
                + " and ORGENTITY_ID = " + orgentityId
                + (budgettypId == -1 ? "" : (" and BUDGETTYP_ID = " + budgettypId))
                ),
                "null"
                );
            DBColumn column = new DBColumn();

            long lohnkomponenteId = DBColumn.GetValid(
                _db.lookup(
                "ID",
                "LOHNKOMPONENTE",
                "LOHN_ID = " + lohnId + " and KOMPONENTE_ID = " + komponenteId
                ),
                (long)-1
                );
                      
            if (lohnkomponenteId == -1) {
                sql = "insert into lohnkomponente ("
                    + "lohn_id,"
                    + "komponente_id,"
                    + "budget_id,"
                    + "betrag,"
                    + (description != null ? "kommentar," : "")
                    + (info != null ? "information," : "")
                    + "bearbeitungsstatus"
                    + ") values (";
                sql = column.AddToSql(sql, _lohnkomponenteTable.Columns["lohn_id"], lohnId) + ",";
                sql = column.AddToSql(sql, _lohnkomponenteTable.Columns["komponente_id"], komponenteId) + ",";
                sql += budgetIdString + ",";
                sql = column.AddToSql(sql, _lohnkomponenteTable.Columns["betrag"], betrag) + ",";
                if (description != null) sql = column.AddToSql(sql, _lohnkomponenteTable.Columns["kommentar"], description) + ",";
                if (info != null) sql = column.AddToSql(sql, _lohnkomponenteTable.Columns["information"], info) + ",";
                sql = column.AddToSql(sql, _lohnkomponenteTable.Columns["bearbeitungsstatus"], (int)LohnModule.EditingState.Pending) + ")";
            }
            else {
                sql = "update lohnkomponente set budget_id = " + budgetIdString;
                sql += column.AddToSql(",betrag=",  _lohnkomponenteTable.Columns["betrag"], betrag);
                if (description != null) sql += column.AddToSql(",kommentar=",  _lohnkomponenteTable.Columns["kommentar"], description);
                if (info != null) sql += column.AddToSql(",information=",  _lohnkomponenteTable.Columns["information"], info);
                sql += " where id = " + lohnkomponenteId;
            }

            _db.execute(sql);
        }

        /// <summary>
        /// Umcodieren von der OGS-Codierung in die Codierung des Lohnmoduls
        /// </summary>
        /// <param name="salaryKind"></param>
        /// <returns></returns>
        private int lohnart(int salaryKind) {
            int returnValue = salaryKind;

            switch (salaryKind) {
            case (int)LohnModule.SalaryKind.annualWage:
                returnValue = 1;
                break;
            case (int)LohnModule.SalaryKind.monthlyWage:
                returnValue = 2;
                break;
            case (int)LohnModule.SalaryKind.dailyWage:
                returnValue = 3;
                break;
            case (int)LohnModule.SalaryKind.hourlyWage:
                returnValue = 4;
                break;
            default:
                break;
            }

            return returnValue;
        }

        /// <summary>
        /// Umcodieren von der Schnittstellen in die Psoft-Codierung
        /// </summary>
        /// <param name="geschlecht"></param>
        /// <returns></returns>
        private int sex(string geschlecht) {
            int returnValue = 0;

            switch (geschlecht) {
            case "m":
                returnValue = 1;
                break;
            case "f":
                returnValue = 2;
                break;
            }

            return returnValue;
        }

        /// <summary>
        /// Holt den long-Wert aus der Hashtable und behandelt den Fall, wenn
        /// der key in der Table nicht gefunden wird
        /// </summary>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private long getHashtableValue(Hashtable table, object key, long defaultValue) {
            long returnValue = defaultValue;

            if (table.ContainsKey(key) && table[key] is long) {
                returnValue = (long)table[key];
            }

            return returnValue;
        }
        
        private int assertError(int errorCode) {
            if (errorCode < 0) {
                string errorMessage = GetError(errorCode);
                
                throw new TransferException("("+errorCode+ ") "+errorMessage);
            }
            return errorCode;
        }
    }
}
