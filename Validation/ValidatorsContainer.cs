using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Validation
{
    public sealed class ValidatorsContainer<TContext> : IValidator<TContext>
    {
        private readonly IEnumerable<IValidator<TContext>> validators;

        public ValidatorsContainer(params IValidator<TContext>[] validators)
        {
            this.validators = validators;
        }

        public async Task<DomainResult> ValidateAsync(TContext context)
        {
            var errors = new List<DomainError>();
            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(context);
                if (!result.Successed)
                {
                    errors.AddRange(result.Errors);
                }
            }

            return errors.Any()
                ? DomainResult.Error(errors)
                : DomainResult.Success();
        }
    }
}
