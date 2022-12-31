using EADA.Core.Exceptions;
using EADA.Core.FluentValidatorExtensions.Contracts;
using EADA.Core.FluentValidatorExtensions.Enums;
using FluentValidation;

namespace EADA.Core.FluentValidatorExtensions.Helpers.Extensions;

public static class FluentValidatorExtensions
{
    /// <summary>
    /// Adds a common rules est specified by Enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="validator"></param>
    /// <param name="commonRuleSet"></param>
    /// <param name="action"></param>
    public static void AddCommonRuleSet<T>(
        this AbstractValidator<T> validator,
        CommonRuleSet commonRuleSet,
        Action action) =>
        validator.RuleSet($"{commonRuleSet}".ToLowerInvariant(), action);
    /// <summary>
    /// Ensures the collection us properly prepared for validation.
    /// </summary>
    /// <param name="ruleSet"></param>
    /// <returns></returns>
    private static List<CommonRuleSet> GetPreparedRuleSet(CommonRuleSet[] ruleSet)
    {
        List<CommonRuleSet> values = ruleSet?.ToList() ?? new List<CommonRuleSet>();
        if (!values.Contains(CommonRuleSet.Default)) values.Add(CommonRuleSet.Default);
        return values;
    }

    public static async Task ValidateCommonAsync<T>(this IValidator<T> validator, T instance, params CommonRuleSet[] ruleSet)
    {
        var values = GetPreparedRuleSet(ruleSet);
        var result = await validator.ValidateAsync(instance, x => x.IncludeRuleSets(values.ToRuleSetString()));
        if (result.IsValid) return;

        throw new DataValidationError((instance as INamedArg)?.Name ?? "", result);
    }

    /// <summary>
    /// Parses and joins the rule-sets into a single comma-delimited string.
    /// </summary>
    /// <param name="ruleSets"></param>
    /// <returns></returns>
    public static string[] ToRuleSetString(this IEnumerable<CommonRuleSet> ruleSets) =>
        ruleSets.Select(x => $"{x:G}".ToLowerInvariant()).ToArray();
}