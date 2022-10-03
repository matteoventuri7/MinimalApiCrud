using System.Diagnostics.CodeAnalysis;

namespace MinimalApiCrud.Test
{
    internal class TestModelComparer : IEqualityComparer<TestModel?>
    {
        public bool Equals(TestModel? x, TestModel? y)
        {
            return x?.Id == y?.Id &&
                x?.Name == y?.Name &&
                x?.Result == y?.Result &&
                x?.Number == y?.Number;
        }

        public int GetHashCode([DisallowNull] TestModel obj)
        {
            return HashCode.Combine(obj.Id, obj.Name, obj.Result, obj.Number);
        }
    }
}