using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FirstApi2xd.Contracts.v1;
using FirstApi2xd.Contracts.v1.Requests;
using FirstApi2xd.Contracts.v1.Responses;
using FirstApi2xd.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FirstApi2xd.IntegrationTest
{
    public class IntegrationTest : IDisposable
    {

        protected readonly HttpClient TestClient;
        private readonly IServiceProvider _serviceProvider;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Using an empty DB(in memory)
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@integration.com",
                Password = "SomePassword22123!"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registrationResponse.Token;
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
           var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
           return await response.Content.ReadAsAsync<PostResponse>();
        }

        public void Dispose()
        {
            // Delete the DB on every test
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();

        }
    }
}