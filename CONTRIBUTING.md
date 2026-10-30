# Contributing

Thanks for helping improve this sample. It's a .NET 10 console app with five use cases for GPT-6 Astra on Microsoft Foundry, using the Responses API through `Microsoft.Extensions.AI` and Entra ID: a bug investigation with tools, a BI insight as structured JSON, a templated status report, a human-approved record update, and an investment point of view from two documents. Contributions that make it clearer, more correct, or more useful to other .NET developers are welcome.

## Ways to contribute

- **Report a bug.** A use case fails, or the README doesn't match what the code does.
- **Fix the docs.** Typos, unclear steps, or a missing troubleshooting case.
- **Improve the sample.** For example, running one use case from the command line, or printing token usage for each call.
- **Share results.** If you ran a use case against your own data, an issue describing what the model got right or wrong helps others.

For anything larger than a small fix, open an issue first so we can agree on the approach before you write code.

## Set up

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and the Azure CLI.
2. Clone the repo and build it:

   ```bash
   git clone https://github.com/taswar/GptSixAstraDemo.git
   cd GptSixAstraDemo
   dotnet build
   ```

3. To run it, deploy `gpt-6-astra` on a Microsoft Foundry resource in a region that supports the Responses API, set `AZURE_AI_ENDPOINT` with user secrets, and sign in with `az login`, as described in the [README](README.md#configuration). The sample uses Entra ID only, with no API key.

## Guidelines

- **Never commit secrets.** No keys, endpoints, tenant IDs or tokens in code, config files or screenshots. Settings belong in user secrets.
- **Keep the Responses API.** GPT-6 Astra doesn't allow tools with reasoning effort on `/chat/completions`. Keep the `ResponsesClient` on the `/openai/v1` endpoint.
- **Keep Entra ID.** The sample uses `DefaultAzureCredential` with the `https://ai.azure.com/.default` scope. Keep it that way.
- **Keep humans in charge.** Use case 4 only proposes updates through `ProposeRecordUpdate`. Don't add tools that write, send or delete.
- **Keep the data fictional.** The logs, sales figures, notes, filing excerpt and analyst note are made up. Don't add real company filings or customer data, and don't present the investment output as advice.
- **Keep it a sample.** The code should stay short and readable in one `Program.cs`. Prefer clarity over abstraction, and add a dependency only when it clearly helps.
- **Match the existing style.** Target `net10.0`, keep nullable reference types on, and follow the layout already in `Program.cs`.
- **Keep the README in sync.** If you add a use case, change settings or change output, update the README in the same pull request.
- **One change per pull request.** Small, focused pull requests are easier to review and merge.

## Pull request checklist

- [ ] `dotnet build` succeeds with no new warnings.
- [ ] Any use case you changed runs against your own deployment, or you explain why you couldn't run it.
- [ ] No secrets, endpoints or personal data are in the diff.
- [ ] The README reflects any change in use cases, settings or output.
- [ ] The pull request description says what changed and why.

The build workflow runs on every pull request and has to pass before a merge.

## Reporting a bug

Open an issue and include:

- Which use case failed and what you expected.
- The full error message, with any endpoints removed.
- Your .NET SDK version (`dotnet --version`) and operating system.
- Your deployment name and region.

## Security issues

Please don't report security problems in a public issue. Use **Security → Report a vulnerability** on the repo to send a private report.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE) that covers this project.
