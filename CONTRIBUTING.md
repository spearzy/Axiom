# Contributing

Thanks for your interest in contributing to Axiom.

## Development Setup

```bash
dotnet restore Axiom.sln
dotnet build Axiom.sln -c Debug
dotnet test Axiom.sln -c Debug
```

## Pull Requests

- Keep changes focused and small.
- Include tests for behaviour changes.
- Update docs when public behaviour changes.
- Do not push directly to `main`; create a branch and open a pull request.
- Branch naming convention: `codex/<short-topic>` for agent-created branches.

## Code Style

- Follow `.editorconfig` and analyser guidance.
- Keep failure output deterministic and testable.
