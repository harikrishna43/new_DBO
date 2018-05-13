using DBO.Data.Models;
using DBO.Data.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace DBO.Extensions
{
    public static class ConfigureMapper
    {
        public static void Configure()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Advertisement, AdvertisementViewModel>()
                    .ForMember(dest => dest.Skills, opts => opts.MapFrom(src => src
                        .AdsSkills.Select(x => new SelectListItem {Value = x.SkillId.ToString(), Text = x.Skill.Name})))
                    .ForMember(dest => dest.Skills, opts => opts.MapFrom(src => src
                        .AdsIndustries.Select(x =>
                            new SelectListItem {Value = x.IndustryId.ToString(), Text = x.Industry.Name})))
                    .ForMember(dest => dest.SelectedSkills,
                        opts => opts.MapFrom(src => src.AdsSkills.Select(x => x.SkillId)))
                    .ForMember(dest => dest.SelectedIndustries,
                        opts => opts.MapFrom(src => src.AdsIndustries.Select(x => x.IndustryId)))
                    .ForMember(dest => dest.CompanyName, opts => opts.MapFrom(
                        src => src.User != null ? (src.User.Company != null ? src.User.Company.Name : null) : null));
                    

                cfg.CreateMap<AdvertisementViewModel, Advertisement>()
                    .ForMember(dest => dest.AdsSkills, opts => opts.MapFrom(src => src
                        .SelectedSkills.Select(x => new AdsSkills
                        {
                            SkillId = int.Parse(x),
                            AdvertisementId = src.Id
                        })))
                    .ForMember(dest => dest.AdsIndustries, opts => opts.MapFrom(src => src
                        .SelectedIndustries.Select(x => new AdsIndustries
                        {
                            
                            IndustryId = int.Parse(x),
                            AdvertisemenId = src.Id
                        })));

                cfg.CreateMap<DisplayAdViewModel, Advertisement>().ReverseMap();
            });

        }
    }
}