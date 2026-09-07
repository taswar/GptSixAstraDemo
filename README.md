# Gpt-6-Astra Demo, .NET C# Developer (Microsoft Foundry)

Console sample demonstrating **gpt-6-astra** in Microsoft Foundry via the **Responses API**,
using `Microsoft.Extensions.AI` for tool calling, structured JSON output, and long-context
synthesis.

## Why the Responses API

`gpt-6-astra` doesn't allow `tools` + `reasoning_effort` together on `/chat/completions`.
This sample calls the model through `OpenAI.Responses.ResponsesClient` against the
`{endpoint}/openai/v1` surface instead, wrapped as an `IChatClient` via
`Microsoft.Extensions.AI.OpenAI`.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An Azure subscription with a **Microsoft Foundry** resource that has `gpt-6-astra`
  deployed (a region that supports the Responses API)
- Microsoft Entra ID access to the resource (the sample authenticates with
  `DefaultAzureCredential`, e.g. `az login` or IDE sign-in), scoped to
  `https://ai.azure.com/.default`

### NuGet packages

| Package | Purpose |
|---|---|
| `Azure.Identity` (1.21.0) | `DefaultAzureCredential` for Microsoft Entra ID auth |
| `OpenAI` (2.12.0) | `OpenAI.Responses.ResponsesClient` — the Responses API client |
| `Microsoft.Extensions.AI` (10.9.0) | `IChatClient`, `ChatOptions`, function tools, structured output |
| `Microsoft.Extensions.AI.OpenAI` (10.9.0) | `AsIChatClient()` adapter for `ResponsesClient` |
| `Microsoft.Extensions.Configuration.UserSecrets` (10.0.11) | Loads endpoint config from user secrets |
| `Microsoft.Extensions.Configuration.EnvironmentVariables` (10.0.11) | Loads endpoint config from environment variables |

## Configuration

Set your Foundry resource endpoint with user secrets (recommended for local dev):

```powershell
dotnet user-secrets set "AZURE_AI_ENDPOINT" "https://<your-resource-name>.cognitiveservices.azure.com"
```

Or via an environment variable:

```powershell
$env:AZURE_AI_ENDPOINT = "https://<your-resource-name>.cognitiveservices.azure.com"
```

Optionally override the deployment name (defaults to `gpt-6-astra`):

```powershell
dotnet user-secrets set "AZURE_OPENAI_DEPLOYMENT" "<your-deployment-name>"
```

## Running the program

```powershell
cd GptSixAstraDemo
dotnet restore
dotnet build
dotnet run
```

The Financial Services use case reads two input files from the project's working
directory — `northwind-q3-10q-excerpt.txt` and `internal-analyst-note.txt` — which are
already included in this folder.

## Sample use cases

1. **Software Engineering — Deliberate Bug Investigation and Fix Proposal**
   Uses function tools (`GetRecentErrorLogs`, `GetRecentCommits`) with high reasoning
   effort to investigate a production bug and propose a fix with a confidence level.

2. **Business Intelligence — Power BI Insight Synthesis**
   Uses structured JSON output (`ChatResponseFormat.ForJsonSchema<RegionalInsight>`) to
   turn quarterly regional sales data into a headline, trade-off, and recommended action.

3. **Professional Work — Template-Based Report Generation**
   Produces a weekly status report constrained to a fixed section template (Summary,
   Progress This Week, Risks, Next Week) from raw team notes.

4. **Application Workflows — Acting Through Approved Interfaces**
   Uses scoped function tools (`LookUpCustomerRecord`, `ProposeRecordUpdate`) to look up a
   customer record and propose an update for human approval, without applying it directly.

5. **Financial Services — Long-Context Synthesis Into an Investment Point of View**
   Combines a filing excerpt and an internal analyst note (read from local text files) into
   a one-page investment point of view: Thesis, Supporting Evidence, Risks, Recommendation.
