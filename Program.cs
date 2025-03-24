using System.Diagnostics;
using ConvertMarkdownToHTML.html.converters;

namespace ConvertMarkdownToHTML
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run(typeof(Program).Assembly);

            string inputFilePath = Path.GetFullPath("../../../resources/ExampleInput.md");
            string outputFilePath;

            Stopwatch stopwatch = Stopwatch.StartNew();
            MarkdownToHtml(inputFilePath, out outputFilePath);
            stopwatch.Stop();
            Console.WriteLine(outputFilePath);
            Console.WriteLine(string.Format("Time: {0:00}:{1:00}:{2:00}:{3:00}:", stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds, stopwatch.ElapsedMilliseconds / 10));
        }

        public static void MarkdownToHtml(string inputMarkdownFilePath, out string outputMarkdownFilePath)
        {
            HTMLConverter converter = new HTMLConverter();
            string[] loadedLines = File.ReadAllLines(inputMarkdownFilePath);
            outputMarkdownFilePath = converter.Convert(loadedLines);
        }
    }
}
