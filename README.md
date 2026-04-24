# AgentToolbox.DotNet

> A .NET MCP platform that gives AI agents real diagnostic tools for API health monitoring.

![Status](https://img.shields.io/badge/status-under%20construction-yellow)

## Solution Structure

```
/src
  AgentToolbox.McpServer        → ASP.NET Core host for the MCP server
  AgentToolbox.Tools            → All tool implementations
  AgentToolbox.Tools.Contracts  → Shared DTOs, interfaces, enums
  AgentToolbox.Client           → Console app: MCP client + LLM orchestration
/tests
  AgentToolbox.Tools.Tests      → Unit tests
  AgentToolbox.Integration.Tests → End-to-end MCP protocol tests
/docs
  /architecture                 → ADRs
  /demo                         → Demo scripts and recordings
```
