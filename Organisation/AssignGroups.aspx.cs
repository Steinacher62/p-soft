using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Organisation.Controls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Organisation
{
    /// <summary>
    /// Summary description for AssignGroups.
    /// </summary>
    public partial class AssignGroups : PsoftTreeViewPage
	{
		protected void Page_Load(object sender, System.EventArgs e) 
		{
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			AssignGroupTree _assTree = (AssignGroupTree)this.LoadPSOFTControl(AssignGroupTree.Path, "_assTree");
            _assTree.XID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"], 0);
            _assTree.BackUrl = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"],"");
            _assTree.NextUrl = ch.psoft.Util.Validate.GetValid(Request.QueryString["nextURL"],"");
            _assTree.OwnerTable = ch.psoft.Util.Validate.GetValid(Request.QueryString["ownerTable"], "");
            _assTree.OwnerID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ownerID"], -1L);

            //Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, _assTree);		

            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {

                switch (_assTree.OwnerTable)
                {
                    case "TASKLIST":
                        PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get("tasklist","oeAssignTasklistGroup");
                        _assTree.TableName = ch.psoft.Util.Validate.GetValid(Request.QueryString["table"], "TASKLIST_GROUP_TASKLIST");
                        break;
                    case "CONTACTV":
                        string contactTable = DBColumn.GetValid(db.lookup("TABLENAME",_assTree.OwnerTable,"ID="+_assTree.OwnerID),"");
                        if (contactTable != "")
                        {
                            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get("contact","oeAssignContactGroup");
                            _assTree.TableName = ch.psoft.Util.Validate.GetValid(Request.QueryString["table"], "CONTACT_GROUP_"+contactTable);
                        }
                        break;
                    case "PROJECT":
                        PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get("project","oeAssignProjectGroup");
                        _assTree.TableName = ch.psoft.Util.Validate.GetValid(Request.QueryString["table"], "PROJECT_GROUP_PROJECT");
                        break;
                }
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }

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
