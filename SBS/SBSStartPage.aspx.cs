using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.SBS
{
    public partial class SBSStartPage : PsoftDetailPage
    {
        protected DataTable srcTable;
        public long seminarID;
        private DataRow seminarRow;
        private DataRow[] modules;
        private DataRow[][] folders;
        private DataTable documents;
        private DataTable subFolders;
        private DataTable[] subDocuments;
        private const int minNrOfRows = 3;
        private const int minNrOfCols = 6;
        private Boolean isAdmin = false;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            /// Setting breadcrumb caption
            BreadcrumbCaption = _mapper.get(SbsModule.LANG_SCOPE_SBS, SbsModule.LANG_MNEMO_STARTPAGE);
            if (Request.QueryString["seminarID"] != null)
            {
                Session.Add("seminarID", Request.QueryString["seminarID"]);
            }

            if(Session["seminarID"] != null)
            {
                seminarID = Convert.ToInt32(Session["seminarID"]);
            }
            buildPage();
           

        }

        public void buildPage()
        {
            getData(seminarID);

            if (srcTable.Rows.Count < 1)
            {
                // throw new Exception("no Data Found!");
                return;
            }
            HtmlTable table = addHtmlTable();
            startPageMenue.Controls.Add(table);

            HtmlGenericControl[] descList = getDescriptions();
            foreach (HtmlGenericControl div in descList)
            {
                if (div != null)
                {
                    startPageMenue.Controls.Add(div);
                }
            }
        }

        public void buildPage(object startPageControl)
        {
            PlaceHolder startPageMenue = (PlaceHolder)startPageControl;
            getData(seminarID);

            if (srcTable.Rows.Count < 1)
            {
                // throw new Exception("no Data Found!");
                return;
            }
            HtmlTable table = addHtmlTable();
            startPageMenue.Controls.Add(table);

            HtmlGenericControl[] descList = getDescriptions();
            foreach (HtmlGenericControl div in descList)
            {
                if (div != null)
                {
                    startPageMenue.Controls.Add(div);
                }
            }
        }

        private void getData(long seminarID)
        {
            // get Data
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();

            if (seminarID == 0)
            {
                seminarID = ((long)db.lookup("Max(SEMINAR_REF)", "SBS_USER_SEMINARS", "USER_REF=" + db.userId, -1L));
            }

            int numberOfModules = (int)db.lookup("count(ID)", "SBS_FOLDERS", "PARENT_ID = " + seminarID + "AND SEMINAR_ID = " + seminarID);

            srcTable = db.getDataTable("SELECT * FROM SBSSeminarData WHERE SEMINAR_ID = " + seminarID + " OR (ID =" + seminarID + " AND SEMINAR_ID = 0) ORDER BY PARENT_ID, NAME");

            if (srcTable.Rows.Count < 1)
            {
                // seminar not found
                return;
            }

            long groupAccessorId = DBColumn.GetValid(db.lookup("ID", "ACCESSOR", "TITLE = 'Administratoren'"),(long)-1);

            isAdmin = db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true);
            

            seminarRow = srcTable.Select("ID = " + seminarID)[0];
            modules = srcTable.Select("PARENT_ID = " + seminarID);

            // reverse order of modules
            Array.Reverse(modules);

            //fill up to min number of columns
            if (modules.Length < minNrOfCols)
            {
                DataRow[] temp = new DataRow[6];
                Array.Copy(modules,temp,modules.Length);
                modules = temp;
                numberOfModules = modules.Length;
            }

            //get folders for each module
            folders = new DataRow[numberOfModules][];
            for (int i = 0; i < numberOfModules; i++)
            {
                folders[i] = modules[i]!= null ? srcTable.Select("PARENT_ID = " + modules[i]["ID"]) : new DataRow[0];
            }

            String folderIds = "";
            foreach(DataRow[] folderlist in folders){
                foreach (DataRow row in folderlist)
                {
                    if (folderIds.Equals(""))
                    {
                        folderIds = "( " + row["ID"].ToString();
                    }
                    else
                    {
                        folderIds += " , " + row["ID"].ToString();
                    }
                }
        }
            if (folderIds.Equals(""))
            {
                documents = new DataTable();
                subFolders = new DataTable();
                subDocuments = new DataTable[subFolders.Rows.Count];
                return;
            }
            folderIds+=")";

            documents = db.getDataTable("SELECT * FROM SBSSeminarData WHERE FOLDER_ID IN "+folderIds +" AND DOCUMENT_ID != 0 ORDER BY NAME");
            subFolders = db.getDataTable("SELECT * FROM SBSSeminarData WHERE PARENT_ID IN " + folderIds + " AND DOCUMENT_ID = 0 ORDER BY NAME");

            subDocuments = new DataTable[subFolders.Rows.Count];
            for (int i = 0; i < subFolders.Rows.Count; i++ )
            {
                subDocuments[i] = db.getDataTable("SELECT * FROM SBSSeminarData WHERE FOLDER_ID = " + subFolders.Rows[i]["ID"] + " AND DOCUMENT_ID != 0 ORDER BY NAME");
            }
        }

        private HtmlTable addHtmlTable()
        {
            HtmlTable table = new HtmlTable();
            table.Attributes["Class"] = "SeminarTable";

            //title
            HtmlTableRow titleRow = new HtmlTableRow();
            HtmlTableCell titleCell = new HtmlTableCell();

            titleCell.InnerText = seminarRow["TITLE"].ToString();
            titleCell.ColSpan = modules.Length;
            titleCell.Attributes["Class"] = "TitleCell";
            titleRow.Attributes["Class"] = "TitleRow";
            titleRow.Cells.Add(titleCell);
            table.Rows.Add(titleRow);

            HtmlTableRow titleRow2 = new HtmlTableRow();
            HtmlTableCell titleCell2 = new HtmlTableCell();

            titleCell2.InnerText = seminarRow["DESCRIPTION"].ToString();
            titleCell2.ColSpan = modules.Length;
            titleCell2.Attributes["Class"] = "TitleCellDescription";
            titleRow2.Attributes["Class"] = "TitleRow";
            titleRow2.Cells.Add(titleCell2);
            table.Rows.Add(titleRow2);

            //modules + Date
            int nrOfColumns = modules.Length;

            HtmlTableRow dateRow = new HtmlTableRow();
            HtmlTableRow moduleRow = new HtmlTableRow();


            for (int i = 0; i < nrOfColumns; i++)
            {
                HtmlTableCell dateCell = new HtmlTableCell();
                dateCell.InnerText = modules[i] != null ? modules[i]["DATUM"].ToString() : "";
                dateCell.Attributes["Class"] = "DateCell";
                dateRow.Cells.Add(dateCell);
                dateRow.Attributes["Class"] = "DateRow";

                HtmlTableCell moduleCell = new HtmlTableCell();
                moduleCell.InnerText = modules[i] != null ? modules[i]["TITLE"].ToString() : "";
                moduleCell.Attributes["Class"] = "ModuleCell";
                moduleCell.Attributes["Class"] += modules[i] != null && !modules[i]["DESCRIPTION"].ToString().Equals("") ? " ClickableDescription" : "";
                moduleCell.Attributes["id"] = modules[i] != null ? "ModuleCell" + modules[i]["ID"].ToString() : "";
                moduleRow.Cells.Add(moduleCell);
                moduleRow.Attributes["Class"] = "ModuleRow";
            }

            table.Rows.Add(dateRow);
            table.Rows.Add(moduleRow);

            //Folders
            int nrOfRows = 0;

            for (int i = 0; i < folders.Length; i++)
            {
                if (nrOfRows < folders[i].Length)
                {
                    nrOfRows = folders[i].Length;
                }
            }

            nrOfRows = Math.Max(minNrOfRows, nrOfRows);
            HtmlTableRow[] FolderRows = new HtmlTableRow[nrOfRows];
            for (int i = 0; i < nrOfRows; i++)
            {
                FolderRows[i] = new HtmlTableRow();
                for (int j = 0; j < nrOfColumns; j++)
                {
                    HtmlTableCell FolderCell = new HtmlTableCell();
                    if (folders[j].Length > i)
                    {
                        FolderCell.InnerText = folders[j][i]["TITLE"].ToString();
                        FolderCell.Attributes["id"] = "FolderCell" + folders[j][i]["ID"].ToString();
                        DateTime release = DateTime.Now;
                        String folderId = srcTable.Select("ID =" + folders[j][i]["ID"].ToString() + " AND PARENT_ID <> 0 AND PARENT_ID <> SEMINAR_ID AND DOCUMENT_ID = 0")[0]["PARENT_ID"].ToString();                   
                        DataRow[] rows = srcTable.Select("ID =" + folderId + " AND PARENT_ID <> 0 AND PARENT_ID = SEMINAR_ID AND DOCUMENT_ID = 0");
                        if (isAdmin || (!DateTime.TryParse(rows[0]["RELEASE"].ToString(), out release)) || release < DateTime.Now)
                        {
                            FolderCell.Attributes["Class"] = "ClickableFolder" + (!folders[j][i]["DESCRIPTION"].ToString().Equals("") ? " ClickableDescription" : "");
                        }
                    }
                    FolderCell.Attributes["Class"] += " FolderCell";
                    FolderRows[i].Cells.Add(FolderCell);
                }
                FolderRows[i].Attributes["Class"] = "FolderRow";
                table.Rows.Add(FolderRows[i]);
            }

            HtmlTableRow documentRow = new HtmlTableRow();
            HtmlTableCell documentCell = new HtmlTableCell();

            documentRow.Attributes["Class"] = "DocumentRow";

            HtmlGenericControl orderedList = new HtmlGenericControl("ol");

            orderedList.Attributes["Class"] = "DocumentsList";

            for(int i = 0; i<subFolders.Rows.Count; i++){
                HtmlGenericControl listEntry = new HtmlGenericControl("li");
                HtmlImage info = new HtmlImage();
                info.Attributes["Class"] += " ClickableDescription";
                info.Attributes["id"] = "DocumentCell" + subFolders.Rows[i]["ID"].ToString();
                info.Src = "../images/folder.gif";
                listEntry.Controls.Add(info);

                HtmlAnchor link = new HtmlAnchor();
                link.InnerText = subFolders.Rows[i]["TITLE"].ToString();
                link.Attributes["href"] = "javascript:void(0);";
                link.Attributes["Class"] = "SubFolder";
                link.Attributes["id"] = "subFolder_" + subFolders.Rows[i]["ID"].ToString();

                listEntry.Attributes["Class"] = "DocumentListEntry";
                listEntry.Attributes["Class"] += " HiddenDocument";
                
                listEntry.Attributes["Class"] += " FolderEntry" + subFolders.Rows[i]["PARENT_ID"].ToString();

                listEntry.Controls.Add(link);

                HtmlGenericControl subList = new HtmlGenericControl("ul");
                subList.Attributes["id"] = "subDocs_" + subFolders.Rows[i]["ID"].ToString();

                subList.Attributes["Class"] = "DocumentListEntry";
                subList.Attributes["Class"] += " HiddenDocument";
                subList.Attributes["Class"] += " SubDocs"; 
                subList.Attributes["Class"] += " FolderEntry" + subFolders.Rows[i]["PARENT_ID"].ToString();

                foreach (DataRow row in subDocuments[i].Rows)
                {
                    HtmlGenericControl subListEntry = new HtmlGenericControl("li");
                    HtmlImage subInfo = new HtmlImage();
                    subInfo.Attributes["Class"] += " ClickableDescription";
                    subInfo.Attributes["id"] = "DocumentCell" + row["ID"].ToString();
                    subInfo.Src = "../images/icon_info.png";
                    subListEntry.Controls.Add(subInfo);

                    HtmlAnchor subLink = new HtmlAnchor();
                    subLink.InnerText = row["TITLE"].ToString();
                   
                    DateTime release = DateTime.Now;
                String folderId = srcTable.Select("ID =" + row["FOLDER_ID"].ToString() + " AND PARENT_ID <> 0 AND PARENT_ID <> SEMINAR_ID AND DOCUMENT_ID = 0")[0]["PARENT_ID"].ToString();
                     folderId = srcTable.Select("ID =" + folderId + " AND PARENT_ID <> 0 AND PARENT_ID <> SEMINAR_ID AND DOCUMENT_ID = 0")[0]["PARENT_ID"].ToString();
                DataRow[] rows = srcTable.Select("ID =" +folderId + " AND PARENT_ID <> 0 AND PARENT_ID = SEMINAR_ID AND DOCUMENT_ID = 0");
                if (isAdmin || (!DateTime.TryParse(rows[0]["RELEASE"].ToString(), out release)) || release < DateTime.Now)
                {
                    if (row["NAME"].ToString().EndsWith(".pdf"))
                    {
                        String pdfLink = "Flowpaper/PdfViewer.aspx?pdfPath=" + ".." + (Global.Config.getModuleParam("SBS", "SeminarsURl", "") + row["URL"].ToString()).Replace("\\", "/");
                        subLink.Attributes["href"] = pdfLink;
                    }
                    else
                    {
                        subLink.Attributes["href"] = "HtmlView.html?pdfPath=" + ".." + (Global.Config.getModuleParam("SBS", "SeminarsURl", "") + row["URL"].ToString()).Replace("\\", "/");
                    }
                }
                    subListEntry.Attributes["id"] = "subFolder_" + row["ID"].ToString();

                    subListEntry.Attributes["Class"] = "DocumentsubListEntry";
                    subListEntry.Attributes["Class"] += " FolderEntry" + subFolders.Rows[i]["PARENT_ID"].ToString();

                    subListEntry.Controls.Add(subLink);
                    subList.Controls.Add(subListEntry);
                }

                orderedList.Controls.Add(listEntry);
                orderedList.Controls.Add(subList);
            }

            foreach (DataRow row in documents.Rows)
            {
                HtmlGenericControl listEntry = new HtmlGenericControl("li");
                HtmlImage info = new HtmlImage();
                info.Attributes["Class"] += " ClickableDescription";
                info.Attributes["id"] = "DocumentCell" + row["ID"].ToString();
                info.Src = "../images/icon_info.png";
                listEntry.Controls.Add(info);

                HtmlAnchor link = new HtmlAnchor();
                link.InnerText = row["TITLE"].ToString();

                DateTime release = DateTime.Now;
                String folderId = srcTable.Select("ID =" + row["FOLDER_ID"].ToString() + " AND PARENT_ID <> 0 AND PARENT_ID <> SEMINAR_ID AND DOCUMENT_ID = 0")[0]["PARENT_ID"].ToString();
                DataRow[] rows = srcTable.Select("ID =" +folderId + " AND PARENT_ID <> 0 AND PARENT_ID = SEMINAR_ID AND DOCUMENT_ID = 0");
                if (isAdmin || (!DateTime.TryParse(rows[0]["RELEASE"].ToString(), out release)) || release < DateTime.Now)
                {
                    if (row["NAME"].ToString().EndsWith(".pdf"))
                    {
                        String pdfLink = "Flowpaper/PdfViewer.aspx?pdfPath=" + ".." + (Global.Config.getModuleParam("SBS", "SeminarsURl", "") + row["URL"].ToString()).Replace("\\", "/");
                        link.Attributes["href"] = pdfLink;
                    }
                    else
                    {
                        link.Attributes["href"] = "HtmlView.html?pdfPath=" + ".." + (Global.Config.getModuleParam("SBS", "SeminarsURl", "") + row["URL"].ToString()).Replace("\\", "/");
                    }
                }
                listEntry.Attributes["Class"] = "DocumentListEntry";
                listEntry.Attributes["Class"] += " HiddenDocument";

                listEntry.Attributes["Class"] += " FolderEntry" + row["FOLDER_ID"].ToString();

                listEntry.Controls.Add(link);
                orderedList.Controls.Add(listEntry);
            }

            documentCell.ColSpan = nrOfColumns;
            documentCell.Controls.Add(orderedList);

            documentRow.Cells.Add(documentCell);
            table.Rows.Add(documentRow);


            //copyright
            HtmlTableRow copyrightRow = new HtmlTableRow();
            HtmlTableCell copyrightCell = new HtmlTableCell();

            copyrightCell.Attributes["Class"] = "CopyrightCell";
            copyrightCell.InnerText = "© 2016 Swiss Board School St.-Gallen";
            copyrightCell.ColSpan = nrOfColumns;

            copyrightRow.Controls.Add(copyrightCell);
            table.Controls.Add(copyrightRow);

            return table;

        }

        private HtmlGenericControl[] getDescriptions()
        {
            int nrOfFolders = 0;
            foreach (DataRow[] rows in folders)
            {
                nrOfFolders += rows.GetLength(0);
            }
            foreach (DataTable tbl in subDocuments)
            {
                nrOfFolders += tbl.Rows.Count+1;
            }
            int numberOfDesc = modules.Length + nrOfFolders ;
            int numberOfDocs = documents.Rows.Count;
            HtmlGenericControl[] descList = new HtmlGenericControl[numberOfDesc + numberOfDocs];

            int i = 0;
            for (; i < modules.Length; ++i)
            {
                if (modules[i] != null)
                {
                    descList[i] = new HtmlGenericControl("div");
                    descList[i].InnerText = modules[i]["DESCRIPTION"].ToString();
                    descList[i].Attributes["Class"] = "Description";
                    descList[i].Attributes["id"] = "descriptionModuleCell" + modules[i]["ID"].ToString();
                }
            }
            i--;
            foreach (DataRow[] rows in folders)
            {
                foreach (DataRow row in rows)
                {
                    i++;
                    if (row != null)
                    {
                        descList[i] = new HtmlGenericControl("div");
                        descList[i].InnerText = row["DESCRIPTION"].ToString();
                        descList[i].Attributes["Class"] = "Description";
                        descList[i].Attributes["id"] = "descriptionFolderCell" + row["ID"].ToString();
                    }
                }
            }

            foreach (DataRow row in subFolders.Rows)
            {
                    i++;
                    if (row != null)
                    {
                        descList[i] = new HtmlGenericControl("div");
                        descList[i].InnerText = row["DESCRIPTION"].ToString();
                        descList[i].Attributes["Class"] = "Description";
                        descList[i].Attributes["id"] = "descriptionDocumentCell" + row["ID"].ToString();
                    }
                
            }



            foreach (DataRow row in documents.Rows)
            {
                i++;
                descList[i] = new HtmlGenericControl("div");
                String text = !row["AUTHOR"].ToString().Equals("") ? "Author: " + row["AUTHOR"].ToString() + "<br>" : "";
                text += !row["DOZENT"].ToString().Equals("") ? "Dozent: " + row["DOZENT"].ToString() + "<br>" : "";
                text += row["DESCRIPTION"].ToString();
                descList[i].InnerHtml = text;
                descList[i].Attributes["Class"] = "Description";
                descList[i].Attributes["id"] = "descriptionDocumentCell" + row["ID"].ToString();
            }

            foreach (DataTable tbl in subDocuments)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    i++;
                    descList[i] = new HtmlGenericControl("div");
                    String text = !row["AUTHOR"].ToString().Equals("") ? "Author: " + row["AUTHOR"].ToString() + "<br>" : "";
                    text += !row["DOZENT"].ToString().Equals("") ? "Dozent: " + row["DOZENT"].ToString() + "<br>" : "";
                    text += row["DESCRIPTION"].ToString();
                    descList[i].InnerHtml = text;
                    descList[i].Attributes["Class"] = "Description";
                    descList[i].Attributes["id"] = "descriptionDocumentCell" + row["ID"].ToString();
                }
            }
            return descList;
        }
    }
}