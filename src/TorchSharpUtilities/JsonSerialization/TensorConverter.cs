using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TorchSharp;
using static TorchSharp.torch;

namespace YueYinqiu.Su.TorchSharpUtilities.JsonSerialization;
public sealed class TensorConverter : JsonConverter<Tensor>
{
    public override Tensor? Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(Tensor));
        var bytes = reader.GetBytesFromBase64();
        using var stream = new MemoryStream(bytes);
        return Tensor.Load(stream);
    }

    public override void Write(
        Utf8JsonWriter writer, Tensor value, JsonSerializerOptions options)
    {
        using var stream = new MemoryStream();
        using var detached = value.detach();
        using var contiguous = detached.contiguous();
        contiguous.Save(stream);
        writer.WriteBase64StringValue(stream.ToArray());
    }
}
