namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PasswordEdit.
    /// </summary>
    public partial class PasswordEdit : PSOFTInputViewUserControl {
        public const string PARAM_ID = "PARAM_ID";
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";

        protected System.Web.UI.HtmlControls.HtmlForm Edit;

        private TableRow _oldPwdRow;
        private TableCell _oldPwdLbl;
        private TableCell _oldPwdVal;
        private TextBox _oldPwdInput;
        private TableRow _newPwdRow;
        private TableCell _newPwdLbl;
        private TableCell _newPwdVal;
        private TextBox _newPwdInput;
        private TableRow _newPwdRowC;
        private TableCell _newPwdLblC;
        private TableCell _newPwdValC;
        private TextBox _newPwdInputC;

        private static int _IDX_oldPwdInput = 0;
        private static int _IDX_newPwdInput = 1;
        private static int _IDX_newPwdInputC = 2;


        public static string Path {
            get {return Global.Config.baseURL + "/Person/Controls/PasswordEdit.ascx";}
        }

		#region Properities
        public string IDParam {
            get {return GetString(PARAM_ID);}
            set {SetParam(PARAM_ID, value);}
        }

        public string BackUrl {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            base.InputType = InputMaskBuilder.InputType.Edit;

            if (!IsPostBack) 
            {
                apply.Text = _mapper.get("apply");
            }

            editTab.Rows.Clear();

            _oldPwdRow = new TableRow();
            _oldPwdLbl = new TableCell();
            _oldPwdVal = new TableCell();
            _oldPwdInput = new TextBox();
            _oldPwdInput.TextMode = TextBoxMode.Password;
            PsoftContentPage.SetFocusControl(_oldPwdInput, true);
            _oldPwdLbl.Text = _mapper.get("person","oldPassword");
            _oldPwdRow.CssClass = "InputMask";
            _oldPwdLbl.CssClass = "InputMask_Label";
            _oldPwdVal.CssClass = "InputMask_Value";
            _oldPwdInput.CssClass = "InputMask_Label";
            _oldPwdVal.Controls.Add(_oldPwdInput);
            _oldPwdRow.Cells.Add(_oldPwdLbl);
            _oldPwdRow.Cells.Add(_oldPwdVal);
            editTab.Rows.AddAt(_IDX_oldPwdInput,_oldPwdRow);

            _newPwdRow = new TableRow();
            _newPwdLbl = new TableCell();
            _newPwdVal = new TableCell();
            _newPwdInput = new TextBox();
            _newPwdInput.TextMode = TextBoxMode.Password;
            _newPwdLbl.Text = _mapper.get("person","newPassword");            
            _newPwdRow.CssClass = "InputMask";
            _newPwdLbl.CssClass = "InputMask_Label";
            _newPwdVal.CssClass = "InputMask_Value";
            _newPwdInput.CssClass = "InputMask_Label";
            _newPwdVal.Controls.Add(_newPwdInput);
            _newPwdRow.Cells.Add(_newPwdLbl);
            _newPwdRow.Cells.Add(_newPwdVal);
            editTab.Rows.AddAt(_IDX_newPwdInput,_newPwdRow);

            _newPwdRowC = new TableRow();
            _newPwdLblC = new TableCell();
            _newPwdValC = new TableCell();
            _newPwdInputC = new TextBox();
            _newPwdInputC.TextMode = TextBoxMode.Password;
            _newPwdLblC.Text = _mapper.get("person","newPasswordConfirm");            
            _newPwdRowC.CssClass = "InputMask";
            _newPwdLblC.CssClass = "InputMask_Label";
            _newPwdValC.CssClass = "InputMask_Value";
            _newPwdInputC.CssClass = "InputMask_Label";
            _newPwdValC.Controls.Add(_newPwdInputC);
            _newPwdRowC.Cells.Add(_newPwdLblC);
            _newPwdRowC.Cells.Add(_newPwdValC);
            editTab.Rows.AddAt(_IDX_newPwdInputC,_newPwdRowC);

        }


        private void mapControls () {
            apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            int error = 0;
            string ERR_oldPwdInput = "";
            string ERR_newPwdInput = "";
            string ERR_newPwdInputC = "";

            DBData db = DBData.getDBData(Session);
            db.connect();
            try {
                if (_oldPwdInput.Text == ch.psoft.Util.Validate.GetValid(db.lookup("PASSWORD","PERSON","ID="+db.userId).ToString(),""))
                {
                    if (_newPwdInput.Text != _newPwdInputC.Text)
                    {
                        ERR_newPwdInputC = "notEqual";
                        error++;
                    }
                    else if (_newPwdInput.Text == "")
                    {
                        ERR_newPwdInput = "isEmpty";
                        error++;
                    }
                }
                else 
                {
                    ERR_oldPwdInput = "notValid";
                    error++;
                }
                if (error == 0)
                {
                    db.execute("update PERSON set PASSWORD='"+_newPwdInput.Text+"' where ID="+db.userId);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();   
            }

            _oldPwdInput.CssClass = "InputMask_Label";
            _newPwdInput.CssClass = "InputMask_Label";
            _newPwdInputC.CssClass = "InputMask_Label";

            switch (error)
            {
                case 0:
                    if (BackUrl != "")
                        Response.Redirect(BackUrl, false);
                    break;
                default:
                    int offset = 1;
                    if (ERR_oldPwdInput == "notValid")
                    {                        
                        TableRow errorRow = new TableRow();
                        TableCell errorCell= new TableCell();
                        errorRow.Cells.Add(new TableCell());
                        errorRow.Cells.Add(errorCell);
                        errorRow.CssClass = "InputMask_Error";
                        editTab.Rows.AddAt(_IDX_oldPwdInput+offset,errorRow);
                        errorCell.Text = _mapper.get("error","confirmError");
                        _oldPwdInput.CssClass = "InputMask_Error";
                        offset++;
                    }
                    if (ERR_newPwdInputC == "notEqual")
                    {
                        TableRow errorRow = new TableRow();
                        TableCell errorCell= new TableCell();
                        errorRow.Cells.Add(new TableCell());
                        errorRow.Cells.Add(errorCell);
                        errorRow.CssClass = "InputMask_Error";
                        editTab.Rows.AddAt(_IDX_newPwdInputC+offset,errorRow);
                        errorCell.Text = _mapper.get("error","inputErr_50");
                        _newPwdInputC.CssClass = "InputMask_Error";
                        offset++;
                    }
                    if (ERR_newPwdInput == "isEmpty")
                    {
                        TableRow errorRow = new TableRow();
                        TableCell errorCell= new TableCell();
                        errorRow.Cells.Add(new TableCell());
                        errorRow.Cells.Add(errorCell);
                        errorRow.CssClass = "InputMask_Error";
                        editTab.Rows.AddAt(_IDX_newPwdInput+offset,errorRow);
                        errorCell.Text = _mapper.get("error","inputErr_40");
                        _newPwdInput.CssClass = "InputMask_Error";
                        offset++;
                    }
                    break;
            }

        }

  

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            mapControls();
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