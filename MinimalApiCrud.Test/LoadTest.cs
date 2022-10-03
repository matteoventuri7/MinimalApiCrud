using Microsoft.Extensions.Primitives;
using Moq;

namespace MinimalApiCrud.Test
{
    public sealed class LoadTest : CrudTest
    {
        [Fact]
        public async Task GetAllImpl_Should_Return_AllEntities()
        {
            var dataset = Enumerable.Range(0, 100).Select(i => new TestModel(i, "test" + i, 1, true)).ToArray();

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, dataset);

            var result = await builder.GetAllImpl<TestModel>()!;

            Assert.Equal(dataset, result, TestModelComparer);

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }

        [Theory]
        [InlineData(1, 10, 50)]
        [InlineData(2, 10, 50)]
        [InlineData(3, 10, 22)]
        [InlineData(1, 10, 4)]
        public async Task GetAllImpl_Should_Return_FirstPageEntities(int nPage, int pageSize, int total)
        {
            var dataset = Enumerable.Range(0, total).Select(i => new TestModel(i, "test" + i, 1, true)).ToArray();

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, dataset);

            var result = await builder.GetAllImpl<TestModel>(nPage, pageSize)!;

            Assert.Equal(dataset.Skip((nPage - 1) * pageSize).Take(pageSize).ToArray(), result.ToArray(), TestModelComparer);

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }

        [Fact]
        public async Task GetOneByIdImpl_Should_Return_Entity()
        {
            int entityId = 1;
            var dbEntity = new TestModel(entityId, "test1", 1, true);

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, Clone(new TestModel[] { dbEntity }));

            var result = await builder.GetOneByIdImpl<TestModel?>(entityId);

            Assert.Equal(dbEntity, result, TestModelComparer);

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }

        [Fact]
        public async Task GetOneByIdImpl_Should_Return_Null()
        {
            int entityId = 1;

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, Enumerable.Empty<TestModel>());

            var result = await builder.GetOneByIdImpl<TestModel?>(entityId);

            Assert.Null(result);

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }

        [Fact]
        public async Task FilterImpl_Should_Return_FilteredData_AND_Logic()
        {
            var whereClauses = new Dictionary<string, string> {
                    {nameof(TestModel.Result),$"{nameof(TestModel.Result)} == @0" },
                    {nameof(TestModel.Number),$"{nameof(TestModel.Number)} <= @0" }};
            var filterLogic = FilterLogic.AND;

            var dataset = Enumerable.Range(0, 100)
                .Select(i => new TestModel(i, "test" + i, i * 2.3, (i % 2) == 0))
                .ToArray();

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, dataset);
            try
            {
                builder.Filter(whereClauses, filterLogic);
            }
            catch { /* no problem */ }

            var result = await builder.FilterImpl<TestModel?>(
                new QueryStringRequest(new Dictionary<string, StringValues>()
                {
                    {nameof(TestModel.Result), true.ToString() }, {nameof(TestModel.Number), 10.ToString() }
                }));

            var expectedData = dataset.Where(x => x.Result && x.Number <= 10).ToArray();
            Assert.Equal(expectedData, result.ToArray(), TestModelComparer);

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }

        [Fact]
        public async Task FilterImpl_Should_Return_FilteredData_OR_Logic()
        {
            var whereClauses = new Dictionary<string, string> {
                    {nameof(TestModel.Result),$"{nameof(TestModel.Result)} == @0" },
                    {nameof(TestModel.Number),$"{nameof(TestModel.Number)} <= @0" }};
            var filterLogic = FilterLogic.OR;

            var dataset = Enumerable.Range(0, 100)
                .Select(i => new TestModel(i, "test" + i, i * 2.3, (i % 2) == 0))
                .ToArray();

            var builder = SetupMock(out var mContext);
            SeedDataContext(mContext, dataset);
            try
            {
                builder.Filter(whereClauses, filterLogic);
            }
            catch { /* no problem */ }

            var result = await builder.FilterImpl<TestModel?>(
                new QueryStringRequest(new Dictionary<string, StringValues>()
                {
                    {nameof(TestModel.Result), true.ToString() }, {nameof(TestModel.Number), int.MaxValue.ToString() }
                }));

            var expectedData = dataset.ToArray();
            Assert.Equal(expectedData, result.ToArray(), TestModelComparer);

            mContext.Verify(x => x.Set<TestModel>(), Times.Once);
        }
    }
}
