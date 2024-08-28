// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Steeltoe.Management.Configuration;
using Steeltoe.Management.Endpoint.Actuators.Environment;

namespace Steeltoe.Management.Endpoint.Test.Actuators.Environment;

public sealed class EnvironmentEndpointOptionsTest : BaseTest
{
    private static readonly string[] DefaultKeysToSanitize =
    [
        "password",
        "secret",
        "key",
        "token",
        ".*credentials.*",
        "vcap_services"
    ];

    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        EnvironmentEndpointOptions options = GetOptionsFromSettings<EnvironmentEndpointOptions, ConfigureEnvironmentEndpointOptions>();

        Assert.Equal("env", options.Id);
        Assert.Equal(DefaultKeysToSanitize, options.KeysToSanitize);
        Assert.Equal(EndpointPermissions.Restricted, options.RequiredPermissions);
    }
}
