using System.Collections.Generic;
using System.Collections.ObjectModel;
using JulMar.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for ObjectClonerTest and is intended
    ///to contain all ObjectClonerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjectClonerTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void NullClone()
        {
            string source = null;
            string clone = ObjectCloner.Clone(source);
            Assert.IsNull(clone);
        }


        [TestMethod()]
        public void StringClone()
        {
            const string source = "This is a string";
            string copy = ObjectCloner.Clone(source);
            Assert.AreEqual(copy, source);
        }

        [TestMethod()]
        public void IntClone()
        {
            int source = 5;
            int copy = ObjectCloner.Clone(source);
            Assert.AreEqual(copy, source);
        }

        [TestMethod()]
        public void DoubleClone()
        {
            double source = Math.PI;
            double copy = ObjectCloner.Clone(source);
            Assert.AreEqual(copy, source);
        }

        [TestMethod()]
        public void FloatClone()
        {
            float source = (float) Math.PI;
            float copy = ObjectCloner.Clone(source);
            Assert.AreEqual(copy, source);
        }

        class NoConstructorTestObject
        {
            private NoConstructorTestObject()
            {
            }
            public NoConstructorTestObject(int i)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void TestCannotCreate()
        {
            NoConstructorTestObject source = new NoConstructorTestObject(int.MaxValue);
            var copy = ObjectCloner.Clone(source);
            Assert.AreEqual(source, copy);
        }

        class SimpleTestClass
        {
            private int intVal;
            private string stringVal;

            public SimpleTestClass()
            {
            }

            public SimpleTestClass(int iVal, string sVal)
            {
                intVal = iVal;
                stringVal = sVal;
            }

            public bool Compare(SimpleTestClass other)
            {
                return this != other && intVal == other.intVal && stringVal == other.stringVal;
            }
        }

        [TestMethod]
        public void TestSimpleObject()
        {
            SimpleTestClass source = new SimpleTestClass(int.MaxValue, "Test123");
            SimpleTestClass copy = ObjectCloner.Clone(source);
            Assert.AreNotEqual(source,copy);
            Assert.IsTrue(source.Compare(copy));
        }

        [TestMethod]
        public void TestArray_VT()
        {
            Random rnd = new Random();
            int[] source = new int[100];
            for (int i = 0; i < source.Length; i++)
                source[i] = rnd.Next();

            int[] clone = ObjectCloner.Clone(source);
            CollectionAssert.AreEqual(clone, source);
        }

        [TestMethod]
        public void TestArray_MD()
        {
            Random rnd = new Random();
            int[,] source = new int[10,10];
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    source[x,y] = rnd.Next();

            int[,] clone = ObjectCloner.Clone(source);
            CollectionAssert.AreEqual(clone, source);
        }

        [TestMethod]
        public void TestArray_Jagged()
        {
            Random rnd = new Random();
            var source = new[] { new[]{1,2}, new[]{3,4,5}, new[]{6,7,8,9}, new[]{10,11,12} }; 
            var clone = ObjectCloner.Clone(source);

            Assert.AreEqual(clone.Length, source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                CollectionAssert.AreEqual(clone[i], source[i]);
            }
        }

        [TestMethod]
        public void ComplexObjectTest()
        {
            DataTable dt = new DataTable("TestTable");
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));

            dt.LoadDataRow(new object[] { "Mark", 42 }, true);
            dt.LoadDataRow(new object[] { "Julie", 36 }, true);
            dt.LoadDataRow(new object[] { "Amanda", 11 }, true);
            dt.LoadDataRow(new object[] { "Caroline", 9 }, true);
            dt.LoadDataRow(new object[] { "Cassidy", 6 }, true);
            dt.LoadDataRow(new object[] { "Abby", 3 }, true);

            DataTable clone = ObjectCloner.Clone(dt);
            Assert.AreEqual(clone.Columns.Count, dt.Columns.Count);
            Assert.AreEqual(clone.Columns[0].ColumnName, "Name");
            Assert.AreEqual(clone.Columns[1].ColumnName, "Age");

            Assert.AreEqual(clone.Rows.Count, dt.Rows.Count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Assert.AreEqual(clone.Rows[i]["Name"], dt.Rows[i]["Name"]);
                Assert.AreEqual(clone.Rows[i]["Age"], dt.Rows[i]["Age"]);
            }
        }

        [TestMethod]
        public void DictionaryTest()
        {
            var source = new Dictionary<string, SimpleTestClass>();
            source["Mark"] = new SimpleTestClass(42,"kraM");
            source["Julie"] = new SimpleTestClass(36,"eiluJ");

            var clone = ObjectCloner.Clone(source);
            Assert.AreEqual(2, clone.Count);
            CollectionAssert.AreEqual(source.Keys, clone.Keys);

            foreach (var kvp in source)
            {
                var test = clone[kvp.Key];
                Assert.IsTrue(test.Compare(kvp.Value));
            }

            Assert.IsTrue(clone.ContainsKey("Mark"));
            Assert.IsTrue(clone.ContainsKey("Julie"));
        }

        [TestMethod]
        public void ListTest()
        {
            var source = new ObservableCollection<SimpleTestClass>();
            for (int i = 0; i < 10; i++)
                source.Add(new SimpleTestClass(i, "Test #" + i));

            var clone = ObjectCloner.Clone(source);
            CollectionAssert.AreEqual(clone, source);
        }

        [TestMethod]
        public void StackTest()
        {
            var source = new Stack<string>();
            for (int i = 0; i < 10; i++)
                source.Push("Test #" + i);

            var clone = ObjectCloner.Clone(source);
            CollectionAssert.AreEqual(clone, source);
        }

        [TestMethod]
        public void RecursiveTest()
        {
            SimpleTestClass[] source = new SimpleTestClass[4];
            source[0] = source[1] = source[2] = source[3] = new SimpleTestClass(10, "Test");
            var clone = ObjectCloner.Clone(source);
            Assert.IsTrue(source[0].Compare(clone[0]));
            Assert.AreSame(clone[0],clone[1]);
            Assert.AreSame(clone[1], clone[2]);
            Assert.AreSame(clone[2], clone[3]);
        }
    }
}
