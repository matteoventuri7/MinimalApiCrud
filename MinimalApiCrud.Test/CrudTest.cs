using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;

namespace MinimalApiCrud.Test
{
    public abstract class CrudTest
    {
        protected readonly IEqualityComparer<TestModel?> TestModelComparer = new TestModelComparer();

        protected MinimalApiCrudBuilder<TestModel, int, TestDataContext> SetupMock(out Mock<TestDataContext> mockedDataContext)
        {
            mockedDataContext = new Mock<TestDataContext>();
            var mockedEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();

            var mockedServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var mockedServiceScope = new Mock<IServiceScope>();
            var mockedServiceProvider = new Mock<IServiceProvider>();

            mockedEndpointRouteBuilder
                .SetupGet(x => x.ServiceProvider)
                .Returns(mockedServiceProvider.Object);

            mockedServiceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(mockedServiceScope.Object);

            mockedServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(mockedServiceScopeFactory.Object);
            mockedServiceProvider
            .Setup(x => x.GetService(typeof(TestDataContext)))
                .Returns(mockedDataContext.Object);

            mockedServiceScope
                .SetupGet(x => x.ServiceProvider)
                .Returns(mockedServiceProvider.Object);

            var builder = new MinimalApiCrudBuilder<TestModel, int, TestDataContext>(mockedEndpointRouteBuilder.Object);
            return builder;
        }

        protected void SeedDataContext(Mock<TestDataContext> mContext, IEnumerable<TestModel> testModels)
        {
            mContext.Setup(x => x.Set<TestModel>()).Returns(testModels.AsQueryable());
        }

        protected T Clone<T>(T toClone) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize<T>(toClone))!;

        protected async ValueTask<int> GetResponse(IResult result)
        {
            using MemoryStream ms = new();

            var mockHttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
                Response =
                {
                    Body = ms,
                },
            };

            await result.ExecuteAsync(mockHttpContext);
            return mockHttpContext.Response.StatusCode;
        }

        protected async ValueTask<(int statusCode, T? value)> GetResponse<T>(IResult result)
        {
            T? res = default;
            using MemoryStream ms = new();

            var mockHttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
                Response =
                {
                    Body = ms,
                },
            };

            await result.ExecuteAsync(mockHttpContext);
            if (mockHttpContext.Response.Body.Length != 0)
            {
                mockHttpContext.Response.Body.Position = 0;
                var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                res = await JsonSerializer.DeserializeAsync<T>(mockHttpContext.Response.Body, jsonOptions);
            }
            return (mockHttpContext.Response.StatusCode, res);
        }
    }
}