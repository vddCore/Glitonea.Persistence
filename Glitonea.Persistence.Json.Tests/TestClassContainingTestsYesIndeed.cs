namespace Glitonea.Persistence.Json.Tests;

using Glitonea.Persistence.Json.Tests.TestModels;
using Shouldly;

public class TestClassContainingTestsYesIndeed
{
    [Fact]
    public void SettingsFile_IsCreated_WhenDoesNotExist()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        
        using (var settingsFile = new SettingsFile<TestSettingsObject>(path))
        {
            settingsFile.Path.ShouldBeEquivalentTo(path);
            File.Exists(settingsFile.Path).ShouldBeTrue();
        }
        
        File.Delete(path);
    }
}