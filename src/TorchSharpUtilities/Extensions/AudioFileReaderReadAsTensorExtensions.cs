using NAudio.Wave;
using System.Diagnostics;
using TorchSharp;

namespace YueYinqiu.Su.TorchSharpUtilities.Extensions;
public static class AudioFileReaderReadAsTensorExtensions
{
    public static torch.Tensor ReadAsTensor(
        this AudioFileReader audio,
        bool channelsFirst = true,
        int bufferSize = 1024)
    {
        const int bitsPerByte = 8;
        Debug.Assert(audio.WaveFormat.BitsPerSample % bitsPerByte is 0);
        var sampleSize = audio.WaveFormat.BitsPerSample / bitsPerByte;
        Debug.Assert(sampleSize is not 0);

        var totalChannel = audio.WaveFormat.Channels;
        var totalFrame = audio.Length - audio.Position;
        Debug.Assert(totalFrame % (sampleSize * totalChannel) is 0);
        totalFrame = totalFrame / sampleSize / totalChannel;

        var result = torch.empty(totalChannel, totalFrame, torch.ScalarType.Float32);
        var buffer = new float[bufferSize * totalChannel];
        for (var currentFrame = 0; ;)
        {
            var read = audio.Read(buffer, 0, buffer.Length);
            Debug.Assert(read % totalChannel == 0);

            for (int currentBufferItem = 0; currentBufferItem < read;)
            {
                for (int currentChannel = 0; currentChannel < totalChannel;)
                {
                    result[currentChannel, currentFrame] = buffer[currentBufferItem];
                    currentChannel++;
                    currentBufferItem++;
                }
                currentFrame++;
            }
            if (read < buffer.Length)
                break;
        }
        if (channelsFirst)
            return result;
        else
            return result.transpose(0, 1);
    }
}
