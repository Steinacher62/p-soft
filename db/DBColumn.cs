using ch.appl.psoft.Common;
using ch.psoft.db;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.db
{
    /// <summary>
    /// Summary description for DBColumn.
    /// </summary>
    public class DBColumn : SQLColumn {
        /// <summary>
        /// Kreiert neues Control gemaess der erweiterten Kolonne 
        /// </summary>
        /// <param name="col">DB Kolonne</param>
        /// <param name="cssClass">Style</param>
        /// <param name="fill">True: falls control eine Liste ist, wird sie gemaess der "In"-property gefuellt.
        /// Bei DataTable ist Kolonne 0 die Id und Kolonne 1 der Wert
        /// </param>
        /// <param name="sz">fak. Fieldsize.</param>
        /// <returns>Control</returns>
        public static WebControl NewInputControl(DataColumn col, string cssClass, bool fill, params int[] wh) {
            bool colsFix = wh.Length > 0 && wh[0] >= 0;
            bool rowsFix = wh.Length > 1 && wh[1] >= 0;
            int cols = colsFix ? wh[0] : (col.MaxLength >= 0 ? Math.Min(col.MaxLength,30) : 30);
            int rows = rowsFix ? wh[1] : (col.MaxLength > 128 ? 5 : 1);
            Type typ = (Type) col.ExtendedProperties["InputControlType"];
            ConstructorInfo constructor = typ.GetConstructor(new Type[0]);
            WebControl control = (WebControl) constructor.Invoke(new object[0]);

            if (control is TextBox) {
                TextBox text = control as TextBox;

                text.Columns = cols;
                if (rows > 1) {
                    text.TextMode = TextBoxMode.MultiLine;
                    if (!colsFix) text.Columns = Math.Min(cols,50);
                }
                else if (!colsFix && GetBaseType(col) == InputDataType.Date) text.Columns = 15;
                text.Rows = rows;

                if (col.ExtendedProperties["Password"] is bool)
                    if ((bool) col.ExtendedProperties["Password"])
                        text.TextMode = TextBoxMode.Password;
            }
            else if (fill && control is ListControl) {
                ListControl list = control as ListControl;
                if (control is DropDownList){
                    list.Items.Add(new ListItem(" ",""));
                }

                if (col.ExtendedProperties["In"] is DataTable) {
                    DataTable data = col.ExtendedProperties["In"] as DataTable;
                    foreach (DataRow r in data.Rows) {
                        list.Items.Add(new ListItem(r[1].ToString(),r[0].ToString()));
                    }
                }
                else if (col.ExtendedProperties["In"] is ArrayList) {
                    ArrayList data = col.ExtendedProperties["In"] as ArrayList;

                    for (int i=0; i<data.Count; i++) {
                        if (data[i] != null) list.Items.Add(new ListItem(data[i].ToString(),i.ToString()));
                    }
                }
                else if (col.ExtendedProperties["In"] is string[,]){
                    string[,] enums = col.ExtendedProperties["In"] as string[,];
                        if (enums.GetLength(0) >= 2){
                            for (int i=0; i<enums.GetLength(1); i++){
                                list.Items.Add(new ListItem(enums[1,i], enums[0,i]));
                            }
                        }
                }
            }
            else if (fill && control is BitsetCtrl) {
                BitsetCtrl bitset = control as BitsetCtrl;
                if (col.ExtendedProperties["In"] is DataTable) {
                    DataTable data = col.ExtendedProperties["In"] as DataTable;
                    int idx = 0;
                    foreach (DataRow r in data.Rows) {
                        if (!DBColumn.IsNull(r[0])) bitset.setBit(idx,r[0].ToString(),false);
                        idx++;
                    }
                }
                else if (col.ExtendedProperties["In"] is ArrayList) {
                    ArrayList data = col.ExtendedProperties["In"] as ArrayList;
                    for (int i=0; i<data.Count; i++) {
                        if (data[i] != null) bitset.setBit(i,data[i].ToString(),false);
                    }
                }
            }

            if (cssClass != "") control.CssClass = cssClass;
            return control;
        }
        /// <summary>
        /// Setzt in einer Drop Down List die Wahl auf einen vorgegebenen Wert
        /// </summary>
        /// <param name="list">Drop Down List</param>
        /// <param name="valueToSelect">Liste mit Auswahlmöglichkeit</param>
        public static void selectListControlItem(ListControl list, string valueToSelect) {
            for (int counter = 0; counter < list.Items.Count; counter++) {
                if(list.Items[counter].Value.Equals(valueToSelect)) {
                    list.ClearSelection();
                    list.Items[counter].Selected = true;
                    break;
                }
            }
        }
        /// <summary>
        /// Gibt default Relation auf Grund von Datentyp und Inputcontroltyp
        /// </summary>
        /// <param name="col">DB Kolonne</param>
        /// <returns>Relation</returns>
        public static Relation GetDefaultRelation (DataColumn col) {
            Type typ = (Type) col.ExtendedProperties["InputControlType"];

            switch (GetBaseType(col)) {
            case InputDataType.Integer:
            case InputDataType.Double:
            case InputDataType.Date:
                return Relation.EQ;
            case InputDataType.String:
                if (typ.IsSubclassOf(typeof(ListControl))){
                    return Relation.EQ;
                }
                return Relation.LIKE;
            default:
                break;
            }
            return Relation.EQ;
        }

        /// <summary>
        /// Initialiert Erweiterungen gemäss Datentyp table.GetType
        /// Nach Default culture rufen !
        /// - InputControlType  (WebControl-Type for input masks)
        /// - ListControlType   (WebControl-Type for listings)
        /// - DetailControlType (WebControl-Type for details)
        /// - ContextLink       (string for link in listings and details)
        /// - ContextLinkTarget (string for target-frame of ContextLink in listings and details)
        /// - Format            (string)
        /// - Unit              (string)
        /// - CharSet           (string)
        /// - Visibility        (Visibility)
        /// - In                (ArrayList oder DataTable)
        /// - Min               (gemaess DataColumn.DataType)
        /// - Max               (gemaess DataColumn.DataType)
        /// - Views             (string)
        /// </summary>
        /// <param name="table"></param>
        public override void InitExtension (DataColumn col) {
            base.InitExtension(col);
            col.ExtendedProperties.Add("InputControlType",typeof(TextBox));
            col.ExtendedProperties.Add("ListControlType",null);
            col.ExtendedProperties.Add("DetailControlType",null);
            col.ExtendedProperties.Add("ContextLink", null);
            col.ExtendedProperties.Add("ContextLinkTarget", "_self");
        }
        public static string LookupIn (DataColumn col, object id, bool http) {
            if (col.ExtendedProperties["In"] is DataTable) {
                DataTable data = col.ExtendedProperties["In"] as DataTable;
                foreach (DataRow r in data.Rows) {
                    if (r[0].Equals(id)) return http ? HttpUtility.HtmlEncode(r[1].ToString()) : r[1].ToString();
                }
            }
            else if (col.ExtendedProperties["In"] is ArrayList) {
                ArrayList list = col.ExtendedProperties["In"] as ArrayList;
                int idx = ch.psoft.Util.Validate.GetValid(id.ToString(),-1);
                
                if (idx >= 0 && idx < list.Count)
                    return http ? HttpUtility.HtmlEncode(list[idx].ToString()) : list[idx].ToString();
            }
            else if (col.ExtendedProperties["In"] is string[,]){
                string[,] property = col.ExtendedProperties["In"] as string[,];
                string idStr = id.ToString();

                if (property.GetLength(0) >= 2){
                    for (int i=0; i<property.GetLength(1); i++){
                        if (property[0,i] == idStr){
                            return http ? HttpUtility.HtmlEncode(property [1,i]) : property [1,i];
                        }
                    }
                }
            }
            else if (col.ExtendedProperties["In"] is object[,]){
                object[,] property = col.ExtendedProperties["In"] as object[,];

                if (property.GetLength(0) >= 2){
                    for (int i=0; i<property.GetLength(1); i++){
                        if (property[0,i].Equals(id)){
                            return http ? HttpUtility.HtmlEncode(property [1,i].ToString()) : property [1,i].ToString();
                        }
                    }
                }
            }
            else if (col.ExtendedProperties["In"] != null) {
                Type typ = col.ExtendedProperties["In"].GetType();
                MethodInfo method = typ.GetMethod("lookup");
                if (method != null) {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 3 && parameters[0].ParameterType == typeof(DataColumn)) {
                        object[] param = {col,id,http};
                        return method.Invoke(col.ExtendedProperties["In"],param).ToString();
                    }
                }
            }
            return "";
        }
    }
}
