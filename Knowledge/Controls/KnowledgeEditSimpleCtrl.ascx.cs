using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace ch.appl.psoft.Knowledge.Controls
{
    public partial class KnowledgeEditSimple : PSOFTInputViewUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //editor.ID = "Input-" + _idPrefix + "-" + col.ColumnName + (confirm ? "_Confirm" : "");
            editor.Language = "de-De";
            //editor.OnClientLoad = "EditorLoaded";
            String[] ImagePaths = { "~/Knowledge/Templates/Images" };
            String[] DocumentPaths = { "~/Knowledge/Templates/Documents" };
            String[] FlashPaths = { "~/Knowledge/Templates/Flash" };
            String[] MediaPaths = { "~/Knowledge/Templates/Medias" };
            string[] HtmlPaths = { "~/Knowledge/Templates/HtmlDocs" };
            String[] extension = { "*.mp4" };

            editor.ImageManager.UploadPaths = ImagePaths;
            editor.ImageManager.DeletePaths = ImagePaths;
            editor.ImageManager.ViewPaths = ImagePaths;
            editor.DocumentManager.UploadPaths = DocumentPaths;
            editor.DocumentManager.DeletePaths = DocumentPaths;
            editor.DocumentManager.ViewPaths = DocumentPaths;
            editor.FlashManager.UploadPaths = FlashPaths;
            editor.FlashManager.DeletePaths = FlashPaths;
            editor.FlashManager.ViewPaths = FlashPaths;
            editor.FlashManager.MaxUploadFileSize = 200000000;
            editor.MediaManager.UploadPaths = MediaPaths;
            editor.MediaManager.DeletePaths = MediaPaths;
            editor.MediaManager.ViewPaths = MediaPaths;
            editor.MediaManager.MaxUploadFileSize = 2000000000;
            editor.MediaManager.SearchPatterns = extension;
            editor.TemplateManager.UploadPaths = HtmlPaths;
            editor.TemplateManager.ViewPaths = HtmlPaths;
            editor.TemplateManager.DeletePaths = HtmlPaths;

            editor.EnableViewState = false;


        }

       
    }
}