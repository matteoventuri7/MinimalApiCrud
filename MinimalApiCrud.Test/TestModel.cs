namespace MinimalApiCrud.Test
{
    internal class TestModel : IEntity<int>, IEquatable<TestModel>
    {
        public TestModel(int id, string name, double number, bool result)
        {
            Id = id;
            Name = name;
            Number = number;
            Result = result;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Number { get; set; }
        public bool Result { get; set; }

        public bool Equals(TestModel? other) => Id == other?.Id;

        public override bool Equals(object? obj) => Equals(obj as TestModel);

        public override int GetHashCode() => Id.GetHashCode();
    }
}