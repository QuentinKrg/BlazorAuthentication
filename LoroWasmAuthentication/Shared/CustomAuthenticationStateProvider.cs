using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LoroWasmAuthentication.Shared
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILoginViewModel _loginViewModel;
        private readonly IAccessTokenService _accessTokenService;

        public CustomAuthenticationStateProvider(ILoginViewModel loginViewModel, IAccessTokenService accessTokenService)
        {
            _loginViewModel = loginViewModel;
            _accessTokenService = accessTokenService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User currentUser = await GetUserByJWTAsync();

            if (currentUser != null)
            {
                //create claimsPrincipal
                var claimsPrincipal = GetClaimsPrinciple(currentUser);
                return new AuthenticationState(claimsPrincipal);
            }
            else
            {
                await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated()
        {
            var user = await GetUserByJWTAsync();
            var claimsPrincipal = GetClaimsPrinciple(user);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task<User> GetUserByJWTAsync()
        {
            //pulling the token from localStorage
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            if (jwtToken == null) return null;

            
            return await _loginViewModel.GetUserByJWTAsync(jwtToken);
        }

        public ClaimsPrincipal GetClaimsPrinciple(User currentUser)
        {
            ClaimsPrincipal claimsPrincipal = new();
            try
            {
                var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, currentUser.FirstName),
                new Claim("Lastname", currentUser.LastName),
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.UserLogin))
            };

                if (currentUser.Roles == null || currentUser.Roles.Count == 0)
                {
                    var claimRole = new Claim(ClaimTypes.Role, "");
                }
                else
                {
                    currentUser.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.ToString())));
                }

                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(claims, "serverAuth");
                //create claimsPrincipal
                claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return claimsPrincipal;
        }
    }
}
