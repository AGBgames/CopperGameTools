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

/// <summary>
/// Holds a readonly value.
/// </summary>
/// <typeparam name="T">Type of the readonly value.</typeparam>
/// <param name="value">Value of the readonly value.</param>
public class StrongReadOnlyHolder<T>(T value)
{
    private readonly T _value = value;

    public T Value() => _value;
}
/// <summary>
/// Holds a value that can only be changed if the WeakReadOnlyHolder is "unlocked".
/// </summary>
/// <typeparam name="T">Type of the value.</typeparam>
/// <param name="value">Value of the value.</param>
public class WeakReadOnlyHolder<T>(T value)
{
    private T _value = value;

    public bool Locked = true;
    public T Value() => _value;

    public void Set(T newValue)
    {
        if (_value == null) 
            return;
        if (Locked)
            throw new WeakReadOnlyHolderLockedException($"Trying to update value of locked WeakConst<{_value.GetType().Name}>");
        _value = newValue;
    }
}

class WeakReadOnlyHolderLockedException(string? message) : Exception(message);
