using System.Text.Json;

namespace MyAvaloniaTemplate.Extensions;

public static class ObjectCloner
{
    public static T? JsonClone<T>(this T obj)
    {
        string json = JsonSerializer.Serialize(obj);
        T? newObj = JsonSerializer.Deserialize<T>(json);
        return newObj;
    }
}