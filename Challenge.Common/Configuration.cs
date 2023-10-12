using Microsoft.Extensions.Configuration;

namespace Challenge.Common
{
    public class Configuration
    {

        protected readonly IConfiguration _configuration;

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAppSettingValue(string key)
        {
            return _configuration.GetSection("AppSettings").GetSection(key).Value;
        }
    }
}
