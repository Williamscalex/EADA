using EADA.Core.Contracts.Configuration;

namespace EADA.Core.Domain.Configuration;

public class ConnectionStrings : IConnectionStrings
{
    public string DefaultConnection { get; set; }
}