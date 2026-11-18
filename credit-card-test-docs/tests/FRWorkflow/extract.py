"""Compile the checked-out method text; fail if its source boundaries change."""
import re
import sys
from pathlib import Path

source_root, output = map(Path, sys.argv[1:])


def read(filename):
    return (source_root / filename).read_text(encoding="utf-8-sig")


def method(source, signature):
    start = f"        {signature}\n        {{\n"
    if source.count(start) != 1:
        raise ValueError(f"Expected exactly one method boundary: {signature}")
    offset = source.index(start)
    end = source.index("\n        }\n", offset + len(start))
    return source[offset:end + len("\n        }")]


def constant(source, name):
    matches = re.findall(rf"^        private const \w+ {name} = [^\n]+", source, re.M)
    if len(matches) != 1:
        raise ValueError(f"Expected exactly one constant: {name}")
    return matches[0]


settle = read("Form513B.cs")
cancel = read("Form513D.cs")
consts = read("Common/Consts.cs")
enums = []
for name in ("FRProcessResult", "FRErrorType"):
    start = consts.index(f"        public enum {name}\n")
    end = consts.index("\n        }\n", start)
    enums.append(consts[start:end + len("\n        }")])

settle_members = [
    *(constant(settle, name) for name in
      ("SETTLEMENT_STATUS_COLUMN", "LABEL_FORMAT_SETTLE", "LABEL_FORMAT_CHECK")),
    method(settle, "private async Task SettleExec(IProgress<ProgressState> p, ProgressState progressState)"),
    method(settle, "private async Task ConsistencyExec(IProgress<ProgressState> p, ProgressState progressState)"),
]
cancel_method = method(cancel, "private async Task<ValueTuple<bool, string>> CancelExec()")
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(
    "using System.Data;\nusing System.Drawing;\n"
    + "partial class Form513B\n{\n" + "\n\n".join(settle_members) + "\n}\n"
    + "partial class Form513D\n{\n" + cancel_method + "\n}\n"
    + "static class Consts\n{\n" + "\n\n".join(enums) + "\n}\n",
    encoding="utf-8",
)
