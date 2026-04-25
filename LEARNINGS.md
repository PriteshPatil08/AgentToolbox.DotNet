# LEARNINGS

---

## Step 1 — Solution Structure & Repository Setup

> A .NET solution is a container, not code — it's a manifest of projects and their relationships.
> `Directory.Build.props` is inherited by every project in the repo, making it the single source of truth for build policy.
> `TreatWarningsAsErrors` turns the compiler into a quality gate, not a suggestion box.

**Technical Topics**
- `dotnet new sln` / `dotnet sln add` — solution and project registration
- MSBuild `Directory.Build.props` — solution-wide property inheritance
- `.editorconfig` — cross-editor formatting contract
- `dotnet new gitignore` — standard .NET ignore rules
- Roslyn Analysers (`AnalysisMode=All`, `NoWarn`) — static analysis configuration
- xUnit — .NET test framework
- `TreatWarningsAsErrors` — compiler as quality gate

---

## Step 2 — Minimal MCP Server (Empty Shell)

> `Microsoft.NET.Sdk.Web` drags in Kestrel — for stdio transport, swap it for `Microsoft.NET.Sdk` and carry only what you need.
> Console logging writes to stdout by default; with stdio transport, that corrupts the JSON-RPC stream — redirect it to stderr with `LogToStandardErrorThreshold`.
> `Host.CreateApplicationBuilder` is the right base for a protocol server: DI + config + lifetime, without any HTTP listener.

**Technical Topics**
- `ModelContextProtocol` NuGet SDK — MCP server registration and stdio transport
- `Microsoft.NET.Sdk` vs `Microsoft.NET.Sdk.Web` — SDK selection and its implicit package implications
- `Host.CreateApplicationBuilder` — generic host vs web host
- `AddMcpServer().WithStdioServerTransport()` — MCP stdio protocol wiring
- `McpServerOptions` / `Implementation` — server identity in the MCP initialize handshake
- `LogToStandardErrorThreshold` — redirecting console logs to stderr to protect stdout
- CA2007 (`ConfigureAwait`) — Roslyn analyser enforcing async best practices
- `IOptions<T>` / `Configure<T>` — options pattern for configuration binding
