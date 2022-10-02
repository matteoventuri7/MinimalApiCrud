using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace MinimalApiCrud
{
    public sealed class QueryStringRequest : Dictionary<string, StringValues>
    {
        public QueryStringRequest(IEnumerable<KeyValuePair<string, StringValues>> data) : base(data, StringComparer.OrdinalIgnoreCase)
        {

        }

        public static ValueTask<QueryStringRequest> BindAsync(HttpContext httpContext, ParameterInfo parameter)
        {
            return ValueTask.FromResult(new QueryStringRequest(httpContext.Request.Query));
        }
    }
}