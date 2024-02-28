using NAudio.Wave;
using TorchSharp;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class WaveFileWriterWriteTensorExtensions
{
    public static void WriteTensor(
        this WaveFileWriter writer,
        torch.Tensor tensor,
        bool channelsFirst = true)
    {
        if (tensor.Dimensions is not 2)
            throw new ArgumentException(
                $"{nameof(tensor)} should have exactly two dimensions: " +
                $"one for channels and one for frames. " +
                $"But the given value is '{tensor.ToString(TensorStringStyle.Metadata)}'.",
                nameof(tensor));

        using (torch.NewDisposeScope())
        {
            if (!channelsFirst)
                tensor = tensor.transpose(0, 1);

            var channelCount = tensor.size(0);
            var frameCount = tensor.size(1);
            for (int frame = 0; frame < frameCount; frame++)
            {
                for (int channel = 0; channel < channelCount; channel++)
                    writer.WriteSample((float)tensor[channel, frame]);
            }
        }
        writer.Flush();
    }
}
