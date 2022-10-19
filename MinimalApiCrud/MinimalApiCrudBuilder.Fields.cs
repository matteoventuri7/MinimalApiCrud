using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiCrud
{
    public partial class MinimalApiCrudBuilder<Tmodel, Tid, Tcontext> : IDisposable
        where Tmodel : class, IEntity<Tid>
        where Tcontext : class, IDataContext<Tmodel>
    {
        private Tcontext GetDataContextService { get => (Tcontext)_serviceScope.ServiceProvider.GetService(typeof(Tcontext))!; }
        private readonly IEndpointRouteBuilder _enpoints;
        private readonly IServiceScope _serviceScope;
        private Dictionary<string, string> _filterWhereClauses = null!;
        private FilterLogic _filterLogic;

        public MinimalApiCrudBuilder(IEndpointRouteBuilder endpoints)
        {
            _enpoints = endpoints;
            var serviceScopeFactory = (IServiceScopeFactory)endpoints.ServiceProvider.GetService(typeof(IServiceScopeFactory))!;
            _serviceScope = serviceScopeFactory.CreateScope();
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
