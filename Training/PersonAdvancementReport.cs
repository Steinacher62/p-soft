using ch.appl.psoft.Report;
using System;
using System.Collections;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Training
{
    /// <summary>
    /// Summary description for PersonAdvancementReport.
    /// </summary>
    public class PersonAdvancementReport : PsoftPDFReport
    {
        protected DataTable _competenceLevels = null;
        protected long _personID = -1;
        protected long _advancementID = -1;

        private XmlNode _nTable;
        private XmlNode _nRow;
        private XmlNode _nCell;
        private DataTable _personTable;
        private DataTable _demandTable;

        public PersonAdvancementReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

        public XmlNode AppendAdvancementTable()
        {
            return AppendTable("420,100");
        }

        /* if advancementID < 0, all the persons advancments will be reported  */
        public void createReport(int personID, int advancementID)
        {
            _personID = personID;
            _advancementID = advancementID;

            Interface.DBObjects.Tree dbTree = _db.Tree("TRAININGGROUP");
            Hashtable groupNames = new Hashtable();

            _personTable = _db.Person.getWholeNameMATable(true);
            _demandTable = TrainingModule.getTrainingDemandTable(_db);

            writeHeaderAndFooter(_map.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_REP_PERSONTRAINING), DateTime.Now.ToString("d"));

            // person detail
            _nTable = AppendDetailTable();
            DataTable table = _db.getDataTableExt("select ID, FIRSTNAME, PNAME, PERSONNELNUMBER, PHONE, EMAIL from PERSON where ID=" + _personID, "PERSON");
            addDetail(_db, _nTable, table, "MA", true);

            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createAttribute("padding-top", "10"));
            _nCell = _nRow.AppendChild(createCell());
            appendHLine(520);
            appendVSpace(10);

            _nTable = AppendAdvancementTable();
            _nRow = _nTable.AppendChild(createRow());
            _nRow.Attributes.Append(createClassAttribute("sectionHeader"));
            _nCell = _nRow.AppendChild(createCell());
            _nCell.InnerText = _map.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_REP_ADVACEMENT);

            table = _db.getDataTable("select * from TRAININGGROUP order by ORDNUMBER, ID", "TRAININGGROUP");
            foreach (DataRow row in table.Rows) 
            {
                long trainingGroupID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                string trainingGroupName = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                groupNames.Add(trainingGroupID,trainingGroupName);
                
            }
            long root = -1;
            string sql = "";
            foreach (DataRow row in table.Rows) 
            {
                long trainingGroupID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                if (root < 0)
                {
                    root = dbTree.getRoot(trainingGroupID);
                }
                ArrayList list = dbTree.GetPath(trainingGroupID,false);
                String path = "";
                System.Collections.IEnumerator myEnumerator = list.GetEnumerator();
                int cnt = 0;
                while ( myEnumerator.MoveNext() )
                {
                    if (root.ToString() == (myEnumerator.Current).ToString()) continue;
                    if (groupNames.ContainsKey(myEnumerator.Current))
                    {
                        if (cnt > 0)
                        {
                            path += "/";
                        }
                        cnt++;
                        path += groupNames[myEnumerator.Current];
                    }
                }
                sql = "select * from TRAINING_ADVANCEMENT where PERSON_ID =" + _personID + " and TRAINING_ID in (select ID from TRAINING where TRAININGGROUP_ID = " + trainingGroupID + ") order by DONE_DATE";
                if (_advancementID > -1)
                {
                    sql = "select * from TRAINING_ADVANCEMENT where ID =" + _advancementID + " and TRAINING_ID in (select ID from TRAINING where TRAININGGROUP_ID = " + trainingGroupID + ")";
                }
                DataTable advancementTable = _db.getDataTableExt(sql, "TRAINING_ADVANCEMENT");
                String title = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString(), "");
                if (path != "")
                {
                    title = path + "/" + title;
                }
                createGroup(advancementTable,title);
            }

            sql = "select * from TRAINING_ADVANCEMENT where PERSON_ID =" + _personID + " and TRAINING_ID is null order by DONE_DATE";
            if (_advancementID > -1)
            {
                sql = "select * from TRAINING_ADVANCEMENT where ID =" + _advancementID + " and TRAINING_ID is null";
            }
            DataTable standaloneAdvancementTable = _db.getDataTableExt(sql, "TRAINING_ADVANCEMENT");
            createGroup(standaloneAdvancementTable,_map.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_REP_STANDALONE_ADVACEMENT));

            appendVSpace(20);
            appendHLine(520);
        }

        private void createGroup(DataTable elemTable, String groupTitle)
        {
            if (elemTable.Rows.Count > 0) 
            {
                appendVSpace(20);
                _nTable = AppendAdvancementTable();
                _nRow = _nTable.AppendChild(createRow());
                _nRow.Attributes.Append(createClassAttribute("title"));
                _nCell = _nRow.AppendChild(createCell());
                _nCell.Attributes.Append(createAttribute("colspan", "2"));
                _nCell.InnerText = groupTitle;
                
                int rowCnt = -1;
                foreach (DataRow elemRow in elemTable.Rows)
                {
                    rowCnt++;
                    appendVSpace(8);
                    if (rowCnt > 0)
                    {
                        appendVSpace(8);
                    }                   
                    _nTable = AppendAdvancementTable();

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("subTitle"));
                    _nCell.InnerText = ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "TITLE")].ToString(), "");

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "DESCRIPTION")].ToString(), ""));
                   
                    appendVSpace(10);

                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailName"));
                    _nCell.InnerText = _map.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_REP_CONTROLLING);
                    
                    _nRow = _nTable.AppendChild(createRow());
                    _nCell = _nRow.AppendChild(createCell());
                    _nCell.Attributes.Append(createClassAttribute("detailValue"));
                    addCellText(_nCell, ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "CONTROLLING")].ToString(), ""));

                    String advancmentID = ch.psoft.Util.Validate.GetValid(elemRow[_db.langAttrName(elemRow.Table.TableName, "ID")].ToString(), "");
                    DataTable detailTable = new DataTable();
                    if (Global.isModuleEnabled("ahb"))
                    {
                        detailTable = _db.getDataTableExt("select START_DATE, TOBEDONE_DATE, DONE_DATE, PROVIDER, COST_EXTERNAL, LOCATION, INSTRUCTOR, RESPONSIBLE_PERSON_ID,TRAINING_DEMAND_ID, OBLIGATION from TRAINING_ADVANCEMENT where ID =" + advancmentID, "TRAINING_ADVANCEMENT");
                    }
                    else
                    {
                        detailTable = _db.getDataTableExt("select TOBEDONE_DATE, DONE_DATE, COST_EXTERNAL, COST_INTERNAL, LOCATION, INSTRUCTOR, RESPONSIBLE_PERSON_ID, TRAINING_DEMAND_ID from TRAINING_ADVANCEMENT where ID =" + advancmentID, "TRAINING_ADVANCEMENT");
                    }
                    detailTable.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = _personTable;
                    detailTable.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = _demandTable;

                    appendVSpace(10);
                    _nTable = AppendDetailTable();
                    addDetail(_db, _nTable, detailTable);
                    
                }
            }
        }
    }


}
