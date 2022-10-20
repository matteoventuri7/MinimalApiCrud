using System.Collections;

namespace MinimalApiCrud
{
    public interface IResponse
    {
        int Count { get; }
        IEnumerable Items { get; }
    }
}