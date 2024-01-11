// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Steeltoe.Common;
using Steeltoe.Discovery;

namespace Steeltoe.Configuration.ConfigServer;

/// <summary>
/// Replace bootstrapped components used by <see cref="ConfigServerConfigurationProvider" /> with objects provided by Dependency Injection.
/// </summary>
internal sealed class ConfigServerHostedService : IHostedService
{
    private readonly ConfigServerConfigurationProvider _configurationProvider;
    private readonly IDiscoveryClient? _discoveryClient;

    public ConfigServerHostedService(IConfigurationRoot configuration)
        : this(configuration, null)
    {
    }

    public ConfigServerHostedService(IConfigurationRoot configuration, IDiscoveryClient? discoveryClient)
    {
        ArgumentGuard.NotNull(configuration);

        _configurationProvider = configuration.FindConfigurationProvider<ConfigServerConfigurationProvider>() ??
            throw new ArgumentException("ConfigServerConfigurationProvider was not found in configuration.", nameof(configuration));

        _discoveryClient = discoveryClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _configurationProvider.ProvideRuntimeReplacementsAsync(_discoveryClient, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _configurationProvider.ShutdownAsync(cancellationToken);
    }
}
