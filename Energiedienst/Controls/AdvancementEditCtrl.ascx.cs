namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Training;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Net.Mail;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for AdvancementEditCtrl.
    /// </summary>
    public partial class AdvancementEditCtrl : PSOFTInputViewUserControl {
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_ADVANCEMENT_ID = "PARAM_ADVANCEMENT_ID";
        public const string PARAM_TRAINING_ID = "PARAM_TRAINING_ID";



        protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;        
        private ArrayList _enablingControls = new ArrayList();
        private bool AdvancemantReadOnly = true;

        public static string Path {
            get {return Global.Config.baseURL + "/Energiedienst/Controls/AdvancementEditCtrl.ascx";}
        }

		#region Properities
        public long PersonID {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }

        public long AdvancementID {
            get {return GetLong(PARAM_ADVANCEMENT_ID);}
            set {SetParam(PARAM_ADVANCEMENT_ID, value);}
        }
        public long TrainingID 
        {
            get {return GetLong(PARAM_TRAINING_ID);}
            set {SetParam(PARAM_TRAINING_ID, value);}
        }

        private Control _treeCtrl = null;
        public Control TreeCtrl
        {
            get { return _treeCtrl;}
            set { _treeCtrl = value;}
        }
        #endregion 

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _config = Global.Config;
            base.CheckOrder = true;
            _db = DBData.getDBData(Session);
            _db.connect();

                apply.Text = _mapper.get("apply");
                advancementDetailTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_CT_ADVANCEMENT_DETAIL);
                apply.Visible = false;

                apply.Text = "Speichern";
                CBTreeFlag.Visible = false;
                CBTreeFlag.Checked = false;
                CBTreeFlag.Enabled = false;
                if (AdvancementID > 0)
                {
                    VIEWED_FLAG.Text = "Eingesehen von " + _db.lookup("PNAME", "PERSON", "ID = '" + _db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID = '" + AdvancementID + "'").ToString() + "'") + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = '" + _db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID = '" + AdvancementID + "'").ToString() + "'");
                    if (_db.lookup("RELEASE", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString().Equals("") && (!(_db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString().Equals(_db.userId.ToString()))))
                    {
                        AdvancemantReadOnly = false;
                    }
                    else
                    {
                        releaseButton.Enabled = false;
                        apply.Enabled = false;
                    }

                    if (!(_db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString().Equals(_db.userId.ToString())))
                    {
                        VIEWED_FLAG.Enabled = false;
                        if ((_db.lookup("RELEASE", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString().Equals("")))
                        {
                        releaseButton.Enabled = true;
                        apply.Enabled = true;
                        }
                    }

                    if(_db.lookup("RELEASE", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString().Equals(""))
                    {
                        VIEWED_FLAG.Enabled = false;
                    }
                }
                else
                {
                    releaseButton.Enabled = false;
                }

            
            
            try {
                if (PersonID > 0) 
                {

                    _table = _db.getDataTableExt("select * from TRAINING_ADVANCEMENT where ID=" + AdvancementID, "TRAINING_ADVANCEMENT");
                    _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(false);
                    _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = TrainingModule.getTrainingDemandTable(_db);
                    _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    
                    View = "TRAINING_ADVANCEMENT";
                    if (AdvancementID > 0) 
                    {
                        InputType = InputMaskBuilder.InputType.Edit;
                        string [] states = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true);
                        if (!Global.isModuleEnabled("energiedienst"))
                        {
                            _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                            _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(states);
                        }
                        else
                        {
                            _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        }
                    }
                    else 
                    {
                        InputType = InputMaskBuilder.InputType.Add;
                        _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }
                    if (Global.isModuleEnabled("energiedienst") && _table.Rows.Count > 0)
                    {
                        if ( _table.Rows[0]["VIEWED_FLAG"] == DBNull.Value)
                            VIEWED_FLAG.Checked = false;
                        else
                        {
                            VIEWED_FLAG.Checked = true;
                            VIEWED_FLAG.Enabled = false;
                        }
                    }
                    else
                    {
                        VIEWED_FLAG.Visible = false;
                    }

                    if ((_table.Rows.Count > 0 && _db.userId == (long)_table.Rows[0]["PERSON_ID"]) && (Global.isModuleEnabled("energiedienst")) && _table.Rows[0]["VIEWED_FLAG"]== DBNull.Value && !_db.lookup("RELEASE", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString().Equals(""))
                    {
                        VIEWED_FLAG.CheckedChanged += new System.EventHandler(viewed_Changed);
                        VIEWED_FLAG.Enabled = true;
                       // _table.Columns["VIEWED_FLAG"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                
                        
                    }
                    LoadInput(_db, _table, advancementEdit);
                    if (InputType == InputMaskBuilder.InputType.Add){
                        setInputValue(_table, advancementEdit, "RESPONSIBLE_PERSON_ID", _db.userId.ToString());
                    }

                    if (TrainingID > 0)
                    {
                        DataTable table = _db.getDataTable("select * from TRAINING where ID=" + TrainingID);
                
                        foreach (TableRow r in advancementEdit.Rows) 
                        {
                            try 
                            {
                                TableCell c = r.Cells[1];
                                Control ctrl = c.Controls[0];

                                if (ctrl is TextBox) 
                                {
                                    string name = ctrl.ID;
                                    int idx = name.IndexOf("-",6);
                                    if (idx >= 0) 
                                    {
                                        name = name.Substring(idx+1);
                                        string textValue = _db.dbColumn.GetDisplayValue(table.Columns[name],table.Rows[0][name],false);
                                        ((TextBox) ctrl).Text = textValue;
                                    }
                                }
                            }
                            catch {}
                        }

                        
                    }
                    apply.Visible = true;
                }
                
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) 
        {
           ArrayList elems = new ArrayList();
           elems.Capacity = 2;
           elems.Add(col.ColumnName);
           elems.Add(r.Cells[1].Controls[0]);
           _enablingControls.Add(elems);     

           //if (!IsPostBack) {
               controlEnabling(elems);
          // }
        }

        private void controlEnabling(ArrayList elem)
        {
            bool enable = true;
            String colName = (String) elem[0];
            Control ctrl = (Control) elem[1];
 
            
            
            
            if (isNew()) // addAdvancement
            {
                if (CBTreeFlag.Enabled) // tree visible
                {
                    if (isInherit()) // training selected from tree
                    {
                        // all enabled except of ..
                        if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                        {
                            enable = false;
                        }
                        if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "DESCRIPTION")))
                        {
                            enable = false;
                        }
                        if (colName.Equals("COST_EXTERNAL"))
                        {
                            enable = false;
                        }
                        if (colName.Equals("COST_INTERNAL"))
                        {
                            enable = false;
                        }
                        if (colName.Equals("LOCATION"))
                        {
                            enable = false;
                        }
                        if (colName.Equals("INSTRUCTOR"))
                        {
                            enable = false;
                        }
                        
                    }
                    else
                    {
                        enable = false; // all disabled
                    }
                }
                else 
                {
                    enable = true; // all enabled
                }
            }
            else // editAdvancement
            {
                if (CBTreeFlag.Enabled) // tree visible
                {
                    // all enabled except of ..
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "DESCRIPTION")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COST_EXTERNAL"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COST_INTERNAL"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("LOCATION"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("INSTRUCTOR"))
                    {
                        enable = false;
                    }
                    
                }
                else 
                {
                    enable = true; // all enabled
                }

                if ((_db.userId == (long)_table.Rows[0]["PERSON_ID"]) && (Global.isModuleEnabled("energiedienst"))|| AdvancemantReadOnly)
                {
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "DESCRIPTION")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("TOBEDONE_DATE"))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "CONTROLLING")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("TOBEDONE_DATE1"))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "CONTROLLING")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COMMENT"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COMMENT"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("STATE"))
                    {
                        enable = false;
                    }
                    if (colName.Equals(" VIEWED_FLAG"))
                    {
                        enable = true;
                    }
                   
                }
            }
            if (ctrl is TextBox) 
            {
                TextBox tx = (TextBox) ctrl;
                tx.Enabled = enable;
            }
            if (ctrl is DropDownCtrl && Global.isModuleEnabled("energiedienst"))
            {
                DropDownCtrl dd = (DropDownCtrl)ctrl;
                dd.Enabled = enable;
            }
            if (ctrl is Table && Global.isModuleEnabled("energiedienst"))
            {
                Table tab = (Table)ctrl;
                tab.Enabled = enable;
            }
        }
 

        private bool isNew()
        {
            if (AdvancementID > 0)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        private bool showTree()
        {
            if (isNew() || TrainingID > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isInherit()
        {
            if (TrainingID > 0)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        protected void viewed_Changed(object sender, System.EventArgs e)
        {
            _db.connect();
            _db.execute("Update TRAINING_ADVANCEMENT SET VIEWED_FLAG = 1 WHERE ID = " + AdvancementID);

            string toID = _db.lookup("RESPONSIBLE_PERSON_ID", "TRAINING_ADVANCEMENT", "ID= " + +AdvancementID).ToString();

            _db.disconnect();
            sendMail("Statusänderung Personalentwicklungsbedarf", "<font face=\"Arial\" size=\"3\">Der Status des Personalentwicklungsbedarfs wurde von sender auf 'Eingesehen' geändert.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">https://srv132/p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>",toID);
        }

        protected void release_Click(object sender, System.EventArgs e)
        {
            releaseButton.Enabled = false;
            apply.Enabled = false;
            
            DateTime time = System.DateTime.Now;
            _db.connect();
            _db.execute("UPDATE TRAINING_ADVANCEMENT SET RELEASE = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID ='" + AdvancementID + "'");
            _db.execute("UPDATE TRAINING_ADVANCEMENT SET RESPONSIBLE_PERSON_ID = '" + _db.userId + "' WHERE ID ='" + AdvancementID + "'");

            string toID = _db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID= " + +AdvancementID).ToString();
            _db.disconnect();
            sendMail("Statusänderung Personalentwicklungsbedarf", "<font face=\"Arial\" size=\"3\">Der Status Ihres Personalentwicklungsbedarfs wurde von sender auf 'Gespräch geführt' geändert.<br><br>Bitte überprüfen Sie Ihren Personalentwicklungsbedarf und visieren Sie diese mit einem Klick auf 'Eingesehen von receiver'.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">https://srv132/p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>",toID);

        }

        protected void apply_Click(object sender, System.EventArgs e) 
        {
            if (checkInputValue(_table, advancementEdit)) 
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    StringBuilder sqlB = getSql(_table, advancementEdit, true);
                    
                    if (InputType == InputMaskBuilder.InputType.Add) 
                    {
                        AdvancementID = _db.newId("TRAINING_ADVANCEMENT");
                        extendSql(sqlB, _table, "ID", AdvancementID);
                        if (PersonID > 0)
                        {
                            extendSql(sqlB, _table, "PERSON_ID", PersonID);
                            if(Global.isModuleEnabled("energiedienst"))
                            {
                                extendSql(sqlB, _table, "RESPONSIBLE_PERSON_ID", _db.userId);
                            }
                        }
                        if (TrainingID > 0 && CBTreeFlag.Enabled)
                        {
                            extendSql(sqlB, _table, "TRAINING_ID", TrainingID);
                        }
                    }
                    else if (InputType == InputMaskBuilder.InputType.Edit)
                    {
                        if (TrainingID > 0 && !CBTreeFlag.Enabled)
                        {
                            extendSql(sqlB, _table, "TRAINING_ID", null);
                        }
                        if (TrainingID > 0 && CBTreeFlag.Enabled)
                        {
                            extendSql(sqlB, _table, "TRAINING_ID", TrainingID);
                        }
                    }
                    string sql = endExtendSql(sqlB);
                    if (sql.Length > 0)
                        _db.execute(sql);

                    _db.commit();
                }
                catch (Exception ex) 
                {
                    _db.rollback();
                    DoOnException(ex);
                }
                finally 
                {
                    _db.disconnect();   
                }
            
                Response.Redirect(Global.Config.baseURL + "/Energiedienst/Advancement.aspx?advancementID=" + AdvancementID + "&personID=" + PersonID);
            }		
        }

        private void MapButtonMethods() 
        {
            this.CBTreeFlag.CheckedChanged += new System.EventHandler(this.CBTreeFlag_CheckedChanged);
        }

        private void CBTreeFlag_CheckedChanged(object sender, System.EventArgs e) 
        {
            CBTreeFlag.Enabled = CBTreeFlag.Checked;
            foreach (Object o in _enablingControls)
            {
                controlEnabling((ArrayList) o);
            }
            if (_treeCtrl != null)
            {
                _treeCtrl.Visible = CBTreeFlag.Enabled;
            }
        }

        protected void sendMail(string subject, string message,string toID)
        {
            _db.connect();
            string _personId = _db.lookup("PERSON_ID", "TRAINING_ADVANCEMENT", "ID = " + AdvancementID).ToString();

            MailMessage myMessage = new MailMessage();
            try
            {
            myMessage.From = new MailAddress(_db.lookup("EMAIL", "PERSON", "ID = " + _db.userId).ToString(), _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId,"").ToString());
            myMessage.To.Add(_db.lookup("EMAIL", "PERSON", "ID = " + toID).ToString());
                        }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

            string senderName = _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId).ToString();
            string receiverName = _db.lookup("PNAME", "PERSON", "ID = " + _personId).ToString() + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = " + _personId).ToString();
            message = message.Replace("sender", senderName);
            message = message.Replace("receiver", receiverName);

            myMessage.Body = message;
            SmtpClient mySmtpClient = new SmtpClient();
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(Global.Config.getModuleParam("dispatch", "UserName", ""), Global.Config.getModuleParam("dispatch", "passwordFrom", ""));
            mySmtpClient.Host = Global.Config.getModuleParam("dispatch", "smtpServer", "");
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            mySmtpClient.ServicePoint.MaxIdleTime = 1;

            try
            {
                mySmtpClient.Send(myMessage);
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(this.GetType(), "myalert", "alert('Eine e-Mail Benachrichtigung mit der Statusänderung wurde erfolgreich versandt.'); window.location = \"" + Request.Url.AbsoluteUri + "\";", true);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            MapButtonMethods();
        }
		
        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}
