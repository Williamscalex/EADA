using System.Security.Cryptography.X509Certificates;
using EADA.Core.Domain.Mapped.Expense;
using EADA.Core.FluentValidatorExtensions.Enums;
using EADA.Core.FluentValidatorExtensions.Helpers.Extensions;
using FluentValidation;

namespace EADA.Core.Domain.Messages.Args;

public class ExpenseArgs
{
    public int  ExpenseId { get; set; }
    public string ExpenseName { get; set; }
    public int ExpenseTypeId { get; set; }
    public int ExpenseCategoryId { get; set; }
    public decimal? CostPerMonth { get; set; }
    public decimal? CostPerYear { get; set; }
    public string Description { get; set; }

    public class Validator : AbstractValidator<ExpenseArgs>
    {
        public Validator()
        {
            this.AddCommonRuleSet(CommonRuleSet.Edit, () =>
            {
                RuleFor(x => x.ExpenseId).GreaterThan(0);
                RuleFor(x => x.ExpenseName).NotNull().NotEmpty().WithMessage("A name is required for this expense.");
                RuleFor(x => x.ExpenseTypeId).GreaterThan(0);
                RuleFor(x => x.ExpenseCategoryId).GreaterThan(0);
                RuleFor(x => x.CostPerMonth).NotNull()
                    .When(x => x.CostPerYear == null)
                    .WithMessage("A Cost Per Month is required when there is no Cost Per Year.");
                RuleFor(x => x.CostPerYear).NotNull()
                    .When(x => x.CostPerMonth == null)
                    .WithMessage("Cost Per Year is required when there is no Cost Per Month.");
            });

            this.AddCommonRuleSet(CommonRuleSet.Create, () =>
            {
                RuleFor(x => x.ExpenseName).NotNull().NotEmpty().WithMessage("A name is required for expenses.");
                RuleFor(x => x.ExpenseTypeId).GreaterThan(0);
                RuleFor(x => x.ExpenseCategoryId).GreaterThan(0);
                RuleFor(x => x.CostPerMonth).NotNull()
                    .When(x => x.CostPerYear == null)
                    .WithMessage("A Cost Per Month is required when there is no Cost Per Year.");
                RuleFor(x => x.CostPerYear).NotNull()
                    .When(x => x.CostPerMonth == null)
                    .WithMessage("Cost Per Year is required when there is no Cost Per Month.");
            });
        }
    }
}