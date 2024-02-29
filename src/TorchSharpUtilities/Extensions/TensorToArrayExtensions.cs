using System.Numerics;
using TorchSharp;
using static TorchSharp.torch;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class TensorToArrayExtensions
{
    private static Type GetCsharpType(ScalarType scalarType)
    {
        return scalarType switch
        {
            ScalarType.Byte => typeof(sbyte),
            ScalarType.Int8 => typeof(byte),
            ScalarType.Int16 => typeof(short),
            ScalarType.Int32 => typeof(int),
            ScalarType.Int64 => typeof(long),
            ScalarType.Float16 => typeof(Half),
            ScalarType.Float32 => typeof(float),
            ScalarType.Float64 => typeof(double),
            ScalarType.ComplexFloat32 => typeof(Complex),
            ScalarType.ComplexFloat64 => typeof(Complex),
            ScalarType.Bool => typeof(bool),
            ScalarType.BFloat16 => typeof(float),
            _ => throw new ArgumentException(
                "The given scalar type is not expected.", nameof(scalarType))
        };
    }

    private static Func<Tensor, object> GetConverter(Type csharpType)
    {
        if (csharpType == typeof(sbyte))
            return (x) => TensorExtensionMethods.ToSByte(x);
        if (csharpType == typeof(byte))
            return (x) => TensorExtensionMethods.ToByte(x);
        if (csharpType == typeof(short))
            return (x) => TensorExtensionMethods.ToInt16(x);
        if (csharpType == typeof(int))
            return (x) => TensorExtensionMethods.ToInt32(x);
        if (csharpType == typeof(long))
            return (x) => TensorExtensionMethods.ToInt64(x);
        if (csharpType == typeof(Half))
            return (x) => TensorExtensionMethods.ToHalf(x);
        if (csharpType == typeof(float))
            return (x) => TensorExtensionMethods.ToSingle(x);
        if (csharpType == typeof(double))
            return (x) => TensorExtensionMethods.ToDouble(x);
        if (csharpType == typeof(Complex))
            return (x) => TensorExtensionMethods.ToComplex64(x);
        if (csharpType == typeof(bool))
            return (x) => TensorExtensionMethods.ToBoolean(x);
        throw new ArgumentException(
                "The given type is not expected.", nameof(csharpType));
    }

    public static Array ToArray(this Tensor tensor, Type? elementType = null)
    {
        if (elementType is null)
            elementType = GetCsharpType(tensor.dtype);
        var converter = GetConverter(elementType);

        var size = tensor.size();
        if (size.Length is 0)
            throw new ArgumentException(
                $"The given tensor is a scalar, which cannot be converted to an array.",
                nameof(tensor));

        var result = Array.CreateInstance(elementType, size);

        for (var indices = new long[size.Length]; ;)
        {
            result.SetValue(converter(tensor[indices]), indices);

            for (int i = 0; ;)
            {
                indices[i]++;
                if (indices[i] < size[i])
                    break;

                indices[i] = 0;
                i++;
                if (i < size.Length)
                    continue;

                return result;
            }
        }
    }
}
