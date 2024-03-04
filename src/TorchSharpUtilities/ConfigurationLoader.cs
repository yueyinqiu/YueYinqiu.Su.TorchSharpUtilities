namespace YueYinqiu.Su.TorchSharpUtilities;

public sealed class ConfigurationLoader<T> where T : class, new()
{
    internal sealed record ConfigurationsWithVersion(string Version, T Configurations);

    public ConfigurationLoader(string version)
    {
        this.Version = version;
        this.File = new FileInfo("./configuration.json");
        this.Default = new T();
        this.LoadingMessage =
            $"Loading configurations...{Console.Out.NewLine}";
        this.CreatingMessage =
            $"Cannot find the configuration file. " +
            $"This is not an error, " +
            $"and now the default one has been created.{Console.Out.NewLine}" +
            $"Please edit it as needed, " +
            $"and then restart the application: " +
            $"{this.File.FullName}";
        this.Terminator = () =>
        {
            Console.Out.Flush();
            Environment.Exit(0);
        };
    }

    public string Version { get; set; }
    public FileInfo File { get; set; }
    public T Default { get; set; }
    public string LoadingMessage { get; set; }
    public string CreatingMessage { get; set; }
    public Action Terminator { get; set; }

    public T LoadOrCreate()
    {
        Console.WriteLine(this.LoadingMessage);

        if (this.File.Exists)
        {
            var result = HumanFriendlyJson.Deserialize<
                ConfigurationsWithVersion>(this.File);
            if (result?.Version == this.Version)
                return result.Configurations;
        }

        HumanFriendlyJson.Serialize<
            ConfigurationsWithVersion>(this.File, new(this.Version, this.Default));
        Console.WriteLine(this.CreatingMessage);

        this.Terminator();
        throw new Exception(this.CreatingMessage);
    }
}
