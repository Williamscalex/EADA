using System.Reflection;
using EADA.Core.Attributes;
using EADA.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EADA.Core.Abstracts
{
    public abstract class AppConfigurationBase<TMainConfiguration>
    {
        /// <summary>
        /// Registers the given configuration instance for all of its interfaces.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">An instance of the configuration class.</param>
        /// <param name="alsoRegisterItemizedConfiguration">When true, this will also register properties of the <see cref="TMainConfiguration"/> class
        /// that (1) have the <see cref="ConfigurationImplementationAttributes"/> and (2) are an interface type.</param>
        /// <returns></returns>
        /// <exception cref="AppConfigurationBaseExceptions"></exception>
        public static IServiceCollection RegisterConfiguration(IServiceCollection services, TMainConfiguration configuration, bool alsoRegisterItemizedConfiguration = false)
        {
            try
            {
                foreach(var i in typeof(TMainConfiguration).GetInterfaces())
                {
                    services.TryAddSingleton(i, sp => configuration);
                }
                if (alsoRegisterItemizedConfiguration)
                {
                    RegisterItemizedConfiguration(services, configuration);
                }
            }
            catch (AppConfigurationBaseExceptions){
                throw;
            }
            catch (Exception e)
            {
                throw new AppConfigurationBaseExceptions(e);
            }
            return services;
        }

        /// <summary>
        /// Registers properties of the <see cref="TMainConfiguration"/> class
        /// that (1) have the <see cref="ConfigurationImplementationAttributes"/> and (2) are an interface type.
        /// </summary>
        /// <inheritdoc cref="RegisterConfiguration"/>
        /// <returns></returns>
        public static IServiceCollection RegisterItemizedConfiguration(IServiceCollection services, TMainConfiguration configuration)
        {
            try
            {
                //Get the implementaion type of the main configuration
                var configType = typeof(TMainConfiguration);
                //Get the parent assembly
                var assembly = configType.Assembly;
                //Find all properties implementing interfaces and tagged wit the ConfigurationImplementationAttribute.
                foreach(var pi in GetConfigurationImplementationProperties())
                {
                    var @interface = pi.PropertyType;
                    Type typeImplementationInterface = null;
                    var implementations = GetImplementation(assembly, @interface);
                    try
                    {
                        typeImplementationInterface = implementations.SingleOrDefault();
                    }
                    catch (Exception e)
                    {
                        throw new TooManyConfigurationImplementationsException(@interface, implementations);
                    }

                    if (typeImplementationInterface == null) continue;
                    //get the property's instance to register
                    try
                    {
                        services.TryAddSingleton(@interface, sp => pi.GetValue(configuration));
                    }
                    catch (Exception e)
                    {
                        throw new AppConfigurationBaseExceptions($"Failed to register the configuration interface \"{@interface.Name}\" for the configurationf property \"{pi.Name}\".");
                    }
                }
                return services;
            }
            catch (AppConfigurationBaseExceptions)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AppConfigurationBaseExceptions(e);
            }
        }

        private static IEnumerable<Type> GetImplementation(Assembly assembly, Type interfaceType) => assembly.GetExportedTypes().Where(x => x.GetInterface(interfaceType.Name) != null);

        private static IEnumerable<PropertyInfo> GetConfigurationImplementationProperties()
        {
            var configType = typeof(TMainConfiguration);
            var configAttributeName = nameof(ConfigurationImplementationAttributes);
            return configType
                .GetProperties()
                .Where(x => x.PropertyType.IsInterface &&
                x.CustomAttributes.Any(a => a.AttributeType.Name == configAttributeName));
        }
    }
}
