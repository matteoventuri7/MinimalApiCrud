using System.Collections;

namespace MinimalApiCrud
{
    public interface IResponseAdapter
    {
        IResponse GetResult(IEnumerable items, int totalCount);
    }
}