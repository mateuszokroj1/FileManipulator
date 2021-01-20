using System.Linq;

using Xunit;

namespace Core.Tests.Helpers
{
    public class AlphanumericComparerTest : IClassFixture<AlphanumericComparerFixture>
    {
        public AlphanumericComparerTest(AlphanumericComparerFixture fixture)
        {
            this.fixture = fixture;
        }

        private AlphanumericComparerFixture fixture;

        [Fact]
        public void Comparer_CommonUseInSorter()
        {
            var list = new string[]
            {
                "0",
                "1",
                "12",
                "a300",
                "13",
                "0100",
                "az143 2343.t",
                "az143 2343.e"
            };

            var result = list.OrderBy(item => item, fixture.Comparer);

            Assert.Equal(8, result.Count());

            Assert.Equal("0", result.ElementAt(0));
            Assert.Equal("1", result.ElementAt(1));
            Assert.Equal("12", result.ElementAt(2));
            Assert.Equal("13", result.ElementAt(3));
            Assert.Equal("0100", result.ElementAt(4));
            Assert.Equal("az143 2343.e", result.ElementAt(5));
            Assert.Equal("az143 2343.e", result.ElementAt(6));
        }
    }
}
