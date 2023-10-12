using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Challenge.Application.Website.Models;
using Challenge.Common;
using Challenge.Domain.TokenService;

namespace Challenge.Application.Website.Controllers
{
    public class BaseController : Controller
    {

        private readonly Configuration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IMemoryCache _memoryCache;

        private static string urlService;
        private static string _token;

        public BaseController(Configuration configuration, ITokenService tokenService, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _memoryCache = memoryCache;
            urlService = _configuration.GetAppSettingValue(Constants.URLServiceKey);
            GetToken();
        }
        protected void GetToken()
        {
            if (_memoryCache.TryGetValue("TokenKey", out _token))
            {
                if (_tokenService.GetTokenInfo(_token) == null)
                {
                    _token = _tokenService.GenerateToken(_configuration.GetAppSettingValue(Constants.UserTokenKey), _configuration.GetAppSettingValue(Constants.PassTokenKey));
                    _memoryCache.Set("TokenKey", _token);
                }
            }
            else
            {
                _token = _tokenService.GenerateToken(_configuration.GetAppSettingValue(Constants.UserTokenKey), _configuration.GetAppSettingValue(Constants.PassTokenKey));
                _memoryCache.Set("TokenKey", _token);
            }

        }


        internal static async Task<T> GetAsync<T>(string method, object param = null) where T : class
        {

            ResponseFactory<T> response = await RestExecutor.ExecuteRequest<ResponseFactory<T>>
                                                         (urlService,
                                                          method,
                                                          false,
                                                          RestExecutor.Verb.Get,
                                                          new Dictionary<string, object>{{ "Authorization", _token}},
                                                          param);

            if (response != null)
            {
                if (response.Code <= 0 && response.Status != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(response.Message);
                }
                return response.Body;
            }
            else
            {
                return null;
            }
        }

        internal static async Task<ResponseModel> PostAsync<P, T>(string method, T element) where T : class where P : class
        {
            ResponseFactory<P> response = await RestExecutor.ExecuteRequest<ResponseFactory<P>>
                                             (urlService,
                                              method,
                                              false,
                                              RestExecutor.Verb.Post,
                                              new Dictionary<string, object> { { "Authorization", _token } },
                                              element);

            return CreateResponse(response);
        }

        internal static async Task<ResponseModel> PutAsync<P, T>(string method, T element) where P : class where T : class
        {
            ResponseFactory<P> response = await RestExecutor.ExecuteRequest<ResponseFactory<P>>
                                                         (urlService,
                                                          method,
                                                          false,
                                                          RestExecutor.Verb.Put,
                                                          new Dictionary<string, object> { { "Authorization", _token } },
                                                          element);


            return CreateResponse(response);
        }

        internal static async Task<ResponseModel> DeleteAsync(string method, object param = null)
        {
            ResponseFactory<string> response = await RestExecutor.ExecuteRequest<ResponseFactory<string>>
                                                         (urlService,
                                                          method,
                                                          false,
                                                          RestExecutor.Verb.Get,
                                                          new Dictionary<string, object> { { "Authorization", _token } },
                                                          param);

            return CreateResponse(response);
        }

        private static ResponseModel CreateResponse<T>(ResponseFactory<T> response)
        {
            ResponseModel responseVM = new ResponseModel();

            if (response != null)
            {
                if (response.Code <= 0)
                {
                    responseVM.IsCorrect = false;
                    responseVM.Message = response.Message;
                    responseVM.Status = System.Net.HttpStatusCode.NotFound;
                }
                else
                {
                    responseVM.IsCorrect = true;
                    responseVM.Data = response.Body;
                    responseVM.Message = response.Message;
                    responseVM.Status = System.Net.HttpStatusCode.OK;
                }
                return responseVM;
            }
            else
            {
                responseVM.IsCorrect = false;
                responseVM.Message = Messages.RequestFailed;
                responseVM.Status = System.Net.HttpStatusCode.InternalServerError;
                return responseVM;
            }
        }               
    }
}
