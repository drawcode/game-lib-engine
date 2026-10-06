// Bitty view-parser golden vectors (C# runner).
//
// Run (no Unity needed; from anywhere):
//   dotnet run --project <engine>/Engine/UI/Bitty/Core/vectors~ -- <bitty-base>/vectors/ui.bitty
//   ... -- <dir> --write-expected     only for a NEW case (cases that already have expected.json are kept)
//
// Layout (same as bitty-base vectors): <dir>/<case>/input.json  = the view JSON fed to BittyParser.Parse
//                                      <dir>/<case>/expected.json = { "errors": [...], "tree": [lines] | null }
// `tree` is the iter6 dump (contexts/gamedev/agnostic/scripts/bitty_dump_eval.cs): one line per node, every public
// BittyNode field in declaration order, children indented one space per depth. `errors` is every BittyParser.logError
// message. tree null = Parse returned null.
// Prints PASS/FAIL per case and "N passed, M failed"; exit code 1 on any failure.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Engine.UI.Bitty;

public static class Program {

    private static readonly JsonSerializerOptions Opts = new JsonSerializerOptions(JsonSerializerOptions.Default) {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static int Main(string[] args) {

        string dir = null;
        bool write = false;

        for (int i = 0; i < args.Length; i++) {
            if (args[i] == "--write-expected") { write = true; }
            else { dir = args[i]; }
        }

        if (dir == null || !Directory.Exists(dir)) {
            Console.Error.WriteLine("usage: bitty-vectors <vectorsDir> [--write-expected]");
            return 2;
        }

        string[] cases = Directory.GetDirectories(dir);
        Array.Sort(cases, StringComparer.Ordinal);

        int pass = 0, fail = 0;

        foreach (string caseDir in cases) {

            string id = Path.GetFileName(caseDir);
            string inputPath = Path.Combine(caseDir, "input.json");
            string expectedPath = Path.Combine(caseDir, "expected.json");

            if (!File.Exists(inputPath)) { continue; }

            JsonObject actual = Run(File.ReadAllText(inputPath));
            string actualText = actual.ToJsonString(Opts) + "\n";

            if (!File.Exists(expectedPath)) {
                if (write) {
                    File.WriteAllText(expectedPath, actualText);
                    Console.WriteLine("WROTE " + id);
                }
                else {
                    Console.WriteLine("FAIL " + id + " (no expected.json)");
                    fail++;
                }
                continue;
            }

            // Compare normalized (re-serialized) so whitespace in a hand-written expected does not matter.
            string expectedText = JsonNode.Parse(File.ReadAllText(expectedPath)).ToJsonString(Opts) + "\n";

            if (expectedText == actualText) {
                Console.WriteLine("PASS " + id);
                pass++;
            }
            else {
                Console.WriteLine("FAIL " + id + ": " + FirstDiff(expectedText, actualText));
                fail++;
            }
        }

        Console.WriteLine(pass + " passed, " + fail + " failed");
        return fail == 0 ? 0 : 1;
    }

    private static string FirstDiff(string exp, string act) {
        string[] e = exp.Split('\n');
        string[] a = act.Split('\n');
        for (int i = 0; i < Math.Max(e.Length, a.Length); i++) {
            string el = i < e.Length ? e[i] : "<end>";
            string al = i < a.Length ? a[i] : "<end>";
            if (el != al) { return "line " + (i + 1) + "\n  expected: " + el.Trim() + "\n  actual:   " + al.Trim(); }
        }
        return "(no diff?)";
    }

    private const string JsonFailed = "BittyParser: JSON parse failed: ";

    private static JsonObject Run(string json) {

        List<string> errors = new List<string>();
        // The exception text after "JSON parse failed:" is runtime-specific (.NET vs Mono vs a C++ parser), so
        // only the stable prefix is part of the vector.
        BittyParser.logError = m => errors.Add(m.StartsWith(JsonFailed, StringComparison.Ordinal) ? JsonFailed.TrimEnd() : m);

        BittyNode node = BittyParser.Parse(json);
        BittyParser.logError = null;

        JsonObject o = new JsonObject();
        JsonArray errs = new JsonArray();
        foreach (string s in errors) { errs.Add(s); }
        o["errors"] = errs;

        if (node == null) {
            o["tree"] = null;
        }
        else {
            JsonArray lines = new JsonArray();
            Walk(node, 0, lines);
            o["tree"] = lines;
        }

        return o;
    }

    // Mirrors bitty_dump_eval.cs line-for-line (field order = reflection order, minus children).
    private static void Walk(BittyNode n, int depth, JsonArray lines) {
        StringBuilder b = new StringBuilder();
        b.Append(' ', depth);
        foreach (System.Reflection.FieldInfo f in typeof(BittyNode).GetFields()) {
            if (f.Name == "children") { continue; }
            b.Append(f.Name).Append('=').Append(Fmt(f.GetValue(n))).Append(' ');
        }
        lines.Add(b.ToString());
        foreach (BittyNode c in n.children) { Walk(c, depth + 1, lines); }
    }

    private static string Fmt(object o) {
        if (o == null) { return "~"; }
        string s = o as string;
        if (s != null) { return "'" + s + "'"; }
        List<string> l = o as List<string>;
        if (l != null) { return "[" + string.Join("|", l) + "]"; }
        Dictionary<string, string> d = o as Dictionary<string, string>;
        if (d != null) {
            List<string> ks = new List<string>();
            foreach (KeyValuePair<string, string> kv in d) { ks.Add(kv.Key + "=" + kv.Value); }
            ks.Sort(StringComparer.Ordinal);
            return "{" + string.Join(",", ks) + "}";
        }
        if (o is double || o is float) { return Convert.ToDouble(o).ToString("R", CultureInfo.InvariantCulture); }
        return o.GetType().Name + ":" + o;
    }
}
