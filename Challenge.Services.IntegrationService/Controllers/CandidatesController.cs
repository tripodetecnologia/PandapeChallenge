using Microsoft.AspNetCore.Mvc;
using Challenge.Domain.LogService;
using Challenge.Common;
using Microsoft.AspNetCore.Authorization;
using Challenge.Domain.DTO.ViewModel;
using Challenge.Domain.CandidatesService;

namespace Challenge.Services.IntegrationService.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {

        #region Declarations

        private readonly ICandidatesService _candidatesService;
        private readonly ILogService _logService;
        
        #endregion

        #region Constructor

        public CandidatesController(ICandidatesService allyService, ILogService logService)
        {
            _candidatesService = allyService;
            _logService = logService;            
        }

        #endregion

        #region Methods  


        [HttpGet]
        public async Task<ActionResult> GetList()
        {
            try
            {
                return Ok(await _candidatesService.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidatesController, Method: GetList, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Ok(ResponseFactory<string>.RequestError());
            }
            finally
            {
                _logService.Generate();
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdate(CandidatesVM model)
        {
            try
            {
                return Ok(await _candidatesService.AddOrUpdateAsync(model));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidatesController, Method: AddOrUpdate, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
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
                return Ok(await _candidatesService.DeleteAsync(id));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidatesController, Method: Delete, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
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
                return Ok(await _candidatesService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidatesController, Method: GetById, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
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