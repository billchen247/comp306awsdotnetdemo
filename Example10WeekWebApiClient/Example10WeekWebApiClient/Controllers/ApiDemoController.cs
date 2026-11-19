using System.Net;
using System.Text;
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
                model.EndpointUsed = url;
                model.OperationLogs.Add($"GET all: {GetStatusText(HttpStatusCode.OK)}");

                var initialJson = SendHttpWebRequest(url, "GET");
                model.Items = ParseTodoItems(initialJson);

                var createdItem = new TodoItem
                {
                    Title = $"HttpWebRequest Demo {DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = "Created from HttpWebRequest CRUD demo",
                    IsComplete = false
                };

                var createJson = SendHttpWebRequest(url, "POST", createdItem);
                var created = ParseTodoItem(createJson);
                var createdId = created?.Id ?? 0;

                model.OperationLogs.Add(createdId > 0
                    ? $"POST create: {GetStatusText(HttpStatusCode.Created)} (Id={createdId})"
                    : $"POST create: {GetStatusText(HttpStatusCode.Created)}");

                if (createdId > 0)
                {
                    var updateItem = new TodoItem
                    {
                        Id = createdId,
                        Title = $"HttpWebRequest Updated {DateTime.UtcNow:yyyyMMddHHmmss}",
                        Description = "Updated from HttpWebRequest CRUD demo",
                        IsComplete = true
                    };

                    SendHttpWebRequest($"{url}/{createdId}", "PUT", updateItem);
                    model.OperationLogs.Add($"PUT update: {GetStatusText(HttpStatusCode.NoContent)}");

                    SendHttpWebRequest($"{url}/{createdId}", "DELETE");
                    model.OperationLogs.Add($"DELETE: {GetStatusText(HttpStatusCode.NoContent)}");
                }
                else
                {
                    model.OperationLogs.Add("PUT update: skipped (could not determine created item id).");
                    model.OperationLogs.Add("DELETE: skipped (could not determine created item id).");
                }

                var finalJson = SendHttpWebRequest(url, "GET");
                model.Items = ParseTodoItems(finalJson);
                return View(model);
            }
            catch (Exception ex)
            {
                model.OperationLogs.Add($"Endpoint failed ({url}): {ex.Message}");
            }
        }

        model.ErrorMessage = "Failed to run CRUD demo on all candidate endpoints.";
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
                model.EndpointUsed = url;

                var initialJson = SendWebClientRequest(url, "GET");
                model.Items = ParseTodoItems(initialJson);
                model.OperationLogs.Add($"GET all: {GetStatusText(HttpStatusCode.OK)}");

                var createdItem = new TodoItem
                {
                    Title = $"WebClient Demo {DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = "Created from WebClient CRUD demo",
                    IsComplete = false
                };

                var createJson = SendWebClientRequest(url, "POST", createdItem);
                var created = ParseTodoItem(createJson);
                var createdId = created?.Id ?? 0;

                model.OperationLogs.Add(createdId > 0
                    ? $"POST create: {GetStatusText(HttpStatusCode.Created)} (Id={createdId})"
                    : $"POST create: {GetStatusText(HttpStatusCode.Created)}");

                if (createdId > 0)
                {
                    var updateItem = new TodoItem
                    {
                        Id = createdId,
                        Title = $"WebClient Updated {DateTime.UtcNow:yyyyMMddHHmmss}",
                        Description = "Updated from WebClient CRUD demo",
                        IsComplete = true
                    };

                    SendWebClientRequest($"{url}/{createdId}", "PUT", updateItem);
                    model.OperationLogs.Add($"PUT update: {GetStatusText(HttpStatusCode.NoContent)}");

                    SendWebClientRequest($"{url}/{createdId}", "DELETE");
                    model.OperationLogs.Add($"DELETE: {GetStatusText(HttpStatusCode.NoContent)}");
                }
                else
                {
                    model.OperationLogs.Add("PUT update: skipped (could not determine created item id).");
                    model.OperationLogs.Add("DELETE: skipped (could not determine created item id).");
                }

                var finalJson = SendWebClientRequest(url, "GET");
                model.Items = ParseTodoItems(finalJson);
                return View(model);
            }
            catch (Exception ex)
            {
                model.OperationLogs.Add($"Endpoint failed ({url}): {ex.Message}");
            }
        }

        model.ErrorMessage = "Failed to run CRUD demo on all candidate endpoints.";
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
                model.EndpointUsed = url;

                var listResponse = await client.GetAsync(url);
                if (!listResponse.IsSuccessStatusCode)
                {
                    model.OperationLogs.Add($"GET all failed on {url}: {(int)listResponse.StatusCode} {listResponse.ReasonPhrase}");
                    continue;
                }

                var initialJson = await listResponse.Content.ReadAsStringAsync();
                model.Items = ParseTodoItems(initialJson);
                model.OperationLogs.Add($"GET all: {GetStatusText(listResponse.StatusCode)}");

                var createdItem = new TodoItem
                {
                    Title = $"HttpClient Demo {DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = "Created from HttpClient CRUD demo",
                    IsComplete = false
                };

                var createContent = BuildJsonContent(createdItem);
                var createResponse = await client.PostAsync(url, createContent);
                if (!createResponse.IsSuccessStatusCode)
                {
                    model.OperationLogs.Add($"POST create failed: {GetStatusText(createResponse.StatusCode)}");
                    continue;
                }

                var createJson = await createResponse.Content.ReadAsStringAsync();
                var created = ParseTodoItem(createJson);
                var createdId = created?.Id ?? 0;
                model.OperationLogs.Add(createdId > 0
                    ? $"POST create: {GetStatusText(createResponse.StatusCode)} (Id={createdId})"
                    : $"POST create: {GetStatusText(createResponse.StatusCode)}");

                if (createdId > 0)
                {
                    var updateItem = new TodoItem
                    {
                        Id = createdId,
                        Title = $"HttpClient Updated {DateTime.UtcNow:yyyyMMddHHmmss}",
                        Description = "Updated from HttpClient CRUD demo",
                        IsComplete = true
                    };

                    var updateResponse = await client.PutAsync($"{url}/{createdId}", BuildJsonContent(updateItem));
                    model.OperationLogs.Add(updateResponse.IsSuccessStatusCode
                        ? $"PUT update: {GetStatusText(updateResponse.StatusCode)}"
                        : $"PUT update failed: {GetStatusText(updateResponse.StatusCode)}");

                    var deleteResponse = await client.DeleteAsync($"{url}/{createdId}");
                    model.OperationLogs.Add(deleteResponse.IsSuccessStatusCode
                        ? $"DELETE: {GetStatusText(deleteResponse.StatusCode)}"
                        : $"DELETE failed: {GetStatusText(deleteResponse.StatusCode)}");
                }
                else
                {
                    model.OperationLogs.Add("PUT update: skipped (could not determine created item id).");
                    model.OperationLogs.Add("DELETE: skipped (could not determine created item id).");
                }

                var finalResponse = await client.GetAsync(url);
                if (finalResponse.IsSuccessStatusCode)
                {
                    var finalJson = await finalResponse.Content.ReadAsStringAsync();
                    model.Items = ParseTodoItems(finalJson);
                }
                else
                {
                    model.OperationLogs.Add($"Final GET all failed: {GetStatusText(finalResponse.StatusCode)}");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                model.OperationLogs.Add($"Endpoint failed ({url}): {ex.Message}");
            }
        }

        model.ErrorMessage = "Failed to run CRUD demo on all candidate endpoints.";
        return View(model);
    }

    private static string SendHttpWebRequest(string url, string method, TodoItem? payload = null)
    {
#pragma warning disable SYSLIB0014
        var request = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014
        request.Method = method;
        request.ContentType = "application/json";

        if (payload is not null)
        {
            var body = JsonSerializer.Serialize(payload, JsonOptions);
            using var requestStream = request.GetRequestStream();
            using var writer = new StreamWriter(requestStream);
            writer.Write(body);
        }

        using var response = (HttpWebResponse)request.GetResponse();
        using var responseStream = response.GetResponseStream();
        using var reader = new StreamReader(responseStream!);
        return reader.ReadToEnd();
    }

    private static string SendWebClientRequest(string url, string method, TodoItem? payload = null)
    {
#pragma warning disable SYSLIB0014
        using var client = new WebClient();
#pragma warning restore SYSLIB0014
        client.Headers[HttpRequestHeader.ContentType] = "application/json";

        if (payload is null)
        {
            return method == "GET"
                ? client.DownloadString(url)
                : client.UploadString(url, method, string.Empty);
        }

        var body = JsonSerializer.Serialize(payload, JsonOptions);
        return client.UploadString(url, method, body);
    }

    private static StringContent BuildJsonContent(TodoItem item)
    {
        return new StringContent(JsonSerializer.Serialize(item, JsonOptions), Encoding.UTF8, "application/json");
    }

    private static TodoItem? ParseTodoItem(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("value", out var valueElement) && valueElement.ValueKind == JsonValueKind.Object)
            {
                return JsonSerializer.Deserialize<TodoItem>(valueElement.GetRawText(), JsonOptions);
            }

            return JsonSerializer.Deserialize<TodoItem>(json, JsonOptions);
        }

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            var first = root[0];
            return first.ValueKind == JsonValueKind.Object
                ? JsonSerializer.Deserialize<TodoItem>(first.GetRawText(), JsonOptions)
                : null;
        }

        return null;
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

    private static string GetStatusText(HttpStatusCode statusCode)
    {
        return $"{(int)statusCode} {statusCode}";
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
