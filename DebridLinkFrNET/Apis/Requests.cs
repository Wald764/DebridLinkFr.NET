using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using DebridLinkFrNET.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DebridLinkFrNET;

internal class Requests
{
    private readonly HttpClient _httpClient;
    private readonly Store _store;

    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };

    public Requests(HttpClient httpClient, Store store)
    {
        _httpClient = httpClient;
        _store = store;
    }

    private async Task<(HttpStatusCode StatusCode, String? ReasonPhrase, String? Text)> Request(String url,
                                        Boolean requireAuthentication, 
                                        RequestType requestType,
                                        HttpContent? data,
                                        IDictionary<String, String>? parameters,
                                        CancellationToken cancellationToken)
    {
        url = $"{Store.API_URL}{url}";

        if (parameters is {Count: > 0})
        {
            var parametersString = String.Join("&", parameters.Select(m => $"{m.Key}={HttpUtility.UrlEncode(m.Value)}"));

            url = $"{url}?{parametersString}";
        }

        var method = requestType switch
        {
            RequestType.Get => HttpMethod.Get,
            RequestType.Post => HttpMethod.Post,
            RequestType.Put => HttpMethod.Put,
            RequestType.Delete => HttpMethod.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
        };

        // The Authorization header is set on the request itself rather than on HttpClient.DefaultRequestHeaders:
        // the HttpClient may be shared between threads, clients or the caller's own code.
        using var request = new HttpRequestMessage(method, url);
        request.Content = data;

        if (requireAuthentication)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _store.ApiKey);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return (response.StatusCode, response.ReasonPhrase, null);
        }

        var buffer = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        var text = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

        return (response.StatusCode, response.ReasonPhrase, text);
    }

    private async Task<T?> RequestValue<T>(String url,
                                           Boolean requireAuthentication,
                                           RequestType requestType,
                                           HttpContent? data,
                                           IDictionary<String, String>? parameters,
                                           CancellationToken cancellationToken)
    {
        var (statusCode, reasonPhrase, requestResult) = await Request(url, requireAuthentication, requestType, data, parameters, cancellationToken).ConfigureAwait(false);

        var isSuccessStatusCode = (Int32) statusCode is >= 200 and <= 299;

        if (requestResult == null)
        {
            if (!isSuccessStatusCode)
            {
                throw CreateHttpException(statusCode, reasonPhrase);
            }

            return default;
        }

        ApiResponse<T>? result;

        try
        {
            result = JsonConvert.DeserializeObject<ApiResponse<T>>(requestResult, JsonSerializerSettings);
        }
        catch (JsonException ex)
        {
            if (!isSuccessStatusCode)
            {
                // Typically an HTML error page (proxy, maintenance...).
                throw CreateHttpException(statusCode, reasonPhrase);
            }

            throw new Exception($"Unable to deserialize DebridLinkFr API response to {typeof(T).Name}. Response was: {requestResult}", ex);
        }

        if (result == null)
        {
            if (!isSuccessStatusCode)
            {
                throw CreateHttpException(statusCode, reasonPhrase);
            }

            throw new Exception($"Unable to deserialize DebridLinkFr API response to {typeof(T).Name}. Response was: {requestResult}",
                                new Exception("Response was null"));
        }

        if (!result.Success)
        {
            if (!String.IsNullOrWhiteSpace(result.Error))
            {
                throw new DebridLinkFrException(result.Error!, result.Error!, statusCode);
            }

            if (!isSuccessStatusCode)
            {
                throw CreateHttpException(statusCode, reasonPhrase);
            }

            throw new Exception($"Unable to deserialize DebridLinkFr API response to {typeof(T).Name}. Response was: {requestResult}",
                                new JsonSerializationException($"Unknown error. Response was: {result}"));
        }

        if (!isSuccessStatusCode)
        {
            throw CreateHttpException(statusCode, reasonPhrase);
        }

        return result.Value;
    }

    private static DebridLinkFrException CreateHttpException(HttpStatusCode statusCode, String? reasonPhrase)
    {
        return new DebridLinkFrException($"Unexpected HTTP status {(Int32) statusCode} {reasonPhrase}".TrimEnd(), "httpError", statusCode);
    }

    private async Task<T> Request<T>(String url,
                                     Boolean requireAuthentication,
                                     RequestType requestType,
                                     HttpContent? data,
                                     IDictionary<String, String>? parameters,
                                     CancellationToken cancellationToken)
        where T : class, new()
    {
        var value = await RequestValue<T>(url, requireAuthentication, requestType, data, parameters, cancellationToken).ConfigureAwait(false);

        return value ?? new T();
    }

    /// <summary>
    ///     POST request whose "value" can be either a single object or an array of objects.
    ///     The response is read only once, the request is never sent twice.
    /// </summary>
    public async Task<List<T>> PostRequestSingleOrListAsync<T>(String url, IEnumerable<KeyValuePair<String, String>>? data, Boolean requireAuthentication, CancellationToken cancellationToken)
        where T : class
    {
        using var content = data != null ? new FormUrlEncodedContent(data) : null;
        var value = await RequestValue<JToken>(url, requireAuthentication, RequestType.Post, content, null, cancellationToken).ConfigureAwait(false);

        var serializer = JsonSerializer.Create(JsonSerializerSettings);

        return value switch
        {
            null or { Type: JTokenType.Null } => new List<T>(),
            JArray array => array.ToObject<List<T>>(serializer) ?? new List<T>(),
            _ => value.ToObject<T>(serializer) is { } single ? new List<T> { single } : new List<T>()
        };
    }
        
    public async Task<T> GetRequestAsync<T>(String url, Boolean requireAuthentication, IDictionary<String, String>? parameters, CancellationToken cancellationToken)
        where T : class, new()
    {
        return await Request<T>(url, requireAuthentication, RequestType.Get, null, parameters, cancellationToken).ConfigureAwait(false);
    }
        
    public async Task<T> DeleteRequestAsync<T>(String url, Boolean requireAuthentication, IDictionary<String, String>? parameters, CancellationToken cancellationToken)
        where T : class, new()
    {
        return await Request<T>(url, requireAuthentication, RequestType.Delete, null, parameters, cancellationToken).ConfigureAwait(false);
    }
        
    public async Task<T> PostRequestAsync<T>(String url, IEnumerable<KeyValuePair<String, String>>? data, Boolean requireAuthentication, CancellationToken cancellationToken)
        where T : class, new()
    {
        var content = data != null ? new FormUrlEncodedContent(data) : null;
        return await Request<T>(url, requireAuthentication, RequestType.Post, content, null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> PostFileRequestAsync<T>(String url, Byte[] file, Boolean requireAuthentication, CancellationToken cancellationToken)
        where T : class, new()
    {
        using var multipartFormDataContent = new MultipartFormDataContent();
        multipartFormDataContent.Headers.ContentType.MediaType = "multipart/form-data";

        var fileContent = new StreamContent(new MemoryStream(file));
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") 
        { 
            Name = "file",
            FileName = "1.torrent"
        };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-bittorrent");

        multipartFormDataContent.Add(fileContent);
            
        return await Request<T>(url, requireAuthentication, RequestType.Post, multipartFormDataContent, null, cancellationToken).ConfigureAwait(false);
    }
        
    private enum RequestType
    {
        Get,
        Post,
        Put,
        Delete
    }
}