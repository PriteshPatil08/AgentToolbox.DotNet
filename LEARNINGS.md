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
