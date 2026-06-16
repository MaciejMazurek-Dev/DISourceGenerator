namespace DISourceGenerator.Library
{
    internal record struct ClassToGenerate
    {
        public readonly string Name;

        public ClassToGenerate(string className)
        {
            Name = className;
        }
    }
}
