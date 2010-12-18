using System;
using System.ComponentModel.Composition;
using JulMar.Core.Services;
using JulMar.Windows.Mvvm;

namespace MefReleaseTest
{
    class Program
    {
        [Import] private TestClass _testClass;

        static void Main(string[] args)
        {
            StaticRun();

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced);

            Console.WriteLine("Program terminated.");
        }

        private static void StaticRun()
        {
            var resolver = IoCComposer.Instance;
            Program p = new Program();
            resolver.ComposeOnce(p);
            p.Run();
        }

        public Program()
        {
            Console.WriteLine("Created program");

            new MyViewModel();
        }

        private void Run()
        {
            Console.WriteLine("TestClass : {0}", _testClass != null);

            _testClass = null;

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced);

            Console.WriteLine("Run finished");
            Console.ReadLine();
        }

        ~Program()
        {
            Console.WriteLine("Program collected");
        }
    }

    public class MyViewModel : ViewModel
    {
        public MyViewModel() : base(true, true)
        {
            Console.WriteLine("Created MyViewModel");
        }

        ~MyViewModel()
        {
            Console.WriteLine("MyViewModel GC'd");
        }
    }

    [Export(typeof(TestClass))]
    public class TestClass
    {
        public TestClass()
        {
            Console.WriteLine("Created TestClass {0}", GetHashCode());
        }

        ~TestClass()
        {
            Console.WriteLine("TestClass {0} GCd", GetHashCode());
        }
    }
}
