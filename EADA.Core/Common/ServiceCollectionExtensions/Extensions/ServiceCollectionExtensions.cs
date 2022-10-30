using System.Diagnostics.Contracts;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace EADA.Core.Extensions;

public static partial class ServiceCollectionExtensions
{
    public enum Lifestyle
    {
        Scoped,
        Transient,
        Singleton
    }

    public static void RegisterSectionByName(
        this IServiceCollection services,
        string sectionName,
        Assembly assembly,
        Lifestyle lifestyle = Lifestyle.Transient,
        HashSet<Type> excludeInterfaces = null,
        params string[] additionalINamespaces)
    {
        Action<Type, Type> addService;

        switch (lifestyle)
        {
            case Lifestyle.Transient:
                addService = (serviceType, implementationType) =>
                    services.AddTransient(serviceType, implementationType);
                break;
            case Lifestyle.Scoped:
                addService = (serviceType, implementationType) =>
                    services.AddScoped(serviceType, implementationType);
                break;
            case Lifestyle.Singleton:
                addService = (serviceType, implementationType) =>
                    services.AddSingleton(serviceType, implementationType);
                break;
            default:
                addService = (serviceType, implementationType) =>
                    services.AddTransient(serviceType, implementationType);
                break;
        }

        var nsList = additionalINamespaces ?? new string[0];
        var namespaceSuffixesToInclude = nsList.Select(ns => $"{sectionName}.{ns}").Distinct().ToList();
        namespaceSuffixesToInclude.Add(sectionName);

        var exportedTypes = assembly.GetExportedTypes();
        var namespacesWithSuffixData = exportedTypes
            .Select(t => t.Namespace)
            .Distinct()
            .Select(ns =>
            {
                var segments = ns.Split(".");
                var suffix = segments.Length > 1 && segments[^1] != sectionName
                    ? $"{segments[^2]}.{segments[^1]}"
                    : segments[^1];
                return new { FullNamespaces = ns, Suffix = suffix };
            });

        var namespacesToSearch = namespacesWithSuffixData
            .Where(data => namespaceSuffixesToInclude.Contains(data.Suffix))
            .Select(x => x.FullNamespaces);
        var myNamespaces = namespacesToSearch.ToList();

        var registrations = exportedTypes
            .Where(t => namespacesToSearch.Contains(t.Namespace) && t.GetInterfaces().Any())
            .Select(t =>
            {
                Type? @interface = t.GetInterfaces().FirstOrDefault(x => (x?.Namespace?.EndsWith("Contracts") ?? false)
                                                                         && excludeInterfaces.All(i =>
                                                                             i.Name != x.Name));
                if (@interface == null)
                {
                    foreach (var ns in nsList)
                    {
                        @interface = t.GetInterfaces().FirstOrDefault(x => (x?.Namespace?.EndsWith(ns) ?? false)
                                     && excludeInterfaces.All(i => i.Name != x.Name));
                        if(@interface != null)break;
                    }
                }

                return new
                {
                    Service = @interface,
                    Implementation = t
                };
            });

        foreach (var reg in registrations)
        {
            if(reg?.Service == null || reg?.Implementation == null) continue;
            addService(reg.Service, reg.Implementation);
        }
    }
}