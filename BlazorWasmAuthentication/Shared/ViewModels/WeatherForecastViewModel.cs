namespace BlazorWasmAuthentication.Shared
{
    public interface IWeatherForecastViewModel
    {
        public WeatherForecast[] forecasts { get; }
        public Task LoadData();
    }

    public class WeatherForecastViewModel : IWeatherForecastViewModel
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _accessTokenService;

        public WeatherForecast[]? forecasts { get; private set; }

        public WeatherForecastViewModel(HttpClient httpClient, IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }

        public async Task LoadData()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            forecasts = await _httpClient.GetAsync<WeatherForecast[]>("WeatherForecast", jwtToken);
        }
    }
}
