using EADA.Core.FluentValidatorExtensions.Enums;
using EADA.Core.FluentValidatorExtensions.Helpers.Extensions;
using FluentValidation;

namespace EADA.Core.Domain.Messages.Args;

public class ExpenseCategoryArgs
{
    public int ExpenseCategoryId { get; set; }
    public string CategoryName { get; set; }

    public class Validator : AbstractValidator<ExpenseCategoryArgs>
    {
        public Validator()
        {
            this.AddCommonRuleSet(CommonRuleSet.Create, () =>
            {
                RuleFor(x => x.CategoryName).NotEmpty().NotNull();
            });

            this.AddCommonRuleSet(CommonRuleSet.Edit, () =>
            {
                RuleFor(x => x.CategoryName).NotEmpty().NotNull();
                RuleFor(x => x.ExpenseCategoryId).GreaterThan(0);
            });
        }
    }
}