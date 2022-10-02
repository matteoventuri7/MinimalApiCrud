using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace MinimalApiCrud
{
    public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext>
    {
        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Insert(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => Insert<Tmodel>(pattern, config);
        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Insert<Tentity>(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapPost(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}", InsertImpl<Tentity>);

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Update(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => Update<Tmodel>(pattern, config);
        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Update<Tentity>(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapPut(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}", async (Tid id, Tentity data) =>
            {
                if (data is null)
                    return Results.BadRequest();

                var modelDB = await LoadOneById(id);

                if (modelDB is null)
                    return Results.NotFound(id);

                data.Adapt<Tentity, Tmodel>(modelDB);
                await _dbContext.UpdateAsync(modelDB);

                return Results.Ok();
            });

            if (config is not null)
                config(builder);

            return this;
        }
                
        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Delete(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapDelete(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}", async (Tid id) =>
            {
                var data = await LoadOneById<Tmodel>(id);

                if (data is null)
                    return Results.NotFound(id);

                await _dbContext.RemoveAsync(data);

                return Results.NoContent();
            });

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetAll(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => GetAll<Tmodel>(pattern, config);

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetAll<Tentity>(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapGet(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}/list", async (int? pageNumber, int? pageSize) =>
            {
                if (pageNumber is not null && pageSize is not null)
                {
                    return await Task.Factory.StartNew(() => _dbContext.Set<Tmodel>()
                            .Skip((pageNumber.Value - 1) * pageSize.Value)
                            .Take(pageSize.Value)
                            .ProjectToType<Tentity>()
                            .ToList());
                }

                return await Task.Factory.StartNew(() => _dbContext.Set<Tmodel>()
                            .ProjectToType<Tentity>()
                            .ToList());
            });

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetOneById(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => GetOneById<Tmodel>(pattern, config);

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetOneById<Tentity>(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapGet(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}/find", async (Tid id) =>
            {
                return await LoadOneById<Tentity>(id);
            });

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Filter(Dictionary<string, string> whereClauses, FilterLogic filterLogic, string? pattern = null,
            Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => Filter<Tmodel>(whereClauses, filterLogic, pattern, config);

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Filter<Tentity>(Dictionary<string, string> whereClauses, FilterLogic filterLogic, string? pattern = null,
            Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapGet(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}/filter", async (HttpContext context) =>
            {
                var q = _dbContext.Set<Tmodel>();

                var expressions = new List<Expression<Func<Tmodel, bool>>>(whereClauses.Count);
                var placeholdersWhere = new List<string>(whereClauses.Count);

                int i = 0;
                foreach (var kv in whereClauses)
                {
                    if (context.Request.Query.TryGetValue(kv.Key, out StringValues fieldFilter))
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
                    if (filterLogic is FilterLogic.AND)
                        logicOperator = " && ";
                    else
                        logicOperator = " || ";
                    fullClause = string.Join(logicOperator, placeholdersWhere);
                    q = q.Where(fullClause, expressions.ToArray());
                }

                return await Task.Factory.StartNew(() => q
                        .ProjectToType<Tentity>()
                        .ToList());
            });

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> SetupMapping<Tsource, Tdest>(Action<TypeAdapterSetter<Tsource, Tdest>> mappingConfig)
        {
            mappingConfig(TypeAdapterConfig<Tsource, Tdest>.ForType());
            return this;
        }
    }
}