using System.Linq;
using System.Threading.Tasks;
using FirstApi2xd.Contracts.v1;
using FirstApi2xd.Contracts.v1.Requests;
using FirstApi2xd.Contracts.v1.Responses;
using FirstApi2xd.Domain;
using FirstApi2xd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstApi2xd.Controllers.V1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });

            }

            if (request.Role == "" || (request.Role.ToLower() != "admin" && request.Role.ToLower() != "poster"))
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = new []{"Invalid role"}
                });
            }
            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password, request.Role);


            return asd(authResponse);
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {

            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            return asd(authResponse);
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Login([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            return asd(authResponse);
        }

        private IActionResult asd(AuthenticationResult authResponse)
        {
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}