using System.Collections;

namespace MinimalApiCrud
{
    public class DefaultResponse : IResponse
    {
        public int Count { get; set; }

        public IEnumerable Items { get; set; }

        public DefaultResponse(int count, IEnumerable items)
        {
            Count = count;
            Items = items ?? new object[0];
        }
    }
}