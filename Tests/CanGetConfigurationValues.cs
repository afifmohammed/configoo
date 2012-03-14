﻿using System;
using System.Collections.Generic;
using Configoo;
using NUnit.Framework;
using Ninject;

namespace Tests
{
    public class TestConfigurationValues : IGetConfigurationValues
    {
        public IDictionary<string, object> List
        {
            get { return new Dictionary<string, object>() { { "state", "VIC" } }; }
        }
    }

    [TestFixture]
    public class CanGetConfigurationValues
    {
        [TearDown]
        public void Clear()
        {
            Configured._value = null;
        }

        [Test]
        public void WhenCustomConfigurationValuesAreLoaded()
        {
            string state;
            using (var k = new StandardKernel())
            {
                k.Load<Configooness>();
                state = Configured.Value.For("state");
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
                age = Configured.Value.For("Age", @default: 31);
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
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
                k.Load(new Configooness(s => s.Excluding<TestConfigurationValues>()));
                Assert.Throws<KeyNotFoundException>(() => Configured.Value.For("name"));
            }
        }

        [Test]
        public void ShouldThrowWhenModuleNotLoadedInKernel()
        {
            Assert.Throws<InvalidOperationException>(() => { var v = Configured.Value; });
        }

        private class Person { public int Age { get; set; } }
    }
}
