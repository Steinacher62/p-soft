using ch.appl.psoft.Interface;
using ch.psoft.db;
using System;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for DateSelector.
    /// </summary>
    /// <param name="inputCtrlID">ID of the opener's input-control that should receive the selected date-value</param>
    public partial class DateSelector : System.Web.UI.Page
	{

        protected String _pageTitle = "";
        protected String _onloadString = ";";
        protected String _inputCtrlID = "";
		protected String _format = "";
		protected String _value = "";
		protected DateTime _dateTime;
    
		protected void Page_Load(object sender, System.EventArgs e)
		{
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            _pageTitle = map.get("dateSelector", "pageTitle");
            _inputCtrlID = Request.QueryString["inputCtrlID"];
            _format = ch.psoft.Util.Validate.GetValid(
					Request.QueryString["format"], 
					SessionData.getDBColumn(Session).UserCulture.DateTimeFormat.ShortDatePattern
				);
			_value = Request.QueryString["value"];

			if (_value.Equals(""))
			{
				_dateTime = DateTime.Now;
			}
			else
			{
				try
				{
					_dateTime = SQLColumn.parseDate(
							_value,
							_format,
							SessionData.getDBColumn(Session).UserCulture
						);
				}
				catch (FormatException)
				{
					_dateTime = DateTime.Now;
				}
			}

            if (!IsPostBack)
            {
				selectButton.Text = map.get("next");
                calendar.TodaysDate = DateTime.Now;
                calendar.SelectedDate = _dateTime;
                calendar.VisibleDate = _dateTime;

                // There is a bug in the calendar control which does not allow to fully control
                // its appearance through usage of CssClasses - therefore we hardcode it here...
                calendar.TitleStyle.BackColor = Color.FromArgb(0xbd, 0xe0,0xef);
                calendar.TodayDayStyle.ForeColor = Global.HighlightColor;
                calendar.TodayDayStyle.Font.Bold = true;
                calendar.SelectedDayStyle.BackColor = Global.HighlightColor;
                calendar.SelectedDayStyle.Font.Bold = true;
                calendar.OtherMonthDayStyle.ForeColor = Color.Gray;
                calendar.WeekendDayStyle.BackColor = Color.FromArgb(0xed, 0xf0, 0xff);
            }
		}

        /// <summary>
        /// Creates a button with all the necessary attributes to open the date-selector dialog.
        /// </summary>
        /// <param name="textBox">TextBox to fill with the date-string</param>
        /// <returns></returns>
        public static HtmlInputButton getDateSelectorButton(Control textBox, string format)
		{
            HtmlInputButton b = new HtmlInputButton();
                
            b.Value = "...";
            b.Attributes.Add("class", "InputMask_DateSelector");
            b.Attributes.Add(
				"onclick",
				"text = window.document.getElementById('" + textBox.ClientID + "').value;"
					+ "window.open('"
					+ Global.Config.baseURL + "/Common/DateSelector.aspx"
					+ "?inputCtrlID=" + textBox.ClientID
					+ "&format=" + format
					+ "&value=' + text,"
					+ "'_blank',"
					+ "'locationbar=0,menubar=0,titlebar=0,status=0,toolbar=0,resizable=1,width=200,height=250'"
					+ ");"
			);
            return b;
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
        }
		#endregion

        protected void selectButton_Click(object sender, System.EventArgs e)
        {
            DateTime theDate = calendar.SelectedDate;
			TimeSpan time = _dateTime.Subtract(_dateTime.Date);
			theDate = theDate.Add(time);
            _onloadString = "window.opener.document.getElementById('" + _inputCtrlID + "').value='" + theDate.ToString(_format, DateTimeFormatInfo.InvariantInfo) + "'; window.close();";
        }
	}
}
