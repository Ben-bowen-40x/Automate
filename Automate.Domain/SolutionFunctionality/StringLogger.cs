using System.Runtime.CompilerServices;
using System.Text;
using Automate.Domain.ValueObjects;

namespace Automate.Domain.SolutionFunctionality;

public class StringLogger
{
    #region Public
    #region NewLog private overloads
    private static void NewLog()
    {
        _newLogCalled = true;
        _builder ??= new();
    }

    private static void NewLog(DateTime start, params string[] args)
    {
        NewLog([.. args, $"Log started at {start.ToString(DateTimeStrings.InternalDateTimeFormat)}"]);
    }

    private static void NewLog(params string[] args)
    {
        NewLog();
        _firstEntry ??= string.Join(_delimiter, args);
        AddLogInternal(args);
    }
    #endregion

    /// <summary>
    /// <para>Accepts <paramref name="start"/> that indicates the time at which the log started.</para>
    /// <para>Accepts <paramref name="sender"/> of type <see cref="object"/>, which should be the containing object where the log occurs</para>
    /// <para>Accepts <paramref name="memberName"/>, which is the member of the containing <see cref="object"/></para>
    /// <para>Accepts <paramref name="args"/> of type <see cref="string"/>, which constitute the contents of the initial log message</para>
    /// <para>Note that <paramref name="args"/> will be added to the log first, followed by the combined, full name of <paramref name="sender"/> and <paramref name="memberName"/>, and <paramref name="start"/> will be added last</para>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="sender"></param>
    /// <param name="memberName"></param>
    /// <param name="args"></param>
    public static void NewLog(DateTime start, object sender, [CallerMemberName] string memberName = "", params string[] args)
    {
        string location = GetFullName.GetMemberName(sender, memberName);
        NewLog(start, [.. args, location]);
    }

    /// <summary>
    /// <para>The <paramref name="args"/> and <paramref name="start"/> date are used to name the log.</para>
    /// <para>The name of the log will be <paramref name="args"/> first and then the <paramref name="start"/> date, separated by a default delimiter.</para>
    /// <para>The default delimiter is determined by the program and not the user because the correct pattern and delimiter must be used later when automatically deleting old log files.</para>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="args"></param>
    public static void NameLog(DateTime start, params string[] args)
    {
        if (_logName is null || _logName == string.Empty || _logName == default)
        {
            string logDate = start.ToString(DateTimeStrings.FileDateTimeFormat2);
            _logName = string.Join(DateTimeStrings.FileDateTimeFormat2Delimiter, [.. args, logDate]);
        }
    }
    private static string? _logName;

    /// <summary>
    /// Checks whether the <see cref="Log"/> is empty. If it is not empty, it adds the <paramref name="args"/> to the log and saves the contents to a local file
    /// <para>As with <see cref="NewLog(DateTime, object, string, string[])"/>, requires the <paramref name="end"/> time, the name of the <paramref name="sender"/>, the <paramref name="memberName"/> where <see cref="ProduceLog(DateTime, object, string, string[])"/> is called, and accepts <paramref name="args"/> of type <see cref="string"/>, which constitute the final log message</para>
    /// </summary>
    /// <param name="args"></param>
    public static void ProduceLog(DateTime end, object sender, [CallerMemberName] string memberName = "", params string[] args)
    {
        if (_firstEntry is not null)
        {
            string location = GetFullName.GetMemberName(sender, memberName);
            AddLogInternal([.. args, $"Ended Log {location}", $"Log produced at {end.ToString(DateTimeStrings.InternalDateTimeFormat)}"]);
            _builder!.Append("\n\n");
            Log();
        }
    }

    /// <summary>
    /// <para></para>
    /// <para>If the logger already has logs in it and is not empty, the first argument of <paramref name="args"/> is appended directly to the most recent entry without a new line character before, but it adds a newline character after.</para>
    /// <para>However, if the logger has no logs in it, nothing will be added. This is useful for testing situations where a logger has not been created</para>
    /// </summary>
    /// <param name="args"></param>
    public static void AddLog(params string[] args)
    {
        if (_firstEntry is not null)
        {
            AddLogInternal([$"{nameof(AddLog)}:", .. args]);
        }
    }

