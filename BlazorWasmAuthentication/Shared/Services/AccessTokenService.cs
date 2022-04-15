using Microsoft.JSInterop;

namespace BlazorWasmAuthentication.Shared
{
    public interface IAccessTokenService {
        Task<string> GetAccessTokenAsync(string tokenName);
        Task RemoveAccessTokenAsync(string tokenName);
        Task SetAccessTokenAsync(string tokenName, string tokenValue);
    }

    public class AccessTokenService : IAccessTokenService
    {
        
        private readonly IJSRuntime _jsRuntime;

        public AccessTokenService(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
        }

        public async Task<string> GetAccessTokenAsync(string tokenName)
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", tokenName);
        }

        public async Task SetAccessTokenAsync(string tokenName, string tokenValue)
        {

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", tokenName, tokenValue);
        }

        public async Task RemoveAccessTokenAsync(string tokenName)
        {

            await _jsRuntime.InvokeAsync<string>("localStorage.removeItem", tokenName);
        }
    }
}
