using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for PDFList.
    /// </summary>
    public class PDFList : XMLPDFList {
        protected DataTable _layout = new DataTable();
        protected DataTable _header = new DataTable();
        protected DataTable _column = new DataTable();
        protected DataTable _data = new DataTable();
        protected LanguageMapper _map = null;
        private ReportModule.Layout _listLayout = ReportModule.Layout.Uniform;
        private string[] colors = {"white","#cccccc"};

        public PDFList() {
        }
        public void writeList(string title, string path, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues) {
            int id = (int) db.lookup("id","reportlayout","title_mnemo='"+title+"'");
            writeList(id,path,"",map,db,data,session,substituteValues);
        }
        public override void writeList(long id, string path, string imagePath, LanguageMapper map, DBData db, DataTable data, HttpSessionState session, params string[] substituteValues) {

            try {
                base.open(id,path,db,session);

                _map = map;
                _data = data;
                if (!base.prepareLayout(id, out _layout, out _header, out _column, ref _data,substituteValues)) return;

                if ((ReportModule.ReportType) _layout.Rows[0]["type"] != ReportModule.ReportType.List) return;

                _listLayout = (ReportModule.Layout) _layout.Rows[0]["layout"];

                if (!_append) {
                    _writer.WriteStartElement("document");
                    _writer.WriteAttributeString("page-size","A4");
                    _writer.WriteAttributeString("margin-left","0.5cm");
                    _writer.WriteAttributeString("margin-right","0.5cm");
                    _writer.WriteAttributeString("margin-top","0.2cm");
                    _writer.WriteAttributeString("margin-bottom","0.5cm");

//                    _writer.WriteStartElement("images");
//                    _writer.WriteStartElement("image");
//                    _writer.WriteAttributeString("directory",imagePath);
//                    _writer.WriteAttributeString("file-name",ReportModule.headerLogoImage);
//                    _writer.WriteAttributeString("image-name","logo");
//                    _writer.WriteEndElement();
//                    _writer.WriteEndElement();

                    _writer.WriteStartElement("next-page");
                    _writer.WriteAttributeString("orientation",((ReportModule.Format)_layout.Rows[0]["format"]).ToString().ToLower());
                    _writer.WriteEndElement();
                }
                
                // footer
                _writer.WriteStartElement("page-footer");
                _writer.WriteAttributeString("space-before","10");
                _writer.WriteAttributeString("font-name","helvetica");
                _writer.WriteAttributeString("font-size","8");
                _writer.WriteStartElement("row");
                _writer.WriteAttributeString("vertical-align","bottom");

                _writer.WriteStartElement("cell");
                _writer.WriteAttributeString("align","left");
                _writer.WriteString(_map.get("reportLayout", _layout.Rows[0]["title_mnemo"].ToString()));
                _writer.WriteString(" \u00a9 p-soft");
                _writer.WriteEndElement();

                _writer.WriteStartElement("cell");
                _writer.WriteAttributeString("align","center");
                _writer.WriteString(_map.get("reportLayout", "stand") + ": " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                _writer.WriteEndElement();

                _writer.WriteStartElement("cell");
                _writer.WriteAttributeString("align","right");
                _writer.WriteString(_map.get("reportLayout", "page") + " ");
                _writer.WriteStartElement("page-number");
                _writer.WriteEndElement();
                _writer.WriteString("/");
                _writer.WriteStartElement("forward-reference");
                _writer.WriteAttributeString("name","total-pages");
                _writer.WriteEndElement();
                _writer.WriteEndElement();

                _writer.WriteEndElement();
                _writer.WriteEndElement();

                // header
                _writer.WriteStartElement("page-header");
                _writer.WriteAttributeString("border-width-all","0");
                _writer.WriteStartElement("row");
                _writer.WriteStartElement("cell");

                _writer.WriteStartElement("table");
                _writer.WriteAttributeString("border-width-all","0");
                _writer.WriteAttributeString("widths","*,2.5cm");
                writeHeader(substituteValues);
                _writer.WriteEndElement(); // table

                _writer.WriteEndElement(); // cell
                _writer.WriteEndElement(); // row
                string w = writeColumnHeader();

                _writer.WriteEndElement(); // page header

                if (_append) {
                    _writer.WriteStartElement("new-page");
                    _writer.WriteEndElement();
                }
                // body
                writeList(w);

                if (!_extend) {
                    _writer.WriteEndElement(); // document
                }

                base.close();
                base.savePDF();
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                throw e;
            }
            finally {
                if (_writer != null) base.close();
            }
        }
        private void writeHeader(string[] substituteValues) {
            int rownum = -1;

            for (int r = 0; r < _header.Rows.Count; r++) {
                DataRow row = _header.Rows[r];

                if ((int) row["rownumber"] != rownum) {
                    string widths = "";
                    if (rownum >= 0) {
                        _writer.WriteEndElement(); // row
                        _writer.WriteEndElement(); // table
                        _writer.WriteEndElement(); // cell
//                        if (rownum == 0) {
//                            _writer.WriteStartElement("cell");
//                            _writer.WriteAttributeString("align","right");
//                            _writer.WriteAttributeString("vertical-align","top");
//                            _writer.WriteAttributeString("rowspan",_header.Rows.Count.ToString());
//                            _writer.WriteAttributeString("padding-left","15");
//                            _writer.WriteAttributeString("min-height","1.5cm");
//                            _writer.WriteStartElement("show-image");
//                            _writer.WriteAttributeString("image-name","logo");
//                            //_writer.WriteAttributeString("scale-width","100");
//                            _writer.WriteEndElement();
//                            _writer.WriteEndElement();
//                        }
                        _writer.WriteEndElement(); // row
                    }
                    _writer.WriteStartElement("row");
                    _writer.WriteStartElement("cell");
                    _writer.WriteStartElement("table");
                    _writer.WriteAttributeString("border-width-all","0");
                    rownum = (int) row["rownumber"];
                    for (int rr = r; rr < _header.Rows.Count; rr++) {
                        DataRow tmpRow = _header.Rows[rr];

                        if ((int) tmpRow["rownumber"] != rownum) break;
                        if (!DBColumn.IsNull(tmpRow["textwidth"])) widths += ","+tmpRow["textwidth"];
                    }
                    if (widths != "") _writer.WriteAttributeString("widths",widths.Substring(1));
                    _writer.WriteStartElement("row");
                }
                ReportModule.HAlign halign = (ReportModule.HAlign) row["halign"];
                ReportModule.VAlign valign = (ReportModule.VAlign) row["valign"];

                _writer.WriteStartElement("cell");
                _writer.WriteAttributeString("font-name",row["font"].ToString());
                _writer.WriteAttributeString("font-size",row["fontsize"].ToString());
                _writer.WriteAttributeString("align",halign.ToString().ToLower());
                _writer.WriteAttributeString("vertical-align",valign.ToString().ToLower());
                if (!DBColumn.IsNull(row["rowheight"])) _writer.WriteAttributeString("min-height",row["rowheight"].ToString());
                _writer.WriteString(base.substitute(_map.get("reportLayout", row["text_mnemo"].ToString()),substituteValues));
                _writer.WriteEndElement();
            }
            if (rownum >= 0) {
                _writer.WriteEndElement(); // row
                _writer.WriteEndElement(); // table
                _writer.WriteEndElement(); // cell
//                if (rownum == 0) {
//                    _writer.WriteStartElement("cell");
//                    _writer.WriteAttributeString("align","right");
//                    _writer.WriteAttributeString("vertical-align","top");
//                    _writer.WriteAttributeString("rowspan",_header.Rows.Count.ToString());
//                    _writer.WriteStartElement("show-image");
//                    _writer.WriteAttributeString("image-name","logo");
//                    //_writer.WriteAttributeString("scale-width","100");
//                    _writer.WriteEndElement();
//                    _writer.WriteEndElement();
//                }
                _writer.WriteEndElement(); // row
            }
        }
        private string writeColumnHeader() {
            string widths = "";

            _writer.WriteStartElement("row");
            _writer.WriteStartElement("cell");
            _writer.WriteStartElement("table");
            _writer.WriteAttributeString("padding-right","5");
            _writer.WriteAttributeString("border-width-all","0");

            foreach (DataRow row in _column.Rows) {
                if (!DBColumn.IsNull(row["columnwidth"])) widths += ","+row["columnwidth"];
            }
            if (widths != "") {
                widths = widths.Substring(1);
                _writer.WriteAttributeString("widths",widths);
            }
            _writer.WriteStartElement("row");

            foreach (DataRow row in _column.Rows) {
                _writer.WriteStartElement("cell");
                if (!DBColumn.IsNull(row["headername_mnemo"])) {
                    ReportModule.HAlign halign = (ReportModule.HAlign) row["headerhalign"];
                    ReportModule.VAlign valign = (ReportModule.VAlign) row["headervalign"];

                    _writer.WriteAttributeString("font-name",row["headerfont"].ToString());
                    _writer.WriteAttributeString("font-size",row["headerfontsize"].ToString());
                    _writer.WriteAttributeString("align",halign.ToString().ToLower());
                    _writer.WriteAttributeString("vertical-align",valign.ToString().ToLower());
                    if (!DBColumn.IsNull(row["headerrowheight"])) _writer.WriteAttributeString("min-height",row["headerrowheight"].ToString());
                    _writer.WriteString(_map.get("reportLayout", row["headername_mnemo"].ToString()));
                }
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
            _writer.WriteEndElement();
            _writer.WriteEndElement();
            _writer.WriteEndElement();

            return widths;
        }
        private void writeList(string widths) {
            int rowNum = 0;
            int rowPerPage = int.MaxValue;

            if (!DBColumn.IsNull(_layout.Rows[0]["ROWSPERPAGE"])) rowPerPage = (int) _layout.Rows[0]["ROWSPERPAGE"];

            _writer.WriteStartElement("table");
            if (_listLayout == ReportModule.Layout.Grid) _writer.WriteAttributeString("border-width-all","0.5");
            else _writer.WriteAttributeString("border-width-all","0");
            _writer.WriteAttributeString("padding-right","5");
            if (widths != "") _writer.WriteAttributeString("widths",widths);

            foreach (DataRow row in _data.Rows) {
                _writer.WriteStartElement("row");
                if (_listLayout == ReportModule.Layout.Computer) {
                    _writer.WriteAttributeString("fill-color",colors[rowNum % 2]);
                }
                foreach (DataRow cell in _column.Rows) {
                    ReportModule.HAlign halign = (ReportModule.HAlign) cell["halign"];
                    ReportModule.VAlign valign = (ReportModule.VAlign) cell["valign"];
                    int idx = 0;
                    string pattern = DBColumn.IsNull(cell["formatpattern"]) ? "" : cell["formatpattern"].ToString();
                    string text = pattern;

                    _writer.WriteStartElement("cell");
                    _writer.WriteAttributeString("font-name",cell["font"].ToString());
                    _writer.WriteAttributeString("font-size",cell["fontsize"].ToString());
                    _writer.WriteAttributeString("align",halign.ToString().ToLower());
                    _writer.WriteAttributeString("vertical-align",valign.ToString().ToLower());
                    if (!DBColumn.IsNull(cell["rowheight"])) {
                        _writer.WriteAttributeString("min-height",cell["rowheight"].ToString());
                    }
                    foreach (string attr in cell[_db.langAttrName("reportcolumn", "attributname")].ToString().Split(',')) {
                        if (!DBColumn.IsNull(row[attr])) {
                            string display = _db.dbColumn.GetDisplayValue(_data.Columns[attr],row[attr],false);
                            if (pattern == "") text += display;
                            else text = text.Replace("$"+idx,display);
                        }
                        else if (pattern != "") {
                            int i = text.IndexOf("$"+idx);
                            int len = text.Length;

                            while (i >= 0) {
                                text = text.Remove(i,1);
                                if (text.Length == i || text[i] == '$') break;
                            }
                        }
                        idx++;
                    }
                    if (text != "") _writer.WriteString(text);
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
                rowNum++;
                if ((rowNum % rowPerPage) == 0) {
                    _writer.WriteEndElement();
                    _writer.WriteStartElement("new-page");
                    _writer.WriteEndElement();

                    _writer.WriteStartElement("table");
                    if (_listLayout == ReportModule.Layout.Grid) _writer.WriteAttributeString("border-width-all","0.5");
                    else _writer.WriteAttributeString("border-width-all","0");
                    _writer.WriteAttributeString("padding-right","5");
                    if (widths != "") _writer.WriteAttributeString("widths",widths);
                }
            }
            if (rowNum > 0) {
                _writer.WriteStartElement("row");
                _writer.WriteAttributeString("border-width-all","0");
                _writer.WriteStartElement("cell");
                _writer.WriteAttributeString("font-name",_column.Rows[0]["font"].ToString());
                _writer.WriteAttributeString("font-size",_column.Rows[0]["fontsize"].ToString());
                _writer.WriteAttributeString("align","left");
                _writer.WriteAttributeString("vertical-align","bottom");
                int sz = int.Parse(_column.Rows[0]["fontsize"].ToString())+10;
                _writer.WriteAttributeString("min-height",sz.ToString());
                _writer.WriteString(_map.get("reportLayout", "total") + ": ");
                _writer.WriteString(rowNum.ToString());
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
        }
    }
}
