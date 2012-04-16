using System;
using System.Collections.Generic;
using Configoo;
using NUnit.Framework;
using Ninject;

namespace Tests
{
    public class TestAppConfig : Configured
    {
        protected TestAppConfig(LookupValues lookupValues) : base(lookupValues)
        {
            lookupValues.Get("howlongisapieceofstring", "9876543210.9876543210");
            lookupValues.Get("dob", "1995-10-10");
        }
    }

    [TestFixture]
    public class CanGetConfigurationValues
    {
        [Test]
        public void WhenDecimalIsRequestedFromAppConfig()
        {
            decimal answer;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                answer = A<TestAppConfig>.Value.For<decimal>("howlongisapieceofstring");
            }

            Assert.AreEqual(9876543210.9876543210, answer);
        }

        [Test]
        public void WhenMissingKeyWithDefaultValueIsRequestedFromAppConfig()
        {
            int age;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                A<TestAppConfig>.Value.For("age", 23);
                age = A<TestAppConfig>.Value.For<int>("age");
            }

            Assert.AreEqual(23, age);
        }

        [Test]
        public void WhenDateIsRequestedFromAppConfig()
        {
            DateTime dob;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                dob = A<TestAppConfig>.Value.For<DateTime>("dob");
            }

            Assert.AreEqual(new DateTime(1995, 10, 10), dob);
        }

        [Test]
        public void WhenDefaultStringValueProvidedForMissingKey()
        {
            string name;
            using(var k = new StandardKernel())
            {
                k.Load(new Configooness());
                name = A<Configured>.Value.For("name", @default: "jack");
            }

            Assert.AreEqual("jack", name);
        }

        [Test]
        public void WhenDefaultIntValueProvidedForMissingKey()
        {
            int age;
            using (var k = new StandardKernel())
            {
                k.Load(new Configooness());
                age = A<Configured>.Value.For("Age", @default: 31);
            }
            
            Assert.AreEqual(31, age);
        }

        [Test]
        public void WhenDefaultIntValueProvidedForMissingKeyForDelegate()
        {
            int age;
            using (var k = new StandardKernel())
            {
                k.Load(new Configooness());
                age = A<Configured>.Value.For<Person, int>(x => x.Age, @default: 22);
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
                k.Load(new Configooness());
                person = A<Configured>.Value.For("John", @default);
            }

            Assert.AreEqual(@default.Age, person.Age);
        }

        [Test]
        public void WhenKeyMatchesPredicate()
        {
            int weight;
            using (var k = new StandardKernel())
            {
                k.Load(new Configooness());
                A<Configured>.Value.For("weight", @default: 49);

                weight = A<Configured>.Value.For<int>(x => x == "weight");
            }

            Assert.AreEqual(49, weight);
        }


        [Test]
        public void ShouldThrowWhenKeyMissingWithNoDefault()
        {
            using(var k = new StandardKernel())
            {
                k.Load(new Configooness());
                Assert.Throws<KeyNotFoundException>(() => A<Configured>.Value.For("name"));
            }
        }

        [Test]
        public void ShouldThrowWhenModuleNotLoadedInKernel()
        {
            Assert.Throws<InvalidOperationException>(() => { var v = A<Configured>.Value; });
        }

        [Test]
        public void ShouldResolveClassByName()
        {
            Person person;
            var john = new Person { Age = 25 };
            using (var k = new StandardKernel())
            {
                k.Load(new Configooness());
                A<Configured>.Value.For(john);
                person = A<Configured>.Value.For<Person>();
            }

            Assert.IsNotNull(person);
            Assert.AreEqual(john.Age, person.Age);
        }
        private class Person { public int Age { get; set; } }
    }
}
