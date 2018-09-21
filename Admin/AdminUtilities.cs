using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace ch.appl.psoft.Admin
{
    public class Adminutilities
    {
        public static DataTable GetDropDownTable(DataTable sourceTable, string idColumn, string textcolumn, bool addEmtyItem)
        {

            if (addEmtyItem)
            {
                DataRow EmptyRow = sourceTable.NewRow();
                EmptyRow[idColumn] = "0";
                EmptyRow[textcolumn] = "";
                sourceTable.Rows.Add(EmptyRow);
            }
            sourceTable.DefaultView.Sort = textcolumn;
            return sourceTable;
        }

        public static IList GetIListFromXML(HttpSessionState session, string scope, string name, bool mapIndex)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(session);
            ArrayList entries = new ArrayList(_map.getEnum(scope, name, mapIndex));
            IList entries1 = new List<entryList>();
            int i = 0;
            foreach (object listEntry in entries)
            {

                entries1.Add(new entryList() { Id = i, Entry = listEntry.ToString() });
                i++;
            }
            return entries1;
        }

        private class entryList
        {
            public int Id { get; set; }
            public string Entry { get; set; }
        }

        public static IDictionary<T, V> toDictionary<T, V>(Object objAttached)
        {
            var dicCurrent = new Dictionary<T, V>();
            foreach (DictionaryEntry dicData in (objAttached as IDictionary))
            {
                dicCurrent.Add((T)dicData.Key, (V)dicData.Value);
            }
            return dicCurrent;
        }

        public static Dictionary<string, object> DataSetToJson(DataSet dataset)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

            Dictionary<string, object> ssvalue = new Dictionary<string, object>();

            foreach (DataTable table in dataset.Tables)
            {
                List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> childRow;

                string tablename = table.TableName;
                foreach (DataRow row in table.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                    parentRow.Add(childRow);
                }

                ssvalue.Add(tablename, parentRow);
            }

            return ssvalue;
        }



    }
    public class CustomDataErrorObj
    {
        public HttpSessionState session { get; set; }
        public string shortMessage { get; set; }
        public string Message { get; set; }
        public virtual string getErrorObj(string Data)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(session);
            DataAndError dataAndError = new DataAndError();
            dataAndError.data = Data;
            dataAndError.errorTitle = _map.get("error", "error");
            if (!(shortMessage == null))
            {
                dataAndError.errorShortMessage = _map.get("error", shortMessage);
            }
            if (!(Message == null))
            {
                dataAndError.errorMessage = Message;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(dataAndError) ;
        }

        public virtual string getErrorObj(RadTreeNodeData[] Data)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(session);
            DataAndErrorNodes dataAndError = new DataAndErrorNodes();
            dataAndError.data = Data;
            dataAndError.errorTitle = _map.get("error", "error");
            if (!(shortMessage == null))
            {
                dataAndError.errorShortMessage = _map.get("error", shortMessage);
            }
            if (!(Message == null))
            {
                dataAndError.errorMessage = Message;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(dataAndError);
        }

        public virtual string getErrorObj(Dictionary<string, object> Data)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(session);
            DataAndErrorDictionary dataAndError = new DataAndErrorDictionary();
            dataAndError.data = Data;
            dataAndError.errorTitle = _map.get("error", "error");
            if (!(shortMessage == null))
            {
                dataAndError.errorShortMessage = _map.get("error", shortMessage);
            }
            if (!(Message == null))
            {
                dataAndError.errorMessage = Message;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(dataAndError);
        }




        private class DataAndError
        {
            public string data;
            public string errorTitle;
            public string errorShortMessage;
            public string errorMessage;
        }

        private class DataAndErrorNodes
        {
            public RadTreeNodeData[] data;
            public string errorTitle;
            public string errorShortMessage;
            public string errorMessage;
        }

        private class DataAndErrorDictionary
        {
            public Dictionary<string, object> data;
            public string errorTitle;
            public string errorShortMessage;
            public string errorMessage;
        }


    }



}
