#pragma warning disable OPENAI001 // Responses API is experimental in the OpenAI .NET SDK
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Responses;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Text.Json.Serialization;
 
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();
 
var deploymentName = config["AZURE_OPENAI_DEPLOYMENT"] ?? "gpt-6-astra";
var resourceEndpoint = config["AZURE_AI_ENDPOINT"]
    ?? throw new InvalidOperationException(
        "AZURE_AI_ENDPOINT is not set. Run: dotnet user-secrets set \"AZURE_AI_ENDPOINT\" \"&lt;your-endpoint&gt;\"");

// The Responses API is only reachable on the v1 surface, not the deployments/api-version surface.
var responsesEndpoint = new Uri($"{resourceEndpoint.TrimEnd('/')}/openai/v1");
var tokenPolicy = new BearerTokenPolicy(new DefaultAzureCredential(), "https://ai.azure.com/.default");

// gpt-6-astra doesn't allow tools + reasoning_effort on /chat/completions; use /responses instead.
IChatClient chatClient = new ResponsesClient(
        authenticationPolicy: tokenPolicy,
        options: new ResponsesClientOptions { Endpoint = responsesEndpoint })
    .AsIChatClient(deploymentName)
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(GetRecentErrorLogs),
        AIFunctionFactory.Create(GetRecentCommits)
    ],
    // Deep, multi-step decision support - worth paying for higher reasoning effort.
    AdditionalProperties = new AdditionalPropertiesDictionary
    {
        ["reasoning_effort"] = "high" // low | medium | high
    }
};

//Use Case 1: Software Engineering — Deliberate Bug Investigation and Fix Proposal 

var messages = new List<ChatMessage>
{
    new(ChatRole.System,
        "You are a senior engineer investigating a production bug. Pull both recent error " +
        "logs and recent commit history before concluding a root cause. State your recommended " +
        "fix, your confidence level, and the next action a human reviewer should take."),
    new(ChatRole.User, "Users report 'checkout-api' intermittently returns HTTP 500 on order submission since this morning. What's going on and what should we do?")
};

// Use Case 1: Software Engineering — Deliberate Bug Investigation and Fix Proposal
Console.WriteLine("Use Case 1: Software Engineering — Deliberate Bug Investigation and Fix Proposal");
Console.WriteLine("*********************************************************************************");
var response = await chatClient.GetResponseAsync(messages, chatOptions);
Console.WriteLine(response.Text);
Console.WriteLine("*********************************************************************************");

// --- Tool stand-ins for real observability/source-control APIs ---

[Description("Gets recent error log entries for a named service.")]
static string GetRecentErrorLogs(
    [Description("The service name, e.g. checkout-api")] string serviceName)
{
    return serviceName switch
    {
        "checkout-api" => "09:14 NullReferenceException at OrderTotalCalculator.Apply(discount). " +
                           "Occurs on ~8% of requests, only when a promo code is present.",
        _ => "No recent errors found."
    };
}
 
[Description("Gets a summary of recent commits merged to a named service's main branch.")]
static string GetRecentCommits(
    [Description("The service name, e.g. checkout-api")] string serviceName)
{
    return serviceName switch
    {
        "checkout-api" => "06:40 - 'Refactor discount pipeline to support stacked promo codes' " +
                           "(touches OrderTotalCalculator.cs, PromoCodeResolver.cs).",
        _ => "No recent commits found."
    };
}

// Use Case 2: Business Intelligence — Power BI Insight Synthesis
 
var chatOptionsBI = new ChatOptions
{
    ResponseFormat = ChatResponseFormat.ForJsonSchema<RegionalInsight>()
};
 
var biMessages = new List<ChatMessage>
{
    new(ChatRole.System,
        "You are a BI analyst. Given quarterly regional sales data, identify the clearest " +
        "trade-off, recommend one action, and flag anything that needs a human to verify " +
        "before it goes in a report."),
    new(ChatRole.User, """
        Q3 regional sales summary (vs. Q2):
        - West: revenue +18%, returns +22%, avg order value flat
        - East: revenue +4%, returns -3%, avg order value +11%
        - Central: revenue -6%, returns +2%, avg order value -9%
        What should we highlight to leadership, and what's the trade-off?
        """)
};
 
var biResponse = await chatClient.GetResponseAsync<RegionalInsight>(biMessages, chatOptionsBI);
var insight = biResponse.Result;
Console.WriteLine("Case 2: Business Intelligence — Power BI Insight Synthesis");
Console.WriteLine("**********************************************************");
Console.WriteLine($"Headline: {insight.Headline}");
Console.WriteLine($"Trade-off: {insight.TradeOff}");
Console.WriteLine($"Recommended action: {insight.RecommendedAction}");
Console.WriteLine($"Needs human verification: {insight.NeedsVerification}");
Console.WriteLine("**********************************************************");


