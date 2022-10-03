using Microsoft.AspNetCore.Http;
using Moq;

namespace MinimalApiCrud.Test
{
    public sealed class UpdateTest : CrudTest
    {
        [Fact]
        public async Task UpdateTest_Should_Update_Existing_Entity()
        {
            TestModel entity = new TestModel(100, "test1", 4, false);
            var dataset = new TestModel[] { Clone(entity) };

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, dataset);
            mContext.Setup(x => x.UpdateAsync(It.Is(entity, TestModelComparer)))
                .ReturnsAsync(1);

            var result = await builder.UpdateImpl(entity.Id, entity)!;

            Assert.Equal(await GetResponse(Results.Ok()), await GetResponse(result));

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
            mContext.Verify(x => x.UpdateAsync(It.Is(entity, TestModelComparer)), Times.Once);            
        }

        [Fact]
        public async Task UpdateTest_Should_NotUpdate_NotExisting_Entity()
        {
            int entityId = 5;
            TestModel entity = new TestModel(entityId, "xxxxx", 6.6, true);

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, Enumerable.Empty<TestModel>());

            var result = await builder.UpdateImpl(entity.Id, entity)!;

            Assert.Equal(await GetResponse<int>(Results.NotFound(entityId)), await GetResponse<int>(result));

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
            mContext.Verify(x => x.UpdateAsync(It.IsAny<TestModel>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTest_Should_Error_NullEntity()
        {
            var builder = SetupMock(out var mContext);

            var result = await builder.UpdateImpl(1, null as TestModel)!;

            Assert.Equal(await GetResponse(Results.BadRequest()), await GetResponse(result));

            mContext.Verify(x => x.Set<TestModel>(), Times.Never);
            mContext.Verify(x => x.UpdateAsync(It.IsAny<TestModel>()), Times.Never);
        }
    }
}
