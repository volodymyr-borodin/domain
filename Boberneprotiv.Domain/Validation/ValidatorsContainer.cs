using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Validation;

public sealed class ValidatorsContainer<TContext> : IValidator<TContext>
{
    private readonly IEnumerable<IValidator<TContext>> validators;

    public ValidatorsContainer(params IValidator<TContext>[] validators)
    {
        this.validators = validators;
    }

    public async Task<DomainResult> ValidateAsync(TContext context)
    {
        var results = new List<DomainResult>(validators.Count());
        foreach (var validator in validators)
        {
            results.Add(await validator.ValidateAsync(context));
        }

        return DomainResult.MergeErrors(results, null, null);
    }
}