    /// <summary>
    /// <para>Allows the caller to decide whether to <paramref name="includeLineEnder"/> after the arguments have been added to the <see cref="StringBuilder"/></para>
    /// <para>This is a method that triggers it's twin <see cref="EndAlludeLog(string[])"/> if and only if this method is called</para>
    /// </summary>
    /// <param name="includeLineEnder"></param>
    /// <param name="args"></param>
    public static void AlludeLog(bool includeLineEnder, params string[] args)
    {
        if (_firstEntry is not null)
        {
            _alludedLog ??= new();
            _alludedLog.Append(string.Join(_delimiter, [nameof(AlludeLog), .. args]));
            if (includeLineEnder)
            {
                _alludedLog.Append('\n');
            }
            _addLog = true;
        }
    }
    private static StringBuilder? _alludedLog;

    /// <summary>
    /// <para>This is a method that is only triggered if its twin, <see cref="AlludeLog(bool, string[])"/> is called first</para>
    /// <para>If <paramref name="endLog"/> is false, then all logs entered by <see cref="AlludeLog(bool, string[])"/> will not be added</para>
    /// </summary>
    /// <param name="args"></param>
    public static void EndAlludeLog(bool endLog, params string[] args)
    {
        if (_addLog && _firstEntry is not null && endLog)
        {
            _alludedLog ??= new(); // Add a new allude log stringbuilder, if one does not already exist
            _alludedLog.Append(string.Join(_delimiter, [nameof(EndAlludeLog), .. args])); // Append method arguments to alludelog stringbuilder
            AddLogInternal(_alludedLog.ToString()); // Adds the alludelog stringbuilder 
        }
        StopAlludeLog();
    }
    /// <summary>
    /// <para>This is a method that resets the relevant class variables when its twin, <see cref="EndAlludeLog(string[])"/> is called and no log has been added.</para>
    /// </summary>
    private static void StopAlludeLog()
    {
        _alludedLog = null;
        _addLog = false;
    }
    private static bool _addLog = false;

    /// <summary>
    /// <para>This allows the caller to set the <paramref name="delimiter"/> if desired. If a delimiter is not set, then the default delimiter is used. The delimiter default = <see cref="_defaultDelimiter"/> </para>
    /// <para>When the log is produced in <see cref="ProduceLog(string[])"/>, then the delimiter is set to the <see cref="default"/> delimiter</para>
    /// </summary>
    /// <param name="delimiter"></param>
    public static void SetDelimiter(string delimiter) => _delimiter = delimiter;
    #endregion

    #region Internal
    // Executed code
    internal static void AddLogInternal(params string[] args)
    {
        _builder ??= new();
        Append(args);
        _builder.Append('\n');
    }
    internal static DirectoryInfo InfoFolder => _infoFolder ??= FolderFinder.GetLocalFolder(nameof(Domain), @".info");

    // Members
    internal static DirectoryInfo? _infoFolder;
    internal const long max = 10000;

    // Members that should be null at the beginning of a log
    internal static string? _firstEntry;
    internal static bool _newLogCalled;
    internal static StringBuilder? _builder;
    internal static bool _logged;
    #endregion

