using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.psoft.Util;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace ch.appl.psoft.Interface
{
    public class psoftHub : Hub
    {
        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>(StringComparer.InvariantCultureIgnoreCase);
        public string GetMessage(string message)
        {
            return message;
        }


        public override Task OnConnected()
        {

            string profileId = HttpContext.Current.User.Identity.Name;

            string connectionId = Context.ConnectionId;
            var user = Users.GetOrAdd(profileId, _ => new User
            {
                ProfileId = profileId,
                ConnectionIds = new HashSet<string>()
            });
            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                Groups.Add(connectionId, user.ProfileId);
            }
            return base.OnConnected();

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            string profileId = Context.User.Identity.Name;
            User user;

            Users.TryGetValue(profileId, out user);
            if (user != null)
            {

                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    Groups.Remove(connectionId, user.ProfileId);
                    if (!user.ConnectionIds.Any())
                    {
                        User removedUser;
                        Users.TryRemove(profileId, out removedUser);
                    }
                }
            }
            return base.OnDisconnected(stopCalled);
        }
        private User GetUser(string profileId)
        {
            User user;
            Users.TryGetValue(profileId, out user);
            return user;
        }

        public IEnumerable<string> GetConnectedUser()
        {
            return Users.Where(x =>
            {
                lock (x.Value.ConnectionIds)
                {
                    return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
                }
            }).Select(x => x.Key);
        }

        public void SendUserMessage(String username, String message)
        {
            Clients.Group(username).sendUserMessage(message);
        }

        [HubMethodName("sendNotifications")]
        public void SendNotifications(string message)
        {

            List<string> messages = new List<string>();
            messages.Add(message);

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<psoftHub>();
            context.Clients.All.RecieveNotification(message);

        }

        public void sendCellChange(string matrixId, string cellId, string text)
        {
            List<string> messages = new List<string>();
            messages.Add(matrixId);
            messages.Add(cellId);
            messages.Add(text);

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<psoftHub>();
            context.Clients.All.updateCell(messages);
        }

        public void sendCellDeleted(string cellId)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<psoftHub>();
            context.Clients.All.deleteCell(cellId);
        }

        public void sendTextChange(string matrixId, string cellId, string text)
        {
            List<string> messages = new List<string>();
            messages.Add(matrixId);
            messages.Add(cellId);
            messages.Add(text);

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<psoftHub>();
            context.Clients.All.updateText(messages);
        }
    }

}


