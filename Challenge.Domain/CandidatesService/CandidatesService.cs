using AutoMapper;
using Challenge.Common;
using Challenge.DataAccess.Core;
using Challenge.DataAccess.Model;
using Challenge.Domain.DTO.ViewModel;
using Challenge.Domain.LogService;
using System.Text;

namespace Challenge.Domain.CandidatesService
{
    public class CandidatesService : ICandidatesService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        #endregion

        #region Constructor
        public CandidatesService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logService = logService;
        }
        #endregion

        #region Methods
        public async Task<ResponseFactory<string>> AddOrUpdateAsync(CandidatesVM model)
        {

            string validation = await Validations(model);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                return ResponseFactory<string>.RequestValidation(validation);
            }

            Candidates candidate = _mapper.Map<Candidates>(model);

            if (candidate.IdCandidate != 0)
            {
                candidate.ModifyDate = DateTime.Now;
                _unitOfWork.Repository<Candidates>().Update(candidate);
            }
            else
            {
                candidate.InsertDate = DateTime.Now;
            }

            _unitOfWork.Repository<Candidates>().AddOrUpdate(candidate);

            await _unitOfWork.SaveChangesAsync();

            return ResponseFactory<string>.RequestOK(Messages.SaveOK);

        }

        public async Task<ResponseFactory<string>> DeleteAsync(int id)
        {            

            try
            {
                if ((await _unitOfWork.Repository<Candidates>().GetAsync(x => x.IdCandidate == id)).Any())
                {
                    if ((await _unitOfWork.Repository<CandidateExperiences>().GetAsync(x => x.IdCandidate == id)).Any())
                    {
                        return ResponseFactory<string>.RequestValidation(string.Format(Messages.RecordWithRelation, "experiencia candidato"));
                    }

                    _unitOfWork.Repository<Candidates>().Remove(x => x.IdCandidate.Equals(id));

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

        public async Task<ResponseFactory<IEnumerable<CandidatesVM>>> GetAllAsync()
        {

            List<CandidatesVM> listCandidatesVM = _mapper.Map<List<CandidatesVM>>((await _unitOfWork.Repository<Candidates>().GetAsync()).ToList());

            return ResponseFactory<IEnumerable<CandidatesVM>>.RequestOK(listCandidatesVM.OrderBy(x => x.Name).ThenBy(x => x.Surname));

        }

        public async Task<ResponseFactory<CandidatesVM>> GetByIdAsync(int id)
        {
            CandidatesVM allyVM = _mapper.Map<CandidatesVM>((await _unitOfWork.Repository<Candidates>().GetAsync(x => x.IdCandidate == id)).FirstOrDefault());

            return ResponseFactory<CandidatesVM>.RequestOK(allyVM);
        }

        private async Task<string> Validations(CandidatesVM model)
        {
            StringBuilder errors = new StringBuilder();


            if ((await _unitOfWork.Repository<Candidates>().GetAsync(x => x.IdCandidate != model.IdCandidate &&
                                                                          x.Name.ToUpper() == model.Name.ToUpper() &&
                                                                          x.Surname.ToUpper() == model.Surname.ToUpper() &&
                                                                          x.Email.ToUpper() == model.Email.ToUpper())).Any())
            {
                errors.Append(string.Format(Messages.RecordExist, "Candidato"));
            }

            if (DateTime.Now.Year - model.Birthdate.Year < Constants.MinLegalAge)
            {
                errors.Append(Constants.HTML.BR);
                errors.Append(Messages.BirthDateError);
            }

            return errors.ToString();
        }

        #endregion
    }
}
