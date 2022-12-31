using EADA.Core.Contracts.Configuration;
using EADA.Infrastructure;
using EADA.Infrastructure.Contexts;

namespace EADA.Web;

public class HttpUnitOfWork : UnitOfWork
{
    public HttpUnitOfWork(
        AppDbContext context,
        IMainConfiguration mainConfiguration) : base(context, mainConfiguration)
    {

    }
}