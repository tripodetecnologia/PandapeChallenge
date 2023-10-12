using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Challenge.Domain.LogService;
using Challenge.Common;
using Challenge.Application.Website.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Challenge.Domain.TokenService;
using Challenge.Application.Website.Models;
using Challenge.Domain.DTO.ViewModel;
using Challenge.Application.Website.Utils;


namespace Fogacoop.Application.Website.Areas.Management.Controllers
{
    public class CandidateExperiencesController : BaseController
    {
        #region Private fields        
        
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        #endregion

        public CandidateExperiencesController(IMapper mapper, ILogService logService, Configuration configuration, ITokenService tokenService, IMemoryCache memoryCache) : 
                                             base(configuration, tokenService, memoryCache)
        {
            _mapper = mapper;        
            _logService = logService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(int idCandidate)
        {
            
            var listCandidateExperiencesVM = await GetAsync<List<CandidateExperiencesVM>>("CandidateExperiences/GetExperiencesByCandidate", new { candidateId = idCandidate });

            return PartialView("_IndexGrid", _mapper.Map<List<CandidateExperiencesModel>>(listCandidateExperiencesVM));
        }

        [HttpGet]
        public PartialViewResult Create(int idCandidate)
        {
            ViewBag.TitleModal = "Adicionar Experiencia Candidato";

            var candidateExperience = new CandidateExperiencesModel() { IdCandidate = idCandidate};

            return PartialView("_CreateOrEdit", candidateExperience);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Experiencia Candidato";

            var candidateExperiences = await GetAsync<CandidateExperiencesVM>("CandidateExperiences/GetById", new { id = id });                   

            return PartialView("_CreateOrEdit", _mapper.Map<CandidateExperiencesModel>(candidateExperiences));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(CandidateExperiencesModel candidateExperience)
        {
            try
            {

                var candidateExperiencesVM = _mapper.Map<CandidateExperiencesVM>(candidateExperience);

                ResponseModel response = await PostAsync<string, CandidateExperiencesVM>("CandidateExperiences/AddOrUpdate", candidateExperiencesVM);

                if (response.IsCorrect)
                {
                    return Json(JsonResponseFactory.SuccessResponse(Messages.SaveOK));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse(response.Message));
                }
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidateExperiencesController, Method: AddOrUpdate, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Json(JsonResponseFactory.ErrorResponse(Messages.UnexpectedError));
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
                ResponseModel response = await DeleteAsync("CandidateExperiences/Delete", new { id = id });

                if (response.IsCorrect)
                {
                    return Json(JsonResponseFactory.SuccessResponse(Messages.DeleteOK));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse(response.Message));
                }
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Class: CandidateExperiencesController, Method: Delete, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Json(JsonResponseFactory.ErrorResponse(Messages.UnexpectedError));
            }
            finally
            {
                _logService.Generate();
            }
        }        
    }
}