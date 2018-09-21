using ch.appl.psoft.db;
using ch.appl.psoft.Knowledge;
using ch.appl.psoft.Knowledge.Controls;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
namespace ch.appl.psoft.Morph.Controls
{
    public partial class MatrixDetailCtrl : PSOFTListViewUserControl
    {
        protected DBData _db;
        private long _matrixID = -1L;
        private long _slaveId = -1L;
        protected MatrixDataContainer matrixData;
        private Boolean isNovisReport;
        public bool isSimpleKnowledge = false;
        // Properties
        public static string Path
        {
            get
            {
                return (Global.Config.baseURL + "/Morph/Controls/MatrixDetailCtrl.ascx");
            }
        }
        public long SlaveID
        {
            get
            {
                return this._slaveId;
            }
            set
            {
                this._slaveId = value;
            }
        }
        public long MatrixID
        {
            get
            {
                return this._matrixID;
            }
            set
            {
                this._matrixID = value;
            }
        }
        public Boolean IsNovisReport
        {
            get
            {
                return this.isNovisReport;
            }
            set
            {
                this.isNovisReport = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // is page refresh?
            bool isPageRefresh = false;
            //Hack MSr querystring matrixID <> Session matriID ???
            //if (Session["matrixId"] != null)
            //{
            //    if (!Request.QueryString["matrixId"].Equals(Session["matrixId"]))
            //    {
            //        Session["matrixId"] = null;
            //    }
            //}
            HiddenField isSimpleKnowledge = new HiddenField();
            isSimpleKnowledge.ID = "isSimpleKnowledge";
            this.Controls.Add(getKnowledgeWindowView());
            if (Knowledge.KnowledgeModule.simpleKnowledge)
            {
                this.Controls.Add(getKnowledgeWindowEdit());
                isSimpleKnowledge.Value = "true";
            }
            else
            {
                isSimpleKnowledge.Value = "false";
            }
            this.Controls.Add(isSimpleKnowledge);
            if (!IsPostBack || !Session["matrixId"].Equals(_matrixID.ToString()))
            {
                if (Session["matrixId"] != null && Session["matrixId"].Equals(_matrixID.ToString() + isNovisReport.ToString()))
                {
                    isPageRefresh = true;
                }
                else if (Session["matrixId"] == null)
                {
                    Session.Add("matrixId", _matrixID.ToString() + isNovisReport.ToString());
                }
                else
                {
                    Session["matrixId"] = _matrixID.ToString() + isNovisReport.ToString();
                    isPageRefresh = false;
                }
            }
            //add SVG placeholder
            this._db = DBData.getDBData(base.Session);
            this._db.connect();
            //get datasource
            MatrixDataContainer matrixData;
            if (!Global.isModuleEnabled("novis") || !isPageRefresh)
            {
                if (IsNovisReport)
                {
                    matrixData = new MatrixDataContainer(_matrixID, _db, isNovisReport);
                }
                else
                {
                    matrixData = new MatrixDataContainer(_matrixID, _db);
                }
            }
            else
            {
                matrixData = new MatrixDataContainer(_matrixID, _db);
            }
            //scrTable = _db.getDataTable("SELECT * FROM Matrix_" + _db.userId + " WHERE DimensionId IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + _matrixID + ") ORDER BY DimensionOrdNumber, LINE");
            if (isNovisReport)
            {
                //scrTable.Merge(_db.getDataTable("SELECT * FROM Matrix_" + _db.userId + " WHERE DimensionId NOT IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + _matrixID + ")ORDER BY MATRIXID, DimensionOrdNumber, Line "));
            }
            //Title
            //Daten in Table schreiben---
            DataTable tblTitel = new DataTable();
            tblTitel = this._db.getDataTable("select * from matrix WHERE ID=" + MatrixID, new object[0]);
            //Main
            //create main Table
            HtmlTable MyTable = new HtmlTable();
            MyTable.ID = "MainTable";
            MyTable.Attributes["Class"] = MatrixID.ToString();
            //Coloration
            //Daten in Table schreiben---
            DataTable tblColor = new DataTable();
            tblColor = this._db.getDataTable("select * from coloration WHERE MATRIX_ID=" + MatrixID + " ORDER BY ORDNUMBER", new object[0]);
            //--------------------------------TITLE-------------------------------
            //add Title Rows
            HtmlTableRow titleRow1 = new HtmlTableRow();
            HtmlTableRow titleRow2 = new HtmlTableRow();
            HtmlTableCell[] titleCells = new HtmlTableCell[4];
            for (int i = 0; i < 4; i++)
            {
                titleCells[i] = new HtmlTableCell();
                if (i < 1)
                {
                    titleCells[i].Attributes["Class"] = "TitleRow1";
                    titleRow1.Cells.Add(titleCells[i]);
                    titleCells[i].ID = "TitleCell0_0";
                }
                else
                {
                    titleCells[i].Attributes["Class"] = "TitleRow2";
                    titleRow2.Cells.Add(titleCells[i]);
                    titleCells[i].ID = "TitleCell1_" + (i - 1).ToString();
                }
            }
            titleCells[0].Attributes["colspan"] = matrixData.colCount.ToString();
            titleCells[1].Attributes["colspan"] = "2";
            titleCells[2].Attributes["colspan"] = "4";
            titleCells[3].Attributes["colspan"] = (matrixData.colCount - 6).ToString();
            MyTable.Rows.Add(titleRow1);
            MyTable.Rows.Add(titleRow2);
            //add inner tables Title
            //row 1
            HtmlTable titleCellTable1 = new HtmlTable();
            titleCellTable1.Attributes["Class"] = "TitleCellTable";
            HtmlTableRow[] titleCellTableRows1 = new HtmlTableRow[5];
            HtmlTableCell[] titleCellTableCells1 = new HtmlTableCell[11 + matrixData.colCount];
            for (int i = 0; i < 5; i++)
            {
                titleCellTableRows1[i] = new HtmlTableRow();
                titleCellTable1.Rows.Add(titleCellTableRows1[i]);
            }
            for (int i = 0; i < 11; i++)
            {
                titleCellTableCells1[i] = new HtmlTableCell();
                titleCellTableCells1[i].ID = "TitleRowCell" + i.ToString();
                switch (i)
                {
                    case 0:
                        titleCellTableRows1[0].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = "2";
                        titleCellTableCells1[i].Attributes["rowspan"] = "4";
                        break;
                    case 1:
                        titleCellTableRows1[0].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = "4";
                        titleCellTableCells1[i].Attributes["rowspan"] = "2";
                        break;
                    case 2:
                        titleCellTableRows1[0].Cells.Add(titleCellTableCells1[i]);
                        break;
                    case 3:
                        titleCellTableRows1[0].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = (matrixData.colCount - 7).ToString();
                        break;
                    case 4:
                        titleCellTableRows1[1].Cells.Add(titleCellTableCells1[i]);
                        break;
                    case 5:
                        titleCellTableRows1[1].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = (matrixData.colCount - 7).ToString();
                        break;
                    case 6:
                        titleCellTableRows1[2].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = "4";
                        titleCellTableCells1[i].Attributes["rowspan"] = "2";
                        break;
                    case 7:
                        titleCellTableRows1[2].Cells.Add(titleCellTableCells1[i]);
                        break;
                    case 8:
                        titleCellTableRows1[2].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = (matrixData.colCount - 7).ToString();
                        break;
                    case 9:
                        titleCellTableRows1[3].Cells.Add(titleCellTableCells1[i]);
                        break;
                    case 10:
                        titleCellTableRows1[3].Cells.Add(titleCellTableCells1[i]);
                        titleCellTableCells1[i].Attributes["colspan"] = (matrixData.colCount - 7).ToString();
                        break;
                }
            }
            //titlesupportRow for adjusting cellwidth according to nummer of columns
            for (int i = 11; i < matrixData.colCount + 11; i++)
            {
                titleCellTableCells1[i] = new HtmlTableCell();
                titleCellTableRows1[4].Cells.Add(titleCellTableCells1[i]);
            }
            titleCellTableRows1[4].Attributes["Class"] = "TitleSupportRow";
            titleCells[0].Controls.Add(titleCellTable1);
            //Row 2 Cell1 ( Description Cell )
            HtmlTable titleCellTable1_0 = new HtmlTable();
            titleCellTable1_0.Attributes["Class"] = "TitleCellTable";
            HtmlTableRow[] titleCellTableRows2 = new HtmlTableRow[2];
            HtmlTableCell[] titleCellTableCells2 = new HtmlTableCell[2];
            for (int i = 0; i < 2; i++)
            {
                titleCellTableRows2[i] = new HtmlTableRow();
                titleCellTableCells2[i] = new HtmlTableCell();
                titleCellTableCells2[i].ID = "DescriptionCell" + i.ToString();
                titleCellTableRows2[i].Cells.Add(titleCellTableCells2[i]);
                titleCellTable1_0.Rows.Add(titleCellTableRows2[i]);
            }
            titleCellTableRows2[0].Attributes["Class"] = "TitleCellTableHeader";
            titleCells[1].Controls.Add(titleCellTable1_0);
            //Row 2 Cell2 ( Legend )
            HtmlTable titleCellTable1_1 = new HtmlTable();
            titleCellTable1_1.Attributes["Class"] = "TitleCellTable";
            HtmlTableRow[] titleCellTableRows3 = new HtmlTableRow[5];
            HtmlTableCell[] titleCellTableCells3 = new HtmlTableCell[9];
            titleCellTableCells3[0] = new HtmlTableCell();
            titleCellTableCells3[0].ID = "Legend0_0";
            titleCellTableCells3[0].Attributes["colspan"] = "2";
            titleCellTableRows3[0] = new HtmlTableRow();
            titleCellTableRows3[0].Cells.Add(titleCellTableCells3[0]);
            titleCellTableRows3[0].Attributes["Class"] = "TitleCellTableHeader";
            titleCellTable1_1.Rows.Add(titleCellTableRows3[0]);
            for (int i = 1; i < 5; i++)
            {
                titleCellTableRows3[i] = new HtmlTableRow();
                for (int cell = 0; cell < 2; cell++)
                {
                    titleCellTableCells3[cell + i * 2 - 1] = new HtmlTableCell();
                    titleCellTableCells3[cell + i * 2 - 1].ID = "Legend" + i.ToString() + "_" + cell.ToString();
                    titleCellTableRows3[i].Cells.Add(titleCellTableCells3[cell + i * 2 - 1]);
                    titleCellTableCells3[cell + i * 2 - 1].Attributes["Class"] = "TitleChange";
                }
                titleCellTable1_1.Rows.Add(titleCellTableRows3[i]);
            }
            titleCells[2].Controls.Add(titleCellTable1_1);
            //add colored squares
            DataTable matrixColors = _db.getDataTable("SELECT * FROM COLORATION WHERE MATRIX_ID=" + MatrixID + " ORDER BY ORDNUMBER");
            foreach (DataRow row in matrixColors.Rows)
            {
                titleCellTableCells3[row.Table.Rows.IndexOf(row) + 1].InnerHtml = "<span style= \" Background-color:" + System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(int.Parse(row.ItemArray[6].ToString()))) + "; \"></span>" + row.ItemArray[5].ToString();
            }
            //Row 2 Cell3 ( Wirkungspakete )
            HtmlTable titleCellTable1_2 = new HtmlTable();
            titleCellTable1_2.Attributes["Class"] = "TitleCellTable";
            HtmlTableRow[] titleCellTableRows4 = new HtmlTableRow[5];
            HtmlTableCell[] titleCellTableCells4 = new HtmlTableCell[9];
            titleCellTableCells4[0] = new HtmlTableCell();
            titleCellTableCells4[0].ID = "WirkungspaketeHeader";
            titleCellTableCells4[0].Attributes["colspan"] = "3";
            titleCellTableRows4[0] = new HtmlTableRow();
            titleCellTableCells4[0].InnerText = base._mapper.get("matrix", "causeContainer");
            titleCellTableRows4[0].Cells.Add(titleCellTableCells4[0]);
            titleCellTableRows4[0].Attributes["Class"] = "TitleCellTableHeader";
            titleCellTable1_2.Rows.Add(titleCellTableRows4[0]);
            titleCellTableRows4[0].Height = "0px";
            DataTable WirkungspaketColors = _db.getDataTable("SELECT * FROM Wirkungspaket WHERE MATRIX_ID=" + MatrixID);
            for (int i = 1; i < 5; i++)
            {
                titleCellTableRows4[i] = new HtmlTableRow();
                for (int cell = 0; cell < 2; cell++)
                {
                    titleCellTableCells4[cell + i * 2 - 1] = new HtmlTableCell();
                    titleCellTableRows4[i].Cells.Add(titleCellTableCells4[cell + i * 2 - 1]);
                    titleCellTableCells4[cell + i * 2 - 1].Attributes["Class"] = "WirkungspaketCell";
                }
                titleCellTableRows4[i].Controls.Add(new HtmlTableCell());
                titleCellTable1_2.Rows.Add(titleCellTableRows4[i]);
            }
            HtmlInputButton[] wirkungspaketButton = new HtmlInputButton[5];
            for (int i = 1; i < 6; i++)
            {
                titleCellTableCells4[i].ID = "Wirkungspakete_" + WirkungspaketColors.Rows[i - 1].ItemArray[0].ToString();
                titleCellTableCells4[i].Attributes["Class"] = titleCellTableCells4[i].Attributes["Class"].ToString() + " TitleChange";
                titleCellTableCells4[i].Attributes["OnClick"] = "SetActiveWP(this)";
                //set first WP active
                //if (i == 1)
                //{
                //    titleCellTableCells4[i].Attributes["Class"] += " WirkungspaketCell ActiveWP";
                //}
                wirkungspaketButton[i - 1] = new HtmlInputButton();
                wirkungspaketButton[i - 1].Value = "";
                wirkungspaketButton[i - 1].ID = "wirkungspaketButton" + i.ToString();
                titleCellTableCells4[i].InnerHtml = "<span id=\"wirkungspaketColor" + i.ToString() + "\" style=\" background-color : " + System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(int.Parse(WirkungspaketColors.Rows[i - 1].ItemArray[4].ToString()))) + "; \"></span>" + WirkungspaketColors.Rows[i - 1].ItemArray[5].ToString() + "<img src=\"../images/Morph/trash.png\" class=\"RemoveWirkungspaket\"></img>";
                titleCellTableCells4[i].Controls.AddAt(0, wirkungspaketButton[i - 1]);
            }
            titleCells[3].Controls.Add(titleCellTable1_2);
            //check permissions + create main menu
            DBData data = DBData.getDBData(this.Session);
            int y = 0;
            HtmlTableCell[] mapFunctionsArray = new HtmlTableCell[8];
            if (data.hasRowAuthorisation(8, "Matrix", _matrixID, true, true))
            {
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<img  href=\"javascript:void(0);\" src=\"../images/editMode_map.png\" title=\"" + base._mapper.get("matrix", "editMode") + "\" id=\"editModeButton\">";
                y++;
            }
            if (data.hasRowAuthorisation(4, "Matrix", _matrixID, true, true) && (!Global.isModuleEnabled("SBS") || (data.hasRowAuthorisation(16, "Matrix", _matrixID, true, true))))
            {
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<img  href=\"javascript:void(0);\" src=\"../images/addmap_map.png\" title=\"" + base._mapper.get("matrix", "newMap") + "\" id=\"newTableButton\">";
                y++;
            }
            if (data.hasRowAuthorisation(16, "Matrix", _matrixID, true, true))
            {
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<img  href=\"javascript:void(0);\"  src=\"../images/copy_map.png\" title=\"" + base._mapper.get("matrix", "copyMap") + "\" id=\"copyTableButton\">";
                y++;
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<img href=\"javascript:void(0);\"  src=\"../images/delete_map.png\" title=\"" + base._mapper.get("matrix", "deleteMap") + "\" id=\"deleteTableButton\">";
                y++;
            }
            if (data.hasRowAuthorisation(32, "Matrix", _matrixID, true, true))
            {
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<img  href=\"javascript:void(0);\"  src=\"../images/permissions_map.png\" title=\"" + base._mapper.get("matrix", "permissions") + "\" id=\"permissionsButton\">";
                y++;
            }
            mapFunctionsArray[y] = new HtmlTableCell();
            mapFunctionsArray[y].InnerHtml = "<img  href=\"javascript:void(0);\"  src=\"../images/print_map.png\" title=\"" + base._mapper.get("matrix", "printMap") + "\" id=\"printTableButton\">";
            y++;
            mapFunctionsArray[y] = new HtmlTableCell();
            mapFunctionsArray[y].InnerHtml = "<img  href=\"javascript:void(0);\"  src=\"../images/open_other_map.png\" title=\"" + base._mapper.get("matrix", "changeMap") + "\" id=\"changeTableButton\">";
            y++;
            if (Global.Config.isModuleEnabled("mks") && data.hasRowAuthorisation(32, "Matrix", _matrixID, true, true))
            {
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<a href=\"../MKS/UploadExcel.aspx?MatrixId=" + MatrixID + "\"><img src=\"../images/upload_excel.png\" border=\"0\" title=\"" + base._mapper.get("matrix", "importExcel") + "\" id=\"uploadExcel\"></a>";
                y++;
            }
            if (data.hasRowAuthorisation(2, "Matrix", _matrixID, true, true) && !(Global.Config.isModuleEnabled("mks") && data.hasRowAuthorisation(32, "Matrix", _matrixID, true, true)))
            {
                mapFunctionsArray[y] = new HtmlTableCell();
                mapFunctionsArray[y].InnerHtml = "<img href=\"javascript:void(0);\" src=\"../images/max_map.png\" title=\"" + base._mapper.get("matrix", "maxMap") + "\" id=\"mapMaxButton\">";
                y++;
            }
            for (; y < mapFunctionsArray.Length; y++)
            {
                mapFunctionsArray[y] = new HtmlTableCell();
            }
            HtmlTableRow mapFunctionsRows1 = new HtmlTableRow();
            HtmlTableRow mapFunctionsRows2 = new HtmlTableRow();
            for (y = 0; y < mapFunctionsArray.Length; y++)
            {
                if (y < 4)
                {
                    mapFunctionsRows1.Controls.Add(mapFunctionsArray[y]);
                }
                else
                {
                    mapFunctionsRows2.Controls.Add(mapFunctionsArray[y]);
                }
            }
            HtmlTable mapFunctionsMenu = new HtmlTable();
            mapFunctionsMenu.Controls.Add(mapFunctionsRows1);
            mapFunctionsMenu.Controls.Add(mapFunctionsRows2);
            //add title content
            titleCellTableCells1[1].InnerText = tblTitel.Rows[0]["TITLE"].ToString();
            titleCellTableCells1[1].Attributes["Class"] += "TitleChange";
            titleCellTableCells2[1].InnerText = tblTitel.Rows[0]["DESCRIPTION"].ToString();
            titleCellTableCells1[2].InnerText = base._mapper.get("matrix", "responsible");
            titleCellTableCells1[4].InnerText = base._mapper.get("matrix", "date");
            titleCellTableCells1[6].InnerText = tblTitel.Rows[0]["SUBTITLE"].ToString();
            titleCellTableCells1[6].Attributes["Class"] += "TitleChange";
            titleCellTableCells1[7].InnerText = base._mapper.get("matrix", "lastChange");
            titleCellTableCells1[9].InnerText = base._mapper.get("matrix", "revision");
            titleCellTableCells1[10].Attributes["Class"] += "TitleChange";
            if (Global.Config.ApplicationName.ToUpper().Equals("ULS"))
            {
                titleCellTableCells2[0].InnerText = (char)9426 + " Unternehmer-Leitsystem ";
            }
            else
            {
                titleCellTableCells2[0].InnerText = (char)9426 + " THE SOKRATES MAP CONCEPT ";
            }
            titleCellTableCells1[3].InnerText = tblTitel.Rows[0]["AUTHOR"].ToString(); ;
            titleCellTableCells1[3].Attributes["Class"] += "TitleChange";
            titleCellTableCells1[5].InnerText = tblTitel.Rows[0]["CREATIONDATE"].ToString();
            //titleCellTableCells1[5].Attributes["Class"] += "TitleChange";
            titleCellTableCells1[8].InnerText = tblTitel.Rows[0]["LASTCHANGE"].ToString();
            titleCellTableCells1[10].InnerText = tblTitel.Rows[0]["REVISION"].ToString();
            //titleCellTableCells1[10].Attributes["Class"] += "TitleChange";
            titleCellTableCells1[0].Controls.Add(mapFunctionsMenu);
            System.Web.UI.HtmlControls.HtmlGenericControl mainMenu =
        new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
            mainMenu.ID = "mainMenu";
            mainMenu.InnerHtml = "<select id=\"SelectMap\" size=\"10\" style=\"color: black\"></select>";
            mainMenu.Attributes["Style"] += "display:none;";
            this.Controls.Add(mainMenu);
            //--------------------------------------MAIN------------------------------------------
            //add Main Rows------
            int rowNr = matrixData.rowCount;
            int colNr = matrixData.colCount;
            int firstColumns = 2;
            bool secondColVisible = tblTitel.Rows[0]["TITLECOL2"].ToString().Equals("1");
            HtmlTableRow[] mainRows = new HtmlTableRow[rowNr];
            HtmlTableCell[] mainCells = new HtmlTableCell[rowNr * colNr];
            for (int rowCount = 0; rowCount < rowNr; rowCount++)
            {
                mainRows[rowCount] = new HtmlTableRow();
                mainRows[rowCount].Attributes["Class"] = "MainRow";
                for (int colCount = 0; colCount < colNr; colCount++)
                {
                    mainCells[rowCount * colNr + colCount] = new HtmlTableCell();
                    mainCells[rowCount * colNr + colCount].ID = "MainCell" + matrixData.matrixTable[rowCount, colCount].dimensionID.ToString() + "_" + matrixData.matrixTable[rowCount, colCount].ID;

                    if (colCount == 0)
                    {
                        mainCells[rowCount * colNr + colCount].Attributes["Class"] = "MainCell RowCell0";

                    }

                    if (colCount == 1)
                    {
                        mainCells[rowCount * colNr + colCount].Attributes["Class"] = "RowCell1 MainCell";
                    }

                    if (colCount > 1)
                    {
                        mainCells[rowCount * colNr + colCount].Attributes["Class"] = "RowCellDetail MainCell";
                    }

                    mainRows[rowCount].Cells.Add(mainCells[rowCount * colNr + colCount]);
                }
                MyTable.Rows.Add(mainRows[rowCount]);
            }
            this.Controls.Add(MyTable);
            if (Global.Config.ApplicationName.Equals("Sokrates"))
            {
                //add row for Sokrateslogo
                HtmlTable sokratesLogo = new HtmlTable();
                //sokratesLogo.Width = "100%";
                sokratesLogo.ID = "sokratesLogoFooter";
                this.Controls.Add(sokratesLogo);
                HtmlTableRow sokratesLogoRow = new HtmlTableRow();
                HtmlTableCell sokratesLogoCell = new HtmlTableCell();

                sokratesLogoCell.Align = "left";
                sokratesLogoCell.InnerHtml = "<img src=\"../images/morph/SokratesFooterLogo.png\" border=\"0\" height=\"20px\"></img>";
                sokratesLogoRow.Cells.Add(sokratesLogoCell);
                sokratesLogo.Rows.Add(sokratesLogoRow);
            }
            //add innerTables
            CellTable[] CellTables = new CellTable[rowNr * colNr];
            string cellColor;
            string cellText;
            DataTable colorTable = _db.getDataTable("SELECT ID, ORDNUMBER, COLOR FROM COLORATION WHERE MAtRIX_ID =" + _matrixID);
            for (int rowCount = 0; rowCount < rowNr; rowCount++)
            {
                for (int colCount = 0; colCount < colNr; colCount++)
                {
                    if (matrixData.matrixTable[rowCount, colCount].colorID == 0)
                    {
                        if (colCount < firstColumns)
                        {
                            cellColor = "FirstColumn";
                        }
                        else
                        {
                            cellColor = "Default";
                        }
                        if (!isNovisReport && matrixData.matrixTable[rowCount, colCount].knowledgeID != 0)
                        {
                            //if (Knowledge.KnowledgeModule.simpleKnowledge)
                            //{
                            cellText = "<a href=\"#\" knowledgeId = \"" + matrixData.matrixTable[rowCount, colCount].knowledgeID + "\" onclick=\"getKnowledgeSimple(" + matrixData.matrixTable[rowCount, colCount].knowledgeID + "); return false;\">" + matrixData.matrixTable[rowCount, colCount].title + "</a>";
                            //}
                            //else
                            //{
                            //    cellText = "<a href=\"" + KnowledgeDetail.GetURL(new object[] { "knowledgeID", matrixData.matrixTable[rowCount, colCount].knowledgeID }) + "\"" + ">" + matrixData.matrixTable[rowCount, colCount].title + "</a>";
                            //}
                        }
                        else
                        {
                            cellText = matrixData.matrixTable[rowCount, colCount].title;
                        }
                    }
                    else
                    {

                        cellColor = colorTable.Rows.Find(matrixData.matrixTable[rowCount, colCount].colorID)[1].ToString();

                    }
                    if (!isNovisReport && matrixData.matrixTable[rowCount, colCount].knowledgeID != 0)
                    {
                        cellText = "";
                        //if (Knowledge.KnowledgeModule.simpleKnowledge)
                        //{
                        cellText = "<a href=\"#\" knowledgeId = \"" + matrixData.matrixTable[rowCount, colCount].knowledgeID + "\" onclick=\"getKnowledgeSimple(" + matrixData.matrixTable[rowCount, colCount].knowledgeID + "); return false;\">" + matrixData.matrixTable[rowCount, colCount].title + "</a>";
                        //}
                        //else
                        //{
                        //    cellText = "<a href=\"" + KnowledgeDetail.GetURL(new object[] { "knowledgeID", matrixData.matrixTable[rowCount, colCount].knowledgeID }) + "\"" + ">" + matrixData.matrixTable[rowCount, colCount].title + "</a>";
                        //}
                    }
                    else
                    {
                        cellText = matrixData.matrixTable[rowCount, colCount].title;
                    }

                    CellTables[rowCount * colNr + colCount] = new CellTable(cellText, cellColor, rowCount * colNr, colCount);
                    if (colCount == 0)
                    {
                        mainCells[rowCount * colNr + colCount].Attributes["rowspan"] = matrixData.dimensionData.Rows[rowCount]["TITLEROWSPAN"].ToString();
                    }
                    else if (colCount == 1 && !secondColVisible)
                    {
                        mainCells[rowCount * colNr + colCount].Attributes["Style"] += "display:none;";
                    }
                    CellTables[rowCount * colNr + colCount].CellTbl.ID = "CellText_" + (rowCount * colNr).ToString() + "_" + colCount.ToString();
                    mainCells[rowCount * colNr + colCount].Controls.Add(CellTables[rowCount * colNr + colCount].CellTbl);
                }
            }
            DBData data1 = DBData.getDBData(this.Session);
            bool authorisation = data1.hasRowAuthorisation(4, "Matrix", _matrixID, true, true);
            // add icons and cellcolor
            for (int rowCount = 0; rowCount < rowNr; rowCount++)
            {
                for (int colCount = 0; colCount < colNr; colCount++)
                {
                    CellTables[rowCount * colNr + colCount].SupportCellB.InnerText = matrixData.matrixTable[rowCount, colCount].subtitle;
                    if (matrixData.matrixTable[rowCount, colCount].colorID != 0)
                    {
                        //mainCells[rowCount * colNr + colCount].Attributes["style"] = "background-color:" + System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(int.Parse(_db.lookup("color", "coloration", "ID= '" + scrTable.Rows[rowCount * 4]["CellColor_" + (colCount - firstColumns)].ToString() + "'").ToString()))) + ";";
                        if (colCount < firstColumns)
                        {
                            mainCells[rowCount * colNr + colCount].Attributes["style"] = "background-color:" + System.Drawing.ColorTranslator.ToHtml(calculateColorFirstColumn(Color.FromArgb(int.Parse(colorTable.Rows.Find(matrixData.matrixTable[rowCount, colCount].colorID)[2].ToString())))) + ";";
                        }
                        else
                        {
                            mainCells[rowCount * colNr + colCount].Attributes["style"] = "background-color:" + System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(int.Parse(colorTable.Rows.Find(matrixData.matrixTable[rowCount, colCount].colorID)[2].ToString()))) + ";";

                        }
                    }
                    if (matrixData.matrixTable[rowCount, colCount].ID != "")
                    {
                        if (authorisation)
                        {
                            CellTables[rowCount * colNr + colCount].SupportCellR.InnerHtml = CellTables[rowCount * colNr + colCount].SupportCellR.InnerHtml + "<a Class=\"WsButtonDel WsButtonDelVisible\" href=\"javascript:void(0);\"><img src=\"../images/DeleteWe.png\" border=\"0\"></img></a><a Class=\"WsButtonAdd WsButtonAddInvisible\" href=\"javascript:void(0);\"><img src=\"../images/CreateWe.png\" border=\"0\"></img></a>";

                            CellTables[rowCount * colNr + colCount].SupportCell.InnerHtml = "<a Class=\"ColorButton\" href=\"javascript:void(0);\"><img src=\"../images/pen.png\" border=\"0\"></img></a>";

                        }
                        else
                        {
                            CellTables[rowCount * colNr + colCount].SupportCellR.InnerHtml = CellTables[rowCount * colNr + colCount].SupportCellR.InnerHtml + "<a Class=\"WsButtonAdd\" href=\"\"></a><a Class=\"WsButtonDel\" href=\"\"></a>";

                            CellTables[rowCount * colNr + colCount].SupportCell.InnerHtml = "<a Class=\"ColorButton\" href=\"\"></a>";

                        }
                        if (!isNovisReport && matrixData.matrixTable[rowCount, colCount].subMatrixID != 0)
                        {
                            CellTables[rowCount * colNr + colCount].SupportCellR.InnerHtml += "<br><a href=\"" + MatrixDetail.GetURL(new object[] { "matrixID", matrixData.matrixTable[rowCount, colCount].subMatrixID }) + "\"><img src=\"../images/morph/uf_matrix.png\" border=\"0\"></img></a>";
                        }
                    }
                }
            }
            //store Permission clientside
            HiddenField Permission = new HiddenField();
            Permission.ID = "Permission";
            if (authorisation)
            {
                Permission.Value = "WRITE";
            }
            else
            {
                Permission.Value = "READ";
            }
            this.Controls.Add(Permission);
            createBackgroundImg(tblColor);
            //create AbbruchButton
            HtmlGenericControl AbbruchButtonContainer = new HtmlGenericControl();
            HtmlAnchor AbbruchButton = new HtmlAnchor();
            AbbruchButton.ID = "AbbruchButton";
            AbbruchButton.InnerText = base._mapper.get("matrix", "cancel");
            AbbruchButton.Attributes.Add("href", "javascript:void(0);");
            AbbruchButton.Attributes["Style"] += "display:none;";
            AbbruchButtonContainer.Controls.Add(AbbruchButton);
            this.Controls.Add(AbbruchButtonContainer);
            //create coloring menu
            HtmlTable colorMenu = new HtmlTable();
            colorMenu.ID = "colorMenu";
            colorMenu.Attributes["Style"] += "display:none;";
            this.Controls.Add(colorMenu);
            HtmlTableRow[] colorMenuRows = new HtmlTableRow[10];
            HtmlTableCell[] colorMenuCells = new HtmlTableCell[10];
            for (int i = 0; i < matrixColors.Rows.Count; i++)
            {
                colorMenuRows[i] = new HtmlTableRow();
                colorMenuCells[i] = new HtmlTableCell();
                colorMenuCells[i].Attributes["Class"] = "ColorMenuButton";
                colorMenuCells[i].Attributes["href"] = "javascript:void(0);";
                colorMenuCells[i].InnerHtml = "<span style= \" Background-color:" + System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(int.Parse(matrixColors.Rows[i][6].ToString()))) + "; \"> </span> <span Class=\"ColorMenuButtonText\" href=\"javascript:void(0);\">" + matrixColors.Rows[i][5].ToString() + "</span>";
                colorMenuRows[i].Controls.Add(colorMenuCells[i]);
                colorMenu.Controls.Add(colorMenuRows[i]);
            }

            colorMenuRows[9] = new HtmlTableRow();
            colorMenuCells[9] = new HtmlTableCell();
            colorMenuCells[9].Attributes["Class"] = "ColorMenuButton";
            colorMenuCells[9].Attributes["href"] = "javascript:void(0);";
            colorMenuCells[9].InnerHtml = "<span Class=\"ColorMenuButtonText\" href=\"javascript:void(0);\">" + base._mapper.get("matrix", "resetColor") + "</span>";
            colorMenuRows[9].Controls.Add(colorMenuCells[9]);
            colorMenu.Controls.Add(colorMenuRows[9]);

            // create print menu
            HtmlTable printMenu = new HtmlTable();
            printMenu.ID = "printMenu";
            printMenu.Attributes["Style"] += "display:none;";
            this.Controls.Add(printMenu);
            HtmlTableRow[] printMenuRows = new HtmlTableRow[3];
            HtmlTableCell[] printMenuCells = new HtmlTableCell[3];
            for (int i = 0; i < 3; i++)
            {
                printMenuRows[i] = new HtmlTableRow();
                printMenuCells[i] = new HtmlTableCell();
                printMenuRows[i].Controls.Add(printMenuCells[i]);
                printMenu.Controls.Add(printMenuRows[i]);
            }
            printMenuCells[0].InnerHtml = base._mapper.get("matrix", "orientation");
            printMenuCells[1].InnerHtml = "<select id =\"selectOrientation\"><option>" + base._mapper.get("matrix", "vertical") + "</option><option>" + base._mapper.get("matrix", "horizontal") + "</option></select>";
            HtmlInputButton printbutton = new HtmlInputButton();
            printbutton.ID = "printButton";
            printbutton.Value = base._mapper.get("matrix", "print");
            printbutton.Attributes["href"] = "javascript:void(0);";
            printMenuCells[2].Controls.Add(printbutton);
            HtmlInputButton cancelprintbutton = new HtmlInputButton();
            cancelprintbutton.ID = "cancelPrintButton";
            cancelprintbutton.Value = base._mapper.get("matrix", "cancel");
            cancelprintbutton.Attributes["href"] = "javascript:void(0);";
            printMenuCells[2].Controls.Add(cancelprintbutton);
            //create cell edit menu +
            HtmlTable plusMenu = new HtmlTable();
            plusMenu.ID = "plusMenu";
            plusMenu.Attributes["Style"] += "display:none;";
            this.Controls.Add(plusMenu);
            HtmlTableRow[] plusMenuRows = new HtmlTableRow[10];
            HtmlTableCell[] plusMenuCells = new HtmlTableCell[10];
            HtmlAnchor[] plusMenuAnchor = new HtmlAnchor[10];
            for (int i = 0; i < 10; i++)
            {
                plusMenuRows[i] = new HtmlTableRow();
                plusMenuCells[i] = new HtmlTableCell();
                plusMenuAnchor[i] = new HtmlAnchor();
                plusMenuAnchor[i].Attributes["Class"] = "PlusMenuButton";
                plusMenuAnchor[i].Attributes["href"] = "javascript:void(0);";
                plusMenuCells[i].Controls.Add(plusMenuAnchor[i]);
                plusMenuRows[i].Controls.Add(plusMenuCells[i]);
                plusMenu.Controls.Add(plusMenuRows[i]);
            }
            plusMenuCells[0].ID = "AddCellAfter";
            plusMenuAnchor[0].InnerText = base._mapper.get("matrix", "insertSokratesFollowing");
            plusMenuCells[1].ID = "AddCellBefore";
            plusMenuAnchor[1].InnerText = base._mapper.get("matrix", "insertSokratesPrevious");
            plusMenuCells[2].ID = "AddNewKnowledge";
            plusMenuAnchor[2].InnerText = base._mapper.get("matrix", "createLocalKnowledge");
            plusMenuCells[3].ID = "AddKnowledge";
            plusMenuAnchor[3].InnerText = base._mapper.get("matrix", "knowledgeLink");
            plusMenuCells[4].ID = "EditKnowledge";
            plusMenuAnchor[4].InnerText = base._mapper.get("matrix", "editKnowledgeLink");
            plusMenuCells[5].ID = "AddSubmatrix";
            plusMenuAnchor[5].InnerText = base._mapper.get("matrix", "addSubmatrix");
            plusMenuCells[6].ID = "AddRowAfter";
            plusMenuAnchor[6].InnerText = base._mapper.get("matrix", "insertRowFollowing");
            plusMenuCells[7].ID = "AddRowBefore";
            plusMenuAnchor[7].InnerText = base._mapper.get("matrix", "insertRowPrevious");
            plusMenuCells[8].ID = "MergeLowerCell";
            plusMenuAnchor[8].InnerText = base._mapper.get("matrix", "joinFollowingSokrateselement");
            plusMenuCells[9].ID = "AddTitleColumn2";
            plusMenuAnchor[9].InnerText = base._mapper.get("matrix", "addtitleColumn2");
            if (Global.isModuleEnabled("SBS") && !_db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", _db.userId, true, true))
            {
                plusMenuAnchor[0].Visible = false;
                plusMenuAnchor[1].Visible = false;
                plusMenuAnchor[2].Visible = false;
                plusMenuAnchor[3].Visible = false;
                plusMenuAnchor[4].Visible = false;
                plusMenuAnchor[5].Visible = false;
                plusMenuAnchor[6].Visible = false;
                plusMenuAnchor[7].Visible = false;
                plusMenuAnchor[8].Visible = false;
                plusMenuAnchor[9].Visible = false;
            }
            //create cell edit menu -
            HtmlTable minusMenu = new HtmlTable();
            minusMenu.ID = "minusMenu";
            minusMenu.Attributes["Style"] += "display:none;";
            this.Controls.Add(minusMenu);
            HtmlTableRow[] minusMenuRows = new HtmlTableRow[6];
            HtmlTableCell[] minusMenuCells = new HtmlTableCell[6];
            HtmlAnchor[] minusMenuAnchor = new HtmlAnchor[6];
            for (int i = 0; i < 6; i++)
            {
                minusMenuRows[i] = new HtmlTableRow();
                minusMenuCells[i] = new HtmlTableCell();
                minusMenuAnchor[i] = new HtmlAnchor();
                minusMenuAnchor[i].Attributes["Class"] = "MinusMenuButton";
                minusMenuAnchor[i].Attributes["href"] = "javascript:void(0);";
                minusMenuCells[i].Controls.Add(minusMenuAnchor[i]);
                minusMenuRows[i].Controls.Add(minusMenuCells[i]);
                minusMenu.Controls.Add(minusMenuRows[i]);
            }
            minusMenuAnchor[0].ID = "RemoveCell";
            minusMenuAnchor[0].InnerText = base._mapper.get("matrix", "removeCell");
            minusMenuCells[1].ID = "RemoveKnowledge";
            minusMenuAnchor[1].InnerText = base._mapper.get("matrix", "removeKnowledge");
            minusMenuCells[2].ID = "RemoveSubmatrix";
            minusMenuAnchor[2].InnerText = base._mapper.get("matrix", "removeSubmatrix");
            minusMenuCells[3].ID = "RemoveRow";
            minusMenuAnchor[3].InnerText = base._mapper.get("matrix", "removeRow");
            minusMenuCells[4].ID = "SeparateCell";
            minusMenuAnchor[4].InnerText = base._mapper.get("matrix", "separateSokrateselement");
            minusMenuCells[5].ID = "RemoveTitleColumn2";
            minusMenuAnchor[5].InnerText = base._mapper.get("matrix", "removeTitleColumn2");
            if (Global.isModuleEnabled("SBS") && !_db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", _db.userId, true, true))
            {
                minusMenuAnchor[0].Visible = false;
                minusMenuAnchor[1].Visible = false;
                minusMenuAnchor[2].Visible = false;
                minusMenuAnchor[3].Visible = false;
                minusMenuAnchor[4].Visible = false;
                minusMenuAnchor[5].Visible = false;
            }
            // load gfk javascript and andjust the menue
            if (Global.isModuleEnabled("gfk"))
            {
                // load javascript
                if (_db.lookup("IS_GFK_TEMPLATE", "MATRIX", "ID = " + MatrixID).ToString() == "True")
                {
                    Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='../GFK/gfkFormulaManager.js'></script>"));
                    for (int i = 0; i < 2; i++)
                    {
                        HtmlTableCell additionalOption = new HtmlTableCell();
                        if (i == 0)
                        {
                            additionalOption.InnerText = "Formel für Zeile Hinzufügen";
                            additionalOption.ID = "FormelButton";
                        }
                        else
                        {
                            additionalOption.InnerText = "Formel für Zelle Hinzufügen";
                            additionalOption.ID = "FormelButtonCell";
                        }
                        HtmlTableRow additionalRow = new HtmlTableRow();
                        additionalRow.Controls.Add(additionalOption);
                        plusMenu.Controls.Add(additionalRow);
                    }
                }
            }
            // load novis javascript and andjust the menue
            if (Global.isModuleEnabled("novis"))
            {
                if (tblTitel.Rows[0]["IS_NOVIS_TEMPLATE"].ToString().Equals("True"))
                {
                    //add javascript for template
                    this.Controls.Add(new LiteralControl("<script type='text/javascript' src='../Novis/templateEdit.js'></script>"));
                    minusMenu.Controls.RemoveAt(1);
                    plusMenu.Controls.RemoveAt(3);
                    plusMenu.Controls.RemoveAt(2);
                }
                else
                {
                    //add javascript for derived map
                    this.Controls.Add(new LiteralControl("<script type='text/javascript' src='../Novis/derivativeEdit.js'></script>"));
                    minusMenu.Controls.RemoveAt(5);
                    minusMenu.Controls.RemoveAt(4);
                    minusMenu.Controls.RemoveAt(3);
                    minusMenu.Controls.RemoveAt(2);
                    minusMenu.Controls.RemoveAt(0);
                    plusMenu.Controls.RemoveAt(9);
                    plusMenu.Controls.RemoveAt(8);
                    plusMenu.Controls.RemoveAt(7);
                    plusMenu.Controls.RemoveAt(6);
                    plusMenu.Controls.RemoveAt(5);
                    plusMenu.Controls.RemoveAt(4);
                    plusMenu.Controls.RemoveAt(1);
                    plusMenu.Controls.RemoveAt(0);
                }
            }


        }

        private Color calculateColorFirstColumn(Color color)
        {
            //color to be mixed in and amount
            //if these values are changed they must also be changed in the javascript on the client side calculation (colormenu.js calculateColorFirstColumn())
            Color backgroundColor = Color.FromArgb(-2960685);
            float transparency = 0.5f;

            byte r = (byte)((backgroundColor.R * transparency) + color.R * (1 - transparency));
            byte g = (byte)((backgroundColor.G * transparency) + color.G * (1 - transparency));
            byte b = (byte)((backgroundColor.B * transparency) + color.B * (1 - transparency));
            return Color.FromArgb(r, g, b);
        }


        public class CellTable // tabelle für mainCell
        {
            public HtmlTable CellTbl = new HtmlTable();
            private HtmlTableRow TextRow = new HtmlTableRow();
            private HtmlTableRow SupportRow = new HtmlTableRow();
            public HtmlTableCell TextCell = new HtmlTableCell();
            public HtmlTableCell SupportCellR = new HtmlTableCell();
            public HtmlTableCell SupportCellB = new HtmlTableCell();
            public HtmlTableCell SupportCell = new HtmlTableCell();
            public CellTable(string cellText, string colorID, int row, int col) // create main cell inner table
            {
                TextCell.InnerHtml = cellText;
                SupportRow.Cells.Add(SupportCellB);
                SupportRow.Cells.Add(SupportCell);
                SupportRow.Attributes["Class"] = "SupportRow";
                TextRow.Attributes["Class"] = "TextRow";
                SupportCellR.Attributes["Class"] = "SupportColumn SupportColumnAbove Support" + colorID;
                SupportCell.Attributes["Class"] = "SupportColumn SupportColumnBelow Support" + colorID;
                SupportCellB.Attributes["Class"] = "TextColumn TextColumnBelow Support" + colorID;
                TextCell.Attributes["Class"] = "TextColumn TextColumnAbove ";
                CellTbl.Attributes["Class"] = "CellTable SupportColor" + colorID;
                TextCell.Attributes["Class"] += " Color" + colorID;
                TextCell.ID = "TextCell_" + row.ToString() + "_" + col.ToString();
                TextRow.Cells.Add(TextCell);
                TextRow.Cells.Add(SupportCellR);
                CellTbl.Rows.Add(TextRow);
                CellTbl.Rows.Add(SupportRow);
            }
        }
        private KnowledgeEditSimple getKnowledgeWindowEdit()
        {
            KnowledgeEditSimple knowledgeEdit = (KnowledgeEditSimple)LoadControl("../../Knowledge/Controls/KnowledgeEditSimpleCtrl.ascx");
            return knowledgeEdit;
        }
        private KnowledgeViewSimple getKnowledgeWindowView()
        {
            KnowledgeViewSimple knowledgeView = (KnowledgeViewSimple)LoadControl("../../Knowledge/Controls/KnowledgeViewSimpleCtrl.ascx");
            return knowledgeView;
        }
        protected void createBackgroundImg(DataTable tblColor)
        {
            Bitmap bitmap = new Bitmap(50, 50);
            Graphics graphics = Graphics.FromImage(bitmap);
            Color cellColor;
            string imgName;
            for (int i = 0; i < tblColor.Rows.Count + 2; i++)
            {
                if (i < tblColor.Rows.Count)
                {
                    cellColor = Color.FromArgb(int.Parse(tblColor.Rows[i]["COLOR"].ToString()));
                    imgName = tblColor.Rows[i]["ORDNUMBER"].ToString();
                }
                else if (i == tblColor.Rows.Count)
                {
                    cellColor = Color.FromArgb(255, 217, 217, 217);
                    imgName = "Default";
                }
                else
                {
                    cellColor = Color.LightBlue;
                    imgName = "FirstColumn";
                }
                //LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 50, 50), cellColor, Color.White, LinearGradientMode.Vertical);
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 50, 50), cellColor, cellColor, LinearGradientMode.Vertical);
                brush.SetSigmaBellShape(0.2f, 0.8f);
                graphics.FillRectangle(brush, new Rectangle(0, 0, 50, 50));
                //bitmap.Save(Global.Config.getModuleParam("morph", "morphImagePath", "").ToString() + "\\Color" + imgName + ".png", ImageFormat.Png);
            }
        }
        protected static DataTable getMatrix(long MatrixId, DBData db)
        {
            //get matrix from db and write it to datatable
            //execute stored procedure to write it in single table
            long userId = db.userId;
            db.execute("EXEC GET_Matrix " + MatrixId + ", " + userId + "0");
            DataTable matrix = db.getDataTable("SELECT * FROM Matrix_" + userId + " ORDER BY DimensionOrdNumber, LINE");
            return matrix;
        }
        protected static DataTable getMatrix(long MatrixId, DBData db, Boolean includeSubmatrix)
        {
            //get matrix from db and write it to datatable
            //execute stored procedure to write it in single table
            long userId = db.userId;
            db.execute("EXEC GET_Matrix " + MatrixId + ", " + userId + ", " + (includeSubmatrix ? 1 : 0));
            DataTable matrix = db.getDataTable("SELECT * FROM Matrix_" + userId + " WHERE DimensionId IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + MatrixId + ") ORDER BY DimensionOrdNumber, LINE");
            if (includeSubmatrix)
            {
                matrix.Merge(db.getDataTable("SELECT * FROM Matrix_" + userId + " WHERE DimensionId NOT IN (SELECT ID FROM DIMENSION WHERE MATRIX_ID =" + MatrixId + ") ORDER BY DimensionOrdNumber, LINE"));
            }
            return matrix;
        }
    }
}
