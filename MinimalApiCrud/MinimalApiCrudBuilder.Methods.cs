using Mapster;

namespace MinimalApiCrud
{
	public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext>
	{
		public async Task<T?> LoadOneById<T>(Tcontext ctx, Tid id)
		{
			return await Task.Factory.StartNew(() => ctx.Set<Tmodel>()
				.Where(x => x.Id.Equals(id))
				.ProjectToType<T>()
				.SingleOrDefault());
		}

		public async Task<Tmodel?> LoadOneById(Tcontext ctx, Tid id)
		{
			return await Task.Factory.StartNew(() => ctx.Set<Tmodel>()
				.Where(x => x.Id.Equals(id))
				.SingleOrDefault());
		}
	}
}
