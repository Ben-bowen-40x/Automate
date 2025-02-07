using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CSharpFunctionalExtensions;
using HtmlAgilityPack;
using System.Net;

namespace Automate.Infrastructure.AngleService;

public class SharpQueryService
{
    #region Ignore
    // Ctor and weird class are used for testing only
    private class Factory(ISharpQuerySettings _settings) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name = "")
        {
            // Create an HttpClientHandler and assign the CookieContainer to it
            HttpClientHandler handler = new()
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true, // Ensure that the handler uses the CookieContainer
            };

            HttpClient client = new(handler)
            {
                BaseAddress = new Uri(_settings.GuliagarBase!)
            };
            return client;
        }
    }

    /// <summary>
    /// Used in testing only
    /// </summary>
    /// <param name="settings"></param>
    internal SharpQueryService(IInfrastructureSettings settings)
    {
        _createClientInput = settings;
        _sqSettings = settings;
        _factory = new Factory(_sqSettings);
    }

    #endregion
    private readonly IInfrastructureSettings _createClientInput;
    private readonly ISharpQuerySettings _sqSettings;
    private readonly IHttpClientFactory _factory;
    public SharpQueryService(IInfrastructureSettings settings, IHttpClientFactory factory) : this(settings)
    {
        _factory = factory;
    }

    public async Task<Result> SubmitForm_Guliagar()
    {
        IConfiguration config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
        IBrowsingContext context = BrowsingContext.New(config);
        Url url = new(_sqSettings.GuliagarUrl2!);
        Task<IDocument> doc = context.OpenAsync(url);
        if (doc.IsCompletedSuccessfully)
        {
            IHtmlFormElement? form = doc.Result.QuerySelector<IHtmlFormElement>("form");

            // Instantiate explicit form objects
            if (form is not null)
            {
                // Find and set the first element
                IHtmlFormElement? e1Input = form.Elements[_sqSettings.GuliagarNameElement!] as IHtmlFormElement;
                if (e1Input is not null) e1Input.NodeValue = _sqSettings.GuliagarName!;
                else return Result.Failure($"{nameof(_sqSettings.GuliagarNameElement)} did not exist, therefore, we could not continue.");

                // Find and set the second element
                var e2Input = form.Elements[_sqSettings.GuliagarKeyElement!] as IHtmlFormElement;
                if (e2Input is not null) e2Input.NodeValue = _sqSettings.GuliagarKey!;
                else return Result.Failure($"{nameof(_sqSettings.GuliagarKeyElement)} did not exist, therefore, we could not continue.");

                // Submit the form
                Task<IDocument> initialPage = form.SubmitAsync(new { e1Input, e2Input });
            }
        }

        return Result.Failure($"The {nameof(IHtmlFormElement)} could not be found. Here was the offered Url's name as saved in settings: {nameof(_sqSettings.GuliagarUrl2)}\nHere is the url itself: {_sqSettings.GuliagarUrl2}");
    }

    public async Task<Result<HttpResponseMessage>> Submit_Form_Guliagar()
    {
        try
        {
            // Send request to url
            HttpClient client = _factory.CreateClient(_createClientInput.Cookie!);

            // Submit to form
            MultipartFormDataContent content = new()
            {
                { new StringContent(_sqSettings.GuliagarName!), "username" },
                { new StringContent(_sqSettings.GuliagarKey!), "password" }
            };
            HttpResponseMessage submissionResp = await client.PostAsync(_sqSettings.GuliagarBase! + "/data/login/", content);

            // Hope that the cookie works and that we are now allowed to get the next page, beyond the form
            HttpResponseMessage response = await client.GetAsync(_sqSettings.GuliagarUrl2!);

            HtmlDocument htmlDoc = new();
            var html = await response.Content.ReadAsStringAsync();
            htmlDoc.LoadHtml(html);

            return response;
        }
        catch (Exception e) { return Result.Failure<HttpResponseMessage>($"The call failed and produced the following error: {e.Message}"); }
    }
}

