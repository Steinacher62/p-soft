using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Data;

namespace ch.appl.psoft.Contact
{
    /// <summary>
    /// Summary description for ContactModule.
    /// </summary>
    public class ContactModule : psoftModule
	{
        public const string LANG_SCOPE_CONTACT                  = "contact";

        public const string LANG_MNEMO_SEARCHCONTACTCONTACT     = "searchContactContact";
        public const string LANG_MNEMO_SEARCHCONTACTJOURNAL     = "searchContactJournal";
        public const string LANG_MNEMO_CONTACTKIND_PERSON       = "contactKindPerson";
        public const string LANG_MNEMO_CONTACTKIND_FIRM         = "contactKindFirm";
        public const string LANG_MNEMO_ADDEDITCONTACTCONTACT    = "addEditContactContact";
        public const string LANG_MNEMO_ADDEDITCONTACTFIRM       = "addEditContactFirm";
        public const string LANG_MNEMO_ADDEDITCONTACTADDRESS    = "addEditContactAddress";

        //breadcrumb captions
        public const string LANG_MNEMO_BCSEARCHCONTACT          = "bcSearchContact";

        public const string LANG_MNEMO_BCADDCONTACT             = "bcAddContact";
        public const string LANG_MNEMO_BCADDFIRM                = "bcAddFirm";

        public const string LANG_MNEMO_BCEDITCONTACT            = "bcEditContact";
        public const string LANG_MNEMO_BCEDITFIRM               = "bcEditFirm";

        public const string LANG_MNEMO_BC_ADDCONTACTGROUP       = "bcAddContactGroup";
        public const string LANG_MNEMO_BC_ADDJOURNAL            = "bcAddJournal";

        public const string LANG_MNEMO_BC_EDITCONTACTGROUP      = "bcEditContactGroup";
        public const string LANG_MNEMO_BC_EMPTYCONTACTGROUP     = "bcEmptyContactGroup";
        public const string LANG_MNEMO_BC_NOCONTACTGROUP        = "bcNoContactGroup";

        //page title
        public const string LANG_MNEMO_PTSEARCHCONTACT          = "ptSearchContact";
        public const string LANG_MNEMO_PTSEARCHCONTACTLIST      = "ptSearchContactList";

        //controls title
        public const string LANG_MNEMO_CTSEARCHCONTACTLIST      = "ctContactListSearchResult";
        public const string LANG_MNEMO_CTCONTACTGROUPLIST       = "ctContactGroupList";
        public const string LANG_MNEMO_CTFIRMCONTACTLIST        = "ctFirmContactList";
        public const string LANG_MNEMO_CTPERSONCONTACTGROUPS    = "ctPersonContactGroups";
        public const string LANG_MNEMO_CT_CONTACTJOURNALLIST    = "ctContactJournalList";

        //context-menus
        public const string LANG_MNEMO_CM_EDIT_CONTACT          = "cmEditContact";
        public const string LANG_MNEMO_CM_DELETE_CONTACT        = "cmDeleteContact";
        public const string LANG_MNEMO_CM_DELETE_FROM_GROUP     = "cmDeleteFromGroup";
        public const string LANG_MNEMO_CM_DELETE_FROM_SELECTION = "cmDeleteFromSelection";
        public const string LANG_MNEMO_CM_NEW_JOURNAL           = "cmNewJournal";
        public const string LANG_MNEMO_CM_LIST_JOURNAL          = "cmListJournal";
        public const string LANG_MNEMO_CM_NEW_CONTACTPERSON     = "cmNewContactPerson";
        public const string LANG_MNEMO_CM_LIST_CONTACTPERSONS   = "cmListContactPersons";
        public const string LANG_MNEMO_CM_LIST_CONTACTS         = "cmListContacts";
        public const string LANG_MNEMO_CM_SAVE_AS_GROUP         = "cmSaveAsGroups";
        public const string LANG_MNEMO_CM_EDIT_CONTACTGROUP     = "cmEditContactGroup";
        public const string LANG_MNEMO_CM_SERIAL_LETTER         = "cmSerialLetter";
        public const string LANG_MNEMO_CM_SERIAL_EMAIL          = "cmSerialEmail";
        public const string LANG_MNEMO_CM_CREATE_TASKLIST       = "cmCreateTasklist";
        public const string LANG_MNEMO_CM_ADD_TO_GROUP          = "cmAddToGroup";
        public const string LANG_MNEMO_CM_ADD_TO_SELECTION      = "cmAddToSelection";