    #region Private
    // If the log builder is not null then it will save the built log to the local log files
    private static void Log()
    {
        if (_builder is not null && _builder.ToString().Length > 0)
        {
            // Note that the log occurred
            _logged = true;
            SetLogFileNames(out string logFile1, out string logFile2, out string logFile3);

            // If the first one does not exist, create it
            if (!File.Exists(logFile1))
                File.WriteAllText(logFile1, "");

            // If the first logger is too long, attempt to write to the second one
            FileInfo log1 = new(logFile1);
            if (log1.Length >= max)
            {
                if (!File.Exists(logFile2))
                    File.WriteAllText(logFile2, "");
                FileInfo log2 = new(logFile2);
                if (!File.Exists(logFile3))
                    File.WriteAllText(logFile3, "");
                FileInfo log3 = new(logFile3);

                CheckFolders(log1, log2, log3);
            }
            // The first logger is not too long, so write to it
            else
                File.AppendAllText(log1.FullName, _builder.ToString());

            RemoveHistoricalLogs(log1, new(logFile2), new(logFile3));

            // Reset the private items
            _builder = null;
            _firstEntry = null;
            _newLogCalled = false;
            _delimiter = _defaultDelimiter;
        }

        // Locals
        static void CheckFolders(FileInfo logFolder, FileInfo logFolder2, FileInfo logFolder3)
        {
            // If the second logger does not exist, create it
            if (!logFolder2.Exists)
                File.WriteAllText(logFolder2.FullName, "");

            // If the second logger is too long, then the first one is also too long
            if (logFolder2.Length >= max)
            {
                CheckNextFolder(logFolder, logFolder2, logFolder3);
            }
            else
            {
                // Write contents to the second file, because it has room
                File.AppendAllText(logFolder2.FullName, _builder!.ToString());
            }

            // Remove historical files from log folder
        }

        static void CheckNextFolder(FileInfo logFolder, FileInfo logFolder2, FileInfo logFolder3)
        {
            // If the third logger does not exist, create it
            if (!logFolder3.Exists)
                File.WriteAllText(logFolder3.FullName, "");

            // If the third logger is too long, then the first and second are also too long
            if (logFolder3.Length >= max)
            {
                // Delete the contents of all files and start over with the first file
                File.WriteAllText(logFolder2.FullName, string.Empty);
                File.WriteAllText(logFolder3.FullName, string.Empty);
                File.WriteAllText(logFolder.FullName, _builder!.ToString());
            }
            else
            {
                // Delete the contents of the first and second files and write the information into the third file, which has room
                File.WriteAllText(logFolder.FullName, string.Empty);
                File.WriteAllText(logFolder2.FullName, string.Empty);
                File.AppendAllText(logFolder3.FullName, _builder!.ToString());
            }
        }

        static void SetLogFileNames(out string logFile1, out string logFile2, out string logFile3)
        {
            // Perform an empty check / null check on the log name using a ternary if/then statement
            string lName = _logName is null || _logName == string.Empty || _logName.Length == 0
                ? string.Empty
                : _logName;

            // Remove any periods that came through to the log name in order to force the correct file extensions
            string logName = lName.Contains('.')
                ? string.Join("", lName.Split('.'))
                : lName;

            // Create the logger files
            logFile1 = logName == string.Empty ? InfoFolder + LocalLogs : InfoFolder + logName + ".txt";
            logFile2 = logName == string.Empty ? InfoFolder + LocalLogs2 : InfoFolder + logName + "_Two.txt";
            logFile3 = logName == string.Empty ? InfoFolder + LocalLogs3 : InfoFolder + logName + "_Three.txt";
        }
    }

    // Append the arguments to the builder without any newline characters
    private static void Append(params string[] args)
    {
        _builder ??= new();
        _builder.Append(string.Join(_delimiter, args));
    }

    // Log File names
    private const string LocalLogs = "LocalLogs.txt";
    private const string LocalLogs2 = "LocalLogs2.txt";
    private const string LocalLogs3 = "LocalLogs3.txt";

    // This is used to determine whether
    // Default delimiter
    private const string _defaultDelimiter = "\n\t";
    private static string _delimiter = _defaultDelimiter;

    // If the logger is not empty, then log the result before it's destroyed
    ~StringLogger()
    {
        if (!_logged)
        {
            Log();
            _logged = false;
        }
    }

    // For compliance, remove historical logs

    private static readonly TimeSpan thirty = TimeSpan.FromDays(30);
    private static void RemoveHistoricalLogs(params FileInfo[] logFiles)
    {
        // Check the current time
        var now = DateTime.Now;

        // Files to delete
        List<FileInfo> filesForDelete = new(logFiles.Length);

        // Iterate through the given log files and delete any that are older than thirty days ago
        foreach (var file in logFiles)
        {
            DirectoryInfo dir = file.Directory!;
            FileInfo[] files = dir.GetFiles();
            foreach (var f in files)
            {
                // Split the name of the current file by the file name date delimiter
                string[] nameSplit = f.Name.Split(DateTimeStrings.FileDateTimeFormat2Delimiter);

                // Attempt to find the part of the name that is an actual date
                foreach (var name in nameSplit)
                {
                    if (DateTime.TryParse(name, out DateTime dateTime))
                    {
                        // Check whether the date 
                        if (DateTime.Compare(dateTime.Date, now.Date - thirty) < 0)
                        {
                            filesForDelete.Add(f);
                        }
                    }
                }
            }
        }

        // Delete the collected files
        foreach (var file in filesForDelete)
        {
            File.Delete(file.FullName);
        }
    }
    #endregion
}
