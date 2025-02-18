namespace Glitonea.Persistence.Json;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

public abstract class SettingsFile
{
    public string Path { get; protected set; }

    internal SettingsFile(string path)
    {
        Path = path;
    }
}

public class SettingsFile<T> : SettingsFile, IDisposable
    where T : SettingsObject, new()
{
    public bool SaveOnDispose { get; set; } = true;
    public T Settings { get; private set; }

    public SettingsFile(string path)
        : base(path)
    {
        if (!TryLoad(out var settings))
        {
            settings = new T();
            Save(settings);
        }

        Settings = settings;
        Settings.PropertyChanged += Settings_PropertyChanged;
    }

    public void Save()
        => Save(Settings);

    public bool TrySave()
    {
        try
        {
            Save(Settings);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Reset()
    {
        if (File.Exists(Path))
            File.Delete(Path);

        Settings.PropertyChanged -= Settings_PropertyChanged;
        Settings = new T();
        Settings.PropertyChanged += Settings_PropertyChanged;

        Save(Settings);
    }

    public bool TryReset()
    {
        try
        {
            Reset();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Reload()
    {
        Settings.PropertyChanged -= Settings_PropertyChanged;
        Settings = Load();
        Settings.PropertyChanged += Settings_PropertyChanged;
    }

    public bool TryReload()
    {
        try
        {
            Reload();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private T Load()
    {
        using (var sr = new StreamReader(Path))
        {
            var settings = JsonSerializer.Deserialize<T>(sr.ReadToEnd());

            if (settings == null)
                throw new SettingsException(this, $"Failed to deserialize settings file: {Path}");

            return settings;
        }
    }

    private bool TryLoad([MaybeNullWhen(false)] out T settings)
    {
        try
        {
            settings = Load();
            return true;
        }
        catch
        {
            settings = null;
            return false;
        }
    }

    private void Save(T settings)
    {
        using (var sw = new StreamWriter(Path, false))
        {
            sw.AutoFlush = true;
            
            sw.WriteLine(
                JsonSerializer.Serialize(
                    settings, 
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
        }
    }

    private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        => TrySave();

    public void Dispose()
    {
        Settings.PropertyChanged -= Settings_PropertyChanged;

        if (SaveOnDispose)
            Save();
    }
}