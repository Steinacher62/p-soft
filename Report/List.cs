using ch.appl.psoft.db;
using System;
using System.Data;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for Lsit.
    /// </summary>
    public class List {
        protected DBData _db = null;
        protected bool _append = false;
        protected bool _extend = false;
        private bool doDisconnect = false;

        public List() {
        }

        public virtual void open(DBData db) {
            if (_append) return;
            if (db != null && !db.isConnected) {
                db.connect();
                doDisconnect = true;
            }
            _db = db;
        }
        public virtual void close() {
            if (_extend) return;
            if (_db != null) {
                if (doDisconnect) _db.disconnect();
                _db = null;
            }
            doDisconnect = false;
        }
        public virtual bool prepareLayout(long id,
            out DataTable layout, 
            out DataTable headerRows, 
            out DataTable columns, 
            ref DataTable data,
            string[] substituteValues) {

            string sql = "select * from reportlayout where id = "+id;

            headerRows = null;
            columns = null;
            layout = _db.getDataTable(sql);
            if (layout.Rows.Count > 0) {
                sql = "select * from reportheader where reportlayout_ref = "+layout.Rows[0]["id"]+" order by rownumber,halign";
                headerRows = _db.getDataTable(sql);
                sql = "select * from reportcolumn where reportlayout_ref = "+layout.Rows[0]["id"]+" order by columnnumber";
                columns = _db.getDataTable(sql);

                if (data == null) {
                    sql = substitute(layout.Rows[0][_db.langAttrName("reportlayout","sql")].ToString(),substituteValues);
                    if (DBColumn.IsNull(layout.Rows[0]["dbtable"])) data = _db.getDataTable(sql);
                    else data = _db.getDataTableExt(sql,layout.Rows[0]["dbtable"].ToString());
                }
                return true;
            }
            return false;
        }
        public virtual string substitute(string text, params string[] substituteValues) {
            int idx = 0;
            string rep = text;
            
            foreach (string s in substituteValues) {
                rep = rep.Replace("$"+idx,s);
                idx++;
            }
            while (true) {
                idx = rep.IndexOf("lookup[");
                if (idx >= 0) {
                    int idx2 = rep.IndexOf("]",idx);
                    string lookup = rep.Substring(idx,idx2-idx+1);
                    string[] param = lookup.Substring(7,lookup.Length-8).Split(';');
                    string lookupValue = "";

                    if (param.Length == 4)
                        lookupValue = _db.lookup(param[0],param[1],param[2],bool.Parse(param[3]));
                    if (param.Length == 5)
                        lookupValue = _db.lookup(param[0],param[1],param[2],param[3],bool.Parse(param[4]));
                    
                    rep = rep.Replace(lookup,lookupValue);
                }
                else {
                    idx = rep.IndexOf("shortDate[]");
                    if (idx >= 0) rep = rep.Replace("shortDate[]",DateTime.Now.ToString(_db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern));
                    else break;
                }
            }
            return rep;
        }
    }
}
