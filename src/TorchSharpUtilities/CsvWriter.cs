using System.Globalization;

namespace YueYinqiu.Su.TorchSharpUtilities;

public sealed class CsvWriter<T> : IDisposable
{
    private readonly CsvHelper.CsvWriter writer;
    public CsvWriter(string path)
    {
        var streamWriter = new StreamWriter(path, false);
        this.writer = new CsvHelper.CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        this.writer.WriteHeader<T>();
        this.writer.NextRecord();
    }
    
    public void Write(T value)
    {
        this.writer.WriteRecord(value);
        this.writer.NextRecord();
    }

    public void Flush()
    {
        this.writer.Flush();
    }

    public void Dispose()
    {
        this.writer.Dispose();
    }
}
