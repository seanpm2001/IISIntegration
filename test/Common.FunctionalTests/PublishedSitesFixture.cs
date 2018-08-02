// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Server.IntegrationTesting.IIS;
using Xunit;

namespace Microsoft.AspNetCore.Server.IISIntegration.FunctionalTests
{
    /// <summary>
    /// This type just maps collection names to available fixtures
    /// </summary>
    [CollectionDefinition(Name)]
    public class PublishedSitesCollection : ICollectionFixture<PublishedSitesFixture>
    {
        public const string Name = nameof(PublishedSitesCollection);
    }

    public class PublishedSitesFixture : IDisposable
    {
        public CachingApplicationPublisher InProcessTestSite { get; } = new CachingApplicationPublisher(Helpers.GetInProcessTestSitesPath());
        public CachingApplicationPublisher OutOfProcessTestSite { get; } = new CachingApplicationPublisher(Helpers.GetOutOfProcessTestSitesPath());
        public CachingApplicationPublisher StartupExceptionWebsite { get; } = new CachingApplicationPublisher(Helpers.GetTestWebSitePath("StartupExceptionWebsite"));
        public CachingApplicationPublisher OverriddenServerWebSite { get; } = new CachingApplicationPublisher(Helpers.GetTestWebSitePath("OverriddenServerWebSite"));

        public void Dispose()
        {
            InProcessTestSite.Dispose();
            OutOfProcessTestSite.Dispose();
            StartupExceptionWebsite.Dispose();
            OverriddenServerWebSite.Dispose();
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(HostingModel hostingModel = HostingModel.InProcess, bool publish = false)
        {
            var publisher = hostingModel == HostingModel.InProcess ? InProcessTestSite : OutOfProcessTestSite;
            return GetBaseDeploymentParameters(publisher, hostingModel, publish);
        }
        public IISDeploymentParameters GetBaseDeploymentParameters(TestVariant variant, bool publish = false)
        {
            var publisher = variant.HostingModel == HostingModel.InProcess ? InProcessTestSite : OutOfProcessTestSite;
            return GetBaseDeploymentParameters(publisher, new DeploymentParameters(variant), publish);
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(ApplicationPublisher publisher, HostingModel hostingModel = HostingModel.InProcess, bool publish = false)
        {
            return GetBaseDeploymentParameters(
                publisher,
                new DeploymentParameters(publisher.ApplicationPath, DeployerSelector.ServerType, RuntimeFlavor.CoreClr, RuntimeArchitecture.x64)
                {
                    HostingModel = hostingModel
                },
                publish);
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(ApplicationPublisher publisher, DeploymentParameters baseParameters, bool publish = false)
        {
            return new IISDeploymentParameters(baseParameters)
            {
                ApplicationPublisher = publisher,
                ApplicationPath =  publisher.ApplicationPath,
                TargetFramework = Tfm.NetCoreApp21,
                ApplicationType = ApplicationType.Portable,
                AncmVersion = AncmVersion.AspNetCoreModuleV2,
                PublishApplicationBeforeDeployment = publish,
            };
        }
    }
}
