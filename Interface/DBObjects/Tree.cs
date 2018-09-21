using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System.Collections;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for Tree.
    /// </summary>
    public class Tree : DBObject 
    {
        private string _tableName = "NODE";
        private string _parentForeignKey = "PARENT_ID";
        private string _IDColumn = "ID";

        public Tree(string tableName, DBData db, HttpSessionState session) : base(db, session) {
            _tableName = tableName;
        }
        
        public override int delete(long id, bool rootEnable) 
        {
            return _db.execute("delete from " + _tableName + " where " + _IDColumn + "=" + id);
        }

        /// <summary>
        /// Test ob id ein Blatt ist
        /// <param name="id">Id</param>
        /// <return>true, falls der Node ein Blatt ist</return>
        /// </summary>
        public bool IsLeaf(long id) {
            object obj = _db.lookup("1",_tableName,_parentForeignKey+"="+id);
            return obj == null;
        }
        
        /// <summary>
        /// Ergänzt eine Liste von Node-Ids durch die Id's aller untergeordneten
        /// Nodes
        /// </summary>
        /// <param name="ids">list of root ids</param>
        /// <returns>ergänzte Liste</returns>
        public ArrayList AddAllSubnodes(ArrayList ids) {
            return new ArrayList(AddAllSubnodes(SQLDB.BuildSQLArray(ids.ToArray())).Split(','));
        }

        /// <summary>
        /// Ergänzt eine Liste von Node-Ids durch die Id's aller untergeordneten
        /// Nodes
        /// </summary>
        /// <param name="nodeIds">Liste von Node-Ids durch ',' getrennt</param>
        /// <returns>ergänzte Liste mit ',' getrennt</returns>
        public string AddAllSubnodes(string nodeIds) {
            if (nodeIds == "")
                return "";
            
            string completedIdList = nodeIds;
            string sql = "SELECT " + _IDColumn + " FROM " + _tableName
                + " WHERE " + _parentForeignKey + " IN(" + nodeIds + ")"
                + " AND " + _IDColumn + " NOT IN(" + nodeIds + ")";
            DataTable data = _db.getDataTable(sql,Logger.VERBOSE);
            
            foreach (DataRow row in data.Rows) {
                completedIdList += "," + AddAllSubnodes(row[0].ToString());
            }
            
            return completedIdList;
        }
        
        /// <summary>
        /// Ergänzt eine Liste von Node-Ids durch die Id's aller untergeordneten
        /// Nodes in der SearchResult-tabelle
        /// </summary>
        /// <param name="nodeIds">Liste von Node-Ids durch ',' getrennt</param>
        /// <param name="resultId">SearchResult-Id</param>
        /// <returns>#rows inserted</returns>
        public int AddAllSubnodes(string nodeIds, long resultId) {
            string sql = "insert into SearchResult (id,tablename,row_id) select " + resultId + ",'" + _tableName + "',id from " + _tableName + " where " + _IDColumn + " in (" + nodeIds + ")";

            if (nodeIds == "") return 0;
            
            int rows = _db.execute(sql,Logger.VERBOSE);
            return rows + AddAllSubnodes(resultId);
        }

        /// <summary>
        /// Ergänzt eine Liste von Node-Ids durch die Id's aller untergeordneten
        /// Nodes in der SearchResult-tabelle
        /// </summary>
        /// <param name="resultId">SearchResult-Id der zu ergäzenden liste</param>
        /// <returns>#rows inserted</returns>
        public int AddAllSubnodes(long resultId) {
            string select = "select row_id from SearchResult where id = " + resultId + " and tablename='" + _tableName + "'";
            string sql = "insert into SearchResult (id,tablename,row_id)"
                + " SELECT " + resultId + ",'" + _tableName + "'," + _IDColumn + " FROM " + _tableName
                + " WHERE " + _parentForeignKey + " IN (" + select + ")"
                + " AND " + _IDColumn + " NOT IN (" + select + ")";
            int rows = 0;
            int inserts;
            
            do {
                inserts = _db.execute(sql,Logger.VERBOSE);
                rows += inserts;
            }
            while (inserts > 0);
            return rows;
        }

        /// <summary>
        /// Returns parentpath to a certain node
        /// </summary>
        /// <param name="nodeID">ID of a node</param>
        /// <param name="inclNode">path inclusive nodeId</param>
        /// <returns></returns>
        public ArrayList GetPath(long nodeID, bool inclNode) {
            ArrayList retValue = new ArrayList();
            string sql = "select " + _parentForeignKey + " from " + _tableName + " where " + _IDColumn + " = " + nodeID;

            DataTable table = _db.getDataTable(sql,Logger.VERBOSE);

            foreach (DataRow row in table.Rows) {
                if (!SQLColumn.IsNull(row[0])) {
                    long nID = (long) row[0];
                    retValue.AddRange(GetPath(nID,true));
                }
            }
            if (inclNode) retValue.Add(nodeID);
            return retValue;
        }

        /// <summary>
        /// Search node in subtree
        /// </summary>
        /// <param name="root">ID of subtree</param>
        /// <param name="search">search id</param>
        /// <returns>search id if found else 0</returns>
        public long Find(long root, long search) {
            ArrayList list = AddAllSubnodes(new ArrayList());

            if (list.Contains(search)) return search;
            return 0;
        }

        /// <summary>
        /// Get Root-node of a tree-node or leaf
        /// </summary>
        /// <param name="node">Tree-node</param>
        /// <returns>root-node of node or 0</returns>
        public long getRoot(long node) {
            string sql = "select " + _parentForeignKey + " from " + _tableName + " where " + _IDColumn + " = ";
            long root = node;
            DataTable table;

            while (true) {
                table = _db.getDataTable(sql+root);
                if (table.Rows.Count == 0) return 0;
                if (SQLColumn.IsNull(table.Rows[0][0])) return root;
                root = (long) table.Rows[0][0];
            }
        }

        /// <summary>
        /// Modi fürs Summieren im Tree
        /// </summary>
        public enum TreeSumMode
        {
            Hierarchic, // Summieren der Werte aller Knoten hierarchieabwärts inklusive des Startknotens
            ChildsOnly  // Summieren der Werte der Child-Knoten
        }

        /// <summary>
        /// Delegate für Wert zu einem Tree-Knoten fürs hierarchische Summieren
        /// </summary>
        public delegate double NodeValue(long nodeId, params object[] parameterList);

        /// <summary>
        /// Summiert Werte im Tree hierarchisch (über alle Knoten und den Startknoten)
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="nodeValue">Liefert den zu summierenden Wert (delegate)</param>
        /// <returns></returns>
        public double getHierarchicTreeSum(long nodeId, NodeValue nodeValue, params object[] parameterList)
        {
            return getTreeSum(nodeId, nodeValue, TreeSumMode.Hierarchic, parameterList);
        }
        
        /// <summary>
        /// Summiert Werte im Tree
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="nodeValue">Liefert den zu summierenden Wert (delegate)</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public double getTreeSum(
            long nodeId,
            NodeValue nodeValue,
            TreeSumMode mode,
            params object[] parameterList
        )
        {
            long id = -1;
            double returnValue = 0;

            switch (mode)
            {
                case TreeSumMode.Hierarchic: // inklusive Wert des Startknotens
                    returnValue = nodeValue(nodeId, parameterList);
                    break;
            }

            _db.connect();

            try
            {
                string sql = "SELECT " + _IDColumn + " FROM " + _tableName
                    + " WHERE " + _parentForeignKey + " = " + nodeId;
                DataTable data = _db.getDataTable(sql, Logger.VERBOSE);
                
                foreach (DataRow row in data.Rows)
                {
                    id = DBColumn.GetValid(row[0], (long)-1);

                    switch (mode)
                    {
                        case TreeSumMode.Hierarchic:
                            returnValue += getTreeSum(id, nodeValue, mode, parameterList); // rekursiv
                            break;
                        case TreeSumMode.ChildsOnly:
                            returnValue = nodeValue(nodeId, parameterList);
                            break;
                    }
                }
            }
            finally
            {
                _db.disconnect();
            }

            return returnValue;
        }
    }
}
