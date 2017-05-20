using FluentAssertions;
using NUnit.Framework;

namespace MyProject.Tests
{
    [TestFixture]
    public class GreaterTests
    {
        [Test]
        public void SayHello_ShouldReturnNotEmptyString()
        {
            Greater.SayHello().Should().NotBeEmpty("Because only invalid greeting can be empty");
        }
    }
}
