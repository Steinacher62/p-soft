namespace ch.appl.psoft.Interface
{
    class DefaultValues
    {
        public static string BaseURL { get { return "/Psoft"; } }
        public static string StylesheetDirectory { get { return "/XML/stylesheets/"; }}
        public static string TmpDirectory { get { return "/tmp/"; } }
        public static string SuggestionExcelXSLT { get { return "suggestion_excel_dflt.xslt"; } }
        public static string TasklistExcelXSLT { get { return "tasklist_excel_dflt.xslt"; } }
        public static string DebugXML { get { return "1"; } }
        public static string ExcelSuffix { get { return ".xsl"; } }
        public static string XMLSuffix { get { return ".xml"; } }

        // Project transformation stylesheets filenames
        public static string ProjectScoreCardExcelXSLT { get { return "project_scorecard_excel_dflt.xslt"; } }
        public static string ProjectCostControlExcelXSLT { get { return "project_costcontrol_excel_dflt.xslt"; } }
        public static string ProjectControllingExcelXSLT { get { return "project_controlling_excel_dflt.xslt"; } }

        
    }
}
