# Sentinel.MCP — Quiz Bank

---

## Quiz 1 — MCP Server Setup & .NET Fundamentals

**Q1. What does `ConfigureAwait(false)` do and why do we use it?**
> It tells the awaiter not to resume on the original synchronization context after the await completes.
> In UI or ASP.NET Classic apps, the sync context is the UI thread — resuming on it unnecessarily causes deadlocks.
> In a console/generic host (like our MCP server), there is no sync context, but it's still correct practice and avoids overhead.

---

**Q2. What is the difference between `Host.CreateApplicationBuilder` and `WebApplication.CreateBuilder`?**
> `WebApplication.CreateBuilder` sets up a full web stack — Kestrel HTTP server, middleware pipeline, routing, and HTTP-specific services.
> `Host.CreateApplicationBuilder` sets up only the generic hosting infrastructure — DI, configuration, logging, and background services.
> We use the generic host because our MCP server communicates over stdio, not HTTP, so spinning up Kestrel would be wasteful and wrong.

---

**Q3. Why do we redirect all logs to stderr in an MCP stdio server?**
> The MCP protocol uses stdout as the communication channel — JSON-RPC messages flow in and out of it.
> If log lines leak into stdout, the client receives garbled JSON and the protocol breaks.
> Redirecting everything to stderr via `LogToStandardErrorThreshold = LogLevel.Trace` keeps stdout clean for the protocol.

---

**Q4. Why register tools with `AddMcpServer().WithTools<T>()` instead of instantiating them manually?**
> The MCP SDK uses reflection to discover tool methods, read their `[Description]` attributes, and build the `tools/list` schema.
> Registering through the SDK wires up dependency injection — so your tool can receive `IHttpClientFactory` and other services through the constructor.
> If you new them up manually, you bypass DI and the SDK never knows the tool exists.

---

**Q5. Why does the Contracts project exist as a separate project?**
> Contracts hold shared data shapes — request/response DTOs and interfaces — that both the Tools project and any client need to reference.
> If they lived inside Tools, a client project would have to take a dependency on tool implementation code just to understand the response shape.
> Separation of Contracts from implementation is how you avoid circular dependencies and keep the surface area each project exposes minimal.

---

**Q6. Why do MCP tools return `ToolResult` instead of throwing exceptions?**
> The MCP protocol is JSON-RPC — exceptions don't cross process or network boundaries in a meaningful way.
> An unhandled exception from a tool gets swallowed or turned into a generic error message; the AI model can't reason about it or suggest a fix.
> Returning a structured `ToolError` with an `ErrorCode` gives the model something actionable — it can tell the user "DNS failed" vs "SSL error" vs "timed out."

---

## Quiz 2 — HealthCheckTool Internals

**Q7. Why use `Stopwatch` instead of subtracting two `DateTime.UtcNow` values for latency?**
> `DateTime.UtcNow` is tied to the system clock, which can jump forward or backward due to NTP synchronisation.
> If NTP adjusts the clock between your two reads, the subtraction gives a nonsensical or even negative duration.
> `Stopwatch` uses a monotonic hardware counter that only ever increases, making it the correct tool for elapsed time measurement.

---

**Q8. What does `out T` covariance mean on `IToolResult<out T>`?**
> The `out` keyword means `T` can only appear in output positions — return types and readable properties — never as a method parameter.
> This allows `IToolResult<HealthCheckResponse>` to be treated as `IToolResult<object>` without a cast, because the narrower type can always satisfy the broader contract.
> In practice it means you can hold any tool result in a variable typed as `IToolResult<object>` without losing type safety.

---

**Q9. What does `CancellationTokenSource.CreateLinkedTokenSource` do?**
> It creates a new `CancellationTokenSource` that cancels when *either* of the two source tokens is cancelled.
> This lets us combine the caller's cancellation token (user abort) with our own timeout token (CancelAfter) without replacing either.
> Neither the caller nor our timeout "owns" the new token — the linked source manages that, which is why creation responsibility is outsourced.

---

**Q10. Why use `IHttpClientFactory` instead of `new HttpClient()`?**
> `HttpClient` holds a `HttpMessageHandler` that caches DNS resolutions and keeps sockets open; creating a new one per request exhausts OS socket handles (socket exhaustion).
> `IHttpClientFactory` manages a pool of handlers with controlled lifetimes — it recycles them before their DNS cache goes stale.
> It also enables named clients with pre-configured settings (like `AllowAutoRedirect = false`) registered once at startup.

---

