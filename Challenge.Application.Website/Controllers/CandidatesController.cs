using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Challenge.Application.Website.Models;
using Challenge.Application.Website.Utils;
using Challenge.Common;
using Challenge.Domain.DTO.ViewModel;
using Challenge.Domain.LogService;
using Challenge.Domain.TokenService;

namespace Challenge.Application.Website.Controllers
{ 
    public class CandidatesController : BaseController
    {
        private readonly Configuration _configuration;        
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        public CandidatesController(IMapper mapper, ILogService logService, Configuration configuration, ITokenService tokenService, IMemoryCache memoryCache) 
                                    : base(configuration, tokenService, memoryCache)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logService = logService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var candidatesList = await GetAsync<List<CandidatesVM>>("Candidates/GetList");

            return PartialView("_IndexGrid", _mapper.Map<List<CandidatesModel>>(candidatesList));
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Registrar Candidato";
             
            return PartialView("_CreateOrEdit");
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Candidato";

            var candidate = _mapper.Map<CandidatesModel>(await GetAsync<CandidatesVM>("Candidates/GetById", new { id = id }));              

            return PartialView("_CreateOrEdit", candidate);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(CandidatesModel candidate)
        {
            try
            {               
                
                var candidatesVM = _mapper.Map<CandidatesVM>(candidate);
               
                ResponseModel response = await PostAsync<string, CandidatesVM>("Candidates/AddOrUpdate", candidatesVM);

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
                _logService.Add(LogKey.Error, $"Class: CandidatesController, Method: AddOrUpdate, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
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
                ResponseModel response = await DeleteAsync("Candidates/Delete", new { id = id });

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
                _logService.Add(LogKey.Error, $"Class: CandidatesController, Method: Delete, Trace: {ex.StackTrace}, Error Message: {ex.Message}");
                return Json(JsonResponseFactory.ErrorResponse(Messages.UnexpectedError));
            }
            finally
            {
                _logService.Generate();
            }
        }
    }
}