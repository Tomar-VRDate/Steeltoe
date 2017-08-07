﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using Xunit;

namespace Steeltoe.Extensions.Logging.CloudFoundry.Test
{
    public class CloudFoundryLoggerProviderTest
    {
        [Fact]
        public void Create_CreatesCorrectLogger()
        {
            var appsettings = @"
{
  'Logging': {
    'IncludeScopes': false,
    'LogLevel': {
        'Default': 'Information',
        'System': 'Information',
        'Microsoft': 'Information',
        'A': 'Information'
        }
    }
}";
            var config = GetConfig(appsettings);
            LoggerFactory fac = new LoggerFactory();
            var loggingSection = config.GetSection("Logging");

            var settings = new CloudFoundryLoggerSettings(loggingSection);
            var provider = new CloudFoundryLoggerProvider(settings);
            fac.AddProvider(provider);

            ILogger logger = fac.CreateLogger(typeof(A.B.C.D.TestClass));
            Assert.NotNull(logger);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.False(logger.IsEnabled(LogLevel.Debug));

            provider.Dispose();
        }

        [Fact]
        public void SetLogLevel_UpdatesLogger()
        {
            var appsettings = @"
{
  'Logging': {
    'IncludeScopes': false,
    'LogLevel': {
        'Default': 'Information',
        'System': 'Information',
        'Microsoft': 'Information',
        'A': 'Information'
        }
    }
}";
            var config = GetConfig(appsettings);
            LoggerFactory fac = new LoggerFactory();
            var loggingSection = config.GetSection("Logging");

            var settings = new CloudFoundryLoggerSettings(loggingSection);
            var provider = new CloudFoundryLoggerProvider(settings);
            fac.AddProvider(provider);

            ILogger logger = fac.CreateLogger(typeof(A.B.C.D.TestClass));
            Assert.NotNull(logger);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.False(logger.IsEnabled(LogLevel.Debug));

            provider.SetLogLevel("A", LogLevel.Debug);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.True(logger.IsEnabled(LogLevel.Debug));


            provider.SetLogLevel("A", LogLevel.Information);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.False(logger.IsEnabled(LogLevel.Debug));

            provider.Dispose();
        }
        [Fact]
        public void GetLoggerConfigurations_ReturnsExpected()
        {
            var appsettings = @"
{
  'Logging': {
    'IncludeScopes': false,
    'LogLevel': {
        'Default': 'Information',
        'System': 'Information',
        'Microsoft': 'Information',
        'A': 'Information'
        }
    }
}";
            var config = GetConfig(appsettings);
            LoggerFactory fac = new LoggerFactory();
            var loggingSection = config.GetSection("Logging");

            var settings = new CloudFoundryLoggerSettings(loggingSection);
            var provider = new CloudFoundryLoggerProvider(settings);
            fac.AddProvider(provider);

            ILogger logger = fac.CreateLogger(typeof(A.B.C.D.TestClass));

            var logConfig = provider.GetLoggerConfigurations();
            Assert.Equal(6, logConfig.Count);
            Assert.Contains(new LoggerConfiguration("Default", LogLevel.Information, LogLevel.Information), logConfig);
            Assert.Contains(new LoggerConfiguration("A.B.C.D.TestClass", null, LogLevel.Information), logConfig);
            Assert.Contains(new LoggerConfiguration("A.B.C.D", null, LogLevel.Information), logConfig);
            Assert.Contains(new LoggerConfiguration("A.B.C", null, LogLevel.Information), logConfig);
            Assert.Contains(new LoggerConfiguration("A.B", null, LogLevel.Information), logConfig);
            Assert.Contains(new LoggerConfiguration("A", LogLevel.Information, LogLevel.Information), logConfig);

            provider.Dispose();

        }

        private IConfiguration GetConfig(string json)
        {
            var path = TestHelpers.CreateTempFile(json);
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(directory);

            configurationBuilder.AddJsonFile(fileName);
            var config = configurationBuilder.Build();
            return config;
        }
    }
}

namespace A.B.C.D
{
    class TestClass { }
}
