using System;
using System.Threading.Tasks;
using FirstApi2xd.Contracts.v1;
using FirstApi2xd.Contracts.v1.Requests;
using FirstApi2xd.Domain;
using FirstApi2xd.Extensions;
using FirstApi2xd.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi2xd.Controllers.V1
{
    // Roles = "Admin,Post" Admite los dos roles
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Poster")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;

        public TagsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]

        // [Authorize(Policy = "TagViewer")] Sirve para que el metodo sea accesible unicamente con el claim
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAllTagsAsync());
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> GetTagByName([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);
            
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPost(ApiRoutes.Tags.Create)]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {

            var newTag = new Tags
            {
                Name = request.Name,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };
            var created = await _postService.CreateTagAsync(newTag);
            if (!created)
            {
                return BadRequest(new[] {"Unable to create tag"});
            }

            return Ok(new {TagName= newTag.Name});
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}