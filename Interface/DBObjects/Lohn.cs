using ch.appl.psoft.db;
using System;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Lohn.
    /// </summary>
    public class Lohn : DBObject {
        public Lohn(DBData db, HttpSessionState session) : base(db, session) {
        }

        /// <summary>
        /// Der Funktion zugeordnete Basislohn der relevanten Variante
        /// Falls die Lohntabelle zusätzlich leistungsabhängig ist, wird
        /// der Mittelwert zurückgegeben
        /// </summary>
        /// <param name="functionID"></param>
        /// <returns></returns>
        public double getBasislohn(long funktionId) {
            double returnValue = 0;

            long parametersetId = getParametersetForFunktion(funktionId);
            int lohntabellenSchluessel = DBColumn.GetValid(
                _db.lookup("LOHNTABELLEN_SCHLUESSEL", "PARAMETERSET", "ID = " + parametersetId),
                -1
                );

            switch (lohntabellenSchluessel) {
            case 1: // Funktionswerte
                double funktionswert = DBColumn.GetValid(
                    _db.lookup(
                    "FUNKTIONSWERT",
                    "FUNKTIONSBEWERTUNG",
                    "FUNKTION_ID = " + funktionId
                    + " and GUELTIG_AB <= getdate()"
                    + " and isnull(GUELTIG_BIS, getdate()) >= getdate()"
                    + " and FUNKTIONSWERT is not null"
                    ),
                    (double)0
                    );
                long funktionswertLong = (long)funktionswert; // abschneiden
                returnValue  = DBColumn.GetValid(
                    _db.lookup(
                    "avg(WERT1)",
                    "LOHNTABELLE",
                    "PARAMETERSET_ID = " + parametersetId
                    + " and TYP = 1" // lohntabelle
                    + " and SCHLUESSEL = " + funktionswertLong
                    ),
                    (double)0
                    );
                break;
            case 2: // Funktionen
                returnValue  = DBColumn.GetValid(
                    _db.lookup(
                    "avg(WERT1)",
                    "LOHNTABELLE",
                    "PARAMETERSET_ID = " + parametersetId
                    + " and TYP = 1" // lohntabelle
                    + " and SCHLUESSEL = " + funktionId
                    ),
                    (double)0
                    );
                break;
            case 3: // Funktionsfamilien
                returnValue  = DBColumn.GetValid(
                    _db.lookup(
                    "avg(L.WERT1)",
                    "LOHNTABELLE L inner join FUNKTION F"
                    + " on L.SCHLUESSEL = F.FUNKTIONSFAMILIE_ID",
                    "L.PARAMETERSET_ID = " + parametersetId
                    + " and L.TYP = 1" // lohntabelle
                    + " and F.ID = " + funktionId
                    ),
                    (double)0
                    );
                break;
            default:
                break;
            }

            return returnValue;
        }

        /// <summary>
        /// Get Ist-Lohn
        /// </summary>
        /// <param name="empId">Employment ID</param>
        /// <returns>Istlohn</returns>
        public double getIstLohn(long empId) {
            return _db.lookup("ISTLOHN","LOHN","EMPLOYMENT_ID="+empId+" and VARIANTE_ID="+getVariante(),0.0);
        }
        /// <summary>
        /// Relevante Variante
        /// 
        /// Dezentral ist immer genau eine Variante relevant. Welche es ist, ist noch
        /// nicht klar. Möglichkeiten:
        /// 1. neuste definitve
        /// 2. neuste produktive
        /// 3. Die aktive (variante.hauptvariante = 1) (Diese ist jetzt implementiert)
        /// </summary>
        /// <returns>Id resp -1, falls nicht gefunden</returns>
        public long getVariante() {
            return DBColumn.GetValid(
                _db.lookup("ID", "VARIANTE", "HAUPTVARIANTE = 1"),
                (long)-1
                );
        }

        /// <summary>
        /// Parameterset für Funktion gemäss Levelregel ohne OE-Level (= Level 2)
        /// in der relevanten Variante
        /// </summary>
        /// <param name="funktionId"></param>
        /// <returns>Id resp -1, falls nicht gefunden</returns>
        public long getParametersetForFunktion(long funktionId) {
            long parametersetId = -1;
            long varianteId = getVariante();

            // Welche Levels sind eingeschaltet?
            object[] values = _db.lookup(
                new String[] {"LEVEL_1_AKTIVIERT", "LEVEL_3_AKTIVIERT", "LEVEL_4_AKTIVIERT"},
                "VARIANTE",
                "ID = " + varianteId
                );
            int level_1_aktiviert = DBColumn.GetValid(values[0], 0);
            int level_3_aktiviert = DBColumn.GetValid(values[1], 0);
            int level_4_aktiviert = DBColumn.GetValid(values[2], 0);

            // Parameterset suchen
            if (level_4_aktiviert == 1) {
                parametersetId = DBColumn.GetValid(
                    _db.lookup(
                    "PARAMETERSET_ID",
                    "PARAMETERSETZUTEILUNG", 
                    "FUNKTION_ID = " + funktionId
                    ),
                    (long)-1
                    );
            }

            if (level_3_aktiviert == 1 && parametersetId == -1) {
                parametersetId = DBColumn.GetValid(
                    _db.lookup(
                    "P.PARAMETERSET_ID",
                    "PARAMETERSETZUTEILUNG P"
                    + " inner join FUNKTION F"
                    + " on F.FUNKTIONSFAMILIE_ID = P.FUNKTIONSFAMILIE_ID", 
                    "F.ID = " + funktionId
                    ),
                    (long)-1
                    );
            }

            if (level_1_aktiviert == 1 && parametersetId == -1) {
                parametersetId = DBColumn.GetValid(
                    _db.lookup(
                    "PARAMETERSET_ID",
                    "PARAMETERSETZUTEILUNG", 
                    "VARIANTE_ID = " + varianteId
                    ),
                    (long)-1
                    );
            }

            return parametersetId;
        }

        public int deleteBudget(long id) {
            return _db.execute("delete from budget where id = "+id);
        }

        public int deleteVariante(long id, bool cascade) {
            return _db.execute("delete from variante where id = "+id);
        }
		
        public override int delete(long id, bool cascade) {
            return _db.execute("delete from lohn where id = "+id);
        }

		/* Im Fall, dass ein Budget gelöscht werden muss, werden vorgängig alle Lohnkomponenten 
		 * welche auf das Budget verweisen, historisiert (referenz auf NULL gesetzt)  */
		public int disconnectLohnkomponenten(long budgetID) 
		{
			return _db.execute("update LOHNKOMPONENTE set BUDGET_ID = null where BUDGET_ID = " + budgetID);
		}
    }
}
