using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ch.appl.psoft.Interface.OWinStartup))]

namespace ch.appl.psoft.Interface
{
    public class OWinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
