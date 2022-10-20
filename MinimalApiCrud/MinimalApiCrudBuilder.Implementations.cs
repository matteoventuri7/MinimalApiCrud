using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
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

        public async Task<IResponse> GetAllImpl<Tentity>(int? pageNumber, int? pageSize)
        {
            IEnumerable<Tentity> items;
            int totalCount = 0;

            if (pageNumber is not null && pageSize is not null)
            {
                items = await Task.Factory.StartNew(() =>
                {
                    totalCount = GetDataContextService.Set<Tmodel>().Count();
                    return GetDataContextService.Set<Tmodel>()
                        .Skip((pageNumber.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value)
                        .ProjectToType<Tentity>()
                        .ToArray();
                });
            }
            else
            {
                items = await Task.Factory.StartNew(() =>
                {
                    totalCount = GetDataContextService.Set<Tmodel>().Count();
                    return GetDataContextService.Set<Tmodel>()
                        .ProjectToType<Tentity>()
                        .ToArray();
                });
            }

            return _getAllResponseAdapter.GetResult(items, totalCount);
        }

        public async Task<Tentity?> GetOneByIdImpl<Tentity>(Tid id)
        {
            return await LoadOneById<Tentity>(GetDataContextService, id);
        }

        public async Task<IResponse> FilterImpl<Tentity>(QueryStringRequest request)
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

            int totalCount = 0;

            var items = await Task.Factory.StartNew(() =>
            {
                totalCount = q.Count();
                return q
                    .ProjectToType<Tentity>()
                    .ToArray();
            });

            return _filterResponseAdapter.GetResult(items, totalCount);
        }
    }
}
