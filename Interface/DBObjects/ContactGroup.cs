using ch.appl.psoft.db;
using System.Data;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for ContactGroup.
    /// </summary>
    public class ContactGroup  : DBObject 
	{
		public ContactGroup(DBData db, HttpSessionState session) : base(db, session)
		{
		}

		/// <summary>
		/// Kopiert eine Kontaktgruppe mit zu gehörigen Zuordnungen
		/// </summary>
		/// <param name="Id">Original</param>
		/// <param name="targetId">Zielkontaktgruppe (falls > 0, werden nur die Zuteilungen kopiert)</param>
		/// <returns>Id der Kopie</returns>
		public long copy(long Id, long targetId) 
		{
			long newId = targetId;

			if (targetId <= 0)
			{
				newId = _db.newId("CONTACT_GROUP");
				string colNames = "," + _db.getColumnNames("CONTACT_GROUP") + ",";
				string attrs = colNames;
	            
				attrs = attrs.Replace(",ID,", "," + newId + ",");
				attrs = attrs.Replace(",EXTERNAL_REF,", ",");
				colNames = colNames.Replace(",EXTERNAL_REF,", ",");
				attrs = attrs.Replace(",UID,", ",");
				colNames = colNames.Replace(",UID,", ",");

				attrs = attrs.Substring(1, attrs.Length - 2);
				colNames = colNames.Substring(1, colNames.Length - 2);
				string sql = "insert into CONTACT_GROUP (" + colNames + ")"
					+ " select " + attrs + " from CONTACT_GROUP where ID=" + Id;
	               
				_db.execute(sql);
			}

			long assignId = 0;
			DataTable table = _db.getDataTable(
					"select distinct S.FIRM_ID"
						+ " from CONTACT_GROUP_FIRM S"
						+ " where S.CONTACT_GROUP_ID=" + Id
						+ " and not exists ("
						+ " select 1 from CONTACT_GROUP_FIRM T"
						+ " where S.FIRM_ID = T.FIRM_ID"
						+ " and T.CONTACT_GROUP_ID=" + newId
						+ ")"
				);

			foreach (DataRow row in table.Rows) 
			{
				assignId = DBColumn.GetValid(row[0], (long)0);
				addContact_group_firm(newId, assignId);
			}

			table = _db.getDataTable(
					"select distinct S.PERSON_ID"
						+ " from CONTACT_GROUP_PERSON S"
						+ " where S.CONTACT_GROUP_ID=" + Id
						+ " and not exists ("
						+ " select 1 from CONTACT_GROUP_PERSON T"
						+ " where S.PERSON_ID = T.PERSON_ID"
						+ " and T.CONTACT_GROUP_ID=" + newId
						+ ")"
				);

			foreach (DataRow row in table.Rows) 
			{
				assignId = DBColumn.GetValid(row[0], (long)0);
				addContact_group_person(newId, assignId);
			}
            
			return newId;
		}

		/// <summary>
		/// Zuordnung Firma-Kontaktgruppe zufügen, falls noch keine vorhanden ist
		/// </summary>
		/// <param name="contact_groupId"></param>
		/// <param name="firmId"></param>
		/// <returns>Id der neuen Zuordnung</returns>
		public long addContact_group_firm(long contact_groupId, long firmId) 
		{
			long newId = DBColumn.GetValid(
					_db.lookup(
						"ID", 
						"CONTACT_GROUP_FIRM",
						"CONTACT_GROUP_ID=" + contact_groupId + " and FIRM_ID=" + firmId
					),
					(long)0
				);

			if (newId == 0 && contact_groupId > 0 && firmId > 0)
			{
				newId = _db.newId("CONTACT_GROUP_FIRM");
				string sql = "insert into CONTACT_GROUP_FIRM ("
					+ "ID,CONTACT_GROUP_ID,FIRM_ID"
					+ ") values ("
					+ newId + "," + contact_groupId + "," + firmId
					+ ")";
				_db.execute(sql);
			}
            
			return newId;
		}

		/// <summary>
		/// Zuordnung Person-Kontaktgruppe zufügen, falls noch keine vorhanden ist
		/// </summary>
		/// <param name="contact_groupId"></param>
		/// <param name="personId"></param>
		/// <returns>Id der neuen Zuordnung</returns>
		public long addContact_group_person(long contact_groupId, long personId) 
		{
			long newId = DBColumn.GetValid(
					_db.lookup(
						"ID", 
						"CONTACT_GROUP_PERSON",
						"CONTACT_GROUP_ID=" + contact_groupId + " and PERSON_ID=" + personId
					),
					(long)0
				);

			if (newId == 0 && contact_groupId > 0 && personId > 0)
			{
				newId = _db.newId("CONTACT_GROUP_PERSON");
				string sql = "insert into CONTACT_GROUP_PERSON ("
					+ "ID,CONTACT_GROUP_ID,PERSON_ID"
					+ ") values ("
					+ newId + "," + contact_groupId + "," + personId
					+ ")";
				_db.execute(sql);
			}
            
			return newId;
		}

		public override int delete(long ID, bool cascade) 
		{
			// für die Child-Tabellen CONTACT_GROUP_FIRM und CONTACT_GROUP_PERSON gilt
			// on delete cascade!
			string sql = "delete from CONTACT_GROUP where id = " + ID;
			return _db.execute(sql);
		}
	}
}
