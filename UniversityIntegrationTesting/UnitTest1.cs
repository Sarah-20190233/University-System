using Microsoft.AspNetCore.Builder;
using Xunit;

namespace UniversityIntegrationTesting
{
    public class Tests :IClassFixture<CustomWebApplicationFactory<Program>>
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}