using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Rastreador.Helpers;

namespace Rastreador
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            #region seta o Jobde Atualização de encomendas para rodar de 1 em 1 minuto
                //// construct a scheduler factory
                //ISchedulerFactory schedFact = new StdSchedulerFactory();

                //// get a scheduler
                //IScheduler sched = schedFact.GetScheduler();
                //sched.Start();

                //// construct job info
                //IJobDetail jobDetail = JobBuilder.Create(typeof(AtualizaEncomendasJob))
                //.WithIdentity("myJob", "group1")
                //.Build();
                //ITrigger trigger = TriggerBuilder.Create()
                //.WithIdentity("trigger3", "group1")
                //.WithCronSchedule("0 0/1 * * * ?")            
                //.ForJob("myJob", "group1")
                //.Build();
                //sched.ScheduleJob(jobDetail, trigger); 
            #endregion
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

        }
    }
}