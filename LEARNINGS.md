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

---

## Chore — .NET 10 Migration

> `TargetFramework` set in both `Directory.Build.props` and individual `.csproj` files means the project file wins — both must be updated.
> The SDK version and the target framework are independent: you can run a .NET 10 SDK and still target `net9.0`, or target `net10.0` as we did.
> `NETSDK1057` is an informational message about preview SDKs, not an error — it disappears when .NET 10 ships stable.

**Technical Topics**
- `TargetFramework` TFM — `net9.0` → `net10.0`
- MSBuild property evaluation order — project file overrides `Directory.Build.props`
- SDK version vs TFM — independent concerns
- `NETSDK1057` — preview SDK informational message

---

## Knowledge Consolidation — Quiz Session

> `ConfigureAwait(false)` prevents deadlocks in library code by not resuming on the captured synchronisation context; console apps have no sync context so the risk is lower, but the habit matters.
> `new HttpClient()` in a loop causes socket exhaustion — sockets enter TIME_WAIT for up to 240 seconds; `IHttpClientFactory` solves this by pooling `HttpMessageHandler` instances.
> Structured `ToolError` responses keep the LLM in the conversation loop — it can read error codes and self-correct; an unhandled exception destroys the information needed to recover.

**Technical Topics**
- `ConfigureAwait(false)` — synchronisation context, deadlock prevention
- Synchronisation context — UI thread, ASP.NET request thread, console (none)
- Socket exhaustion — `HttpClient` TIME_WAIT, port pool depletion
- `IHttpClientFactory` — `HttpMessageHandler` pooling, DNS TTL respect
- `AddHttpClient<T>()` — typed client registration in DI
- Fake `HttpMessageHandler` — unit test isolation for HTTP
- `HttpRequestError` enum (.NET 8+) — `NameResolutionError`, `SecureConnectionError`
- Builder pattern — `IMcpServerBuilder`, fluent chaining, Open/Closed Principle
- MCP `initialize` handshake — bidirectional, version negotiation, capability exchange
- `initialized` notification — handshake completion signal
- `tools/list` — separate request, happens after handshake
- JSON-RPC 2.0 — wire format underlying MCP
- Streamable HTTP transport — HTTP-based MCP transport, SSE successor
- Dependency direction — Contracts has no deps; arrow flows inward toward stability
- Structured errors vs exceptions — LLM self-correction via `ToolError`
- CA1716 — reserved keyword conflict across languages
- CA1000 — static members on generic types, non-generic companion class fix
- CA1056 — `string` URL properties should be `System.Uri`

---

## Step 3 (Part 1) — Tool Contracts Layer

> The contracts library is the most stable layer in the system — it has zero dependencies and everything else depends on it.
> `IToolResult<out T>` uses covariance so `ToolResult<HealthCheckResponse>` satisfies `IToolResult<object>` without a cast — `T` only appears as output.
> Factory methods `ToolResult.Ok<T>()` and `ToolResult.Fail<T>()` live on a non-generic companion class so the compiler can infer `T` — callers never specify it explicitly.

**Technical Topics**
- `IToolResult<out T>` — covariant generic interface
- `out T` covariance — T only as return type, never parameter
- `ToolResult<T>` / `ToolResult` — generic + non-generic companion class pattern
- `ToolErrorCode` enum — vocabulary of failure (finite, matchable, intentional)
- `ToolError` — structured error with `FieldErrors` for LLM self-correction
- `sealed` — data carrier classes that aren't meant to be subclassed
- `init` — immutable-after-construction properties
- `Uri?` vs `string` — CA1056, type-safe URL representation
- `[]` — collection expression syntax (net9+/net10)
- Static factory methods — `Ok` / `Fail` enforce valid object state at construction
- `DateTime.UtcNow` on every result — telemetry-ready from day one

---

## Step 3 (Part 2) — HealthCheckTool Implementation

> `Stopwatch` uses the CPU's monotonic performance counter — it never jumps, never goes backward, and is never adjusted by NTP; `DateTime` subtraction can produce negative or wildly wrong durations mid-flight.
> `CancellationTokenSource.CreateLinkedTokenSource` wires the caller's token and the tool's timeout together — either fires the cancellation; the `when (!cancellationToken.IsCancellationRequested)` guard distinguishes timeout from clean shutdown.
> `[McpServerTool]` + `[Description]` on method and parameters is the schema the LLM reads — every word is AI-facing documentation, not developer-facing.

**Technical Topics**
- `[McpServerToolType]` / `[McpServerTool]` — MCP SDK tool registration via attributes
- `[Description]` on method and parameters — LLM-facing schema documentation
- `System.ComponentModel.DescriptionAttribute` — separate from `McpServerTool`, placed on method
- `IHttpClientFactory` — injected instead of `HttpClient`, enables named clients
- Named HTTP clients — `AddHttpClient("name")` + `ConfigurePrimaryHttpMessageHandler`
- `HttpClientHandler.AllowAutoRedirect` — per-client redirect behaviour
- `Stopwatch` vs `DateTime` — monotonic counter vs wall clock, NTP jump risk
- `CancellationTokenSource.CreateLinkedTokenSource` — composing caller + timeout cancellation
- `cts.CancelAfter` — timeout without blocking a thread
- `catch when (!cancellationToken.IsCancellationRequested)` — routing timeout vs shutdown
- `HttpRequestError` enum (.NET 8+) — `NameResolutionError`, `SecureConnectionError`
- CA1054 — `string` URL parameters should be `Uri`
- CA2234 — prefer `HttpClient.GetAsync(Uri)` over `GetAsync(string)`
- `#pragma warning disable CA1031` — deliberate broad catch, suppressed locally not globally
- `is >= 200 and < 300` — C# 9 relational pattern matching
- `.WithTools<T>()` — registering a tool type with the MCP server builder
