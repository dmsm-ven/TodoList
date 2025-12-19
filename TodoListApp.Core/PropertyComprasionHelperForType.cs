using System.Reflection;

namespace TodoListApp.Core;

public static class PropertyComprasionHelper
{
    public static IReadOnlyDictionary<string, string> GetDifferentProperties<T>(T a, T b)
    {
        return PropertyComprasionHelperForType<T>.GetDifferentProperties(a, b);
    }
}

public static class PropertyComprasionHelperForType<T>
{
    private static readonly PropertyInfo[] typeProps = typeof(T).GetProperties();

    public static IReadOnlyDictionary<string, string> GetDifferentProperties(T left, T right)
    {
        var dic = new Dictionary<string, string>();

        if (left == null || right == null)
            return dic;


        foreach (var prop in typeProps)
        {
            var newVal = prop.GetValue(left)?.ToString() ?? string.Empty;
            var oldVal = prop.GetValue(right)?.ToString() ?? string.Empty;

            //Если тип DateTimeOffset
            if (TryToDateTimeOffset(prop.GetValue(left), out var dtLeft) &&
                TryToDateTimeOffset(prop.GetValue(right), out var dtRight) &&
                dtLeft.UtcDateTime != dtRight.UtcDateTime)
            {
                dic[prop.Name] = dtLeft.ToString();
            }
            //Сравниваем по строке .ToString()
            else if (!string.Equals(newVal, oldVal, StringComparison.Ordinal))
            {
                dic[prop.Name] = newVal ?? string.Empty;
            }
        }

        return dic;
    }

    private static bool TryToDateTimeOffset(object value, out DateTimeOffset dto)
    {

        if (value is DateTimeOffset direct) { dto = direct; return true; }
        if (value is string s && DateTimeOffset.TryParse(s, out var parsed))
        {
            dto = parsed;
            return true;
        }
        dto = default;
        return false;
    }
}
