using Blazored.LocalStorage;
using LoroWasmAuthentication.Client;
using LoroWasmAuthentication.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddHttpClient();

// authentication http clients
// transactional named http clients
var clientConfigurator = void (HttpClient client) => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>("LoginViewModelClient", clientConfigurator);
builder.Services.AddHttpClient<IWeatherForecastViewModel, WeatherForecastViewModel>("WeatherForecastViewModelClient", clientConfigurator);
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
