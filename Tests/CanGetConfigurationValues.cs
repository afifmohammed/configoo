using System;
using System.Collections.Generic;
using Configoo;
using NUnit.Framework;
using Ninject;

namespace Tests
{
    public class TestConfigurationValues : IGetConfigurationValues
    {
        private readonly IDictionary<string, object> _values;
        public TestConfigurationValues()
        {
            _values = new Dictionary<string, object> {{"state", "VIC"}};
        }
        public IEnumerable<string> Keys
        {
            get { return _values.Keys; }
        }

        public TValue Get<TValue>(string key, TValue @default)
        {
            var lowered = key.Trim().ToLower();
            if(!_values.ContainsKey(lowered))
                _values.Add(lowered, @default);

            return (TValue)_values[lowered];
        }
    }
    
    public class TestConfigured : Configured
    {
        private TestConfigured() : base(new TestConfigurationValues())
        {}
    }

    [TestFixture]
    public class CanGetConfigurationValues
    {
        [Test]
        public void WhenCustomConfigurationIsLoaded()
        {
            string state;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                state = A<TestConfigured>.Value.For("state");
            }

            Assert.AreEqual("VIC", state);
        }

        [Test]
        public void WhenCustomConfigurationValuesAreLoaded()
        {
            string state;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                state = A<Configured>.Value.For("state");
            }

            Assert.AreEqual("VIC", state);
        }

        [Test]
        public void WhenDefaultStringValueProvidedForMissingKey()
        {
            string name;
            using(var k = new StandardKernel())
            {
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
                A<Configured>.Value.For(john);
                person = A<Configured>.Value.For<Person>();
            }

            Assert.IsNotNull(person);
            Assert.AreEqual(john.Age, person.Age);
        }
        private class Person { public int Age { get; set; } }
    }
}