        //context-menu title
        public const string LANG_MNEMO_CMT_CONTACTPERSON        = "cmtContactPerson";
        public const string LANG_MNEMO_CMT_CONTACTFIRM          = "cmtContactFirm";
        public const string LANG_MNEMO_CMT_CONTACTGROUP         = "cmtContactGroup";
        public const string LANG_MNEMO_CMT_ALLCONTACTGROUPS     = "cmtAllContactGroups";
        public const string LANG_MNEMO_CMT_SELECTION_CONTACTS   = "cmtSelectionContacts";
        public const string LANG_MNEMO_CMT_GROUP_CONTACTS       = "cmtGroupContacts";
        public const string LANG_MNEMO_CMT_FIRM_CONTACTS        = "cmtFirmContacts";
        public const string LANG_MNEMO_CMT_DISPATCH             = "cmtDispatch";
        public const string LANG_MNEMO_CMT_TASKLISTS            = "cmtTasklists";

        //navigation-menu
        public const string LANG_MNEMO_SUBNAV_TITLE             = "subNavTitle";
        public const string LANG_MNEMO_SUBNAV_SEARCHCONTACT     = "subNavSearchContact";
        public const string LANG_MNEMO_SUBNAV_MYCONTACTGROUPS   = "subNavMyContactGroups";
        public const string LANG_MNEMO_SUBNAV_NEWCONTACTGROUP   = "subNavNewContactGroup";
        public const string LANG_MNEMO_SUBNAV_NEWCONTACTPERSON  = "subNavNewContactPerson";
        public const string LANG_MNEMO_SUBNAV_NEWCONTACTCOMPANY = "subNavNewContactCompany";

        // type
        public const string TYPE_PERSON = "person";
        public const string TYPE_FIRM   = "firm";

        public ContactModule() : base()
        {
            m_NameMnemonic = "contact";
            m_StartURL = psoft.Contact.ContactSearch.GetURL();
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode)
        {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Contact/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public static DataTable getContactTypes(DBData db)
        {
            return db.getDataTable("select ID, " + db.langAttrName("CONTACT_TYPE", "TITLE") + " from CONTACT_TYPE order by " + db.langAttrName("CONTACT_TYPE", "TITLE"));
        }

        public static DataTable getJournaltTypes(DBData db)
        {
            return db.getDataTable("select ID, " + db.langAttrName("JOURNAL_TYPE", "TITLE") + " from JOURNAL_TYPE order by " + db.langAttrName("JOURNAL_TYPE", "TITLE"));
        }

        public static DataTable getContactRoles(DBData db)
        {
            return db.getDataTable("select ID, " + db.langAttrName("CONTACT_ROLE", "TITLE") + " from CONTACT_ROLE order by " + db.langAttrName("CONTACT_ROLE", "TITLE"));
        }

        public static DataTable getContactKinds(DBData db, LanguageMapper map)
        {
            return db.getDataTable("select null  as ID, '' as TITLE union select 'PERSON', '" +  map.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CONTACTKIND_PERSON) + "' union select 'FIRM', '" +  map.get(ContactModule.LANG_SCOPE_CONTACT, ContactModule.LANG_MNEMO_CONTACTKIND_FIRM) + "' order by TITLE");
        }
    
        public static DataTable getContactFirms(DBData db)
        {
            return db.getDataTable("select ID, TITLE from FIRM where (TYP&2)>0 order by TITLE");
        }
    }
}
