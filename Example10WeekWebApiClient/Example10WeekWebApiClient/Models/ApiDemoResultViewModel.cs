namespace Example10WeekWebApiClient.Models;

public class ApiDemoResultViewModel
{
    public string MethodName { get; set; } = string.Empty;
    public string EndpointUsed { get; set; } = string.Empty;
    public List<TodoItem> Items { get; set; } = [];
    public List<string> OperationLogs { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
