using Python.Runtime;
using System.Diagnostics;

namespace YueYinqiu.Su.TorchSharpUtilities;

public sealed record EmbeddablePython(DirectoryInfo Directory)
{
    private string PythonCommand => Path.GetFullPath("python", this.Directory.FullName);

    public async ValueTask EnsurePipAsync(
        string getPip = "https://bootstrap.pypa.io/get-pip.py")
    {
        {
            using var process = new Process()
            {
                StartInfo = new(this.PythonCommand, ["-m", "pip", "--version"])
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            _ = process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode is 0)
                return;
        }

        {
            var getPipFile = Path.GetTempFileName();
            using HttpClient client = new HttpClient();
            var stream = await client.GetStreamAsync(getPip);
            using (var file = File.Open(getPipFile, FileMode.Truncate))
                await stream.CopyToAsync(file);

            using var process = new Process()
            {
                StartInfo = new(this.PythonCommand, [getPipFile])
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            _ = process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode is not 0)
                throw new ProcessFailedException($"Failed to install pip.", process);

            foreach (var pth in this.Directory.EnumerateFiles("*._pth"))
                pth.Delete();
        }
    }

    public async ValueTask PipInstallAsync(
        string package, string index = "https://pypi.org/simple")
    {
        using var process = new Process()
        {
            StartInfo = new(this.PythonCommand, ["-m", "pip", "install", package, "-i", index])
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };
        _ = process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode is not 0)
            throw new ProcessFailedException($"Failed to install package '{package}'.", process);
    }

    public void InitializePythonNet()
    {
        const string prefix = "python3";
        foreach (var file in this.Directory.EnumerateFiles($"{prefix}*"))
        {
            var fileName = Path.GetFileNameWithoutExtension(file.Name);

            Debug.Assert(fileName.StartsWith(prefix));
            if (fileName == prefix)
                continue;

            Runtime.PythonDLL = file.FullName;
            break;
        }
        PythonEngine.Initialize();
    }
}
