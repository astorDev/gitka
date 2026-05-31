namespace Gitka.Api;

public static class GitPathGuard
{
    public static void Validate(string value, string paramName)
    {
        if (Path.IsPathRooted(value) || value.Split('/', '\\').Any(s => s == ".."))
            throw new ArgumentException($"{paramName} contains invalid path traversal segments.", paramName);
    }

    public static string ValidateWithinBase(string filepath, string baseDir)
    {
        Validate(filepath, nameof(filepath));
        var resolvedBase = Path.GetFullPath(baseDir) + Path.DirectorySeparatorChar;
        var resolvedPath = Path.GetFullPath(Path.Combine(baseDir, filepath.Replace('/', Path.DirectorySeparatorChar)));
        if (!resolvedPath.StartsWith(resolvedBase, StringComparison.Ordinal))
            throw new ArgumentException("filepath attempts to escape the repository directory.", nameof(filepath));
        return resolvedPath;
    }
}
