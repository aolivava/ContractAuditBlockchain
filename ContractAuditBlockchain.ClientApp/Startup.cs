using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ContractAuditBlockchain.ClientApp.Startup))]
namespace ContractAuditBlockchain.ClientApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
