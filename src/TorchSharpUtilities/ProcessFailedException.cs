using System.Diagnostics;

namespace YueYinqiu.Su.TorchSharpUtilities;

public class ProcessFailedException(
    string? message, Process? process, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Process? Process { get; } = process;
}
