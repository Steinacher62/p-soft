using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using Telerik.Web.Spreadsheet;
using Telerik.Windows.Documents.Core;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.Pdf;
using Telerik.Windows.Zip;




namespace ch.appl.psoft.SBS.Controls
{
    public partial class AddEditUserDetail : PSOFTMapperUserControl
    {
        public bool showImportErrorWindow = false;
        public static string Path
        {
            get { return Global.Config.baseURL + "/SBS/Controls/AddEditUserDetail.ascx"; }
        }


        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Session.Add("PERSON_ID", 0);
                ClientDataSource.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/Test.asmx/";
                ClientDataSourceSeminars.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/Test.asmx/";
                ClientDataSourceMatrix.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/Test.asmx/";
                ClientDataSourceMatrixSeminar.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/Test.asmx/";

                CultureInfo newCulture = CultureInfo.CreateSpecificCulture("de-DE");
                GridUser.Culture = newCulture;
                GridUser.ClientSettings.Scrolling.AllowScroll = true;
                GridUser.ClientSettings.Scrolling.UseStaticHeaders = true;
                GridUser.ClientSettings.Scrolling.FrozenColumnsCount = 2;

                GridUser.CommandItemStyle.CssClass = "FileExplorerToolbar";
                GridSeminars.CommandItemStyle.CssClass = "FileExplorerToolbar";
                GridSeminars.ClientSettings.Scrolling.UseStaticHeaders = true;
                GridMatrixSeminar.CommandItemStyle.CssClass = "FileExplorerToolbar";
                GridMatrix.ClientSettings.Scrolling.UseStaticHeaders = true;
                GridMatrix.CommandItemStyle.CssClass = "FileExplorerToolbar";
            }

