using FarNet.Stateless;
using Stateless.Graph;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace PS.FarNet.Stateless;

[Cmdlet("Show", "StateMachine")]
public sealed class ShowStateMachineCommand : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true)]
    public object Machine { get; set; }

    [Parameter]
    public string Output { get; set; }

    private static readonly Regex LambdaMethod = new("\\blambda_method\\d*");

    private static readonly ScriptBlock GetViz = ScriptBlock.Create("Get-Command viz-standalone.js -CommandType Application -ErrorAction Ignore");
    private static readonly ScriptBlock InvokeItem = ScriptBlock.Create("Invoke-Item -LiteralPath $args[0]");

    protected override void BeginProcessing()
    {
        var helper = new MetaMachine(Machine.ToBaseObject());
        var info = helper.GetInfo();

        var dot = UmlDotGraph.Format(info);
        dot = LambdaMethod.Replace(dot, "Function");

        if (Output != null)
        {
            Output = GetUnresolvedProviderPathFromPSPath(Output);
            File.WriteAllText(Output, dot);
            return;
        }

        var url = GetViz.InvokeReturnAsIs().ToBaseObject() is CommandInfo command
            ? "file:///" + command.Source.Replace('\\', '/')
            : "https://github.com/mdaines/viz-js/releases/download/release-viz-3.7.0/viz-standalone.js";

        var sb = new StringBuilder();
        sb.AppendLine("""
            <!DOCTYPE html>
            <html lang="en">
            <head>
            <meta charset="utf-8">
            <title>Stateless</title>
            </head>
            <body>
            """);
        sb.AppendLine($"<script src=\"{url}\"></script>");
        sb.AppendLine("<script>");
        sb.Append("Viz.instance().then(function(viz) {document.body.appendChild(viz.renderSVGElement(\"");
        sb.Append(dot.Replace("\r", "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\\n"));
        sb.AppendLine("\"))})");
        sb.AppendLine("""
            </script>
            </body>
            </html>
            """);

        var temp = Path.GetTempPath() + "Stateless.html";
        File.WriteAllText(temp, sb.ToString());

        InvokeItem.Invoke(temp);
    }
}
