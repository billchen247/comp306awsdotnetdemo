using System.Net;
using System.Text.Json;
using Example10WeekWebApiClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example10WeekWebApiClient.Controllers;

public class ApiDemoController : Controller
{
    private static readonly string[] CandidateUrls =
    [
        "http://ecsweek10todoapi-1437806556.us-east-1.elb.amazonaws.com:8080/api/Tod",
        "http://ecsweek10todoapi-1437806556.us-east-1.elb.amazonaws.com:8080/api/Todo"
    ];

    private readonly IHttpClientFactory _httpClientFactory;

    public ApiDemoController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult HttpWebRequestDemo()
    {
        var model = new ApiDemoResultViewModel
        {
            MethodName = "HttpWebRequest/HttpWebResponse"
        };

        foreach (var url in CandidateUrls)
        {
            try
            {
#pragma warning disable SYSLIB0014
                var request = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014
                request.Method = "GET";

                using var response = (HttpWebResponse)request.GetResponse();
                using var responseStream = response.GetResponseStream();
                using var reader = new StreamReader(responseStream!);
                var json = reader.ReadToEnd();

                model.Items = ParseTodoItems(json);
                model.EndpointUsed = url;
                return View(model);
            }
            catch
            {
                // Try next URL candidate.
            }
        }

        model.ErrorMessage = "Failed to retrieve TODO items from all candidate endpoints.";
        return View(model);
    }

    public IActionResult WebClientDemo()
    {
        var model = new ApiDemoResultViewModel
        {
            MethodName = "WebClient"
        };

        foreach (var url in CandidateUrls)
        {
            try
            {
#pragma warning disable SYSLIB0014
                using var client = new WebClient();
#pragma warning restore SYSLIB0014
                var json = client.DownloadString(url);

                model.Items = ParseTodoItems(json);
                model.EndpointUsed = url;
                return View(model);
            }
            catch
            {
                // Try next URL candidate.
            }
        }

        model.ErrorMessage = "Failed to retrieve TODO items from all candidate endpoints.";
        return View(model);
    }

    public async Task<IActionResult> HttpClientDemo()
    {
        var model = new ApiDemoResultViewModel
        {
            MethodName = "HttpClient (async)"
        };

        var client = _httpClientFactory.CreateClient();

        foreach (var url in CandidateUrls)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync();
                model.Items = ParseTodoItems(json);
                model.EndpointUsed = url;
                return View(model);
            }
            catch
            {
                // Try next URL candidate.
            }
        }

        model.ErrorMessage = "Failed to retrieve TODO items from all candidate endpoints.";
        return View(model);
    }

    private static List<TodoItem> ParseTodoItems(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<TodoItem>>(json, JsonOptions) ?? [];
        }

        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("value", out var valueElement) && valueElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<TodoItem>>(valueElement.GetRawText(), JsonOptions) ?? [];
        }

        return [];
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