            if (Request.Browser.MSDomVersion.Major == 0) // Non IE Browser?
            {
                Response.Cache.SetNoStore(); // No client side cashing for non IE browsers
            }
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable seminars = db.getDataTable("SELECT ID, NAME FROM SBS_SEMINARS ORDER BY NAME");
            ddSeminars.DataValueField = "ID";
            ddSeminars.DataTextField = "NAME";
            ddSeminars.DataSource = seminars;
            ddSeminars.DataBind();
            Session.Add("SelectedSeminar", seminars.Rows[0]["ID"].ToString());
            db.disconnect();



        }



        protected void AsyncUpload1_FileUploaded(object sender, Telerik.Web.UI.FileUploadedEventArgs e)
        {
            string path = Server.MapPath("~/reports/");
            e.File.SaveAs(path + e.File.GetName());

            Workbook wb = new Workbook();

            wb = Workbook.Load(path + e.File.GetName());

            int length = wb.Sheets.Count;
            int indexOfSheet = 0;
            for (int i = 0; i < length; i++)
            {
                if (wb.Sheets[i].Name.Equals("User"))
                {
                    indexOfSheet = i;
                    break;
                }

            }

            Worksheet ws = wb.Sheets[indexOfSheet];


            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable errorTable = new DataTable();
            DataColumn ID = new DataColumn("Id");
            DataColumn TITLE = new DataColumn("Titel");
            DataColumn FIRSTNAME = new DataColumn("Vorname");
            DataColumn PNAME = new DataColumn("Name");
            DataColumn FUNKTION = new DataColumn("Funktion");
            DataColumn EMAIL = new DataColumn("E-Mail");
            DataColumn PASSWORD = new DataColumn("Passwort");
            DataColumn ERROR = new DataColumn("Fehler");
            errorTable.Columns.Add(ID);
            errorTable.Columns.Add(TITLE);
            errorTable.Columns.Add(FIRSTNAME);
            errorTable.Columns.Add(PNAME);
            errorTable.Columns.Add(FUNKTION);
            errorTable.Columns.Add(EMAIL);
            errorTable.Columns.Add(PASSWORD);
            errorTable.Columns.Add(ERROR);

            string id = "";
            string title = "";
            string firstname = "";
            string name = "";
            string funktion;
            string email = "";
            string password = "";
            string error = "";

            long seminarID = Convert.ToInt32(ddSeminars.SelectedValue);
            try
            {

                String strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=" + path + e.File.GetName() + "; " +
                "Extended Properties=Excel 12.0 Xml";

                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + ws.Name + "$]", strConn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dd = ds.Tables[0];


                for (int i = 0; i < dd.Rows.Count; i++)
                {
                    try
                    {

                        bool existUser = false;

                        id = dd.Rows[i][0].ToString().Trim().Replace("'", "''");
                        if (id.Length > 0)
                        {
                            title = dd.Rows[i][1].ToString().Trim().Replace("'", "''");
                            firstname = dd.Rows[i][2].ToString().Trim().Replace("'", "''");
                            if (firstname.Length == 0)
                            {
                                error = "Vorname fehlt";
                                throw new Exception();
                            }
                            name = dd.Rows[i][3].ToString().Trim().Replace("'", "''");
                            if (name.Length == 0)
                            {
                                error = "Name fehlt";
                                throw new Exception();
                            }
                            funktion = dd.Rows[i][4].ToString().Trim().Replace("'", "''");
                            email = dd.Rows[i][5].ToString().Trim().Replace("'", "''");
                            if (!Regex.IsMatch(email, @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                                                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"))
                            {
                                error = "Ungültige E-Mailadresse";
                                throw new Exception();
                            }
                            password = dd.Rows[i][6].ToString().Trim().Replace("'", "''");
                            if (password.Length == 0)
                            {
                                error = "Passwort fehlt";
                                throw new Exception();
                            }

                            if (Convert.ToInt16(db.lookup("count(EMAIL)", "PErSON", "EMAIL='" + email + "'")) > 0)
                            {
                                existUser = true;
                            }

                            if (existUser)
                            {
                                db.execute("UPDATE PERSON SET TITLE = '" + title + "', FIRSTNAME = '" + firstname + "', PNAME='" + name + "', EMAIL= '" + email + "', LOGIN= '" + email + "', PASSWORD='" + password + "' WHERE EMAIL = '" + email + "'");
                                long userId = Convert.ToInt32(db.lookup("ID", "PERSON", "EMAIL= '" + email + "'"));
                                if (Convert.ToInt16(db.lookup("count(id)", "SBS_USER_SEMINARS", "USER_REF = " + userId + " AND SEMINAR_REF = " + seminarID)) == 0)
                                {
                                    db.execute("INSERT INTO SBS_USER_SEMINARS (USER_REF, SEMINAR_REF) VALUES (" + userId + "," + seminarID + ")");
                                }
                            }
                            else
                            {
                                db.execute("INSERT INTO PERSON (TITLE, FIRSTNAME, PNAME, EMAIL, LOGIN, PASSWORD) VALUES('" + title + "', '" + firstname + "', '" + name + "', '" + email + "', '" + email + "', '" + password + "')");
                                long userId = Convert.ToInt32(db.lookup("max(ID)", "PERSON", ""));
                                db.execute("INSERT INTO ACCESSOR(id, visible, tablename, row_id) values( " + userId + ", 0, 'PERSON', " + userId + ")");
                                db.execute("INSERT INTO ACCESSOR_GROUP_ASSIGNMENT(ACCESSOR_MEMBER_ID, ACCESSOR_GROUP_ID) values(" + userId + ", 1)");
                                db.execute("INSERT INTO SBS_USER_SEMINARS (USER_REF, SEMINAR_REF) VALUES (" + userId + "," + seminarID + ")");
                            }
                        }
                    }

                    catch (Exception exeption)
                    {
                        if (error.Length == 0)
                        {
                            error = exeption.Message;
                        }
                        DataRow newRow = errorTable.NewRow();
                        newRow["Id"] = id;
                        newRow["Titel"] = title;
                        newRow["Vorname"] = firstname;
                        newRow["Name"] = name;
                        newRow["E-Mail"] = email;
                        newRow["Passwort"] = password;
                        newRow["Fehler"] = error;
                        errorTable.Rows.Add(newRow);
                        error = "";
                    }
                }

                ErrorTable.DataSource = errorTable;
                ErrorTable.DataBind();
                db.disconnect();
                showImportErrorWindow = true;
            }


            catch
            {
                errorMsg.Style["Display"] = "Visible";
                return;
            }

        }

        public void btnDummy_Click(object sender, System.EventArgs e)
        {
            ImportError.Visible = true;
        }
    }
}