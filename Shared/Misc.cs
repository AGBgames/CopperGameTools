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
    /// <summary>
    /// A Valid string is neither null, empty nor just a whitespace.
    /// </summary>
    /// <param name="str">Reference of String to check</param>
    /// <returns>Boolean indicating if the string is valid (defined by the rules)</returns>
    public static bool IsValidString(ref string str)
    {
        return IsValidString(str);
    }
}

// Pretty weird and useless, but why not:

public class StrongConstHolder<T>(T value)
{
    public readonly T Value = value;
}
public class WeakConstHolder<T>(T value)
{
    public bool Locked = true;
    private T _value = value;

    public T Value() => _value;

    public void Set(T newValue)
    {
        if (_value == null) return;
        if (Locked)
            throw new WeakConstHolderLockedException($"Trying to update value of locked WeakConst<{_value.GetType().Name}>");
        _value = newValue;
    }
}

class WeakConstHolderLockedException(string? message) : Exception(message);