using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiCrud
{
    public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> : IDisposable
        where Tid : struct
        where Tmodel : class, IEntity<Tid>
        where Tcontext : class, IDataContext<Tmodel>
    {
        private readonly IDataContext<Tmodel> _dbContext;
        private readonly IEndpointRouteBuilder _enpoints;
        private readonly IServiceScope _serviceScope;

        public MinimalApiCrudBuilder(IEndpointRouteBuilder endpoints)
        {
            _enpoints = endpoints;
            var serviceScopeFactory = (IServiceScopeFactory)endpoints.ServiceProvider.GetService(typeof(IServiceScopeFactory))!;
            if (serviceScopeFactory is null)
            {
                throw new InvalidOperationException("Service scope factory not found.");
            }
            _serviceScope = serviceScopeFactory.CreateScope();
            _dbContext = (Tcontext)_serviceScope.ServiceProvider.GetService(typeof(Tcontext))!;
            if(_dbContext is null)
            {
                throw new InvalidOperationException("Data context service not found");
            }
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
