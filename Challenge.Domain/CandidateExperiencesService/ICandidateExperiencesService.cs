using Challenge.Common;
using Challenge.Domain.DTO.ViewModel;


namespace Challenge.Domain.CandidateExperiencesService
{
    public interface ICandidateExperiencesService
    {

        Task<ResponseFactory<IEnumerable<CandidateExperiencesVM>>> GetExperiencesByCandidate(int candidateId);
        Task<ResponseFactory<CandidateExperiencesVM>> GetByIdAsync(int id);        
        Task <ResponseFactory<string>> AddOrUpdateAsync(CandidateExperiencesVM model);
        Task<ResponseFactory<string>>  DeleteAsync(int id);        

    }
}
