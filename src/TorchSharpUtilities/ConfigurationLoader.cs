using System.Text.Json;

namespace YueYinqiu.Su.TorchSharpUtilities;

public sealed class ConfigurationLoader<T> where T : class, new()
{
    public ConfigurationLoader()
    {
        this.File = new FileInfo("./configuration.json");
        this.Serializer = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        this.Default = new T();
        this.Message =
            $"This is not an error.{Console.Out.NewLine}" +
            $"It's just that the application requires a configuration file to continue, " +
            $"however which does not exist.{Console.Out.NewLine}" +
            $"Now the default configuration file has been created.{Console.Out.NewLine}" +
            $"Please edit it as needed, " +
            $"and then restart the application.{Console.Out.NewLine}" +
            $"File path: {this.File.FullName}";
        this.Terminating = () => Environment.Exit(0);
    }

    public FileInfo File { get; set; }
    public JsonSerializerOptions Serializer { get; set; }

    public T Default { get; set; }
    public string Message { get; set; }
    public Action Terminating { get; set; }

    public T LoadOrCreate()
    {
        if (this.File.Exists)
        {
            using (var stream = this.File.OpenRead())
            {
                var result = JsonSerializer.Deserialize<T>(stream, this.Serializer);
                if (result is not null)
                    return result;
            }
        }

        using (var stream = this.File.Open(FileMode.Create))
            JsonSerializer.Serialize(stream, this.Default, this.Serializer);
        Console.WriteLine(this.Message);
        this.Terminating();
        throw new Exception(this.Message);
    }
}
