using EADA.Core.FluentValidatorExtensions.Enums;
using EADA.Core.FluentValidatorExtensions.Helpers.Extensions;
using FluentValidation;

namespace EADA.Core.Domain.Messages.Args;

public class ExpenseTypeArgs
{
    public int ExpenseTypeId { get; set; }
    public string TypeName { get; set; }

    public class Validator : AbstractValidator<ExpenseTypeArgs>
    {
        public Validator()
        {
            this.AddCommonRuleSet(CommonRuleSet.Create, () =>
            {
                RuleFor(x => x.TypeName).NotEmpty().NotNull();
            });

            this.AddCommonRuleSet(CommonRuleSet.Edit, () =>
            {
                RuleFor(x => x.TypeName).NotEmpty().NotNull();
                RuleFor(x => x.ExpenseTypeId).GreaterThan(0);
            });
        }
    }
}