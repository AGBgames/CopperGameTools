namespace CopperGameTools.Builder;

public class ProjectFileKey(string key, string value)
{
    public string Key { get; } = key;
    public string Value { get; set; } = value;
}
