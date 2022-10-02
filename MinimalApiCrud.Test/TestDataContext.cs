namespace MinimalApiCrud.Test
{
    internal class TestDataContext : IDataContext<TestModel>
    {
        private HashSet<TestModel> _data;

        public TestDataContext(int seedDataCount)
        {
            SeedData(seedDataCount);
        }

        private void SeedData(int seedDataCount)
        {
            _data = new HashSet<TestModel>(seedDataCount);
            var rnd = new Random((int)DateTime.UtcNow.Ticks);

            for (int i = 1; i <= seedDataCount; i++)
            {
                _data.Add(new TestModel(i, "test" + i, rnd.NextDouble() * 10, rnd.Next(0, 2) == 0));
            }
        }

        public ValueTask<int> AddAsync(TestModel model)
        {
            var countPreAdd = _data.Count;
            _data.Add(model);
            return ValueTask.FromResult(_data.Count - countPreAdd);
        }

        public ValueTask<int> RemoveAsync(TestModel model)
        {
            var removed = _data.Remove(model);
            if (!removed)
            {
                throw new Exception("Not found");
            }
            return ValueTask.FromResult(1);
        }

        public IQueryable<TestModel> Set<T>() where T : class
        {
            return _data.AsQueryable();
        }

        public async ValueTask<int> UpdateAsync(TestModel model)
        {
            await RemoveAsync(model);
            return await AddAsync(model);
        }
    }
}