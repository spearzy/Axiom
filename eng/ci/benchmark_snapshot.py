#!/usr/bin/env python3
from __future__ import annotations

import csv
import json
import sys
from dataclasses import dataclass
from datetime import date
from decimal import Decimal, ROUND_HALF_UP
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parents[2]
RESULTS_DIR = REPO_ROOT / "BenchmarkDotNet.Artifacts" / "results"
SNAPSHOT_PATH = REPO_ROOT / "docs" / "benchmarks.snapshot.json"
DOC_PATH = REPO_ROOT / "docs" / "benchmarks.md"


@dataclass(frozen=True)
class ScenarioSpec:
    csv_name: str
    method_name: str
    scenario_label: str
    what_it_shows: str


SCENARIOS = [
    ScenarioSpec(
        csv_name="Axiom.Benchmarks.Scenarios.ScalarAssertionBenchmarks-report.csv",
        method_name="Be pass (int)",
        scenario_label="Simple `Be(...)` pass",
        what_it_shows="The baseline pass path stays cheap.",
    ),
    ScenarioSpec(
        csv_name="Axiom.Benchmarks.Scenarios.ScalarAssertionBenchmarks-report.csv",
        method_name="Contain pass (small int array)",
        scenario_label="Collection `Contain(...)` pass",
        what_it_shows="Common collection membership checks stay cheap on small inputs.",
    ),
    ScenarioSpec(
        csv_name="Axiom.Benchmarks.Scenarios.EquivalencyBenchmarks-report.csv",
        method_name="BeEquivalentTo pass (medium graph)",
        scenario_label="`BeEquivalentTo(...)` pass",
        what_it_shows="Structural comparison does more work, but only when you use it.",
    ),
    ScenarioSpec(
        csv_name="Axiom.Benchmarks.Scenarios.ScalarAssertionBenchmarks-report.csv",
        method_name="ContainAllAsync pass (small stream)",
        scenario_label="Async `ContainAllAsync(...)` pass",
        what_it_shows="Async stream assertions pay to enumerate the stream, not before.",
    ),
    ScenarioSpec(
        csv_name="Axiom.Benchmarks.Scenarios.ScalarAssertionBenchmarks-report.csv",
        method_name="BeNormalized pass (float[3])",
        scenario_label="Vector `BeNormalized(...)` pass",
        what_it_shows="Vector-specific checks stay focused on the metric they compute.",
    ),
    ScenarioSpec(
        csv_name="Axiom.Benchmarks.Scenarios.ScalarAssertionBenchmarks-report.csv",
        method_name="HaveRecallAt pass (top-3)",
        scenario_label="Ranking `HaveRecallAt(...)` pass",
        what_it_shows="Retrieval metrics add top-k and set work only when you ask for them.",
    ),
]


def quantize_decimal(value: str, places: str) -> str:
    decimal_value = Decimal(value)
    return format(decimal_value.quantize(Decimal(places), rounding=ROUND_HALF_UP), "f")


def format_mean(value: str) -> str:
    amount, unit = value.split(" ", 1)
    decimal_value = Decimal(amount)

    if unit == "ns":
        places = "0.1" if decimal_value >= Decimal("10") else "0.01"
    elif unit == "μs":
        places = "0.1"
    elif unit == "ms":
        places = "0.01"
    else:
        places = "0.01"

    return f"{quantize_decimal(amount, places)} {unit}"


def format_allocated(value: str) -> str:
    amount, unit = value.split(" ", 1)
    if unit == "B":
        return f"{int(Decimal(amount))} B"

    places = "0.1" if Decimal(amount) >= Decimal("10") else "0.01"
    return f"{quantize_decimal(amount, places)} {unit}"

def refresh_snapshot() -> None:
    snapshot = {
        "generated_on": str(date.today()),
        "job": "ShortRun",
        "rows": build_rows(),
    }

    SNAPSHOT_PATH.write_text(json.dumps(snapshot, indent=2) + "\n", encoding="utf-8")
    DOC_PATH.write_text(render_docs(snapshot), encoding="utf-8")


def build_rows() -> list[dict[str, str]]:
    rows = []
    for scenario in SCENARIOS:
        csv_path = RESULTS_DIR / scenario.csv_name
        if not csv_path.exists():
            raise SystemExit(f"Expected benchmark result was not produced: {csv_path}")

        csv_rows = list(csv.DictReader(csv_path.read_text(encoding="utf-8").splitlines()))
        match = next((row for row in csv_rows if row["Method"].strip("'") == scenario.method_name), None)
        if match is None:
            raise SystemExit(f"Benchmark row '{scenario.method_name}' was not found in {csv_path.name}")

        rows.append(
            {
                "scenario": scenario.scenario_label,
                "mean": format_mean(match["Mean"]),
                "allocated": format_allocated(match["Allocated"]),
                "what_it_shows": scenario.what_it_shows,
            }
        )

    return rows


def render_docs(snapshot: dict[str, object]) -> str:
    rows = snapshot["rows"]
    assert isinstance(rows, list)

    table_lines = [
        "| Scenario | Mean | Allocated | What it shows |",
        "| --- | ---: | ---: | --- |",
    ]
    for row in rows:
        assert isinstance(row, dict)
        table_lines.append(
            f"| {row['scenario']} | `{row['mean']}` | `{row['allocated']}` | {row['what_it_shows']} |"
        )

    generated_on = snapshot["generated_on"]
    assert isinstance(generated_on, str)

    body = "\n".join(table_lines)
    return f"""# Benchmarks

Axiom tracks a small set of representative benchmarks. Simple assertions are expected to stay cheap, while more capable features cost more only when you use them. These snapshots exist to catch regressions and show the shape of the library.

Snapshot last refreshed: {generated_on}.

Refreshes are manual and intentional. CI only validates that the committed snapshot and this page stay in sync.

## Representative Benchmarks

{body}

## Methodology

These numbers come from the repo's BenchmarkDotNet suite using the shared `ShortRun` configuration and a Release build. The published values are rounded so tiny machine noise does not churn the docs. Use them to spot regressions and understand Axiom's pay-to-play shape, not to predict full test-suite runtime or compare different machines.
"""


def validate_snapshot() -> None:
    if not SNAPSHOT_PATH.exists():
        raise SystemExit(f"Snapshot file is missing: {SNAPSHOT_PATH}")

    snapshot = json.loads(SNAPSHOT_PATH.read_text(encoding="utf-8"))
    expected_docs = render_docs(snapshot)
    actual_docs = DOC_PATH.read_text(encoding="utf-8")

    if actual_docs != expected_docs:
        raise SystemExit(
            "docs/benchmarks.md is out of sync with docs/benchmarks.snapshot.json. "
            "Run 'bash eng/ci/run-benchmark-snapshot.sh refresh'."
        )


def main() -> None:
    if len(sys.argv) != 2 or sys.argv[1] not in {"refresh", "validate"}:
        raise SystemExit("Usage: python3 eng/ci/benchmark_snapshot.py [refresh|validate]")

    if sys.argv[1] == "refresh":
        refresh_snapshot()
        return

    validate_snapshot()


if __name__ == "__main__":
    main()
