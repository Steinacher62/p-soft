using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Summary description for SubNavMenu.
    /// </summary>
    public partial class SubNavMenu : PsoftMenuPage {
        private const string PAGE_URL = "/Lohn/SubNavMenu.aspx";

        static SubNavMenu() {
            SetPageParams(PAGE_URL, "moduleName");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            // GetQueryValue() hat hier nicht funktioniert (?)
            LohnModule.KundenModuleName
                = ch.psoft.Util.Validate.GetValid(Request.QueryString["moduleName"], "");

            // Setting main page layout
            MenuPageLayout pageLayout = (MenuPageLayout) LoadPSOFTControl(MenuPageLayout.Path, "_pl");
            PageLayoutControl = pageLayout;

            // Setting content layout of page layout
            SimpleContentLayout contentLayout = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting detail parameters
            MenuControl ctrl = (MenuControl)this.LoadPSOFTControl(MenuControl.Path, "_ctrl");
            ctrl.Title = _mapper.get("ModuleNames", LohnModule.KundenModuleName);

            // Infos aus DB
            bool budgetBearbeitung = LohnModule.BudgetModusConfig >= (int)LohnModule.BudgetModus.budgetImport;
            DataTable budgettypTable = null;
            bool adminRight = false;
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                long memberAccessorId = db.getAccessorID(SessionData.getUserID(Session));
                long groupAccessorId = DBColumn.GetValid(
                    db.lookup("ID", "ACCESSOR", "TITLE = 'Administratoren'"),
                    (long)-1
                    );
                adminRight = db.isAccessorGroupMember(memberAccessorId, groupAccessorId, true);

                budgettypTable = db.getDataTableExt(
                    "select ID, BEZEICHNUNG from BUDGETTYP order by BEZEICHNUNG",
                    "BUDGETTYP"
                    );
            }
            finally {
                db.disconnect();
            }
                
            if (budgetBearbeitung) {
                ctrl.addMenuGroup(null, "budgetGroup", _mapper.get("lohn", "budgetGroup"));

                if (budgettypTable.Rows.Count == 0) {
                    foreach (string component in LohnModule.EditableComponentList) {
                        ctrl.addMenuItem(
                            "budgetGroup",
                            component + "BudgetLink",
                            _mapper.get(LohnModule.KundenModuleName, component + "Link"),
                            Main.GetURL("salaryComponent", component, "context", "budget")
                            );
                    }
                }
                else {
                    foreach (string component in LohnModule.EditableComponentList) {
                        ctrl.addMenuGroup(
                            "budgetGroup",
                            component + "BudgetGroup",
                            _mapper.get(LohnModule.KundenModuleName, component + "Link")
                            );

                        foreach (DataRow row in budgettypTable.Rows) {
                            ctrl.addMenuItem(
                                component + "BudgetGroup",
                                component + row["BEZEICHNUNG"] + "BudgetLink",
                                row["BEZEICHNUNG"].ToString(),
                                Main.GetURL(
                                "salaryComponent", component,
                                "context", "budget",
                                "budgettypId", row["ID"]
                                )
                                );
                        }
                    }
                }
            }

            ctrl.addMenuGroup(null, "editingGroup", _mapper.get("lohn", "editingGroup"));
                
            if (budgettypTable.Rows.Count == 0) {
                foreach (string component in LohnModule.EditableComponentList) {
                    ctrl.addMenuItem(
                        "editingGroup",
                        component + "Link",
                        _mapper.get(LohnModule.KundenModuleName, component + "Link"),
                        Main.GetURL("salaryComponent", component, "context", "adjustment")
                        );
                }
            }
            else {
                foreach (string component in LohnModule.EditableComponentList) {
                    ctrl.addMenuGroup(
                        "editingGroup",
                        component + "EditingGroup",
                        _mapper.get(LohnModule.KundenModuleName, component + "Link")
                        );

                    foreach (DataRow row in budgettypTable.Rows) {
                        ctrl.addMenuItem(
                            component + "EditingGroup",
                            component + row["BEZEICHNUNG"] + "BudgetLink",
                            row["BEZEICHNUNG"].ToString(),
                            Main.GetURL(
                            "salaryComponent", component,
                            "context", "adjustment",
                            "budgettypId", row["ID"]
                            )
                            );
                    }
                }
            }

            if (LohnModule.MitGenehmigungsverfahren) {
                ctrl.addMenuGroup(null, "genehmigungGroup", _mapper.get("lohn", "genehmigungGroup"));
                    
                foreach (string component in LohnModule.EditableComponentList) {
                    ctrl.addMenuItem(
                        "genehmigungGroup",
                        component + "GenehmigungLink",
                        _mapper.get(LohnModule.KundenModuleName, component + "Link"),
                        Main.GetURL("salaryComponent", component, "context", "approvement")
                        );
                }

                ctrl.addMenuGroup(null, "budgetCheckGroup", _mapper.get("lohn", "budgetCheckGroup"));

                if (budgettypTable.Rows.Count == 0) {
                    foreach (string component in LohnModule.EditableComponentList) {
                        ctrl.addMenuItem(
                            "budgetCheckGroup",
                            component + "BudgetCheckLink",
                            _mapper.get(LohnModule.KundenModuleName, component + "Link"),
                            Main.GetURL("salaryComponent", component, "context", "budgetcheck")
                            );
                    }
                }
                else {
                    foreach (string component in LohnModule.EditableComponentList) {
                        ctrl.addMenuGroup(
                            "budgetCheckGroup",
                            component + "BudgetCheckGroup",
                            _mapper.get(LohnModule.KundenModuleName, component + "Link")
                            );

                        foreach (DataRow row in budgettypTable.Rows) {
                            ctrl.addMenuItem(
                                component + "BudgetCheckGroup",
                                component + row["BEZEICHNUNG"] + "BudgetCheckLink",
                                row["BEZEICHNUNG"].ToString(),
                                Main.GetURL(
                                "salaryComponent", component,
                                "context", "budgetcheck",
                                "budgettypId", row["ID"]
                                )
                                );
                        }
                    }
                }
            }

            ctrl.addMenuGroup(null, "overviewGroup", _mapper.get("lohn", "overviewGroup"));

            foreach (string component in LohnModule.EditableComponentList) {
                ctrl.addMenuItem(
                    "overviewGroup",
                    component + "OverviewLink",
                    _mapper.get(LohnModule.KundenModuleName, component + "Link"),
                    Main.GetURL("salaryComponent", component, "context", "overview")
                    );
            }

            if (adminRight) {
                ctrl.addMenuGroup(null, "administrationGroup", _mapper.get("lohn", "administrationGroup"));
                
                foreach (string component in LohnModule.EditableComponentList) {
                    ctrl.addMenuItem(
                        "administrationGroup",
                        component + "Administration",
                        _mapper.get(LohnModule.KundenModuleName, component + "Link"),
                        Admin.GetURL("salaryComponent", component)
                        );
                }

                ctrl.addMenuItem(
                    "administrationGroup",
                    "importExport",
                    _mapper.get("lohn", "importExportLink"),
                    ImportExport.GetURL()
                    );
            }

            //Setting content layout user controls
            SetPageLayoutContentControl(SimpleContentLayout.CONTENT, ctrl);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
        }
		#endregion
    }
}
