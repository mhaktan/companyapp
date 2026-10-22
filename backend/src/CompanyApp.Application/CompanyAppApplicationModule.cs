using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace CompanyApp
{
    [DependsOn(typeof(CompanyAppCoreModule), typeof(AbpAutoMapperModule))]
    public class CompanyAppApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(CompanyAppApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CompanyAppApplicationModule).GetAssembly());
        }
    }
}
