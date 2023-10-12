using AutoMapper;
using Challenge.Application.Website.Models;
using Challenge.Domain.DTO.ViewModel;

namespace Challenge.Application.Website.Mapper
{
    public class MappingProfileWeb : Profile
    {
        public MappingProfileWeb()
        {
            CreateMap<CandidatesVM, CandidatesModel>().ReverseMap();
            CreateMap<CandidateExperiencesVM, CandidateExperiencesModel>().ReverseMap();
        }
    }
}
