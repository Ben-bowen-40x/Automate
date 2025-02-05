using Automate.Domain.SolutionFunctionality;
using System.Text;

namespace Automate.Domain.Test;

public class StringLoggerTests
{
    [Fact]
    public void StringLogger_LogsAsExpected()
    {
        // Assemble and Act
        int argCounter = 0;
        StringLogger.NewLog(DateTime.Now, $"This is argument {++argCounter}");
        string? firstEntry = StringLogger._firstEntry;
        StringBuilder? builder = StringLogger._builder;
        bool newlogcalled = StringLogger._newLogCalled;

        // Assert
        Assert.NotNull(firstEntry);
        Assert.NotNull(builder);
        Assert.True(newlogcalled);
    }
}
