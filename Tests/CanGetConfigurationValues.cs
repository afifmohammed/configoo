using System.Collections.Generic;
using Configoo;
using NUnit.Framework;
using Ninject;

namespace Tests
{
    [TestFixture]
    public class CanGetConfigurationValues
    {
        [Test]
        public void WhenDefaultStringValueProvidedForMissingKey()
        {
            string name;
            using(var k = new StandardKernel())
            {
                k.Load<Configooness>();
                name = Configured.Value.For("name", @default: "jack");
            }

            Assert.AreEqual("jack", name);
        }

        [Test]
        public void WhenDefaultIntValueProvidedForMissingKey()
        {
            int age;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                age = Configured.Value.For("age", @default: 22);
            }

            Assert.AreEqual(22, age);
        }

        [Test]
        public void WhenDefaultIntValueProvidedForMissingKeyForDelegate()
        {
            int age;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                age = (int)Configured.Value.For<Person>(x => x.Age, @default: 22);
            }

            Assert.AreEqual(22, age);
        }

        [Test]
        public void WhenDefaultClassValueProvidedForMissingKey()
        {
            Person person;
            var @default = new Person {Age = 25};
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                person = Configured.Value.For("John", @default);
            }

            Assert.AreEqual(@default.Age, person.Age);
        }

        [Test]
        public void WhenKeyMatchesPredicate()
        {
            int weight;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                Configured.Value.For("weight", @default: 49);

                weight = Configured.Value.For<int>(x => x == "weight");
            }

            Assert.AreEqual(49, weight);
        }


        [Test]
        public void ShouldThrowWhenKeyMissingWithNoDefault()
        {
            using(var k = new StandardKernel())
            {
                k.Load<Configooness>();
                Assert.Throws<KeyNotFoundException>(() => Configured.Value.For("name"));
            }
        }

        private class Person { public int Age { get; set; } }
    }
}
