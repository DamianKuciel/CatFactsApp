using System;
using System.Collections.Generic;
using CatFactsApp.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace CatFactsApp.Tests
{
    public class AppOptionsTests
    {
        private IServiceProvider BuildServiceProvider(Dictionary<string, string?> inMemorySettings)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var services = new ServiceCollection();

            services.AddOptions<AppOptions>()
                .Bind(configuration.GetSection("AppOptions"))
                .ValidateDataAnnotations()
                .Validate(options =>
                {
                    if (string.IsNullOrWhiteSpace(options.ApiUrl)) return true;
                    return Uri.TryCreate(options.ApiUrl, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
                }, "ApiUrl must be an absolute HTTPS URL.")
                .ValidateOnStart();

            return services.BuildServiceProvider();
        }

        [Fact]
        public void ValidConfiguration_StartsNormally()
        {
            var settings = new Dictionary<string, string?>
            {
                {"AppOptions:ApiUrl", "https://catfact.ninja/fact"},
                {"AppOptions:OutputFilePath", "facts.txt"}
            };
            var provider = BuildServiceProvider(settings);

            var options = provider.GetRequiredService<IOptions<AppOptions>>().Value;
            Assert.Equal("https://catfact.ninja/fact", options.ApiUrl);
        }

        [Fact]
        public void InvalidConfiguration_ThrowsException_OnMissingApiUrl()
        {
            var settings = new Dictionary<string, string?>
            {
                {"AppOptions:ApiUrl", ""},
                {"AppOptions:OutputFilePath", "facts.txt"}
            };
            var provider = BuildServiceProvider(settings);

            var ex = Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<AppOptions>>().Value);
            Assert.Contains("ApiUrl is required", ex.Message);
        }

        [Fact]
        public void InvalidConfiguration_ThrowsException_OnNonHttpsUrl()
        {
            var settings = new Dictionary<string, string?>
            {
                {"AppOptions:ApiUrl", "http://catfact.ninja/fact"}, 
                {"AppOptions:OutputFilePath", "facts.txt"}
            };
            var provider = BuildServiceProvider(settings);

            var ex = Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<AppOptions>>().Value);
            Assert.Contains("ApiUrl must be an absolute HTTPS URL", ex.Message);
        }
    }
}