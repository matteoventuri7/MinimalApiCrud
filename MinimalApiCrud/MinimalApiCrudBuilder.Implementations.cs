using Mapster;
using Microsoft.AspNetCore.Http;

namespace MinimalApiCrud
{
	public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext>
	{
        public async Task<IResult> InsertImpl<Tentity>(Tentity data)
		{
            if (data is null)
                return Results.BadRequest();

            var model = data.Adapt<Tmodel>();
            var addResult = await _dbContext.AddAsync(model);

            if(addResult == 0)
            {
                return Results.Conflict("No entity added");
            }

            return Results.Created(string.Empty, model.Adapt<Tentity>());
        }
	}
}
