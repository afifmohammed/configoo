using System.Collections.Generic;
using Configoo;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class CanGetConfiguredValue
    {
        [Test]
        public void WhenDecimalIsRequestedFromAppConfig()
        {
            var lookup = new AppConfig();
            
            lookup.Get("howlongisapieceofstring", "9876543210.9876543210");
            lookup.Get("dob", "1995-10-10");
            
            Assert.That(lookup.Value().For<decimal>("howlongisapieceofstring"), Is.EqualTo(9876543210.9876543210));
        }

        [Test]
        public void WhenMissingKeyWithDefaultValueIsRequestedFromLookup()
        {
            var lookup = new AppConfig();
            
            lookup.Value().For("age", 23);

            Assert.That(lookup.Value().For<int>("age"), Is.EqualTo(23));
        }

        [Test]
        public void ShouldThrowWhenKeyMissingWithNoDefault()
        {
            Assert.Throws<KeyNotFoundException>(() => new AppConfig().Value().For("name"));
        }

        [Test]
        public void WhenDefaultClassValueProvidedForMissingKey()
        {
            var @default = new Person { Age = 25 };

            var person = new AppConfig().Value().For("jon", @default);
            
            Assert.That(person.Age, Is.EqualTo(@default.Age));
        }

        private class Person { public int Age { get; set; } }
    }
}