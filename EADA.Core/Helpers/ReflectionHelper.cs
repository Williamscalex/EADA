using System.Reflection;

namespace EADA.Core.Helpers;

public static class ReflectionHelper
{
    /// <summary>
    /// Retrieves all constant values form the class type <paramref name="parentClass"/> where the type equals <typeparamref name="TOutput"/>
    /// </summary>
    /// <typeparam name="TOutput">The type of constant that should be returned. All other types will be ignored.</typeparam>
    /// <param name="parentClass">Contains the constants.</param>
    /// <returns></returns>
    public static IEnumerable<TOutput> GetConstantsOfType<TOutput>(Type parentClass)
    {
        FieldInfo[] fieldInfos =
            parentClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        foreach (var fi in fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly))
        {
            var value = fi.GetRawConstantValue();
            if (value is TOutput output) yield return output;
        }
    }

    public static IEnumerable<FieldInfo> GetConstantsOfType(Type parentClass)
    {
        FieldInfo[] fieldInfos =
            parentClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        foreach (var fi in fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly))
        {
            yield return fi;
        }
    }
}