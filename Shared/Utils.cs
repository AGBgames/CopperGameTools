namespace CopperGameTools.Shared;

public abstract class Utils
{
    public static bool IsValidString(string str)
    {
        return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
    }
}