using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiCrud
{
    public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> : IDisposable
        where Tid : struct
        where Tmodel : class, IEntity<Tid>
        where Tcontext : IDataContext<Tmodel>
    {
        private readonly IDataContext<Tmodel> _dbContext;
        private readonly IEndpointRouteBuilder _enpoints;
        private readonly IServiceScope _serviceScope;

        public MinimalApiCrudBuilder(IEndpointRouteBuilder endpoints)
        {
            _enpoints = endpoints;
            _serviceScope = endpoints.ServiceProvider.CreateScope();
            _dbContext = _serviceScope.ServiceProvider.GetRequiredService<Tcontext>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _serviceScope?.Dispose();
        }
    }
}
