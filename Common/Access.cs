using System.Data;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Handler prüft row-access access
    /// </summary>
    public delegate bool RowAccessHandler(DataTable table, DataRow row, bool isRowAccessPermitted, int requestedAuthorisation);
    /// <summary>
    /// Summary description for Access.
    /// </summary>
    public abstract class Access {
        /// <summary>
        /// Row access event
        /// </summary>
        public event RowAccessHandler onRowAccess = null;

        protected bool rowAccess (DataTable table, DataRow row, bool isRowAccessPermitted, int requestedAuthorisation) {
            if (onRowAccess != null) {
                // Invokes the delegates. 
                return onRowAccess(table, row, isRowAccessPermitted, requestedAuthorisation);
            }
            return isRowAccessPermitted;
        }
    }
}
