using System.Web;
using System.Web.SessionState;


namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for PsoftModule.
    /// </summary>
    public abstract class psoftModule
	{
        protected Global m_Global = null;
        protected bool m_IsVisible = true;
        protected string m_NameMnemonic = "undefined";
        protected string m_StartURL = "";
        protected string m_SubNavMenuURL = "";

        public psoftModule() {}

        /// <summary>
        /// True if the module should be visible with a menu-item in the header-page.
        /// </summary>
        public virtual bool IsVisible(HttpSessionState session)
        {
            return m_IsVisible;
        }

        /// <summary>
        /// The internal name of the module.
        /// </summary>
        public virtual string Name
        {
            get{ return m_NameMnemonic; }
        }

        /// <summary>
        /// The localized name of the module.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public virtual string GetLocalName(LanguageMapper map)
        {
            return map.get("ModuleNames", m_NameMnemonic);
        }

        /// <summary>
        /// The start URL opened, when clicked on module in the header menu.
        /// </summary>
        public virtual string StartURL {
            get{ return m_StartURL; }
        }

        /// <summary>
        /// The navigation menu URL to be opened, when clicked on module in the header menu (if StartURL is not defined).
        /// </summary>
        public virtual string SubNavMenu {
            get{ return m_SubNavMenuURL; }
        }

        /// <summary>
        /// Called to load the language-dependent XML-file for the module.
        /// </summary>
        /// <param name="map">LanguageMapper used to load</param>
        /// <param name="languageCode">Language-code to load</param>
        public virtual void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
        }

        /// <summary>
        /// Called when application starts. Used for module initialisation.
        /// </summary>
        /// <param name="application">The application</param>
        public virtual void Application_Start(HttpApplicationState application)
        {
        }

        /// <summary>
        /// Called when application ends. Used for module clean-up.
        /// </summary>
        /// <param name="application">The application</param>
        public virtual void Application_End(HttpApplicationState application)
        {
        }
	
        /// <summary>
        /// Called when authenticating a request.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Application_AuthenticateRequest(HttpContext context)
        {
        }

        /// <summary>
        /// Called when session starts.
        /// </summary>
        /// <param name="session">The new session</param>
        public virtual void Session_Start(HttpSessionState session)
        {
        }

        /// <summary>
        /// Called when session ends.
        /// </summary>
        /// <param name="session">The ending session</param>
        public virtual void Session_End(HttpSessionState session)
        {
        }

        /// <summary>
        /// Called after a user logs in
        /// </summary>
        /// <param name="session"></param>
        public virtual void OnAfterLogin(HttpSessionState session){
        }
    }
}
