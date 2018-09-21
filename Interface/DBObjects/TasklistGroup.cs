using ch.appl.psoft.db;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Summary description for TasklistGroup.
    /// </summary>
    public class TasklistGroup : DBObject
	{
		public TasklistGroup(DBData db, HttpSessionState session) : base(db, session)
		{
		}

		/// <summary>
		/// Zuordnung Tasklistgruppe-Tasklistgruppe zufügen, falls noch keine vorhanden ist
		/// </summary>
		/// <param name="tasklist_groupId"></param>
		/// <param name="tasklistId"></param>
		/// <returns>Id der neuen Zuordnung</returns>
		public long addTasklist_group_tasklist(long tasklist_groupId, long tasklistId) 
		{
			long newId = DBColumn.GetValid(
					_db.lookup(
						"ID", 
						"TASKLIST_GROUP_TASKLIST",
						"TASKLIST_GROUP_ID=" + tasklist_groupId + " and TASKLIST_ID=" + tasklistId
					),
					(long)0
				);

			if (newId == 0 && tasklist_groupId > 0 && tasklistId > 0)
			{
				newId = _db.newId("TASKLIST_GROUP_TASKLIST");
				string sql = "insert into TASKLIST_GROUP_TASKLIST ("
					+ "ID,TASKLIST_GROUP_ID,TASKLIST_ID"
					+ ") values ("
					+ newId + "," + tasklist_groupId + "," + tasklistId
					+ ")";
				_db.execute(sql);
			}
            
			return newId;
		}

		public override int delete(long ID, bool cascade) 
		{
			// für die Child-Tabelle TASKLIST_GROUP_TASKLIST gilt on delete cascade!
			string sql = "delete from TASKLIST_GROUP where id = " + ID;
			return _db.execute(sql);
		}
	}
}
