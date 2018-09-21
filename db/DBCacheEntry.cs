using ch.psoft.Util;
using System;
using System.Threading;

namespace ch.appl.psoft.db
{

    /// <summary>
    /// Base-class providing caching-functionalities for database data.
    /// </summary>
    public abstract class DBCacheEntry {
        private long _stale_ticks = 0;
        private long _timeStamp = 0;
        private Thread _refreshThread = null;
        private DBData _db = null;
        private static int _threadNr = 1;

        private DBCacheEntry(){}

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="staleSeconds">Number of seconds after that the cache-entry becomes stale. If 0 the entry remains fresh forever.</param>
        public DBCacheEntry(long staleSeconds) {
            _stale_ticks = staleSeconds * 1000 * 1000 * 10;
        }

        /// <summary>
        /// Returns true if the cache-entry is stale.
        /// </summary>
        public bool IsStale{
            get{
                lock (this){
                    return (_timeStamp == 0) || ((_stale_ticks > 0) && (_timeStamp + _stale_ticks < DateTime.Now.Ticks));
                }
            }
        }

        /// <summary>
        /// The onRefreshing() method must be overriden and takes care of refreshing the cache-entry.
        /// It gets called only if the entry is stale.
        /// </summary>
        /// <param name="db">DBData with open connection</param>
        protected abstract void onRefreshing(DBData db);

        /// <summary>
        /// Call this method in property-accessors to ensure the freshness of the cache-entry!
        /// </summary>
        /// <param name="db">DBData with open connection</param>
        protected void refreshIfStale(DBData db){
            lock (this){
                if (IsStale){
                    onRefreshing(db);
                    _timeStamp = DateTime.Now.Ticks;
                }
            }
        }

        /// <summary>
        /// Sets the cache-entry as stale.
        /// </summary>
        public void setStale(){
            lock (this){
                _timeStamp = 0;
            }
        }

        /// <summary>
        /// Loads the cache-entry in a separate thread.
        /// </summary>
        /// <param name="db">DBData without open connection</param>
        public void refreshAsynchronous(DBData db){
            lock (this){
                _db = db;
                setStale();
                _refreshThread = new Thread(new ThreadStart(refreshingThread));
                _refreshThread.Priority = ThreadPriority.BelowNormal;
                _refreshThread.Name = "Cache_" + _threadNr++;
                _refreshThread.Start();
            }
        }

        /// <summary>
        /// Thread-function to refresh the cache-entry asynchronously.
        /// </summary>
        private void refreshingThread(){
            lock (this){
                _db.connect();
                try{
                    Logger.Log("starting asynchronous refresh...", Logger.DEBUG);
                    refreshIfStale(_db);
                    Logger.Log("... asynchronous refresh finished.", Logger.DEBUG);
                }
                catch(Exception ex){
                    Logger.Log(ex, Logger.ERROR);
                }
                finally{
                    _db.disconnect();
                }
            }
        }
    }
}
