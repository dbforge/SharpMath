#:property GenerateDocumentationFile=true
#:property NoWarn=$(NoWarn);CS1591

// Fails if the line coverage of an assembly in a Cobertura report is below a threshold.
// Usage: dotnet run eng/CheckCoverage.cs -- <threshold-percent> <report.xml>=<AssemblyName> [...]

using System.Globalization;
using System.Xml.Linq;

if (args.Length < 2 || !double.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var threshold))
{
    Console.Error.WriteLine("Usage: CheckCoverage <threshold-percent> <report.xml>=<AssemblyName> [...]");
    return 2;
}

var failed = false;
foreach (var argument in args[1..])
{
    var separator = argument.LastIndexOf('=');
    if (separator <= 0 || separator == argument.Length - 1)
    {
        Console.Error.WriteLine($"Expected <report.xml>=<AssemblyName>, got '{argument}'.");
        failed = true;
        continue;
    }

    var (path, assembly) = (argument[..separator], argument[(separator + 1)..]);
    var package = XDocument.Load(path).Descendants("package")
        .FirstOrDefault(element => (string?) element.Attribute("name") == assembly);
    if (package is null)
    {
        Console.Error.WriteLine($"{assembly}: not found in {path}");
        failed = true;
        continue;
    }

    var coverage = 100 * double.Parse((string) package.Attribute("line-rate")!, CultureInfo.InvariantCulture);
    var passed = coverage >= threshold;
    failed |= !passed;
    Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
        $"{assembly}: {coverage:F1} % line coverage (threshold {threshold:0.##} %) {(passed ? "OK" : "FAILED")}"));
}

return failed ? 1 : 0;
