using McpService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Security;

namespace McpService.Tests
{
    [TestClass]
    public class MefExportImportTests
    {
        private CompositionContainer _container;

        [TestInitialize]
        public void Initialize()
        {
            _container = new CompositionContainer(
                new AggregateCatalog(
                    new AssemblyCatalog(typeof(MefExportImportTests).Assembly)));
        }

        [TestMethod]
        public void PromptAttribute_ExportsIMcpServerPrompt_ForMefComposition()
        {
            // Act
            var exports = _container.GetExports<IMcpServerPrompt>();

            // Assert
            Assert.IsNotNull(exports);
            Assert.IsTrue(exports.Count() > 0);
        }

        [TestMethod]
        public void PromptAttribute_ExportsCanBeImportedByContractType()
        {
            // Act
            var imports = _container.GetExports<IMcpServerPrompt>();

            // Assert
            Assert.IsTrue(imports.Count() > 0);
            foreach (var import in imports)
            {
                Assert.IsNotNull(import.Value);
                Assert.IsInstanceOfType(import.Value, typeof(IMcpServerPrompt));
            }
        }

        [TestMethod]
        public void PromptAttribute_ExportsWithCorrectLazyLoading()
        {
            // Act
            var lazyExports = _container.GetExports<IMcpServerPrompt>();

            // Assert
            bool isEmpty = true;
            foreach (var lazyExport in lazyExports)
            {
                // Value should not be created until accessed
                Assert.IsFalse(lazyExport.IsValueCreated);
                
                // Accessing Value should create the instance
                var instance = lazyExport.Value;
                Assert.IsTrue(lazyExport.IsValueCreated);
                Assert.IsNotNull(instance);

                isEmpty = false;
            }
            Assert.IsFalse(isEmpty);
        }

        [TestMethod]
        public void PromptAttribute_ExportsWithRecompositionSupport()
        {
            // Act
            var initialExports = _container.GetExports<IMcpServerPrompt>();
            var initialCount = initialExports.Count();

            // Simulate recomposition (in real scenarios, this would be triggered by catalog changes)
            var recomposedExports = _container.GetExports<IMcpServerPrompt>();
            var recomposedCount = recomposedExports.Count();

            // Assert
            Assert.IsTrue(initialCount > 0);
            Assert.AreEqual(initialCount, recomposedCount);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _container?.Dispose();
        }

        // Additional test class with different prompt
        public class BasicPromptClass : McpServerPrompt
        {
            public BasicPromptClass()
                : base(McpServerPromptOptions.Create(nameof(BasicPromptClass)))
            { }
        }

        public class BasicPromptWithArgumentsClass : McpServerPrompt
        {
            public BasicPromptWithArgumentsClass()
                : base(McpServerPromptOptions
                      .Create(nameof(BasicPromptWithArgumentsClass))
                      .WithArgument("Arg1"))
            { }
        }
    }
}
