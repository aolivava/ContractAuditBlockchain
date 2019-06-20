using System.Reflection;
using System.Web.Mvc;
using ContractAuditBlockchain.ApiAccess.Api;
using ContractAuditBlockchain.BusinessLogic.AccessControl;
using ContractAuditBlockchain.Core.Config;
using ContractAuditBlockchain.Domain;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace ContractAuditBlockchain.ClientApp.App_Start
{

    public static class SimpleInjectorInitializer
    {
        public static void Initialize()
        {
            var container = new Container();

            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void InitializeContainer(Container container)
        {
            Lifestyle ls = new WebRequestLifestyle();
            container.Register<IApplicationDbContext, ApplicationEntities>(ls);
            container.Register<IApplicationUserManager, ApplicationUserManager>(ls);
            container.Register<IApplicationRoleManager, ApplicationRoleManager>(ls);

            container.Register<IAccessControl, AccessControlLogic>(ls);

            container.Register<AdminParticipant>(ls);
            container.Register<ClientParticipant>(ls);
            container.Register<RentContract>(ls);
            container.Register<IApplicationConfig, ApplicationConfig>(Lifestyle.Singleton);

            container.Register(typeof(IRepository<,>), typeof(EFRepository<,>), ls);
        }
    }
}