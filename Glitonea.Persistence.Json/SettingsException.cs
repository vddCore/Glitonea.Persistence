namespace Glitonea.Persistence.Json;

public class SettingsException : Exception
{
    public SettingsFile File { get; }
    
    public SettingsException(SettingsFile file, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        File = file;
    }
}