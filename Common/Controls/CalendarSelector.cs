using AjaxControlToolkit;
using ch.appl.psoft.Interface;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common.Controls
{
    public class CalendarSelector : UserControl
    {
        #region Properties

        public ImageButton imgButton { get; set; }
        public CalendarExtender cal { get; set; }

        string imgButtonID { get; set; }
        string calID { get; set; }

        TextBox tb { get; set; }
        string format { get; set; }
        LanguageMapper mapper { get; set; }

        #endregion

        #region Constructors

        private CalendarSelector()
            : base()
        {
            this.imgButtonID = "calimgbutton";
            this.calID = "calendarext";
        }

        private CalendarSelector(HttpSessionState Session)
            : this()
        {
            this.mapper = LanguageMapper.getLanguageMapper(Session);
        }

        private CalendarSelector(LanguageMapper mapper)
            : this()
        {
            this.mapper = mapper;
        }

        public CalendarSelector(HttpSessionState Session, TextBox tb, string format) : this(Session)
        {
            this.tb = tb;
            this.format = format;
            this.imgButtonID = tb.ClientID + "_" + imgButtonID;
            this.calID = tb.ClientID + "_" + calID;
            init();
        }

        public CalendarSelector(LanguageMapper mapper, TextBox tb, string format) : this(mapper)
        {
            this.tb = tb;
            this.format = format;
            this.imgButtonID = tb.ClientID + "_" + imgButtonID;
            this.calID = tb.ClientID + "_" + calID;
            init();
        }

        public CalendarSelector(LanguageMapper mapper, TextBox tb, string format, string imgButtonID, string calID)
            : this(mapper)
        {
            this.tb = tb;
            this.format = format;
            this.imgButtonID = imgButtonID;
            this.calID = calID;
            init();
        }

        public CalendarSelector(HttpSessionState Session, TextBox tb, string format, string imgButtonID, string calID)
            : this(Session)
        {
            this.tb = tb;
            this.format = format;
            this.imgButtonID = imgButtonID;
            this.calID = calID;
            init();
        }

        #endregion

        private void init()
        {
            //create ImageButton
            this.imgButton = new ImageButton();
            this.imgButton.ID = imgButtonID;
            this.imgButton.CssClass = "calendar";
            this.imgButton.ImageUrl = Global.Config.baseURL + "/images/calendar.png";
            this.imgButton.AlternateText = mapper.get("clickcalendar");
            this.imgButton.TabIndex = 10;

            //create CalendarExtender
            this.cal = new CalendarExtender();
            this.cal.CssClass = "MyCalendar";
            this.cal.ID = calID;
            this.cal.PopupButtonID = imgButtonID;
            this.cal.TargetControlID = tb.ID;

            if (format != null && format.Trim() != "")
            {
                this.cal.Format = format;
            }
            else
            {
                this.cal.Format = "dd.MM.yyyy";
            }
        }
    }
}
