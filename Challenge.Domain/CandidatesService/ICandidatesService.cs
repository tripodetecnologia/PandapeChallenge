using Challenge.Common;
using Challenge.Domain.DTO.ViewModel;


namespace Challenge.Domain.CandidatesService
{
    public interface ICandidatesService
    {
        
        Task<ResponseFactory<IEnumerable<CandidatesVM>>> GetAllAsync();
        Task<ResponseFactory<CandidatesVM>> GetByIdAsync(int id);
        Task <ResponseFactory<string>> AddOrUpdateAsync(CandidatesVM model);
        Task<ResponseFactory<string>>  DeleteAsync(int id);
        
    }
}
