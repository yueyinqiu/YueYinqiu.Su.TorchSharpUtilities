using System.Text.Json;
using System.Text.Json.Serialization;

namespace YueYinqiu.Su.TorchSharpUtilities.JsonSerialization;
public static class HumanFriendlyJson
{
    public static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters =
        {
            new JsonStringEnumConverter(),
            new TensorConverter()
        }
    };

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }
    public static void Serialize<T>(Stream utf8Json, T value)
    {
        JsonSerializer.Serialize(utf8Json, value, SerializerOptions);
    }
    public static void Serialize<T>(FileInfo utf8JsonFile, T value)
    {
        utf8JsonFile.Directory?.Create();
        using var utf8Json = utf8JsonFile.Open(FileMode.Create);
        JsonSerializer.Serialize(utf8Json, value, SerializerOptions);
    }
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }
    public static T? Deserialize<T>(Stream utf8Json)
    {
        return JsonSerializer.Deserialize<T>(utf8Json, SerializerOptions);
    }
    public static T? Deserialize<T>(FileInfo utf8JsonFile)
    {
        using var utf8Json = utf8JsonFile.OpenRead();
        return JsonSerializer.Deserialize<T>(utf8Json, SerializerOptions);
    }

    public static T? Clone<T>(T t)
    {
        return Deserialize<T>(Serialize(t));
    }
}
