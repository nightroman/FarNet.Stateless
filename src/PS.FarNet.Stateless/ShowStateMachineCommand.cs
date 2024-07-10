using FarNet.Stateless;
using Stateless.Graph;
using System.IO;
using System.Management.Automation;

namespace PS.FarNet.Stateless;

[Cmdlet("Show", "StateMachine")]
public sealed class ShowStateMachineCommand : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true)]
    public object Machine { get; set; }

    protected override void BeginProcessing()
    {
        var helper = new MetaMachine(Machine);
        var info = helper.GetInfo();
        var dot = UmlDotGraph.Format(info);

        dot = dot.Replace("lambda_method", "fn");

        var script1 = ScriptBlock.Create("Get-Command viz-standalone.js -CommandType Application -ErrorAction Ignore");
        var res1 = script1.InvokeReturnAsIs().ToBaseObject() as CommandInfo;
        var url = res1 is null
            ? "https://github.com/mdaines/viz-js/releases/download/release-viz-3.7.0/viz-standalone.js"
            : "file:///" + res1.Source.Replace('\\', '/');

        var writer = new StringWriter();
        writer.WriteLine("""
            <!DOCTYPE html>
            <html lang="en">
            <head>
            <meta charset="utf-8">
            <title>Stateless</title>
            </head>
            <body>
            """);
        writer.WriteLine($"<script src=\"{url}\"></script>");
        writer.WriteLine("<script>");
        writer.Write("Viz.instance().then(function(viz) {document.body.appendChild(viz.renderSVGElement(\"");
        writer.Write(dot.Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\\n"));
        writer.WriteLine("\"))})");
        writer.WriteLine("""
            </script>
            </body>
            </html>
            """);

        var temp = Path.GetTempPath() + "Stateless.html";
        File.WriteAllText(temp, writer.ToString());

        var script2 = ScriptBlock.Create("Invoke-Item -LiteralPath $args[0]");
        script2.Invoke(temp);
    }
}
