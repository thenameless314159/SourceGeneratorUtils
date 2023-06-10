namespace SourceGeneratorUtils;

/// <summary>
/// Basic equality compared of <see cref="FileInfo"/> that compares the <see cref="FileInfo.FullName"/> property.
/// </summary>
public sealed class FileInfoEqualityComparer : IEqualityComparer<FileInfo>
{
    /// <summary>
    /// The singleton instance of <see cref="FileInfoEqualityComparer"/>.
    /// </summary>
    public static IEqualityComparer<FileInfo> Instance { get; } = new FileInfoEqualityComparer();

    /// <inheritdoc />
    public bool Equals(FileInfo? x, FileInfo? y)
    {
        if (x is null) return false;
        if (y is null) return false;

        if (ReferenceEquals(x, y)) 
            return true;

        return x.FullName == y.FullName;
    }

    /// <inheritdoc />
    public int GetHashCode(FileInfo obj) => obj.GetHashCode();
}