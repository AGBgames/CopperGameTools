namespace CopperGameTools.Shared;

public abstract class Utils
{
    /// <summary>
    /// A Valid string is neither null, empty nor just a whitespace.
    /// </summary>
    /// <param name="str">String to check</param>
    /// <returns>Boolean indicating if the string is valid (defined by the rules)</returns>
    public static bool IsValidString(string str)
    {
        return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
    }
}

public class ConstantValue<T>(T value)
{
    public readonly T Value = value;
}