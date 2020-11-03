using System.Threading.Tasks;

namespace Domain.Validation
{
    public interface IValidator<in T>
    {
        Task<DomainResult> ValidateAsync(T context);
    }
}
