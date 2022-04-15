using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LoroWasmAuthentication.Shared
{
    public interface ILoginViewModel
    {
        string Password { get; set; }
        string Userlogin { get; set; }
        Task<AuthenticationResponse> AuthenticateJWT();
        public Task<User> GetUserByJWTAsync(string jwtToken);
    }

    public class LoginViewModel : ILoginViewModel
    {
        [Required]
        public string Userlogin { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        private readonly HttpClient _httpClient;

        public LoginViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthenticationResponse> AuthenticateJWT()
        {
            //creating authentication request
            var authenticationRequest = new AuthenticationRequest
            {
                UserLogin = Userlogin,
                Password = Password,
            };

            //authenticating the request
            var httpMessageResponse = await _httpClient.PostAsJsonAsync("user/authenticatejwt", authenticationRequest);

            //sending the token to the client to store
            return await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }

        public async Task<User> GetUserByJWTAsync(string jwtToken)
        {
            jwtToken = $@"""{jwtToken}""";
            try
            {
                //preparing the http request
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "user/getuserbyjwt")
                {
                    Content = new StringContent(jwtToken)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }
                };

                //making the http request
                var response = await _httpClient.SendAsync(requestMessage);

                //returning the user if found
                var returnedUser = await response.Content.ReadFromJsonAsync<User>();
                if (returnedUser != null) return await Task.FromResult(returnedUser);
                else return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
                return null;
            }
        }
    }
}