// Use Case 3: Professional Work — Template-Based Report Generation
Console.WriteLine("Case 3: Professional Work — Template-Based Report Generation");
Console.WriteLine("**********************************************************");
var reportMessages = new List<ChatMessage>
{
    new(ChatRole.System, """
        You produce weekly status reports for a project template with exactly these
        sections, in this order: Summary, Progress This Week, Risks, Next Week.
        Keep tone professional and concise. Do not invent details not provided.
        """),
    new(ChatRole.User, """
        Project: Order Fulfillment Modernization
        Raw notes from the team:
        - Migrated inventory sync job to the new event bus, passed load testing
        - Warehouse API integration is 2 days behind schedule due to a vendor sandbox outage
        - Next week: finish warehouse API integration, start UAT with ops team
        - Risk: vendor sandbox reliability could delay UAT start if it recurs
        """)
};
 
var reportResponse = await chatClient.GetResponseAsync(reportMessages);
Console.WriteLine(reportResponse.Text);
Console.WriteLine("**********************************************************");

// Use Case 4: Application Workflows — Acting Through Approved Interfaces
Console.WriteLine("Case 4: Application Workflows — Acting Through Approved Interfaces");
Console.WriteLine("**********************************************************");
var workflowChatOptions = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(LookUpCustomerRecord),
        AIFunctionFactory.Create(ProposeRecordUpdate)
    ]
};
 
var workflowMessages = new List<ChatMessage>
{
    new(ChatRole.System,
        "You process customer update requests submitted via a support form. Look up the " +
        "current record before proposing any change. Never apply an update directly - " +
        "only propose it for a human approver to confirm."),
    new(ChatRole.User, "Form submission: customer ACC-4471 says their billing email should now be finance@northwind-retail.com instead of the old one.")
};
 
var workflowResponse = await chatClient.GetResponseAsync(workflowMessages, workflowChatOptions);
Console.WriteLine(workflowResponse.Text);
Console.WriteLine("**********************************************************");
 
// --- Scoped tool stand-ins - the model proposes, a human/approved system applies ---
 
[Description("Looks up a customer record by account ID.")]
static string LookUpCustomerRecord(
    [Description("The account ID, e.g. ACC-4471")] string accountId)
{
    return accountId switch
    {
        "ACC-4471" => "Account: Northwind Retail. Current billing email: billing-old@northwind-retail.com. Status: active.",
        _ => "Account not found."
    };
}
 
[Description("Proposes a record update for human approval. Does not apply the change.")]
static string ProposeRecordUpdate(
    [Description("The account ID")] string accountId,
    [Description("The field to change")] string field,
    [Description("The new value")] string newValue)
{
    return $"Proposed update queued for approval: {accountId} / {field} -> {newValue}. Awaiting reviewer confirmation.";
}

// Use Case 5: Financial Services — Long-Context Synthesis Into an Investment Point of View
Console.WriteLine("Case 5: Financial Services — Long-Context Synthesis Into an Investment Point of View");
Console.WriteLine("**********************************************************");
var filingExcerpt = await File.ReadAllTextAsync("northwind-q3-10q-excerpt.txt");
var researchNote = await File.ReadAllTextAsync("internal-analyst-note.txt");
 
var financeMessages = new List<ChatMessage>
{
    new(ChatRole.System,
        "You are a financial analyst assistant. Synthesize the filing excerpt and internal " +
        "note into a one-page investment point of view, in the firm's house style: " +
        "Thesis, Supporting Evidence, Risks, Recommendation. Cite which source each point " +
        "came from (filing or internal note)."),
    new(ChatRole.User, $"""
        FILING EXCERPT:
        {filingExcerpt}
 
        INTERNAL ANALYST NOTE:
        {researchNote}
 
        Draft the point of view.
        """)
};
 
var financeResponse = await chatClient.GetResponseAsync(financeMessages);
Console.WriteLine(financeResponse.Text);
Console.WriteLine("**********************************************************");


record RegionalInsight(
    [property: JsonPropertyName("headline")] string Headline,
    [property: JsonPropertyName("trade_off")] string TradeOff,
    [property: JsonPropertyName("recommended_action")] string RecommendedAction,
    [property: JsonPropertyName("needs_verification")] string NeedsVerification);