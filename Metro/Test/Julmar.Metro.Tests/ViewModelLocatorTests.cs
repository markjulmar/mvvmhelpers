using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using JulMar.Windows.Mvvm;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
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

    [ExportViewModel("myViewModel4"), Shared]
    public sealed class SampleViewModel4 : SimpleViewModel
    {
    }

    [ExportViewModel("myViewModel5")]
    public sealed class SampleViewModel5 : SimpleViewModel
    {
        public SampleViewModel5()
        {
        }

        public SampleViewModel5(string data)
        {
        }
    }

    [Export]
    public sealed class InputData
    {
    }

    [ExportViewModel("myViewModel6")]
    public sealed class SampleViewModel6 : SimpleViewModel
    {
        public InputData _data;

        [ImportingConstructor]
        public SampleViewModel6(InputData data)
        {
            _data = data;
        }
    }

    [TestClass]
    public class ViewModelLocatorTests
    {
        [Import]
        public IViewModelLocator Locator { get; set; }

        public ViewModelLocatorTests()
        {
            DynamicComposer.Instance.Compose(this);
        }

        [TestMethod]
        public void TestForViewModelLocation()
        {
            object value = Locator.Locate("myViewModel");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel));
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestForParameterizedViewModelLocation()
        {
            object value = Locator.Locate("myViewModel6");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel6));
            Assert.IsNotNull(value);

            SampleViewModel6 vm = (SampleViewModel6) value;
            Assert.IsNotNull(vm._data);
        }

        [TestMethod]
        public void TestForMultiViewModelLocation()
        {
            object value = Locator.Locate("myViewModel");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel));
            Assert.IsNotNull(value);

            object value2 = Locator.Locate("myViewModel");
            Assert.IsInstanceOfType(value2, typeof(SampleViewModel));
            Assert.IsNotNull(value2);
            Assert.AreNotSame(value, value2);
        }

        [TestMethod]
        public void TestForMultiViewModelLocationMultipleTimes()
        {
            object value = Locator.Locate("myViewModel6");
            object value2 = Locator.Locate("myViewModel6");
            Assert.IsNotNull(value2);
            Assert.AreNotSame(value, value2);

            SampleViewModel6 vm1 = (SampleViewModel6) value;
            SampleViewModel6 vm2 = (SampleViewModel6)value2;
            Assert.AreNotSame(vm1._data, vm2._data);
        }

        [TestMethod]
        public void TestForSameViewModelLocation()
        {
            object value = Locator.Locate("myViewModel4");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel4));
            Assert.IsNotNull(value);

            object value2 = Locator.Locate("myViewModel4");
            Assert.IsInstanceOfType(value2, typeof(SampleViewModel4));
            Assert.IsNotNull(value2);
            Assert.AreSame(value, value2);
        }


        [TestMethod]
        public void TestForViewModelLocationWithFullVM()
        {
            object value = Locator.Locate("myViewModel2");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel2));
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestForViewModelLocationWithNoBase()
        {
            object value = Locator.Locate("myViewModel3");
            Assert.IsInstanceOfType(value, typeof(SampleViewModel3));
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TestViewModelLocatorResource()
        {
            ViewModelLocatorResource vmRes = new ViewModelLocatorResource();
            object value = vmRes.ViewModels["myViewModel2"];
            Assert.IsInstanceOfType(value, typeof(SampleViewModel2));
            Assert.IsNotNull(value);
        }
    }
}
