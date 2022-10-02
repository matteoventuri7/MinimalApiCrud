using Microsoft.AspNetCore.Routing;

namespace MinimalApiCrud
{
    public static class MapCrudExtensions
    {
        public static MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> MapCrud<Tmodel, Tid, Tcontext>(this IEndpointRouteBuilder endpoints)
            where Tid : struct
            where Tmodel : class, IEntity<Tid>
            where Tcontext : class, IDataContext<Tmodel>
        {
            return new MinimalApiCrudBuilder<Tmodel, Tid, Tcontext>(endpoints);
        }
    }
}