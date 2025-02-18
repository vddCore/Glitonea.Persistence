namespace Glitonea.Persistence.Json.Tests.TestModels;

public class TestSettingsObject : SettingsObject
{
    private string _testEntry = string.Empty;

    public string TestEntry
    {
        get => _testEntry;
        
        set
        {
            if (value == _testEntry) 
                return;
            
            _testEntry = value;

            RaisePropertyChanged();
        }
    }
}