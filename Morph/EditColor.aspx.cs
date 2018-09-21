using ch.appl.psoft.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Morph
{
    public partial class EditColor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData data = DBData.getDBData(this.Session);
            string characteristicID = Request.QueryString["characteristicid"];
            string matrixID = Request.QueryString["matrixid"];
            string setID = Request.QueryString["setid"];

            data.connect();

            if (setID == "" || setID == null)
            {
                // erster Seitenaufruf, Optionen anzeigen

                // verfügbare Farben auflisten
                List<MatrixColor> farben = new List<MatrixColor>();

                DataTable colortable = data.getDataTable("SELECT ID, TITLE, COLOR FROM COLORATION WHERE MATRIX_ID = " + matrixID, new object[] { "COLORATION" });

                for (int idx = 0; idx < colortable.Rows.Count; idx++)
                {
                    farben.Add(new MatrixColor(Convert.ToInt32(colortable.Rows[idx]["ID"]), colortable.Rows[idx]["TITLE"].ToString(), Convert.ToInt32(colortable.Rows[idx]["COLOR"])));
                }

                Table auswahl = new Table();

                foreach (MatrixColor aktColor in farben)
                {
                    TableRow aktRow = new TableRow();
                    TableCell aktCell = new TableCell();
                    aktCell.Style.Add("background-color", aktColor.toCSS());
                    aktCell.Text = "<a href=\"editcolor.aspx?characteristicid=" + characteristicID + "&matrixid=" + matrixID + "&setid=" + aktColor.id + "\">" + aktColor.title + "</a>";
                    aktRow.Cells.Add(aktCell);
                    auswahl.Rows.Add(aktRow);
                }

                lblOutput.Controls.Add(auswahl);
            }
            else
            {
                // Farbe gewählt, speichern und Bestätigung anzeigen

                data.execute("UPDATE CHARACTERISTIC SET COLOR_ID = " + setID + " WHERE ID = " + characteristicID + ";", new int[] { });

                lblOutput.Text = "Auswahl gespeichert<br><br>";
                lblOutput.Text += "<a href=\"\" onclick=\"javascript:var URL = unescape(window.opener.location); window.opener.location.href = URL; window.close();\">schliessen</a>";

                lblHead.Text = "<script type=\"text/javascript\">var URL = unescape(window.opener.location); window.opener.location.href = URL; window.close();</script>";

            }

            data.disconnect();
        }
    }
}
