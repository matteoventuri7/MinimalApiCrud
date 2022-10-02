using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;

namespace MinimalApiCrud.Test
{
    public class CrudTest
    {
        private const int SEED_DATA_COUNT = 10;
        private readonly Mock<IEndpointRouteBuilder> _mockedEndpointRouteBuilder;
        private readonly MinimalApiCrudBuilder<TestModel, int, TestDataContext> _builder;

        public CrudTest()
        {
            _mockedEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();
            var mockedServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var mockedServiceScope = new Mock<IServiceScope>();
            var mockedServiceProvider = new Mock<IServiceProvider>();

            _mockedEndpointRouteBuilder
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
                .Returns(new TestDataContext(SEED_DATA_COUNT));

            mockedServiceScope
                .SetupGet(x => x.ServiceProvider)
                .Returns(mockedServiceProvider.Object);

            _builder = new MinimalApiCrudBuilder<TestModel, int, TestDataContext>(_mockedEndpointRouteBuilder.Object);
        }

        [Fact]
        public async Task InsertTest_Should_Insert_New_Entity()
        {
            var entity = new TestModel(SEED_DATA_COUNT + 1, "test1", 4, false);

            var result = await _builder.InsertImpl(entity)!;
            Assert.Equal(await GetResponse<TestModel>(Results.Created(string.Empty, entity)),
                        await GetResponse<TestModel>(result));
        }

        [Fact]
        public async Task InsertTest_Should_Fail_Existing_Entity()
        {
            var entity = new TestModel(SEED_DATA_COUNT - 1, "test1", 4, false);

            var result = await _builder.InsertImpl(entity)!;
            Assert.Equal(await GetResponse<string>(Results.Conflict("No entity added")),
                        await GetResponse<string>(result));
        }

        [Fact]
        public async Task InsertTest_Should_Fail_Null_Entity()
        {
            var result = await _builder.InsertImpl(null as TestModel)!;
            Assert.Equal(await GetResponse(Results.BadRequest()),
                        await GetResponse(result));
        }

        private async ValueTask<int> GetResponse(IResult result)
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

        private async ValueTask<(int statusCode, T? value)> GetResponse<T>(IResult result)
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