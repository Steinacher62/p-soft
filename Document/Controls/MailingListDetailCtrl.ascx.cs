namespace ch.appl.psoft.Document.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    //using Redemption;

    using Interface;
    using System;
    using System.Data;
    using System.IO;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public partial class MailingListDetailCtrl : PSOFTListViewUserControl {

		private DetailBuilder _detailBuilder;
        private DataTable _table;
        private DBData _db;

        protected System.Web.UI.WebControls.Table messageTab;

		private string href = "";
		private string messagename = "";
		private int nrofattach = 0;

        //never used, thus commented
        //private string eml;

        //never used, thus commented
        //private string msg;

		public MailingListDetailCtrl() : base() 
		{
			HeaderEnabled = true;
			DeleteEnabled = false;
			EditEnabled = false;
			UseJavaScriptToSort = true;
			UseFirstLetterAsPageSelector = true;
		}

        public static string Path {
            get {return Global.Config.baseURL + "/Document/Controls/MailingListDetailCtrl.ascx";}
        }

		#region Properities
		public long _messageID {
			get {return GetLong("contextID");}
			set {SetParam("contextID", value);}
		}

		public long _exchangeFolderID 
		{
			get {return GetLong("xID");}
			set {SetParam("xID", value);}
		}
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

			_detailBuilder = new DetailBuilder();
            _db = DBData.getDBData(Session);

			try {
                _db.connect();

				string sql = "select ID, FOLDERNAME, MESSAGENAME, CREATIONDATE, HREF, EML_FROM, NROFATTACH from EXCHANGE_MESSAGES_TMP where " 
					+ "SESSIONID='" + Session.SessionID + "' AND ID=" + _messageID;

				_table = _db.getDataTableExt(sql, "EXCHANGE_MESSAGES_TMP");

				if(_table.Rows.Count > 0) 
				{
					href = _table.Rows[0][_table.Columns["HREF"]].ToString();
					messagename = _table.Rows[0][_table.Columns["MESSAGENAME"]].ToString();
					nrofattach = Convert.ToInt32(_table.Rows[0][_table.Columns["NROFATTACH"]].ToString());

					Button downloadEML = new Button();
					//Button downloadMSG = new Button();
					downloadEML.Text = _mapper.get("exchange","downloadEML");
					//downloadMSG.Text = _mapper.get("exchange","downloadMSG");
					downloadEML.Click += new EventHandler(DownloadEMLBtn_Click);
					//downloadMSG.Click += new EventHandler(DownloadMSGBtn_Click);
					TableCell cell1 = new TableCell();
					//TableCell cell2 = new TableCell();
					cell1.Controls.Add(downloadEML);
					//cell2.Controls.Add(downloadMSG);
					TableRow row1 = new TableRow();
					//TableRow row2 = new TableRow();
					row1.Cells.Add(cell1);
					//row2.Cells.Add(cell2);
              
					messageTab = new Table();
					_detailBuilder.load(_db, _table, messageTab, _mapper);
					
					if(nrofattach > 0) 
					{
						sql = "select * from EXCHANGE_ATTACHEMENTS_TMP where SESSIONID='" + Session.SessionID + "' AND MESSAGE_ID=" + _messageID;

						IDColumn = "ID";

						_table = _db.getDataTableExt(sql, "EXCHANGE_ATTACHEMENTS_TMP");
//						attachementTab = new Table();
						_table.Columns["FILENAME"].ExtendedProperties["ContextLink"] = "../Document/MailDownloadAttachement.aspx?xID=" + _exchangeFolderID + "&messageID=%MESSAGE_ID&attachID=%ID";

						LoadList(_db, _table, attachementTab);
						TableCell attachCell = new TableCell();
						attachCell.Controls.Add(attachementTab);
						TableRow attachRow = new TableRow();
						attachRow.Controls.Add(attachCell);
						messageTab.Rows.Add(attachRow);
					}//if
					
					messageTab.Rows.Add(row1);
					//messageTab.Rows.Add(row2);
				
					//splitTab = new Table();
					TableRow splitTabRow = new TableRow();
					TableCell splitTabCellLeft = new TableCell();
					TableCell splitTabCellRight = new TableCell();

					//Add messageTab
					splitTabCellLeft.Controls.Add(messageTab);
					splitTabCellLeft.Attributes["width"] = "30%";
					splitTabCellLeft.Attributes["valign"] = "top";

					//Create Table for IFrame
					Table iFrameTable = new Table();
					iFrameTable.Attributes["style"] = "height:100%;width:100%";

					//Add Table to the right Tab
					splitTabCellRight.Controls.Add(iFrameTable);
					splitTabCellRight.Attributes["style"] = "height:100%;width:70%";
				
					//Create Rows
					TableRow iFrameTableRow1 = new TableRow();
					TableRow iFrameTableRow2 = new TableRow();
					iFrameTableRow2.Attributes["style"] = "height:100%;";
				
					//Add Rows
					iFrameTable.Controls.Add(iFrameTableRow1);
					iFrameTable.Controls.Add(iFrameTableRow2);

					//One TableData for each Row
					TableCell iFrameTableRow1Cell = new TableCell();
					TableCell iFrameTableRow2Cell = new TableCell();

					//Add Cells
					iFrameTableRow1.Controls.Add(iFrameTableRow1Cell);
					iFrameTableRow2.Controls.Add(iFrameTableRow2Cell);

					//Create Row1 Table
					Table iFrameTableRow1CellTable = new Table();
					iFrameTableRow1CellTable.Attributes["width"] = "100%";

					//Add Table
					iFrameTableRow1Cell.Controls.Add(iFrameTableRow1CellTable);

					//Create Row
					TableRow iFrameTableRow1CellTableRow = new TableRow();
				
					//Add that Row
					iFrameTableRow1CellTable.Controls.Add(iFrameTableRow1CellTableRow);

					//Create the innerCells
					TableCell iFrameTableRow1CellTableRowCell1 = new TableCell();
					TableCell iFrameTableRow1CellTableRowCell2 = new TableCell();

					//And Adding
					iFrameTableRow1CellTableRow.Controls.Add(iFrameTableRow1CellTableRowCell1);
					iFrameTableRow1CellTableRow.Controls.Add(iFrameTableRow1CellTableRowCell2);

					//string style = (string) Session["style"];
					//if(style == null) 
					//{
					//	style = "html";
					//}

					//Create IFrame
					HtmlControl frame1 = new System.Web.UI.HtmlControls.HtmlGenericControl("iframe");
					//frame1.Attributes["src"] = "http://www.google.com";
					frame1.Attributes["src"] = "../Document/MailShowIFrame.aspx?contextID=" + _messageID + "&xID=" + _exchangeFolderID;
					frame1.Attributes["frameborder"] = "1";
					frame1.Attributes["scrolling"] = "auto";
					frame1.Attributes["name"] = "emailFrame";
					frame1.ID = "emailFrame";
					frame1.Attributes["width"] = "100%";
					frame1.Attributes["runat"] = "server";
					frame1.Attributes["onload"] = "adjustIFrameHeight('" + frame1.ID + "');";
					frame1.Attributes["style"] = "height:100%;";

					//Create RadioButtons and Label
					iFrameTableRow1CellTableRowCell1.Wrap = false;
					iFrameTableRow1CellTableRowCell1.CssClass = "Detail_Label";
					iFrameTableRow1CellTableRowCell1.Text = _mapper.get("exchange","content");

					//	System.Web.UI.HtmlControls.HtmlInputRadioButton radioButtonHTML = new System.Web.UI.HtmlControls.HtmlInputRadioButton();
					//	System.Web.UI.HtmlControls.HtmlInputRadioButton radioButtonText = new System.Web.UI.HtmlControls.HtmlInputRadioButton();

					//	radioButtonHTML.Name = "style";
					//	radioButtonText.Name = "style";
					//	radioButtonHTML.Value = "html";
					//	radioButtonText.Value = "text";
					//	radioButtonHTML.ID = "htmlstyle";
					//	radioButtonText.ID = "textstyle";
					//	radioButtonHTML.Attributes["onChange"] = "ReloadIFrame('" + frame1.ClientID + "','" + _exchangeFolderID + "','" + _messageID + "','text');";
					//	radioButtonText.Attributes["onChange"] = "ReloadIFrame('" + frame1.ClientID + "','" + _exchangeFolderID + "','" + _messageID + "','html');";
					//radioButtonHTML.Attributes["onselect"] = "ReloadIFrame('" + frame1.ClientID + "','" + _exchangeFolderID + "','" + _messageID + "','html');";
					//radioButtonText.Attributes["onselect"] = "ReloadIFrame('" + frame1.ClientID + "','" + _exchangeFolderID + "','" + _messageID + "','text');";
					/*
						if(style == "html") 
						{
							radioButtonHTML.Checked = true;
						} 
						else if(style == "text")
						{
							radioButtonText.Checked = true;
						} 
						else 
						{
							radioButtonHTML.Checked = true;
						}

						Label label1 = new Label();
						label1.Text = "HTML";
						label1.Attributes["valign"] = "top";
						Label label2 = new Label();
						label2.Text = "Text";
						label2.Attributes["valign"] = "top";

						iFrameTableRow1CellTableRowCell2.Controls.Add(radioButtonHTML);
						iFrameTableRow1CellTableRowCell2.Controls.Add(label1);
						iFrameTableRow1CellTableRowCell2.Controls.Add(radioButtonText);
						iFrameTableRow1CellTableRowCell2.Controls.Add(label2);
						iFrameTableRow1CellTableRowCell2.HorizontalAlign = HorizontalAlign.Right;
					*/
					iFrameTableRow2Cell.Controls.Add(frame1);
				
					splitTabRow.Cells.Add(splitTabCellLeft);
					splitTabRow.Cells.Add(splitTabCellRight);

					splitTab.Rows.Add(splitTabRow);
				}
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

		void DownloadEMLBtn_Click(Object sender, EventArgs e) 
		{
			ExchangeHelper eh = ExchangeHelper.getExchangeHelper(Session, _exchangeFolderID);
			
			//eh.DownloadMessageAsEML(href, "Test3.EML");

			Stream emlStream = eh.getFileStream(href);

			Response.ContentType = "message/rfc822";
			Response.AddHeader("Content-Disposition", "attachment; filename=" + messagename + ".eml");

			byte [] buffer = new byte [4096];
			int len = 0;

			while ((len = emlStream.Read(buffer,0,buffer.Length)) > 0 )
			{
				Response.Flush();
				Response.OutputStream.Write(buffer,0,len);
			
			}
			emlStream.Close();

			Response.End();



		}//DownloadEMLBtn_Click
/*
		void DownloadMSGBtn_Click(Object sender, EventArgs e) 
		{
			//Get Connection to Exchange
			ExchangeHelper eh = ExchangeHelper.getExchangeHelper(Session, _exchangeFolderID);
			
			//Get Tmp-Dir
			string tmpDir = Global.Config.documentTempDirectory;

			//EML-Filename/MSG-Filenam
			eml = tmpDir + "\\" + messagename + ".eml";
			msg = tmpDir + "\\" + messagename + ".msg";

			//Get Stream of EML and write to Tmp-Dir
			Stream emlStream = eh.getFileStream(href);
			FileStream emlFile = new FileStream(eml, FileMode.Create);
			
			byte [] buffer = new byte [4096];
			int len = 0;

			while ((len = emlStream.Read(buffer,0,buffer.Length)) > 0 )
			{
				emlFile.Write(buffer,0,len);
			}

			//http://blogs.msdn.com/mstehle/archive/2007/10/03/fyi-why-are-mapi-and-cdo-1-21-not-supported-in-managed-net-code.aspx
			//Clean up
			emlFile.Close();
			emlStream.Close();

			//Now start Conversion
			//That would be the standard way, but don't ask me why, it just does not work with IIS
			try 
			{
				IRDOSession RDOsession = new RDOSessionClass();
				RDOMail RDOmsg = RDOsession.GetMessageFromMsgFile(msg,true);
				RDOmsg.Import(eml,1024);
				RDOmsg.Save();
			} 
			catch (Exception ex) 
			{
				Logger.Log(ex,Logger.ERROR);
			}
			//It's not hard :)

			
			//Now write out the msg to the Response like in DownloadEMLBtn_Click
			FileStream msgFile = new FileStream(msg, FileMode.Open);

			//There is actually no mime-type like that, I just came up with that
			Response.ContentType = "application/msg";
			Response.AddHeader("Content-Disposition", "attachment; filename=" + messagename + ".msg");

			buffer = new byte [4096];
			len = 0;

			while ((len = msgFile.Read(buffer,0,buffer.Length)) > 0 )
			{
				Response.Flush();
				Response.OutputStream.Write(buffer,0,len);
			
			}//while

			//Don't forget
			msgFile.Close();

			//The Response Stream will be closed by the browser
			Response.End();
		}//DownloadMSGBtn_Click
*/

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
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

		}
		#endregion
    }
}
