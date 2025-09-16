﻿using BasicProvider.Tools;
using McpService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace BasicProvider
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(BasicProviderPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class BasicProviderPackage : AsyncPackage
    {
        private IMcpCapabilityProvider? m_provider;

        /// <summary>
        /// BasicProviderPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0f98818c-dc10-4ae1-80d6-d858f30d1c0c";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            var builder = await this.CreateMcpCapabilityProviderBuilderAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                builder.Services
                    .AddMcpServer()
                        .WithTools<EchoTool>();
                m_provider = await builder.BuildAsync(cancellationToken);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                (m_provider as IDisposable)?.Dispose();
                m_provider = null;
            }

            base.Dispose(disposing);
        }
    }
}