**Q11. Why can't we change `HttpClient.DefaultRequestHeaders` or `AllowAutoRedirect` per request?**
> `HttpClient` is designed to be reused across many requests — its handler and default settings are shared state.
> Mutating them mid-flight would affect concurrent requests running on the same instance, causing race conditions.
> The correct pattern is to bake the difference into separate named clients at startup — one with `AllowAutoRedirect = true`, one with `false`.

---

**Q12. Why does HTTP/2 have no reason phrase (e.g. "OK", "Not Found")?**
> HTTP/1.1 sent status lines as text: `HTTP/1.1 200 OK` — the reason phrase was part of the wire format.
> HTTP/2 is a binary protocol; headers are compressed with HPACK and there is no status line, only a `:status` pseudo-header with just the numeric code.
> Sending human-readable reason phrases over HTTP/2 would waste bandwidth and defeat compression — so they were dropped entirely.

---

**Q13. What does the `out` keyword require of `T` in `IToolResult<out T>`, and what would break if you removed it?**
> With `out T`, the compiler enforces that `T` appears only in return/output positions — you cannot write a method that *accepts* a `T` parameter.
> This guarantees that reading a `T` from a covariant interface is always safe — a `HealthCheckResponse` is always an `object`, so upcasting is sound.
> Without `out`, the interface is invariant — `IToolResult<HealthCheckResponse>` and `IToolResult<object>` are completely unrelated types.

---

## Quiz 3 — HealthCheckTool Completion

**Q14. What is CA1031 and why do we suppress it with `#pragma` instead of removing the catch?**
> CA1031 warns that catching `Exception` (the base type) is too broad — you might silently swallow bugs that should crash the program.
> We need the broad catch as a last resort because we have already handled all known specific exceptions above it; anything left is truly unknown.
> We suppress it with `#pragma warning disable/restore CA1031` scoped tightly to just that catch block, not the whole file, to keep the intent clear.

---

**Q15. What is the `Server` response header and why did we end up removing it from `HealthCheckResponse`?**
> The `Server` header is sent by the web server to identify itself — e.g. `nginx/1.25.3` or `Microsoft-IIS/10.0`.
> It became redundant in our response because the status code already carries the health signal the tool is meant to provide.
> Many production servers also suppress or spoof this header for security reasons, making it unreliable data.

---

**Q16. Why do we expose `IReadOnlyList<string>` instead of `List<string>` on a public response DTO property?**
> CA1002 flags `List<T>` in public APIs because it exposes mutation methods (`Add`, `Remove`, `Clear`) that callers should not be using on a response.
> `IReadOnlyList<string>` communicates intent clearly: this is data you read, not a collection you modify.
> A `List<string>` internally still satisfies `IReadOnlyList<string>` at assignment, so no extra wrapping is needed.

---

**Q17. What is CA1000 and how did we fix it?**
> CA1000 warns against static members on generic types — calling `ToolResult<T>.Ok(...)` requires you to specify `T` explicitly at the call site.
> The fix is to move the static factory methods to a non-generic companion class `ToolResult` (no type parameter), so the compiler can infer `T` from the argument.
> `ToolResult.Ok(data, ms)` lets the compiler see the type of `data` and infer `T` automatically — no `<HealthCheckResponse>` annotation needed.

---

**Q18. Why is `ExecutedAtUtc = DateTime.UtcNow` inside `ToolResult.Ok()` a subtle timing bug?**
> `ToolResult.Ok()` is called *after* the operation completes and the stopwatch is stopped — so `DateTime.UtcNow` captures the end time, not the start time.
> `ExecutedAtUtc` implies "when was this tool executed," which should be the moment it *started*, not the moment the result was assembled.
> The correct fix is to capture `DateTime.UtcNow` at the top of the method alongside `Stopwatch.StartNew()` and pass it through to `ToolResult.Ok()`.

---

**Q19. Why does `Dictionary<string, string>` work for response headers even though headers can have multiple values?**
> HTTP allows multiple values for the same header name — e.g. multiple `Set-Cookie` headers.
> We use `string.Join(", ", values)` to collapse multiple values into one comma-separated string per key before putting them in the dictionary.
> This is a deliberate lossy simplification for readability — if we needed to round-trip headers exactly, we'd use `Dictionary<string, List<string>>`.

---

**Q20. What is `IReadOnlyList<string>` vs `IList<string>` vs `List<string>` — and when do you pick each?**
> `List<string>` is the concrete type — pick it internally when you need to build and mutate a collection.
> `IList<string>` is the mutable interface — expose it when callers are allowed to add or remove items.
> `IReadOnlyList<string>` is the read-only interface — expose it on DTOs and responses where callers should only read, preserving the flexibility to change the backing type later.
