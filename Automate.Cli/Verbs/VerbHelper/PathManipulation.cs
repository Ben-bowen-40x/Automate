using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs.VerbHelper;

internal enum FileType
{
    Csv,
    Json,
    Sql,
    Txt
}

public static class PathManipulation
{
    internal static string RetrieveParentDir(FileInfo fileLoc) => RetrieveParentDir(fileLoc.FullName);

    internal static string RetrieveParentDir(string fileLoc)
    {
        // Strip the file name from the path
        List<string> separated = [.. fileLoc.Split('\\')];
        separated.RemoveAt(separated.Count - 1);// O(n) complexity but Omega(1) execution, because it's the last element

        // Reform the path, but this time the parent directory only
        string parent = string.Join('\\', separated);
        return parent;
    }

    internal static bool TryCreate(this FileInfo location, out string error)
    {
        try
        {
            return location.FullName.TryCreate(out error);
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    internal static Result TryCreate(this FileInfo location) => location.FullName.TryCreate();

    internal static Result TryCreate(this string location)
    {
        try
        {
            bool result = location.TryCreate(out string error);
            if (result)
            {
                return Result.Success();
            }
            else
            {
                return Result.Failure(error);
            }
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    internal static bool TryCreate(this string location, out string error)
    {
        bool result = false;
        error = string.Empty;
        try
        {
            if (!Path.Exists(location))
            {
                var file = File.Create(location);
                file.Close();
            }
            result = true;
        }
        catch (Exception ex)
        {
            error = ex.ToString();
        }
        return result;
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
                : $"This was the literal input, which is not a valid path and therefore does not exist:   \n\"{path}\"";
        }
        return newLineTab
            // DO NOT change the weird spacing in the string, please.
            ? $"This was the literal input: \n\t{path}\nAnd this is the actual path, confirmed to exist by the system: \n\t{Path.GetFullPath(path)}"
            : $"This was the literal input:   \n{path}\nAnd this is the actual path, confirmed to exist by the system:   \n{Path.GetFullPath(path)}";
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
                ? $"This was the literal input, which is not a valid path and therefore does not exist:\n\t\"{path}\""
                : $"This was the literal input, which is not a valid path and therefore does not exist:  \n\"{path}\"";
        }
        pathInfo = new(path);
        return newLineTab
            // DO NOT change the weird spacing in the string, please. At the moment, it lines things up in a pleasing way
            ? $"This was the literal input:\n\t{path}\nAnd this is the actual path, confirmed to exist by the system:\n\t{Path.GetFullPath(path)}"
            : $"This was the literal input:  \n{path}\nAnd this is the actual path, confirmed to exist by the system:  \n{Path.GetFullPath(path)}";
    }

    internal static Result<FileType> VerifyFileType(string location)
    {
        if (!File.Exists(location))
            return Result.Failure<FileType>($"The provided file location does not exist. Here is the provided file location: \"{location}\"");

        FileInfo fileInfo = new(location);
        Result<FileType> result = VerifyFileType(fileInfo);
        return result;
    }

    internal static Result<FileType> VerifyFileType(FileInfo fileInfo)
    {
        try
        {
            return fileInfo.Extension switch
            {
                ".json" => FileType.Json,
                ".csv" => FileType.Csv,
                ".txt" => FileType.Txt,
                ".sql" => FileType.Sql,
                _ => Result.Failure<FileType>($"The provided {nameof(FileType)} has a file extension that is unrecognized.\nThis is the provided file:\n\"{fileInfo.FullName}\"")
            };
        }
        catch (Exception ex)
        {
            return Result.Failure<FileType>(ex.Message);
        }
    }
}

