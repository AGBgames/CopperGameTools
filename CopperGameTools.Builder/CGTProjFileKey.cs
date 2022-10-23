namespace CopperGameTools.Builder;

public class CGTProjFileKey
{
	public CGTProjFileKey(string key, string value, int line)
    {
        Key = key;
        Value = value;
        Line = line;
    }
	
	public string Key { get; }
	public string Value { get; set; }
	public int Line { get; }
}
