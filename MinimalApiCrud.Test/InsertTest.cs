using Microsoft.AspNetCore.Http;
using Moq;

namespace MinimalApiCrud.Test
{
    public sealed class InsertTest : CrudTest
    {
        [Fact]
        public async Task InsertTest_Should_Insert_New_Entity()
        {
            var entity = new TestModel(100, "test1", 4, false);
            var builder = SetupMock(out var mContext);
            mContext.Setup(x => x.AddAsync(It.Is(entity, TestModelComparer)))
                .ReturnsAsync(1);

            var result = await builder.InsertImpl(entity)!;

            Assert.Equal(await GetResponse<TestModel>(Results.Created(string.Empty, entity)),
                        await GetResponse<TestModel>(result));
            mContext.Verify(x => x.AddAsync(It.Is(entity, TestModelComparer)), Times.Once);
        }

        [Fact]
        public async Task InsertTest_Should_Fail_Existing_Entity()
        {
            var entity = new TestModel(100, "test1", 4, false);
            var builder = SetupMock(out var mContext);
            mContext.Setup(x => x.AddAsync(It.Is(entity, TestModelComparer)))
                .ReturnsAsync(0);

            var result = await builder.InsertImpl(entity)!;

            Assert.Equal(await GetResponse<string>(Results.Conflict("No entity added")), await GetResponse<string>(result));
            mContext.Verify(x => x.AddAsync(It.Is(entity, TestModelComparer)), Times.Once);
        }

        [Fact]
        public async Task InsertTest_Should_Fail_Null_Entity()
        {
            var builder = SetupMock(out var mContext);

            var result = await builder.InsertImpl(null as TestModel)!;

            Assert.Equal(await GetResponse(Results.BadRequest()), await GetResponse(result));
            mContext.Verify(x => x.AddAsync(It.IsAny<TestModel>()), Times.Never);
        }
    }
}
