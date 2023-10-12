using AutoMapper;
using Challenge.Common;
using Challenge.DataAccess.Core;
using Challenge.DataAccess.Model;
using Challenge.Domain.DTO.ViewModel;
using Challenge.Domain.LogService;
using System.Text;

namespace Challenge.Domain.CandidateExperiencesService
{
    public class CandidateExperiencesService : ICandidateExperiencesService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        #endregion

        #region Constructor
        public CandidateExperiencesService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logService = logService;
        }
        #endregion

        #region Methods
        public async Task<ResponseFactory<string>> AddOrUpdateAsync(CandidateExperiencesVM model)
        {

            string validation = await Validations(model);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                return ResponseFactory<string>.RequestValidation(validation);
            }

            CandidateExperiences candidateExperiences = _mapper.Map<CandidateExperiences>(model);

            if (candidateExperiences.IdCandidateExperience != 0)
            {
                candidateExperiences.ModifyDate = DateTime.Now;
                _unitOfWork.Repository<CandidateExperiences>().Update(candidateExperiences);
            }
            else
            {
                candidateExperiences.InsertDate = DateTime.Now;
            }

            _unitOfWork.Repository<CandidateExperiences>().AddOrUpdate(candidateExperiences);

            await _unitOfWork.SaveChangesAsync();

            return ResponseFactory<string>.RequestOK(Messages.SaveOK);                      
           

        }

        public async Task<ResponseFactory<string>> DeleteAsync(int id)
        {            

            try
            {
                if ((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x => x.IdCandidateExperience == id)).Any())
                {
                    _unitOfWork.Repository<CandidateExperiences>().Remove(x => x.IdCandidateExperience.Equals(id));

                    await _unitOfWork.SaveChangesAsync();

                    return ResponseFactory<string>.RequestOK(Messages.SaveOK);
                }
                else
                {
                    return ResponseFactory<string>.RequestValidation(Messages.RecordNotFound);
                }
            }
            catch (Exception e)
            {
                _logService.Add(LogKey.Error, $"Trace: {e.StackTrace}, Error Message: {e.Message}");

                throw;
            }
            finally
            {
                _logService.Generate();
            }
        }

        public async Task<ResponseFactory<IEnumerable<CandidateExperiencesVM>>> GetExperiencesByCandidate(int candidateId)
        {

            List<CandidateExperiencesVM> listCandidateExperiencesVM = _mapper.Map<List<CandidateExperiencesVM>>((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x=> x.IdCandidate == candidateId, "Candidate")).OrderByDescending(x => x.BeginDate).ToList());

            return ResponseFactory<IEnumerable<CandidateExperiencesVM>>.RequestOK(listCandidateExperiencesVM);

        }



        public async Task<ResponseFactory<CandidateExperiencesVM>> GetByIdAsync(int id)
        {
            CandidateExperiencesVM candidateExperiencesVM = _mapper.Map<CandidateExperiencesVM>((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x => x.IdCandidateExperience == id, "Candidate")).FirstOrDefault());

            return ResponseFactory<CandidateExperiencesVM>.RequestOK(candidateExperiencesVM);

        }        

        private async Task<string> Validations(CandidateExperiencesVM model)
        {
            StringBuilder errors = new StringBuilder();

            if ((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x => x.IdCandidateExperience != model.IdCandidateExperience &&
                                                                                    x.IdCandidate == model.IdCandidate &&
                                                                                    x.Company.ToUpper() == model.Company.ToUpper() &&
                                                                                    x.Job.ToUpper() == model.Job.ToUpper() &&
                                                                                    x.BeginDate == model.BeginDate &&
                                                                                    x.EndDate == model.EndDate)).Any())
            {
                errors.Append(Messages.ErrorNameAlreadyExist);
            }

            if ((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x => x.IdCandidateExperience != model.IdCandidateExperience &&
                                                                                    x.IdCandidate == model.IdCandidate &&
                                                                                    (model.BeginDate >= x.BeginDate && model.BeginDate <= (x.EndDate == null ? DateTime.Now : x.EndDate)))).Any())
            {
                errors.Append(Constants.HTML.BR);
                errors.Append(Messages.ErrorBeginDate);
            }

            if ((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x => x.IdCandidateExperience != model.IdCandidateExperience &&
                                                                                    x.IdCandidate == model.IdCandidate &&
                                                                                    ((model.EndDate == null ? DateTime.Now : model.EndDate) >= x.BeginDate && (model.EndDate == null ? DateTime.Now : model.EndDate) <= (x.EndDate == null ? DateTime.Now : x.EndDate)))).Any())
            {
                errors.Append(Constants.HTML.BR);
                errors.Append(Messages.ErrorEndDate);
            }

            if (model.EndDate.HasValue && model.BeginDate > model.EndDate.Value)
            {
                errors.Append(Constants.HTML.BR);
                errors.Append(Messages.BeginDateGreaterThanEndDateError);
            }

            return errors.ToString();
        }

        #endregion
    }
}
