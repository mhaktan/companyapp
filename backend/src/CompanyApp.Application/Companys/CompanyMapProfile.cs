using AutoMapper;
using CompanyApp.Entities;
using CompanyApp.Companys.Dto;

namespace CompanyApp.Companys
{
    public class CompanyMapProfile : Profile
    {
        public CompanyMapProfile()
        {
            CreateMap<Company, CompanyDto>();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<CompanyDto, Company>();
        }
    }
}
