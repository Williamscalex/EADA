using System.Globalization;

namespace EADA.Core.Contracts.Configuration;

public interface IMainConfiguration
{
    public string ApplicationName { get; }
    /// <summary>
    /// The application URL used by the client
    /// </summary>
    public string ApplicationUrl { get; }
    /// <summary>
    /// The API URL used by the server. This may be different in development environments, but will otherwise be the same as <see cref="ApplicationUrl"/>.
    /// </summary>
    public string ApiUrl { get; }
    /// <summary>
    /// The HTTPS port to use.
    /// </summary>
    public int HttpsPort { get; }
    /// <summary>
    /// Determines whether HTTPS redirection should be used.
    /// </summary>
    public bool UseHttps { get; }
    public string AdministratorEmail { get; }
    public string AdministratorPhone { get; }
    public string EnvironmentName { get; }
    public IConnectionStrings ConnectionStrings { get; }
}