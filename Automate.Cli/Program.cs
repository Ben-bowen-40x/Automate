using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Automate.Cli.Verbs;
using Automate.Domain.SolutionFunctionality;

namespace Automate.Cli;

internal class Program
{
    static void Main(string[] args)
    {
        #region Start Log
        object sender = new Program();
        string member = nameof(Main);
        StringLogger.NewLog(DateTime.Now, sender, member, [$"Started execution from {nameof(Program)}.{nameof(Main)}. Here are the arguments input by the user:", .. args]);
        var set = SetDotnetEnv();
        #endregion

        IHostBuilder builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.ConfigureCli(context.Configuration);
            });
        IHost host = builder.Build();
        IServiceProvider service = host.Services.CreateScope().ServiceProvider;
        ParserResult<object> result = Execute(args, service);

        #region End Log
        StringLogger.ProduceLog(DateTime.Now, sender, member, $"Ended Execution");
        #endregion
    }

    private static int Run(object obj, IServiceProvider service)
    {
        return obj switch
        {
            IVerb o => o.Run(service),
            _ => throw new Exception($"The verb used for parsing the command line does not implement {nameof(IVerb)}, which is invalid."),
        };
    }

    private static ParserResult<object> Execute(string[] args, IServiceProvider service)
    {
        Type[] types = LoadVerbs();
        ParserResult<object> result = Parser.Default.ParseArguments(args, types)
            .WithParsed(obj => Run(obj, service))
            .WithNotParsed(o => HandleError(o));

        return result;
    }

    #region Private Members
    private static Type[] LoadVerbs()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IVerb)))
            .Where(t => t.IsClass)
            .ToArray();
    }

    private static int HandleError(object o)
    {
        return o switch
        {
            IEnumerable<Error> e => Error(e),
            _ => ObjectError(o),
        };

        static int Error(IEnumerable<Error> e)
        {
            e.ToList().ForEach(r => Console.WriteLine(r.Tag));
            return ProgramErrorCodes.Error;
        }

        static int ObjectError(object o)
        {
            Console.WriteLine(o.ToString());
            return ProgramErrorCodes.Error;
        }
    }

    private static bool SetDotnetEnv()
    {
        string dev = "Development";
        string var = "DOTNET_ENVIRONMENT";
        bool reset = false;
        bool isDebug = new DoIfDebug().IsDebug();
        if (isDebug || !isDebug)
        {
            string? env = Environment.GetEnvironmentVariable(var);
            if (string.IsNullOrWhiteSpace(env) || !env.Equals(dev, StringComparison.CurrentCultureIgnoreCase))
            {
                Environment.SetEnvironmentVariable(var, dev);
            }
            reset = true;
        }
        var v = Environment.GetEnvironmentVariable(var); // Please leave this here for debugging porpoises
        return reset;
    }
    #endregion
}
