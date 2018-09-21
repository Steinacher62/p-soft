using System.Collections.Generic;

namespace ch.appl.psoft.Interface
{
    public class User
    {
        /// <summary> 
        /// Property to get/set ProfileId 
        /// </summary> 
        public string ProfileId
        {
            get;
            set;
        }

        /// <summary> 
        /// Propoerty to get/set multiple ConnectionId 
        /// </summary> 
        public HashSet<string> ConnectionIds
        {
            get;
            set;
        }
    }
}