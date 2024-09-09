using System.Net.Http.Json;

namespace Automate.Infrastructure.LeafClientService;

internal class LeafApiService<T>
{
    private static T? Val { get; set; }
    internal static bool Value(out T? value)
    {
        value = Val;
        return IsSuccess;
    }
    internal static bool ErrorMessage(out string message)
    {
        message = IsSuccess
            ? "There was no error"
            : ErrMessage is null ? "There was an error, but the error message was never captured" : ErrMessage!;
        return IsSuccess;
    }
    internal static bool IsSuccess { get; private set; } = false; // The default state of this object is that the result is in a failure state. Therefore, the default of IsSuccess is false

    internal static bool IsError => !IsSuccess;

    private static string? ErrMessage { get; set; }
    internal static async void GetJsonResultAsync(IHttpClientFactory factory, Uri url, string name = "")
    {
        // Ensure that any data from previous calls are cleared
        Clear();

        // Create the client
        HttpClient client = name == string.Empty
            ? factory.CreateClient()
            : factory.CreateClient(name);

        // Attempt to make the call
        try
        {
            await CallAsync(url, client);
        }
        catch (Exception ex) // Handle error
        {
            ErrMessage = $"The call failed completely with the following error: {ex.Message}";
        }

        // If we've made it to this point, then there was an error
        IsSuccess = false;

        // Local
        static void Clear()
        {
            Val = default;
            IsSuccess = default;
            ErrMessage = default;
        }
    }

    private static async Task CallAsync(Uri url, HttpClient client)
    {
        // Accept the response
        HttpResponseMessage response = await client.GetAsync(url);

        // Check whether the response succeeded
        if (response.IsSuccessStatusCode)
        {
            Val = await response.Content.ReadFromJsonAsync<T>();
            if (Val is null)
            {
                IsSuccess = false;
                ErrMessage = "The call succeeded, but it returned null values";
            }
            else
            {
                IsSuccess = true;
            }
            return; // We MUST return here. Otherwise, an error will occur
        }
        else // Handle failure status code
        {
            ErrMessage = $"The call returned with the following failure reason phrase: {response.ReasonPhrase}";
        }
    }
}
