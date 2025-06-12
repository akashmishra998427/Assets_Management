namespace Assets_Management.Services
{
    public class ApiConnect
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiConnect(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string CoreApiUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return string.Empty;

            string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            if (baseUrl == "http://192.168.41.9:97/")
            {
                return _configuration["Apisettings:coreApi"];
            }
            else
            {
                //return "http://192.168.41.149:76/api";
                return "https://CoreApi.richagroup.com/api";
                //return "http://180.151.12.214:91/api";
            }
        }

        public string WebApiUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return string.Empty;
            string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return _configuration["Apisettings:BaseUrl"];
        }

        public string CoreApi2()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return string.Empty;
            string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return _configuration["Apisettings:coreApi"];
        }
    }
}
