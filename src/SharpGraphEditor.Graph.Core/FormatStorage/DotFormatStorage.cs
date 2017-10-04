using System;
using System.Diagnostics;
using System.IO;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class DotFormatStorage : BaseFormatStorage
    {
        public string DotExternalFile => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "GraphViz", "dot.exe");

        public override void Open(TextReader reader, IGraph graph)
        {
            CheckGraphViz();

            base.Open(reader, graph);

            var plainExtFormat = new GraphVizPlainExtFormatStorage();

            var plainExtText = ExecuteGraphViz();
            using (var stream = new StringReader(plainExtText))
            {
                plainExtFormat.Open(stream, graph);
            }
        }

        private void CheckGraphViz()
        {
            if (!File.Exists(DotExternalFile))
            {
                throw new FileNotFoundException($"cant find GraphViz: \"{DotExternalFile}\"");
            }
        }

        private string ExecuteGraphViz()
        {
            using (var graphVizProc = new Process())
            {
                graphVizProc.StartInfo.CreateNoWindow = true;
                graphVizProc.StartInfo.RedirectStandardOutput = true;
                graphVizProc.StartInfo.RedirectStandardError = true;
                graphVizProc.StartInfo.UseShellExecute = false;
                graphVizProc.StartInfo.FileName = DotExternalFile;
                graphVizProc.StartInfo.Arguments = $"\"{CurrentFile}\" -Tplain-ext ";

                graphVizProc.Start();
                var result = graphVizProc.StandardOutput.ReadToEnd();
                var errorMessage = graphVizProc.StandardError.ReadToEnd();
                graphVizProc.WaitForExit();

                if (String.IsNullOrWhiteSpace(errorMessage))
                {
                    return result;
                }
                throw new InvalidOperationException($"GraphViz return error: \"{errorMessage}\"");
            }
        }

        public override void Save(TextWriter writer, IGraph graph)
        {
            throw new NotSupportedException("this format suports only opening");
        }
    }
}
