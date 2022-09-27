using Mapster;

namespace MinimalApiCrud
{
	public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext>
	{
		public async Task<T?> LoadOneById<T>(Tid id)
		{
			return await Task.Factory.StartNew(() => _dbContext.Set<Tmodel>()
				.Where(x => x.Id.Equals(id))
				.ProjectToType<T>()
				.SingleOrDefault());
		}

		public async Task<Tmodel?> LoadOneById(Tid id)
		{
			return await Task.Factory.StartNew(() => _dbContext.Set<Tmodel>()
				.Where(x => x.Id.Equals(id))
				.SingleOrDefault());
		}
	}
}
