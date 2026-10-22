using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CompanyApp.EntityFrameworkCore;

namespace CompanyApp.Web.Host
{
    [DependsOn(typeof(CompanyAppApplicationModule), typeof(CompanyAppEntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class CompanyAppWebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(CompanyAppApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CompanyAppWebHostModule).GetAssembly());
        }
    }
}
