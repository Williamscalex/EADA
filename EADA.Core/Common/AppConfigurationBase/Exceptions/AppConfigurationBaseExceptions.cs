using System.Text;

namespace EADA.Core.Exceptions
{
    public class AppConfigurationBaseExceptions : Exception
    {
        protected const string DefaultMessage = @"The AppConfigurationBase implementation caused an unexpected error.
                                  This type of error typically occurs during the startup porcess when attempting to dynamically register the
                                  configuration interfaces with their corresponding POCO implementations.";
        public AppConfigurationBaseExceptions() : base(DefaultMessage) { }
        public AppConfigurationBaseExceptions(string message) : base(message) { }
        public AppConfigurationBaseExceptions(Exception innerException) : base(DefaultMessage, innerException) { }
        public AppConfigurationBaseExceptions(string message, Exception innerException): base(DefaultMessage, innerException) { }
    }

    /// <summary>
    /// Represents an error that occurs when a configuration interface is implemented multiple times.
    /// </summary>
    public sealed class TooManyConfigurationImplementationsException : AppConfigurationBaseExceptions
    {
        private new const string DefaultMessage = @"While registering the configuration interfaces,
                                                    multiple implementations were found, but a single implementation was expected.";
        public new string Message { get; private set; }
        public TooManyConfigurationImplementationsException() : base(DefaultMessage) { }

        public TooManyConfigurationImplementationsException(Type interfaceType, IEnumerable<Type> implementationTypes) : base()
        {
            Message = GetMessage(interfaceType, implementationTypes);
        }

        /// <summary>
        /// Gets a description message about which configuration interfaces caused the issue.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="implementationTypes"></param>
        /// <returns></returns>
        private string GetMessage(Type interfaceType, IEnumerable<Type> implementationTypes)
        {
            var implementationCount = implementationTypes.Count();
            var sb = new StringBuilder($"While registering the configuration interface \"{interfaceType.Name}\", {implementationCount} implementation were found." +
                                        $"There should only be a single implementation for eahc configuration-specific interface! \"{interfaceType.Name}\" was implemented in :");
            var counter = 0;
            const int maxCount = 3;
            foreach(var it in implementationTypes)
            {
                counter++;
                if(counter == 3)
                {
                    var remainingImplementations = implementationCount - maxCount;
                    sb.AppendLine($"and {remainingImplementations} other {(remainingImplementations > 1 ? "types" : "type")}.");
                    break;
                }
                else
                {
                    sb.AppendLine($"{it.Name}");
                }
            }
            return sb.ToString();
        }
    }
}
