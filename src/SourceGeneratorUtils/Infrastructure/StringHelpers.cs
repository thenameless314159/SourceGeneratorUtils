﻿namespace SourceGeneratorUtils;

public static class StringHelpers
{
    /// <summary>
    /// Returns a new string with the first character in upper case.
    /// </summary>
    /// <param name="str">The input <see cref="string"/>.</param>
    /// <returns>A new string with the first character in upper case</returns>
    /// <exception cref="ArgumentNullException"><see pref="str"/> is null or whitespace.</exception>
    public static string FirstCharToUpperInvariant(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) 
            throw new ArgumentNullException(nameof(str));

        if (char.IsUpper(str[0])) 
            return str;

        var array = str.ToCharArray();
        array[0] = char.ToUpperInvariant(array[0]);

        return new string(array);
    }

    /// <summary>
    /// Returns a new string with the first character in lower case.
    /// </summary>
    /// <param name="str">The input <see cref="string"/>.</param>
    /// <returns>A new string with the first character in lower case</returns>
    /// <exception cref="ArgumentNullException"><see pref="str"/> is null or whitespace.</exception>
    public static string FirstCharToLowerInvariant(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentNullException(nameof(str));

        if (char.IsLower(str[0]))
            return str;

        var array = str.ToCharArray();
        array[0] = char.ToLowerInvariant(array[0]);

        return new string(array);
    }
}