using BlazorWasmAuthentication.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorWasmAuthentication.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public const string GROUP_USER = "USER";
        public const string GROUP_ADMIN = "ADMIN";

        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("authenticatejwt")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateJWT(AuthenticationRequest authenticationRequest)
        {
            
            if (authenticationRequest.UserLogin != "admin" && authenticationRequest.Password != "1234")
            {
                return NotFound();
            }
            else
            {
                string token = string.Empty;

                // Check if the user has the necessary access roles
                // Check the roles
                List<String> claimRoles = new List<string>();

                claimRoles.Add(GROUP_ADMIN);

                // No role found for this user
                if (claimRoles == null || claimRoles.Count == 0)
                {
                    return StatusCode(403); // Access to resquested resource is forbidden
                }
                try
                {
                    User user = GetUserByUserLogin(authenticationRequest.UserLogin).Result;
                    claimRoles.ForEach(r => user.Roles.Add(r));
                    token = GenerateJwtToken(user);
                }
                catch (Exception e)
                {

                    throw e;
                }


                return await Task.FromResult(new AuthenticationResponse() { Token = token });
            }
        }

        protected async Task<User> GetUserByUserLogin(string userLogin)
        {
            var user = new User();
            user.FirstName = "FirstName";
            user.LastName = "LastName";
            user.UserLogin = userLogin;

            return user;
        }

        [HttpPost("getuserbyjwt")]
        public async Task<ActionResult<User>> GetUserByJWT([FromBody] string jwtToken)
        {
            try
            {
                // Getting the secret key
                string secretKey = _configuration["JWTSettings:SecretKey"];
                var key = Encoding.ASCII.GetBytes(secretKey);

                // Preparing the validation parameters
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                // Validating the token
                var principle = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = (JwtSecurityToken)securityToken;

                if (jwtSecurityToken != null
                    && jwtSecurityToken.ValidTo > DateTime.Now
                    && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Returning the user if found
                    User returnUser = new User();
                    returnUser.UserLogin = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
                    returnUser.FirstName = principle.FindFirst(ClaimTypes.Name)?.Value.ToString();
                    returnUser.LastName = principle.FindFirst("Lastname")?.Value.ToString();
                    principle.FindAll(ClaimTypes.Role)?.ToList().ForEach(x => returnUser.Roles.Add(x.Value.ToString()));

                    return returnUser;
                }
            }
            catch (Exception ex)
            {
                // Logging the error and returning null
                Console.WriteLine("Exception : " + ex.Message);
                return null;
            }
            // Returning null if token is not validated
            return null;
        }

        protected string GenerateJwtToken(User user)
        {
            //getting the secret key
            string secretKey = _configuration["JWTSettings:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim("Lastname", user.LastName),
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserLogin))
            };

            if (user.Roles == null || user.Roles.Count == 0)
            {
                var claimRole = new Claim(ClaimTypes.Role, "");
            }
            else
            {
                user.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.ToString())));
            }

            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(claims, "serverAuth");
           
            // generate token that is valid for 7 days
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                //IssuedAt = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //creating a token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //returning the token back
            return tokenHandler.WriteToken(token);
        }
    }
}
