using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EADA.Core.Abstracts;
using EADA.Core.Attributes;
using EADA.Core.Contracts.Configuration;
using Microsoft.Extensions.Configuration;

namespace EADA.Core.Domain.Configuration
{
    public class MainConfiguration : AppConfigurationBase<MainConfiguration>, IMainConfiguration
    {
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public string ApiUrl { get; set; }
        public int HttpsPort { get; set; }
        public bool UseHttps { get; set; }
        public string AdministratorEmail { get; set; }
        public string AdministratorPhone { get; set; }
        public string EnvironmentName { get; set; }
        [ConfigurationImplementationAttributes]
        public IConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();

        public static MainConfiguration FromConfiguration(IConfiguration configuration, string environmentName)
        {
            var mainConfiguration = configuration.GetSection(nameof(MainConfiguration)).Get<MainConfiguration>();
            mainConfiguration.EnvironmentName = environmentName;
            mainConfiguration.ConnectionStrings = configuration.GetSection(nameof(MainConfiguration.ConnectionStrings))
                .Get<ConnectionStrings>();
            return mainConfiguration;
        }

    }
}
