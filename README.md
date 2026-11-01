# GPT-6 Astra C# Demo (Microsoft Foundry)

![GPT-6 Astra now generally available in Microsoft Foundry](https://taswar.zeytinsoft.com/wp-content/uploads/2026/09/GPT-6-Astra-for-CSharp-Developers.png)

[![Build](https://github.com/taswar/GptSixAstraDemo/actions/workflows/dotnet.yml/badge.svg)](https://github.com/taswar/GptSixAstraDemo/actions/workflows/dotnet.yml)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen)](CONTRIBUTING.md)

- 📖 Full walkthrough: [GPT-6 Astra for C# Developers](https://taswar.zeytinsoft.com/gpt-6-astra-csharp-developers-guide/?utm_source=github&utm_medium=readme&utm_campaign=GptSixAstraDemo&utm_content=walkthrough)
- ⭐ If this sample helps you, star the repo so other .NET developers can find it.
- 🔁 Related: [GPT-6.1 Sol C# Demo](https://github.com/taswar/Gpt-6.1-Sol-CSharp-Demo), which uses the same Responses API setup for the next model
- For more C# samples using other AI models, see [AI models for C# developers](https://taswar.zeytinsoft.com/foundry-ai-models-csharp/?utm_source=github&utm_medium=readme&utm_campaign=GptSixAstraDemo&utm_content=ai-models-hub)
- For my free ebook on prompt engineering, see [Prompt Engineering for .NET Developers](https://github.com/taswar/PromptEngineeringForDotNetDevelopers)

Console sample demonstrating **gpt-6-astra** in Microsoft Foundry via the **Responses API**,
using `Microsoft.Extensions.AI` for tool calling, structured JSON output, and long-context
synthesis.

## Why the Responses API

`gpt-6-astra` doesn't allow `tools` + `reasoning_effort` together on `/chat/completions`.
This sample calls the model through `OpenAI.Responses.ResponsesClient` against the
`{endpoint}/openai/v1` surface instead, wrapped as an `IChatClient` via
`Microsoft.Extensions.AI.OpenAI`.

## Quick start

```powershell
git clone https://github.com/taswar/GptSixAstraDemo.git
cd GptSixAstraDemo
dotnet user-secrets set "AZURE_AI_ENDPOINT" "https://<your-resource-name>.cognitiveservices.azure.com"
az login
dotnet run
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An Azure subscription with a **Microsoft Foundry** resource that has `gpt-6-astra`
  deployed (a region that supports the Responses API)
- Access to the resource: Microsoft Entra ID by default (the sample authenticates with
  `DefaultAzureCredential`, e.g. `az login` or IDE sign-in, scoped to
  `https://ai.azure.com/.default`), or an API key for a first try

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

Optionally use an API key instead of Entra ID. When `AZURE_OPENAI_API_KEY` is set, the sample
sends it in the `api-key` header; leave it unset to use `DefaultAzureCredential`:

```powershell
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "<your-api-key>"
```

Entra ID is the better choice for anything beyond a first try.

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

## Project layout

```
Program.cs                    Client setup, the five use cases and the tool stand-ins
GptSixAstraDemo.csproj        .NET 10 project and package references
northwind-q3-10q-excerpt.txt  Fictional filing excerpt for use case 5
internal-analyst-note.txt     Fictional analyst note for use case 5
CONTRIBUTING.md               How to contribute
.github/workflows/dotnet.yml  Build workflow
LICENSE                       MIT license
```

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
