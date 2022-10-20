using System.Collections;

namespace MinimalApiCrud
{
    public class DefaultResponseAdapter : IResponseAdapter
    {
        public virtual IResponse GetResult(IEnumerable items, int totalCount)
        {
            return new DefaultResponse(totalCount, items);
        }
    }
}