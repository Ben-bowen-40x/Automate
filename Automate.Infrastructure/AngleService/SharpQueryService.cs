using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.AngleService;

public class SharpQueryService(ISharpQuerySettings qsettings)
{
    private readonly ISharpQuerySettings settings = qsettings;

    public async Task<Result> SubmitForm_Guliagar()
    {
        IConfiguration config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
        IBrowsingContext context = BrowsingContext.New(config);
        IDocument doc = await context.OpenAsync(settings.GuliagarUrl!);
        IHtmlFormElement? form = doc.QuerySelector<IHtmlFormElement>("form");

        // Instantiate explicit form objects
        if (form is not null)
        {
            // Find and set the first element
            var e1Input = form.Elements[settings.GuliagarNameElement!] as IHtmlFormElement;
            if (e1Input is not null) e1Input.NodeValue = settings.GuliagarName!;
            else return Result.Failure($"{nameof(settings.GuliagarNameElement)} did not exist, therefore, we could not continue.");

            // Find and set the second element
            var e2Input = form.Elements[settings.GuliagarKeyElement!] as IHtmlFormElement;
            if (e2Input is not null) e2Input.NodeValue = settings.GuliagarKey!;
            else return Result.Failure($"{nameof(settings.GuliagarKeyElement)} did not exist, therefore, we could not continue.");

            // Submit the form
            IDocument initialPage = await form.SubmitAsync(new { e1Input, e2Input });
        }

        return Result.Failure($"The {nameof(IHtmlFormElement)} called {nameof(form)} in the code could not be found. Here was the offered Url's name as saved in settings: {nameof(settings.GuliagarUrl)}\nHere is the url itself: {settings.GuliagarUrl}");
    }
}

