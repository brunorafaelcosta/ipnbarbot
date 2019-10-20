using System;
using Microsoft.Extensions.Configuration;

namespace ipnbarbot.Infrastructure.Utils
{
    public class LocalizationHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _translationsConfiguration;

        public LocalizationHelper(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._translationsConfiguration = configuration.GetSection("Translations"); 
        }

        public string Localize(string key, string language = "pt-PT")
        {
            var languageSection = this._translationsConfiguration.GetSection(language);
            
            return languageSection.GetValue<string>(key);
        }
    } 
}