using System.Threading.Tasks;
using FirstApi2xd.Contracts.v1;
using FirstApi2xd.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi2xd.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;

        public TagsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAllTagsAsync());
        }
    }
}