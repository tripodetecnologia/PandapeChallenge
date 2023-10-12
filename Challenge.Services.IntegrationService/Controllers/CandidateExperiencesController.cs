using Microsoft.AspNetCore.Mvc;
using Challenge.Domain.LogService;
using Challenge.Common;
using Microsoft.AspNetCore.Authorization;
using Challenge.Domain.DTO.ViewModel;
using Challenge.Domain.CandidateExperiencesService;

namespace Challenge.Services.IntegrationService.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CandidateExperiencesController : ControllerBase
    {

        #region Declarations

        private readonly ICandidateExperiencesService _candidateExperiencesService;
        private readonly ILogService _logService;
        
        #endregion

        #region Constructor

        public CandidateExperiencesController(ICandidateExperiencesService candidateExperiencesService, ILogService logService)
        {
            _candidateExperiencesService = candidateExperiencesService;
            _logService = logService;            
        }

        #endregion

        #region Methods  


        [HttpGet]
        public async Task<ActionResult> GetExperiencesByCandidate(int candidateId)
        {
            try
            {
                return Ok(await _candidateExperiencesService.GetExperiencesByCandidate(candidateId));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidateExperiencesController, Method: GetExperiencesByCandidate, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError());
            }
            finally
            {
                _logService.Generate();
            }
        }



        [HttpPost]
        public async Task<ActionResult> AddOrUpdate(CandidateExperiencesVM model)
        {
            try
            {
                return Ok(await _candidateExperiencesService.AddOrUpdateAsync(model));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidateExperiencesController, Method: AddOrUpdate, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError());
            }
            finally
            {
                _logService.Generate();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                return Ok(await _candidateExperiencesService.DeleteAsync(id));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidateExperiencesController, Method: Delete, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError());
            }
            finally
            {
                _logService.Generate();
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _candidateExperiencesService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidateExperiencesController, Method: GetById, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError());
            }
            finally
            {
                _logService.Generate();
            }
        }

        #endregion

    }

}