using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Extend node and navigate treeload
    /// </summary>
    /// <param name="response">Response stream to write extensions</param>
    /// <param name="nodeName">name of the node</param>
    /// <param name="row">Datarow</param>
    /// <param name="level">current load level (starting with 0)</param>
    /// <return>true: goon load this branch, false: abort load this branch</return>
    public delegate bool ExtendNodeHandler(HttpResponse response, string nodeName, DataRow row, int level);
    public delegate bool ExtendTextNodeHandler(StringBuilder build, string nodeName, DataRow row, int level);
    public delegate bool ExtendLeafNodeHandler(HttpResponse response, string nodeName, DataRow row);
    /// <summary>
    /// Summary description for Tree.
    /// </summary>
    public class Tree : Access {
        private HttpResponse _response;
        private string _branchTableName = "NODE";
        private string _branchIDColumn = "ID";
        private string _branchIDPrefix = "";
        private string _branchParentColumn = "PARENT_ID";
        private string _branchOrderColumn = "";
        private string _branchLabelColumn = "TITLE";
        private string _branchToolTipColum = "";
        private string _branchURL;
        private string _extendedBranchRestriction = "";
        private string _extendedLeafRestriction = "";
        private int _maxCaptionLength = -1;
        private bool _leafsBeforeBranches = false;

        private string _leafTableName = "";
        private string _leafIDColumn = "ID";
        private string _leafIDPrefix = "";
        private string _leafParentColumn = "NODE_ID";
        private string _leafOrderColumn = "";
        private string _leafLabelColumn = "TITLE";
        private string _leafToolTipColum = "";
        private string _leafURL;
        private string[] _treeNames = new string[0];
        private DBData _db;

        private int _branchTableAuthorisations = 0;
        private int _leafTableAuthorisations = 0;

        /// <summary>
        /// Extend node event.
        /// </summary>
        public event ExtendNodeHandler extendNode = null;
        public event ExtendTextNodeHandler extendTextNode = null;
        public event ExtendLeafNodeHandler extendLeafNode = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response">HTTP response</param>
        public Tree(HttpResponse response, string targetPage) {
            _response = response;
            BranchURL = targetPage;
            LeafURL = targetPage;
        }

        /// <summary>
        /// Constructor for custom table
        /// </summary>
        /// <param name="response">HTTP response</param>
        public Tree(string tableName, HttpResponse response, string targetPage) 
        {
            _branchTableName = tableName;
            _response = response;
            BranchURL = targetPage;
            LeafURL = targetPage;
        }

        /// <summary>
        /// Constructor for custom table with leafs
        /// </summary>
        /// <param name="response">HTTP response</param>
        public Tree(string branchTableName, string leafTableName, string leafParentColumn, HttpResponse response, string branchURL, string leafURL) 
        {
            _branchTableName = branchTableName;
            _leafTableName = leafTableName;
            _leafParentColumn = leafParentColumn;
            _response = response;
            BranchURL = branchURL;
            LeafURL = leafURL;
        }

        /// <summary>
        /// Set extended restriction for tree nodes
        /// </summary>
        public string extendedBranchRestriction {
            set { _extendedBranchRestriction = " and "+value; }
        }
        /// <summary>
        /// Set extended restriction for tree leafs
        /// </summary>
        public string extendedLeafRestriction {
            set { _extendedLeafRestriction = " and "+value; }
        }
        /// <summary>
        /// Overload default branch Tablename 'NODE'
        /// </summary>
        public string BranchTableName 
        {
            set { _branchTableName = value.ToUpper(); }
        }

        /// <summary>
        /// Overload default branch id-Columnname 'ID'
        /// </summary>
        public string BranchIDColumn {
            set { _branchIDColumn = value; }
        }

        /// <summary>
        /// Set prefix for branch ID
        /// </summary>
        public string branchIDPrefix {
            set { _branchIDPrefix = value; }
        }

        /// <summary>
        /// Overload default branch parent-Columnname 'PARENT_ID'
        /// </summary>
        public string BranchParentColumn
        {
            set { _branchParentColumn = value; }
        }

        /// <summary>
        /// Overload default branch order-Columnname 'ORDNUMBER, TITLE'
        /// </summary>
        public string BranchOrderColumn 
        {
            set { _branchOrderColumn = value; }
        }
        
        /// <summary>
        /// Overload default branch Label-Columnname 'TITLE'
        /// </summary>
        public string BranchLabelColumn
        {
            set { _branchLabelColumn = value; }
        }
        
        /// <summary>
        /// Set a column-name used as source for tool-tips for branches
        /// </summary>
        public string BranchToolTipColum {
            set { _branchToolTipColum = value; }
        }

        /// <summary>
        /// Overload default leaf Tablename 'NODE'
        /// </summary>
        public string LeafTableName 
        {
            set { _leafTableName = value.ToUpper(); }
        }
       
        /// <summary>
        /// Overload default leaf id-Columnname 'ID'
        /// </summary>
        public string LeafIDColumn {
            set { _leafIDColumn = value; }
        }
        /// <summary>
        /// Set prefix for leaf ID
        /// </summary>
        public string leafIDPrefix {
            set { _leafIDPrefix = value; }
        }
        
        /// <summary>
        /// Overload default leaf parent-Columnname 'PARENT_ID'
        /// </summary>
        public string LeafParentColumn
        {
            set { _leafParentColumn = value; }
        }
        
        /// <summary>
        /// Overload default leaf order-Columnname 'ORDNUMBER, TITLE'
        /// </summary>
        public string LeafOrderColumn 
        {
            set { _leafOrderColumn = value; }
        }
        
        /// <summary>
        /// Overload default leaf Label-Columnname 'TITLE'
        /// </summary>
        public string LeafLabelColumn {
            set { _leafLabelColumn = value; }
        }
        
        /// <summary>
        /// Set a column-name used as source for tool-tips for leafs
        /// </summary>
        public string LeafToolTipColum {
            set { _leafToolTipColum = value; }
        }

        /// <summary>
        /// Set maximal length of caption. If longer, tool-tip will be displayed.
        /// </summary>
        public int MaxCaptionLength {
            set { _maxCaptionLength = value; }
        }
        
        /// <summary>
        /// Display leafs before branches.
        /// </summary>
        public bool LeafsBeforeBranches{
            set { _leafsBeforeBranches = value; }
        }

        /// <summary>
        /// creates the parent-SQL for the branches
        /// </summary>
        public string BranchParentSQL(string ID) 
        {
            return "select * from " + _branchTableName + " where " + _branchParentColumn + "=" + ID + _extendedBranchRestriction+" order by " + _branchOrderColumn;
        }
        
        /// <summary>
        /// creates the parent-SQL for the leafs
        /// </summary>
        public string LeafParentSQL(string ID) 
        {
            return "select * from " + _leafTableName + " where " + _leafParentColumn + "=" + ID + _extendedLeafRestriction+" order by " + _leafOrderColumn;
        }
        
        /// <summary>
        /// creates the root-SQL
        /// </summary>
        public string rootSQL(string ID) 
        {
            return "select * from " + _branchTableName + " where " + _branchIDColumn + "=" + ID + _extendedBranchRestriction+" order by " + _branchOrderColumn;
        }
        
        /// <summary>
        /// load tree
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rootNodeId"></param>
        /// <param name="rootNodeName"></param>
        public bool build(DBData db, long rootNodeId, string rootNodeName) 
        {
            long[] rootNodes = {rootNodeId};
            string[] names = {rootNodeName};
            return build (db,rootNodes,names);
        }

        /// <summary>
        /// load tree with a set of roots
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rootNodeIds">array of root-nodes</param>
        /// <param name="rootNodeNames">array of root-node names</param>
        public bool build(DBData db, long [] rootNodeIds, string [] rootNodeNames) {
            bool empty = true;
            try {
                _db = db;
                _branchLabelColumn = db.langAttrName(_branchTableName, _branchLabelColumn);
                _leafLabelColumn = db.langAttrName(_leafTableName, _leafLabelColumn);
                if (_branchToolTipColum != ""){
                    _branchToolTipColum = db.langAttrName(_branchTableName, _branchToolTipColum);
                }
                if (_leafToolTipColum != ""){
                    _leafToolTipColum = db.langAttrName(_leafTableName, _leafToolTipColum);
                }
                if (_branchOrderColumn == ""){
                    _branchOrderColumn = db.langAttrName(_branchTableName, "ORDNUMBER") + "," + db.langAttrName(_branchTableName, "TITLE");
                }
                if (_leafOrderColumn == ""){
                    _leafOrderColumn = db.langAttrName(_leafTableName, "ORDNUMBER") + "," + db.langAttrName(_leafTableName, "TITLE");
                }
                string sql = "";
                int nrOfNodes = Math.Min(rootNodeIds.Length, rootNodeNames.Length);
                string label = "";
                string toolTip = "";
                DataTable table,childs;

                if (nrOfNodes <= 0)
                    return false;

                _response.ContentType = "Text/html";
                _response.Write("var nodes = new Array();\n");
                _response.Write("var node = null;\n");

                _branchTableAuthorisations = db.getTableAuthorisations(_branchTableName, true);
                _leafTableAuthorisations = db.getTableAuthorisations(_leafTableName, true);

                for (int i=0; i<nrOfNodes; i++) {
                    sql = rootSQL(rootNodeIds[i].ToString());
                    table = db.getDataTable(sql,_branchTableName);

                    if (table.Rows.Count == 0) continue;

                    if (db.hasAuthorisation(DBData.AUTHORISATION.READ, _branchTableAuthorisations) || db.hasAuthorisation(DBData.AUTHORISATION.READ, db.getRowAuthorisations(table, 0, true, false), db.hasAuthorisation(DBData.AUTHORISATION.READ, _branchTableAuthorisations))) {
                        empty = false;
                        getLabelAndToolTip(table.Rows[0], _branchLabelColumn, _branchToolTipColum, out label, out toolTip);
                        label = this._treeNames.Length <= i || this._treeNames[i] == "" ? label : this._treeNames[i];

                        if (this._branchURL == "")
                            _response.Write("var "+rootNodeNames[i]+" = newNode('" + label + "',null," + toolTip + ");\n");
                        else 
                            _response.Write("var "+rootNodeNames[i]+" = newNode('" + label + "','" + _branchURL.Replace("%ID", rootNodeIds[i].ToString()) + "'," + toolTip + ");\n");

                        if (this.onExtendNode(_response, rootNodeNames[i], table.Rows[0], 0)) {
                            _response.Write("nodes.push("+rootNodeNames[i]+");\n");	//note: push methode is supported from IE version 5.5
                            _response.Write(rootNodeNames[i]+".xID = '" + _branchIDPrefix+rootNodeIds[i] + "';\n");
                            _response.Write(rootNodeNames[i]+".treeID = '" + rootNodeNames[i] + "';\n");
                            _response.Write(rootNodeNames[i]+".cssClass = 'Tree_Header';\n");
                    
                            if (!_leafsBeforeBranches){
                                sql = BranchParentSQL(rootNodeIds[i].ToString());
                                childs = _db.getDataTable(sql, _branchTableName);
                                writeBranches(childs, 0);
                            }
                        
                            if (_leafTableName != "") {
                                sql = LeafParentSQL(rootNodeIds[i].ToString());
                                childs = _db.getDataTable(sql, _leafTableName);
                                writeLeafs(childs, 0);
                            }

                            if (_leafsBeforeBranches){
                                sql = BranchParentSQL(rootNodeIds[i].ToString());
                                childs = _db.getDataTable(sql, _branchTableName);
                                writeBranches(childs, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            return !empty;
        }
        
        /// <summary>
        /// Add Subtree to Node
        /// </summary>
        /// <param name="db">DB</param>
        /// <param name="id">Parent Node ID</param>
        /// <param name="scriptNodename">Node Name on script-file (default: node)</param>
        public void extendTree(DBData db, long id, string scriptNodename, StringBuilder load) {
            string sql = BranchParentSQL(id.ToString());

            _db = db;
            _db.connect();
            try {
                DataTable table = _db.getDataTable(sql, _branchTableName);

                load.Append("nodes.push(").Append(scriptNodename).Append(");\n");
                writeBranches(load,table,0);
                load.Append("nodes.pop();\n");
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }

        /// <summary>
        /// Set/Get name of tree
        /// </summary>
        public string treeName {
            get {return _treeNames[0];}
            set {_treeNames = new string[] {value};}
        }

        /// <summary>
        /// Set/Get names of tree
        /// </summary>
        public string[] treeNames {
            get {return _treeNames;}
            set {_treeNames = value;}
        }

        /// <summary>
        /// Set/get URL for tree branches
        /// </summary>
        public string BranchURL {
            get {return _branchURL;}
            set {_branchURL = value.Replace("%25ID", "%ID");}
        }

        /// <summary>
        /// Set/get URL for tree leafs
        /// </summary>
        public string LeafURL {
            get {return _leafURL;}
            set {_leafURL = value.Replace("%25ID", "%ID");}
        }

        private void getLabelAndToolTip(DataRow row, string labelColumn, string toolTipColumn, out string label, out string toolTip){
            label = toolTip = "";
            if (toolTipColumn != ""){
                toolTip = PSOFTConvert.ToJavascript(row[toolTipColumn].ToString());
            }
            label = PSOFTConvert.ToJavascript(row[labelColumn].ToString());
            if (_maxCaptionLength > 2 && label.Length > (_maxCaptionLength+2)){
                toolTip = label + (toolTip != ""? ":\\r\\n" + toolTip : "");
                label = label.Substring(0, _maxCaptionLength) + "...";
            }
            if (toolTip == ""){
                toolTip = "null";
            }
            else{
                toolTip = "'" + toolTip + "'";
            }
        }

        private void writeBranches(DataTable table, int level) {
            string sql;
            DataTable childs;
            string label = "";
            string toolTip = "";

            level++;
            foreach (DataRow r in table.Rows) {
                if (rowAccess (table, r,
                    _db.hasAuthorisation(DBData.AUTHORISATION.READ, _branchTableAuthorisations) || _db.hasAuthorisation(DBData.AUTHORISATION.READ, _db.getRowAuthorisations(table, r, true, false), _db.hasAuthorisation(DBData.AUTHORISATION.READ, _branchTableAuthorisations)),
                    DBData.AUTHORISATION.READ)) 
                {
                    _response.Write("node = nodes[nodes.length-1];\n");
                    getLabelAndToolTip(r, _branchLabelColumn, _branchToolTipColum, out label, out toolTip);

                    if (_branchURL == "")
                        _response.Write("node = node.addChild(newNode('" + label + "',null," + toolTip + "));\n");
                    else
                        _response.Write("node = node.addChild(newNode('" + label + "','" + _branchURL.Replace("%ID", _branchIDPrefix+r[_branchIDColumn]) + "'," + toolTip + "));\n");
                    _response.Write("node.xID = '" + _branchIDPrefix+r[_branchIDColumn] + "';\n");
                    _response.Write("node.cssClass = 'Tree';\n");
                    if (this.onExtendNode(_response, "node", r, level)) {
                        _response.Write("nodes.push(node);\n");

                        if (!_leafsBeforeBranches){
                            sql = BranchParentSQL(r[_branchIDColumn].ToString());
                            childs = _db.getDataTable(sql, _branchTableName);
                            writeBranches(childs, level);
                        }

                        if (_leafTableName != "")
                        {
                            sql = LeafParentSQL(r[_branchIDColumn].ToString());
                            childs = _db.getDataTable(sql, _leafTableName);
                            writeLeafs(childs, level);
                        }

                        if (_leafsBeforeBranches){
                            sql = BranchParentSQL(r[_branchIDColumn].ToString());
                            childs = _db.getDataTable(sql, _branchTableName);
                            writeBranches(childs, level);
                        }
                        _response.Write("nodes.pop();\n");
                    }
                }
            }
            level--;
        }

        private void writeBranches(StringBuilder extend, DataTable table, int level) {
            string sql;
            DataTable childs;
            string label = "";
            string toolTip = "";

            level++;
            foreach (DataRow r in table.Rows) {
                if (rowAccess (table, r,
                    _db.hasAuthorisation(DBData.AUTHORISATION.READ, _branchTableAuthorisations) || _db.hasAuthorisation(DBData.AUTHORISATION.READ, _db.getRowAuthorisations(table, r, true, false), _db.hasAuthorisation(DBData.AUTHORISATION.READ, _branchTableAuthorisations)),
                    DBData.AUTHORISATION.READ)) 
                {
                    extend.Append("node = nodes[nodes.length-1];\n");
                    getLabelAndToolTip(r, _branchLabelColumn, _branchToolTipColum, out label, out toolTip);

                    if (_branchURL == "")
                        extend.Append("node = node.addChild(newNode('").Append(label).Append("',null," + toolTip + "));\n");
                    else
                        extend.Append("node = node.addChild(newNode('").Append(label).Append("','").Append(_branchURL.Replace("%ID",_branchIDPrefix+r[_branchIDColumn])).Append("'," + toolTip + "));\n");
                    extend.Append("node.xID = '").Append(_branchIDPrefix).Append(r[_branchIDColumn]).Append("';\n");
                    extend.Append("node.cssClass = 'Tree';\n");
                    if (this.onExtendNode(_response,"node",r,level)) {
                        extend.Append("nodes.push(node);\n");

                        if (!_leafsBeforeBranches){
                            sql = BranchParentSQL(r[_branchIDColumn].ToString());
                            childs = _db.getDataTable(sql, _branchTableName);
                            writeBranches(extend, childs, level);
                        }

                        if (_leafTableName != "")
                        {
                            sql = LeafParentSQL(r[_branchIDColumn].ToString());
                            childs = _db.getDataTable(sql, _leafTableName);
                            writeLeafs(extend, childs, level);
                        }

                        if (_leafsBeforeBranches){
                            sql = BranchParentSQL(r[_branchIDColumn].ToString());
                            childs = _db.getDataTable(sql, _branchTableName);
                            writeBranches(extend, childs, level);
                        }
                        extend.Append("nodes.pop();\n");
                    }
                }
            }
            level--;
        }

        private void writeLeafs(DataTable table, int level) 
        {
            string label = "";
            string toolTip = "";
            level++;
            foreach (DataRow r in table.Rows) 
            {
                if (rowAccess (table, r,
                    _db.hasAuthorisation(DBData.AUTHORISATION.READ, _leafTableAuthorisations) || _db.hasAuthorisation(DBData.AUTHORISATION.READ, _db.getRowAuthorisations(table, r, true, false), _db.hasAuthorisation(DBData.AUTHORISATION.READ, _leafTableAuthorisations)),
                    DBData.AUTHORISATION.READ)) 
                {
                    _response.Write("node = nodes[nodes.length-1];\n");
                    getLabelAndToolTip(r, _leafLabelColumn, _leafToolTipColum, out label, out toolTip);

                    if (_leafURL == "")
                        _response.Write("node = node.addChild(newLink('R','" + label + "',null," + toolTip + "));\n");
                    else
                        _response.Write("node = node.addChild(newLink('R','" + label + "','" + _leafURL.Replace("%ID", _leafIDPrefix+r[_leafIDColumn]) + "'," + toolTip + "));\n");

                    _response.Write("node.xID = '" + _leafIDPrefix+r[_leafIDColumn] + "';\n");
                    _response.Write("node.cssClass = 'Tree';\n");
                    this.onExtendLeafNode(_response,"node",r);
                }
            }
            level--;
        }

        private void writeLeafs(StringBuilder extend, DataTable table, int level) 
        {
            string label = "";
            string toolTip = "";
            level++;
            foreach (DataRow r in table.Rows) 
            {
                if (rowAccess (table, r,
                    _db.hasAuthorisation(DBData.AUTHORISATION.READ, _leafTableAuthorisations) || _db.hasAuthorisation(DBData.AUTHORISATION.READ, _db.getRowAuthorisations(table, r, true, false), _db.hasAuthorisation(DBData.AUTHORISATION.READ, _leafTableAuthorisations)),
                    DBData.AUTHORISATION.READ)) 
                {
                    extend.Append("node = nodes[nodes.length-1];\n");
                    getLabelAndToolTip(r, _leafLabelColumn, _leafToolTipColum, out label, out toolTip);

                    if (_leafURL == "")
                        extend.Append("node = node.addChild(newLink('R','").Append(label).Append("',null," + toolTip + "));\n");
                    else
                        extend.Append("node = node.addChild(newLink('R','").Append(label).Append("','").Append(_leafURL.Replace("%ID",_leafIDPrefix+r[_leafIDColumn])).Append("'," + toolTip + "));\n");

                    extend.Append("node.xID = '").Append(_leafIDPrefix).Append(r[_leafIDColumn]).Append("';\n");
                    extend.Append("node.cssClass = 'Tree';\n");
                }
            }
            level--;
        }

        private bool onExtendNode (HttpResponse response, string nodeName, DataRow row, int level) 
        {
            if (extendNode != null) {
                // Invokes the delegates. 
                return extendNode(response,nodeName, row, level);
            }
            return true;
        }

        private bool onExtendNode (StringBuilder build, string nodeName, DataRow row, int level) {
            if (extendTextNode != null) {
                // Invokes the delegates. 
                return extendTextNode(build,nodeName, row, level);
            }
            return true;
        }

        private bool onExtendLeafNode (HttpResponse response, string nodeName, DataRow row)
        {
            if (extendLeafNode != null)
            {
                // Invokes the delegates.
                return extendLeafNode(response,nodeName,row);
            }
            return true;
        }
    }
}
