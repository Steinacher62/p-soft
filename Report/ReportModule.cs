using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.IO;

namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for ReportModule.
    /// </summary>
    public class ReportModule : psoftModule 
    {
        public const string REPORTS_DIRECTORY = "/Reports";

        public enum Target 
        {
            Undefined = 0,
            Overall,
            OrgEntity,        // selected organisation entity
            PersonSelection,  // selected persons
            ContactSelection, // selected contacts
            ContactGroup,     // contacts of a contact-group
            ContactFirm,      // contacts of a contact-firm
        }

        public enum ReportType 
        {
            Undefined = 0,
            List,
            Email,
            Letter,
            EmailAndLetter,
            ListExcel
        }
        public enum Format 
        {
            Portrait,
            Landscape
        }
        public enum Layout 
        {
            Uniform,
            Computer,
            Grid
        }
        public enum HAlign 
        {
            Left,
            Center,
            Right
        }
        public enum VAlign 
        {
            Top,
            Middle,
            Bottom
        }
        
        public static string headerLogoImage{
            get { return Global.Config.getModuleParam("report", "headerLogoImage", "PsoftDogBlack.gif"); }
        }

        public static bool debugXML
        {
            get { return Global.Config.getModuleParam("report", "debugXML", DefaultValues.DebugXML) == "1"; }
        }

        public ReportModule() : base() 
        {
            m_NameMnemonic = "report";
            m_StartURL = psoft.Report.ReportLayoutSelect.GetURL("target",(int)Target.Overall, "nextURL",psoft.Report.ReportListing.GetURL("reportLayoutID","%ID"));
            m_IsVisible = false;
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Report/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static void CleanupReportsDirectory(){
            string [] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory.ToString() + ReportModule.REPORTS_DIRECTORY);
            foreach (string file in files){
                try{
                    File.Delete(file);
                }
                catch (Exception ex){
                    Logger.Log(ex, Logger.ERROR);
                }
            }
        }
    }
}
