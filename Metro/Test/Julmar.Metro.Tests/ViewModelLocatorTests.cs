using JulMar.Windows.Mvvm;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julmar.Metro.Tests
{
    [ExportViewModel("myViewModel")]
    public sealed class SampleViewModel : SimpleViewModel
    {
    }

    [ExportViewModel("myViewModel2")]
    public sealed class SampleViewModel2 : ViewModel
    {
    }

    [ExportViewModel("myViewModel3")]
    public sealed class SampleViewModel3
    {
    }

    [TestClass]
    public class ViewModelLocatorTests
    {
        [TestMethod]
        public void TestForViewModelLocation()
        {
            ViewModelLocator locator = new ViewModelLocator();

            object value = locator.Locate("myViewModel");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel));
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestForViewModelLocationWithFullVM()
        {
            ViewModelLocator locator = new ViewModelLocator();

            object value = locator.Locate("myViewModel2");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel2));
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestForViewModelLocationWithNoBase()
        {
            ViewModelLocator locator = new ViewModelLocator();

            object value = locator.Locate("myViewModel3");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel3));
            Assert.IsNotNull(value);
        }

    }
}
