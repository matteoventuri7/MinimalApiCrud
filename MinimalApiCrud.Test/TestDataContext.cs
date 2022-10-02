namespace MinimalApiCrud.Test
{
    public class TestDataContext : IDataContext<TestModel>
    {
        public virtual ValueTask<int> AddAsync(TestModel model)
        {
            return ValueTask.FromResult(0);
        }

        public virtual ValueTask<int> RemoveAsync(TestModel model)
        {
            return ValueTask.FromResult(0);
        }

        public virtual IQueryable<TestModel> Set<T>() where T : class
        {
            return null!;
        }

        public virtual ValueTask<int> UpdateAsync(TestModel model)
        {
            return ValueTask.FromResult(0);
        }
    }
}