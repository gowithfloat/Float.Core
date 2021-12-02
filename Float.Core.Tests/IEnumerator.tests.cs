// <copyright file="IEnumerator.tests.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Extensions;
using Xunit;

namespace Float.Core.Tests
{
    public class IEnumeratorTests
    {
        [Fact]
        public void TestNullIfEmpty()
        {
            IEnumerable<object> list1 = new List<object> { };
            Assert.Null(list1.NullIfEmpty());

            IEnumerable<object> list2 = new List<object> { "one", 2, new { value = 3 } };
            Assert.NotNull(list2.NullIfEmpty());
        }

        [Fact]
        public void TestForEach()
        {
            IEnumerable<object> list1 = new List<object> { "one", 2, new { value = 3 } };
            var count = 0;

            list1.ForEach(e => count += 1);

            Assert.Equal(3, count);
        }

        [Fact]
        public void TestZip()
        {
            var list1 = new List<int> { 1, 2, 3 };
            var list2 = new List<float> { 1.1f, 2.2f, 3.3f };
            var list3 = new List<string> { "one", "two", "three" };

            var list4 = list1.Zip(list2, list3, (a, b, c) => $"{c} is {b} is {a}");

            Assert.Equal("one is 1.1 is 1", list4.ElementAt(0));
            Assert.Equal("two is 2.2 is 2", list4.ElementAt(1));
            Assert.Equal("three is 3.3 is 3", list4.ElementAt(2));
        }

        [Fact]
        public void TestKeys()
        {
            IEnumerable<KeyValuePair<string, object>> dict1 = new Dictionary<string, object>
            {
                { "first", new { Value = 1 } },
                { "second", new { Value = 2 } },
                { "third", new { Value = 3 } }
            };

            var keys = dict1.Keys();

            foreach (var key in keys)
            {
                Assert.False(string.IsNullOrWhiteSpace(key));
            }
        }

        [Fact]
        public void TestValues()
        {
            IEnumerable<KeyValuePair<string, object>> dict1 = new Dictionary<string, object>
            {
                { "first", new { Value = 1 } },
                { "second", new { Value = 2 } },
                { "third", new { Value = 3 } }
            };

            var values = dict1.Values();

            foreach (var value in values)
            {
                Assert.NotNull(value);
            }
        }

        [Fact]
        public void TestToDictionary()
        {
            IEnumerable<KeyValuePair<string, int>> dict1 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("first", 111),
                new KeyValuePair<string, int>("second", 222),
                new KeyValuePair<string, int>("third", 333)
            };

            var dict2 = dict1.ToDictionary();

            Assert.Equal(111, dict2["first"]);
            Assert.Equal(222, dict2["second"]);
            Assert.Equal(333, dict2["third"]);
        }

        [Fact]
        public void TestAny()
        {
            IEnumerable list1 = new List<string> { };
            Assert.False(list1.Any());

            IEnumerable list2 = new List<string> { "any" };
            Assert.True(list2.Any());
        }

        [Fact]
        public void TestAnyFunc()
        {
            IEnumerable list1 = new List<string> { };
            Assert.False(list1.Any(el => (el as string) == "test"));

            IEnumerable list2 = new List<string> { "any", "test", "none" };
            Assert.True(list2.Any(el => (el as string) == "test"));
        }

        class TestClass1
        {
        }

        class TestClass2: TestClass1
        {
        }

        [Fact]
        public void TestContainsInstanceOf()
        {
            IEnumerable list1 = new List<object> { new Uri("https://www.gowithfloat.com"), "float", 37, new TestClass2() };
            Assert.True(list1.ContainsInstanceOf<Uri>());
            Assert.True(list1.ContainsInstanceOf<string>());
            Assert.True(list1.ContainsInstanceOf<int>());
            Assert.False(list1.ContainsInstanceOf<TestClass1>());
            Assert.True(list1.ContainsInstanceOf<TestClass2>());
        }

        [Fact]
        public void TestContainsSubclassOf()
        {
            IEnumerable list1 = new List<object> { new Uri("https://www.gowithfloat.com"), "float", 37, new TestClass2() };
            Assert.True(list1.ContainsSubclassOf<TestClass1>());
            Assert.False(list1.ContainsSubclassOf<TestClass2>());
        }
    }
}
