using System;
using System.IO;
using System.Web;

namespace ch.appl.psoft.SBS
{
    public partial class pdfViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //generate png and JSON files of pdf if not available

            String path = HttpUtility.UrlDecode("../"+ Request.Url.Query.Substring(Request.Url.Query.IndexOf("pdfPath=",0)+8));
           
            String[] splitPath = path.Split('/');
            String fileName = splitPath[splitPath.Length-1];

            // Set the Docs Path where the processed files will be stored   
            string uploadFolderAbsPath = Server.MapPath(path).Replace(fileName, "");
            // Set the PDF Source Path and filename without extension
            string sourceFilePath = Server.MapPath(path).Replace(fileName,"");
            fileName = fileName.Substring(0, fileName.Length - 4);

            PDFProcess pdfProcess = new PDFProcess();
            
            //Convert PDF to Images   
            if (!File.Exists(sourceFilePath+fileName+ "_1.png"))
            {
                pdfProcess.PDF2Image(fileName, uploadFolderAbsPath, sourceFilePath, fileName+".pdf");
            }
            
            //Convert PDF to JSON   
            if (!File.Exists(sourceFilePath + fileName  + ".js"))
            {
                pdfProcess.PDF2JSON(fileName, uploadFolderAbsPath, sourceFilePath, fileName+".pdf");
            }
    
        }


    }
}