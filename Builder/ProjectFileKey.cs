namespace CopperGameTools.Builder;
public class ProjectFileKey(string key, string value, int line)
{
    public string Key { get; } = key;
    public string Value { get; set; } = value;
    public int Line { get; } = line;
}
