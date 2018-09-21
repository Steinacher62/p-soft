using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for AddressBuilder.
    /// </summary>
    public class AddressBuilder
	{
        public static void AppendDetail(Table table, string detailLabel, string detailValue){
            if (detailValue != null && detailValue != ""){
                Label label = new Label();
                label.CssClass = "detail_value";
                label.Text = detailValue;
                AppendDetailControl(table, detailLabel, label);
            }
        }
        public static void AppendEmail(Table table, string detailLabel, string detailValue)
        {
            if (detailValue != null && detailValue != "")
            {
                EMailLink elabel = new EMailLink();
                elabel.CssClass = "detail_value";
                elabel.Text = elabel.NavigateUrl = detailValue;
                AppendDetailControl(table, detailLabel, elabel);
            }
        }

        public static void AppendDetailControl(Table table, string detailLabel, Control control){
            Label label = null;
            if (detailLabel != null && detailLabel != ""){
                label = new Label();
                label.CssClass = "detail_label";
                label.Text = detailLabel;
            }
            AppendControls(table, label, control);
        }
			
        public static void AppendControls(Table table, Control controlLabel, Control controlValue){
            if (controlLabel != null || controlValue != null){
                TableCell cell;
                TableRow row = new TableRow();
                table.Rows.Add(row);
                bool hasLabel = false;
                if (controlLabel != null){
                    hasLabel = true;
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(controlLabel);
                }

                cell = new TableCell();
                row.Cells.Add(cell);
                cell.ColumnSpan = hasLabel? 1:2;
                cell.Controls.Add(controlValue);
            }
        }

        public static string GetAddress(DataTable dataTable, int row){
            string address = DBData.getValue(dataTable, row, "ADDRESS1").ToString();
            if (DBData.getValue(dataTable, row, "ADDRESS2").ToString() != "") {
                address += "<br>" + DBData.getValue(dataTable, row, "ADDRESS2").ToString();
            }
            if (DBData.getValue(dataTable, row, "ADDRESS3").ToString() != "") {
                address += "<br>" + DBData.getValue(dataTable, row, "ADDRESS3").ToString();
            }
            return address;
        }

        public static void AppendAddressBlock(Table table, DataTable data, int rowIndex){
            AppendDetail(table, null, GetAddress(data, rowIndex));
            string zipCity = DBData.getValue(data, rowIndex, "COUNTRY").ToString();
            zipCity += ((zipCity != "")? " - " : "") + DBData.getValue(data, rowIndex, "ZIP").ToString() + " " + DBData.getValue(data, rowIndex, "CITY").ToString();
            AppendDetail(table, null, zipCity);
        }

        /// <summary>
        /// append the address information to the person detail view
        /// </summary>
        /// <param name="table"></param>
        /// <param name="data"></param>
        /// <param name="rowIndex"></param>
        /// <param name="mapper"></param>
       
        
        public static void AppendAddressExtensions(Table table, DataTable data, int rowIndex, LanguageMapper mapper){
            AppendDetail(table, mapper.get("ADDRESS","PHONE"), DBData.getValue(data, rowIndex, "PHONE").ToString());
            
            // if AHB get Fieldname (if no data) Fields phone, mobile and E-Mail
            if ((Global.isModuleEnabled("ahb")) & (DBData.getValue(data, rowIndex, "PHONE").ToString() == ""))
                {
                    AppendDetail(table, mapper.get("ADDRESS", "PHONE"), " ");
                }
            AppendDetail(table, mapper.get("ADDRESS","FAX"), DBData.getValue(data, rowIndex, "FAX").ToString());
            AppendDetail(table, mapper.get("ADDRESS", "MOBILE"), DBData.getValue(data, rowIndex, "MOBILE").ToString());
            if ((Global.isModuleEnabled("ahb")) & (DBData.getValue(data, rowIndex, "MOBILE").ToString() == "") & (DBData.getValue(data, rowIndex, "PERSON_ID").ToString() != ""))
                
                {
                    AppendDetail(table, mapper.get("ADDRESS", "MOBILE"), " ");
                }
            AppendEmail(table, mapper.get("ADDRESS", "EMAIL_PRIVATE"), DBData.getValue(data, rowIndex, "EMAIL_PRIVATE").ToString());
            if ((Global.isModuleEnabled("ahb")) & (DBData.getValue(data, rowIndex, "EMAIL_PRIVATE").ToString() == ""))
                {
                    AppendDetail(table, mapper.get("ADDRESS", "EMAIL_PRIVATE"), " ");
                }
            if (Global.isModuleEnabled("ahb"))
            {

            }
        }

        public static void BuildAddressTable(Table addressTable, DataTable data, int rowIndex, LanguageMapper mapper)
        {
            Table table;
            TableRow addressRow, row;
            TableCell cell;

            // horizontal line,
            row = new TableRow();
            addressTable.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.ColumnSpan = 2;
            cell.Height = Unit.Pixel(1);
            cell.Width = Unit.Percentage(100);
            cell.BackColor = System.Drawing.Color.FromArgb(0xb4, 0xbb, 0xcf);

            addressRow = new TableRow();
            addressTable.Rows.Add(addressRow);

            // Address
            cell = new TableCell();
            addressRow.Cells.Add(cell);
            cell.VerticalAlign = VerticalAlign.Top;
            table = new Table();
            cell.Controls.Add(table);
            table.CellSpacing=0;
            table.CellPadding=1;
            AppendAddressBlock(table, data, rowIndex);

            // Phone, Fax
            cell = new TableCell();
            addressRow.Cells.Add(cell);
            cell.VerticalAlign = VerticalAlign.Top;
            table = new Table();
            cell.Controls.Add(table);
            table.CellSpacing=0;
            table.CellPadding=1;
            AppendAddressExtensions(table, data, rowIndex, mapper);
        }

    }
}
