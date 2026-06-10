using Xunit;
using VerifyXunit;

namespace DISourceGenerator.Tests
{
    public class GeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesSourceCodeCorrectly()
        {
            string source = @"
                              [Marker]
                              public class partial TestClass
                              {
                              }
                              ";

            return TestHelper.Verify(source);
        }
    }
}
