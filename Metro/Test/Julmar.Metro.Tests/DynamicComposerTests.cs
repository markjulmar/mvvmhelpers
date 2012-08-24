using JulMar.Core.Services;
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
    [Export]
    public class ANonSharedClass
    {
    }

    [Export, Shared]
    public class ASharedClass
    {
    }

    public interface ITestInterface
    {
        void SomeMethod();
    }

    [Export(typeof(ITestInterface))]
    sealed class PrivateClassWithInterface : ITestInterface
    {
        public void SomeMethod()
        {
        }
    }


    [TestClass]
    public class DynamicComposerTests
    {
        [TestMethod]
        public void TryComposeWithKnownClass()
        {
            ANonSharedClass value = DynamicComposer.Instance.GetExportedValue<ANonSharedClass>();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TryComposeWithKnownClassForNonShared()
        {
            ANonSharedClass value = DynamicComposer.Instance.GetExportedValue<ANonSharedClass>();
            Assert.IsNotNull(value);

            ANonSharedClass value2 = DynamicComposer.Instance.GetExportedValue<ANonSharedClass>();
            Assert.AreNotSame(value, value2);
        }

        [TestMethod]
        public void TryComposeWithKnownClassForShared()
        {
            ASharedClass value = DynamicComposer.Instance.GetExportedValue<ASharedClass>();
            Assert.IsNotNull(value);

            ASharedClass value2 = DynamicComposer.Instance.GetExportedValue<ASharedClass>();
            Assert.AreSame(value, value2);
        }

        [TestMethod]
        public void TryComposeForInterface()
        {
            var value = DynamicComposer.Instance.GetExportedValue<ITestInterface>();
            Assert.IsNotNull(value);

            try
            {
                var value2 = DynamicComposer.Instance.GetExportedValue<PrivateClassWithInterface>();
                Assert.Fail("Did not throw expected exception - composition failed");
            }
            catch (CompositionFailedException)
            {
            }
        }

    }
}
