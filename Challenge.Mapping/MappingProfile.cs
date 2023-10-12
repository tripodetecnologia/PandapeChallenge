using AutoMapper;
using Challenge.DataAccess.Model;
using Challenge.Domain.DTO.ViewModel;

namespace Challenge.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Candidates, CandidatesVM>().ReverseMap();
            CreateMap<CandidateExperiences, CandidateExperiencesVM>().ReverseMap();            

        }
    }
}