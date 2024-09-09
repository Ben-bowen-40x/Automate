using Automate.Infrastructure.LeafClientService;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using System.Collections;
using System.Net.Http.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Automate.Infrastructure.GenericClientService;

internal class GenericClient
{
    private const string _clientName = "GenericClient1112223334445556667778889999";
    /// <summary>
    /// <para>The <paramref name="baseUri"/> will be treated as full, with all parameters and endpoints in their proper order if <paramref name="headers"/> is default</para>
    /// <para>In such case as <paramref name="name"/> is default, that SHOULD mean that <paramref name="headers"/> is unnecessary</para>
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="factory"></param>
    /// <param name="baseUri"></param>
    /// <param name="headers"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<R> Call<T, R>(IHttpClientFactory factory, Uri baseUri, out string error, HeaderAndEndpointValues? headers = default, string name = "")
    {
        // Create the client
        HttpClient client = name == string.Empty
            ? factory.CreateClient(_clientName)
            : factory.CreateClient(name);

        // Set the base address
        if (headers != default)
        {
            client.BaseAddress = headers.ApplyAddOns(baseUri);
            if (headers.AuthorizationHeader != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", headers.AuthorizationHeader);
            }
        }
        else
        {
            client.BaseAddress = baseUri;
        }

        // Make the call
        error = "";
        try
        {
            return AttemptToGet<R>(ref error, client);
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        return [];

        static List<R> AttemptToGet<R>(ref string error, HttpClient client)
        {
            Task<HttpResponseMessage> message = GetAsync(client);
            if (message.IsCompletedSuccessfully)
            {
                HttpResponseMessage response = message.Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<R[]?> wrappedResult = GetResponseAsync<R>(response);
                    if (wrappedResult.IsCompletedSuccessfully)
                    {
                        R[]? nullableResult = wrappedResult.Result;

                        if (nullableResult is not null && nullableResult.Length > 0)
                            return [.. nullableResult!];
                        error = "Either the result was nullable or the result was empty";
                    }
                    error = wrappedResult.Exception!.Message;
                }
                error = response.ReasonPhrase!;
            }
            error = "Attempt failure";
            return [];
        }
    }
    private static async Task<HttpResponseMessage> GetAsync(HttpClient client)
    {
        return await client.GetAsync(client.BaseAddress);
    }
    private static async Task<R[]?> GetResponseAsync<R>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<R[]>();
    }
}

internal class HeaderAndEndpointValues
{
    public const string DefaultAcceptHeader = "application/json";

    private string? authorizationHeader;

    public string? AuthorizationHeader
    {
        get { return authorizationHeader; }
        private set { authorizationHeader = value; }
    }

    /// <summary>
    /// <para>If the <paramref name="type"/> is a bearer and the <paramref name="value"/> is a token, then the result will be</para>
    /// <para><paramref name="type"/> <paramref name="value"/></para>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string SetAuthorizationHeader(string type, string value) => authorizationHeader ??= type + " " + value;

    /// <summary>
    /// The caller must ensure that these strings are actual endpoints and endpoint parameters in the correct order
    /// </summary>
    public string[]? AddOnsToTheBaseUri { private get; set; }

    /// <summary>
    /// This method applies <see cref="AddOnsToTheBaseUri"/>
    /// </summary>
    /// <returns><see cref="Uri"/></returns>
    public Uri ApplyAddOns(Uri baseUri)
    {
        return AddOnsToTheBaseUri is null
            ? baseUri
            : new(baseUri.OriginalString + string.Join("", AddOnsToTheBaseUri));
    }

    /// <summary>
    /// This method applies <see cref="AddOnsToTheBaseUri"/>
    /// </summary>
    /// <returns><see cref="Uri"/></returns>
    public Uri ApplyAddOns(string baseUriStr)
    {
        return ApplyAddOns(new Uri(baseUriStr));
    }
}