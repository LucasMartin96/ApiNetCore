using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FirstApi2xd.Contracts.v1;
using FirstApi2xd.Contracts.v1.Requests;
using FirstApi2xd.Contracts.v1.Responses;
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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Poster")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]

        // [Authorize(Policy = "TagViewer")] Sirve para que el metodo sea accesible unicamente con el claim
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            return Ok(_mapper.Map<List<TagResponse>>(tags));
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> GetTagByName([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);
            
            if (tag == null)
                return NotFound();

            return Ok(_mapper.Map<TagResponse>(tag));
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

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name);


            return Created(locationUri,_mapper.Map<TagResponse>(newTag));
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "MustWorkForLucas")]
        public async Task<IActionResult> DeleteTag([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);
            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}