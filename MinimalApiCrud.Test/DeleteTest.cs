using Microsoft.AspNetCore.Http;
using Moq;

namespace MinimalApiCrud.Test
{
    public sealed class DeleteTest : CrudTest
    {
        [Fact]
        public async Task DeleteTest_Should_Delete_Existing_Entity()
        {
            int entityId = 100;
            var entity = new TestModel(entityId, "test1", 4, false);
            var builder = SetupMock(out var mContext);
            mContext.Setup(x => x.Set<TestModel>()).Returns(new TestModel[] { entity }.AsQueryable());
            mContext.Setup(x => x.RemoveAsync(It.Is(entity, TestModelComparer)))
                .ReturnsAsync(1);

            var result = await builder.DeleteImpl(entityId)!;

            Assert.Equal(await GetResponse(Results.NoContent()), await GetResponse(result));
            mContext.Verify(x => x.RemoveAsync(It.Is(entity, TestModelComparer)), Times.Once);
            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }

        [Fact]
        public async Task DeleteTest_Should_Fail_NotExisting_Entity()
        {
            int entityId = 100;
            var entity = new TestModel(entityId, "test1", 4, false);
            var builder = SetupMock(out var mContext);
            mContext.Setup(x => x.Set<TestModel>()).Returns(new TestModel[] { new TestModel(entityId + 1, "test2", 4.2, true) }.AsQueryable());
            mContext.Setup(x => x.RemoveAsync(It.Is(entity, TestModelComparer)))
                .ReturnsAsync(0);

            var result = await builder.DeleteImpl(entityId)!;

            Assert.Equal(await GetResponse<int>(Results.NotFound(entityId)), await GetResponse<int>(result));
            mContext.Verify(x => x.RemoveAsync(It.Is(entity, TestModelComparer)), Times.Never);
            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }
    }
}
