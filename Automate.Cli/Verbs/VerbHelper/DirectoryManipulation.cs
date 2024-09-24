using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs.VerbHelper;

public class DirectoryManipulation
{

    internal static string RetrieveParentDir(string fileLoc)
    {
        // Strip the file name from the path
        List<string> separated = [.. fileLoc.Split('\\')];
        separated.RemoveAt(separated.Count - 1);

        // Reform the path, but this time the parent directory only
        string parent = string.Join('\\', separated);
        return parent;
    }

    internal static string RetrieveParentDir(FileInfo fileLoc)
    {
        return RetrieveParentDir(fileLoc.FullName);
    }

    /// <summary>
    /// <para>If <paramref name="newLineTab"/> is <see cref="true"/> then the string will be delimited with newlines and tabs.</para>
    /// <para>If <paramref name="newLineTab"/> is <see cref="false"/> then the string will be delimited with newlines only.</para>
    /// <para>Note that <paramref name="newLineTab"/> defaults to <see cref="true"/></para>
    /// </summary>
    /// <param name="newLineTab"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    internal static string LocationInformation(string path, bool newLineTab = true)
    {
        if (!Path.Exists(path))
        {
            return newLineTab
                ? $"This was the literal input, which is not a valid path and therefore does not exist: \n\t\"{path}\""
                : $"This was the literal input, which is not a valid path and therefore does not exist: \n\"{path}\"";
        }
        return newLineTab
            // DO NOT change the weird spacing in the string, please.
            ? $"This was the literal input: \n\t{path}\nAnd this is the actual path, confirmed to exist by the system: \n\t{Path.GetFullPath(path)}"
            : $"This was the literal input: \n{path}  \nAnd this is the actual path, confirmed to exist by the system:   \n{Path.GetFullPath(path)}";
    }

    /// <summary>
    /// <para>If <paramref name="newLineTab"/> is <see cref="true"/> then the string will be delimited with newlines and tabs.</para>
    /// <para>If <paramref name="newLineTab"/> is <see cref="false"/> then the string will be delimited with newlines only.</para>
    /// <para>Note that <paramref name="newLineTab"/> defaults to <see cref="true"/></para>
    /// </summary>
    /// <param name="newLineTab"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    internal static string LocationInformation(string path, out FileInfo pathInfo, bool newLineTab = true)
    {
        if (!Path.Exists(path))
        {
            pathInfo = new("./");
            return newLineTab
                ? $"This was the literal input, which is not a valid path and therefore does not exist: \n\t\"{path}\""
                : $"This was the literal input, which is not a valid path and therefore does not exist: \n\"{path}\"";
        }
        pathInfo = new(path);
        return newLineTab
            // DO NOT change the weird spacing in the string, please.
            ? $"This was the literal input: \n\t{path}\nAnd this is the actual path, confirmed to exist by the system: \n\t{Path.GetFullPath(path)}"
            : $"This was the literal input: \n{path}  \nAnd this is the actual path, confirmed to exist by the system:   \n{Path.GetFullPath(path)}";
    }

    internal static Result<FileType> VerifyType(string location)
    {
        if (!File.Exists(location))
            return Result.Failure<FileType>($"The provided file location does not exist. Here is the provided file location: \"{location}\"");

        FileInfo fileInfo = new(location);
        var ext = fileInfo.Extension;
        return ext switch
        {
            ".json" => FileType.Json,
            ".csv" => FileType.Csv,
            ".txt" => FileType.Txt,
            _ => Result.Failure<FileType>($"The provided file location has a file extension that is unrecognized. This is the provided extension: \"{ext}\"")
        };
    }
}

internal enum FileType
{
    Csv,
    Json,
    Txt
}
