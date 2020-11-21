using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FirstApi2xd.Contracts.v1;
using FirstApi2xd.Contracts.v1.Requests;
using FirstApi2xd.Contracts.v1.Responses;
using FirstApi2xd.Domain;
using FluentAssertions;
using Xunit;

namespace FirstApi2xd.IntegrationTest
{
    public class PostsControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            // Authentication
            await AuthenticateAsync();


            //Act

            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();

        }

        [Fact]
        public async Task Get_ReturnsPosts_WhenPostExistsInTheDatabase()
        {
            //Arrange

            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Name= "TestPost"
            });
            // Act

            var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            //Assert

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPost = await response.Content.ReadAsAsync<Post>();
            returnedPost.Id.Should().Be(createdPost.Id);
            returnedPost.Name.Should().Be("TestPost");

        }
    }
}