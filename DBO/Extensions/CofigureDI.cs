using Autofac;
using Autofac.Integration.Mvc;

using DBO.Data;
using DBO.Data.Repositories.Contract;
using DBO.Data.Repositories.Implementation;
using DBO.Services.Contract;
using DBO.Services.Implementation;

using System.Web.Mvc;

namespace DBO.Extensions
{
    public static class CofigureDI
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<ApplicationDbContext>().InstancePerRequest();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<AdsRepository>().As<IAdsRepository>().InstancePerDependency();

            builder.RegisterType<SubdataService>().AsImplementedInterfaces();
            builder.RegisterType<FollowerService>().AsImplementedInterfaces();
            builder.RegisterType<NewsService>().AsImplementedInterfaces();
            builder.RegisterType<AdsService>().As<IAdsService>().InstancePerDependency();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerDependency();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}