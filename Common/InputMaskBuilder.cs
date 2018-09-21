using ch.appl.psoft.Common.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Common
{
    public delegate void BuildSQLHandler(StringBuilder build, System.Web.UI.Control control, DataColumn col, object val);
    /// <summary>
    /// Diverse Hilfsmittel zum Generieren von Input-tabellen
    /// </summary>
    public class InputMaskBuilder
    {
        public enum InputType
        {
            Add,
            Edit,
            Search
        }
        private InputType _inputTyp = InputType.Add;
        private DataColumn _idColumn = null;
        private DBColumn _dbColumn;
        private bool _modifCheck = true;
        private LanguageMapper _map;
        private DBData _db;
        private Table _inputTab = null;
        private string _viewName = "";
        private bool _checkOrder = false;
        private bool _showRelation = false;
        private string _idPrefix = "";
        private HttpSessionState _session;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InputMaskBuilder()
        {
        }
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="typ">Inputart</param>
        public InputMaskBuilder(InputType typ, HttpSessionState session)
        {
            _inputTyp = typ;
            _dbColumn = SessionData.getDBColumn(session);
        }
        /// <summary>
        /// Set Session
        /// </summary>
        public HttpSessionState Session
        {
            set { _session = value; }
        }
        /// <summary>
        /// Set check columnorder in select
        /// </summary>
        public bool checkOrder
        {
            set { _checkOrder = value; }
        }
        /// <summary>
        /// Get DB-column
        /// </summary>
        public DBColumn dbColumn
        {
            set { _dbColumn = value; }
            get { return _dbColumn; }
        }
        /// <summary>
        /// Set/Get flag for update only modified values (for type Edit only).
        /// Default = true
        /// </summary>
        public bool modifCheck
        {
            get { return this._modifCheck; }
            set { this._modifCheck = value; }
        }
        public DataColumn idColumn
        {
            set { this._idColumn = value; }
            get { return this._idColumn; }
        }

        /// <summary>
        /// Prefix to be added for control-IDs
        /// </summary>
        public string IDPrefix
        {
            set { _idPrefix = value; }
            get { return _idPrefix; }
        }
        /// <summary>
        /// Should the relation-dropdown for the search-mask be showed?
        /// </summary>
        public bool ShowRelation
        {
            set { _showRelation = value; }
            get { return _showRelation; }
        }
        /// <summary>
        /// Add property event. 
        /// </summary>
        public event AddPropertyHandler addRow = null;
        /// <summary>
        /// Build SQL-string event.
        /// </summary>
        public event BuildSQLHandler buildSQL = null;
        /// <summary>
        /// Get input type
        /// </summary>
        public InputType inputType
        {
            set { this._inputTyp = value; }
            get { return this._inputTyp; }
        }
        /// <summary>
        /// Bildet eine Input-tabelle gemaess typ
        /// </summary>
        /// <param name="db">DB</param>
        /// <param name="table">Tabelle mit erweiterter Kolonnendefinition</param>
        /// <param name="inputTab">Web Tabelle mit such-attributen</param>
        /// <param name="map">Sprach mapping</param>
        /// <param name="view">fak. view to load</param>
        public void load(DBData db, DataTable table, Table inputTab, LanguageMapper map, params string[] param)
        {
            string view = param.Length > 0 ? param[0] : "";

            _inputTab = inputTab;
            _map = map;
            _db = db;
            _viewName = view == "" ? table.TableName : view;

            if (_idPrefix == "")
                _idPrefix = table.TableName;

            switch (this._inputTyp)
            {
                case InputType.Search:
                    _db.scanColumn += new ScanActionColumnHandler(this.scanSearch);
                    _db.scanTableColumn(table, _viewName, (int)SQLColumn.Visibility.SEARCH, DBData.AUTHORISATION.READ, _checkOrder);
                    _db.scanColumn -= new ScanActionColumnHandler(this.scanSearch); 
                    break;
                case InputType.Add:
                    _db.scanColumn += new ScanActionColumnHandler(this.scanAdd);
                    _db.scanTableColumn(table, _viewName, (int)SQLColumn.Visibility.ADD, DBData.AUTHORISATION.INSERT, _checkOrder);
                    _db.scanColumn -= new ScanActionColumnHandler(this.scanAdd);
                    break;
                case InputType.Edit:
                    _db.scanRow += new ScanActionRowHandler(this.scanEditRow);
                    _db.scanCell += new ScanActionCellHandler(this.scanEdit);
                    _db.scanTableData(table, _viewName, (int)SQLColumn.Visibility.EDIT, DBData.AUTHORISATION.UPDATE, DBData.APPLICATION_RIGHT.COMMON, _checkOrder, false);
                    _db.scanRow -= new ScanActionRowHandler(this.scanEditRow);
                    _db.scanCell -= new ScanActionCellHandler(this.scanEdit);
                    break;
                default:
                    return;
            }
        }
        private TableRow addLabel(DataColumn col)
        {
            return addLabel(col, false);
        }
        private TableRow addLabel(DataColumn col, bool confirm)
        {
            TableRow r = new TableRow();
            TableCell c = new TableCell();

            r.ID = _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");
            r.CssClass = "InputMask";
            r.VerticalAlign = VerticalAlign.Top;
            _inputTab.Rows.Add(r);
            r.Cells.Add(c);
            c.CssClass = "InputMask_Label";
            c.Text = _map.get(_viewName, col.ColumnName + (confirm ? "_CONFIRM" : ""));
            c.EnableViewState = false;
            c.ID = "Label_" + _idPrefix + "_" + col.ColumnName + (confirm ? "_Confirm" : "");
            return r;
        }

        private void scanSearch(DataTable table, DataColumn col, int columnNumber)
        {
            if (columnNumber < 0) return;

            TableRow r = addLabel(col);
            TableCell c = new TableCell();
            WebControl inputCtrl;
            ListControl relationCtrl;
            string format = "", unit = "";
            SQLColumn.InputDataType dataType;
            Label lbl;
            string postFix = "";

            r.Cells.Add(c);
            if (DBColumn.GetBaseType(col) == DBColumn.InputDataType.Date)
            {
                // fix typo in CSS class / 02.02.10 / mkr (was InpuMask_Label)
                c.CssClass = "InputMask_Label";
                postFix = "-From";
                r.ID += "-From";
                c.Text = _map.get("from");
                c.VerticalAlign = VerticalAlign.Middle;
            }
            else
            {
                relationCtrl = NewRelationList();
                relationCtrl.SelectedIndex = (int)DBColumn.GetDefaultRelation(col);
                c.Controls.Add(relationCtrl);
                c.ID = "Relation-" + _idPrefix + "-" + col.ColumnName;
                if (!_showRelation)
                    relationCtrl.Visible = false;
            }
            c = new TableCell();
            c.CssClass = "inputMask_Value";
            r.Cells.Add(c);

            inputCtrl = DBColumn.NewInputControl(col, "InputValue", true, 0, 1);
            inputCtrl.ID = "Input-" + _idPrefix + "-" + col.ColumnName + postFix;

            //HACK: jump to search button after tab
            if (col.ColumnName == "PNAME")
            {
                //inputCtrl.TabIndex = 99;
                //inputCtrl.Attributes.Add("onblur", "javascript:alert('test');");
                inputCtrl.Attributes.Add("onkeypress", @"javascript:if(window.event.keyCode == 13) document.getElementById('ContentPlaceHolder1__sL__sC_Search_apply').click(); else return true;");
            }

            setFocusControl(inputCtrl);

            if (inputCtrl is TextBox)
            {
                if ((inputCtrl as TextBox).TextMode == TextBoxMode.MultiLine) c.ColumnSpan = 2;
            }

            inputCtrl.CssClass = "inputMask_Value";
            c.Controls.Add(inputCtrl);
            inputCtrl.EnableViewState = false;
            format = (string)col.ExtendedProperties["Format"];
            dataType = SQLColumn.GetBaseType(col);

            if (format != "" && dataType == SQLColumn.InputDataType.Date) inputCtrl.ToolTip = format;

            unit = (string)col.ExtendedProperties["Unit"];
            if (unit != "")
            {
                lbl = new Label();
                lbl.Text = "&nbsp;" + unit;
                c.Controls.Add(lbl);
            }

            if (dataType == DBColumn.InputDataType.Date)
            {
                //c.Controls.Add(DateSelector.getDateSelectorButton(inputCtrl, format));
                //c.Controls.Add(CalendarSelector.getDateSelectorButton((TextBox) inputCtrl, format));
                //set inputmasklabel to valign middle
                ((TableCell)r.Controls[0]).VerticalAlign = VerticalAlign.Middle;
                //create calendar
                CalendarSelector calsel = new CalendarSelector(_map, (TextBox)inputCtrl, format);
                //create Table, Row and 2 Cells
                Table calTable = new Table();
                TableRow calRow = new TableRow();
                TableCell calCell1 = new TableCell();
                TableCell calCell2 = new TableCell();
                //style table
                calTable.Style.Add(HtmlTextWriterStyle.MarginLeft, "-1px");
                calTable.CellSpacing = 0;
                //add textbos to cell 1
                calCell1.Controls.Add(inputCtrl);
                //add calendar to cell 2
                calCell2.Controls.Add(calsel.imgButton);
                calCell2.Controls.Add(calsel.cal);
                //add cells to row
                calRow.Controls.Add(calCell1);
                calRow.Controls.Add(calCell2);
                //add row to table
                calTable.Controls.Add(calRow);
                //add table to c
                c.Controls.Add(calTable);

                //CalendarSelector calsel = new CalendarSelector(_map, (TextBox)inputCtrl, format);
                //c.Controls.Add(calsel.imgButton);
                //c.Controls.Add(calsel.cal);

                onAddRow(null, col, r);
                // date to
                r = new TableRow();
                r.ID = _idPrefix + "-" + col.ColumnName + "To";
                r.CssClass = "InputMask";
                r.VerticalAlign = VerticalAlign.Top;
                _inputTab.Rows.Add(r);
                c = new TableCell();
                r.Cells.Add(c);
                c = new TableCell();
                r.Cells.Add(c);
                // fix typo in CSS class / 02.02.10 / mkr (was InpuMask_Label)
                c.CssClass = "InputMask_Label";
                c.Text = _map.get("to");
                c.VerticalAlign = VerticalAlign.Middle;

                c = new TableCell();
                c.CssClass = "inputMask_Value";
                r.Cells.Add(c);
                inputCtrl = DBColumn.NewInputControl(col, "InputValue", true, 0, 1);
                inputCtrl.ID = "Input-" + _idPrefix + "-" + col.ColumnName + "-To";
                inputCtrl.CssClass = "inputMask_Value";
                c.Controls.Add(inputCtrl);
                inputCtrl.EnableViewState = false;
                format = (string)col.ExtendedProperties["Format"];
                dataType = SQLColumn.GetBaseType(col);

                if (format != "") inputCtrl.ToolTip = format;

                ((TableCell)r.Controls[0]).VerticalAlign = VerticalAlign.Middle;
                //create calendar
                CalendarSelector calsel2 = new CalendarSelector(_map, (TextBox)inputCtrl, format);
                //create Table, Row and 2 Cells
                Table calTable2 = new Table();
                TableRow calRow2 = new TableRow();
                TableCell calCell21 = new TableCell();
                TableCell calCell22 = new TableCell();
                //style table
                calTable2.Style.Add(HtmlTextWriterStyle.MarginLeft, "-1px");
                calTable2.CellSpacing = 0;
                //add textbos to cell 1
                calCell21.Controls.Add(inputCtrl);
                //add calendar to cell 2
                calCell22.Controls.Add(calsel2.imgButton);
                calCell22.Controls.Add(calsel2.cal);
                //add cells to row
                calRow2.Controls.Add(calCell21);
                calRow2.Controls.Add(calCell22);
                //add row to table
                calTable2.Controls.Add(calRow2);
                //add table to c
                c.Controls.Add(calTable2);
            }

            this.onAddRow(null, col, r);
        }

        private void scanAdd(DataTable table, DataColumn col, int columnNumber)
        {
            scanAdd(table, col, columnNumber, false);
        }

        private void scanAdd(DataTable table, DataColumn col, int columnNumber, bool confirm)
        {
            if (columnNumber < 0) return;

            TableRow r = addLabel(col, confirm);
            TableCell c = new TableCell();
            WebControl inputCtrl;
            string format = "", unit = "";
            SQLColumn.InputDataType dataType;
            Label lbl;
            int cols, rows;

            _map.getSize(_viewName, col.ColumnName, out cols, out rows);

            if (!(bool)col.ExtendedProperties["Nullable"]) r.Cells[0].CssClass = "InputMask_Label_NotNull";

            r.Cells.Add(c);

            Type typ = (Type)col.ExtendedProperties["InputControlType"];
            if (typ == typeof(RadEditor))
            {
                RadEditor editor = new RadEditor();
                editor.ID = "Input-" + _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");
                editor.Language = "de-De";
                editor.OnClientLoad = "EditorLoaded";
                String[] ImagePaths = { "~/Knowledge/Templates/Images" };
                String[] DocumentPaths = { "~/Knowledge/Templates/Documents" };
                String[] FlashPaths = { "~/Knowledge/Templates/Flash" };
                String[] MediaPaths = { "~/Knowledge/Templates/Medias" };
                string[] HtmlPaths = { "~/Knowledge/Templates/HtmlDocs" };
                String[] extension = { "*.mp4" };

                editor.ImageManager.UploadPaths = ImagePaths;
                editor.ImageManager.DeletePaths = ImagePaths;
                editor.ImageManager.ViewPaths = ImagePaths;
                editor.DocumentManager.UploadPaths = DocumentPaths;
                editor.DocumentManager.DeletePaths = DocumentPaths;
                editor.DocumentManager.ViewPaths = DocumentPaths;
                editor.FlashManager.UploadPaths = FlashPaths;
                editor.FlashManager.DeletePaths = FlashPaths;
                editor.FlashManager.ViewPaths = FlashPaths;
                editor.FlashManager.MaxUploadFileSize = 200000000;
                editor.MediaManager.UploadPaths = MediaPaths;
                editor.MediaManager.DeletePaths = MediaPaths;
                editor.MediaManager.ViewPaths = MediaPaths;
                editor.MediaManager.MaxUploadFileSize = 2000000000;
                editor.MediaManager.SearchPatterns = extension;
                editor.TemplateManager.UploadPaths = HtmlPaths;
                editor.TemplateManager.ViewPaths = HtmlPaths;
                editor.TemplateManager.DeletePaths = HtmlPaths;

                editor.EnableViewState = false;
                // editor.Content = _dbColumn.GetDisplayValue(col, cellValue, false);
                c.Controls.Add(editor);

            }
            else
            {
                inputCtrl = DBColumn.NewInputControl(col, "InputValue", true, cols, rows);
                inputCtrl.ID = "Input-" + _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");
                c.CssClass = "inputMask_Value";
                setFocusControl(inputCtrl);

                if (inputCtrl is TextBox)
                {
                    switch ((inputCtrl as TextBox).TextMode)
                    {
                        case TextBoxMode.MultiLine:
                            c.ColumnSpan = 2;
                            break;
                        case TextBoxMode.Password:
                            if (!confirm)
                                scanAdd(table, col, columnNumber, true);
                            break;
                    }
                }

                if (inputCtrl is WikiBoxCtrl)
                {
                    (inputCtrl as WikiBoxCtrl).Session = _session;
                    (inputCtrl as WikiBoxCtrl).Refresh();
                }
                inputCtrl.CssClass = "inputMask_Value";
                c.Controls.Add(inputCtrl);
                inputCtrl.EnableViewState = false;
                format = (string)col.ExtendedProperties["Format"];
                dataType = SQLColumn.GetBaseType(col);

                unit = (string)col.ExtendedProperties["Unit"];
                if (unit != "")
                {
                    lbl = new Label();
                    lbl.Text = "&nbsp;" + unit;
                    c.Controls.Add(lbl);
                }
                if (dataType == DBColumn.InputDataType.Date)
                {
                    if (format != "") inputCtrl.ToolTip = format;

                    //set inputmasklabel to valign middle
                    ((TableCell)r.Controls[0]).VerticalAlign = VerticalAlign.Middle;
                    //create calendar
                    CalendarSelector calsel = new CalendarSelector(_map, (TextBox)inputCtrl, format);
                    //create Table, Row and 2 Cells
                    Table calTable = new Table();
                    TableRow calRow = new TableRow();
                    TableCell calCell1 = new TableCell();
                    TableCell calCell2 = new TableCell();
                    //style table
                    calTable.Style.Add(HtmlTextWriterStyle.MarginLeft, "-1px");
                    calTable.CellSpacing = 0;
                    //add textbos to cell 1
                    calCell1.Controls.Add(inputCtrl);
                    //add calendar to cell 2
                    calCell2.Controls.Add(calsel.imgButton);
                    calCell2.Controls.Add(calsel.cal);
                    //add cells to row
                    calRow.Controls.Add(calCell1);
                    calRow.Controls.Add(calCell2);
                    //add row to table
                    calTable.Controls.Add(calRow);

                    // temp: give calbox same id as textbox
                    //calsel.ID = "Input-" + _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");

                    //add table to c
                    c.Controls.Add(calTable);
                }
            }
            this.onAddRow(null, col, r);
        }

        private bool scanEditRow(DataTable table, DataRow row, int rowNumber, int rowAuthorisations)
        {
            return rowNumber == 1;
        }
        private void scanEdit(DataTable table, DataRow row, int rowNumber, DataColumn col, int columnNumber)
        {
            scanEdit(table, row, rowNumber, col, columnNumber, false);
        }
        private void scanEdit(DataTable table, DataRow row, int rowNumber, DataColumn col, int columnNumber, bool confirm)
        {
            if (columnNumber < 0) return;

            TableRow r = addLabel(col, confirm);
            TableCell c = new TableCell();
            WebControl inputCtrl;
            string format = "", unit = "";
            SQLColumn.InputDataType dataType;
            Label lbl;
            object cellValue = row[col];
            int cols, rows;
            _map.getSize(_viewName, col.ColumnName, out cols, out rows);

            if (!(bool)col.ExtendedProperties["Nullable"]) r.Cells[0].CssClass = "InputMask_Label_NotNull";
            c = new TableCell();

            r.Cells.Add(c);

            Type typ = (Type)col.ExtendedProperties["InputControlType"];
            if (typ == typeof(RadEditor))
            {
                RadEditor editor = new RadEditor();
                editor.ID = "Input-" + _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");
                editor.Language = "de-De";
                editor.OnClientLoad = "EditorLoaded";
                String[] ImagePaths = { "~/Knowledge/Templates/Images" };
                String[] DocumentPaths = { "~/Knowledge/Templates/Documents" };
                String[] FlashPaths = { "~/Knowledge/Templates/Flash" };
                String[] MediaPaths = { "~/Knowledge/Templates/Medias" };
                string[] HtmlPaths = { "~/Knowledge/Templates/HtmlDocs" };
                String[] extension = { "*.mp4" };

                editor.ImageManager.UploadPaths = ImagePaths;
                editor.ImageManager.DeletePaths = ImagePaths;
                editor.ImageManager.ViewPaths = ImagePaths;
                editor.DocumentManager.UploadPaths = DocumentPaths;
                editor.DocumentManager.DeletePaths = DocumentPaths;
                editor.DocumentManager.ViewPaths = DocumentPaths;
                editor.FlashManager.UploadPaths = FlashPaths;
                editor.FlashManager.DeletePaths = FlashPaths;
                editor.FlashManager.ViewPaths = FlashPaths;
                editor.FlashManager.MaxUploadFileSize = 200000000;
                editor.MediaManager.UploadPaths = MediaPaths;
                editor.MediaManager.DeletePaths = MediaPaths;
                editor.MediaManager.ViewPaths = MediaPaths;
                editor.MediaManager.MaxUploadFileSize = 2000000000;
                editor.MediaManager.SearchPatterns = extension;
                editor.TemplateManager.UploadPaths = HtmlPaths;
                editor.TemplateManager.ViewPaths = HtmlPaths;
                editor.TemplateManager.DeletePaths = HtmlPaths;

                editor.EnableViewState = false;
                editor.Content = _dbColumn.GetDisplayValue(col, cellValue, false);
                c.Controls.Add(editor);
            }
            else
            {
                inputCtrl = DBColumn.NewInputControl(col, "InputValue", true, cols, rows);
                inputCtrl.ID = "Input-" + _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");
                c.CssClass = "inputMask_Value";

                setFocusControl(inputCtrl);

                if (inputCtrl is TextBox)
                {
                    switch ((inputCtrl as TextBox).TextMode)
                    {
                        case TextBoxMode.MultiLine:
                            c.ColumnSpan = 2;
                            break;
                        case TextBoxMode.Password:
                            if (!confirm)
                                scanEdit(table, row, rowNumber, col, columnNumber, true);
                            break;
                    }
                }

                if (inputCtrl is TwoValuesTextBox)
                {
                    if (col.ExtendedProperties["In"] == null)
                    {
                        (inputCtrl as TwoValuesTextBox).OrigText
                            = _dbColumn.GetDisplayValue(col, cellValue, false);
                    }
                    else
                    {
                        (inputCtrl as TwoValuesTextBox).OrigText
                            = DBColumn.LookupIn(col, cellValue, false);
                    }

                    if (cellValue is int)
                    {
                        (inputCtrl as TwoValuesTextBox).LongText = ((int)cellValue).ToString(_dbColumn.UserCulture);
                    }
                    else if (cellValue is long)
                    {
                        (inputCtrl as TwoValuesTextBox).LongText = ((long)cellValue).ToString(_dbColumn.UserCulture);
                    }
                    else if (cellValue is double)
                    {
                        (inputCtrl as TwoValuesTextBox).LongText = ((double)cellValue).ToString(_dbColumn.UserCulture);
                    }
                    else
                    {
                        (inputCtrl as TwoValuesTextBox).LongText = cellValue.ToString();
                    }
                }
                else if (inputCtrl is TextBox)
                {
                    if (col.ExtendedProperties["In"] == null) (inputCtrl as TextBox).Text = _dbColumn.GetDisplayValue(col, cellValue, false);
                    else (inputCtrl as TextBox).Text = DBColumn.LookupIn(col, cellValue, false);
                }
                else if (inputCtrl is Button)
                {
                    (inputCtrl as Button).Text = _dbColumn.GetDisplayValue(col, cellValue, true);
                }
                else if (inputCtrl is Label)
                {
                    if (col.ExtendedProperties["In"] == null) (inputCtrl as Label).Text = _dbColumn.GetDisplayValue(col, cellValue, true);
                    else (inputCtrl as Label).Text = DBColumn.LookupIn(col, cellValue, true);
                }
                else if (inputCtrl is HyperLink)
                {
                    (inputCtrl as HyperLink).Text = _dbColumn.GetDisplayValue(col, cellValue, true);
                }
                else if (inputCtrl is CheckBox)
                {
                    String str = _dbColumn.GetDisplayValue(col, cellValue, true);
                    if ((str == "") || (str == "0"))
                        str = "False";
                    else
                        str = "True";
                    (inputCtrl as CheckBox).Checked = Convert.ToBoolean(str);
                }
                else if (inputCtrl is BitsetCtrl)
                {
                    int val = DBColumn.GetValid(cellValue, 0);
                    BitsetCtrl bitset = inputCtrl as BitsetCtrl;
                    int mask = 1;
                    int idx = 0;

                    foreach (CheckBox item in bitset.items)
                    {
                        if (item == null) continue;
                        if (bitset.mode == BitsetCtrl.Mode.BitMask)
                        {
                            item.Checked = (val & mask) > 0;
                            mask *= 2;
                        }
                        else
                        {
                            item.Checked = val == idx;
                            idx++;
                        }
                    }
                }
                else if (inputCtrl is ListControl)
                {
                    DBColumn.selectListControlItem(inputCtrl as ListControl, cellValue.ToString());
                }
                else if (inputCtrl is WikiBoxCtrl)
                {
                    (inputCtrl as WikiBoxCtrl).Session = _session;
                    (inputCtrl as WikiBoxCtrl).UID = DBColumn.GetValid(row["UID"], "-1");
                    (inputCtrl as WikiBoxCtrl).SetWikiBoxText(_dbColumn.GetDisplayValue(col, cellValue, false));
                    (inputCtrl as WikiBoxCtrl).Refresh();
                }

                inputCtrl.CssClass = "inputMask_Value";
                c.Controls.Add(inputCtrl);
                inputCtrl.EnableViewState = false;
                format = (string)col.ExtendedProperties["Format"];
                dataType = SQLColumn.GetBaseType(col);


                unit = (string)col.ExtendedProperties["Unit"];
                if (unit != "")
                {
                    lbl = new Label();
                    lbl.Text = "&nbsp;" + unit;
                    c.Controls.Add(lbl);
                }

                if (dataType == DBColumn.InputDataType.Date)
                {
                    if (format != "") inputCtrl.ToolTip = format;

                    //set inputmasklabel to valign middle
                    ((TableCell)r.Controls[0]).VerticalAlign = VerticalAlign.Middle;
                    //create calendar
                    CalendarSelector calsel = new CalendarSelector(_map, (TextBox)inputCtrl, format);
                    //create Table, Row and 2 Cells
                    Table calTable = new Table();
                    TableRow calRow = new TableRow();
                    TableCell calCell1 = new TableCell();
                    TableCell calCell2 = new TableCell();
                    //style table
                    calTable.Style.Add(HtmlTextWriterStyle.MarginLeft, "-1px");
                    calTable.CellSpacing = 0;
                    //add textbos to cell 1
                    calCell1.Controls.Add(inputCtrl);
                    //add calendar to cell 2
                    calCell2.Controls.Add(calsel.imgButton);
                    calCell2.Controls.Add(calsel.cal);
                    //add cells to row
                    calRow.Controls.Add(calCell1);
                    calRow.Controls.Add(calCell2);
                    //add row to table
                    calTable.Controls.Add(calRow);
                    //add table to c
                    c.Controls.Add(calTable);
                }
            }

            this.onAddRow(row, col, r);
        }

        /// <summary>
        /// Bildet sql substring gemaess typ
        /// Beispiel:
        /// Search: select * from person where name='wymann' and vorname like 'kurt'
        /// Add:    insert into person (name,vorname) values('wymann','kurt')
        /// Edit:   update person set name='wymann',vorname='kurt' where id = 4711
        /// </summary>
        /// <param name="table">DB-tabelle</param>
        /// <param name="inputTab">input-Tabelle</param>
        /// <returns>sql-string</returns>
        public string getSql(DataTable table, Table inputTab)
        {
            return getSql(table, inputTab, false).ToString();
        }
        /// <summary>
        /// Bildet sql substring gemaess typ
        /// Beispiel:
        /// Search: select * from person where name='wymann' and vorname like 'kurt'
        /// Add:    insert into person (name,vorname) values('wymann','kurt')
        /// Edit:   update person set name='wymann',vorname='kurt' where id = 4711
        /// </summary>
        /// <param name="table">DB-Tabelle</param>
        /// <param name="inputTab">Tabelle mit den Suchkriterien</param>
        /// <param name="extend">true: sql-string soll erweitert werden können. (extendSql)</param>
        /// <returns>sql-string</returns>
        public StringBuilder getSql(DataTable table, Table inputTab, bool extend)
        {
            return getSql(table, inputTab, extend, true);
        }
        /// <summary>
        /// Bildet sql substring gemaess typ
        /// Beispiel:
        /// Search: select * from person where name='wymann' and vorname like 'kurt'
        /// Add:    insert into person (name,vorname) values('wymann','kurt')
        /// Edit:   update person set name='wymann',vorname='kurt' where id = 4711
        /// </summary>
        /// <param name="table">DB-Tabelle</param>
        /// <param name="inputTab">Tabelle mit den Suchkriterien</param>
        /// <param name="extend">true: sql-string soll erweitert werden können. (extendSql)</param>
        /// <param name="useDistinct">if true a 'select distinct' will be generated</param>
        /// <returns>sql-string</returns>
        public StringBuilder getSql(DataTable table, Table inputTab, bool extend, bool useDistinct)
        {
            StringBuilder search = new StringBuilder("select " + (useDistinct ? "distinct" : "") + " * from ");
            StringBuilder values = new StringBuilder(" values(");
            StringBuilder insert = new StringBuilder("insert into ");
            StringBuilder update = new StringBuilder("update ");
            StringBuilder sql = new StringBuilder();
            ListItem relation = null;
            System.Web.UI.Control ctrl;
            string text, name, postFix;
            string[] names;
            bool ok = false;
            bool isPassword = false;

            search.Append(table.TableName).Append(" where ");
            insert.Append(table.TableName).Append(" (");
            update.Append(table.TableName).Append(" set ");
            foreach (TableRow r in inputTab.Rows)
            {
                TableCell prevCell = null;
                foreach (TableCell cell in r.Cells)
                {
                    string id = null;
                    bool isCal = false;

                    if (cell.HasControls())
                    {
                        try
                        {
                            id = cell.Controls[0].Controls[0].Controls[0].Controls[0].ID;
                            isCal = true;
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            id = cell.Controls[0].ID;
                        }
                    }

                    //if (cell.HasControls() && ch.psoft.Util.Validate.GetValid(cell.Controls[0].ID).StartsWith("Input-")) {
                    if (cell.HasControls() && ch.psoft.Util.Validate.GetValid(id).StartsWith("Input-"))
                    {
                        isPassword = false; // if password and there is no value ("") we do not change the current password
                        //ctrl = cell.Controls[0];
                        if (isCal)
                        {
                            ctrl = cell.Controls[0].Controls[0].Controls[0].Controls[0];
                        }
                        else
                        {
                            ctrl = cell.Controls[0];
                        }
                        names = ctrl.ID.Split('-');
                        if (names.Length < 3) continue;
                        name = names[2];
                        if (name.EndsWith("_Confirm")) continue;
                        postFix = names.Length > 3 ? names[3] : "";
                        text = "";
                        if (this._inputTyp == InputType.Search)
                        {
                            if (prevCell != null && ch.psoft.Util.Validate.GetValid(prevCell.ID).StartsWith("Relation-"))
                                relation = ((DropDownList)prevCell.Controls[0]).SelectedItem;
                            else if (postFix == "From")
                                relation = new ListItem(">=", " >= ");
                            else if (postFix == "To")
                                relation = new ListItem("<=", " <= ");
                        }

                        if (ctrl is TwoValuesTextBox)
                        {
                            text = (ctrl as TwoValuesTextBox).ValidText;
                            isPassword = (ctrl as TwoValuesTextBox).TextMode == TextBoxMode.Password;
                        }
                        else if (ctrl is TextBox)
                        {
                            text = (ctrl as TextBox).Text;
                            isPassword = (ctrl as TextBox).TextMode == TextBoxMode.Password;
                        }
                        else if (ctrl is CheckBox)
                        {
                            text = (ctrl as CheckBox).Checked ? "1" : "0";
                        }
                        else if (ctrl is BitsetCtrl)
                        {
                            int idx = 0;
                            int mask = 1;
                            int val = 0;
                            BitsetCtrl bitset = ctrl as BitsetCtrl;
                            foreach (CheckBox item in bitset.items)
                            {
                                if (item == null) continue;
                                if (item.Checked)
                                {
                                    if (bitset.mode == BitsetCtrl.Mode.BitMask) val += mask;
                                    else text += "," + idx;
                                }
                                idx++;
                                mask *= 2;
                            }
                            if (bitset.mode == BitsetCtrl.Mode.BitMask) text = val.ToString();
                            else if (text != "") text = text.Substring(1);
                            if (this._inputTyp == InputType.Search)
                            {
                                relation = new ListItem("in", " in ");
                                if (text != "") text = "(" + text + ")";
                            }

                        }
                        else if (ctrl is ListControl)
                        {
                            ListItem item = (ctrl as ListControl).SelectedItem;
                            if (item != null) text = item.Value;
                        }
                        else if (ctrl is WikiBoxCtrl)
                        {
                            text = (ctrl as WikiBoxCtrl).Text;
                        }
                        else if (ctrl is RadEditor)
                        {
                            text = (ctrl as RadEditor).Content.ToString();
                        }
                        else continue;

                        switch (this._inputTyp)
                        {
                            case InputType.Add:
                                if (text != "")
                                {
                                    ok = true;
                                    insert.Append(name).Append(",");
                                    onBuildSQL(values, ctrl, table.Columns[name], text);
                                    values = values.Append(",");
                                }
                                break;
                            case InputType.Edit:
                                if ((table.Rows[0][name].ToString() != text && !(isPassword && text == "")) || !_modifCheck)
                                {
                                    ok = true;
                                    update.Append(name).Append("=");
                                    onBuildSQL(update, ctrl, table.Columns[name], text);
                                    update.Append(",");
                                }
                                break;
                            case InputType.Search:
                                if (text != "")
                                {
                                    ok = true;
                                    DBColumn.InputDataType typ = DBColumn.GetBaseType(table.Columns[name]);
                                    switch (typ)
                                    {
                                        case DBColumn.InputDataType.Date:
                                        case DBColumn.InputDataType.Integer:
                                        case DBColumn.InputDataType.Long:
                                            search.Append(name);
                                            break;
                                        default:
                                            if (typ == DBColumn.InputDataType.String && table.Columns[name].MaxLength < 0)
                                            {
                                                // columns of type text cannot be lowered.
                                                search.Append(name);
                                            }
                                            else
                                            {
                                                search.Append("LOWER(").Append(name).Append(")");
                                            }
                                            break;
                                    }
                                    if (relation.Value.Trim() == "like" && text.IndexOf("%") < 0) text = "%" + text + "%";
                                    search.Append(relation.Value);
                                    onBuildSQL(search, null, table.Columns[name], text.ToLower());
                                    search.Append(" and ");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    prevCell = cell;
                }
            }
            if (ok)
            {
                switch (this._inputTyp)
                {
                    case InputType.Add:
                        if (extend)
                        {
                            insert.Replace(",", "?#1)", insert.Length - 1, 1);
                            values.Replace(",", "?#2)", values.Length - 1, 1);
                        }
                        else
                        {
                            insert.Replace(',', ')', insert.Length - 1, 1);
                            values.Replace(',', ')', values.Length - 1, 1);
                        }
                        sql = insert.Append(values);
                        break;
                    case InputType.Edit:
                        if (extend) update.Replace(",", "?#1?#2", update.Length - 1, 1);
                        else update.Replace(',', ' ', update.Length - 1, 1);
                        if (this._idColumn == null) this._idColumn = table.Columns["ID"];
                        sql = update.Append(" where ").Append(this._idColumn.ColumnName).Append(" = ").Append(table.Rows[0][this._idColumn]);
                        break;
                    case InputType.Search:
                        search.Length = search.Length - 5;
                        if (extend) search.Append("?#1?#2");
                        sql = search;
                        break;
                    default:
                        return null;
                }
                if (extend) sql.Insert(0, "1");
            }
            else if (extend)
            {
                switch (this._inputTyp)
                {
                    case InputType.Add:
                        sql = insert.Append("?#1)").Append(" values(?#2)");
                        break;
                    case InputType.Edit:
                        if (this._idColumn == null) this._idColumn = table.Columns["ID"];
                        sql = update.Append("?#1?#2 where ").Append(this._idColumn.ColumnName).Append(" = ").Append(table.Rows[0][this._idColumn]);
                        break;
                    case InputType.Search:
                        sql = search.Append("?#1?#2");
                        break;
                    default:
                        return null;
                }
                sql.Insert(0, "0");
            }
            return sql;
        }
        /// <summary>
        /// Erweitert sql-string. Muss mit endExtendSql abgeschlossen werden
        /// </summary>
        /// <param name="sql">sql-string</param>
        /// <param name="table">DB-Tabelle</param>
        /// <param name="attrName">Attributnamen</param>
        /// <param name="attrValue">Attributwert</param>
        public void extendSql(StringBuilder sql, DataTable table, string attrName, object attrValue)
        {
            char c = sql[0];
            string comma = (c == '0' ? "" : ",");
            string logExp = (c == '0' ? "" : " and ");

            switch (this._inputTyp)
            {
                case InputType.Add:
                    sql = sql.Replace("?#1", comma + attrName + "?#1");
                    sql = sql.Replace("?#2", this._dbColumn.AddToSql(comma, table.Columns[attrName], attrValue) + "?#2");
                    break;
                case InputType.Edit:
                    sql = sql.Replace("?#1", comma + attrName + "=");
                    sql = sql.Replace("?#2", this._dbColumn.AddToSql("", table.Columns[attrName], attrValue) + "?#1?#2");
                    break;
                case InputType.Search:
                    sql = sql.Replace("?#1", logExp + attrName + "=");
                    sql = sql.Replace("?#2", this._dbColumn.AddToSql("", table.Columns[attrName], attrValue) + " ?#1?#2");
                    break;
                default:
                    return;
            }
            if (c == '0') sql = sql.Replace('0', '1', 0, 1);
        }
        /// <summary>
        /// Check if extended Sql-string is empty (call before endExtendSql)
        /// </summary>
        /// <param name="sql">sql-string</param>
        /// <returns>true: string empty</returns>
        public bool emptySql(StringBuilder sql)
        {
            char c = sql[0];

            return c == '0';
        }
        /// <summary>
        /// Schliesst erweiterten sql-string ab
        /// </summary>
        /// <param name="sql">sql-string</param>
        /// <returns>abgeschlossener string</returns>
        public string endExtendSql(StringBuilder sql)
        {
            char c = sql[0];

            if (c == '0') sql.Length = 0;
            else
            {
                sql = sql.Replace("?#1", "").Replace("?#2", "");
                sql = sql.Remove(0, 1);
            }
            return sql.ToString();
        }
        public bool checkInputValue(DataTable table, Table inputTab, LanguageMapper map)
        {
            int state = 0;
            System.Web.UI.Control control = null;
            bool ok = true;
            string[] names;
            string columnName;

            for (int idx = 0; idx < inputTab.Rows.Count; idx++)
            {
                TableRow r = inputTab.Rows[idx];

                foreach (TableCell cell in r.Cells)
                {
                    string id = null;
                    bool isCal = false;

                    if (!cell.HasControls())
                    {
                        continue;
                    }

                    try
                    {
                        id = cell.Controls[0].Controls[0].Controls[0].Controls[0].ID;
                        isCal = true;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        id = cell.Controls[0].ID;
                    }

                    //if (!cell.HasControls() || cell.Controls[0].ID == null || !cell.Controls[0].ID.StartsWith("Input-")) continue;
                    if (id == null || !id.StartsWith("Input-")) continue;
                    //control = cell.Controls[0];
                    if (isCal)
                    {
                        control = cell.Controls[0].Controls[0].Controls[0].Controls[0];
                    }
                    else
                    {
                        control = cell.Controls[0];
                    }
                    names = control.ID.Split('-');
                    if (names.Length < 3) continue;
                    columnName = names[2];
                    state = 0;

                    try
                    {
                        if (control is TextBox)
                        {
                            TextBox textBox = control as TextBox;
                            string text = "";
                            string origControlText = "";

                            if (control is TwoValuesTextBox)
                            {
                                text = (control as TwoValuesTextBox).ValidText;
                            }
                            else
                            {
                                text = textBox.Text;
                            }

                            if (_inputTyp != InputType.Search && textBox.ID.EndsWith("_Confirm"))
                            {

                                string origID = textBox.ID.Substring(0, textBox.ID.LastIndexOf("_Confirm"));
                                System.Web.UI.Control origControl = inputTab.FindControl(origID);
                                TextBox origTextBox = origControl as TextBox;

                                if (origControl is TwoValuesTextBox)
                                {
                                    origControlText = (origControl as TwoValuesTextBox).ValidText;
                                }
                                else
                                {
                                    origControlText = origTextBox.Text;
                                }

                                if (origTextBox != null && origControlText != text)
                                {
                                    state = 50;
                                }
                            }
                            else
                            {
                                state = _dbColumn.Check(table.Columns[columnName], text);
                            }
                        }
                        else if (control is DropDownList)
                        {
                            state = _dbColumn.Check(table.Columns[columnName], (control as DropDownList).SelectedItem.Value);
                        }

                        if (state > 0 && !((_inputTyp == InputType.Search) && (state == 40)))
                        {
                            TableRow errorRow = new TableRow();
                            TableCell errorCell = new TableCell();

                            errorRow.Cells.Add(new TableCell());
                            if (_inputTyp == InputType.Search)
                                errorRow.Cells.Add(new TableCell());
                            errorRow.Cells.Add(errorCell);
                            errorRow.CssClass = "InputMask_Error";
                            inputTab.Rows.AddAt(idx + 1, errorRow);
                            idx++;
                            errorCell.Text = map.get("error", "inputErr_" + state);
                            if (control is TextBox || control is DropDownList)
                            {
                                (control as WebControl).CssClass = "InputMask_Error";
                            }
                            ok = false;
                        }
                    }
                    catch { }
                }
            }
            return ok;
        }

        public object getInputValue(DataTable table, Table inputTab, string columnName)
        {
            System.Web.UI.Control control = null;
            string[] names;
            string colName;

            for (int idx = 0; idx < inputTab.Rows.Count; idx++)
            {
                TableRow r = inputTab.Rows[idx];

                foreach (TableCell cell in r.Cells)
                {
                    if (!cell.HasControls() || cell.Controls[0].ID == null || !cell.Controls[0].ID.StartsWith("Input-")) continue;
                    control = cell.Controls[0];
                    names = control.ID.Split('-');
                    if (names.Length < 3) continue;
                    colName = names[2];

                    if (colName.ToUpper() == columnName.ToUpper())
                    {
                        Object retObject = "";
                        try
                        {
                            if (control is TwoValuesTextBox)
                            {
                                retObject = (control as TwoValuesTextBox).ValidText;
                            }
                            else if (control is TextBox)
                            {
                                retObject = (control as TextBox).Text;
                            }
                            else if (control is Label)
                            {
                                retObject = (control as Label).Text;
                            }
                            else if (control is CheckBox)
                            {
                                retObject = (control as CheckBox).Checked;
                            }
                            else if (control is ListControl)
                            {
                                ListItem item = (control as ListControl).SelectedItem;
                                if (item != null)
                                {
                                    retObject = item.Value;
                                }
                            }
                        }
                        catch { }
                        return retObject;
                    }
                }
            }
            return "";
        }

        public void setInputValue(DataTable table, Table inputTab, string columnName, object value)
        {
            System.Web.UI.Control control = null;
            string[] names;
            string colName;

            for (int idx = 0; idx < inputTab.Rows.Count; idx++)
            {
                TableRow r = inputTab.Rows[idx];

                foreach (TableCell cell in r.Cells)
                {
                    if (!cell.HasControls() || cell.Controls[0].ID == null || !cell.Controls[0].ID.StartsWith("Input-")) continue;
                    control = cell.Controls[0];
                    names = control.ID.Split('-');
                    if (names.Length < 3) continue;
                    colName = names[2];

                    if (colName.ToUpper() == columnName.ToUpper())
                        try
                        {
                            if (control is TwoValuesTextBox)
                            {
                                (control as TwoValuesTextBox).OrigText
                                    = value != null ? value.ToString() : "";
                            }
                            else if (control is TextBox)
                            {
                                (control as TextBox).Text = value != null ? value.ToString() : "";
                            }
                            else if (control is Label)
                            {
                                (control as Label).Text = value != null ? value.ToString() : "";
                            }
                            else if (control is CheckBox)
                            {
                                (control as CheckBox).Checked = value != null ? Convert.ToBoolean(value) : false;
                            }
                            else if (control is ListControl)
                            {
                                ListControl listControl = control as ListControl;
                                foreach (ListItem item in listControl.Items)
                                {
                                    if (item.Value.Equals(value))
                                    {
                                        listControl.SelectedIndex = listControl.Items.IndexOf(item);
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                }
            }
        }

        private static DropDownList NewRelationList()
        {
            DropDownList list = new DropDownList();
            list.Items.Add(new ListItem("~", " like "));
            list.Items.Add(new ListItem("=", " = "));
            list.Items.Add(new ListItem("<>", " <> "));
            list.Items.Add(new ListItem("<", " < "));
            list.Items.Add(new ListItem(">", " > "));
            list.Items.Add(new ListItem("<=", " <= "));
            list.Items.Add(new ListItem(">=", " >= "));
            return list;
        }
        private void onAddRow(DataRow row, DataColumn column, TableRow r)
        {
            if (addRow != null)
            {
                // Invokes the delegates. 
                addRow(row, column, r);
            }
        }
        private void onBuildSQL(StringBuilder build, System.Web.UI.Control control, DataColumn col, object val)
        {
            if (buildSQL != null)
            {
                // Invokes the delegates. 
                buildSQL(build, control, col, val);
            }
            else _dbColumn.AddToSql(build, col, val);
        }

        private void setFocusControl(WebControl control)
        {
            if (control is TextBox || control is CheckBox || control is ListControl)
            {
                PsoftContentPage.SetFocusControl(control, true);
            }
        }
    }
}
