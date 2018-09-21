using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;

namespace ch.appl.psoft.Project.Controls
{

    public partial class BillingListCtrl : PSOFTListViewUserControl
    {
        private long _projectID = -1;
        private long _projectBillingID = -1;

        private string _postDeleteURL;

        protected DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;


        public static string Path
        {
            get { return Global.Config.baseURL + "/Project/Controls/BillingListCtrl.ascx"; }
        }

        public BillingListCtrl()
            : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            DetailEnabled = true;
            EditEnabled = true;

            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

        #region Properties
        public long ProjectID
        {
            get { return _projectID; }
            set { _projectID = value; }
        }

        public long ProjectBillingID
        {
            get { return _projectBillingID; }
            set { _projectBillingID = value; }
        }

        public string PostDeleteURL
        {
            get { return _postDeleteURL; }
            set { _postDeleteURL = value; }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            this.apply.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BT_PRINT_BILLING_REPORT);
            this.subprojects.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CB_PRINT_SUBPROJECTS);

            loadList();
        }


        /// <summary>
        /// Values without VAT, balances and department names currently in the report only.
        /// </summary>
        protected void loadList()
        {
            string sql = "select * from PROJECT_BILLING where PROJECT_ID=" + this._projectID;
            _db = DBData.getDBData(Session);
            listTab.Rows.Clear();
            try
            {
                _db.connect();
                pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CT_BILLING_LIST).Replace("#1", _db.lookup("TITLE", "PROJECT", "ID=" + ProjectID, false));
                DataTable table = _db.getDataTableExt(sql, "PROJECT_BILLING");

                if (_projectBillingID > 0)
                    HighlightRecordID = _projectBillingID;


                LoadList(_db, table, listTab);
            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally
            {
                _db.disconnect();
            }
        }


        /// <summary>
        /// Print out the report to an excel sheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apply_Click(object sender, System.EventArgs e)
        {
            DataToXml dxml = DataToXml.createRoot("PROJECT", this._db, this._mapper, "project"); //root
            dxml.addNodeValue("TITLE", "projectTitle");
            dxml.addIdNode("creditBalance", creditBalance).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_CREDIT_BALANCE));
            dxml.addIdNode("creditorBalance", creditorBalance).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_CREDITOR_BALANCE));
            dxml.addIdNode("debitorBalance", debitorBalance).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_DEBITOR_BALANCE));
            dxml.addIdNode("surplus", surplus).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_SURPLUS));
            dxml.addIdNode("deficit", deficit).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_DEFICIT)); ;
            dxml.addIdNode("department", findDepartment).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_DEPARTMENT)); ;

            DataToXml billingDxml = dxml.createChildMany("PROJECT_ID", "PROJECT_BILLING", "billing", "billings");
            billingDxml.addNodeValue("DATE", "date").convertToShortDate();
            billingDxml.addNodeValue("BILL_NUMBER", "billNumber");
            billingDxml.addNodeValue("DESCRIPTION", "description");
            billingDxml.addNodeValue("CREDIT_VALUE", "credit");
            billingDxml.addNodeValue("CREDIT_VALUE", "creditExclusiveVat").convertUsingDelegate(detractVat).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_CREDIT_EXCL));
            billingDxml.addNodeValue("CREDITOR_VALUE", "creditor");
            billingDxml.addNodeValue("CREDITOR_VALUE", "creditorExclusiveVat").convertUsingDelegate(detractVat).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_CREDITOR_EXCL));
            billingDxml.addNodeValue("DEBITOR_VALUE", "debitor");
            billingDxml.addNodeValue("DEBITOR_VALUE", "debitorExclusiveVat").convertUsingDelegate(detractVat).convertUsingDelegate(detractVat).setCustomTranslation(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_TT_DEBITOR_EXCL));
            billingDxml.addNodeValue("VAT_VALUE", "vat");

            if (this.subprojects.Checked)
            {
                //print out this project and all sub-project bills
                dxml.createChildManyRecursive("PARENT_ID", "PROJECT", "project", "subProjects");
            }

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlElement elm = dxml.extractData(doc, this._projectID);
            XmlElement translation = dxml.generateTranslationStructure(doc);
            
            XmlElement merged = DataToXml.mergeDataWithTranslation(doc, elm, translation, "costControl");
            doc.AppendChild(merged);

            string filenameAbs = XMLReport.getOutputfileAbsolutePath("costControl");
            doc.Save(filenameAbs);
            string filenameRel = XMLReport.getOutputfileRelativePath("costControl");
            Response.Redirect(filenameRel);

        }

        #region XML conversion procedures 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        private string detractVat(string value, DataToXml org)
        {
            long id = org.CurrentId;
            double vat = _db.lookup("VAT_VALUE", "PROJECT_BILLING", "ID = " + id, -1.0);
            if (vat < 0) return value;
            double result = 0;
            if (double.TryParse(value, out result))
            {
                result = result * 100 / (vat + 100.0);
                return result.ToString("0.00");
            }
            return value;
        }

        /// <summary>
        /// Fill in department data.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        private string findDepartment(string value, DataToXml org)
        {
            try
            {
                long projectId = long.Parse(value);
                string sql = "select "
                             + _db.langAttrName("REGISTRY", "TITLE")
                             + " from REGISTRY where ID in (select REGISTRY_ID from REGISTRY_ENTRY where OBJECT_UID in (select UID from project where id = " 
                             + projectId + ") )";
                DataTable data = _db.getDataTable(sql);
                if (data.Rows.Count == 0)
                {
                    return "";
                }
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (DataRow elm in data.Rows)
                {
                    sb.Append(elm[0].ToString());
                    sb.Append(", ");
                }
                if (sb.Length > 2)
                {
                    sb.Remove(sb.Length - 2, 2);
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                ch.psoft.Util.Logger.Log(e, ch.psoft.Util.Logger.ERROR);
                return "";
            }
        }


        /// <summary>
        /// Summe aller Eintraege einer Kolonne.
        /// </summary>
        /// <param name="projectIdStr"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private double sigma(string projectIdStr, string column)
        {
            try
            {
                long projectId = long.Parse(projectIdStr);
                return _db.ProjectBilling.sigma(projectId, column);
            }
            catch (Exception e)
            {
                ch.psoft.Util.Logger.Log(e, ch.psoft.Util.Logger.ERROR);
                return 0;
            }
        }

        /// <summary>
        /// Calculate the cretit balance of the current project (given by "id")
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string creditBalance(string id, DataToXml obj)
        {
            return sigma(id, "CREDIT_VALUE").ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string creditorBalance(string id, DataToXml obj)
        {
            return sigma(id, "CREDITOR_VALUE").ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string debitorBalance(string id, DataToXml obj)
        {
            return sigma(id, "DEBITOR_VALUE").ToString();
        }

        /// <summary>
        /// Value returned if (Kredit - Kreditor + Debitor) is positive.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string surplus(string id, DataToXml obj)
        {
            double ret = sigma(id, "CREDIT_VALUE") - sigma(id, "CREDITOR_VALUE") + sigma(id, "DEBITOR_VALUE");
            if (ret >= 0)
            {
                return ret.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Value returned if (Kredit - Kreditor + Debitor) is negative. 
        /// The returned value is positive.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string deficit(string id, DataToXml obj)
        {
            //same as surplus but negative
            double ret = sigma(id, "CREDIT_VALUE") - sigma(id, "CREDITOR_VALUE") + sigma(id, "DEBITOR_VALUE"); 
            if (ret < 0)
            {
                return (0-ret).ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion 


        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            base.OnInit(e);
            mapControls();
        }

        private void mapControls()
        {
            apply.Click += new System.EventHandler(apply_Click);

        }



        /// 
        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            base.onAddHeaderCell(row, col, r, cell);
        }

        ///
        protected override void onAfterAddCells(DataRow row, TableRow r)
        {
            base.onAfterAddCells(row, r);
        }


    }
}
