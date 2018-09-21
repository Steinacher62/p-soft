
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System.Data;
using System.Xml;


namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Purpose of this class is the user defined extraction of data
    /// contained in the database into XML code. 
    /// The database table name and the fields to be extracted must be given.
    ///
    /// </summary>
    public class DataToXml
    {

        const string NAMESPACE = "";
        const string DBTABLENAME = "tableName";
        const string DATA = "data";
        const string TRANSLATION = "translations";


        /// <summary>
        /// Name of the table from which data must be extracted.
        /// </summary>
        private string DbTableName { get; set; }

        /// <summary>
        /// Key used to identify the rows to be extracted.
        /// </summary>
        private string DbColumnKeyName { get; set; }

        /// <summary>
        /// Reference for database access.
        /// </summary>
        private DBData DB
        {
            get;
            set;
        }

        protected LanguageMapper Mapper
        {
            get;
            set;
        }


        /// <summary>
        /// The parent DataToXml object.
        /// </summary>
        public DataToXml Parent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private XmlElement root;

        /// <summary>
        /// 
        /// </summary>
        private System.Collections.ArrayList columns = new System.Collections.ArrayList();


        private bool HasId
        {
            get;
            set;
        }

        public string XmlRootTagName
        {
            get;
            private set;
        }

        /// <summary>
        /// Create root element. 
        /// </summary>
        /// <param name="key">The key to which the id refers</param>
        /// <param name="tableName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static DataToXml createRoot(string tableName,
                                           DBData db,
                                           LanguageMapper mapper,
                                           bool writeId, 
                                           string xmlRootTagName)
        {
            DataToXml rootnode = new DataToXml(tableName);
            rootnode.DB = db;
            rootnode.DbColumnKeyName = "ID"; // This is used ib the SQL query. 
            rootnode.HasId = writeId;
            rootnode.Mapper = mapper;
            rootnode.XmlRootTagName = xmlRootTagName;
            if (writeId)
            {
                rootnode.addNodeValue("ID", "id");
            }

            return rootnode;
        }


        /// <summary>
        /// Generates and returns the translation XML structure
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        /// 
        public XmlElement generateTranslationStructure(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement(TRANSLATION);
            XmlElement elm = doc.CreateElement(this.XmlRootTagName);

            //original table name
            elm.SetAttributeNode(DBTABLENAME, NAMESPACE);
            elm.SetAttribute(DBTABLENAME, this.DbTableName);

            fillInTranslation(doc, elm);
            root.AppendChild(elm);
            return root;
        }

        private void fillInTranslation(XmlDocument doc, XmlElement pre)
        {
            foreach (ColumnDescription cd in this.columns)
            {
                XmlElement elm = doc.CreateElement(cd.XmlName);

                //original table name
                elm.SetAttributeNode(DBTABLENAME, NAMESPACE);
                elm.SetAttribute(DBTABLENAME, cd.Reference.DbTableName);


                if (cd.Nodetype == ColumnDescription.NodeType.NODE_VALUE)
                {
                    cd.translateTo(elm);
                }
                else if (cd.Nodetype != ColumnDescription.NodeType.NODE_RECURSIVE_REF)
                {
                    cd.Reference.fillInTranslation(doc, elm);
                }
                else
                {
                    continue;
                }
                pre.AppendChild(elm);  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbColumnKeyName"></param>
        /// <param name="dbTableName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static DataToXml createRoot(string dbTableName, DBData db, LanguageMapper mapper, string xmlRootTagName)
        {
            return createRoot(dbTableName, db, mapper, false, xmlRootTagName);
        }




        /// <summary>
        /// Create concatenated element
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tableName"></param>
        /// <param name="xmlTableName"></param>
        /// <returns></returns>
        public DataToXml createChildOne(string tableColumnName, string tableName, string xmlTableName)
        {
            DataToXml refnode = new DataToXml(tableName);
            refnode.DbColumnKeyName = tableColumnName;
            copyValues(refnode);
            if (HasId)
            {
                refnode.addNodeValue("ID", "id");
            }
            this.columns.Add(new ColumnDescription(refnode, tableColumnName, xmlTableName));
            return refnode;
        }

        /// <summary>
        /// Create concatenated element
        /// </summary>
        /// <param name="dbColumnReferencingID">foreign key name on the table to be looked for.
        /// For example PROJECT_ID in the table PHASE</param>
        /// <param name="tableName">Name of the associated table. For example "PHASE".</param>
        /// <param name="xmlTableName">Name of the xml tag (the same for any data entry found).</param>
        /// <param name="tagTreeNodeName">Name of the dataset tag</param>
        /// <returns></returns>
        public DataToXml createChildMany(string dbColumnReferencingID, string tableName, string xmlTableName, string tagTreeNodeName)
        {
            DataToXml refnode = new DataToXml(tableName);
            copyValues(refnode);
            if (HasId)
            {
                refnode.addNodeValue("ID", "id");
            }
            this.columns.Add(new ColumnDescription(refnode, dbColumnReferencingID, xmlTableName, tagTreeNodeName));
            return refnode;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbColumnReferencingID">For example "PARENT_ID" in the table PROJECT</param>
        /// <param name="tableName"></param>
        /// <param name="xmlTableName"></param>
        /// <param name="tagTreeNodeName"></param>
        /// <returns></returns>
        public DataToXml createChildManyRecursive(string dbColumnReferencingID, string tableName, string xmlTableName, string tagTreeNodeName)
        {
            DataToXml refnode = new DataToXml(tableName);
            refnode.DbColumnKeyName = dbColumnReferencingID;
            refnode.columns = this.columns; // same structure as the parent
            copyValues(refnode);
            this.columns.Add(new ColumnDescription(refnode, dbColumnReferencingID, xmlTableName, tagTreeNodeName, true));
            return refnode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refnode"></param>
        private void copyValues(DataToXml refnode)
        {
            refnode.Parent = this;
            refnode.DB = this.DB;
            refnode.HasId = this.HasId;
            refnode.Mapper = this.Mapper;
            refnode.XmlRootTagName = this.XmlRootTagName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName">The name of the current table from where
        /// data must be extracted</param>
        /// <param name="db"></param>
        /// <param name="session"></param>
        private DataToXml(string tableName)
        {
            DbTableName = tableName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="xmlName"></param>
        /// <returns></returns>
        public ColumnDescription addNodeValue(string name, string xmlName)
        {
            ColumnDescription nodeValue = new ColumnDescription(this, name, xmlName, false);
            columns.Add(nodeValue);
            return nodeValue;
        }

        /// <summary>
        /// Call-back the user defined procedure with id value and this as arguments.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userDefined"></param>
        /// <returns></returns>
        public ColumnDescription addIdNode(string xmlName, DelegateConversionPrototype userDefinedProcedure)
        {
            ColumnDescription nodeValue = new ColumnDescription(this, "ID", xmlName, false);
            columns.Add(nodeValue);
            nodeValue.convertUsingDelegate(userDefinedProcedure);
            return nodeValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns>the XML document</returns>
        /// <exception cref="DataToXml.DataToXmlException">if not called by the root node</exception>
        public XmlElement extractData(XmlDocument document, long id)
        {
            if (this.Parent != null)
            {
                //this is not the root node, error
                throw new DataToXml.DataToXmlException(DataToXmlException.ErrorCode.SW_ERROR);
            }

            root = document.CreateElement(this.XmlRootTagName);
            int nrRows;

            root = extractData(document, root, this.DbColumnKeyName, id, out nrRows);

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="translation"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static XmlElement mergeDataWithTranslation(XmlDocument document, XmlElement data, XmlElement translation, string title)
        {
            XmlElement report = document.CreateElement(title);
            XmlElement values = document.CreateElement(DATA);
            values.AppendChild(data);
            report.AppendChild(values);
            report.AppendChild(translation);

            return report;
        }


        /// <summary>
        /// 
        /// </summary>
        public long CurrentId { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xmlparent"></param>
        /// <param name="keyName"></param>
        /// <param name="id"></param>
        /// <param name="nrRows"></param>
        /// <returns></returns>
        private XmlElement extractData(XmlDocument document, XmlElement xmlparent, string keyName, long id, out int nrRows)
        {
            string sql = "select * from " + this.DbTableName + " where " + keyName + " = " + id;
            DataTable data = DB.getDataTableExt(sql, this.DbTableName);

            // For each row, print the values of each column.
            nrRows = data.Rows.Count;
            foreach (DataRow row in data.Rows)
            {
                this.CurrentId = (long)row["ID"]; //calculate the current id (may be used by the translation procedures)
                XmlElement before = xmlparent; //remember the parent 
                foreach (ColumnDescription columnDescription in this.columns)
                {
                    string value = "";
                    if (!columnDescription.columnNameIsReferenceId())
                    {
                        if (nrRows > 1)
                        {
                            //Error: There should be only one row in this case.
                        }
                        if (columnDescription.DbColumnName != null && !data.Columns.Contains(columnDescription.DbColumnName))
                        {
                            //Error: Given column name not found in this table.
                            continue;
                        }
                        else
                        {
                            value = row[columnDescription.DbColumnName].ToString();
                        }
                    }
                    else
                    {
                        // in case of an "external" or "recursive" node this must be the id value of the referencing table
                        value = row["ID"].ToString();
                    }
                    columnDescription.resolve(document, xmlparent, value);
                }
                xmlparent = before;
                nrRows--;
                if (nrRows > 0)
                {
                    //add the next parent for further processing
                    XmlElement parentNode = xmlparent.ParentNode as XmlElement;
                    xmlparent = document.CreateElement(xmlparent.Name) as XmlElement;
                    parentNode.AppendChild(xmlparent);
                }
            }
            nrRows = data.Rows.Count;
            return xmlparent;
        }

        /// <summary>
        /// delegate declaration used for value translation if any
        /// </summary>
        /// <param name="value">Normally the id value, but can also be any value.</param>
        /// <returns>The converted value</returns>
        public delegate string DelegateConversionPrototype(string value, DataToXml current);



        /// <summary>
        /// 
        /// </summary>
        public class ColumnDescription
        {

            const string VALUE = "value";
            const string TRANSLATION = "translation";


            public enum NodeType
            {
                NODE_VALUE,
                NODE_SIMPLE_REF,
                NODE_ONE_TO_MANY_REF,
                NODE_RECURSIVE_REF
            }


            public string DbColumnName { get { return this.dbtableColumnName; } }
            public string XmlName { get; set; }

            public NodeType Nodetype { get { return this.type; } }

            string dbtableColumnName;
            NodeType type;


            public string TreeNodeName { get { return this.treeNodeName; } }
            string treeNodeName;

            public bool Resolved { get { return this.resolved; } }
            bool asAttribute = true;
            bool resolved = false;


            public DataToXml Reference { get { return this.dataToXml; } }
            DataToXml dataToXml;



            /// <summary>
            /// 
            /// </summary>
            System.Collections.Hashtable translator = new System.Collections.Hashtable();

            /// <summary>
            /// 
            /// </summary>
            enum ConversionMethod
            {
                NONE,
                ENUMS,
                DATE,
                DELEGATE
            }


            bool isCustomTranslation = false;
            string customTranslation = "";


            /// <summary>
            /// true if we want the translation to appear as data tag attribute
            /// </summary>
            bool isInlineTranslation = false;

            public bool IsInlineTranslation
            {
                set { this.isInlineTranslation = value; }
            }

            /// <summary>
            /// data value convertion method (if any).
            /// </summary>
            ConversionMethod conversionMethod = ConversionMethod.NONE;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="enumsValues"></param>
            public void convertUsingEnum(string[] enumsValues)
            {
                this.conversionMethod = ConversionMethod.ENUMS;
                for (int k = 0; k < enumsValues.Length; k++)
                {
                    translator[k] = enumsValues[k];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="translation"></param>
            public void setCustomTranslation(string translation)
            {
                this.isCustomTranslation = true;
                this.customTranslation = translation;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="enumsValues"></param>
            public void convertUsingEnum(System.Collections.ArrayList enumsValues)
            {
                this.conversionMethod = ConversionMethod.ENUMS;
                for (int k = 0; k < enumsValues.Count; k++)
                {
                    translator[k] = enumsValues[k];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="translationProcedure">delegate: procedure implemented in order to translate</param>
            public ColumnDescription convertUsingDelegate(DelegateConversionPrototype translationProcedure)
            {
                this.conversionMethod = ConversionMethod.DELEGATE;
                this.conversionProcedure = translationProcedure;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            private DelegateConversionPrototype conversionProcedure;

            /// <summary>
            /// 
            /// </summary>
            public void convertToShortDate()
            {
                this.conversionMethod = ConversionMethod.DATE;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataToXml"></param>
            /// <param name="name"></param>
            /// <param name="asAttribute"></param>
            public ColumnDescription(DataToXml dataToXml, string columnName, string xmlColumnName, bool asAttribute)
            {
                this.dataToXml = dataToXml;
                this.dbtableColumnName = columnName;
                this.XmlName = xmlColumnName;
                this.type = NodeType.NODE_VALUE;
                this.asAttribute = asAttribute;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataToXml"></param>
            /// <param name="referenceName"></param>
            /// <param name="xmlTableName"></param>
            public ColumnDescription(DataToXml dataToXml, string tableColumnName, string xmlName)
            {
                this.dataToXml = dataToXml;
                this.dbtableColumnName = tableColumnName; //foreign key name on the reference table or foreign key name on this table
                this.type = NodeType.NODE_SIMPLE_REF;

                this.XmlName = xmlName;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataToXml"></param>
            /// <param name="foreignKeyName"></param>
            /// <param name="xmlTableName"></param>
            /// <param name="treeNodeName"></param>
            public ColumnDescription(DataToXml dataToXml, string foreignKeyName, string xmlTableName, string treeNodeName)
            {
                this.dataToXml = dataToXml;
                this.dbtableColumnName = foreignKeyName; //foreign key name on the reference table or foreign key name on this table
                this.type = NodeType.NODE_ONE_TO_MANY_REF;

                this.XmlName = xmlTableName;
                this.treeNodeName = treeNodeName;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataToXml"></param>
            /// <param name="foreignKeyName"></param>
            /// <param name="xmlTableName"></param>
            /// <param name="treeNodeName"></param>
            /// <param name="recursive"></param>
            public ColumnDescription(DataToXml dataToXml, string foreignKeyName, string xmlTableName, string treeNodeName, bool recursive)
            {
                this.dataToXml = dataToXml;
                this.dbtableColumnName = foreignKeyName; //foreign key name on the reference table or foreign key name on this table
                if (recursive)
                    this.type = NodeType.NODE_RECURSIVE_REF;
                else
                    this.type = NodeType.NODE_ONE_TO_MANY_REF;
                this.XmlName = xmlTableName;
                this.treeNodeName = treeNodeName;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="element"></param>
            /// <param name="value"></param>
            public void resolve(XmlDocument doc, XmlElement element, string value)
            {
                int nrOfRows = 0;
                switch (this.type)
                {
                    case NodeType.NODE_VALUE:
                        value = convert(value);
                        if (this.asAttribute)
                        {
                            element.SetAttributeNode(XmlName, NAMESPACE);
                            element.SetAttribute(XmlName, value);
                        }
                        else
                        {
                            XmlElement xmlelmvalue = doc.CreateElement(XmlName) as XmlElement;

                            //translation appering by each value only if tha attribute isInlineTranslation is true
                            if (isInlineTranslation)
                            {
                                xmlelmvalue.SetAttributeNode(TRANSLATION, NAMESPACE);
                                if (!isCustomTranslation)
                                {
                                    string translation = "";

                                    if (!this.DbColumnName.Equals("ID"))
                                    {
                                        translation = this.dataToXml.Mapper.get(this.dataToXml.DbTableName, this.DbColumnName);
                                    }
                                    xmlelmvalue.SetAttribute(TRANSLATION, translation);
                                }
                                else
                                {
                                    xmlelmvalue.SetAttribute(TRANSLATION, this.customTranslation);
                                }
                                element.AppendChild(xmlelmvalue);
                            }
                            //value
                            xmlelmvalue.SetAttributeNode(VALUE, NAMESPACE);
                            xmlelmvalue.SetAttribute(VALUE, value);
                            element.AppendChild(xmlelmvalue);
                        }
                        break;
                    case NodeType.NODE_SIMPLE_REF:
                        {
                            XmlElement newElm = element.AppendChild(doc.CreateElement(XmlName)) as XmlElement;
                            long result = 0;
                            if (long.TryParse(value, out result))
                            {
                                this.dataToXml.extractData(doc, newElm, "ID", result, out nrOfRows);
                            }
                        }
                        break;
                    case NodeType.NODE_ONE_TO_MANY_REF:
                        {
                            XmlElement newElm = element.AppendChild(doc.CreateElement(this.TreeNodeName)) as XmlElement;
                            newElm = newElm.AppendChild(doc.CreateElement(XmlName)) as XmlElement;
                            long idvalue = -1;
                            if (long.TryParse(value, out idvalue))
                            {
                                this.dataToXml.extractData(doc, newElm, this.dbtableColumnName, idvalue, out nrOfRows);
                            }
                        }
                        break;
                    case NodeType.NODE_RECURSIVE_REF:
                        {
                            XmlElement recElm = doc.CreateElement(this.TreeNodeName);
                            XmlElement newElm = element.AppendChild(recElm) as XmlElement;
                            newElm = newElm.AppendChild(doc.CreateElement(XmlName)) as XmlElement;
                            this.dataToXml.columns = this.dataToXml.Parent.columns; //recursive (if not already done in constructor)
                            long idvalue = -1;
                            if (long.TryParse(value, out idvalue))
                            {
                                this.dataToXml.extractData(doc, newElm, this.dbtableColumnName, long.Parse(value), out nrOfRows);
                            }
                            if (nrOfRows == 0)
                            {
                                newElm.ParentNode.ParentNode.RemoveChild(recElm);
                            }
                        }
                        break;
                }
                resolved = true;
            }

            /// <summary>
            /// The passed argument is filled in with translation data.
            /// </summary>
            /// <param name="elm"></param>
            public void translateTo(XmlElement elm)
            {
                //translation attribute
                elm.SetAttributeNode(TRANSLATION, NAMESPACE);
                
                //original column name attribute
                if (!this.isCustomTranslation)
                {
                    elm.SetAttributeNode(DBTABLENAME, NAMESPACE);
                    elm.SetAttribute(DBTABLENAME, this.DbColumnName);
                }
                else
                {
                    elm.SetAttribute(DBTABLENAME, "");
                    //elm.SetAttributeNode("custom", NAMESPACE);
                    //elm.SetAttribute("custom", "true");
                }

                if (!this.isCustomTranslation)
                {
                    string translation = "";

                    if (!this.DbColumnName.Equals("ID"))
                    {
                        translation = this.dataToXml.Mapper.get(this.dataToXml.DbTableName, this.DbColumnName);
                    }
                    elm.SetAttribute(TRANSLATION, translation);
                }
                else
                {
                    elm.SetAttribute(TRANSLATION, this.customTranslation);
                }

            }

            /// <summary>
            /// data value conversion procedure.
            /// </summary>
            /// <param name="orgValue"></param>
            /// <returns></returns>
            private string convert(string orgValue)
            {
                switch (this.conversionMethod)
                {
                    case ConversionMethod.NONE:
                        return orgValue;
                    case ConversionMethod.ENUMS:
                        int res = 0;
                        if (int.TryParse(orgValue, out res))
                        {
                            return this.translator[res] as string;
                        }
                        else if (orgValue == "")
                        {
                            return "null";
                        }
                        else
                        {
                            return "***Error: Conversion error";
                        }
                    case ConversionMethod.DATE:
                        {
                            //remove time from date
                            int index = orgValue.IndexOf(" ");
                            if (index > 0)
                                return orgValue.Substring(0, index);
                            else
                                return orgValue;
                        }
                    case ConversionMethod.DELEGATE:
                        {
                            return this.conversionProcedure(orgValue, this.dataToXml);
                        }
                    default:
                        return "***Error: Missing conversion";
                }
            }

            /// <summary>
            /// return true if data is on an external table or recursive.
            /// </summary>
            /// <returns></returns>
            internal bool columnNameIsReferenceId()
            {
                return (Nodetype == NodeType.NODE_ONE_TO_MANY_REF ||
                        Nodetype == NodeType.NODE_RECURSIVE_REF);
            }
        }



        /// <summary>
        /// Exception class.
        /// </summary>
        class DataToXmlException : System.ApplicationException
        {
            public enum ErrorCode
            {
                SW_ERROR
            };

            public ErrorCode Error
            {
                get;
                private set;
            }

            public DataToXmlException(ErrorCode code)
            {
                this.Error = code;
            }
        }


    }


}
