using ch.appl.psoft.db;
using ch.appl.psoft.Dispatch;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Globalization;
using System.Timers;
using System.Web;

namespace ch.appl.psoft.Subscription
{
    using Interface.DBObjects;

    /// <summary>
    /// Summary description for MBO.
    /// </summary>
    public class NewsModule : psoftModule {
        private static Timer NewsMailServer = null;
        private LanguageMapper _applicationMap = null;
        private ElapsedEventHandler _emailHandler = null;
        private DBData _db = null;

        public NewsModule() : base()  {
            m_NameMnemonic = "subscription";
            m_IsVisible = false;
            m_StartURL = "";
        }

        public override void LoadLanguageFile(LanguageMapper map, string languageCode) {
            map.load(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Subscription/XML/language_" + languageCode + ".xml", languageCode, false);
        }

        public override void Application_Start(HttpApplicationState application) {
            _applicationMap = LanguageMapper.getLanguageMapper(application);
            if (NewsMailServer == null) {
                NewsMailServer = new Timer();
                // Set the Interval to 5 min.
                NewsMailServer.Interval = 5 * 60 * 1000;
//                NewsMailServer.Interval = 10 * 1000;
                NewsMailServer.AutoReset = true;
            }
            if (_emailHandler == null) _emailHandler = new ElapsedEventHandler(sendMail);
            NewsMailServer.Elapsed += _emailHandler;
            NewsMailServer.Enabled = Global.Config.isModuleEnabled("dispatch");
        }
        public override void Application_End(HttpApplicationState application) {
            if (NewsMailServer != null) {
                NewsMailServer.Enabled = false;
                NewsMailServer.Elapsed -= _emailHandler;
            }
        }
        private void sendMail(object source, ElapsedEventArgs e) {
            _db = DBData.getDBData();
            string from  = Global.Config.email;

            lock (NewsMailServer) {
                _db.connect();
                try {
                    string template = AppDomain.CurrentDomain.BaseDirectory + "Vorlagen/News.doc";
                    string fromSql = "from (news n inner join newsassign a on n.id = a.news_id) inner join subscription s on a.subscription_id = s.id where n.flag = 0 and s.emailenable = 1 and s.typ = 0"
                        +" and n.visibility = 1 and isnull(n.valid_from,getdate()) <= getdate() and isnull(n.valid_to,getdate()) >= getdate()"
                        +" and s.active = 1 and isnull(s.valid_from,getdate()) <= getdate() and isnull(s.valid_to,getdate()) >= getdate()";
                    string order = " order by n.created asc";
                    DataTable table = _db.getDataTable("select n.*,s.email TOEMAIL,s.person_id TOID "+fromSql+order);
                    MergeAndMail merge = new MergeAndMail(template,
                        "Subject",
                        "Comment",
                        "Title",
                        "Event",
                        "Email"
                        );
                    merge.sendMail += new SendMailHandler(mailSended);

                    if (table.Rows.Count > 0) {
                        foreach (DataRow row in table.Rows) {
                            LanguageMapper toPersonMap = _applicationMap;
                            CultureInfo toPersonCulture = _db.dbColumn.UserCulture;
                            long toID = DBColumn.GetValid(row["TOID"], -1L);
                            string languageCode = Person.getLanguageCode(toID);
                            if (languageCode != "" && languageCode != toPersonMap.LanguageCode){
                                // load person's LanguageMapper...
                                toPersonMap = new LanguageMapper();
                                Global.reloadLanguageMapper(toPersonMap, languageCode);
                                string regionCode = Person.getRegionCode(toID);
                                if (regionCode != ""){
                                    toPersonCulture = new CultureInfo(languageCode + "-" + regionCode);
                                }
                            }

                            // send mail to
                            string to = Global.Config.getModuleParam("dispatch","testEmailAddress","");
                            if (to == "") {
                                to = DBColumn.GetValid(row["TOEMAIL"],"");
                                if (to == "") to = _db.lookup("EMAIL","PERSON","ID="+toID,false);
                                if (to != "") {
                                    string address = _db.lookup("PNAME+' '+FIRSTNAME","PERSON","ID="+toID,false);
                                    to = "\"" + address + "\" <" + to + ">";                                    
                                }
                            }
                            if (to == "") continue;

                            int ev = DBColumn.GetValid(row["EVENT"],0);
                            string evStr = toPersonMap.get("news",((News.ACTION) ev).ToString());
                            string triggerName = toPersonMap.get("news", _db.News.getTriggerName(row));
                            string title = triggerName +": "+_db.News.getTriggerValue(row);
                            long uid = _db.News.getTriggerUID(row);
                            if (uid > 0) {
                                title += " http://";
                                title += Global.Config.domain;
                                title += psoft.Goto.GetURL("uid",uid);
                            }

                            if (!DBColumn.IsNull(row["CREATED"])) evStr += ((DateTime)row["CREATED"]).ToString("d", toPersonCulture);
                            evStr += " (";
                            evStr += _db.Person.getWholeName(row["PERSON_ID"].ToString(), false);
                            evStr += ")";
                            merge.AddRecord(DBColumn.GetValid(row["ID"],0L),
                                toPersonMap.get("news", "emailSubject"),
                                toPersonMap.get("news", "emailComment"),
                                title,
                                evStr,
                                to
                                );
                        }
                        merge.ExecuteMailing(Global.Config.getModuleParam("dispatch","smtpServer",""), from, "Email", "Subject");
                        _db.execute("delete from news where id in (select n.id "+fromSql+")");
                    }
                }
                catch (Exception ex) {
                    Logger.Log(ex,Logger.ERROR);
                }
                finally {
                    _db.disconnect();
                }
            }
        }
        private void mailSended(object state) {
            if (state != null && state is long){
                _db.execute("update news set flag=1 where id=" + (long)state);
            }
        }
    }
}
