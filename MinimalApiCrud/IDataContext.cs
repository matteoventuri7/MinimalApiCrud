namespace MinimalApiCrud
{
    public interface IDataContext<Tmodel>
        where Tmodel : class
    {
        IQueryable<Tmodel> Set<T>() where T : class;
        ValueTask<int> AddAsync(Tmodel model);
        ValueTask<int> RemoveAsync(Tmodel model);
        ValueTask<int> UpdateAsync(Tmodel model);
    }
}