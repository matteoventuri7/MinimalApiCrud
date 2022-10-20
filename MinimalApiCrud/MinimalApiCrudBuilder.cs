using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

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
            var builder = _enpoints.MapPut(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}", UpdateImpl<Tentity>);

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Delete(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapDelete(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}", DeleteImpl);

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetAll(string? pattern = null, IResponseAdapter? responseAdapter = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => GetAll<Tmodel>(pattern, responseAdapter, config);

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetAll<Tentity>(string? pattern = null, IResponseAdapter? responseAdapter = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            _getAllResponseAdapter = _getAllResponseAdapter ?? responseAdapter!;

            var builder = _enpoints.MapGet(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}/list", GetAllImpl<Tentity>);

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetOneById(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => GetOneById<Tmodel>(pattern, config);

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> GetOneById<Tentity>(string? pattern = null, Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            var builder = _enpoints.MapGet(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}/find", GetOneByIdImpl<Tentity>);

            if (config is not null)
                config(builder);

            return this;
        }

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Filter(Dictionary<string, string> whereClauses, FilterLogic filterLogic, string? pattern = null,
            IResponseAdapter? responseAdapter = null,
            Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null) => Filter<Tmodel>(whereClauses, filterLogic, pattern, responseAdapter, config);

        public MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> Filter<Tentity>(Dictionary<string, string> whereClauses, FilterLogic filterLogic, string? pattern = null,
            IResponseAdapter? responseAdapter = null,
            Func<RouteHandlerBuilder, RouteHandlerBuilder>? config = null)
        {
            _filterWhereClauses = whereClauses;
            _filterLogic = filterLogic;
            _filterResponseAdapter = _filterResponseAdapter ?? responseAdapter!;

            var builder = _enpoints.MapGet(pattern ?? $"/{typeof(Tmodel).Name.ToLowerInvariant()}/filter", FilterImpl<Tentity>);

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