using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for Argument.
    /// </summary>
    public partial class Argument : System.Web.UI.Page {
    

        private DBData _db = null;
        private long _id = 0;

        protected void Page_Load(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);
            _id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"],0L);
            if (_id <= 0) return;

            if (!IsPostBack) {
                LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
                apply.Text = map.get("apply");
           
                _db.connect();
                try {
                    string title = _db.langAttrName("OBJECTIVE_ARGUMENT","TITLE");
                    DataTable table = _db.getDataTable("select ID,"+title+" from OBJECTIVE_ARGUMENT order by "+title);

                    argumentList.DataSource = table;
                    argumentList.DataTextField = title;
                    argumentList.DataValueField = "ID";
                    argumentList.DataBind();
                    object[] objs = _db.lookup(new string[] {"argument_id","argument"},"objective","id="+_id);
                    long argId = DBColumn.GetValid(objs[0],0L);
                    string text = DBColumn.GetValid(objs[1],"");
                    if (argId > 0) {
                        int idx = 0;
                        foreach (ListItem item in argumentList.Items) {
                            if (item.Value == argId.ToString()) {
                                argumentList.SelectedIndex = idx;
                                break;
                            }
                            idx++;
                        }
                        argumentText.Text = text;
                    }
                }
                catch (Exception ex) {
                    Logger.Log(ex,Logger.ERROR);
                }
                finally {
                    _db.disconnect();
                }
            }
        }

        private void applyClick(object sender, System.EventArgs e) {
            string sql = "update objective set argument_id=";
            _db.connect();
            try {
                sql += argumentList.SelectedItem.Value;
                sql += ",argument=";
                sql += argumentText.Text == "" ? "null" : "'"+DBColumn.toSql(argumentText.Text)+"'";
                sql += " where id = "+_id;
                _db.execute(sql);
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }

        private void mapControls() {
            apply.Click += new System.EventHandler(this.applyClick);
            apply.Attributes.Add("onclick","window.close()");
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    

        }
		#endregion
    }
}
