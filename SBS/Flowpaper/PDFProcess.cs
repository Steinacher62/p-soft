using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
/// <summary>
/// Summary description for PDF2Image
/// </summary>
public class PDFProcess
{
    //converts pdf to png at upload folder location
	public int PDF2Image(string uploadFileName , string uploadFolder,string sourceFilePath,string sourceFileName)
	{
        try
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = HttpContext.Current.Server.MapPath("~/SBS/FlowPaper/convertExec/mudraw.exe");
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.Arguments = " -r100 -o " + "\"" + uploadFolder + uploadFileName + "_%d.png" + "\" " + "\"" + sourceFilePath + sourceFileName + "\"";

            if (proc.Start())
            {
                proc.WaitForExit();
                proc.Close();
                return 1;
            }
            else
                return 2;
        }
        catch
        {
            return 2;
        }
	}
    //generates pdf info in as json string
    public int PDF2JSON(string uploadFileName, string uploadFolder, string sourceFilePath, string sourceFileName)
    {
        try
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = HttpContext.Current.Server.MapPath("~/SBS/FlowPaper/convertExec/pdf2json.exe");
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.Arguments = "\"" + sourceFilePath + sourceFileName + "\" -enc UTF-8 -compress " + "\"" + uploadFolder + uploadFileName + ".js" + "\"";

            if (proc.Start())
            {
                proc.WaitForExit();
                proc.Close();
                return 1;
            }
            else
                return 2;
        }catch
        {
            return 2;
        }
    }

 
   
}