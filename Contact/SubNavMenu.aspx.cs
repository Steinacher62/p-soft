using ch.appl.psoft.Common;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using System;
using System.Web;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for navigation.
    /// </summary>
    public partial class SubNavMenu : PsoftMenuPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");;
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl)this.LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SUBNAV_TITLE);
            ctrl.addMenuItem(null, "searchContact", _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SUBNAV_SEARCHCONTACT), psoft.Contact.ContactSearch.GetURL());
            ctrl.addMenuItem(null, "myContactGroups", _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SUBNAV_MYCONTACTGROUPS), Global.Config.baseURL + "/Contact/ContactGroupDetail.aspx");
            ctrl.addMenuItem(null, "newContactPerson", _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SUBNAV_NEWCONTACTPERSON), Global.Config.baseURL + "/Contact/ContactAdd.aspx?type=" + ContactModule.TYPE_PERSON + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?ID=%ID&type=" + ContactModule.TYPE_PERSON));
            ctrl.addMenuItem(null, "newContactCompany", _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SUBNAV_NEWCONTACTCOMPANY), Global.Config.baseURL + "/Contact/ContactAdd.aspx?type=" + ContactModule.TYPE_FIRM + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?ID=%ID&type=" + ContactModule.TYPE_FIRM));
            ctrl.addMenuItem(null, "newContactGroup", _mapper.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_SUBNAV_NEWCONTACTGROUP), Global.Config.baseURL + "/Contact/ContactGroupAdd.aspx?personID=" + SessionData.getUserID(Session) + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?xID=%ID&mode=" + ContactDetail.MODE_CONTACTGROUP));

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
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
	}
}
