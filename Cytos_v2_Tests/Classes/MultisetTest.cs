using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Interfaces;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class MultisetTest
    {
        //Note: Multiset constructors are protected. Instead of creating test class it is used NamedMultiset class which already inherit Multiset.

        [TestMethod]
        public void TestConstructors()
        {
            NamedMultiset testConstructor1 = new NamedMultiset();
            Dictionary<string, int> data1 = testConstructor1.ToDictionary();
            Assert.AreEqual(0, data1.Count);

            Dictionary<string, int> testDictionary = new Dictionary<string, int> {{"key", 2}};
            NamedMultiset testConstructor2 = new NamedMultiset(testDictionary);
            Dictionary<string, int> data2 = testConstructor2.ToDictionary();
            Assert.AreEqual(1, data2.Count);
            Assert.IsTrue(data2.ContainsKey("key"));
            Assert.AreEqual(2, data2["key"]);

            List<ISimulationObject> testList = new List<ISimulationObject> {new Protein("testName")};
            NamedMultiset testConstructor3 = new NamedMultiset(testList);
            Dictionary<string, int> data3 = testConstructor3.ToDictionary();
            Assert.AreEqual(1, data3.Count);
            Assert.IsTrue(data3.ContainsKey("testName"));
            Assert.AreEqual(1, data3["testName"]);
        }

        [TestMethod]
        public void TestAdd()
        {
            NamedMultiset set = new NamedMultiset();
            Dictionary<string, int> data = set.ToDictionary();
            Assert.AreEqual(0, data.Count);

            set.Add("item1");
            set.Add("item2");

            data = set.ToDictionary();
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(1, data["item1"]);
            Assert.AreEqual(1, data["item2"]);

            set.Add("item1");
            data = set.ToDictionary();
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(2, data["item1"]);
            Assert.AreEqual(1, data["item2"]);
        }

        [TestMethod]
        public void TestClear()
        {
            NamedMultiset set = new NamedMultiset();
            Dictionary<string, int> data = set.ToDictionary();
            Assert.AreEqual(0, data.Count);

            set.Add("item1");
            set.Add("item2");

            data = set.ToDictionary();
            Assert.AreEqual(2, data.Count);

            set.Clear();
            data = set.ToDictionary();
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestIsSubsetOf()
        {
            NamedMultiset set1 = new NamedMultiset {"item1", "item2"};
            NamedMultiset set2 = new NamedMultiset {"item1"};

            Assert.IsTrue(set2.IsSubsetOf(set1));
            Assert.IsFalse(set1.IsSubsetOf(set2));

            set1.Add("item1");
            Assert.IsTrue(set2.IsSubsetOf(set1));
        }

        [TestMethod]
        public void TestEquals()
        {
            NamedMultiset set1 = new NamedMultiset {"item1"};
            NamedMultiset set2 = new NamedMultiset {"item1"};

            Assert.IsTrue(set1.Equals(set2));

            NamedMultiset set3 = new NamedMultiset();
            Assert.IsFalse(set1.Equals(set3));

            set1.Add("item1");
            Assert.IsFalse(set1.Equals(set2));
        }

        [TestMethod]
        public void TestRemove()
        {
            NamedMultiset set = new NamedMultiset {"item1", "item1", "item2"};

            Dictionary<string, int> data = set.ToDictionary();
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(2, data["item1"]);
            Assert.AreEqual(1, data["item2"]);

            Assert.IsTrue(set.Remove("item1"));

            data = set.ToDictionary();
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(1, data["item1"]);
            Assert.AreEqual(1, data["item2"]);

            Assert.IsTrue(set.Remove("item1"));

            data = set.ToDictionary();
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(1, data["item2"]);

            Assert.IsFalse(set.Remove("item3"));
            Assert.IsFalse(set.Remove("item1"));
        }

        [TestMethod]
        public void TestToString()
        {
            NamedMultiset set = new NamedMultiset {"item1", "item1", "item2"};
            Assert.AreEqual("[item1, 2]\n[item2, 1]", set.ToString());
        }
    }
}
