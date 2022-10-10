namespace CopperGameTools.Builder;

public class CGTProjFileKey
{
	public CGTProjFileKey(string key, string value)
    {
        Key = key;
        Value = value;
    }
	
	public string Key { get; }
	public string Value { get; }
}
