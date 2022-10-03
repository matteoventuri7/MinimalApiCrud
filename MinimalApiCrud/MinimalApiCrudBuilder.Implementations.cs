using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace MinimalApiCrud
{
	public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext>
	{
        public async Task<IResult> InsertImpl<Tentity>(Tentity data)
		{
            if (data is null)
                return Results.BadRequest();

            var model = data.Adapt<Tmodel>();
            var addResult = await GetDataContextService.AddAsync(model);

            if(addResult == 0)
            {
                return Results.Conflict("No entity added");
            }

            return Results.Created(string.Empty, model.Adapt<Tentity>());
        }

        public async Task<IResult> UpdateImpl<Tentity>(Tid id, Tentity data)
        {
            if (data is null)
                return Results.BadRequest();

            var ctx = GetDataContextService;
            var modelDB = await LoadOneById(ctx, id);

            if (modelDB is null)
                return Results.NotFound(id);

            var toUpdate = data.Adapt(modelDB);
            await ctx.UpdateAsync(toUpdate);

            return Results.Ok();
        }

        public async Task<IResult> DeleteImpl(Tid id)
        {
            var ctx = GetDataContextService;
            var data = await LoadOneById<Tmodel>(ctx, id);

            if (data is null)
                return Results.NotFound(id);

            await ctx.RemoveAsync(data);

            return Results.NoContent();
        }

        public async Task<IEnumerable<Tentity>> GetAllImpl<Tentity>(int? pageNumber = null, int? pageSize = null)
        {
            if (pageNumber is not null && pageSize is not null)
            {
                return await Task.Factory.StartNew(() => GetDataContextService.Set<Tmodel>()
                        .Skip((pageNumber.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value)
                        .ProjectToType<Tentity>()
                        .ToArray());
            }

            return await Task.Factory.StartNew(() => GetDataContextService.Set<Tmodel>()
                        .ProjectToType<Tentity>()
                        .ToArray());
        }

        public async Task<Tentity?> GetOneByIdImpl<Tentity>(Tid id)
        {
            return await LoadOneById<Tentity>(GetDataContextService, id);
        }

        public async Task<IEnumerable<Tentity>> FilterImpl<Tentity>(QueryStringRequest request)
        {
            var q = GetDataContextService.Set<Tmodel>();

            var expressions = new List<Expression<Func<Tmodel, bool>>>(_filterWhereClauses.Count);
            var placeholdersWhere = new List<string>(_filterWhereClauses.Count);

            int i = 0;
            foreach (var kv in _filterWhereClauses)
            {
                if (request.TryGetValue(kv.Key, out var fieldFilter))
                {
                    var dwp = DynamicExpressionParser.ParseLambda<Tmodel, bool>(ParsingConfig.Default, true, kv.Value, fieldFilter);
                    expressions.Add(dwp);
                    placeholdersWhere.Add($"@{i++}(it)");
                }
            }

            if (placeholdersWhere.Count > 0)
            {
                string fullClause = null!;
                string logicOperator = null!;
                if (_filterLogic is FilterLogic.AND)
                    logicOperator = " && ";
                else
                    logicOperator = " || ";
                fullClause = string.Join(logicOperator, placeholdersWhere);
                q = q.Where(fullClause, expressions.ToArray());
            }

            return await Task.Factory.StartNew(() => q
                    .ProjectToType<Tentity>()
                    .ToArray());
        }
    }
}
